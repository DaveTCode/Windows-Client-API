using System;

namespace SendsafelyApi.Exceptions
{
    /// <summary>
    /// Thrown when the API fails to connect to the server.
    /// </summary>
    [Serializable]
    public class ServerUnavailableException : BaseException
    {
        public ServerUnavailableException()
            : base()
        { }

        public ServerUnavailableException(string message)
            : base(message)
        { }
    }
}
