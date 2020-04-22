using System;

namespace SendsafelyApi.Exceptions
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