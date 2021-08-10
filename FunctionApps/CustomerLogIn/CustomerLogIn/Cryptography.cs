using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CustomerLogIn
{
    // from http://www.saipanyam.net/2010/03/encrypt-query-strings.html
    public static class Cryptography
    {
        #region Private variables and Constants
        private const string ENCRYPTION_KEY = "Wx0Te1rc0pA";
        // Generated from Pass Phrase  "xxxxxxxx12012010" or some pass phrase that you can remember from
        // https://www.bigbiz.com/genkey.html Key Generator Site

        private const string QUERY_PARTS_DELIMITOR = "&";
        private const string QUERY_PARAMS_DELIMITOR = "=";

        // The salt value used to strengthen the encryption.
        private readonly static byte[] SALT = Encoding.ASCII.GetBytes(ENCRYPTION_KEY);
        private readonly static byte[] key;
        private readonly static byte[] iv;
        private static readonly Rfc2898DeriveBytes keyGenerator;

        #endregion

        #region Constructor

        static Cryptography()
        {
            //We create the Rfc2898DerveBytes once as
            //Rfc2898DeriveBytes uses a pseudo-random number generator based on HMACSHA1. 
            //When calling GetBytes it initializes a new instance of HMAC which takes some time.
            //Subsequent calls to GetBytes does not need to do this initialization.
            keyGenerator = new Rfc2898DeriveBytes(ENCRYPTION_KEY, SALT);
            key = keyGenerator.GetBytes(32);
            //Generate Initialization Vector - This will be less expensive as we have already intialized Rfc2898DeriveBytes
            iv = keyGenerator.GetBytes(16);
        }

        #endregion


        #region Encrypt/Decrypt Methods


        // Encrypts any string using the Rijndael algorithm.

        // The string to encrypt.
        // The name of the query string paramater
        // A Base64 encrypted string.
        public static string Encrypt(string inputText, string queryStringParam)
        {
            // Create a new RijndaelManaged cipher for the symmetric algorithm from the key and iv
            RijndaelManaged rijndaelCipher = new RijndaelManaged { Key = key, IV = iv };

            byte[] plainText = Encoding.Unicode.GetBytes(inputText);

            using (ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainText, 0, plainText.Length);
                        cryptoStream.FlushFinalBlock();
                        return "?" + queryStringParam + "=" + Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }


        public static string Encrypt(string inputText)
        {
            // Create a new RijndaelManaged cipher for the symmetric algorithm from the key and iv
            RijndaelManaged rijndaelCipher = new RijndaelManaged { Key = key, IV = iv };

            byte[] plainText = Encoding.Unicode.GetBytes(inputText);

            using (ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainText, 0, plainText.Length);
                        cryptoStream.FlushFinalBlock();
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }


        // Decrypts a previously encrypted string.

        // The encrypted string to decrypt.
        // A decrypted string.
        public static string Decrypt(string inputText)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            byte[] encryptedData = Convert.FromBase64String(inputText);

            using (ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(key, iv))
            {
                using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] plainText = new byte[encryptedData.Length];
                        int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
                        return Encoding.Unicode.GetString(plainText, 0, decryptedCount);
                    }
                }
            }
        }

        #endregion


    }
}
