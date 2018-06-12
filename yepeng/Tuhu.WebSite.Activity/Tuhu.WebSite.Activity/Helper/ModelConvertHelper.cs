using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Tuhu.WebSite.Web.Activity.Helper
{
    public class ModelConvertHelper<T> where T : new()
    {
        /// <summary>
        /// DataTable转换为List集合
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<T> ConvertToModel(DataTable dt)
        {
            try
            {
                // 定义集合
                IList<T> ts = new List<T>();
                // 获得此模型的类型
                Type type = typeof(T);
                string tempName = "";
                foreach (DataRow dr in dt.Rows)
                {
                    T t = new T();
                    // 获得此模型的公共属性
                    PropertyInfo[] propertys = t.GetType().GetProperties();
                    foreach (PropertyInfo pi in propertys)
                    {
                        tempName = pi.Name;

                        // 检查DataTable是否包含此列
                        if (dt.Columns.Contains(tempName))
                        {
                            // 判断此属性是否有Setter
                            if (!pi.CanWrite) continue;

                            var dt_DataType = dt.Columns[tempName].DataType.ToString().Trim();
                            var model_DataType = pi.PropertyType.FullName;

                            object value = dr[tempName];
                            if (value != DBNull.Value)
                            {
                                if (model_DataType.Equals(dt_DataType))
                                    pi.SetValue(t, value, null);
                                else
                                    pi.SetValue(t, Convert.ChangeType(value.ToString(), pi.PropertyType), null);
                            }
                        }
                    }
                    ts.Add(t);
                }
                return ts;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}