using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RHYANetwork.UtaitePlayer.CryptoModule
{
    public class KeyGen
    {
        // Base key
        private readonly string BASE_KEY = "R2sXSqjvXpBChzsT";




        /// <summary>
        /// Random key 생성
        /// </summary>
        /// <returns></returns>
        public string getRandomKey()
        {
            try
            {
                // UUID 생성
                Guid guid1 = Guid.NewGuid();
                Guid guid2 = Guid.NewGuid();

                // String 합치기
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(guid1.ToString());
                stringBuilder.Append(BASE_KEY);
                stringBuilder.Append(guid2.ToString());

                // MD5 SHA 구하기
                return new MD5Crypto().getMD5Hash(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
