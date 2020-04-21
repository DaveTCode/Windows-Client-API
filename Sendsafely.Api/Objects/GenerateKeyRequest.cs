using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GenerateKeyRequest
    {

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "keyDescription")]
        public string KeyDescription { get; set; }

        [JsonProperty(PropertyName = "smsCode")]
        public string SMSCode { get; set; }
    }
}
