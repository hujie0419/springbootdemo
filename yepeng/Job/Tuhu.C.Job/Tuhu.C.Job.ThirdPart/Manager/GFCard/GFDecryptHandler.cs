using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.ThirdPart.Utils;

namespace Tuhu.C.Job.ThirdPart.Manager
{
    /// <summary>
    /// 解密文件
    /// </summary>
    public class GFDecryptHandler
    {
        private static readonly string _gFCardSecretkey = ConfigurationManager.AppSettings["GFCardSecretkey"];
        public bool DecryptFile(string sourceFilePath, string targetFilePath)
        {
            var result = -1;

            try
            {
                var encode = Encoding.UTF8;
                var key = HexStringToByteArray(_gFCardSecretkey);
                var keylen = key.Length;
                var sourceFilePathBytes = encode.GetBytes(sourceFilePath).Concat(new byte[] { 0x00 }).ToArray();
                var targetFilePathBytes = encode.GetBytes(targetFilePath).Concat(new byte[] { 0x00 }).ToArray();
                result = LibcryptAPIsmJni.CryptFile(3, key, keylen, sourceFilePathBytes, targetFilePathBytes, 1); /*加解密*/
                if (result != 0)
                {
                    JobLogger.GFLogger.Warn($"广发联名卡数据解密失败,解密返回Code:{result}");
                }               
            }
            catch (Exception ex)
            {
                JobLogger.GFLogger.Error(ex);
            }

            return result == 0;
        }
        public static byte[] HexStringToByteArray(String HexString)
        {
            int NumberChars = HexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}
