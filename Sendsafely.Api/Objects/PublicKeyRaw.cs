using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    /// <summary>
    /// An object referencing a phone number. Contains two public variables, a CountryCode and a phonenumber.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PublicKeyRaw
    {
        /// <summary>
        /// The phone numbers country code.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        /// <summary>
        /// The phone number itself.
        /// </summary>
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }
    }
}
