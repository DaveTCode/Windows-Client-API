﻿using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AddMessageRequest
    {

        [JsonProperty(PropertyName = "uploadType")]
        public string UploadType { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
