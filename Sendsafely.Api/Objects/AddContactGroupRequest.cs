using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AddContactGroupRequest
    {
        [JsonProperty(PropertyName = "groupName")]
        public string GroupName { get; set; }

        [JsonProperty(PropertyName = "isEnterpriseGroup")]
        public string IsEnterpriseGroup { get; set; }
    }
}
