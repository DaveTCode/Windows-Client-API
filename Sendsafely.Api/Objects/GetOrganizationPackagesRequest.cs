using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GetOrganizationPackagesRequest
    {

        [JsonProperty(PropertyName = "fromDate")]
        public string FromDate { get; set; }

        [JsonProperty(PropertyName = "toDate")]
        public string ToDate { get; set; }

        [JsonProperty(PropertyName = "sender")]
        public string Sender { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "recipient")]
        public string Recipient { get; set; }

        [JsonProperty(PropertyName = "filename")]
        public string Filename { get; set; }
    }
}
