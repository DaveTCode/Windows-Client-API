namespace Sendsafely.Api
{
    /// <summary>
    /// This object will contain information about a package. Once a package is created this object will be returned. 
    /// If it is passed along when adding recipients and files the object will be updated accordingly.
    /// </summary>
    public class User
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string ClientKey { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool AllowPublicKey { get; set; }

        public int PackageLife { get; set; }
    }
}
