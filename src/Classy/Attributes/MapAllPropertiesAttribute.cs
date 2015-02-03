#region << Usings >>

using System;

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
    }
}
