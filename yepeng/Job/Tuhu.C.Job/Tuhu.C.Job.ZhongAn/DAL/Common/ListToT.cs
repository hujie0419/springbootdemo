//DataSet与泛型集合间的互相转换
//利用反射机制将DataTable的字段与自定义类型的公开属性互相赋值。
//注意：从DataSet到IList<T>的转换，自定义类型的公开属性必须与DataTable中的字段名称
//一致，才能到达想要的结果。建议DataTable的定义从数据库来，自定义类型用O/R Mapping的方式获得。 
using System.Data;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System;
namespace K.DLL.Common
{
    /// <summary> 
    /// 泛型集合与DataSet互相转换 
    /// </summary> 
    public static class ListToT
    {
        /// <summary> 
        /// 集合装换DataSet 
        /// </summary> 
        /// <param name="list">集合</param> 
        public static DataSet ToDataSet(IList p_List)
        {
            DataSet result = new DataSet();
            DataTable _DataTable = new DataTable();
            if (p_List.Count > 0)
            {
                PropertyInfo[] propertys = p_List[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    _DataTable.Columns.Add(pi.Name, pi.PropertyType);
                }

                for (int i = 0; i < p_List.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(p_List[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    _DataTable.LoadDataRow(array, true);
                }
            }
            result.Tables.Add(_DataTable);
            return result;
        }
        /// <summary> 
        /// 泛型集合转换DataSet 
        /// </summary> 
        /// <param name="list">泛型集合</param> 
        public static DataSet ToDataSet<T>(IList<T> list)
        {
            return ToDataSet<T>(list, null);
        }
        /// <summary> 
        /// 泛型集合转换DataSet 
        /// </summary> 
        /// <param name="p_List">泛型集合</param> 
        /// <param name="p_PropertyName">待转换属性名数组</param> 
        public static DataSet ToDataSet<T>(IList<T> p_List, params string[] p_PropertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (p_PropertyName != null)
                propertyNameList.AddRange(p_PropertyName);

            DataSet result = new DataSet();
            DataTable _DataTable = new DataTable();
            if (p_List.Count > 0)
            {
                PropertyInfo[] propertys = p_List[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        // 没有指定属性的情况下全部属性都要转换 
                        _DataTable.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            _DataTable.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < p_List.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(p_List[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(p_List[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    _DataTable.LoadDataRow(array, true);
                }
            }
            result.Tables.Add(_DataTable);
            return result;
        }
        /// <summary> 
        /// DataSet装换为泛型集合 
        /// </summary> 
        /// <param name="p_TableIndex">待转换数据表索引</param> 
        /// <returns></returns> 
        public static IList<T> ToIList<T>(this DataSet p_DataSet)
        {
            if (p_DataSet == null || p_DataSet.Tables.Count < 0)
                return null;

            DataTable p_Data = p_DataSet.Tables[0];
            // 返回值初始化 
            IList<T> result = new List<T>();
            for (int j = 0; j < p_Data.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    for (int i = 0; i < p_Data.Columns.Count; i++)
                    {
                        // 属性与字段名称一致的进行赋值 
                        if (pi.Name.Equals(p_Data.Columns[i].ColumnName))
                        {
                            // 数据库NULL值单独处理 
                            if (p_Data.Rows[j][i] != DBNull.Value)
                                pi.SetValue(_t, p_Data.Rows[j][i], null);
                            else
                                pi.SetValue(_t, null, null);
                            break;
                        }
                    }
                }
                result.Add(_t);
            }
            return result;
        }
        public static IList<T> ToIList<T>(this DataTable p_Data)
        {
            // 返回值初始化 
            IList<T> result = new List<T>();
            for (int j = 0; j < p_Data.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    for (int i = 0; i < p_Data.Columns.Count; i++)
                    {
                        // 属性与字段名称一致的进行赋值 
                        if (pi.Name.Equals(p_Data.Columns[i].ColumnName))
                        {
                            // 数据库NULL值单独处理 
                            if (p_Data.Rows[j][i] != DBNull.Value)
                                pi.SetValue(_t, p_Data.Rows[j][i], null);
                            else
                                pi.SetValue(_t, null, null);
                            break;
                        }
                    }
                }
                result.Add(_t);
            }
            return result;
        }
        /// <summary> 
        /// DataSet装换为泛型集合 
        /// </summary> 
        /// <param name="p_DataSet">DataSet</param> 
        /// <param name="p_TableName">待转换数据表名称</param> 
        public static IList<T> ToIList<T>(this DataSet p_DataSet, string p_TableName)
        {
            int _TableIndex = 0;
            if (p_DataSet == null || p_DataSet.Tables.Count < 0)
                return null;
            if (string.IsNullOrEmpty(p_TableName))
                return null;
            for (int i = 0; i < p_DataSet.Tables.Count; i++)
            {
                // 获取Table名称在Tables集合中的索引值 
                if (p_DataSet.Tables[i].TableName.Equals(p_TableName))
                {
                    _TableIndex = i;
                    break;
                }
            }
            return ToIList<T>(p_DataSet);
        }
    }
}
