using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using SendsafelyApi.Exceptions;
using SendsafelyApi.Objects;
using InvalidKeyException = SendsafelyApi.Exceptions.InvalidKeyException;

namespace SendsafelyApi.Utilities
{
    internal class CryptUtility
    {
        public string GenerateToken()
        {
            var randomBytes = new byte[32];
            var prng = new RNGCryptoServiceProvider();
            prng.GetBytes(randomBytes);

            return EncodingUtil.Base64Encode(randomBytes);
        }

        public void EncryptFile(FileInfo encryptedFile, FileInfo inputFile, string filename, char[] passPhrase, ISendSafelyProgress progress)
        {
            using var outStream = encryptedFile.OpenWrite();
            var cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, true);
            cPk.AddMethod(passPhrase, HashAlgorithmTag.Sha1);
            using var cOut = cPk.Open(outStream, new byte[1 << 16]);
            WriteFileToLiteralData(cOut, PgpLiteralData.Binary, inputFile, filename, inputFile.Length);
        }

        public void DecryptFile(Stream outStream, Stream inputStream, char[] passPhrase)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);

            var pgpF = new PgpObjectFactory(inputStream);
            PgpEncryptedDataList enc;
            var o = pgpF.NextPgpObject();

            //
            // the first object might be a PGP marker packet.
            //
            if (o is PgpEncryptedDataList list)
            {
                enc = list;
            }
            else
            {
                enc = (PgpEncryptedDataList)pgpF.NextPgpObject();
            }

            var pbe = (PgpPbeEncryptedData)enc[0];

            var clear = pbe.GetDataStream(passPhrase);

            var pgpFact = new PgpObjectFactory(clear);

            var ld = (PgpLiteralData)pgpFact.NextPgpObject();

            var unc = ld.GetInputStream();

            var buf = new byte[1 << 16];
            int len;
            while ((len = unc.Read(buf, 0, buf.Length)) > 0)
            {
                outStream.Write(buf, 0, len);
            }

            // Finally verify the integrity
            if (pbe.IsIntegrityProtected())
            {
                if (!pbe.Verify())
                {
                    throw new MessageVerificationException("Failed to verify the message. It might have been modified in transit.");
                }
            }
        }

        public string DecryptMessage(string encryptedMessage, char[] passPhrase)
        {
            // Remove the Base64 encoding
            var rawMessage = Convert.FromBase64String(encryptedMessage);

            Stream inputStream = new MemoryStream(rawMessage);

            inputStream = PgpUtilities.GetDecoderStream(inputStream);

            var pgpF = new PgpObjectFactory(inputStream);
            PgpEncryptedDataList enc;
            var o = pgpF.NextPgpObject();

            //
            // the first object might be a PGP marker packet.
            //
            if (o is PgpEncryptedDataList list)
            {
                enc = list;
            }
            else
            {
                enc = (PgpEncryptedDataList)pgpF.NextPgpObject();
            }

            var pbe = (PgpPbeEncryptedData)enc[0];

            var clear = pbe.GetDataStream(passPhrase);

            var pgpFact = new PgpObjectFactory(clear);

            var ld = (PgpLiteralData)pgpFact.NextPgpObject();

            var unc = ld.GetInputStream();

            string message;
            using (var reader = new StreamReader(unc, Encoding.UTF8))
            {
                message = reader.ReadToEnd();
            }

            // Finally verify the integrity
            if (pbe.IsIntegrityProtected())
            {
                if (!pbe.Verify())
                {
                    throw new MessageVerificationException("Failed to verify the message. It might have been modified in transit.");
                }
            }

            return message;
        }

        public string EncryptMessage(string unencryptedMessage, char[] passPhrase)
        {
            // Convert the input to a byte array. We expect the string to be UTF-8 encoded
            var unencryptedByteArray = Encoding.UTF8.GetBytes(unencryptedMessage);

            var lData = new PgpLiteralDataGenerator();

            // Write the data to a literal
            MemoryStream bOut;
            using (bOut = new MemoryStream())
            {
                using var pOut = lData.Open(bOut, PgpLiteralData.Binary, PgpLiteralData.Console, unencryptedByteArray.Length, DateTime.Now);
                pOut.Write(unencryptedByteArray, 0, unencryptedByteArray.Length);
            }
            lData.Close();

            var cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, true);
            cPk.AddMethod(passPhrase, HashAlgorithmTag.Sha1);

            var bytes = bOut.ToArray();

            MemoryStream encOut;
            using (encOut = new MemoryStream())
            {
                using var cOut = cPk.Open(encOut, bytes.Length);
                cOut.Write(bytes, 0, bytes.Length);
            }

            return Convert.ToBase64String(encOut.ToArray());
        }

        public string EncryptKeycode(string publicKeyStr, string unencryptedKeycode)
        {
            var unencryptedByteArray = Encoding.ASCII.GetBytes(unencryptedKeycode);
            var decodedPublicKey = Encoding.ASCII.GetBytes(publicKeyStr);

            PgpPublicKey key = null;

            var decodedStream = PgpUtilities.GetDecoderStream(new MemoryStream(decodedPublicKey));
            var pubRings = new PgpPublicKeyRingBundle(decodedStream);
            foreach (PgpPublicKeyRing pgpPub in pubRings.GetKeyRings())
            {
                foreach (PgpPublicKey publicKey in pgpPub.GetPublicKeys())
                {
                    if (publicKey.IsEncryptionKey)
                    {
                        key = publicKey;
                        break;
                    }
                }
            }

            if (key == null)
            {
                throw new InvalidKeyException("Can't find encryption key in key ring.");
            }

            var cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, true);
            cPk.AddMethod(key);

            var lData = new PgpLiteralDataGenerator();

            // Write the data to a literal
            MemoryStream bOut;
            using (bOut = new MemoryStream())
            {
                using var pOut = lData.Open(bOut, PgpLiteralData.Binary, PgpLiteralData.Console, unencryptedByteArray.Length, DateTime.Now);
                pOut.Write(unencryptedByteArray, 0, unencryptedByteArray.Length);
            }
            lData.Close();

            var bytes = bOut.ToArray();

            var encOut = new MemoryStream();

            using (var armoredOut = new ArmoredOutputStream(encOut))
            {
                using var cOut = cPk.Open(armoredOut, bytes.Length);
                cOut.Write(bytes, 0, bytes.Length);
            }

            return Encoding.Default.GetString(encOut.ToArray());
        }

        public string DecryptKeycode(string privateKeyStr, string encryptedKeycode)
        {
            var rawMessage = Encoding.ASCII.GetBytes(encryptedKeycode);

            Stream inputStream = new MemoryStream(rawMessage);

            inputStream = PgpUtilities.GetDecoderStream(inputStream);

            var pgpF = new PgpObjectFactory(inputStream);
            PgpEncryptedDataList enc;
            var o = pgpF.NextPgpObject();

            //
            // the first object might be a PGP marker packet.
            //
            if (o is PgpEncryptedDataList list)
            {
                enc = list;
            }
            else
            {
                enc = (PgpEncryptedDataList)pgpF.NextPgpObject();
            }

            var decodedPrivateKey = Encoding.ASCII.GetBytes(privateKeyStr);
            PgpPrivateKey key = null;

            var decodedStream = PgpUtilities.GetDecoderStream(new MemoryStream(decodedPrivateKey));
            var privRings = new PgpSecretKeyRingBundle(decodedStream);

            PgpPublicKeyEncryptedData dataObject = null;
            foreach (PgpPublicKeyEncryptedData encryptedData in enc.GetEncryptedDataObjects())
            {
                key = FindKeyById(privRings, encryptedData.KeyId);
                dataObject = encryptedData;
            }

            if (key == null)
            {
                throw new InvalidKeyException("Can't find encryption key in key ring.");
            }

            var dataStream = dataObject.GetDataStream(key);

            var pgpFact = new PgpObjectFactory(dataStream);

            var ld = (PgpLiteralData)pgpFact.NextPgpObject();

            var unc = ld.GetInputStream();

            using var reader = new StreamReader(unc, Encoding.UTF8);
            var keycode = reader.ReadToEnd();

            return keycode;
        }

        public Keypair GenerateKeyPair(string email)
        {
            var kpgen = new RsaKeyPairGenerator();
            kpgen.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 2048));

            var keyPair = kpgen.GenerateKeyPair();
            var pair = Armor(keyPair, email);

            return pair;
        }

        private Keypair Armor(AsymmetricCipherKeyPair keyPair, string email)
        {
            var privateKey = keyPair.Private;
            var publicKey = keyPair.Public;

            var memOut = new MemoryStream();
            var secretOut = new ArmoredOutputStream(memOut);

            var secretKey = new PgpSecretKey(
                PgpSignature.DefaultCertification,
                PublicKeyAlgorithmTag.RsaGeneral,
                publicKey,
                privateKey,
                DateTime.Now,
                email,
                SymmetricKeyAlgorithmTag.Null,
                null,
                null,
                null,
                new SecureRandom()
            );

            secretKey.Encode(secretOut);
            secretOut.Close();

            var memPublicOut = new MemoryStream();
            Stream publicOut = new ArmoredOutputStream(memPublicOut);

            var key = secretKey.PublicKey;

            key.Encode(publicOut);

            publicOut.Close();

            var privateKeyStr = Encoding.Default.GetString(memOut.ToArray());
            var publicKeyStr = Encoding.Default.GetString(memPublicOut.ToArray());

            var pair = new Keypair
            {
                PrivateKey = privateKeyStr,
                PublicKey = publicKeyStr
            };

            return pair;
        }

        public string Pbkdf2(string value, string salt, int iterations)
        {
            var hash = EncodingUtil.HexEncode(PBKDF2Sha256GetBytes(
                32,
                Encoding.UTF8.GetBytes(value),
                Encoding.UTF8.GetBytes(salt),
                iterations));

            return hash;
        }

        public string CreateSignature(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] hashValue;
            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
            return EncodingUtil.HexEncode(hashValue);
        }

        private byte[] PBKDF2Sha256GetBytes(int dklen, byte[] password, byte[] salt, int iterationCount)
        {
            using var hmac = new HMACSHA256(password);
            var hashLength = hmac.HashSize / 8;
            if ((hmac.HashSize & 7) != 0)
                hashLength++;
            var keyLength = dklen / hashLength;
            if (dklen > 0xFFFFFFFFL * hashLength || dklen < 0)
                throw new ArgumentOutOfRangeException("dklen");
            if (dklen % hashLength != 0)
                keyLength++;
            var extendedKey = new byte[salt.Length + 4];
            Buffer.BlockCopy(salt, 0, extendedKey, 0, salt.Length);
            using var ms = new MemoryStream();
            for (var i = 0; i < keyLength; i++)
            {
                extendedKey[salt.Length] = (byte)(((i + 1) >> 24) & 0xFF);
                extendedKey[salt.Length + 1] = (byte)(((i + 1) >> 16) & 0xFF);
                extendedKey[salt.Length + 2] = (byte)(((i + 1) >> 8) & 0xFF);
                extendedKey[salt.Length + 3] = (byte)((i + 1) & 0xFF);
                var u = hmac.ComputeHash(extendedKey);
                Array.Clear(extendedKey, salt.Length, 4);
                var f = u;
                for (var j = 1; j < iterationCount; j++)
                {
                    u = hmac.ComputeHash(u);
                    for (var k = 0; k < f.Length; k++)
                    {
                        f[k] ^= u[k];
                    }
                }
                ms.Write(f, 0, f.Length);
                Array.Clear(u, 0, u.Length);
                Array.Clear(f, 0, f.Length);
            }
            var dk = new byte[dklen];
            ms.Position = 0;
            ms.Read(dk, 0, dklen);
            ms.Position = 0;
            for (long i = 0; i < ms.Length; i++)
            {
                ms.WriteByte(0);
            }
            Array.Clear(extendedKey, 0, extendedKey.Length);
            return dk;
        }

        private PgpPrivateKey FindKeyById(PgpSecretKeyRingBundle privRings, long keyId)
        {
            var pgpSecKey = privRings.GetSecretKey(keyId);

            return pgpSecKey?.ExtractPrivateKey(null);
        }

        private void WriteFileToLiteralData(Stream pOut, char format, FileInfo dataToRead, string filename, long fileSize)
        {
            //PGPLiteralDataGenerator lData = new PGPLiteralDataGenerator();
            //OutputStream pOut = lData.open(out, fileType, filename, filesize, new Date());
            var lData = new PgpLiteralDataGenerator();
            //lData.Open(pOut, format, dataToRead);

            using var lOut = lData.Open(pOut, format, filename, fileSize, DateTime.Now);
            using Stream inputStream = dataToRead.OpenRead();
            var buf = new byte[1 << 16];
            int len;
            while ((len = inputStream.Read(buf, 0, buf.Length)) > 0)
            {
                lOut.Write(buf, 0, len);
            }
        }
    }
}
