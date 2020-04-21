using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class RecipientHistoryResponse
    {
        [JsonProperty(PropertyName = "response")]
        internal APIResponse Response { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "packages")]
        public List<RecipientHistoryDTO> Packages { get; set; }
    }
}
