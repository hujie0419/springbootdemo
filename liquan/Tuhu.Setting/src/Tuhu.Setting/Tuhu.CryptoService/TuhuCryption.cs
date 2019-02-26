using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.CryptoService
{
    public static class TuhuCryption
    {
        private static readonly Encoding CryEncoder = Encoding.UTF8;
        private const string DateFormat = "yyMMdd";
        private const string PrivateKey = "BwIAAACkAABSU0EyAAQAAAEAAQB/xJrBrgNhsHc0jY2DTdojn+dOEFKjebMBCaOcdhsI18rO1l8a28BUY2OtqaTjGH7BcTSl5DrdUzUMjU4H0ZJZpuCp4DulhnU6m6NcXzcb8mYKcsgSnsKb5JtYMETPxSvGmeL2GAplQ1WFpTxz6OzVINJMrLR1m6bY5cZxP42fuJ1G5RdlscNdiw0Y31lc2uOKIZMPrCii7cpWq4DibTyZv+T0OroUnkVlAEziHwUEZCZ+hHvub/X7gil8wmN7Pb7L/uwC6YTNGSbV0W/5tvhqkWHW++RrpeEWObevCjyXL3pRuDpHnjrBPcSBJ4s/JZUPK1DpSK8D2Z1I/TooBnH4nWEM3iPWQZwEh4Sn1EVanPGN8+pgjhXigI4nL+2aAa3NnIAvALhSobsmm1JbVzR6F5rau7nWZQVvVXgkX7VAFLWVsap4LiiXTI5+z/47dXWqtvSL2b+wpNdd5zQ8IUsXT7OG6Fx89Xo1+8Vwr8Vd3ZSUhG8d7EYg56xvsQasvALKVU9AARIQNaFNlLQAUlard3tsSYZB7Q5YXLT0VoLtrEeZ5uWEUGMsb4CxirBwcqs5MWBbt+VAsUPbYSaOUa6riczsWv3YB3TdDCAOf2dRbECAAsTjVSYNbfHfwT3QQRTaxTFXVUid70dBDjLt0yBWUtI5BDEhI5TPNt/hzWyam3OD8VKaolt6kXt1E1jhsRxlwszqPJCSAEI1U/bzT4XglzYB8kJFJTpn05eecpi0nebEU16zCFv3MCpZzvcbOhk=";
        private const string PublicKey = "BgIAAACkAABSU0ExAAQAAAEAAQB/xJrBrgNhsHc0jY2DTdojn+dOEFKjebMBCaOcdhsI18rO1l8a28BUY2OtqaTjGH7BcTSl5DrdUzUMjU4H0ZJZpuCp4DulhnU6m6NcXzcb8mYKcsgSnsKb5JtYMETPxSvGmeL2GAplQ1WFpTxz6OzVINJMrLR1m6bY5cZxP42fuA==";

        #region RSA 的密钥产生
        public static bool GetBlobKey(out string priKey, out string pubKey)
        {
            priKey = null;
            pubKey = null;

            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                pubKey = Convert.ToBase64String(rsa.ExportCspBlob(false));
                priKey = Convert.ToBase64String(rsa.ExportCspBlob(true));
                rsa.Clear();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 加密

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static string Encrypt(this string plaintext)
        {
            return plaintext.Encrypt(PublicKey);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plaintext">待加密文本</param>
        /// <param name="blobPublicKey">RSA Blob 公钥</param>
        /// <returns></returns>
        public static string Encrypt(this string plaintext, string blobPublicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(Convert.FromBase64String(blobPublicKey));

            Byte[] PlaintextData = CryEncoder.GetBytes(plaintext);
            int MaxBlockSize = rsa.KeySize / 8 - 11;    //加密块最大长度限制

            if (PlaintextData.Length <= MaxBlockSize)
                return Convert.ToBase64String(CryEncoder.GetBytes(DateTime.Now.ToString(DateFormat))) + Convert.ToBase64String(rsa.Encrypt(PlaintextData, false));

            using (MemoryStream PlaiStream = new MemoryStream(PlaintextData))
            using (MemoryStream CrypStream = new MemoryStream())
            {
                Byte[] Buffer = new Byte[MaxBlockSize];
                int BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);

                while (BlockSize > 0)
                {
                    Byte[] ToEncrypt = new Byte[BlockSize];
                    Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);

                    Byte[] Cryptograph = rsa.Encrypt(ToEncrypt, false);
                    CrypStream.Write(Cryptograph, 0, Cryptograph.Length);

                    BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                }

                return Convert.ToBase64String(CryEncoder.GetBytes(DateTime.Now.ToString(DateFormat))) + Convert.ToBase64String(CrypStream.ToArray(), Base64FormattingOptions.None);
            }
        }
        #endregion

        #region 解密
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        public static string Decrypt(this string ciphertext)
        {
            return ciphertext.Decrypt(PrivateKey);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <param name="blobPrivateKey"></param>
        /// <returns></returns>
        public static string Decrypt(this string ciphertext, string blobPrivateKey = PrivateKey)
        {
            if (string.IsNullOrWhiteSpace(ciphertext) || ciphertext.Length < 9 || !IsBase64Date(ciphertext.Substring(0, 8)))
                return ciphertext;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] keybuff = Convert.FromBase64String(blobPrivateKey);
            rsa.ImportCspBlob(keybuff);


            Byte[] CiphertextData = Convert.FromBase64String(ciphertext.Substring(8));
            int MaxBlockSize = rsa.KeySize / 8;    //解密块最大长度限制

            if (CiphertextData.Length <= MaxBlockSize)
                return CryEncoder.GetString(rsa.Decrypt(CiphertextData, false));

            using (MemoryStream CrypStream = new MemoryStream(CiphertextData))
            using (MemoryStream PlaiStream = new MemoryStream())
            {
                Byte[] Buffer = new Byte[MaxBlockSize];
                int BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);

                while (BlockSize > 0)
                {
                    Byte[] ToDecrypt = new Byte[BlockSize];
                    Array.Copy(Buffer, 0, ToDecrypt, 0, BlockSize);

                    Byte[] Plaintext = rsa.Decrypt(ToDecrypt, false);
                    PlaiStream.Write(Plaintext, 0, Plaintext.Length);

                    BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);
                }

                return CryEncoder.GetString(PlaiStream.ToArray());
            }
        }
        #endregion

        private static bool IsBase64Date(string input)
        {
            try
            {
                DateTime t;
                return DateTime.TryParseExact(Encoding.UTF8.GetString(Convert.FromBase64String(input)), DateFormat, new CultureInfo("zh-cn"), System.Globalization.DateTimeStyles.AssumeLocal, out t);
            }
            catch
            {
                return false;
            }
        }
    }
}
