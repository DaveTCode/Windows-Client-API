using System;

namespace Sendsafely.Api.Exceptions
{
    /// <summary>
    /// Thrown when the API key, secret key or both is incorrect. The exception message might contain more detailed information.
    /// </summary>
    [Serializable]
    public class InvalidCredentialsException : BaseException
    {
        public InvalidCredentialsException(string message)
            : base(message)
        {
            ;
        }
    }

}
