using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Tuhu.Provisioning.Common.PinYin
{
 /**
 *设计思路：
 * 首先，我将汉字按拼音分组后建立一个字符串数组（见PyCode.codes），然后使用程序
 * 将PyCode.codes中每一个汉字通过其编码值使用散列函数：
 * 
 *     f(x) = x % PyCode.codes.Length
 *   { 
 *     g(f(x)) = pos(x)
 *     
 * 其中, pos(x)为字符x所属字符串所在的PyCode.codes的数组下标, 然后散列到同
 * PyCode.codes长度相同长度的一个散列表中PyHash.hashes）。
 * 当检索一个汉字的拼音时，首先从PyHash.hashes中获取和
 * 对应的PyCode.codes中数组下标，然后从对应字符串查找，当到要查找的字符时，字符
 * 串的前6个字符即包含了该字的拼音。
 * 
 * 此种方法的好处一是节约了存储空间，二是兼顾了查询效率。
**/
    public static class Pinyin
    {
        /// <summary>
        /// 取中文文本的拼音首字母
        /// </summary>
        /// <param name="text">编码为UTF8的文本</param>
        /// <returns>返回中文对应的拼音首字母</returns>

        public static string GetInitials(string text)
        {
            text = text.Trim();
            StringBuilder chars = new StringBuilder();
            for (var i = 0; i < text.Length; ++i)
            {
                string py = GetPinyin(text[i]);
                if (py != "") chars.Append(py[0]);
            }

            return chars.ToString().ToLower();
        }


        /// <summary>
        /// 取中文文本的拼音首字母
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="encoding">源文本的编码</param>
        /// <returns>返回encoding编码类型中文对应的拼音首字母</returns>
        public static string GetInitials(string text, Encoding encoding)
        {
            string temp = ConvertEncoding(text, encoding, Encoding.UTF8);
            return ConvertEncoding(GetInitials(temp), Encoding.UTF8, encoding);
        }



        /// <summary>
        /// 取中文文本的拼音
        /// </summary>
        /// <param name="text">编码为UTF8的文本</param>
        /// <returns>返回中文文本的拼音</returns>

        public static string GetPinyin(string text)
        {
            StringBuilder sbPinyin = new StringBuilder();
            for (var i = 0; i < text.Length; ++i)
            {
                string py = GetPinyin(text[i]);
                if (py != "") sbPinyin.Append(py);
                sbPinyin.Append(" ");
            }

            return sbPinyin.ToString().Trim();
        }

        /// <summary>
        /// 取中文文本的拼音
        /// </summary>
        /// <param name="text">编码为UTF8的文本</param>
        /// <param name="encoding">源文本的编码</param>
        /// <returns>返回encoding编码类型的中文文本的拼音</returns>
        public static string GetPinyin(string text, Encoding encoding)
        {
            string temp = ConvertEncoding(text.Trim(), encoding, Encoding.UTF8);
            return ConvertEncoding(GetPinyin(temp), Encoding.UTF8, encoding);
        }

        /// <summary>
        /// 取和拼音相同的汉字列表
        /// </summary>
        /// <param name="Pinyin">编码为UTF8的拼音</param>
        /// <returns>取拼音相同的汉字列表，如拼音“ai”将会返回“唉爱……”等</returns>
        public static string GetChineseText(string pinyin)
        {
            string key = pinyin.Trim().ToLower();

            foreach (string str in PyCode.codes)
            {
                if (str.StartsWith(key + " ") || str.StartsWith(key + ":"))
                    return str.Substring(7);
            }

            return "";
        }


        /// <summary>
        /// 取和拼音相同的汉字列表，编码同参数encoding
        /// </summary>
        /// <param name="Pinyin">编码为encoding的拼音</param>
        /// <param name="encoding">编码</param>
        /// <returns>返回编码为encoding的拼音为pinyin的汉字列表，如拼音“ai”将会返回“唉爱……”等</returns>
        public static string GetChineseText(string pinyin, Encoding encoding)
        {
            string text = ConvertEncoding(pinyin, encoding, Encoding.UTF8);
            return ConvertEncoding(GetChineseText(text), Encoding.UTF8, encoding);
        }



        /// <summary>
        /// 返回单个字符的汉字拼音
        /// </summary>
        /// <param name="ch">编码为UTF8的中文字符</param>
        /// <returns>ch对应的拼音</returns>
        public static string GetPinyin(char ch)
        {
            short hash = GetHashIndex(ch);
            for (var i = 0; i < PyHash.hashes[hash].Length; ++i)
            {
                short index = PyHash.hashes[hash][i];
                var pos = PyCode.codes[index].IndexOf(ch, 7);
                if (pos != -1)
                    return PyCode.codes[index].Substring(0, 6).Trim();
            }
            return ch.ToString();
        }

        /// <summary>
        /// 返回单个字符的汉字拼音
        /// </summary>
        /// <param name="ch">编码为encoding的中文字符</param>
        /// <returns>编码为encoding的ch对应的拼音</returns>
        public static string GetPinyin(char ch, Encoding encoding)
        {
            ch = ConvertEncoding(ch.ToString(), encoding, Encoding.UTF8)[0];
            return ConvertEncoding(GetPinyin(ch), Encoding.UTF8, encoding);
        }

        /// <summary>
        /// 转换编码 
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="srcEncoding">源编码</param>
        /// <param name="dstEncoding">目标编码</param>
        /// <returns>目标编码文本</returns>
        public static string ConvertEncoding(string text, Encoding srcEncoding, Encoding dstEncoding)
        {
            byte[] srcBytes = srcEncoding.GetBytes(text);
            byte[] dstBytes = Encoding.Convert(srcEncoding, dstEncoding, srcBytes);
            return dstEncoding.GetString(dstBytes);
        }

        /// <summary>
        /// 取文本索引值
        /// </summary>
        /// <param name="ch">字符</param>
        /// <returns>文本索引值</returns>
        private static short GetHashIndex(char ch)
        {
            return (short)((uint)ch % PyCode.codes.Length);
        }
    }
}