using System;
using System.Collections.Generic;

namespace Sendsafely.Api.Exceptions
{
    /// <summary>
    /// Thrown when a package approval is needed in order for the recipients to be able to access the package. 
    /// When this exception is thrown the approvers must be notified so they can download and approve the other recipients.
    /// </summary>
    [Serializable]
    public class PackageNeedsApprovalException : BaseException
    {
        private string _link;
        private List<string> _approvers;

        public PackageNeedsApprovalException(List<string> approvers)
        {
            _approvers = approvers;
        }

        public List<string> Approvers
        {
            get => _approvers;
            set => _approvers = value;
        }

        public string Link
        {
            get => _link;
            set => _link = value;
        }
    }
}
