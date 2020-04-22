using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AddDropzoneRecipientRequest
    {
        [JsonProperty(PropertyName = "userEmail")]
        public string UserEmail { get; set; }
    }
}
