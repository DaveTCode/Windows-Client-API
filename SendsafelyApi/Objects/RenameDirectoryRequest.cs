using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class RenameDirectoryRequest
    {
        [JsonProperty(PropertyName = "directoryName")]
        public string DirectoryName { get; set; }
    }
}
