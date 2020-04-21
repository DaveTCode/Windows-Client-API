namespace Sendsafely.Api
{
    /// <summary>
    /// A class describing a sendsafely file.
    /// </summary>
    public class File
    {
        public File()
        {
        }

        public File(string fileId, string fileName, long fileSize, int parts)
        {
            FileId = fileId;
            FileName = fileName;
            FileSize = fileSize;
            Parts = parts;
        }

        /// <summary>
        /// The file name of the given file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The file ID of the given file.
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// The file size of the given file.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// The number of parts this file is divided into.
        /// </summary>
        public int Parts { get; set; }
    }
}
