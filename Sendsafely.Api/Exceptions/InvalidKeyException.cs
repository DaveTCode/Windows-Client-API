using System;

namespace Sendsafely.Api.Exceptions
{
    /// <summary>
    /// Thrown when the public key does not contain a public key
    /// </summary>
    [Serializable]
    public class InvalidKeyException : BaseException
    {
        public InvalidKeyException(string message)
            : base(message)
        {
            ;
        }
    }

}
