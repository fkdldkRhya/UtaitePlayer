using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RHYANetwork.UtaitePlayer.CryptoModule
{
    public class MD5Crypto
    {
        /// <summary>
        /// MD5 암호화 합수
        /// </summary>
        /// <param name="input">암호화 데이터</param>
        /// <returns></returns>
        public string getMD5Hash(string input)
        {
            var md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            return sBuilder.ToString();
        }
    }
}
