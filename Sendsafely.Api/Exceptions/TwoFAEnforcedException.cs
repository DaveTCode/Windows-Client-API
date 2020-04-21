using System;

namespace Sendsafely.Api.Exceptions
{
    /// <summary>
    /// Thrown when an organization enforce two fa flag is true
    /// </summary>
    [Serializable]
    public class TwoFAEnforcedException : BaseException
    {
        public TwoFAEnforcedException(string message)
            : base(message)
        {
            ;
        }
    }

}
