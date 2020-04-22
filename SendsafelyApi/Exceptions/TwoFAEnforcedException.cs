using System;

namespace SendsafelyApi.Exceptions
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
