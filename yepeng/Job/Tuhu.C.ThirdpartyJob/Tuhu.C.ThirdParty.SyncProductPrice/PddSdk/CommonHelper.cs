using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace PddSdk
{
    public class CommonHelper
    {

        public static long ToTimestamp(DateTime value)
        {
            TimeSpan ts = value - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        public static SortedDictionary<string, object> FromXml(string xmlInputStr)
        {
            SortedDictionary<string, object> mValues = new SortedDictionary<string, object>();
            if (string.IsNullOrEmpty(xmlInputStr))
            {
                //throw new TuhuException("将空的xml串转换为TransactionData不合法!");
                return mValues;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlInputStr);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                mValues[xe.Name] = xe.InnerText;//获取xml的键值对到TransactionData内部的数据中
            }
            return mValues;
        }
        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="preStr">待加密字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符串</returns>
        public static string GetMd5(string preStr, Encoding encoding)
        {
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(encoding.GetBytes(preStr));
            md5.Clear();
            StringBuilder md5Result = new StringBuilder();
            foreach (byte b in bytes)
                md5Result.Append(b.ToString("X2"));
            return md5Result.ToString();
        }
    }
}
