using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class PackageDTO
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
        internal List<string> Recipients { get; set; }

        [JsonProperty(PropertyName = "filenames")]
        public List<string> Filenames { get; set; }

        [JsonProperty(PropertyName = "approverList")]
        public List<string> Approvers { get; set; }

        [JsonProperty(PropertyName = "life")]
        public int Life { get; set; }

        [JsonProperty(PropertyName = "packageUpdateTimestamp")]
        public DateTime PackageUpdateTimestamp { get; set; }

        [JsonProperty(PropertyName = "packageSender")]
        public string PackageSender { get; set; }

        [JsonProperty(PropertyName = "packageUserName")]
        public string PackageUserName { get; set; }

        [JsonProperty(PropertyName = "packageState")]
        public string PackageState { get; set; }

        [JsonProperty(PropertyName = "contactGroups")]
        public List<string> ContactGroups { get; set; }

        [JsonProperty(PropertyName = "packageParentId")]
        public string PackageParentId { get; set; }
    }
}
