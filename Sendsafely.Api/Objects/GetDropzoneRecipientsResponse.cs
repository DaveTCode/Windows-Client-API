using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GetDropzoneRecipientsResponse : StandardResponse
    {
        [JsonProperty(PropertyName = "recipientEmailAddresses")]
        public List<string> RecipientEmailAddresses { get; set; }
    }


}
