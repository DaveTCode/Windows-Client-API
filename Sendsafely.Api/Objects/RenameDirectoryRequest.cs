using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class RenameDirectoryRequest
    {
        [JsonProperty(PropertyName = "directoryName")]
        public string DirectoryName { get; set; }
    }
}
