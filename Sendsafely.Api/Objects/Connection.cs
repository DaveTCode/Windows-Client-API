using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using Newtonsoft.Json;
using Sendsafely.Api.Exceptions;
using Sendsafely.Api.Utilities;

namespace Sendsafely.Api.Objects
{
    internal class Connection
    {
        private string _apiKey;
        private string _privateKey;
        private readonly WebProxy _proxy;
        private Dictionary<string, string> _keycodes;
        private const string APIKeyHeaderValue = "ss-api-key";
        private const string APITimestampHeaderValue = "ss-request-timestamp";
        private const string APISignatureHeaderValue = "ss-request-signature";

        #region Constructors

        public Connection(string host, string privateKey, string apiKey)
        {
            Initialize(host, privateKey, apiKey);
        }

        public Connection(string host, string privateKey, string apiKey, WebProxy proxy)
        {
            Initialize(host, privateKey, apiKey);
            _proxy = proxy;
        }

        public Connection(string host, WebProxy proxy)
        {
            Initialize(host, null, null);
            _proxy = proxy;
        }

        #endregion

        #region Public Functions

        public string OutlookVersion { get; set; } = null;

        public string ApiHost { get; private set; }

        public string GetKeycode(string packageId)
        {
            if (packageId == null || !_keycodes.ContainsKey(packageId))
            {
                throw new InvalidPackageException("Unknown Package Id");
            }

            return _keycodes[packageId];
        }

        public void AddKeycode(string packageId, string keycode)
        {
            if (_keycodes.ContainsKey(packageId))
            {
                _keycodes.Remove(packageId);
            }
            _keycodes.Add(packageId, keycode);
        }

        public T Send<T>(Endpoint p)
        {
            return Send<T>(p, null);
        }

        public T Send<T>(Endpoint p, object requestObj)
        {
            string respStr;
            try
            {
                respStr = SendRequest(p, requestObj);
            }
            catch (WebException)
            {
                // Server is not reachable..
                throw new ServerUnavailableException("Failed to connect to server.");
            }

            // We'll parse it as a StandardResponse first so we can check for auth failures.
            var authResp = JsonConvert.DeserializeObject<StandardResponse>(respStr);
            switch (authResp.Response)
            {
                case APIResponse.AUTHENTICATION_FAILED:
                    throw new InvalidCredentialsException(authResp.Message);
                case APIResponse.LIMIT_EXCEEDED:
                    throw new LimitExceededException(authResp.Message);
                case APIResponse.UNKNOWN_PACKAGE:
                    throw new InvalidPackageException(authResp.Message);
                default:
                {
                    // Parse the response as T.
                    var resp = JsonConvert.DeserializeObject<T>(respStr);
                    return resp;
                }
            }
        }

        public HttpWebRequest GetRequestforFileUpload(Endpoint p, string boundary, UploadFileRequest requestData)
        {
            return GetRequestforFileUpload(p, boundary, null, requestData);
        }

        public HttpWebRequest GetRequestforFileUpload(Endpoint p, string boundary, string fileId, UploadFileRequest requestData)
        {
            var url = ApiHost + p.Path;

            var wrReq = (HttpWebRequest)WebRequest.Create(url);
            wrReq.Timeout = Timeout.Infinite;
            wrReq.Headers.Add(APIKeyHeaderValue, _apiKey);

            var now = DateTime.UtcNow;
            var dateStr = now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + "+0000";
            var signature = CreateSignature(_privateKey, _apiKey, p.Path, dateStr, ConvertToJson(requestData));

            wrReq.Headers.Add(APISignatureHeaderValue, signature);
            wrReq.Headers.Add(APITimestampHeaderValue, dateStr);

            wrReq.Method = p.Method.ToString();
            wrReq.ContentType = p.ContentType + "; boundary=" + boundary;

            var userAgent = GenerateUserAgent();
            wrReq.UserAgent = userAgent;

            if (_proxy != null)
            {
                wrReq.Proxy = _proxy;
            }

            return wrReq;
        }

        public Stream CallServer(Endpoint p, object request)
        {
            var url = ApiHost + p.Path;
            var wrReq = (HttpWebRequest)WebRequest.Create(url);
            wrReq.Timeout = Timeout.Infinite;

            if (_apiKey != null)
            {
                wrReq.Headers.Add(APIKeyHeaderValue, _apiKey);
            }
            wrReq.Method = p.Method.ToString();

            wrReq.UserAgent = GenerateUserAgent();

            var requestString = "";
            if (request != null)
            {
                requestString = ConvertToJson(request);
            }

            var now = DateTime.UtcNow;
            var dateStr = now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + "+0000";

            if (_apiKey != null && _privateKey != null)
            {
                var signature = CreateSignature(_privateKey, _apiKey, p.Path, dateStr, requestString);
                wrReq.Headers.Add(APISignatureHeaderValue, signature);
            }
            wrReq.Headers.Add(APITimestampHeaderValue, dateStr);

            if (_proxy != null)
            {
                wrReq.Proxy = _proxy;
            }

            wrReq.ContentType = p.ContentType;
            if (request != null)
            {
                WriteOutput(wrReq, requestString);
            }

            var objStream = wrReq.GetResponse().GetResponseStream();
            return objStream;
        }

        #endregion

        #region Private Functions

        private void Initialize(string host, string privateKey, string apiKey)
        {
            SetTlsProtocol();
            _apiKey = apiKey;
            _privateKey = privateKey;

            host = host.TrimEnd("/".ToCharArray());

            ApiHost = host;
            _keycodes = new Dictionary<string, string>();
        }

        private string SendRequest(Endpoint p, object request)
        {
            var objStream = CallServer(p, request);

            var objReader = new StreamReader(objStream);

            var sLine = "";
            var response = "";
            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    response += sLine;
                    Logger.Log(sLine);
                }
            }

            return response;
        }

        private string GenerateUserAgent()
        {
            var userAgent = "Custom (" + Environment.OSVersion + ")";
            if (OutlookVersion != null)
            {
                userAgent += " Outlook Version " + OutlookVersion;
            }
            else
            {
                userAgent += ".NET API";
            }

            return userAgent;
        }

        private static string CreateSignature(string privateKey, string apiKey, string uri, string dateStr, string requestData)
        {
            var content = apiKey + uri + dateStr + requestData;
            Logger.Log("-" + content + "-");
            var cu = new CryptUtility();
            return cu.CreateSignature(privateKey, content);
        }

        private static string ConvertToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private static void WriteOutput(WebRequest req, string requestString)
        {
            // Serialize the object
            var reqData = System.Text.Encoding.UTF8.GetBytes(requestString);

            req.ContentLength = reqData.Length;
            var dataStream = req.GetRequestStream();
            dataStream.Write(reqData, 0, reqData.Length);
        }

        private static void SetTlsProtocol()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var attributes = assembly.GetCustomAttributes(typeof(TargetFrameworkAttribute), false);
            var version = (TargetFrameworkAttribute)attributes[0];

            try
            {
                if (Enum.TryParse("Tls11", out
            SecurityProtocolType flag))
                    ServicePointManager.SecurityProtocol |= flag;
                if (Enum.TryParse("Tls12", out flag))
                    ServicePointManager.SecurityProtocol |= flag;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to set TLS protocol for " + version.FrameworkDisplayName + ", enabled protocols " + GetEnabledSecurityProtocols(), e);
            }
        }

        private static string GetEnabledSecurityProtocols()
        {
            return ServicePointManager.SecurityProtocol.ToString();
        }

        #endregion
    }
}
