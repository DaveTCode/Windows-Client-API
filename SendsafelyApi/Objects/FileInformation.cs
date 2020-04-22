using System;
using System.Collections.Generic;

namespace SendsafelyApi.Objects
{
    public class FileInformation
    {
        public string FileName { get; set; }

        public string FileId { get; set; }

        public string FileSize { get; set; }

        public string CreatedByEmail { get; set; }

        public string CreatedById { get; set; }

        public DateTime Uploaded { get; set; }

        public string UploadedStr { get; set; }

        public List<FileInformation> OldVersions { get; set; }

        public int FileVersion { get; set; }

        public int FileParts { get; set; }
    }
}
