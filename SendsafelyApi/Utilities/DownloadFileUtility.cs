using System;
using System.IO;
using System.Linq;
using SendsafelyApi.Exceptions;
using SendsafelyApi.Objects;

namespace SendsafelyApi.Utilities
{
    internal class DownloadFileUtility
    {
        private readonly PackageInformation _pkgInfo;
        private readonly Directory _directoryInfo;
        private readonly ISendSafelyProgress _progress;
        private readonly Connection _connection;
        private readonly string _downloadApi;
        private readonly int _bufferSize = 1024;
        private readonly string _password;

        public DownloadFileUtility(Connection connection, PackageInformation pkgInfo, ISendSafelyProgress progress, string downloadApi, string password)
        {
            _pkgInfo = pkgInfo;
            _progress = progress;
            _connection = connection;
            _downloadApi = downloadApi;
            _password = password;
        }

        public DownloadFileUtility(Connection connection, Directory directory, PackageInformation pkgInfo, ISendSafelyProgress progress, string downloadApi)
        {
            _pkgInfo = pkgInfo;
            _progress = progress;
            _connection = connection;
            _downloadApi = downloadApi;
            _directoryInfo = directory;
        }

        public FileInfo DownloadFile(string fileId)
        {
            var fileToDownload = FindFile(fileId);

            var newFile = CreateTempFile(fileToDownload);

            var p = CreateEndpoint(_pkgInfo, fileId);
            using var decryptedFileStream = newFile.OpenWrite();
            for (var i = 1; i <= fileToDownload.Parts; i++)
            {
                var tmpFile = CreateTempFile();
                using (var segmentStream = tmpFile.OpenWrite())
                {
                    using var progressStream = new ProgressStream(segmentStream, _progress, "Downloading", fileToDownload.FileSize, 0);
                    DownloadSegment(progressStream, p, i);
                }

                using (var segmentStream = tmpFile.OpenRead())
                {
                    DecryptFile(segmentStream, decryptedFileStream);
                }
            }

            return newFile;
        }

        private Endpoint CreateEndpoint(PackageInformation pkgInfo, string fileId)
        {
            Endpoint p;
            if (_directoryInfo != null)
            {
                p = ConnectionStrings.Endpoints["downloadFileFromDirectory"].Clone();
                p.Path = p.Path.Replace("{packageId}", pkgInfo.PackageId);
                p.Path = p.Path.Replace("{fileId}", fileId);
                p.Path = p.Path.Replace("{directoryId}", _directoryInfo.DirectoryId);
            }
            else
            {
                p = ConnectionStrings.Endpoints["downloadFile"].Clone();
                p.Path = p.Path.Replace("{packageId}", pkgInfo.PackageId);
                p.Path = p.Path.Replace("{fileId}", fileId);
            }

            return p;
        }

        private void DecryptFile(Stream encryptedFile, Stream decryptedFile)
        {
            var cu = new CryptUtility();
            cu.DecryptFile(decryptedFile, encryptedFile, GetDecryptionKey());
        }

        private void DownloadSegment(Stream progressStream, Endpoint p, int part)
        {
            var request = new DownloadFileRequest { Api = _downloadApi, Checksum = CreateChecksum(), Part = part };
            if (_password != null)
            {
                request.Password = _password;
            }

            using var objStream = _connection.CallServer(p, request);
            using var objReader = new StreamReader(objStream);
            var tmp = new byte[_bufferSize];
            int l;

            while ((l = objStream.Read(tmp, 0, _bufferSize)) != 0)
            {
                progressStream.Write(tmp, 0, l);
            }
        }

        private char[] GetDecryptionKey()
        {
            var keyString = _pkgInfo.ServerSecret + _pkgInfo.KeyCode;
            return keyString.ToCharArray();
        }

        private string CreateChecksum()
        {
            var cu = new CryptUtility();
            return cu.Pbkdf2(_pkgInfo.KeyCode, _pkgInfo.PackageCode, 1024);
        }

        private File FindFile(string fileId)
        {
            foreach (var f in _pkgInfo.Files.Where(f => f.FileId.Equals(fileId)))
            {
                return f;
            }

            if (_directoryInfo != null)
            {
                foreach (var f in _directoryInfo.Files.Where(f => f.FileId.Equals(fileId)))
                {
                    return new File(f.FileId, f.FileName, f.FileSize, f.Parts);
                }
            }
            throw new FileDownloadException("Failed to find the file");
        }

        private FileInfo CreateTempFile(File file)
        {
            return CreateTempFile(Guid.NewGuid().ToString());
        }

        private FileInfo CreateTempFile()
        {
            return CreateTempFile(Guid.NewGuid().ToString());
        }

        private FileInfo CreateTempFile(string fileName)
        {
            return new FileInfo(Path.GetTempPath() + fileName);
        }
    }
}
