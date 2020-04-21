using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AddRecipientsRequest
    {
        [JsonProperty(PropertyName = "emails")]
        public List<string> Emails { get; set; }
    }
}
