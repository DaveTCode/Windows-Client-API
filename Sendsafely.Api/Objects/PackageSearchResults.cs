using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PackageSearchResults
    {
        [JsonProperty(PropertyName = "packages")]
        public List<PackageInformation> Packages { get; set; }

        [JsonProperty(PropertyName = "capped")]
        public bool Capped { get; set; }
    }
}