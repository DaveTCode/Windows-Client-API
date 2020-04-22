using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DirectoryResponse
    {
        [JsonProperty(PropertyName = "directoryId")]
        internal string DirectoryId { get; set; }

        [JsonProperty(PropertyName = "name")]
        internal string Name { get; set; }

        [JsonProperty(PropertyName = "created")]
        internal DateTime Created { get; set; }

        [JsonProperty(PropertyName = "subDirectories")]
        internal ICollection<DirectoryResponse> SubDirectories { get; set; } = new Collection<DirectoryResponse>();
    }
}
