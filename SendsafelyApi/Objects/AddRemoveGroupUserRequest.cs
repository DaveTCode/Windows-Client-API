using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AddRemoveGroupUserRequest
    {
        [JsonProperty(PropertyName = "userEmail")]
        public string UserEmail { get; set; }
    }
}
