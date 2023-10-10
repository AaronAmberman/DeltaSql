using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace DeltaSql.Services
{
    /*
     * If this were an actual product for sale more work would go into the encryption and decryption here but
     * seeing as how this is not a paid for product (MIT license) and it is not widely known or used a more 
     * simple encryption and decryption mechanism was used. This can be updated if needed. Good enough for now.
     */

    internal class CryptographyService : ICryptographyService
    {
        public string Decrypt(string encrypted)
        {
            try
            {
                Aes aes = Aes.Create();
                aes.IV = new byte[] { 11, 21, 31, 41, 51, 61, 71, 81, 91, 101, 111, 121, 131, 141, 151, 161 };
                aes.Key = new byte[] { 12, 22, 32, 42, 52, 62, 72, 82, 92, 102, 112, 122, 132, 142, 152, 162, 171, 181, 191, 201, 211, 221, 231, 241, 251, 252, 253, 254, 255, 30, 40, 50 };
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform transform = aes.CreateDecryptor();

                byte[] data = Convert.FromBase64String(encrypted);
                byte[] unencrypted = transform.TransformFinalBlock(data, 0, data.Length);

                string str = Encoding.UTF8.GetString(unencrypted);

                return str;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Decryption failure: {ex}");

                return string.Empty;
            }
        }

        public string Encrypt(string unencrypted)
        {
            try
            {
                Aes aes = Aes.Create();
                aes.IV = new byte[] { 11, 21, 31, 41, 51, 61, 71, 81, 91, 101, 111, 121, 131, 141, 151, 161 };
                aes.Key = new byte[] { 12, 22, 32, 42, 52, 62, 72, 82, 92, 102, 112, 122, 132, 142, 152, 162, 171, 181, 191, 201, 211, 221, 231, 241, 251, 252, 253, 254, 255, 30, 40, 50 };
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform transform = aes.CreateEncryptor();

                byte[] data = Encoding.UTF8.GetBytes(unencrypted);
                byte[] encrypted = transform.TransformFinalBlock(data, 0, data.Length);

                string str = Convert.ToBase64String(encrypted);

                return str;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Encryption failure: {ex}");

                return string.Empty;
            }
        }
    }
}
