using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.DAO.Feedback
{
    public static class Helper
    {
        /// <summary>
        /// 获取表每行的第一列
        /// </summary>
        /// <param name="dt">dataTable</param>
        /// <returns>string类型集合</returns>
        public static List<string> ConvertToStringList(this DataTable dt)
        {
            var rows = dt.Rows.Cast<DataRow>();
            var result = new List<string>();
            foreach (DataRow row in rows)
            {
                if (row != null)
                {
                    result.Add(row[0].ToString());
                }
            }
            return result;
        }

       
    }
}
