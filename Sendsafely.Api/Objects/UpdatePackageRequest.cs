using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UpdatePackageRequest
    {
        [JsonProperty(PropertyName = "life")]
        public int Life { get; set; }
    }
}
