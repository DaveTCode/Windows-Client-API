﻿using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UserDTO
    {
        [JsonProperty(PropertyName = "userEmail")]
        internal string UserEmail { get; set; }

        [JsonProperty(PropertyName = "userId")]
        internal string UserId { get; set; }
    }
}
