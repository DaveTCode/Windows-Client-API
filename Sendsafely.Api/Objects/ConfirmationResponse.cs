using System;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class ConfirmationResponse
    {
        [JsonProperty(PropertyName = "ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "file")]
        public FileResponse File { get; set; }
    }
}
