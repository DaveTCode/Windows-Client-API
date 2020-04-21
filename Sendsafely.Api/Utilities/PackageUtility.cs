using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sendsafely.Api.Exceptions;
using Sendsafely.Api.Objects;

namespace Sendsafely.Api.Utilities
{
    internal class PackageUtility
    {
        private readonly Connection _connection;
        private const long SegmentSize = 2621440;

        #region Constructors

        public PackageUtility(Connection connection)
        {
            _connection = connection;
        }

        #endregion

        #region Public Functions
        public PackageInformation CreatePackage()
        {
            return CreatePackage(false, string.Empty);
        }

        public PackageInformation CreatePackage(bool isWorkspace)
        {
            return CreatePackage(true, string.Empty);
        }


        public PackageInformation CreatePackage(string email)
        {
            return CreatePackage(false, email);
        }

        public PackageInformation CreatePackage(bool isWorkspace, string email)
        {
            var p = ConnectionStrings.Endpoints["createPackage"].Clone();
            var request = new CreatePackageRequest
            {
                Vdr = isWorkspace,
                PackageUserEmail = email
            };
            var response = _connection.Send<CreatePackageResponse>(p, request);

            Logger.Log("Response: " + response.Response);
            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            // Derive keycode
            var cu = new CryptUtility();

            var packageInfo = new PackageInformation();
            Logger.Log("Adding keycode to package: " + packageInfo.PackageId);
            packageInfo.KeyCode = cu.GenerateToken();
            _connection.AddKeycode(response.PackageId, packageInfo.KeyCode);
            packageInfo = GetPackageInformation(response.PackageId);

            return packageInfo;
        }

        public string CreateDirectory(string packageId, string parentDirectoryId, string directoryName)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }
            if (parentDirectoryId == null)
            {
                throw new InvalidPackageException("Parent Directory ID can not be null");
            }

            var p = ConnectionStrings.Endpoints["createDirectory"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{directoryId}", parentDirectoryId);

            var request = new CreateDirectoryRequest
            {
                DirectoryName = directoryName
            };
            var response = _connection.Send<StandardResponse>(p, request);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            return response.Message; //returns directoryId
        }

        public void DeleteDirectory(string packageId, string directoryId)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }
            if (directoryId == null)
            {
                throw new InvalidPackageException("Directory ID can not be null");
            }

            var p = ConnectionStrings.Endpoints["deleteDirectory"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{directoryId}", directoryId);

            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public List<PackageInformation> GetActivePackages()
        {
            var p = ConnectionStrings.Endpoints["activePackages"].Clone();
            var response = _connection.Send<GetPackagesResponse>(p);

            return Convert(response.Packages);
        }

        public List<RecipientHistory> GetRecipientHistory(string recipientEmail)
        {
            var p = ConnectionStrings.Endpoints["recipientInfo"].Clone();
            p.Path = p.Path.Replace("{userEmail}", recipientEmail);
            var response = _connection.Send<RecipientHistoryResponse>(p);

            if (response.Response == APIResponse.INVALID_EMAIL)
            {
                throw new InvalidEmailException(response.Message);
            }
            else if (response.Response == APIResponse.DENIED)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
            else if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
            return Convert(response.Packages);
        }

        public List<PackageInformation> GetReceivedPackages()
        {
            var p = ConnectionStrings.Endpoints["receivedPackages"].Clone();
            var response = _connection.Send<GetPackagesResponse>(p);
            return Convert(response.Packages);
        }


        public List<PackageInformation> GetArchivedPackages()
        {
            var p = ConnectionStrings.Endpoints["archivedPackages"].Clone();
            var response = _connection.Send<GetPackagesResponse>(p);

            return Convert(response.Packages);
        }

        public PackageInformation GetPackageInformation(string packageId)
        {
            var p = ConnectionStrings.Endpoints["packageInformation"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            var response = _connection.Send<PackageInformationResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new InvalidPackageException(response.Message);
            }

            return Convert(response);
        }

        public PackageInformation GetPackageInformationFromLink(string link)
        {
            return GetPackageInformationFromLink(new Uri(link));
        }

        public PackageInformation GetPackageInformationFromLink(Uri link)
        {
            try
            {
                var packageCode = GetPackageCode(link);
                var keyCode = GetKeyCode(link);

                var pkgInfo = GetPackageInformation(packageCode);
                pkgInfo.KeyCode = keyCode;
                return pkgInfo;
            }
            catch (InvalidLinkException e)
            {
                throw new InvalidPackageException(e.Message);
            }
        }

        public bool UpdatePackageLife(string packageId, int life)
        {
            var p = ConnectionStrings.Endpoints["updatePackage"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);

            var req = new UpdatePackageRequest
            {
                Life = life
            };

            var response = _connection.Send<StandardResponse>(p, req);

            var retVal = false;
            if (response.Response == APIResponse.DENIED)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
            if (response.Response == APIResponse.SUCCESS)
            {
                retVal = true;
            }

            return retVal;
        }

        public void UpdatePackageDescriptor(string packageId, string packageDescriptor)
        {
            var p = ConnectionStrings.Endpoints["updatePackage"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);

            var req = new UpdatePackageDescriptorRequest
            {
                Label = packageDescriptor
            };

            var response = _connection.Send<StandardResponse>(p, req);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public void UpdateRecipientRole(string packageId, string recipientId, string role)
        {
            var p = ConnectionStrings.Endpoints["updateRecipient"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{recipientId}", recipientId);

            var req = new UpdateRecipientRequest
            {
                RoleName = role
            };

            var response = _connection.Send<StandardResponse>(p, req);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public Recipient GetRecipient(string packageId, string recipientId)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }

            var p = ConnectionStrings.Endpoints["getRecipient"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{recipientId}", recipientId);

            var response = _connection.Send<GetRecipientResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
            return Convert(response);
        }

        public Recipient AddRecipient(string packageId, string email)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }

            var p = ConnectionStrings.Endpoints["addRecipient"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);

            var request = new AddRecipientRequest
            {
                Email = email,
                AutoEnableSMS = false
            };
            var response = _connection.Send<AddRecipientResponse>(p, request);

            if (response.Response == APIResponse.INVALID_EMAIL || response.Response == APIResponse.DUPLICATE_EMAIL)
            {
                throw new InvalidEmailException(response.Message);
            }
            else if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            var recipient = new Recipient
            {
                NeedsApproval = response.ApprovalRequired,
                Approvers = response.Approvers,
                RecipientId = response.RecipientId,
                PhoneNumbers = response.Phonenumbers,
                RoleName = response.RoleName,
                Email = email
            };

            recipient.PhoneNumbers ??= new List<PhoneNumber>();

            recipient.Approvers ??= new List<string>();

            return recipient;
        }

        public void AddDropzoneRecipient(string email)
        {

            var p = ConnectionStrings.Endpoints["addDropzoneRecipient"].Clone();

            var request = new AddDropzoneRecipientRequest
            {
                UserEmail = email
            };
            var response = _connection.Send<StandardResponse>(p, request);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public List<string> GetDropzoneRecipients()
        {
            var p = ConnectionStrings.Endpoints["getDropzoneRecipients"].Clone();
            var response = _connection.Send<GetDropzoneRecipientsResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
            return response.RecipientEmailAddresses;
        }

        public string GetMessage(string secureLink)
        {
            if (secureLink == null)
            {
                throw new InvalidLinkException("The secure link can not be null");
            }

            // Get the package and keycode from the secure link.
            var packageCode = GetPackageCode(secureLink);
            var keyCode = GetKeyCode(secureLink);

            VerifyKeycode(keyCode);

            var cu = new CryptUtility();
            var checksum = cu.Pbkdf2(keyCode, packageCode, 1024);

            // Get the package information from the server so we can get the package ID and server secret.
            var packageInfo = GetPackageInformation(packageCode);

            // Get the encrypted message.
            var p = ConnectionStrings.Endpoints["getMessage"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageInfo.PackageId);
            p.Path = p.Path.Replace("{checksum}", checksum);
            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new InvalidLinkException("Failed to fetch the message from the server. Make sure the URL is incorrect.");
            }

            if (string.IsNullOrEmpty(response.Message))
            {
                // We have no message, return null
                return null;
            }

            // Finally, decrypt the message
            var key = CreateEncryptionKey(packageInfo.ServerSecret, keyCode);
            var passPhrase = key.ToCharArray();

            cu = new CryptUtility();
            var message = cu.DecryptMessage(response.Message, passPhrase);

            return message;
        }

        public File EncryptAndUploadFile(string packageId, string keycode, string path, ISendSafelyProgress progress, string uploadType)
        {
            return EncryptAndUploadFile(packageId, null, keycode, path, progress, uploadType);
        }

        public File EncryptAndUploadFile(string packageId, string directoryId, string keycode, string path, ISendSafelyProgress progress, string uploadType)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }

            _connection.AddKeycode(packageId, keycode);

            // Get the updated package information.
            var packageInfo = GetPackageInformation(packageId);
            packageInfo.KeyCode = keycode;
            return EncryptAndUploadFile(packageInfo, directoryId, path, progress, uploadType);
        }

        public File EncryptAndUploadFile(PackageInformation packageInfo, string directoryId, string path, ISendSafelyProgress progress, string uploadType)
        {
            if (packageInfo == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }

            VerifyKeycode(packageInfo.KeyCode);

            if (path == null || path.Equals(""))
            {
                throw new FileNotFoundException("Path can not be null or empty");
            }

            var key = CreateEncryptionKey(packageInfo.ServerSecret, packageInfo.KeyCode);
            var passPhrase = key.ToCharArray();
            var unencryptedFile = new FileInfo(path);
            var filename = unencryptedFile.Name;

            var parts = CalculateParts(unencryptedFile.Length);

            string fileId;
            Endpoint p;

            if (directoryId != null)
            {
                fileId = CreateFileId(packageInfo, directoryId, filename, unencryptedFile.Length, parts, uploadType);
                p = ConnectionStrings.Endpoints["uploadFileInDirectory"].Clone();
                p.Path = p.Path.Replace("{packageId}", packageInfo.PackageId);
                p.Path = p.Path.Replace("{fileId}", fileId);
                p.Path = p.Path.Replace("{directoryId}", directoryId);
            }
            else
            {
                fileId = CreateFileId(packageInfo, filename, unencryptedFile.Length, parts, uploadType);
                p = ConnectionStrings.Endpoints["uploadFile"].Clone();
                p.Path = p.Path.Replace("{packageId}", packageInfo.PackageId);
                p.Path = p.Path.Replace("{fileId}", fileId);
            }

            var approximateFilesize = unencryptedFile.Length + parts * 70;
            long uploadedSoFar = 0;

            var partSize = unencryptedFile.Length / parts;

            var totalFilesize = unencryptedFile.Length;
            long longDiff = 0;
            for (var i = 0; i < parts; i++)
            {
                longDiff += partSize;
            }
            var offset = totalFilesize - longDiff;

            using (var readStream = unencryptedFile.OpenRead())
            {
                for (var i = 1; i <= parts; i++)
                {
                    var segment = CreateSegment(readStream, partSize + offset);
                    EncryptAndUploadSegment(p, packageInfo, i, segment, fileId, passPhrase, uploadedSoFar, approximateFilesize, progress, uploadType);
                    uploadedSoFar += segment.Length;
                    segment.Delete();

                    // Offset is only for the first package.
                    offset = 0;
                }
            }

            var file = new File
            {
                FileId = fileId,
                FileName = filename
            };

            return file;
        }

        public void EncryptAndUploadMessage(string packageId, string keycode, string message, string applicationName)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }

            _connection.AddKeycode(packageId, keycode);

            // Get the updated package information.
            var packageInfo = GetPackageInformation(packageId);
            packageInfo.KeyCode = keycode;
            EncryptAndUploadMessage(packageInfo, message, applicationName);
        }

        public void EncryptAndUploadMessage(PackageInformation packageInfo, string message, string applicationName)
        {
            if (packageInfo == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }

            VerifyKeycode(packageInfo.KeyCode);

            message = message == null ? "" : message.Trim();

            var encryptedMessage = "";
            if (!message.Equals(""))
            {
                var key = CreateEncryptionKey(packageInfo.ServerSecret, packageInfo.KeyCode);
                var passPhrase = key.ToCharArray();

                var cu = new CryptUtility();
                encryptedMessage = cu.EncryptMessage(message, passPhrase);
            }

            var p = ConnectionStrings.Endpoints["addMessage"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageInfo.PackageId);

            var request = new AddMessageRequest
            {
                Message = encryptedMessage,
                UploadType = applicationName
            };

            var response = _connection.Send<StandardResponse>(p, request);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public FileInfo DownloadFile(string packageId, string directoryId, string fileId, string keycode, ISendSafelyProgress progress, string downloadAPI, string password)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }

            _connection.AddKeycode(packageId, keycode);

            // Get the updated package information.
            var packageInfo = GetPackageInformation(packageId);
            packageInfo.KeyCode = keycode;

            var downloadUtility = directoryId != null 
                ? new DownloadFileUtility(_connection, GetDirectory(packageId, directoryId), packageInfo, progress, downloadAPI) 
                : new DownloadFileUtility(_connection, packageInfo, progress, downloadAPI, password);

            return downloadUtility.DownloadFile(fileId);
        }

        public string FinalizePackage(string packageId, string keycode)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("PackageId can not be null");
            }

            // Add the keycode in case we don't have it
            _connection.AddKeycode(packageId, keycode);

            // Get the updated package information.
            var packageInfo = GetPackageInformation(packageId);
            return FinalizePackage(packageInfo);
        }

        public string FinalizePackage(string packageId, string keycode, bool allowReplyAll)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("PackageId can not be null");
            }
            var request = new FinalizePackageRequest
            {
                AllowReplyAll = allowReplyAll
            };
            // Add the keycode in case we don't have it
            _connection.AddKeycode(packageId, keycode);

            // Get the updated package information.
            var packageInfo = GetPackageInformation(packageId);
            return FinalizePackage(packageInfo, request);
        }

        public string FinalizePackage(PackageInformation packageInfo)
        {
            var request = new FinalizePackageRequest();
            return FinalizePackage(packageInfo, request);
        }

        public string FinalizePackage(PackageInformation packageInfo, FinalizePackageRequest request)
        {
            if (packageInfo == null)
            {
                throw new InvalidPackageException("PackageInformation can not be null");
            }

            VerifyKeycode(packageInfo.KeyCode);

            EncryptKeycodes(packageInfo);

            var p = ConnectionStrings.Endpoints["finalizePackage"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageInfo.PackageId);

            var cu = new CryptUtility();
            request.Checksum = cu.Pbkdf2(packageInfo.KeyCode, packageInfo.PackageCode, 1024);
            _connection.AddKeycode(packageInfo.PackageId, packageInfo.KeyCode);

            var response = _connection.Send<FinalizePackageResponse>(p, request);

            switch (response.Response)
            {
                case APIResponse.DENIED:
                    throw new PackageFinalizationException(response.Errors);
                case APIResponse.APPROVER_REQUIRED:
                    throw new ApproverRequiredException("Package needs an approver");
                case APIResponse.PACKAGE_NEEDS_APPROVAL:
                    // We are approved at this point. We still return an exception so the user knows that the package requires an approver.
                    throw new PackageNeedsApprovalException(response.Approvers)
                    {
                        Link = response.Message + "#keyCode=" + packageInfo.KeyCode
                    };
            }

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            return response.Message + "#keyCode=" + packageInfo.KeyCode;
        }

        public string FinalizeUndisclosedPackage(string packageId, string keycode)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("PackageId can not be null");
            }

            // Add the keycode in case we don't have it
            _connection.AddKeycode(packageId, keycode);

            // Get the updated package information.
            var packageInfo = GetPackageInformation(packageId);

            //set request object for the undisclosed package.
            var request = new FinalizePackageRequest
            {
                UndisclosedRecipients = true
            };
            return FinalizePackage(packageInfo, request);
        }

        public string FinalizeUndisclosedPackage(string packageId, string password, string keycode)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("PackageId can not be null");
            }

            // Add the keycode in case we don't have it
            _connection.AddKeycode(packageId, keycode);

            // Get the updated package information.
            var packageInfo = GetPackageInformation(packageId);

            //set request object for the undisclosed package.
            var request = new FinalizePackageRequest
            {
                UndisclosedRecipients = true,
                Password = password
            };
            return FinalizePackage(packageInfo, request);
        }

        public string FinalizeUndisclosedPackage(PackageInformation packageInfo, FinalizePackageRequest request)
        {
            if (packageInfo == null)
            {
                throw new InvalidPackageException("PackageInformation can not be null");
            }

            VerifyKeycode(packageInfo.KeyCode);

            EncryptKeycodes(packageInfo);

            var p = ConnectionStrings.Endpoints["finalizePackage"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageInfo.PackageId);

            var cu = new CryptUtility();
            request.Checksum = cu.Pbkdf2(packageInfo.KeyCode, packageInfo.PackageCode, 1024);
            _connection.AddKeycode(packageInfo.PackageId, packageInfo.KeyCode);

            var response = _connection.Send<FinalizePackageResponse>(p, request);

            switch (response.Response)
            {
                case APIResponse.DENIED:
                    throw new PackageFinalizationException(response.Errors);
                case APIResponse.APPROVER_REQUIRED:
                    throw new ApproverRequiredException("Package needs an approver");
                case APIResponse.PACKAGE_NEEDS_APPROVAL:
                {
                    // We are approved at this point. We still return an exception so the user knows that the package requires an approver.
                    throw new PackageNeedsApprovalException(response.Approvers)
                    {
                        Link = response.Message + "#keyCode=" + packageInfo.KeyCode
                    };
                }
            }

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            return response.Message + "#keyCode=" + packageInfo.KeyCode;
        }


        public void DeleteTempPackage(string packageId)
        {
            var p = ConnectionStrings.Endpoints["deleteTempPackage"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);

            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public void DeletePackage(string packageId)
        {
            var p = ConnectionStrings.Endpoints["deletePackage"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);

            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public void AddRecipientPhonenumber(string packageId, string recipientId, string phonenumber, CountryCodes.CountryCode countrycode)
        {
            var p = ConnectionStrings.Endpoints["addRecipientPhonenumber"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{recipientId}", recipientId);

            var urr = new UpdateRecipientRequest
            {
                Countrycode = countrycode.ToString(),
                PhoneNumber = phonenumber
            };

            var response = _connection.Send<StandardResponse>(p, urr);

            switch (response.Response)
            {
                case APIResponse.INVALID_INPUT:
                    throw new InvalidPhonenumberException(response.Message);
                case APIResponse.INVALID_RECIPIENT:
                    throw new InvalidRecipientException(response.Message);
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

        public void DeleteFile(string packageId, string directoryId, string fileId)
        {
            var p = ConnectionStrings.Endpoints["deleteFile"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{directoryId}", directoryId);
            p.Path = p.Path.Replace("{fileId}", fileId);
            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

        }

        public void MoveFile(string packageId, string fileId, string destinationDirectoryId)
        {
            var p = ConnectionStrings.Endpoints["moveFile"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{directoryId}", destinationDirectoryId);
            p.Path = p.Path.Replace("{fileId}", fileId);
            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

        }

        public Directory GetDirectory(string packageId, string directoryId)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }
            if (directoryId == null)
            {
                throw new InvalidPackageException("Directory ID can not be null");
            }

            var p = ConnectionStrings.Endpoints["directoryInformation"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{directoryId}", directoryId);
            var response = _connection.Send<GetDirectoryResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            return Convert(response);
        }

        public void MoveDirectory(string packageId, string sourceDirectoryId, string destinationDirectoryId)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }
            if (sourceDirectoryId == null)
            {
                throw new InvalidPackageException("Source Directory ID can not be null");
            }
            if (destinationDirectoryId == null)
            {
                throw new InvalidPackageException("Destination Directory ID can not be null");
            }

            var p = ConnectionStrings.Endpoints["moveDirectory"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{sourcedirectoryId}", sourceDirectoryId);
            p.Path = p.Path.Replace("{targetdirectoryId}", destinationDirectoryId);

            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public void RenameDirectory(string packageId, string directoryId, string directoryName)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }
            if (directoryId == null)
            {
                throw new InvalidPackageException("Directory ID can not be null");
            }

            var p = ConnectionStrings.Endpoints["renameDirectory"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{directoryId}", directoryId);

            var request = new RenameDirectoryRequest
            {
                DirectoryName = directoryName
            };

            var response = _connection.Send<StandardResponse>(p, request);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public void AddContactGroupToPackage(string packageId, string groupId)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }

            var p = ConnectionStrings.Endpoints["addContactGroupsToPackage"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{groupId}", groupId);

            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public void RemoveContactGroupFromPackage(string packageId, string groupId)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }

            var p = ConnectionStrings.Endpoints["removeContactGroupsToPackage"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{groupId}", groupId);

            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public void RemoveRecipient(string packageId, string recipientId)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }

            var p = ConnectionStrings.Endpoints["removeRecipient"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{recipientId}", recipientId);

            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public FileInformation GetFileInformation(string packageId, string directoryId, string fileId)
        {
            var p = ConnectionStrings.Endpoints["fileInformation"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            p.Path = p.Path.Replace("{directoryId}", directoryId);
            p.Path = p.Path.Replace("{fileId}", fileId);
            var response = _connection.Send<FileInformationResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            return Convert(response);
        }

        public List<ActivityLogEntry> GetActivityLog(string packageId, int rowIndex)
        {
            var p = ConnectionStrings.Endpoints["getActivityLog"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);
            var request = new ActivityLogRequest
            {
                RowIndex = rowIndex
            };
            var response = _connection.Send<GetActivityLogResponse>(p, request);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            return response.ActivityLogEntries;
        }

        public void RemoveDropzoneRecipient(string email)
        {

            var p = ConnectionStrings.Endpoints["removeDropzoneRecipient"].Clone();

            var request = new AddRemoveGroupUserRequest
            {
                UserEmail = email
            };
            var response = _connection.Send<StandardResponse>(p, request);

            if (response.Response == APIResponse.GUESTS_PROHIBITED || response.Response == APIResponse.FAIL)
            {
                throw new InvalidEmailException(response.Message);
            }
            else if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public string CreateContactGroup(string groupName)
        {

            var p = ConnectionStrings.Endpoints["createContactGroup"].Clone();

            var request = new AddContactGroupRequest
            {
                GroupName = groupName
            };
            var response = _connection.Send<AddContactGroupResponse>(p, request);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
            return response.ContactGroupId;
        }

        public string CreateContactGroup(string groupName, string isEnterpriseGroup)
        {

            var p = ConnectionStrings.Endpoints["createContactGroup"].Clone();

            var request = new AddContactGroupRequest
            {
                GroupName = groupName,
                IsEnterpriseGroup = isEnterpriseGroup
            };
            var response = _connection.Send<AddContactGroupResponse>(p, request);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
            return response.ContactGroupId;
        }

        public void DeleteContactGroup(string groupId)
        {

            var p = ConnectionStrings.Endpoints["deleteContactGroup"].Clone();
            p.Path = p.Path.Replace("{groupId}", groupId);
            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public string AddGroupUser(string groupId, string userEmail)
        {

            var p = ConnectionStrings.Endpoints["addContactGroupUser"].Clone();
            p.Path = p.Path.Replace("{groupId}", groupId);

            var request = new AddRemoveGroupUserRequest
            {
                UserEmail = userEmail
            };
            var response = _connection.Send<AddGroupResponse>(p, request);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
            return response.UserId;
        }

        public void RemoveUserFromContactGroup(string groupId, string userId)
        {

            var p = ConnectionStrings.Endpoints["removeUserFromContactGroup"].Clone();
            p.Path = p.Path.Replace("{groupId}", groupId);
            p.Path = p.Path.Replace("{userId}", userId);
            var response = _connection.Send<StandardResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }
        }

        public List<ContactGroup> GetContactGroups(bool isEnterprise)
        {
            var p = isEnterprise 
                ? ConnectionStrings.Endpoints["getEnterpriseContactGroups"].Clone() 
                : ConnectionStrings.Endpoints["getContactGroups"].Clone();

            var response = _connection.Send<GetUserGroupsResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            return response.ContactGroups;
        }

        public List<Recipient> AddRecipients(string packageId, List<string> emails)
        {
            if (packageId == null)
            {
                throw new InvalidPackageException("Package ID can not be null");
            }

            var p = ConnectionStrings.Endpoints["addRecipients"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);

            var request = new AddRecipientsRequest
            {
                Emails = emails
            };
            var response = _connection.Send<AddRecipientsResponse>(p, request);

            if (response.Response == APIResponse.LIMIT_EXCEEDED)
            {
                throw new LimitExceededException(response.Message);
            }

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            var recipients = Convert(response.Recipients);
            return recipients;
        }
        #endregion

        #region Private Functions

        private void EncryptKeycodes(PackageInformation pkgInfo)
        {
            var publicKeys = GetPublicKeys(pkgInfo.PackageId);

            var encryptedKeycodes = EncryptKeycodes(publicKeys, pkgInfo.KeyCode);
            UploadKeycodes(pkgInfo.PackageId, encryptedKeycodes);
        }

        private List<EncryptedKeycode> EncryptKeycodes(List<PublicKeyRaw> publicKeys, string keycode)
        {
            var keycodes = new List<EncryptedKeycode>();
            foreach (var publicKey in publicKeys)
            {
                keycodes.Add(Create(publicKey, keycode));
            }
            return keycodes;
        }

        internal PackageSearchResults GetOrganizationPackages(DateTime? fromDate, DateTime? toDate, string sender, PackageStatus? status, string recipient, string fileName)
        {
            var request = new GetOrganizationPackagesRequest();

            if (fromDate != null)
            {
                request.FromDate = fromDate.Value.ToString("MM/dd/yyyy");
            }
            if (toDate != null)
            {
                request.ToDate = toDate.Value.ToString("MM/dd/yyyy");
            }
            if (status != null)
            {
                request.Status = status.ToString();
            }
            request.Sender = sender;
            request.Recipient = recipient;
            request.Filename = fileName;

            var p = ConnectionStrings.Endpoints["organizationPackages"].Clone();
            var response = _connection.Send<GetOrganizationPakagesResponse>(p, request);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            return Convert(response);
        }

        private PackageSearchResults Convert(GetOrganizationPakagesResponse response)
        {
            var packages = new List<PackageInformation>(response.Packages.Count);
            packages.AddRange(response.Packages.Select(convert));

            var organizationPackages = new PackageSearchResults
            {
                Packages = packages,
                Capped = response.Capped
            };
            return organizationPackages;
        }

        private PackageInformation convert(PackageDTO obj)
        {
            var info = new PackageInformation
            {
                Approvers = obj.Approvers,
                Life = obj.Life,
                NeedsApprover = obj.NeedsApprover,
                PackageCode = obj.PackageCode,
                PackageId = obj.PackageID,
                Recipients = ConvertRecipients(obj.Recipients),
                ServerSecret = obj.ServerSecret,
                PackageOwner = obj.PackageUserName,
                PackageTimestamp = obj.PackageUpdateTimestamp,
                Files = ConvertFiles(obj.Filenames)
            };
            if (!int.TryParse(obj.PackageState, out var stateValue))
            {
                stateValue = 0;
            }
            info.Status = ConvertStateToStatus(stateValue);

            info.State = obj.PackageState;
            return info;
        }

        private static PackageStatus ConvertStateToStatus(int packageState)
        {
            if (packageState < 0)
            {
                return PackageStatus.ARCHIVED;

            }

            if (packageState == 1 || packageState == 2 || packageState == 6)
            {
                return PackageStatus.EXPIRED;
            }

            return PackageStatus.ACTIVE;
        }

        private static List<File> ConvertFiles(List<string> files)
        {
            return files.Select(fileName => new File {FileName = fileName}).ToList();
        }

        private static List<Recipient> ConvertRecipients(List<string> recipients)
        {
            var recipientList = new List<Recipient>();
            if (recipients != null)
            {
                //Recipients are null for incoming (received) items
                recipientList.AddRange(recipients.Select(email => new Recipient {Email = email}));
            }
            return recipientList;
        }

        private List<PublicKeyRaw> GetPublicKeys(string packageId)
        {
            var p = ConnectionStrings.Endpoints["getPublicKeys"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageId);

            var response = _connection.Send<GetPublicKeysResponse>(p);

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            return response.PublicKeys;
        }

        private static EncryptedKeycode Create(PublicKeyRaw publicKey, string keycode)
        {
            var cu = new CryptUtility();
            var encryptedKeycode = new EncryptedKeycode
            {
                ID = publicKey.ID,
                Keycode = cu.EncryptKeycode(publicKey.Key, keycode)
            };
            return encryptedKeycode;
        }

        private void UploadKeycodes(string packageId, List<EncryptedKeycode> encryptedKeycodes)
        {
            foreach (var keycode in encryptedKeycodes)
            {
                var p = ConnectionStrings.Endpoints["addEncryptedKeycode"].Clone();
                p.Path = p.Path.Replace("{packageId}", packageId);
                p.Path = p.Path.Replace("{publicKeyId}", keycode.ID);

                var request = new UploadKeycodeRequest
                {
                    Keycode = keycode.Keycode
                };

                _connection.Send<StandardResponse>(p, request);
            }
        }

        private List<PackageInformation> Convert(List<PackageDTO> rawPackages)
        {
            return rawPackages.Select(Convert).ToList();
        }


        private static Directory Convert(GetDirectoryResponse response)
        {
            return new Directory(response.DirectoryId, response.DirectoryName)
            {
                SubDirectories = response.SubDirectories,
                Files = response.Files
            };
        }

        private Recipient Convert(AddRecipientResponse response)
        {
            var rec = new Recipient
            {
                Email = response.Email,
                NeedsApproval = response.ApprovalRequired,
                RecipientId = response.RecipientId,
                RoleName = response.RoleName
            };
            return rec;
        }

        private List<RecipientHistory> Convert(List<RecipientHistoryDTO> rawPackages)
        {
            return rawPackages.Select(Convert).ToList();
        }

        private PackageInformation Convert(PackageDTO raw)
        {
            var packageInfo = new PackageInformation
            {
                PackageId = raw.PackageID,
                PackageCode = raw.PackageCode,
                ServerSecret = raw.ServerSecret,
                Approvers = raw.Approvers,
                PackageTimestamp = raw.PackageUpdateTimestamp,
                PackageParentId = raw.PackageParentId,

                PackageOwner = raw.PackageUserName,

                Files = new List<File>()
            };
            
            foreach (var f in raw.Filenames.Select(fileName => new File
            {
                FileName = fileName
            }))
            {
                packageInfo.Files.Add(f);
            }

            packageInfo.NeedsApprover = raw.NeedsApprover;

            packageInfo.Recipients = new List<Recipient>();

            if (raw.Recipients != null)
            {
                //Recipients are null for incoming (received) items
                foreach (var r in raw.Recipients.Select(email => new Recipient
                {
                    Email = email
                }))
                {
                    packageInfo.Recipients.Add(r);
                }
            }

            packageInfo.Life = raw.Life;
            try
            {
                packageInfo.KeyCode = _connection.GetKeycode(raw.PackageID);
            }
            catch (InvalidPackageException)
            {
                packageInfo.KeyCode = null;
            }

            return packageInfo;
        }

        private PackageInformation Convert(PackageInformationResponse raw)
        {
            var packageInfo = new PackageInformation
            {
                PackageId = raw.PackageID,
                PackageCode = raw.PackageCode,
                ServerSecret = raw.ServerSecret,
                Approvers = raw.Approvers,
                PackageOwner = raw.PackageSender,
                PackageTimestamp = raw.PackageTimestamp,
                Files = raw.Files ?? new List<File>(),
                RootDirectoryId = raw.RootDirectoryId,

                NeedsApprover = raw.NeedsApprover,
                Recipients = raw.Recipients ?? new List<Recipient>(),
                Life = raw.Life,
                PackageDescriptor = raw.Label,
                IsWorkspace = raw.IsVDR,
                State = raw.State,
                ContactGroups = raw.ContactGroups
            };
            var stateValue = (int)Enum.Parse(typeof(PackageState), raw.State, true);
            packageInfo.Status = ConvertStateToStatus(stateValue);
            packageInfo.PackageParentId = raw.PackageParentId;

            try
            {
                packageInfo.KeyCode = _connection.GetKeycode(raw.PackageID);
            }
            catch (InvalidPackageException)
            {
                packageInfo.KeyCode = null;
            }

            return packageInfo;
        }

        private FileInformation Convert(FileInformationResponse raw)
        {
            return raw.File;
        }

        private RecipientHistory Convert(RecipientHistoryDTO raw)
        {
            var recipientHistory = new RecipientHistory
            {
                PackageCode = raw.PackageCode,
                PackageContainsMessage = raw.PackageContainsMessage,
                PackageID = raw.PackageID,
                PackageLife = raw.PackageLife,
                PackageOS = raw.PackageOS,
                PackageRecipientResponse = raw.PackageRecipientResponse,
                PackageState = raw.PackageState,
                PackageStateColor = raw.PackageStateColor,
                PackageStateStr = raw.PackageStateStr,
                PackageUpdateTimestampStr = raw.PackageUpdateTimestampStr,
                PackageUserId = raw.PackageUserId,
                PackageUserName = raw.PackageUserName,
                Files = raw.Files ?? new List<string>()
            };
            return recipientHistory;
        }

        private string GetKeyCode(string secureLink)
        {
            var myUri = new Uri(secureLink);
            return GetKeyCode(myUri);
        }

        private static string GetKeyCode(Uri secureLink)
        {
            var fragment = secureLink.Fragment;

            var keyCode = fragment.Substring(fragment.IndexOf("=", StringComparison.Ordinal) + 1);

            if (keyCode == null)
            {
                throw new InvalidLinkException("The keycode could not be found in the Secure Link");
            }

            return keyCode;
        }

        private static FileInfo CreateSegment(FileStream inputStream, long bytesToRead)
        {
            var segment = new FileInfo(Path.GetTempFileName());

            long readBytes = 0;
            using var outStream = segment.OpenWrite();
            var buf = new byte[1 << 16];
            int len;
            var bufferBytesToRead = Math.Min(buf.Length, (int)bytesToRead);
            while ((len = inputStream.Read(buf, 0, bufferBytesToRead)) > 0)
            {
                outStream.Write(buf, 0, len);

                readBytes += len;
                bufferBytesToRead = Math.Min((int)(bytesToRead - readBytes), buf.Length);
            }

            return segment;
        }

        private string CreateFileId(PackageInformation packageInfo, string filename, long filesize, int parts, string uploadType)
        {
            return CreateFileId(packageInfo, null, filename, filesize, parts, uploadType);
        }

        private string CreateFileId(PackageInformation packageInfo, string directoryId, string filename, long filesize, int parts, string uploadType)
        {
            var p = ConnectionStrings.Endpoints["createFileId"].Clone();
            p.Path = p.Path.Replace("{packageId}", packageInfo.PackageId);

            var request = new CreateFileIdRequest
            {
                Filename = filename,
                Parts = parts,
                Filesize = filesize,
                DirectoryId = directoryId,

                UploadType = uploadType
            };
            var response = _connection.Send<StandardResponse>(p, request);

            if (response.Response == APIResponse.DENIED || response.Response == APIResponse.FAIL)
            {
                throw new FileUploadException(response.Message);
            }

            if (response.Response != APIResponse.SUCCESS)
            {
                throw new ActionFailedException(response.Response.ToString(), response.Message);
            }

            return response.Message;
        }

        private static int CalculateParts(long filesize)
        {
            if (filesize == 0)
            {
                return 1;
            }

            var parts = (int)((filesize + SegmentSize - 1) / SegmentSize);

            Logger.Log("We need " + parts + " parts");
            return parts;
        }

        private void EncryptAndUploadSegment(Endpoint p, PackageInformation packageInfo, int partIndex, FileInfo unencryptedSegment, string filename, char[] passPhrase, long uploadedSoFar, long filesize, ISendSafelyProgress progress, string uploadType)
        {
            // Create a temp file to store the encrypted data in.
            var encryptedData = new FileInfo(Path.GetTempFileName());

            var cu = new CryptUtility();
            cu.EncryptFile(encryptedData, unencryptedSegment, filename, passPhrase, progress);

            var fu = new FileUploader(_connection, p, progress);
            //Logger.Log("File length: " + encryptedData.Length);

            _connection.AddKeycode(packageInfo.PackageId, packageInfo.KeyCode);

            var requestData = new UploadFileRequest
            {
                UploadType = uploadType,
                FilePart = partIndex
            };

            //StandardResponse response = fu.upload(encryptedData, filename, signature, encryptedData.Length, uploadType);
            fu.Upload(encryptedData, requestData, unencryptedSegment.Name, uploadedSoFar, filesize);
            encryptedData.Delete();
        }

        private static string CreateEncryptionKey(string serverSecret, string keyCode)
        {
            return serverSecret + keyCode;
        }

        private static void VerifyKeycode(string keycode)
        {
            if (keycode == null)
            {
                throw new MissingKeyCodeException("Keycode is null");
            }

            if (keycode.Length == 0)
            {
                throw new MissingKeyCodeException("Keycode is empty");
            }

            if (keycode.Length < 32)
            {
                throw new MissingKeyCodeException("Keycode is to short");
            }
        }

        private static string GetPackageCode(string link)
        {
            return GetParameterFromUrl(new Uri(link), "packageCode");
        }

        private static string GetPackageCode(Uri link)
        {
            return GetParameterFromUrl(link, "packageCode");
        }

        private static string GetParameterFromUrl(Uri link, string parameter)
        {
            var packageCode = System.Web.HttpUtility.ParseQueryString(link.Query).Get(parameter);

            if (packageCode == null)
            {
                throw new InvalidLinkException("Package code could not be found");
            }

            return packageCode;
        }

        private static List<Recipient> Convert(List<RecipientResponse> recipients)
        {
            return recipients.Select(Convert).ToList();
        }

        private static Recipient Convert(RecipientResponse item)
        {
            var recipient = new Recipient
            {
                Email = item.Email,
                RecipientId = item.RecipientId,
                NeedsApproval = item.NeedsApproval
            };
            return recipient;
        }

        private static Recipient Convert(GetRecipientResponse response)
        {
            var recipient = new Recipient
            {
                Email = response.Email,
                NeedsApproval = response.ApprovalRequired,
                RecipientId = response.RecipientId
            };
            return recipient;
        }
        #endregion
    }
}
