using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AddRecipientsResponse : StandardResponse
    {
        [JsonProperty(PropertyName = "recipients")]
        public List<RecipientResponse> Recipients { get; set; }
    }
}
