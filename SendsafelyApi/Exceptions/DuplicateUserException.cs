using System;

namespace SendsafelyApi.Exceptions
{
    /// <summary>
    /// Thrown when a Google Auth user already exists as a native user
    /// </summary>
    [Serializable]
    public class DuplicateUserException : BaseException
    {
        public DuplicateUserException(string message)
            : base(message)
        {
            ;
        }
    }

}
