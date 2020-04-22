using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using SendsafelyApi.Exceptions;
using SendsafelyApi.Objects;

namespace SendsafelyApi.Utilities
{
    internal class FileUploader
    {
        private readonly Endpoint _p;
        private readonly ISendSafelyProgress _progress;
        private readonly Connection _connection;
        private const string Crlf = "\r\n";
        private const string Charset = "UTF-8";
        private const int BufferSize = 1024;
        private const int UploadRetryAttempts = 5;
        private StandardResponse _response;
        private readonly string _boundary;

        public FileUploader(Connection connection, Endpoint p, ISendSafelyProgress progress)
        {
            _p = p;
            _progress = progress;
            _connection = connection;
            _boundary = DateTime.Now.Ticks.ToString();
        }

        //public StandardResponse upload(FileInfo rawFile, String filename, String signature, long fileSize, String uploadType)
        public StandardResponse Upload(FileInfo rawFile, UploadFileRequest requestData, string filename, long uploadedSoFar, long fullFillSize)
        {
            using var fileStream = rawFile.OpenRead();
            using var progressStream = new ProgressStream(fileStream, _progress, "Uploading", fullFillSize, uploadedSoFar);
            var uploadedBytes = UploadSegment(progressStream, requestData, filename, rawFile.Length);

            return _response;
        }

        #region Private Functions

        private long UploadSegment(ProgressStream fileStream, UploadFileRequest requestData, string filename, long segmentSize)
        {
            Logger.Log("Uploading file with size: " + segmentSize);
            long uploadedBytes = 0;
            var responseStr = "";

            var notFinished = true;
            var tryCounter = 0;
            while (notFinished && tryCounter < UploadRetryAttempts)
            {
                HttpWebRequest req = null;
                try
                {
                    req = _connection.GetRequestforFileUpload(_p, _boundary, requestData);
                    req.KeepAlive = false;

                    var suffix = "--" + _boundary + "--" + Crlf;
                    var content = "";
                    content += GetParam("requestData", ConvertToJson(requestData), _boundary);

                    long contentLength = content.Length;
                    contentLength += suffix.Length;
                    contentLength += GetFileSegmentStart(filename).Length;
                    contentLength += segmentSize;
                    contentLength += Crlf.Length;
                    req.ContentLength = contentLength;

                    using (var dataStream = req.GetRequestStream())
                    {
                        Write(content, dataStream);

                        uploadedBytes = SendBinary(fileStream, filename, dataStream, _boundary);

                        Write(suffix, dataStream);
                        dataStream.Flush();
                        notFinished = false;
                    }

                    responseStr = GetResponse(req);
                }
                catch (Exception e)
                {
                    // If an exception was thrown that means an IOException occured. If so we retry a couple of times.
                    req?.Abort();
                    tryCounter++;
                    uploadedBytes = 0;
                }
            }

            if (notFinished)
            {
                throw new FileUploadException("Multiple exceptions thrown when uploading.");
            }

            _response = JsonConvert.DeserializeObject<StandardResponse>(responseStr);

            if (_response == null)
            {
                throw new ActionFailedException("NULL_RESPONSE", "The server response could not be parsed");
            }

            if (_response.Response == APIResponse.AUTHENTICATION_FAILED)
            {
                throw new InvalidCredentialsException(_response.Message);
            }
            else if (_response.Response == APIResponse.UNKNOWN_PACKAGE)
            {
                throw new InvalidPackageException(_response.Message);
            }
            else if (_response.Response == APIResponse.INVALID_EMAIL)
            {
                throw new InvalidEmailException(_response.Message);
            }

            return uploadedBytes;
        }

        private string GetResponse(WebRequest req)
        {
            var response = "";
            using var objStream = req.GetResponse().GetResponseStream();
            using var objReader = new StreamReader(objStream);
            var sLine = "";

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    response += sLine;
                    //Logger.Log(sLine);
                }
            }

            return response;
        }

        private long SendBinary(ProgressStream input, string filename, Stream output, string boundary)
        {
            Write(GetFileSegmentStart(filename), output);
            output.Flush();

            var tmp = new byte[BufferSize];
            int l;

            // Wrap the stream to get some progress updates..
            long uploadedBytes = 0;

            while ((l = input.Read(tmp, 0, BufferSize)) != 0)
            {
                output.Write(tmp, 0, l);
                output.Flush();
                uploadedBytes += l;
            }

            Write(Crlf, output);
            input.Flush();

            return uploadedBytes;
        }

        private string GetFileSegmentStart(string filename)
        {
            var content = "";
            content += "--" + _boundary + Crlf;
            content += "Content-Disposition: form-data; name=\"textFile\"; filename=\"" + filename + "\"" + Crlf;
            content += "Content-Type: text/plain; charset=" + Charset + Crlf;
            content += Crlf;
            return content;
        }

        private string GetParam(string key, string value, string boundary)
        {
            var content = "";
            content += "--" + boundary + Crlf;
            content += "content-disposition: form-data; name=\"" + key + "\"" + Crlf;
            content += "content-type: text/plain; charset=" + Charset + Crlf;
            content += Crlf;
            content += value + Crlf;

            return content;
        }

        private void Write(string str, Stream stream)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(str);
            stream.Write(bytes, 0, bytes.Length);
        }

        private string ConvertToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        #endregion

    }
}
