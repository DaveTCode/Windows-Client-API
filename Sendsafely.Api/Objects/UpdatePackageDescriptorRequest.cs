using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UpdatePackageDescriptorRequest
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
    }
}
