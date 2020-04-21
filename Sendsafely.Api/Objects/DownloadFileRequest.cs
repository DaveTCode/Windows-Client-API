using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class DownloadFileRequest
    {
        [JsonProperty(PropertyName = "checksum")]
        public string Checksum { get; set; }

        [JsonProperty(PropertyName = "part")]
        public int Part { get; set; }

        [JsonProperty(PropertyName = "api")]
        public string Api { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }
}
