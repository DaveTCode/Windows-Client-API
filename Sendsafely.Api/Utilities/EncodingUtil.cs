using System;
using Org.BouncyCastle.Utilities.Encoders;

namespace Sendsafely.Api.Utilities
{
    internal class EncodingUtil
    {

        public static string Base64Encode(byte[] data)
        {
            var dataStr = Convert.ToBase64String(data);

            // Make it web safe
            dataStr = dataStr.Replace('+', '-');
            dataStr = dataStr.Replace('/', '_');
            // Remove '=' since it's not URL safe
            dataStr = dataStr.Replace("=", "");

            return dataStr;
        }

        public static string HexEncode(byte[] value)
        {
            return System.Text.Encoding.UTF8.GetString(Hex.Encode(value));
        }

    }
}
