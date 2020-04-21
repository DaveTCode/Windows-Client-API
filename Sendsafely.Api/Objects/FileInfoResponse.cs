using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class FileInformationResponse : StandardResponse
    {
        [JsonProperty(PropertyName = "file")]
        internal FileInformation File { get; set; }
    }
}
