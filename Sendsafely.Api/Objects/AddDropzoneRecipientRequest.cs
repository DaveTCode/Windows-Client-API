using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AddDropzoneRecipientRequest
    {
        [JsonProperty(PropertyName = "userEmail")]
        public string UserEmail { get; set; }
    }
}
