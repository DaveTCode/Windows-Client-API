using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UpdateRecipientRequest
    {
        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "countrycode")]
        public string Countrycode { get; set; }

        [JsonProperty(PropertyName = "roleName")]
        public string RoleName { get; set; }
    }
}
