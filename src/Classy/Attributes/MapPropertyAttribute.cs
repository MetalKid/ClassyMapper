#region << Usings >>

using System;

#endregion

namespace Classy.Attributes
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

        #endregion

        #region << Constructors >>
        
        /// <summary>
        /// Default constuctor that assumes the property name is based on the property this attribute is
        /// associated with.
        /// </summary>
        /// <param name="propertyName">The name of the property to map to/from.</param>
        /// <param name="fullName">The full class name with namespace of where to map to/from.</param>
        /// <remarks>
        /// Specifying the FullName is only necessary when you are flattening out a hierarchy 
        /// that share the same property names.
        /// </remarks>
        public MapPropertyAttribute(string propertyName = null, string fullName = null)
        {
            PropertyName = propertyName;
            FullName = fullName;
        }

        #endregion

    }
}