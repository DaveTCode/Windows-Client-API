using System.Configuration;
using System.Net;
using SendsafelyApi.Exceptions;
using SendsafelyApi.Objects;

namespace SendsafelyApi.Utilities
{
    internal class StartupUtility
    {
        private readonly Connection _connection;

        public StartupUtility(string host, string privateKey, string apiKey)
        {
            _connection = new Connection(host, privateKey, apiKey);
        }

        public StartupUtility(Connection connection)
        {
            _connection = connection;
        }

        public StartupUtility(string host, string privateKey, string apiKey, WebProxy proxy)
        {
            if (string.IsNullOrEmpty(privateKey))
            {
                throw new InvalidCredentialsException("The private key can't be null or empty");
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidCredentialsException("The API key can't be null or empty");
            }

            _connection = new Connection(host, privateKey, apiKey, proxy);
        }

        public Version VerifyVersion()
        {
            var p = ConnectionStrings.Endpoints["version"].Clone();
            var version = ConfigurationManager.AppSettings["version"] ?? "0.3";
            p.Path = p.Path.Replace("{version}", version);

            var response = _connection.Send<VersionResponse>(p);
            return response.Version;
        }

        public string VerifyCredentials()
        {
            var p = ConnectionStrings.Endpoints["verifyCredentials"].Clone();

            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new InvalidCredentialsException("Failed to verify the credentials.");
            }

            return response.Message;
        }

        public User GetUserInformation()
        {
            var p = ConnectionStrings.Endpoints["userInformation"].Clone();

            var response = _connection.Send<UserInformationResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            return Convert(response);
        }

        public Connection GetConnectionObject()
        {
            return _connection;
        }

        private User Convert(UserInformationResponse response)
        {
            var user = new User
            {
                AllowPublicKey = response.AllowPublicKey,
                ClientKey = response.ClientKey,
                Email = response.Email,
                FirstName = response.FirstName,
                Id = response.Id,
                LastName = response.LastName,
                PackageLife = response.PackageLife
            };
            return user;
        }
    }
}
