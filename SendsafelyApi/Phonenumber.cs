using Newtonsoft.Json;

namespace SendsafelyApi
{
    /// <summary>
    /// An object referencing a phone number. Contains two public variables, a CountryCode and a phonenumber.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PhoneNumber
    {
        /// <summary>
        /// The phone numbers country code.
        /// </summary>
        [JsonProperty(PropertyName = "countryCode")]
        public int CountryCode { get; set; }

        /// <summary>
        /// The phone number itself.
        /// </summary>
        [JsonProperty(PropertyName = "phonenumber")]
        public string Number { get; set; }
    }
}
