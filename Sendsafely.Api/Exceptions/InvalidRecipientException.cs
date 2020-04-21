using System;

namespace Sendsafely.Api.Exceptions
{
    /// <summary>
    /// Thrown when an attempt to update a recipient with an incorrect recipient ID is made.
    /// </summary>
    [Serializable]
    public class InvalidRecipientException : BaseException
    {
        public InvalidRecipientException(string message)
            : base(message)
        {

        }
    }
}
