using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;
using System.Diagnostics;

namespace Tuhu.Provisioning.DataAccess.DAO.Push
{
    /// <summary>
    /// 公用处理类
    /// </summary>
    public static class CommonUtil
    {
        public static String ConvertBoolToTOrF(bool source)
        {
            string result = "F";
            if (source)
            {
                result = "T";
            }
            return result;
        }

        public static int ConvertToWeek<T>(T source)
        {
            DateTime? date = CommonUtil.ConvertDateTimeOrNull(source);
            int result = 0;

            if (date.HasValue)
            {
                result = (int)date.Value.DayOfWeek;
                if (result == 0)
                {
                    result = 7;
                }
            }
            return result;
        }

        public static int ConvertToWeek(DayOfWeek week)
        {
            int result = (int)week;

            if (result == 0)
            {
                result = 7;
            }
            return result;
        }

        public static string ConvertToWeekInfo(DayOfWeek week)
        {
            int resultWeek = (int)week;
            string result = "";
            switch (resultWeek)
            {
                case 1:
                    result = "周一";
                    break;
                case 2:
                    result = "周二";
                    break;
                case 3:
                    result = "周三";
                    break;
                case 4:
                    result = "周四";
                    break;
                case 5:
                    result = "周五";
                    break;
                case 6:
                    result = "周六";
                    break;
                case 0:
                    result = "周日";
                    break;
            }
            return result;
        }

        public static DateTime? ConvertDateTimeOrNull<T>(T source)
        {
            DateTime result;

            if (source == null)
            {
                return null;
            }
            else if (DateTime.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static bool ConvertToBool<T>(T source)
        {
            bool result;

            if (source == null)
            {
                return false;
            }
            if (source.ToString().ToUpper().Equals("T"))
            {
                return true;
            }
            if (source.ToString().ToUpper().Equals("F"))
            {
                return false;
            }
            if (Boolean.TryParse(source.ToString().ToLower(), out result))
            {
                return result;
            }
            else
            {
                return false;
            }
        }

        public static bool IsNumeric(string str)
        {
            if (str == null || str.Length == 0)
                return false;
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            byte[] bytestr = ascii.GetBytes(str);
            foreach (byte c in bytestr)
            {
                if (c < 48 || c > 57)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static V ConvertToEnum<T, V>(T source) where V : new()
        {
            Type type = typeof(V);
            V v = new V();

            if (source == null)
            {
                return v;
            }

            try
            {
                string sourceString = source.ToString();
                return (V)Enum.Parse(type, sourceString, true);
            }
            catch
            {
                return v;
            }
        }

        /// <summary>
        /// 转换数据为int?型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int? ConvertObjectToInt<T>(T source)
        {
            int result;

            if (source == null)
            {
                return null;
            }
            else if (int.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte ConvertToByte<T>(T source)
        {
            byte result;

            if (source == null)
            {
                return default(byte);
            }
            else if (byte.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return default(byte);
            }
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int RoundToInt<T>(T source)
        {
            if (source == null)
            {
                return 0;
            }
            int ret;
            if (int.TryParse(source.ToString(), out ret))
            {
                return ret;
            }
            return
             ConvertObjectToInt32(
            Math.Floor(ConvertObjectToDecimalSingle(source) + 0.5M));
        }

        /// <summary>
        /// 转换数据为decimal?型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal? ConvertObjectToDecimal<T>(T source)
        {
            decimal result;

            if (source == null)
            {
                return null;
            }
            else if (decimal.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal ConvertObjectToDecimalSingle<T>(T source)
        {
            decimal? result = ConvertObjectToDecimal(source);

            if (result != null)
            {
                return result.Value;
            }
            else
            {
                return 0M;
            }
        }

        /// <summary>
        /// 转化为指定精度的decimal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static decimal ConvertObjectToDecimalSingleWithPrecision<T>(T source, int precision = 2)
        {
            decimal? result = ConvertObjectToDecimal(source);

            if (result != null)
            {
                return Math.Round(result.Value, precision, MidpointRounding.AwayFromZero);
            }
            else
            {
                return Math.Round(0.00m, precision, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// 转换数据为short型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static short ConvertObjectToInt16<T>(T source)
        {
            short result;

            if (source == null)
            {
                return 0;
            }
            else if (short.TryParse(String.Format("{0:f0}", source), out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换数据为int型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int ConvertObjectToInt32<T>(T source)
        {
            int result;

            if (source == null)
            {
                return 0;
            }
            else if (int.TryParse(String.Format("{0:f0}", source), out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换数据为long型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static long ConvertObjectToInt64<T>(T source)
        {
            long result;

            if (source == null)
            {
                return 0;
            }
            else if (long.TryParse(String.Format("{0:f0}", source), out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 转化decimal
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static decimal ConvertDecimal(decimal? value)
        {
            return value == null ? 0 : value.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime ConvertDateTime<T>(T source)
        {
            DateTime result;

            if (source == null)
            {
                return default(DateTime);
            }
            else if (DateTime.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return default(DateTime);
            }

        }

        /// <summary>
        /// 将数据转为可空的日期型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime? ConvertDateTimeNullable<T>(T source)
        {
            DateTime result;

            if (source == null)
            {
                return null;
            }
            else if (DateTime.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 转成不为空的日期类型，默认值为2000-1-1，防止接口报错
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ConvertDateTimeNotNull(DateTime? date)
        {
            return date.HasValue ? date.Value : new DateTime(2000, 1, 1);
        }
        public static float ConvertToSingle<T>(T source)
        {
            float result;

            if (source == null)
            {
                return 0f;
            }
            else if (float.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return 0f;
            }
        }

        #region Other CommonFunc

        /// <summary>
        /// 把结果为0，转换成-1
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Decimal? ChangeZeroToNegative(Decimal? source)
        {
            return source.Equals(0M) ? null : source;
        }

        /// <summary>
        /// 不为空并且为-1时，返回font color='red' X /font
        /// 为null或空，返回ConvertToRound的返回值 
        /// </summary>
        public static String ConvertToRoundNegative<T>(T source)
        {
            String result = ConvertToRound(source);

            if (!String.IsNullOrEmpty(result))
            {
                return !result.Equals("-1.00") ? result : "X";
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// 检查是否为整数还是小数。
        /// 如果是整数，去掉保留两位小数(如:111.00 => 111)
        /// 反则，保留两位小数(如:111.12 => 111.12)
        /// </summary>
        public static String CheckIsIntAndConvertToInt(String result)
        {
            if (String.IsNullOrEmpty(result))
            {
                return result;
            }
            else if (result.EndsWith("00"))
            {
                return result.Remove(result.Length - 3);
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// 不为空时，四舍五入保留两位小数
        /// 为空时，返回空字符
        /// </summary>
        public static String ConvertToRound<T>(T source)
        {
            if (source != null)
            {
                if (!String.Empty.Equals(source.ToString()))
                {
                    decimal result = Math.Round((Convert.ToDecimal(source)), 2, MidpointRounding.AwayFromZero);
                    return result.ToString();
                }
                else { return source.ToString(); }
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 转换百分数为小数
        /// 如：15%=>0.15
        /// 为Null或空时，返回空字符
        /// </summary>
        public static String ConvertToPercentDecimal<T>(T source)
        {
            if (source != null)
            {
                if (!String.Empty.Equals(source.ToString()))
                {
                    decimal result = Convert.ToDecimal(source) / 100;

                    return result.ToString();
                }
                else { return source.ToString(); }
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 转换百分数
        /// 如：15=>15%
        /// 为Null或空时，返回空字符
        /// </summary>
        public static String ConvertToPercentString<T>(T source)
        {
            if (source != null)
            {
                if (!String.Empty.Equals(source.ToString()))
                {
                    decimal result = Convert.ToDecimal(source);

                    if (result == 0)
                    {
                        return "0%";
                    }
                    else
                    {
                        return string.Format("{0:#%}", result / 100);
                    }
                }
                else { return source.ToString(); }
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 为-1时，返回为空
        /// 不为空时，保留两位小数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static String ConvertToNull<T>(T source)
        {
            decimal? result = CommonUtil.ConvertObjectToDecimal(source);

            return result != null ? String.Format("{0:F2}", result) : String.Empty;
        }

        public static String ConvertObjectToString<T>(T source)
        {
            return source == null ? String.Empty : source.ToString();
        }

        /// <summary>
        /// object转为布尔（支持数据为T|F的）
        /// </summary>
        public static bool ConvertObjectToBoolViaTF<T>(T source)
        {
            if (source == null)
            {
                return false;
            }
            return source.ToString().Equals("T");
        }

        public static Boolean IsCheckNullOrEmpty(String source)
        {
            return String.IsNullOrEmpty(source);
        }

        /// <summary>
        /// Check &nbsp;
        /// </summary>
        public static String CovertNBSP(String source)
        {
            return source.ToLower().Equals("&nbsp;") ? String.Empty : source;
        }

        /// <summary>
        /// 如果是X,显示为-1
        /// 如果是"&nbsp;"显示为空
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static String ConvertToEmptyOrSymbolX(String source)
        {
            if (source.ToLower().Equals("x"))
            {
                return "-1";
            }
            else
            {
                return (CovertNBSP(source));
            }
        }

        /// <summary>
        /// 转换包含“%"
        /// 如：12% => 12
        /// </summary>
        public static String ConvertSymbolPercent(String source)
        {
            if (source.EndsWith("%"))
            {
                source = source.Substring(0, source.Length - 1);
            }
            else
            {
                source = CovertNBSP(source);
            }

            return source;
        }

        public static String ConvertToZero<T>(T source)
        {
            int? result = ConvertObjectToNullableInt(source);

            return result != null ? result.ToString() : String.Empty;
        }

        public static int? ConvertObjectToNullableInt<T>(T source)
        {
            int result;

            if (source.ToString().Equals(String.Empty))
            {
                return null;
            }
            else if (int.TryParse(String.Format("{0:f0}", source), out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        #endregion

        public static int ConvertToFormatInt<T>(T source, int provider)
        {
            try
            {
                return Convert.ToInt32(source.ToString(), provider);
            }
            catch
            {
                return 0;
            }
        }

        public static string ConvertToTrimString<T>(T source)
        {
            string result = source.ToString();

            if (!String.IsNullOrEmpty(result))
            {
                result = result.Trim();
            }
            return result;
        }

        /// <summary>
        /// 格式化日期
        /// </summary>
        /// <returns></returns>
        public static string FormatEtermDate(DateTime date)
        {
            string ret = string.Empty;
            int month = date.Month;

            switch (month)
            {
                case 1:
                    ret = "JAN";
                    break;
                case 2:
                    ret = "FEB";
                    break;
                case 3:
                    ret = "MAR";
                    break;
                case 4:
                    ret = "APR";
                    break;
                case 5:
                    ret = "MAY";
                    break;
                case 6:
                    ret = "JUN";
                    break;
                case 7:
                    ret = "JUL";
                    break;
                case 8:
                    ret = "AUG";
                    break;
                case 9:
                    ret = "SEP";
                    break;
                case 10:
                    ret = "OCT";
                    break;
                case 11:
                    ret = "NOV";
                    break;
                case 12:
                    ret = "DEC";
                    break;
            }

            int day = date.Day;

            return (date.Day <= 9 ? "0" : "") + date.Day + ret;
        }

        /// <summary>
        /// 安全读取列索引
        /// </summary>
        /// <returns>存在列返回列索引，否则返回-1</returns>
        public static int SafeGetOrdinal(IDataReader dr, string columnName)
        {
            try
            {
                return dr.GetOrdinal(columnName);
            }
            catch
            {
                return -1;
            }
        }

        public static string GetMonth(DateTime date)
        {
            return date.ToString("MMM", CultureInfo.InvariantCulture).ToUpper();
        }

        /// <summary>
        /// 获取间隔秒数
        /// </summary>
        /// <param name="t1">上次执行开始时间</param>
        /// <returns>t1-t2</returns>
        public static decimal GetsSeconds(DateTime t1)
        {
            TimeSpan ts = DateTime.Now - t1;
            return ConvertObjectToDecimalSingle(ts.TotalMilliseconds / 1000);
        }

        /// <summary>
        /// 获取间隔秒数
        /// </summary>
        /// <param name="t1">大时间</param>
        /// <param name="t2">小时间</param>
        /// <returns>t1-t2</returns>
        public static decimal GetsSeconds(DateTime t1, DateTime t2)
        {
            TimeSpan ts = t1 - t2;
            return ConvertObjectToDecimalSingle(ts.TotalMilliseconds / 1000);
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// ASCII转字母
        /// </summary>
        /// <param name="asciiNum">asc码</param>
        /// <returns></returns>
        public static string ConvertASCIIToChar(int asciiNum)
        {
            try
            {
                ASCIIEncoding ascii = new ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiNum };
                return ascii.GetString(byteArray);
            }
            catch
            {
                return String.Empty;
            }
        }
        /// <summary>
        /// 字母转ASCII
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ConvertCharToASCII(string value)
        {
            try
            {
                ASCIIEncoding ascii = new ASCIIEncoding();
                byte[] byteArray = ascii.GetBytes(value);
                return Convert.ToInt32(byteArray[0]);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 移除字符串中特殊的ASCII控制符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpecialASCII(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return str;
                }
                char[] arr = str.ToCharArray();
                StringBuilder sb = new StringBuilder(1024);
                foreach (char item in arr)
                {
                    if ((item >= 0 && item <= 31) || item == 127)
                    {
                        continue;
                    }
                    sb.Append(item);
                }
                return sb.ToString();
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// 是否包含特殊的ASCII控制符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ContainsSpecialASCII(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            char[] arr = str.ToCharArray();
            foreach (char item in arr)
            {
                if ((item >= 0 && item <= 31) || item == 127)
                {
                    return true;
                }
            }
            return false;
        }

        // <summary>
        /// 统计开始执行时间
        /// </summary>
        public static Stopwatch StartExecuteTimeLog()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            return sw;
        }

        /// <summary>
        /// 获取星期信息
        /// </summary>
        /// <returns></returns>
        public static int GetDayOfWeek()
        {

            DateTime now = DateTime.Now;
            int dayOfWeek = (int)now.DayOfWeek;
            return dayOfWeek;
        }

        /// <summary>
        /// 处理多个列表（可以是合并，可以是去除）
        /// 合并：多列表，非重新性合并
        /// 剔除：在第一个列表中，剔除掉其他列表中存在的数据
        /// </summary>
        /// <param name="isCombine">True：合并;False:剔除</param>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static List<String> DealWithLists(bool isCombine, params List<String>[] lists)
        {
            List<String> result = new List<string>();

            if (lists.Length > 0)
            {
                Dictionary<String, String> dict = new Dictionary<string, string>();
                foreach (String item in lists[0])
                {
                    dict.Add(item, String.Empty);
                }
                //合并
                if (isCombine)
                {
                    for (int i = 1; i < lists.Length; i++)
                    {
                        foreach (String item in lists[i])
                        {
                            //不包含，则合并
                            if (!dict.ContainsKey(item))
                            {
                                dict.Add(item, String.Empty);
                            }
                        }
                    }
                }
                //剔除
                else
                {
                    for (int i = 1; i < lists.Length; i++)
                    {
                        foreach (String item in lists[i])
                        {
                            //包含，则剔除
                            if (dict.ContainsKey(item))
                            {
                                dict.Remove(item);
                            }
                        }
                    }
                }

                //转换List
                foreach (String key in dict.Keys)
                {
                    result.Add(key);
                }
            }

            return result;
        }

        #region [ 字符串处理 ]
        /// <summary>
        /// 是否为完整子串（totalString默认以;分割各个部分）
        /// e.g 
        ///     totalString="jason;smill;" 
        ///         partString="smill"  -> 结果为true
        ///         partString="smil" -> 结果是false
        /// </summary>
        /// <param name="partString"></param>
        /// <param name="totalString"></param>
        /// <param name="splitChar">分割字符（默认;）</param>
        /// <returns></returns>
        public static bool IsSubString(String partString, String totalString, char splitChar = ';')
        {
            bool bResult = false;

            totalString = splitChar + totalString.Trim(splitChar) + splitChar;

            bResult = totalString.IndexOf(splitChar + partString + splitChar, StringComparison.CurrentCultureIgnoreCase) != -1;

            return bResult;
        }
        #endregion

        #region [ 日期处理 ]
        /// <summary>
        /// 判断是否不是最小日期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool NotMinDateTime<T>(T source)
        {
            DateTime result;
            if (source == null)
            {
                return false;
            }
            else if (!DateTime.TryParse(source.ToString(), out result))
            {
                return false;
            }
            else if (result == DateTime.MinValue)
                return false;

            return true;
        }

        private static DateTime minSqlDateTime = new DateTime(1753, 1, 1);
        /// <summary>
        /// 最小DB SQL Time
        /// </summary>
        public static DateTime MinSqlDateTime
        {
            get
            {
                return minSqlDateTime;
            }
        }

        /// <summary>
        /// 是否最新DB Time
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsMinSqlDateTime(DateTime dt)
        {
            return minSqlDateTime.Equals(dt);
        }
        #endregion
    }
}
