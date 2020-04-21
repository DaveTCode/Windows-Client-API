using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateDirectoryRequest
    {
        [JsonProperty(PropertyName = "directoryName")]
        public string DirectoryName { get; set; }

    }
}
