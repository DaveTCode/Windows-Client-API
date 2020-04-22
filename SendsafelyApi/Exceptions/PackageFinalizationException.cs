using System;
using System.Collections.Generic;

namespace SendsafelyApi.Exceptions
{
    /// <summary>
    /// Thrown when the package can for some reason not be finalized. 
    /// The exception contains a list of errors that prevented the finalization.
    /// </summary>
    [Serializable]
    public class PackageFinalizationException : BaseException
    {
        private List<string> _errors;

        public PackageFinalizationException(List<string> errors)
        {
            _errors = errors;
        }

        public List<string> Errors
        {
            get => _errors;
            set => _errors = value;
        }
    }
}
