using System;
using Sendsafely.Api.Exceptions;
using Sendsafely.Api.Objects;

namespace Sendsafely.Api.Utilities
{
    internal class RegistrationUtility
    {
        private readonly Connection _connection;

        public RegistrationUtility(Connection connection)
        {
            _connection = connection;
        }

        public void StartRegistration(string email)
        {
            var p = ConnectionStrings.Endpoints["startRegistration"].Clone();
            var req = new StartRegistrationRequest
            {
                Email = email,
                SendPin = false
            };

            var response = _connection.Send<StandardResponse>(p, req);

            switch (response.Response)
            {
                case APIResponse.INVALID_EMAIL:
                    throw new InvalidEmailException(response.Message);
                case APIResponse.AUTH_FORBIDDEN:
                    throw new RegistrationNotAllowedException(response.Message);
                default:
                {
                    if (response.Response != APIResponse.SUCCESS)
                    {
                        throw new ActionFailedException(response.Response.ToString(), response.Message);
                    }

                    break;
                }
            }
        }

        public void StartPINRegistration(string email)
        {
            var p = ConnectionStrings.Endpoints["startRegistration"].Clone();
            var req = new StartRegistrationRequest
            {
                Email = email,
                SendPin = true
            };

            var response = _connection.Send<StandardResponse>(p, req);

            Console.WriteLine($"{response.Response} {response.Message}");
            switch (response.Response)
            {
                case APIResponse.INVALID_EMAIL:
                    throw new InvalidEmailException(response.Message);
                case APIResponse.AUTH_FORBIDDEN:
                    throw new RegistrationNotAllowedException(response.Message);
                case APIResponse.TWO_FA_ENFORCED:
                    throw new TwoFAEnforcedException(response.Message);
                default:
                {
                    if (response.Response != APIResponse.SUCCESS)
                    {
                        throw new ActionFailedException(response.Response.ToString(), response.Message);
                    }

                    break;
                }
            }
        }

        public APICredential FinishPINRegistration(string email, string pincode, string password, string secretQuestion, string answer, string firstName, string lastName, string keyDescription)
        {
            var p = ConnectionStrings.Endpoints["finishPINRegistration"].Clone();

            var req = new FinishRegistrationRequest
            {
                Email = email,
                PinCode = pincode,
                Password = password,
                Question = secretQuestion,
                Answer = answer,
                FirstName = firstName,
                LastName = lastName
            };

            var response = _connection.Send<StandardResponse>(p, req);

            switch (response.Response)
            {
                case APIResponse.PASSWORD_COMPLEXITY:
                    throw new InsufficientPasswordComplexityException(response.Message);
                case APIResponse.TOKEN_EXPIRED:
                    throw new TokenExpiredException(response.Message);
                case APIResponse.PIN_RESEND:
                    throw new PINRefreshException(response.Message);
                case APIResponse.INVALID_TOKEN:
                    throw new InvalidTokenException(response.Message);
                default:
                {
                    if (response.Response != APIResponse.SUCCESS)
                    {
                        throw new ActionFailedException(response.Response.ToString(), response.Message);
                    }

                    break;
                }
            }

            p = ConnectionStrings.Endpoints["generateKey"].Clone();
            var keyReq = new GenerateKeyRequest
            {
                Email = email,
                Password = password,
                KeyDescription = keyDescription
            };

            var response2 = _connection.Send<GenerateAPIKeyResponse>(p, keyReq);

            if (response2.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response2.Response.ToString(), response2.Message);
            }

            var key = new APICredential
            {
                APIKey = response2.APIKey,
                APISecret = response2.APISecret
            };

            return key;
        }

        public APICredential FinishRegistration(string validationLink, string password, string secretQuestion, string answer, string firstName, string lastName, string keyDescription)
        {
            var p = ConnectionStrings.Endpoints["finishRegistration"].Clone();
            p.Path = p.Path.Replace("{token}", ParseValidationLink(validationLink));

            var req = new FinishRegistrationRequest
            {
                Password = password,
                Question = secretQuestion,
                Answer = answer,
                FirstName = firstName,
                LastName = lastName
            };

            var response = _connection.Send<StandardResponse>(p, req);

            switch (response.Response)
            {
                case APIResponse.PASSWORD_COMPLEXITY:
                    throw new InsufficientPasswordComplexityException(response.Message);
                case APIResponse.TOKEN_EXPIRED:
                    throw new TokenExpiredException(response.Message);
                case APIResponse.INVALID_TOKEN:
                    throw new InvalidTokenException(response.Message);
                default:
                {
                    if (response.Response != APIResponse.SUCCESS)
                    {
                        throw new ActionFailedException(response.Response.ToString(), response.Message);
                    }

                    break;
                }
            }

            // Generate the API Key.
            var email = response.Message;

            p = ConnectionStrings.Endpoints["generateKey"].Clone();
            var keyReq = new GenerateKeyRequest
            {
                Email = email,
                Password = password,
                KeyDescription = keyDescription
            };

            var response2 = _connection.Send<GenerateAPIKeyResponse>(p, keyReq);

            if (response2.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response2.Response.ToString(), response2.Message);
            }

            var key = new APICredential
            {
                APIKey = response2.APIKey,
                APISecret = response2.APISecret
            };

            return key;
        }

        public APICredential OAuthGenerateAPIKey(string oauthToken, string keyDescription)
        {
            var p = ConnectionStrings.Endpoints["oauthRegistration"].Clone();
            p.Path = p.Path.Replace("{token}", oauthToken);

            var req = new FinishRegistrationRequest
            {
                KeyDescription = keyDescription
            };

            var response = _connection.Send<GenerateAPIKeyResponse>(p, req);

            switch (response.Response)
            {
                case APIResponse.USER_ALREADY_EXISTS:
                    throw new DuplicateUserException(response.Message);
                case APIResponse.AUTH_FORBIDDEN:
                    throw new RegistrationNotAllowedException(response.Message);
                case APIResponse.AUTHENTICATION_FAILED:
                    throw new InvalidCredentialsException(response.Message);
                default:
                {
                    if (response.Response != APIResponse.SUCCESS)
                    {
                        throw new ActionFailedException(response.Response.ToString(), response.Message);
                    }

                    break;
                }
            }

            var key = new APICredential
            {
                APIKey = response.APIKey,
                APISecret = response.APISecret
            };

            return key;
        }

        public APICredential GenerateAPIKey(string username, string password, string keyDescription)
        {
            var p = ConnectionStrings.Endpoints["generateKey"].Clone();
            var keyReq = new GenerateKeyRequest
            {
                Email = username,
                Password = password,
                KeyDescription = keyDescription
            };

            var response = _connection.Send<GenerateAPIKeyResponse>(p, keyReq);

            switch (response.Response)
            {
                case APIResponse.TWO_FA_REQUIRED:
                    throw new TwoFactorAuthException(response.Message);
                case APIResponse.AUTHENTICATION_FAILED:
                    throw new InvalidCredentialsException(response.Message);
                default:
                {
                    if (response.Response != APIResponse.SUCCESS)
                    {
                        throw new ActionFailedException(response.Response.ToString(), response.Message);
                    }

                    break;
                }
            }

            var key = new APICredential
            {
                APIKey = response.APIKey,
                APISecret = response.APISecret
            };

            return key;
        }

        public APICredential GenerateAPIKey2FA(string validationToken, string smsCode, string keyDescription)
        {
            var p = ConnectionStrings.Endpoints["generateKey2FA"].Clone();
            p.Path = p.Path.Replace("{token}", validationToken);

            var req = new GenerateKeyRequest
            {
                SMSCode = smsCode,
                KeyDescription = keyDescription
            };

            var response = _connection.Send<GenerateAPIKeyResponse>(p, req);

            switch (response.Response)
            {
                case APIResponse.AUTHENTICATION_FAILED:
                    throw new InvalidCredentialsException(response.Message);
                case APIResponse.PIN_RESEND:
                    throw new PINRefreshException(response.Message);
                default:
                {
                    if (response.Response != APIResponse.SUCCESS)
                    {
                        throw new ActionFailedException(response.Response.ToString(), response.Message);
                    }

                    break;
                }
            }

            var key = new APICredential
            {
                APIKey = response.APIKey,
                APISecret = response.APISecret
            };

            return key;
        }

        private static string ParseValidationLink(string link)
        {
            var myUri = new Uri(link);
            var validationToken = System.Web.HttpUtility.ParseQueryString(myUri.Query).Get("validationLink");

            if (validationToken == null)
            {
                throw new InvalidLinkException("The validation link could not be found");
            }

            return validationToken;
        }

    }
}
