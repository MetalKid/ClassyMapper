#region << Usings >>

using System;
using Classy.Enums;

#endregion

namespace Classy.Attributes
{
    /// <summary>
    /// This attribute is used to specify that all properties should be mapped.  If at least one property should
    /// not be mapped, then remvoe this attribute and add MapProperty attributes instead.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class MapAllPropertiesAttribute : Attribute
    {

        #region << Properties >>

        /// <summary>
        /// Gets which properties should be included during mapping.
        /// </summary>
        public MapAllPropertiesTypeEnum AllPropertiesType { get; private set; }

        /// <summary>
        /// Gets the base types that are allowed to be mapped, if any.
        /// </summary>
        public Type[] AllowedBaseTypes { get; private set; }

        #endregion

        #region << Constructors >>

        /// <summary>
        /// Constructor that takes an optional parameter indicating which properties should be included.
        /// </summary>
        /// <param name="allowedBaseTypes">
        /// The base types that are allowed to be mapped during MapAllPropertiesTypeEnum.BaseTypeOnly. 
        /// If none, entire hierarchy is assumed.
        /// </param>
        public MapAllPropertiesAttribute(
          params Type[] allowedBaseTypes)
        {
            AllPropertiesType = MapAllPropertiesTypeEnum.All;
            AllowedBaseTypes = allowedBaseTypes;
        }

        /// <summary>
        /// Constructor that takes an optional parameter indicating which properties should be included.
        /// </summary>
        /// <param name="allPropertiesType">Which properties should be included during mapping.</param>
        /// <param name="allowedBaseTypes">
        /// The base types that are allowed to be mapped during MapAllPropertiesTypeEnum.BaseTypeOnly. 
        /// If none, entire hierarchy is assumed.
        /// </param>
        public MapAllPropertiesAttribute(
            MapAllPropertiesTypeEnum allPropertiesType,
            params Type[] allowedBaseTypes)
        {
            AllPropertiesType = allPropertiesType;
            AllowedBaseTypes = allowedBaseTypes;
        }

        #endregion

    }
}
