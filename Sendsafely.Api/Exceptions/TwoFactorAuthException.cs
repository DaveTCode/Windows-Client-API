using System;

namespace Sendsafely.Api.Exceptions
{
    /// <summary>
    /// Thrown when two factor authentication is required. The exception contains a ValidationToken parameter that must be used when validating the 2FA Code.
    /// </summary>
    [Serializable]
    public class TwoFactorAuthException : BaseException
    {

        private string _validationToken;

        public TwoFactorAuthException(string validationToken)
        {
            _validationToken = validationToken;
        }

        public string ValidationToken
        {
            get => _validationToken;
            set => _validationToken = value;
        }
    }
}
