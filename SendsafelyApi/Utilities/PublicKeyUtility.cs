using SendsafelyApi.Exceptions;
using SendsafelyApi.Objects;

namespace SendsafelyApi.Utilities
{
    internal class PublicKeyUtility
    {
        private const string KeyEmail = "no-reply@sendsafely.com";

        private readonly Connection _connection;

        #region Constructors

        public PublicKeyUtility(Connection connection)
        {
            _connection = connection;
        }

        #endregion

        public PrivateKey GenerateKeyPair(string description)
        {
            var cu = new CryptUtility();
            var keyPair = cu.GenerateKeyPair(KeyEmail);

            var publicKeyId = UploadPublicKey(keyPair.PublicKey, description);

            var privateKey = new PrivateKey
            {
                ArmoredKey = keyPair.PrivateKey,
                PublicKeyID = publicKeyId
            };

            return privateKey;
        }

        public void RevokePublicKey(string publicKeyId)
        {
            var p = ConnectionStrings.Endpoints["revokePublicKey"].Clone();
            p.Path = p.Path.Replace("{publicKeyId}", publicKeyId);
            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new RevokingKeyFailedException("Failed to revoke public key: " + response.Message);
            }
        }

        public string GetKeycode(string packageId, PrivateKey privateKey)
        {
            var publicKeyId = privateKey.PublicKeyID;
            var privateKeyStr = privateKey.ArmoredKey;

            var p = ConnectionStrings.Endpoints["getKeycode"].Clone();
            p.Path = p.Path.Replace("{publicKeyId}", publicKeyId);
            p.Path = p.Path.Replace("{packageId}", packageId);
            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new GettingKeycodeFailedException("Failed to get keycode: " + response.Message);
            }

            var encryptedKeycode = response.Message;

            var cu = new CryptUtility();
            var keycode = cu.DecryptKeycode(privateKeyStr, encryptedKeycode);
            return keycode;
        }

        private string UploadPublicKey(string publicKey, string description)
        {
            var p = ConnectionStrings.Endpoints["addPublicKey"].Clone();
            var request = new AddPublicKeyRequest
            {
                PublicKey = publicKey,
                Description = description
            };
            var response = _connection.Send<AddPublicKeyResponse>(p, request);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new AddingPublicKeyFailedException("Failed to upload public key: " + response.Message);
            }

            return response.ID;
        }
    }
}
