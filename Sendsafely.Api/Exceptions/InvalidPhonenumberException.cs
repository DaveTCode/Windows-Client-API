using System;

namespace Sendsafely.Api.Exceptions
{
    /// <summary>
    /// Thrown when the supplied phone number is in a format not recognized by the server.
    /// </summary>
    [Serializable]
    public class InvalidPhonenumberException : BaseException
    {
        public InvalidPhonenumberException(string message)
            : base(message)
        {

        }
    }
}
