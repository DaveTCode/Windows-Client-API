using System;

namespace Sendsafely.Api.Exceptions
{
    /// <summary>
    /// The BaseException from which all SendSafely exceptions inherit from. By catching this class all custom SendSafely exceptions will be caught.
    /// </summary>
    [Serializable]
    public class BaseException : Exception
    {
        public BaseException(string message)
            : base(message)
        {; }

        public BaseException()
            : base()
        {; }
    }
}
