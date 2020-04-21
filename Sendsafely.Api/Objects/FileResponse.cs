using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class FileResponse
    {
        [JsonProperty(PropertyName = "fileId")]
        public string FileId { get; set; }

        [JsonProperty(PropertyName = "fileName")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "fileSize")]
        public long FileSize { get; set; }

        [JsonProperty(PropertyName = "createdByEmail")]
        public string CreatedByEmail { get; set; }

        [JsonProperty(PropertyName = "parts")]
        public int Parts { get; set; }
    }
}
