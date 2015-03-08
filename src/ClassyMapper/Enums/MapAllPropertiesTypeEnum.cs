namespace ClassyMapper.Enums
{
    /// <summary>
    /// This enum is used to define whether All properties or only base type (inherited) proprties should be mapped.
    /// MapProperty attributes would still be recognized
    /// </summary>
    public enum MapAllPropertiesTypeEnum
    {
        /// <summary>
        /// All properties from the entire inheritence hierarchy are included.
        /// </summary>
        All,
        /// <summary>
        /// All properties below the top are included.
        /// </summary>
        BaseTypeOnly,
        /// <summary>
        /// Only properties from the current level are included.
        /// </summary>
        TopLevelOnly,
        /// <summary>
        /// All properties are excluded from mapping.  Only MapProperty attributes will be included.
        /// </summary>
        None
    }
}
