#region << Usings >>

using System;

#endregion

namespace ClassyMapper.Exceptions
{
    /// <summary>
    /// This exception is thrown when something prevents two classes from being mapped together.
    /// </summary>
    [Serializable]
    public class ClassyMapException : Exception
    {

        #region << Constructors >>

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ClassyMapException() 
            : this(null, null)
        {
        }

        /// <summary>
        /// Constructor that takes a message.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        public ClassyMapException(string message) 
            : this(message, null)
        {
        }

        /// <summary>
        /// Constructor that takes a message and inner exception
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClassyMapException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion

    }
}