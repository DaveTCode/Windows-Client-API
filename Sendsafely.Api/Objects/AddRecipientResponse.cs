using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AddRecipientResponse : StandardResponse
    {
        [JsonProperty(PropertyName = "recipientId")]
        public string RecipientId { get; set; }

        [JsonProperty(PropertyName = "approvalRequired")]
        public bool ApprovalRequired { get; set; }

        [JsonProperty(PropertyName = "approvers")]
        public List<string> Approvers { get; set; }

        [JsonProperty(PropertyName = "phonenumbers")]
        public List<PhoneNumber> Phonenumbers { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "roleName")]
        public string RoleName { get; set; }
    }
}
