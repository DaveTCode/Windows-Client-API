using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class PackageInformationResponse
    {
        [JsonProperty(PropertyName = "response")]
        internal APIResponse Response { get; set; }

        [JsonProperty(PropertyName = "packageId")]
        internal string PackageID { get; set; }

        [JsonProperty(PropertyName = "packageCode")]
        internal string PackageCode { get; set; }

        [JsonProperty(PropertyName = "serverSecret")]
        internal string ServerSecret { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "needsApproval")]
        public bool NeedsApprover { get; set; }

        [JsonProperty(PropertyName = "recipients")]
        internal List<Recipient> Recipients { get; set; }

        [JsonProperty(PropertyName = "files")]
        public List<File> Files { get; set; }

        [JsonProperty(PropertyName = "approverList")]
        public List<string> Approvers { get; set; }

        [JsonProperty(PropertyName = "life")]
        public int Life { get; set; }

        [JsonProperty(PropertyName = "packageTimestamp")]
        public DateTime PackageTimestamp { get; set; }

        [JsonProperty(PropertyName = "packageSender")]
        public string PackageSender { get; set; }

        [JsonProperty(PropertyName = "rootDirectoryId")]
        public string RootDirectoryId { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "isVDR")]
        public bool IsVDR { get; set; }

        /// <summary>
        /// A list of contact groups
        /// </summary>
        [JsonProperty(PropertyName = "contactGroups")]
        public List<ContactGroup> ContactGroups { get; set; }

        /// <summary>
        /// a string of state
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "allowReplyAll")]
        public bool AllowReplyAll { get; set; }

        [JsonProperty(PropertyName = "packageParentId")]
        public string PackageParentId { get; set; }
    }
}
