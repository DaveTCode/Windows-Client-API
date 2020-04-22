using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class StandardResponse
    {
        [JsonProperty(PropertyName = "response")]
        internal APIResponse Response { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
