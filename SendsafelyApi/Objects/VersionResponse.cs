using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class VersionResponse
    {
        [JsonProperty(PropertyName = "version")]
        public Version Version { get; set; }

        [JsonProperty(PropertyName = "response")]
        public APIResponse Response { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
