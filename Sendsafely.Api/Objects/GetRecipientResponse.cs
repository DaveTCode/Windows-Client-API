using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GetRecipientResponse : StandardResponse
    {
        [JsonProperty(PropertyName = "publicKeys")]
        public List<PublicKeyRaw> PublicKeys { get; set; }

        [JsonProperty(PropertyName = "isPackageOwner")]
        public bool IsPackageOwner { get; set; }

        [JsonProperty(PropertyName = "smsAuth")]
        public bool SmsAuth { get; set; }

        [JsonProperty(PropertyName = "fullName")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "approvers")]
        public List<string> Approvers { get; set; }

        [JsonProperty(PropertyName = "checkForPublicKeys")]
        public bool CheckForPublicKeys { get; set; }

        [JsonProperty(PropertyName = "recipientId")]
        public string RecipientId { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "approvalRequired")]
        public bool ApprovalRequired { get; set; }

        [JsonProperty(PropertyName = "phonenumbers")]
        public List<PhoneNumber> Phonenumbers { get; set; }

        [JsonProperty(PropertyName = "autoEnabledNumber")]
        public string AutoEnabledNumber { get; set; }
    }
}
