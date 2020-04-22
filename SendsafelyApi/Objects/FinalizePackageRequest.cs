using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class FinalizePackageRequest
    {
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "checksum")]
        public string Checksum { get; set; }

        [JsonProperty(PropertyName = "undisclosedRecipients")]
        public bool UndisclosedRecipients { get; set; }

        [JsonProperty(PropertyName = "allowReplyAll")]
        public bool AllowReplyAll { get; set; }
    }
}
