using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Server.Utils
{
    public class HashSecurityHelper
    {
        /// <summary>
        /// 将字符串转为hash值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Sha1Encrypt(string input)
        {
            return HashEncrypt(SHA1.Create(), input, Encoding.UTF8);
        }

        /// <summary>
        /// 哈希加密算法
        /// </summary>
        /// <param name="hashAlgorithm"> 所有加密哈希算法实现均必须从中派生的基类 </param>
        /// <param name="input"> 待加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns></returns>
        private static string HashEncrypt(HashAlgorithm hashAlgorithm, string input, Encoding encoding)
        {
            var data = hashAlgorithm.ComputeHash(encoding.GetBytes(input));

            return BitConverter.ToString(data).Replace("-", "");
        }
    }
}
