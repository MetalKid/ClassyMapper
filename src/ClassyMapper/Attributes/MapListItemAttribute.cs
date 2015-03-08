#region << Usings >>

using System;
using System.Collections.Generic;

#endregion

namespace ClassyMapper.Attributes
{
    /// <summary>
    /// This attribute is used to map two classes contained in a list based on key properties that make them unique.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class MapListItemAttribute : Attribute
    {

        #region << Properties >>

        /// <summary>
        /// Gets or sets the list of properties to match up two items in two different lists.
        /// </summary>
        public IEnumerable<string> KeyProperties { get; set; }

        #endregion

        #region << Constructors >>

        /// <summary>
        /// Constructor that takes the names of properties to match up two items in two different lists.
        /// </summary>
        /// <param name="keyProperties">Names of properties to match up two items in two different lists.</param>
        public MapListItemAttribute(params string[] keyProperties)
        {
            if (keyProperties == null || keyProperties.Length == 0)
            {
                throw new ArgumentNullException("keyProperties");
            }
            KeyProperties = keyProperties;
        }

        #endregion

    }
}
