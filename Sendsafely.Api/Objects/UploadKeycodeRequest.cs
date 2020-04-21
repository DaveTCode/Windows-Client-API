using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UploadKeycodeRequest
    {
        [JsonProperty(PropertyName = "keycode")]
        public string Keycode { get; set; }
    }
}
