using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GetUserGroupsResponse : StandardResponse
    {
        [JsonProperty(PropertyName = "contactGroups")]
        public List<ContactGroup> ContactGroups { get; set; }
    }
}
