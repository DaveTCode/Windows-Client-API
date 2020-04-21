﻿using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CreatePackageRequest
    {
        [JsonProperty(PropertyName = "packageUserEmail")]
        public string PackageUserEmail { get; set; }

        [JsonProperty(PropertyName = "vdr")]
        public bool Vdr { get; set; }
    }
}
