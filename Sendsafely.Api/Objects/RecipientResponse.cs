using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class RecipientResponse : StandardResponse
    {
        [JsonProperty(PropertyName = "recipientId")]
        public string RecipientId { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "needsApproval")]
        public bool NeedsApproval { get; set; }

        [JsonProperty(PropertyName = "phonenumbers")]
        public List<PhoneNumber> Phonenumbers { get; set; }

        [JsonProperty(PropertyName = "confirmations")]
        public List<ConfirmationResponse> Confirmations { get; set; }

        [JsonProperty(PropertyName = "autoEnabledNumber")]
        public string AutoEnabledNumber { get; set; }
    }
}
