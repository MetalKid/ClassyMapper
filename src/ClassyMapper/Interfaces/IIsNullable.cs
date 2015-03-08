namespace ClassyMapper.Interfaces
{
    /// <summary>
    /// This interface is used to determine if a mapped to class is actually mapped from a null object.
    /// </summary>
    public interface IIsNullable
    {
        /// <summary>
        /// Gets or sets whether the object is null.
        /// </summary>
        bool IsNull { get; set; }
    }
}
