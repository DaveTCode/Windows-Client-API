using System.Collections.Generic;
using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    internal class GetOrganizationPakagesResponse : StandardResponse
    {
        [JsonProperty(PropertyName = "packages")]
        public List<PackageDTO> Packages { get; set; }

        [JsonProperty(PropertyName = "capped")]
        public bool Capped { get; set; }
    }
}
