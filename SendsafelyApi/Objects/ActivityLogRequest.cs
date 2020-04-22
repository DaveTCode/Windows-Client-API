using Newtonsoft.Json;

namespace SendsafelyApi.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class ActivityLogRequest
    {
        [JsonProperty(PropertyName = "rowIndex")]
        public long RowIndex { get; set; }
    }
}
