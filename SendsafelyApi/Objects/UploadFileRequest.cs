using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UploadFileRequest
    {
        [JsonProperty(PropertyName = "uploadType")]
        public string UploadType { get; set; }

        [JsonProperty(PropertyName = "filePart")]
        public int FilePart { get; set; }
    }
}
