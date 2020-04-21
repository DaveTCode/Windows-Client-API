using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class FinalizePackageResponse
    {
        [JsonProperty(PropertyName = "response")]
        internal APIResponse Response { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public List<string> Errors { get; set; }

        [JsonProperty(PropertyName = "approvers")]
        public List<string> Approvers { get; set; }
    }
}
