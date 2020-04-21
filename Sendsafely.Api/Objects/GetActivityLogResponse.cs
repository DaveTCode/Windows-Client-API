using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GetActivityLogResponse : StandardResponse
    {
        [JsonProperty(PropertyName = "activityLogEntries")]
        internal List<ActivityLogEntry> ActivityLogEntries { get; set; }

        [JsonProperty(PropertyName = "dataCount")]
        internal long DataCount { get; set; }
    }
}
