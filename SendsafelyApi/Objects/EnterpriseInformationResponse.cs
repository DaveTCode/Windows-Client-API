using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class EnterpriseInformationResponse
    {
        [JsonProperty(PropertyName = "host")]
        public string Host { get; set; }

        [JsonProperty(PropertyName = "systemName")]
        public string SystemName { get; set; }

        [JsonProperty(PropertyName = "allowUndisclosedRecipients")]
        public bool AllowUndisclosedRecipients { get; set; }

        [JsonProperty(PropertyName = "outlookBeta")]
        public bool OutlookBeta { get; set; }

        [JsonProperty(PropertyName = "messageEncryption")]
        public bool MessageEncryption { get; set; }
    }
}
