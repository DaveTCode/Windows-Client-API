using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class ActivityLogRequest
    {
        [JsonProperty(PropertyName = "rowIndex")]
        public long RowIndex { get; set; }
    }
}
