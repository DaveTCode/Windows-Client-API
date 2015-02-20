using System;
using System.Collections.Generic;
using System.Text;

namespace SendSafely
{
    /// <summary>
    /// A class describing a confirmation. A confirmation is added to the recipient object every time a file is downloaded 
    /// </summary>
    public class Confirmation
    {
        private String ipAddress;
        private DateTime timestamp;
        private File file;

        /// <summary>
        /// The IP Address from where the file was downloaded.
        /// </summary>
        public String IPAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        /// <summary>
        /// A time stamp from when the file was downloaded
        /// </summary>
        public DateTime TimeStamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        /// <summary>
        /// The file object that was downloaded
        /// </summary>
        public File File
        {
            get { return file; }
            set { file = value; }
        }
    }
}