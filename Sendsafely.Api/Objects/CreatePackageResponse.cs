using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CreatePackageResponse
    {
        [JsonProperty(PropertyName = "response")]
        internal APIResponse Response { get; set; }

        [JsonProperty(PropertyName = "packageId")]
        public string PackageId { get; set; }

        [JsonProperty(PropertyName = "serverSecret")]
        public string ServerSecret { get; set; }

        [JsonProperty(PropertyName = "packageCode")]
        public string PackageCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "rootDirectoryId")]
        public string RootDirectoryId { get; set; }
    }


}
