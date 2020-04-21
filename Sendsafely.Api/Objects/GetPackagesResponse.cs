using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GetPackagesResponse
    {
        [JsonProperty(PropertyName = "response")]
        internal APIResponse Response { get; set; }

        [JsonProperty(PropertyName = "packages")]
        public List<PackageDTO> Packages { get; set; }
    }


}
