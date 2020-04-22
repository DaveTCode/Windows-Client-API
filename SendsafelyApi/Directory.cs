using System.Collections.Generic;
using System.Collections.ObjectModel;
using SendsafelyApi.Objects;

namespace SendsafelyApi
{
    public class Directory
    {
        public Directory()
        {
        }

        public Directory(string directoryId, string directoryName)
        {
            DirectoryId = directoryId;
            DirectoryName = directoryName;
        }

        public Directory UserDirectory { get; set; }

        public string DirectoryName { get; set; }

        public string DirectoryId { get; set; }

        public List<FileResponse> Files { get; set; }

        public Collection<DirectoryResponse> SubDirectories { get; set; }
    }
}
