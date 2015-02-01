namespace Classy
{
    /// <summary>
    /// This class contains configuration data for ClassyMapper.
    /// </summary>
    public class ClassyMapperConfig
    {

        #region << Properties >>

        /// <summary>
        /// Gets or sets whether From object results in the to object getting created with IsNull set to true. 
        /// Default: false
        /// </summary>
        /// <remarks>
        /// Note: This is useful when you have a hierarchy and are working with local SSRS reports and want to ignore
        ///       all of the IsNot Nothing checks.
        /// </remarks>
        public bool CreateToObjectFromNullFromObject { get; set; }

        /// <summary>
        /// Gets or sets the maximum depth to new up null objects once one is found. 
        /// Default: 10
        /// </summary>
        public int MaxNullDepth { get; set; }

        /// <summary>
        /// Gets or sets whether a null list on the From object results in an empty list on the To object. 
        /// Default: true
        /// </summary>
        public bool MapEmptyListFromNullList { get; set; }

        /// <summary>
        /// Gets or sets whether to throw an exception if a MapProperty is not mapped.  
        /// Default: false.
        /// </summary>
        public bool ThrowExceptionIfNoMatchingPropertyFound { get; set; }

        /// <summary>
        /// Gets or sets whether to ignore the case when parsing an enum.  
        /// Default: true.
        /// </summary>
        public bool IgnoreEnumCase { get; set; }

        #endregion

        #region << Cosntructors >>

        /// <summary>
        /// Default constructor that defaults values.
        /// </summary>
        public ClassyMapperConfig()
        {
            MaxNullDepth = 10;
            MapEmptyListFromNullList = true;
            IgnoreEnumCase = true;
        }

        #endregion

    }
}
