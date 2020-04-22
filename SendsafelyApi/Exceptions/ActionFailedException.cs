using System;

namespace SendsafelyApi.Exceptions
{
    /// <summary>
    /// This exception is thrown when the server returned an unexpected response message. The reason can be found in the 
    /// Reason variable. The exception message will contain a longer explanation to the error.
    /// </summary>
    [Serializable]
    public class ActionFailedException : BaseException
    {
        private string _reason;

        public ActionFailedException(string reason, string message)
            : base(message)
        {
            _reason = reason;
        }

        public string Reason
        {
            get => _reason;
            set => _reason = value;
        }
    }
}
