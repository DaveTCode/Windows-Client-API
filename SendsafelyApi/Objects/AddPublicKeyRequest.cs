using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AddPublicKeyRequest
    {
        [JsonProperty(PropertyName = "publicKey")]
        public string PublicKey { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}
