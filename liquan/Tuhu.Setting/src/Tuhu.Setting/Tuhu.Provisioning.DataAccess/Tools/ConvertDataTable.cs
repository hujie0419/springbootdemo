using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Tuhu.Provisioning.DataAccess.Tools
{
    public class ConvertDataTable
    {
        /// <summary>
        /// 将集合转换成数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            DataTable dtReturn = new DataTable();
            PropertyInfo[] propArray = null;
            foreach (T rec in collection)
            {
                if (propArray == null)
                {
                    propArray = rec.GetType().GetProperties();
                    foreach (PropertyInfo pi in propArray)
                    {
                        Type colType = pi.PropertyType;
                        if (colType.IsGenericType && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();
                foreach (PropertyInfo pi in propArray)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) ?? DBNull.Value;
                }
                dtReturn.Rows.Add(dr);
            }

            return dtReturn;
        }
    }
}
