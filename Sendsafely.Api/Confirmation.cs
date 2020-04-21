using System;

namespace Sendsafely.Api
{
    /// <summary>
    /// A class describing a confirmation. A confirmation is added to the recipient object every time a file is downloaded 
    /// </summary>
    public class Confirmation
    {
        /// <summary>
        /// The IP Address from where the file was downloaded.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// A time stamp from when the file was downloaded
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The file object that was downloaded
        /// </summary>
        public File File { get; set; }

        /// <summary>
        /// A flag indicating if the confirmation is for a message. If it is, the File object will be null
        /// </summary>
        public bool isMessage { get; set; }
    }
}
