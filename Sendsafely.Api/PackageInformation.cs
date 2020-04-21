using System;
using System.Collections.Generic;
using Sendsafely.Api.Objects;

namespace Sendsafely.Api
{
    /// <summary>
    /// This object will contain information about a package. Once a package is created this object will be returned. 
    /// If it is passed along when adding recipients and files the object will be updated accordingly.
    /// </summary>
    public class PackageInformation
    {
        /// <summary>
        /// The package ID for the given package.
        /// </summary>
        public string PackageId { get; set; }

        /// <summary>
        /// The package code for the given package. The package code is a part of the URL that must be sent to the recipients.
        /// </summary>
        public string PackageCode { get; set; }

        /// <summary>
        /// The keycode for the package. This key should always be kept client side and never be sent to the server. 
        /// The keycode makes up one part of the encryption key.
        /// </summary>
        public string KeyCode { get; set; }

        /// <summary>
        /// The server secret makes together with the keycode up the encryption key. The server secret is specific 
        /// to a package and passed from the server.
        /// </summary>
        public string ServerSecret { get; set; }

        /// <summary>
        /// NeedsApprover will be true when a package needs to add at least one approver before the package can be finalized.
        /// If the package is finalized without the approver, an exception will be thrown.
        /// </summary>
        public bool NeedsApprover { get; set; }

        /// <summary>
        /// A list of recipients that are currently attached to the package.
        /// </summary>
        public List<Recipient> Recipients { get; set; }

        /// <summary>
        /// A list of files that are currently attached to the package.
        /// </summary>
        public List<File> Files { get; set; }

        /// <summary>
        /// A list of approvers that are currently attached to the package.
        /// </summary>
        public List<string> Approvers { get; set; }

        /// <summary>
        /// The current package life. The package life determines for how long the package 
        /// should be available to the recipients. It's measured in days.
        /// </summary>
        public int Life { get; set; }

        /// <summary>
        /// The timestamp of when the package was finalized.
        /// </summary>
        public DateTime PackageTimestamp { get; set; }

        /// <summary>
        /// The Package Owner of the package.
        /// </summary>
        public string PackageOwner { get; set; }

        /// <summary>
        /// The state of the package.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The status of the package.
        /// </summary>
        public PackageStatus Status { get; set; }

        /// <summary>
        /// The root directory of a workspace package.
        /// </summary>
        public string RootDirectoryId { get; set; }

        /// <summary>
        /// The package descriptor.
        /// </summary>
        public string PackageDescriptor { get; set; }

        /// <summary>
        /// The flag specifying the package is a workspace.
        /// </summary>
        public bool IsWorkspace { get; set; }

        /// <summary>
        /// A list of contact groups
        /// </summary>
        public List<ContactGroup> ContactGroups { get; set; }

        /// <summary>
        /// Allow reply all, false if BCC recipients
        /// </summary>
        public bool AllowReplyAll { get; set; }

        /// <summary>
        /// Package parent id
        /// </summary>
        public string PackageParentId { get; set; }
    }
}
