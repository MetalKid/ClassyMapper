#region << Usings >>

using System;

#endregion

namespace Classy.Attributes
{
    /// <summary>
    /// This attribute is used to convert a byte[] timestamp to a string and vice-versa.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class MapPropertyTimestamp : MapPropertyAttribute
    {
     
        #region << Constructors >>
        
        /// <summary>
        /// Constuctor that takes an optional parameter of what the name of the property is to map to/from.
        /// </summary>
        /// <param name="propertyName">The name of the property on the Entity.</param>
        public MapPropertyTimestamp(string propertyName = null)
            : base(propertyName)
        {
            // Nothing to do
        }

        #endregion

    }
}
