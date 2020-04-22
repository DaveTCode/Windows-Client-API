namespace SendsafelyApi.Objects
{
    internal enum HTTPMethod { PUT, POST, DELETE, GET };

    internal class Endpoint
    {
        public Endpoint(string path, HTTPMethod method, string contentType)
        {
            Path = path;
            Method = method;
            ContentType = contentType;
        }

        public string Path { get; set; }

        public string ContentType { get; set; }

        internal HTTPMethod Method { get; set; }

        internal Endpoint Clone()
        {
            var p = new Endpoint(Path, Method, ContentType);
            return p;
        }
    }
}
