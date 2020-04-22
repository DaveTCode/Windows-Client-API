using System;

namespace SendsafelyApi.Exceptions
{
    /// <summary>
    /// Thrown when the specified user's password does not match the servers demand on password complexity.
    /// </summary>
    [Serializable]
    public class InsufficientPasswordComplexityException : BaseException
    {
        public InsufficientPasswordComplexityException(string message)
            : base(message)
        {
            ;
        }
    }

}
