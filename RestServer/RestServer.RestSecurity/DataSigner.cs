using RestServer.Core.Extensions;
using RestServer.RestSecurity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.RestSecurity
{
    public class DataSigner : IDataSigner
    {
        public string GetDecodedData(string encodedData)
        {
            return Encoding.Unicode.GetString(Convert.FromBase64String(encodedData));
        }

        public string GetEncodedData(string data)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(data));
        }

        public string GetSignature(string data, string key)
        {
            var dataBytes = Encoding.Unicode.GetBytes(data);
            if (null == dataBytes || dataBytes.Length <= 0)
            {
                throw new ArgumentNullException(nameof(dataBytes));
            }

            var keyBytes = Encoding.Default.GetBytes(key);
            if (null == keyBytes || keyBytes.Length <= 0)
            {
                throw new ArgumentNullException(nameof(keyBytes));
            }

            using (var hmac = new HMACSHA256(keyBytes))
            {
                return Convert.ToBase64String(hmac.ComputeHash(dataBytes));
            }
        }

        public bool IsValidSignature(string data, string key, string signature)
        {
            var dataBytes = Encoding.Unicode.GetBytes(data);
            if (null == dataBytes || dataBytes.Length <= 0)
            {
                throw new ArgumentNullException(nameof(dataBytes));
            }

            var keyBytes = Encoding.Default.GetBytes(key);
            if (null == keyBytes || keyBytes.Length <= 0)
            {
                throw new ArgumentNullException(nameof(keyBytes));
            }

            var signatureBytes = Convert.FromBase64String(signature);
            if (null == signatureBytes || signatureBytes.Length <= 0)
            {
                throw new ArgumentNullException(nameof(signatureBytes));
            }

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var expectedHash = hmac.ComputeHash(dataBytes);
                if (expectedHash.Length != signatureBytes.Length)
                {
                    return false;
                }

                for(int i = 0; i < signatureBytes.Length; i++)
                {
                    if(signatureBytes[i] != expectedHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
