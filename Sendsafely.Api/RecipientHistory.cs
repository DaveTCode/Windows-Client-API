using System.Collections.Generic;

namespace Sendsafely.Api
{
    public class RecipientHistory
    {
        public string PackageID { get; set; }

        public string PackageUserName { get; set; }

        public string PackageUserId { get; set; }

        public int PackageState { get; set; }

        public string PackageStateStr { get; set; }

        public string PackageStateColor { get; set; }

        public int PackageLife { get; set; }

        public string PackageUpdateTimestampStr { get; set; }

        public string PackageCode { get; set; }

        public string PackageOS { get; set; }

        public Recipient PackageRecipientResponse { get; set; }

        public List<string> Files { get; set; }

        public bool PackageContainsMessage { get; set; }
    }
}
