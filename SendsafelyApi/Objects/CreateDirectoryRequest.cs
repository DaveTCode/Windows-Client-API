using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateDirectoryRequest
    {
        [JsonProperty(PropertyName = "directoryName")]
        public string DirectoryName { get; set; }

    }
}
