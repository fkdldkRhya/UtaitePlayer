using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RHYANetwork.UtaitePlayer.CryptoModule
{
    public class AESCrypto
    {
        // Main encrypt , decrypt key
        private readonly string MAIN_ENCRYPT_DECRYPT_KEY = "U6Gv9GQ8KxhkY3CqxSum5C6ZBL2wFPQu";
        public readonly string MAIN_ENCRYPT_DECRYPT_IV = "XW5wJ2VvyTrqxT3N";
        // AES key size
        public enum AESKeySize
        {
            SIZE_128 = 128,
            SIZE_256 = 256
        }




        /// <summary>
        /// BaseKey로 문자열 암호화
        /// </summary>
        /// <param name="value">암호화 Value</param>
        /// <returns>암호화 문자열</returns>
        public string encryptAESForBaseKey(string value)
        {
            try
            {
                return encryptAES(value, MAIN_ENCRYPT_DECRYPT_KEY, MAIN_ENCRYPT_DECRYPT_IV, AESKeySize.SIZE_256);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// BaseKey로 문자열 복호화
        /// </summary>
        /// <param name="value">복호화 Value</param>
        /// <returns>복호화 문자열</returns>
        public string decryptAESForBaseKey(string value)
        {
            try
            {
                return decryptAES(value, MAIN_ENCRYPT_DECRYPT_KEY, MAIN_ENCRYPT_DECRYPT_IV, AESKeySize.SIZE_256);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// AES 암호화
        /// </summary>
        /// <param name="plainText">대상 string</param>
        /// <param name="keyString">암호화 키</param>
        /// <param name="ivString">IV 값</param>
        /// <param name="aesKeySize">키 비트 수</param>
        /// <returns>암호화 문자열</returns>
        public string encryptAES(string plainText, string keyString, string ivString, AESKeySize aesKeySize)
        {
            try
            {
                UTF8Encoding ue = new UTF8Encoding();
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = (int)aesKeySize;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Convert.FromBase64String(keyString);
                aes.IV = ue.GetBytes(ivString);

                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Encoding.UTF8.GetBytes(plainText);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                return Convert.ToBase64String(xBuff);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// AES 복호화
        /// </summary>
        /// <param name="combinedString">대상 string</param>
        /// <param name="keyString">복호화 키</param>
        /// <param name="ivString">IV 값</param>
        /// <param name="aesKeySize">키 비트 수</param>
        /// <returns>복호화 문자열</returns>
        public string decryptAES(string combinedString, string keyString, string ivString, AESKeySize aesKeySize)
        {
            try
            {
                UTF8Encoding ue = new UTF8Encoding();
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = (int)aesKeySize;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Convert.FromBase64String(keyString);
                aes.IV = ue.GetBytes(ivString);

                var decrypt = aes.CreateDecryptor();
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Convert.FromBase64String(combinedString);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                return Encoding.UTF8.GetString(xBuff);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
