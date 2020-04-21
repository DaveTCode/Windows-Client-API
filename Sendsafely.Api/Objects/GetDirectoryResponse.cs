using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GetDirectoryResponse : StandardResponse
    {
        [JsonProperty(PropertyName = "directory")]
        internal Directory Directory { get; set; }

        [JsonProperty(PropertyName = "directoryName")]
        internal string DirectoryName { get; set; }

        [JsonProperty(PropertyName = "directoryId")]
        internal string DirectoryId { get; set; }

        [JsonProperty(PropertyName = "files")]
        internal List<FileResponse> Files { get; set; }

        [JsonProperty(PropertyName = "subdirectories")]
        internal Collection<DirectoryResponse> SubDirectories { get; set; }
    }
}
