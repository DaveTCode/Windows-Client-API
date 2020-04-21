using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ContactGroup
    {
        [JsonProperty(PropertyName = "users")]
        public List<ContactGroupMember> Users { get; set; }

        [JsonProperty(PropertyName = "contactGroupId")]
        public string ContactGroupId { get; set; }

        [JsonProperty(PropertyName = "contactGroupName")]
        public string ContactGroupName { get; set; }

        [JsonProperty(PropertyName = "contactGroupIsOrganizationGroup")]
        public bool ContactGroupIsOrganizationGroup { get; set; }
    }
}
