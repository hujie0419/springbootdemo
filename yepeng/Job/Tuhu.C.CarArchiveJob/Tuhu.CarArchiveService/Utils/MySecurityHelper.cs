using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.CarArchiveService.Utils
{
   public static class MySecurityHelper
    {
        private const string keyVal = "encrypt.tuhuCar.vehicles";
        private const string ivVal = "tuhuCar.vehicles";
        private const string keySource = "abcdefghiABCDEFGHIJKLMNOPQRSTUVWXYZjklmnopqrstuvwxyz";
        /// <summary>  
        /// AES加密  
        /// </summary>  
        public static string AesString(this string source)
        {
            byte[] btKey = Encoding.UTF8.GetBytes(keyVal);
            byte[] btIv = Encoding.UTF8.GetBytes(ivVal);
            byte[] byteArray = Encoding.UTF8.GetBytes(source);
            string encrypt;
            Rijndael aes = Rijndael.Create();
            using (MemoryStream mStream = new MemoryStream())
            {
                using (var cStream = new CryptoStream(mStream, aes.CreateEncryptor(btKey, btIv), CryptoStreamMode.Write))
                {
                    cStream.Write(byteArray, 0, byteArray.Length);
                    cStream.FlushFinalBlock();
                    encrypt = Convert.ToBase64String(mStream.ToArray());
                }
            }
            aes.Clear();
            return encrypt;
        }

        /// <summary>  
        /// AES解密  
        /// </summary>  
        public static string UnAesString(this string source)
        {
            byte[] btKey = Encoding.UTF8.GetBytes(keyVal);
            byte[] btIv = Encoding.UTF8.GetBytes(ivVal);
            byte[] byteArray = Convert.FromBase64String(source);
            string decrypt;
            Rijndael aes = Rijndael.Create();
            using (MemoryStream mStream = new MemoryStream())
            {
                using (var  cStream = new CryptoStream(mStream, aes.CreateDecryptor(btKey, btIv), CryptoStreamMode.Write))
                {
                    cStream.Write(byteArray, 0, byteArray.Length);
                    cStream.FlushFinalBlock();
                    decrypt = Encoding.UTF8.GetString(mStream.ToArray());
                }
            }
            aes.Clear();
            return decrypt;
        }
    }
}
