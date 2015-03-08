#region << Usings >>

using System;
using ClassyMapper.Enums;

#endregion

namespace ClassyMapper.Attributes
{
    /// <summary>
    /// This attribute is used to map property values bewteen Entities and ViewModels.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class MapPropertyAttribute : Attribute
    {

        #region << Properties >>

        /// <summary>
        /// Gets or sets the name of the property on map to/from.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the full class name with namespace of where to map to/from.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets whether the property this attribute is attached to should or should not be mapped.
        /// </summary>
        public MapPropertyTypeEnum MapPropertyType { get; set; }

        /// <summary>
        /// Gets or sets whether this is a mapping for a Timestamp (byte[] to/from string).
        /// </summary>
        public bool IsTimestamp { get; set; }

        #endregion

        #region << Constructors >>
        
        /// <summary>
        /// Default constuctor that assumes the property name is based on the property this attribute is
        /// associated with.
        /// </summary>
        /// <param name="propertyName">The name of the property to map to/from.</param>
        /// <param name="mapPropertyType">Whether to include or exclude this property from mapping.</param>
        /// <param name="fullName">The full class name with namespace of where to map to/from.</param>
        /// <param name="isTimesatmp">T
        /// rue if this property is for a Timestamp (byte[] to/from string); false otherwise.
        /// </param>
        /// <remarks>
        /// Specifying the FullName is only necessary when you are flattening out a hierarchy 
        /// that share the same property names.
        /// </remarks>
        public MapPropertyAttribute(
            string propertyName = null,
            MapPropertyTypeEnum mapPropertyType = MapPropertyTypeEnum.Include, 
            string fullName = null, 
            bool isTimesatmp = false)
        {
            PropertyName = propertyName;
            MapPropertyType = mapPropertyType;
            FullName = fullName;
            IsTimestamp = isTimesatmp;
        }

        #endregion

    }
}