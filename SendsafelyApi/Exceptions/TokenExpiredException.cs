using System;

namespace SendsafelyApi.Exceptions
{
    /// <summary>
    /// Thrown when the token or PIN has expired. When this happens, the client should reach out and ask for a new token.
    /// </summary>
    [Serializable]
    public class TokenExpiredException : BaseException
    {

        public TokenExpiredException()
        { }

        public TokenExpiredException(string message)
            : base(message)
        { }
    }
}
