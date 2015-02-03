﻿#region << Usings >>

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Classy.Attributes;
using Classy.Exceptions;
using Classy.Interfaces;

#endregion

namespace Classy
{
    /// <summary>
    /// This class allows mapping from one class to another with the use of MapProperty attributes.  A user can
    /// flatten an object hierarcy and restore it later.  All objects being mapped to MUST have a parameterless
    /// constructor.
    /// </summary>
    public class ClassyMapper : IClassyMapper
    {

        #region << Static Variables >>

        // Cache reflection calls to speed up performance
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyInfoCache =
            new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<PropertyInfo, MapPropertyAttribute> _mapPropertyCache =
            new ConcurrentDictionary<PropertyInfo, MapPropertyAttribute>();
        private static readonly ConcurrentDictionary<Type, MapAllPropertiesAttribute> _mapAllPropertiesCache =
            new ConcurrentDictionary<Type, MapAllPropertiesAttribute>();
        private static readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> _setterMaps =
            new ConcurrentDictionary<PropertyInfo, Action<object, object>>();
        private static readonly ConcurrentDictionary<PropertyInfo, Func<object, object>> _getterMaps =
            new ConcurrentDictionary<PropertyInfo, Func<object, object>>();

        // These types are used often, so perform this once
        private readonly static Type _nullableType = typeof(Nullable<>);
        private readonly static Type _listType = typeof(List<>);
        private readonly static Type _stringType = typeof(string);
        private readonly static Type _mapAllPropertiesAttributeType = typeof(MapAllPropertiesAttribute);
        private readonly static Type _mapPropertyAttributeType = typeof(MapPropertyAttribute);
        private readonly static Type _iEnumerableType = typeof (IEnumerable);

        #endregion

        #region << Static Mapper Methods >>

        /// <summary>
        /// Clear all PropertyInfo, MapPropertyAttribute, and MapAllPropertiesAttribute caching.
        /// </summary>
        public static void ClearCacheObjects()
        {
            _propertyInfoCache.Clear();
            _mapPropertyCache.Clear();
            _mapAllPropertiesCache.Clear();
        }

        /// <summary>
        /// Returns a new instance of ClassyMapper.
        /// </summary>
        /// <param name="config">Configuration data on how the objects will be mapped.</param>
        /// <returns>A new instance of ClassyMapper.</returns>
        public static IClassyMapper New(IClassyMapperConfig config = null)
        {
            return new ClassyMapper(config);
        }

        #endregion

        #region << Static Default Methods >>

        /// <summary>
        /// Copies over all Name/Type matching, settable properties; shallow copy only.
        /// </summary>
        /// <param name="fromObject">The object to copy from.</param>
        /// <param name="toObject">The object to copy to.</param>
        public static void CopyValues<TFrom, TTo>(TFrom fromObject, TTo toObject)
            where TFrom : class
            where TTo : class
        {
            if (fromObject == null || toObject == null)
            {
                return;
            }

            var toProps = GetPropertyInfos(typeof(TTo));
            var fromProps = GetPropertyInfos(typeof(TFrom));

            foreach (var toProp in toProps)
            {
                var fromProp = fromProps.FirstOrDefault(a => a.Name == toProp.Name);
                if (fromProp == null || !toProp.CanWrite || !fromProp.CanRead)
                {
                    continue;
                }

                if (fromProp.PropertyType == toProp.PropertyType ||
                    fromProp.PropertyType.IsAssignableFrom(toProp.PropertyType))
                {
                    toProp.SetValue(toObject, fromProp.GetValue(fromObject, null), null);
                }
            }
        }

        /// <summary>
        /// Defaults all string properties of the class.  Strings are set to string.Empty.
        /// </summary>
        /// <param name="entity">The class to default properties on.</param>
        public static void DefaultStringValues(object entity)
        {
            DefaultStringValues(entity, string.Empty);
        }

        /// <summary>
        /// Defaults all string properties of the entity.
        /// </summary>
        /// <param name="entity">The class to default properties on.</param>
        /// <param name="defaultStringValue">The value to default string values to.</param>
        public static void DefaultStringValues(object entity, string defaultStringValue)
        {
            if (entity == null)
            {
                return;
            }

            PropertyInfo[] props = GetPropertyInfos(entity.GetType());
            foreach (PropertyInfo prop in props)
            {
                DefaultStringValue(entity, prop, defaultStringValue);
            }
        }

        /// <summary>
        /// Defaults all fieldes of the entity if the property is null.  Strings are set to string.Empty.
        /// </summary>
        /// <param name="entity">The class to default properties on.</param>
        public static void DefaultStringValuesIfNull(object entity)
        {
            DefaultStringValuesIfNull(entity, string.Empty);
        }

        /// <summary>
        /// Defaults all fieldes of the entity if the property is null.
        /// </summary>
        /// <param name="entity">The class to default properties on.</param>
        /// <param name="defaultStringValue">The value to default string values to.</param>
        public static void DefaultStringValuesIfNull(object entity, string defaultStringValue)
        {
            if (entity == null)
            {
                return;
            }

            PropertyInfo[] props = GetPropertyInfos(entity.GetType());
            foreach (PropertyInfo prop in props)
            {
                if (!prop.CanWrite)
                {
                    continue;
                }
                var val = prop.GetValue(entity, null);
                bool canDefault = val == null;
                if (canDefault)
                {
                    DefaultStringValue(entity, prop, defaultStringValue);
                }
            }
        }

        /// <summary>
        /// Defaults the property value.
        /// </summary>
        /// <param name="entity">The object holding the data.</param>
        /// <param name="prop">The property to set the value on.</param>
        /// <param name="defaultStringValue">The default value to use for properties of type string.</param>
        private static void DefaultStringValue(object entity, PropertyInfo prop, string defaultStringValue)
        {
            if (prop.PropertyType == _stringType && prop.CanWrite)
            {
                prop.SetValue(entity, defaultStringValue, null);
            }
        }

        #endregion

        #region << Private Static Methods >>

        /// <summary>
        /// Returns the PropertyInfo objects for the given type; Checks a cache, first.
        /// </summary>
        /// <param name="type">The type of class to get the properties for.</param>
        /// <returns>List of PropertyInfo.</returns>
        private static PropertyInfo[] GetPropertyInfos(Type type)
        {
            PropertyInfo[] result;
            if (_propertyInfoCache.TryGetValue(type, out result))
            {
                return result;
            }

            result = type.GetProperties();
            _propertyInfoCache.AddOrUpdate(type, result, (info, orig) => orig);
            return result;
        }

        /// <summary>
        /// Returns the MapAllProperties attribute for the given type, if it exists; Checks a cache, first.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static MapAllPropertiesAttribute GetMapAllPropertiesAttribute(Type type)
        {
            MapAllPropertiesAttribute result;
            if (_mapAllPropertiesCache.TryGetValue(type, out result))
            {
                return result;
            }

            result =
                type.GetCustomAttributes(_mapAllPropertiesAttributeType, false).FirstOrDefault() as
                    MapAllPropertiesAttribute;

            _mapAllPropertiesCache.AddOrUpdate(type, result, (info, orig) => orig);
            return result;
        }

        /// <summary>
        /// Returns the MapProperty attribute for the given property info, if it exists; Checks a cache, first.
        /// </summary>
        /// <param name="pi">The PropertyInfo object that may contain the MapProperty attribute.</param>
        /// <returns>MapPropertyAttribute or null if it does not exist.</returns>
        private static MapPropertyAttribute GetMapPropertyAttribute(PropertyInfo pi)
        {
            MapPropertyAttribute result;
            if (_mapPropertyCache.TryGetValue(pi, out result))
            {
                return result;
            }

            result =
                pi.GetCustomAttributes(_mapPropertyAttributeType, false).FirstOrDefault() as
                    MapPropertyAttribute;

            _mapPropertyCache.AddOrUpdate(pi, result, (info, orig) => orig);
            return result;
        }

        #endregion

        #region << Variables >>

        private readonly IDictionary<Tuple<Type, Type>, Delegate> _customMaps;
        private readonly IDictionary<Type, Delegate> _fromObjects;
        private readonly IDictionary<Tuple<Type, Type>, Delegate> _constructors;
        private readonly IDictionary<object, object> _alreadyMappedEntities;

        #endregion

        #region << Properties >>

        /// <summary>
        /// Gets the configuration data on how the objects will be mapped.
        /// </summary>
        public IClassyMapperConfig Config { get; private set; }

        #endregion

        #region << Constructors >>

        /// <summary>
        /// Constructor that initializes all variables/properties.
        /// </summary>
        /// <param name="config">Configuration data on how the objects will be mapped.</param>
        public ClassyMapper(IClassyMapperConfig config = null)
        {
            Config = config ?? new ClassyMapperConfig();

            _customMaps = new Dictionary<Tuple<Type, Type>, Delegate>();
            _constructors = new Dictionary<Tuple<Type, Type>, Delegate>();
            _fromObjects = new Dictionary<Type, Delegate>();
            _alreadyMappedEntities = new Dictionary<object, object>();
        }

        #endregion

        #region << Public Mapping Methods >>

        /// <summary>
        /// Registers a method to call to do custom mapping when working with a specific TFrom -> TTo combination.
        /// </summary>
        /// <typeparam name="TFrom">The type of object being mapped from.</typeparam>
        /// <typeparam name="TTo">The type of object being mapped to.</typeparam>
        /// <param name="method">The method to invoke.</param>
        /// <returns>Instance of this ClassyMapper for chaining.</returns>
        public IClassyMapper RegisterCustomMap<TFrom, TTo>(Action<TFrom, TTo> method)
        {
            _customMaps.Add(GetCustomMapKey(typeof(TFrom), typeof(TTo)), method);
            return this;
        }

        /// <summary>
        /// Registers a function that returns what objects to map from for a given TFrom object.
        /// This allows a user to flatten a hierarchy.
        /// </summary>
        /// <typeparam name="TFrom">The type of object being mapped from.</typeparam>
        /// <param name="func">The function to invoke when working with a TFrom object.</param>
        /// <returns>Instance of this ClassyMapper for chaining.</returns>
        public IClassyMapper RegisterFromObjects<TFrom>(Func<TFrom, object[]> func)
        {
            _fromObjects.Add(typeof(TFrom), func);
            return this;
        }

        /// <summary>
        /// Registers a method to call when creating a new TTo type object given the TFrom data.
        /// </summary>
        /// <typeparam name="TFrom">The type of object being mapped from.</typeparam>
        /// <typeparam name="TTo">The type of object being created.</typeparam>
        /// <param name="method">The function to invoke when creating a new TTo from a TFrom mapping.</param>
        /// <returns>New instance of TTo.</returns>
        public IClassyMapper RegisterConstructor<TFrom, TTo>(Func<TFrom, TTo> method)
        {
            _constructors.Add(GetCustomMapKey(typeof (TFrom), typeof (TTo)), method);
            return this;
        }

        /// <summary>
        /// Fully maps a list of source object of type TFrom to an IList of TTo type.
        /// </summary>
        /// <typeparam name="TFrom">The type of object being mapped from.</typeparam>
        /// <typeparam name="TTo">The type of object being mapped to.</typeparam>
        /// <param name="fromObjects">The list of objects being mapped from.</param>
        /// <returns>IList of type TTo.</returns>
        public IList<TTo> MapToList<TTo, TFrom>(IEnumerable<TFrom> fromObjects)
        {
            _alreadyMappedEntities.Clear();

            ConcurrentDictionary<int, TTo> result = new ConcurrentDictionary<int, TTo>();
            List<Task> tasks = new List<Task>();
            int i = 0;
            foreach(var from in fromObjects)
            {
                var fromInner = from;
                int currentIndex = i;
                tasks.Add(
                    Task.Factory.StartNew(
                        () =>
                        {
                            var item = (TTo)PerformMap(typeof(TTo), fromInner);
                            result.AddOrUpdate(currentIndex, item, (x, y) => item);
                        }));
                i++;
            }
            Task.WaitAll(tasks.ToArray());
            return result.Select(a => a.Value).ToList();
        }

        /// <summary>
        /// Maps a single source object to an object of type TTo.
        /// </summary>
        /// <typeparam name="TTo">The type of object being mapped to.</typeparam>
        /// <param name="from">The object being mapped from.</param>
        /// <returns></returns>
        public TTo Map<TTo>(object from)
        {
            _alreadyMappedEntities.Clear();
            return (TTo)PerformMap(typeof(TTo), from);
        }

        /// <summary>
        /// Maps a single source object to an object of type TTo that was already created.
        /// </summary>
        /// <typeparam name="TTo">The type of object being mapped to.</typeparam>
        /// <param name="to">The instance of the object being mapped to.</param>
        /// <param name="from">The object being mapped from.</param>
        /// <returns></returns>
        public TTo Map<TTo>(TTo to, object from)
        {
            _alreadyMappedEntities.Clear();
            return (TTo)PerformMap(typeof(TTo), to, from);
        }

        #endregion

        #region << Assign Methods >>

        /// <summary>
        /// Invokes any custom maps that may exist for the given to/from combination.
        /// </summary>
        /// <param name="to">The instance of the object being mapped to.</param>
        /// <param name="from">The object being mapped from.</param>
        private void AssignCustomMap(object to, object from)
        {
            if (to == null || from == null || _customMaps.Keys.Count == 0)
            {
                return;
            }

            Delegate method;
            if (!_customMaps.TryGetValue(GetCustomMapKey(from.GetType(), to.GetType()), out method))
            {
                return;
            }

            if (method != null)
            {
                method.DynamicInvoke(from, to);
            }
        }

        /// <summary>
        /// Maps the property.
        /// </summary>
        /// <param name="mapPropAttr">The MapProperty attribute on the Property, if any.</param>
        /// <param name="toObject">The instance of the object being mapped to.</param>
        /// <param name="toProp">The property being mapped to.</param>
        /// <param name="fromObject">The object being mapped from.</param>
        /// <param name="fromProp">The property being mapped from.</param>
        private void AssignValue(
            MapPropertyAttribute mapPropAttr,
            object toObject,
            PropertyInfo toProp,
            object fromObject,
            PropertyInfo fromProp)
        {
            var value = GetValue(fromObject, fromProp);

            if (AreEqualTypes(toProp, fromProp))
            {
                SetValue(toObject, toProp, value);
            }
            else if (mapPropAttr is MapPropertyTimestamp)
            {
                MapTimestamp(toObject, toProp, value);
            }
            else if (toProp.PropertyType == _stringType)
            {
                // If target property is a string, just call ToString on whatever source property is
                SetValue(toObject, toProp, value == null ? null : value.ToString());
            }
            else if (IsList(toProp))
            {
                MapList(toObject, toProp, value);
            }
            else if (IsAssignableEnumTo(toProp, fromProp))
            {
                if (Enum.IsDefined(toProp.PropertyType, value))
                {
                    SetValue(toObject, toProp, Enum.Parse(toProp.PropertyType, value.ToString(), Config.IgnoreEnumCase));
                }
            }
            else if (IsAssignableEnumFrom(toProp, fromProp))
            {
                SetValue(toObject, toProp, value);
            }
            else if (IsFromNullableToNonNullable(toProp, fromProp) ||
                IsAssignableNullableEnumFrom(toProp, fromProp))
            {
                // Map nullable source to non-nullable target, if not null
                if (value != null)
                {
                    SetValue(toObject, toProp, value);
                }
            }
            else if (IsAssignableNullableEnumTo(toProp, fromProp))
            {
                if (Enum.IsDefined(toProp.PropertyType.GetGenericArguments()[0], value))
                {
                    SetValue(
                        toObject,
                        toProp,
                        Enum.Parse(
                            toProp.PropertyType.GetGenericArguments()[0],
                            value.ToString(),
                            Config.IgnoreEnumCase));
                }
            }
            else if (!toProp.PropertyType.IsPrimitive)
            {
                // Map class/struct
                SetValue(toObject, toProp, PerformMap(toProp.PropertyType, value));
            }
        }

        #endregion

        #region << Map Methods >>

        /// <summary>
        /// Returns a new instance of the given toType.
        /// </summary>
        /// <param name="toType">The type of object to instantiate.</param>
        /// <param name="from">The object being mapped from.</param>
        /// <returns>New instance of the given toType.</returns>
        private object CreateInstance(Type toType, object from)
        {
            if (_constructors.Count == 0 || from == null)
            {
                // Call parameterless constructor; Create instance of To object
                return Activator.CreateInstance(toType);
            }
            var key = GetCustomMapKey(from.GetType(), toType);

            Delegate method;
            return !_constructors.TryGetValue(key, out method) ? 
                Activator.CreateInstance(toType) : 
                method.DynamicInvoke(@from);
        }

        /// <summary>
        /// Returns a mapped object of toType from the given from object.
        /// </summary>
        /// <param name="toType">The type of object to map to.</param>
        /// <param name="from">The object to map from.</param>
        /// <returns>The mapped object of type toType.</returns>
        private object PerformMap(Type toType, object from)
        {
            return PerformMap(toType, CreateInstance(toType, from), from);
        }

        /// <summary>
        /// Returns a mapped object of toType from the given from object.
        /// </summary>
        /// <param name="toType">The type of object to map to.</param>
        /// <param name="toObject">The instance of the object being mapped to.</param>
        /// <param name="from">The object to map from.</param>
        /// <returns>The mapped object of type toType.</returns>
        private object PerformMap(Type toType, object toObject, object from)
        {
            // Get all the properties for the to object we are about to create
            var toMapAllPropertiesAttr = GetMapAllPropertiesAttribute(toType);

            PropertyInfo[] toInfos = GetPropertyInfos(toType);
      
            object[] fromObjects = GetFromObjects(from);

            if (fromObjects.All(a => a == null))
            {
                return HandleNullFromObject(toObject, toInfos);
            }

            lock (_alreadyMappedEntities)
            {
                if (_alreadyMappedEntities.ContainsKey(from))
                {
                    // This from object was already mapped, so return its corresponding to object
                    return _alreadyMappedEntities[from];
                }

                // Used to prevent infinite loops when mapping children
                if (from != null && !_alreadyMappedEntities.ContainsKey(from))
                {
                    _alreadyMappedEntities.Add(from, toObject);
                }
            }

            IDictionary<PropertyInfo, bool> propertiesMapped = new Dictionary<PropertyInfo, bool>();
            foreach (var fromObject in fromObjects)
            {
                PropertyInfo[] fromInfos = fromObject != null ? GetPropertyInfos(fromObject.GetType()) : null;

                if (fromInfos == null)
                {
                    continue;
                }
                PerformMapOnFromProperties(toObject, toInfos, fromObject, fromInfos, propertiesMapped);
                PerformMapOnToProperties(toObject, fromObject, toMapAllPropertiesAttr, fromInfos, toInfos, propertiesMapped);
                CheckForMappingExceptions(toObject, fromObject, propertiesMapped);
                AssignCustomMap(toObject, fromObject);
            }
            return toObject;
        }

        /// <summary>
        /// Maps any valid properties where the MapProperty is on the From object.
        /// </summary>
        /// <param name="toObject">The instance of the object being mapped to.</param>
        /// <param name="toInfos">The PropertyInfo objects of the To object.</param>
        /// <param name="fromObject">The instance of the object being mapped from.</param>
        /// <param name="fromInfos">The PropertyInfo objects of the From object.</param>
        /// <param name="propertiesMapped">The dictionary holding which PropertyInfos got mapped.</param>
        private void PerformMapOnFromProperties(
            object toObject,
            PropertyInfo[] toInfos,
            object fromObject,
            IEnumerable<PropertyInfo> fromInfos,
            IDictionary<PropertyInfo, bool> propertiesMapped)
        {
            var fromMapAllPropertiesAttr = GetMapAllPropertiesAttribute(fromObject.GetType());

            foreach (var fromProp in fromInfos)
            {
                var mapProperty = GetMapPropertyAttribute(fromProp);
                if ((mapProperty == null && fromMapAllPropertiesAttr == null) ||
                    !CanMap(mapProperty, toObject))
                {
                    continue;
                }
                var name = GetName(mapProperty, fromProp);
                if (!fromProp.CanRead)
                {
                    continue;
                }
                propertiesMapped[fromProp] = false;

                var toProp = toInfos.FirstOrDefault(a => a.Name == name);
                if (toProp == null || !toProp.CanWrite)
                {
                    continue;
                }
                propertiesMapped[fromProp] = true;

                AssignValue(mapProperty, toObject, toProp, fromObject, fromProp);
            }
        }

        /// <summary>
        /// Maps any valid properties where the MapProperty is on the To object.
        /// </summary>
        /// <param name="toObject">The instance of the object being mapped to.</param>
        /// <param name="fromObject">The instance of the object being mapped from.</param>
        /// <param name="toMapAllPropertiesAttr">The MapAllPropertiesAttribute the To object has, if any.</param>
        /// <param name="fromInfos">The PropertyInfo objects of the From object.</param>
        /// <param name="toInfos">The PropertyInfo objects of the To object.</param>
        /// <param name="propertiesMapped">The dictionary holding which PropertyInfos got mapped.</param>
        private void PerformMapOnToProperties(
            object toObject,
            object fromObject,
            MapAllPropertiesAttribute toMapAllPropertiesAttr,
            PropertyInfo[] fromInfos,
            IEnumerable<PropertyInfo> toInfos,
            IDictionary<PropertyInfo, bool> propertiesMapped)
        {
            foreach (var toProp in toInfos)
            {
                var mapProperty = GetMapPropertyAttribute(toProp);
                if ((mapProperty == null && toMapAllPropertiesAttr == null) || 
                    !CanMap(mapProperty, fromObject))
                {
                    continue;
                }
                propertiesMapped[toProp] = false;
                var name = GetName(mapProperty, toProp);
                var fromProp = fromInfos.FirstOrDefault(a => a.Name == name);
                if (fromProp == null || !fromProp.CanRead || !toProp.CanWrite)
                {
                    continue;
                }
                propertiesMapped[toProp] = true;
                AssignValue(mapProperty, toObject, toProp, fromObject, fromProp);
            }
        }

        /// <summary>
        /// Maps a Timestamp property.
        /// </summary>
        /// <param name="toObject">The instance of the object being mapped to.</param>
        /// <param name="toProp">The property being mapped to.</param>
        /// <param name="value">The value of this property on the from object.</param>
        private void MapTimestamp(object toObject, PropertyInfo toProp, object value)
        {
            if (value == null)
            {
                return;
            }
            var timestamp = value as string;
            if (timestamp == null)
            {
                // Transform from byte[] to string
                SetValue(toObject, toProp, Convert.ToBase64String((byte[])value));
            }
            else
            {
                // Transform from string to byte[]
                SetValue(toObject, toProp, Convert.FromBase64String(timestamp));
            }
        }

        /// <summary>
        /// Maps a sub list of objects that will map each of those children.
        /// </summary>
        /// <param name="toObject">The instance of the object being mapped to.</param>
        /// <param name="toProp">The property being mapped to.</param>
        /// <param name="value">The value of this property on the from object.</param>
        private void MapList(object toObject, PropertyInfo toProp, object value)
        {
            // Will only map Generic lists
            IEnumerable valueList = (IEnumerable)value;
            if (!Config.MapEmptyListFromNullList && valueList == null)
            {
                return;
            }

            // Get an instance of the list or create one if needed
            // Must use IList in order to call Add
            IList list = GetValue(toObject, toProp) as IList;
            if (list == null)
            {
                // If the list was not already newed up in constructor, then initialize it
                var listType = _listType.MakeGenericType(toProp.PropertyType.GetGenericArguments()[0]);
                if (!toProp.PropertyType.IsAssignableFrom(listType))
                {
                    return; // Can't map this type of list
                }
                list = Activator.CreateInstance(listType) as IList;
                SetValue(toObject, toProp, list);
            }
            if (valueList == null || list == null)
            {
                // Nothing to map
                return;
            }

            ConcurrentDictionary<int, object> result = new ConcurrentDictionary<int, object>();
            List<Task> tasks = new List<Task>();
            int i = 0;
            foreach (var item in valueList)
            {
                var fromInner = item;
                int currentIndex = i;
                tasks.Add(
                    Task.Factory.StartNew(
                        () =>
                        {
                            var childTo = PerformMap(toProp.PropertyType.GetGenericArguments()[0], fromInner);
                            if (childTo != null)
                            {
                                result.AddOrUpdate(currentIndex, childTo, (x, y) => childTo);
                            }
                        }));
                i++;
            }
            Task.WaitAll(tasks.ToArray());
            foreach (var item in result)
            {
                list.Add(item.Value);
            }
        }

        /// <summary>
        /// Maps the null from object to a defaulted 'to' object with IsNull set to true, if the property exists.
        /// </summary>
        /// <param name="toObject">The object being created.</param>
        /// <param name="toInfos">The properties of the object being created.</param>
        /// <param name="depth">The current depth of the class hierarchy.</param>
        private void MapNull(object toObject, IEnumerable<PropertyInfo> toInfos, int depth = 0)
        {
            AssignIsNull(toObject);
            if (depth >= Config.MaxNullDepth)
            {
                return;
            }
            depth++;

            // Only map nested, non-primitive types of objects in this scenario
            foreach (PropertyInfo toInfo in toInfos)
            {
                if (!toInfo.PropertyType.IsClass || toInfo.PropertyType.IsPrimitive ||
                    toInfo.PropertyType == _stringType)
                {
                    continue;
                }

                MapPropertyAttribute attr = GetMapPropertyAttribute(toInfo);
                if (attr == null)
                {
                    continue;
                }
                var constructor = toInfo.PropertyType.GetConstructor(Type.EmptyTypes);
                if (constructor == null) // Must be parameterless constructor
                {
                    continue;
                }
                var value = constructor.Invoke(null);
                SetValue(toObject, toInfo, value);

                MapNull(value, GetPropertyInfos(toInfo.PropertyType), depth);
            }
        }

        /// <summary>
        /// If the object implements IIsNullable, sets IsNull to true.
        /// </summary>
        /// <param name="to">The instance of the object being mapped to.</param>
        private void AssignIsNull(object to)
        {
            var nullable = to as IIsNullable;
            if (nullable != null)
            {
                nullable.IsNull = true;
            }
        }

        #endregion

        #region << Property Info Check Methods >>

        /// <summary>
        /// Returns whether the From property type can be assigned to the To property type.
        /// </summary>
        /// <param name="toProp">The property the value is being set to.</param>
        /// <param name="fromProp">The property the value is coming from.</param>
        /// <returns>True if the From property type can be assigned to the To propety type; false otherwise.</returns>
        private bool AreEqualTypes(PropertyInfo toProp, PropertyInfo fromProp)
        {
            return toProp.PropertyType.IsAssignableFrom(fromProp.PropertyType);
        }

        /// <summary>
        /// Returns true if the From property is nullable, but the To property is not but are the same base type.
        /// </summary>
        /// <param name="toProp">The property the value is being set to.</param>
        /// <param name="fromProp">The property the value is coming from.</param>
        /// <returns>
        /// True if the From property is nullable, but the To property is not but are the same base type; 
        /// false otherwise.
        /// </returns>
        private bool IsFromNullableToNonNullable(PropertyInfo toProp, PropertyInfo fromProp)
        {
            return fromProp.PropertyType.IsGenericType && IsNullableType(fromProp) &&
                   fromProp.PropertyType.GetGenericArguments()[0] == toProp.PropertyType;
        }

        /// <summary>
        /// Returns whether the given property info represents a nullable type.
        /// </summary>
        /// <param name="prop">The property to inspect.</param>
        /// <returns>True if the property is of Nullable type; false otherwise.</returns>
        private bool IsNullableType(PropertyInfo prop)
        {
            return _nullableType.MakeGenericType(prop.PropertyType.GetGenericArguments()[0])
                    .IsAssignableFrom(prop.PropertyType);
        }

        /// <summary>
        /// Returns whether the To property is an enum and is assignable from the From property.
        /// </summary>
        /// <param name="toProp">The property the value is being set to.</param>
        /// <param name="fromProp">The property the value is coming from.</param>
        /// <returns>True if the To property is an enum and has a matching underlying type with the From property.</returns>
        private bool IsAssignableEnumTo(PropertyInfo toProp, PropertyInfo fromProp)
        {
            return toProp.PropertyType.IsEnum &&
                   (toProp.PropertyType.GetEnumUnderlyingType() == fromProp.PropertyType ||
                    fromProp.PropertyType == _stringType);
        }

        /// <summary>
        /// Returns whether the To property is a nullable enum and is assignable from the From property.
        /// </summary>
        /// <param name="toProp">The property the value is being set to.</param>
        /// <param name="fromProp">The property the value is coming from.</param>
        /// <returns>
        /// True if the To property is a nullable enum and has a matching underlying type with the From property;
        /// false otherwise.
        /// </returns>
        private bool IsAssignableNullableEnumTo(PropertyInfo toProp, PropertyInfo fromProp)
        {
            var toGenArgs = toProp.PropertyType.GetGenericArguments();
            return toProp.PropertyType.IsGenericType &&
                IsNullableType(toProp) &&
                toGenArgs[0].IsEnum &&
                (toGenArgs[0].GetEnumUnderlyingType() == fromProp.PropertyType ||
                    fromProp.PropertyType == _stringType);
        }

        /// <summary>
        /// Returns whether the From property is a nullable enum and the To property is assignable to it.
        /// </summary>
        /// <param name="toProp">The property the value is being set to.</param>
        /// <param name="fromProp">The property the value is coming from.</param>
        /// <returns>
        /// True if the From proeprty is a nullable enum and the To value can be assigned it; 
        /// false otherwise.
        /// </returns>
        private bool IsAssignableNullableEnumFrom(PropertyInfo toProp, PropertyInfo fromProp)
        {
            var fromGenArgs = fromProp.PropertyType.GetGenericArguments();
            return fromProp.PropertyType.IsGenericType &&
                IsNullableType(fromProp) &&
                fromGenArgs[0].IsEnum &&
                (fromGenArgs[0].GetEnumUnderlyingType() == toProp.PropertyType ||
                    fromProp.PropertyType == _stringType);
        }

        /// <summary>
        /// Returns whether the From property is an enum and the To property is not but are assignable.
        /// </summary>
        /// <param name="toProp">The property the value is being set to.</param>
        /// <param name="fromProp">The property the value is coming from.</param>
        /// <returns>True if the From property is an Enum but is assignable to the To property.</returns>
        private bool IsAssignableEnumFrom(PropertyInfo toProp, PropertyInfo fromProp)
        {
            return fromProp.PropertyType.IsEnum && fromProp.PropertyType.GetEnumUnderlyingType() == toProp.PropertyType;
        }

        /// <summary>
        /// Returns whether the given property info represents a list-type object.
        /// </summary>
        /// <param name="prop">The property to inspect.</param>
        /// <returns>True if the property is of IEnumerable type; false otherwise.</returns>
        private bool IsList(PropertyInfo prop)
        {
            return prop.PropertyType.IsGenericType && _iEnumerableType.IsAssignableFrom(prop.PropertyType);
        }

        #endregion

        #region << Get/Set Methods >>

        /// <summary>
        /// Sets the given value on the To object for the given property.
        /// </summary>
        /// <param name="toObject">The object to set the value on.</param>
        /// <param name="propInfo">The property to set the value to.</param>
        /// <param name="val">The value to set.</param>
        private void SetValue(object toObject, PropertyInfo propInfo, object val)
        {
            if (!Config.ExpressionTreeGetSetCalls || toObject.GetType().IsValueType)
            {
                // Cannot use ExpressionTree when working with ValueTypes
                propInfo.SetValue(toObject, val, null);
                return;
            }

            Action<object, object> setter;
            if (!_setterMaps.TryGetValue(propInfo, out setter))
            {
                var target = Expression.Parameter(typeof(object), "obj");
                var value = Expression.Parameter(typeof(object), "value");
                var body =
                    Expression.Assign(
                        Expression.Property(Expression.Convert(target, toObject.GetType()), propInfo),
                        Expression.Convert(value, propInfo.PropertyType));

                var lambda = Expression.Lambda<Action<object, object>>(body, target, value);
                setter = lambda.Compile();
                _setterMaps.AddOrUpdate(propInfo, setter, (info, orig) => orig);
            }
            setter(toObject, val);
        }

        /// <summary>
        /// Gets the value on the From object for the given property.
        /// </summary>
        /// <param name="fromObject">The object to get the value from</param>
        /// <param name="propInfo">The property to get the value from.</param>
        /// <returns>The value of the property on the From object.</returns>
        private object GetValue(object fromObject, PropertyInfo propInfo)
        {
            if (!Config.ExpressionTreeGetSetCalls)
            {
                return propInfo.GetValue(fromObject, null);
            }

            Func<object, object> getter;
            if (_getterMaps.TryGetValue(propInfo, out getter))
            {
                return getter(fromObject);
            }
            var target = Expression.Parameter(typeof(object), "obj");

            var body =
                Expression.Convert(
                    Expression.Property(Expression.Convert(target, fromObject.GetType()), propInfo),
                    typeof(object));
            var lambda = Expression.Lambda<Func<object, object>>(body, target);
            getter = lambda.Compile();
            _getterMaps.AddOrUpdate(propInfo, getter, (info, orig) => orig);
            return getter(fromObject);
        }

        #endregion

        #region << Helper Methods >>

        /// <summary>
        /// Handles the scenario when a To object is being mapped from a null From object.
        /// </summary>
        /// <param name="toObject">The instance of the object being mapped to.</param>
        /// <param name="toInfos">The PropertyInfo objects of the To object.</param>
        /// <returns>The To object when the From object is null.</returns>
        private object HandleNullFromObject(object toObject, IEnumerable<PropertyInfo> toInfos)
        {
            if (!Config.CreateToObjectFromNullFromObject)
            {
                return null;
            }
            MapNull(toObject, toInfos);
            return toObject;
        }

        /// <summary>
        /// If applicable, throws a MappingException if a PropertyInfo did not map.
        /// </summary>
        /// <param name="toObject">The instance of the object being mapped to.</param>
        /// <param name="fromObject">The instance of the object being mapped from.</param>
        /// <param name="propertiesMapped">The dictionary holding which PropertyInfos got mapped.</param>
        private void CheckForMappingExceptions(
            object toObject,
            object fromObject,
            IEnumerable<KeyValuePair<PropertyInfo, bool>> propertiesMapped)
        {
            if (!Config.ThrowExceptionIfNoMatchingPropertyFound)
            {
                return;
            }

            IList<string> notMappedNames = new List<string>();
            foreach (var pair in propertiesMapped)
            {
                if (pair.Value)
                {
                    continue;
                }
                notMappedNames.Add(pair.Key.Name);
            }

            if (notMappedNames.Count > 0)
            {
                throw new MappingException(
                    string.Format(
                        "The following properties were not mapped from type '{0}' to type '{1}': {2}",
                        fromObject.GetType().FullName,
                        toObject.GetType().FullName,
                        string.Join(", ", notMappedNames)));
            }
        }

        /// <summary>
        /// Returns whether this property can be mapped.
        /// </summary>
        /// <param name="attr">The MapProperty attribute used to determine if it can be mapped.</param>
        /// <param name="check">The object where the data is being mapped to/from.</param>
        /// <returns>True if this property can be mapped; false otherwise.</returns>
        private bool CanMap(MapPropertyAttribute attr, object check)
        {
            return attr == null ||
                attr.FullName == null ||
                check.GetType().FullName.Equals(attr.FullName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the key to look up if any custom maps exist for the given combination of to/from types.
        /// </summary>
        /// <param name="from">The type of object being mapped from.</param>
        /// <param name="to">he type of object being mapped to.</param>
        /// <returns></returns>
        private Tuple<Type, Type> GetCustomMapKey(Type from, Type to)
        {
            return new Tuple<Type, Type>(from, to);
        }

        /// <summary>
        /// Returns objects to map from, given the from object. (Used to flatten a hierarchy.)
        /// </summary>
        /// <param name="fromObject">The object being mapped from.</param>
        /// <returns>All objects to map from.</returns>
        private object[] GetFromObjects(object fromObject)
        {
            object[] fromObjects = null;

            if (fromObject == null || _fromObjects.Count == 0) // No way to figure out type if null
            {
                return new[] { fromObject };
            }

            Delegate getFromObjects;
            if (!_fromObjects.TryGetValue(fromObject.GetType(), out getFromObjects))
            {
                return new[] { fromObject }; // No from mapping
            }

            if (getFromObjects != null)
            {
                fromObjects = getFromObjects.DynamicInvoke(fromObject) as object[];
            }
            return fromObjects ?? new[] { fromObject };
        }

        /// <summary>
        /// Returns the name of the property.
        /// </summary>
        /// <param name="attr">The property that may have the specific property name to look up.</param>
        /// <param name="pi">The actual property to use if attr is not specified.</param>
        /// <returns>Name of the property to use.</returns>
        private static string GetName(MapPropertyAttribute attr, PropertyInfo pi)
        {
            return attr == null || string.IsNullOrEmpty(attr.PropertyName) ? pi.Name : attr.PropertyName;
        }

        #endregion

    }
}