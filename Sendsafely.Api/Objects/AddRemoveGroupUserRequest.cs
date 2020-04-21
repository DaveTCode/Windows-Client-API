using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AddRemoveGroupUserRequest
    {
        [JsonProperty(PropertyName = "userEmail")]
        public string UserEmail { get; set; }
    }
}
