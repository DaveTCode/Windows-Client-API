using System;

namespace Sendsafely.Api.Objects
{
    public class ActivityLogEntry
    {
        public string ActivityLogId { get; set; }

        public DateTime Timestamp { get; set; }

        public string TimestampStr { get; set; }

        public string IpAddress { get; set; }

        public string PackageId { get; set; }

        public string TargetId { get; set; }

        public string ActionDescription { get; set; }

        public string Action { get; set; }

        public UserDTO User { get; set; }
    }
}
