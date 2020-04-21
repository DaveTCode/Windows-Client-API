using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sendsafely.Api.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class RecipientHistoryDTO
    {
        [JsonProperty(PropertyName = "packageId")]
        internal string PackageID { get; set; }

        [JsonProperty(PropertyName = "packageUserName")]
        internal string PackageUserName { get; set; }

        [JsonProperty(PropertyName = "packageUserId")]
        internal string PackageUserId { get; set; }

        [JsonProperty(PropertyName = "packageState")]
        internal int PackageState { get; set; }

        [JsonProperty(PropertyName = "packageStateStr")]
        internal string PackageStateStr { get; set; }

        [JsonProperty(PropertyName = "packageStateColor")]
        internal string PackageStateColor { get; set; }

        [JsonProperty(PropertyName = "packageLife")]
        internal int PackageLife { get; set; }

        [JsonProperty(PropertyName = "packageUpdateTimestampStr")]
        internal string PackageUpdateTimestampStr { get; set; }

        [JsonProperty(PropertyName = "packageCode")]
        internal string PackageCode { get; set; }

        [JsonProperty(PropertyName = "packageOS")]
        internal string PackageOS { get; set; }

        [JsonProperty(PropertyName = "packageRecipientResponse")]
        internal Recipient PackageRecipientResponse { get; set; }

        [JsonProperty(PropertyName = "filenames")]
        internal List<string> Files { get; set; }

        [JsonProperty(PropertyName = "packageContainsMessage")]
        internal bool PackageContainsMessage { get; set; }
    }
}
