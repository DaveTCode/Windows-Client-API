using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AddContactGroupResponse : StandardResponse
    {
        [JsonProperty(PropertyName = "contactGroupUserEmails")]
        public List<string> ContactGroupUserEmails { get; set; }

        [JsonProperty(PropertyName = "contactGroupId")]
        public string ContactGroupId { get; set; }

        [JsonProperty(PropertyName = "contactGroupName")]
        public string ContactGroupName { get; set; }
    }
}
