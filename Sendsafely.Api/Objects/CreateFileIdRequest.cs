using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateFileIdRequest
    {

        [JsonProperty(PropertyName = "uploadType")]
        public string UploadType { get; set; }

        [JsonProperty(PropertyName = "parts")]
        public int Parts { get; set; }

        [JsonProperty(PropertyName = "filename")]
        public string Filename { get; set; }

        [JsonProperty(PropertyName = "filesize")]
        public long Filesize { get; set; }

        [JsonProperty(PropertyName = "directoryId")]
        public string DirectoryId { get; set; }
    }
}
