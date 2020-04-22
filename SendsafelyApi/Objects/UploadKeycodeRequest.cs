using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UploadKeycodeRequest
    {
        [JsonProperty(PropertyName = "keycode")]
        public string Keycode { get; set; }
    }
}
    }
}
