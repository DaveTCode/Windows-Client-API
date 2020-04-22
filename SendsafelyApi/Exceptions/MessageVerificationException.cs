using System;

namespace SendsafelyApi.Exceptions
{
    /// <summary>
    /// Will be thrown when the API is not yet initialized.
    /// </summary>
    [Serializable]
    public class MessageVerificationException : BaseException
    {
        public MessageVerificationException()
            : base()
        { }

        public MessageVerificationException(string message)
            : base(message)
        { }
    }
}
  { }
    }
}
