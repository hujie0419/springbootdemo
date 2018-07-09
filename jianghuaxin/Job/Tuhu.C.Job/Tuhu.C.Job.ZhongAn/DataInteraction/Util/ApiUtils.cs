using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace Zhongan.DI.Util
{
    /// <summary>
    /// API系统工具类。
    /// </summary>
    public abstract class ApiUtils
    {
        /// <summary>
        /// 查找数组模板中是否存在与key相同的某个元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsExistence(string key, String[] str)
        {
            bool bl = false;
            foreach (string item in str)
            {
                if (key == item) return true;
            }
            return bl;
        }

    }
}
