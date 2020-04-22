using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UpdatePackageDescriptorRequest
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
    }
}
