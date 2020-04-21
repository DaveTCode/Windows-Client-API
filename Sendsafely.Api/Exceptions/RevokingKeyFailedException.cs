using System;

namespace Sendsafely.Api.Exceptions
{
    /// <summary>
    /// Thrown when we failed to revoke a public key
    /// </summary>
    [Serializable]
    public class RevokingKeyFailedException : BaseException
    {
        public RevokingKeyFailedException(string message)
            : base(message)
        {
            ;
        }
    }

}
