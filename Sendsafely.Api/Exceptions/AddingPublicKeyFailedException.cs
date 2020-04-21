using System;

namespace Sendsafely.Api.Exceptions
{
    /// <summary>
    /// Thrown when we failed to create and add a public key
    /// </summary>
    [Serializable]
    public class AddingPublicKeyFailedException : BaseException
    {
        public AddingPublicKeyFailedException(string message)
            : base(message)
        {
            ;
        }
    }

}
