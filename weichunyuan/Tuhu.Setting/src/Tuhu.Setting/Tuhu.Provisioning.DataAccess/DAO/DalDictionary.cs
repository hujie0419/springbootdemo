using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Schema;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.DataAccess
{
    public static class DalDictionary
    {
        public static DataTable GetAllFinanceMarks(SqlConnection con)
        {
            string sql = @"Select dickey,dicvalue from [tbl_Dictionaries] with(nolock) where dictype=N'FinanceMark'";

            var cmd = new SqlCommand(sql);
            return SqlHelper.ExecuteDataTable(con, CommandType.Text, sql);
        }

        public static List<Dictionary> SelectDeliveryType(SqlConnection connection, string dicType)
        {
            var parameters = new[] {
                new SqlParameter("@DicType",dicType)
            };
            string sql = "SELECT DicType,DicKey,DicValue FROM dbo.tbl_Dictionaries WITH(NOLOCK) WHERE DicType=@DicType AND DicKey<>'4NoDelivery'";
            var deliveryTypeList = new List<Dictionary>();
            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql, parameters))
            {
                while (reader.Read())
                {
                    var dic = new Dictionary()
                    {
                        DicType = reader.IsDBNull(0) ? string.Empty : reader.GetTuhuString(0),
                        DicKey = reader.IsDBNull(1) ? string.Empty : reader.GetTuhuString(1),
                        DicValue = reader.IsDBNull(2) ? string.Empty : reader.GetTuhuString(2)
                    };
                    deliveryTypeList.Add(dic);
                }
            }

            return deliveryTypeList;
        }
    }
}
