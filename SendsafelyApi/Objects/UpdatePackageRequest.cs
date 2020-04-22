using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UpdatePackageRequest
    {
        [JsonProperty(PropertyName = "life")]
        public int Life { get; set; }
    }
}
