using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Zhongan.DI.JsonModel;
using Zhongan.DI.Templet;
using Zhongan.DI.Util;
using Zhongan.DI.ZhonganSdkException;

namespace Zhongan.DI.Util
{
    public abstract class SignatureUtils
    {
        //默认编码字符集
        private static string DEFAULT_CHARSET = "UTF-8";

        public static string GetSignContent(IDictionary<string, string> parameters)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);

            // 第二步：把所有参数名和参数值串在一起，生成json格式
            string content = JsonConvert.SerializeObject(sortedParams);

            return content;
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="privateKeyPem"></param>
        /// <returns></returns>
        public static string RSASign(IDictionary<string, string> parameters, string privateKeyPem)
        {
            string signContent = GetSignContent(parameters);

            return RSASign(signContent, privateKeyPem);
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKeyPem"></param>
        /// <returns></returns>
        public static string RSASign(string data, string privateKeyPem)
        {
            return RSASign(data, privateKeyPem, null);
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKeyPem"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string RSASign(string data, string privateKeyPem, string charset)
        {
            RSACryptoServiceProvider rsaCsp = LoadCertificateFile(privateKeyPem);
            byte[] dataBytes = null;
            if (string.IsNullOrEmpty(charset))
            {
                dataBytes = Encoding.UTF8.GetBytes(data);
            }
            else
            {
                dataBytes = Encoding.GetEncoding(charset).GetBytes(data);
            }

            byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");

            return Convert.ToBase64String(signatureBytes);
        }

        /// <summary>
        /// 验签1
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="publicKeyPem"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static bool RSACheckV1(IDictionary<string, string> parameters, string publicKeyPem, string charset)
        {
            string sign = parameters["sign"];

            parameters.Remove("sign");
            parameters.Remove("sign_type");
            string signContent = GetSignContent(parameters);
            return RSACheckContent(signContent, sign, publicKeyPem, charset);
        }

        public static bool RSACheckV1(IDictionary<string, string> parameters, string publicKeyPem)
        {
            string sign = parameters["sign"];

            parameters.Remove("sign");
            parameters.Remove("sign_type");
            string signContent = GetSignContent(parameters);

            return RSACheckContent(signContent, sign, publicKeyPem, DEFAULT_CHARSET);
        }

        /// <summary>
        /// 验签2
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="publicKeyPem"></param>
        /// <returns></returns>
        public static bool RSACheckV2(IDictionary<string, string> parameters, string publicKeyPem)
        {
            string sign = parameters["sign"];

            parameters.Remove("sign");
            string signContent = GetSignContent(parameters);

            return RSACheckContent(signContent, sign, publicKeyPem, DEFAULT_CHARSET);
        }

        public static bool RSACheckV2(IDictionary<string, string> parameters, string publicKeyPem, string charset)
        {
            string sign = parameters["sign"];

            parameters.Remove("sign");
            string signContent = GetSignContent(parameters);

            return RSACheckContent(signContent, sign, publicKeyPem, charset);
        }

        public static bool RSACheckContent(string signContent, string sign, string publicKeyPem, string charset)
        {
            try
            {
                string sPublicKeyPEM = File.ReadAllText(publicKeyPem);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.PersistKeyInCsp = false;
                RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                if (string.IsNullOrEmpty(charset))
                {
                    charset = DEFAULT_CHARSET;
                }
                bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), sha1, Convert.FromBase64String(sign));
                return bVerifyResultOriginal;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 验签，解密
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="zaPublicKey"></param>
        /// <param name="cusPrivateKey"></param>
        /// <param name="isCheckSign"></param>
        /// <param name="isDecrypt"></param>
        /// <returns></returns>
        public static string CheckSignAndDecrypt(IDictionary<string, string> parameters, string zaPublicKey,
                                             string cusPrivateKey, bool isCheckSign, bool isDecrypt)
        {
            string charset = parameters["charset"];
            string bizContent = parameters["bizContent"];

            if (string.IsNullOrEmpty(bizContent)) parameters.Remove("bizContent");

            if (isCheckSign)
            {
                if (!RSACheckV2(parameters, zaPublicKey, charset))
                {
                    throw new ZaSdkException("验签失败:rsaParams=" + parameters);
                }
            }

            if (isDecrypt)
            {
                return RSADecrypt(bizContent, cusPrivateKey, charset);
            }

            return bizContent;
        }

        /// <summary>
        /// 加密加签
        /// </summary>
        /// <param name="bizContent"></param>
        /// <param name="zaPublicKey"></param>
        /// <param name="cusPrivateKey"></param>
        /// <param name="charset"></param>
        /// <param name="isEncrypt"></param>
        /// <param name="isSign"></param>
        /// <returns></returns>
        public static IDictionary<String, String> encryptAndSign(Dictionary<String, String> bizContent, string zaPublicKey,
                                        string cusPrivateKey, string charset, bool isEncrypt,
                                        bool isSign)
        {

            ICollection<String> bizKeys = bizContent.Keys;

            IDictionary<String, String> businessParamMap = new SortedDictionary<String, String>();
            IDictionary<String, String> newMap = new SortedDictionary<String, String>();

            BizParamTemp temp = new BizParamTemp();
            String[] templet = temp.GetPropertys;

            foreach (String key in bizContent.Keys)
            {
                if (ApiUtils.IsExistence(key, templet))
                    newMap.Add(key, bizContent[key]);
                else
                    businessParamMap.Add(key, bizContent[key]);
            }
            if (string.IsNullOrEmpty(charset))
            {
                charset = DEFAULT_CHARSET;
            }

            string content = JsonConvert.SerializeObject(businessParamMap);
            string newMapJson = JsonConvert.SerializeObject(newMap);

            if (isEncrypt)
            {// 加密
                String encrypted = RSAEncrypt(content, zaPublicKey, charset);
                newMap.Add("bizContent", encrypted);

                if (isSign)
                {//签名 
                    newMapJson = JsonConvert.SerializeObject(newMap);//添加bizContent，更新后的json
                    String sign = RSASign(newMapJson, cusPrivateKey, charset);
                    newMap.Add("sign", sign);
                }
            }
            else if (isSign)
            {// 不加密，但需要签名
                newMap.Add("bizContent", content);
                String sign = RSASign(newMapJson, cusPrivateKey, charset);
                newMap.Add("sign", sign);
            }
            else
            {// 不加密，不加签
                newMap.Add("bizContent", content);
            }
            return newMap;
        }

        /// <summary>
        /// 加密处理
        /// </summary>
        /// <param name="content"></param>
        /// <param name="publicKeyPem"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string content, string publicKeyPem, string charset)
        {
            try
            {
                string sPublicKeyPEM = File.ReadAllText(publicKeyPem);

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.PersistKeyInCsp = false;
                RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);

                if (string.IsNullOrEmpty(charset))
                {
                    charset = DEFAULT_CHARSET;
                }
                byte[] data = Encoding.GetEncoding(charset).GetBytes(content);
                int maxBlockSize = rsa.KeySize / 8 - 11; //加密块最大长度限制
                if (data.Length <= maxBlockSize)
                {
                    byte[] cipherbytes = rsa.Encrypt(data, false);
                    return Convert.ToBase64String(cipherbytes);
                }
                MemoryStream plaiStream = new MemoryStream(data);
                MemoryStream crypStream = new MemoryStream();
                Byte[] buffer = new Byte[maxBlockSize];
                int blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
                while (blockSize > 0)
                {
                    Byte[] toEncrypt = new Byte[blockSize];
                    Array.Copy(buffer, 0, toEncrypt, 0, blockSize);
                    Byte[] cryptograph = rsa.Encrypt(toEncrypt, false);
                    crypStream.Write(cryptograph, 0, cryptograph.Length);
                    blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
                }

                return Convert.ToBase64String(crypStream.ToArray(), Base64FormattingOptions.None);
            }
            catch (Exception ex)
            {
                throw new ZaSdkException("EncryptContent = " + content + ",charset = " + charset, ex);
            }
        }

        public static string RSADecrypt(string content, string privateKeyPem, string charset)
        {
            try
            {
                RSACryptoServiceProvider rsaCsp = LoadCertificateFile(privateKeyPem);
                if (string.IsNullOrEmpty(charset))
                {
                    charset = DEFAULT_CHARSET;
                }
                byte[] data = Convert.FromBase64String(content);
                int maxBlockSize = rsaCsp.KeySize / 8; //解密块最大长度限制
                if (data.Length <= maxBlockSize)
                {
                    byte[] cipherbytes = rsaCsp.Decrypt(data, false);
                    return Encoding.GetEncoding(charset).GetString(cipherbytes);
                }
                MemoryStream crypStream = new MemoryStream(data);
                MemoryStream plaiStream = new MemoryStream();
                Byte[] buffer = new Byte[maxBlockSize];
                int blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                while (blockSize > 0)
                {
                    Byte[] toDecrypt = new Byte[blockSize];
                    Array.Copy(buffer, 0, toDecrypt, 0, blockSize);
                    Byte[] cryptograph = rsaCsp.Decrypt(toDecrypt, false);
                    plaiStream.Write(cryptograph, 0, cryptograph.Length);
                    blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                }

                return Encoding.GetEncoding(charset).GetString(plaiStream.ToArray());
            }
            catch (Exception ex)
            {
                throw new ZaSdkException("DecryptContent = " + content + ",charset = " + charset, ex);
            }
        }

        private static string GetPem(string type, byte[] data)
        {
            string pem = Encoding.UTF8.GetString(data);
            string header = String.Format("-----BEGIN {0}-----", type);
            string footer = String.Format("-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length + 2;
            int end = pem.IndexOf(footer, start);
            string privatekey = pem.Substring(start, (end - start));
            return privatekey;
        }

        private static RSACryptoServiceProvider LoadCertificateFile(string privateKeyPem)
        {
            using (System.IO.FileStream fs = System.IO.File.OpenRead(privateKeyPem))
            {
                byte[] data = new byte[fs.Length];
                string res = string.Empty;
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    res = GetPem("RSA PRIVATE KEY", data);
                }
                try
                {
                    RSACryptoServiceProvider rsa = DecodePemPrivateKey(res);
                    return rsa;
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }

        /// <summary>
        /// 对prikey进行处理，C#API默认的prikey是xml格式的，所以需要进行处理
        /// </summary>
        /// <param name="pemstr"></param>
        /// <returns></returns>
        private static RSACryptoServiceProvider DecodePemPrivateKey(String pemstr)
        {
            byte[] pkcs8privatekey;
            pkcs8privatekey = Convert.FromBase64String(pemstr);
            if (pkcs8privatekey != null)
            {

                RSACryptoServiceProvider rsa = DecodePrivateKeyInfo(pkcs8privatekey);
                return rsa;
            }
            else
                return null;
        }

        /// <summary>
        /// 转换prikey
        /// </summary>
        /// <param name="pkcs8"></param>
        /// <returns></returns>
        private static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
        {
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            MemoryStream mem = new MemoryStream(pkcs8);
            int lenstream = (int)mem.Length;
            BinaryReader binr = new BinaryReader(mem);
            byte bt = 0;
            ushort twobytes = 0;

            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x02)
                    return null;

                twobytes = binr.ReadUInt16();

                if (twobytes != 0x0001)
                    return null;

                seq = binr.ReadBytes(15);
                if (!CompareBytearrays(seq, SeqOID))
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x04)
                    return null;

                bt = binr.ReadByte();
                if (bt == 0x81)
                    binr.ReadByte();
                else
                    if (bt == 0x82)
                    binr.ReadUInt16();

                byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
                RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKey(rsaprivkey);
                return rsacsp;
            }
            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }

        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;

                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                CspParameters CspParameters = new CspParameters();
                CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024, CspParameters);
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();	// data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {	//remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);		//last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }
    }
}
