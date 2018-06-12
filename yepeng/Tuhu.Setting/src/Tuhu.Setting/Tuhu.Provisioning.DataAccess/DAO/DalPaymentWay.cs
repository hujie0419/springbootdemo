using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalPaymentWay
    {
        /// <summary>
        /// 查找所有支付方式
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<PaymentWayModel> GetAllPaymentWay(SqlConnection connection)
        {
            var sql = "SELECT * FROM Gungnir..Payment_way_2 WITH (NOLOCK) ORDER BY Payment_way_order";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<PaymentWayModel>().ToList();
        }

        /// <summary>
        /// 修改/添加
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pwModel"></param>
        /// <returns></returns>
        public static bool UpdatePaymentWay(SqlConnection connection, string sqlStr, SqlParameter[] sqlParams)
        {
            if (!string.IsNullOrEmpty(sqlStr) && sqlParams != null)
            {

                var result = SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sqlStr, sqlParams);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }
    }
}
