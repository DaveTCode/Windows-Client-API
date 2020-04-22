using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class FileInformationResponse : StandardResponse
    {
        [JsonProperty(PropertyName = "file")]
        internal FileInformation File { get; set; }
    }
}
