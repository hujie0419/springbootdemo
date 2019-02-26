using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class QRCodeStatisticsConfigDAL
    {
        /// <summary>
        /// 获取微信二维码扫描事件统计
        /// </summary>
        /// <param name="sqlconn"></param>
        /// <param name="queryName"></param>
        /// <returns></returns>
        public static IEnumerable<QRCodeStatisticsConfigModel> GetListByPage(SqlConnection sqlconn, string queryName)
        {
            string strSql = @"
                SELECT * FROM (
	                SELECT  
	                YEAR(CreateTime) 'year',
	                month(CreateTime) 'month',
	                day(CreateTime) 'day',
	                sum(1) 'sum',
	                EventKey,
	                'ChannelName' = ISNULL((SELECT TOP 1 ChannelName FROM Configuration..SE_QRCodeManageConfig WITH(NOLOCK) WHERE TraceId = EventKey),''),
	                Event
	                from  Configuration..SE_QRCodeStatisticsConfig WITH(NOLOCK)
	                group by year(CreateTime),month(CreateTime),day(CreateTime),EventKey,Event
                ) AS tab1 
            ";

            List<SqlParameter> sqlparams = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(queryName))
            {
                strSql += "  WHERE tab1.ChannelName LIKE '%'+@QueryName+'%' ";
                sqlparams.Add(new SqlParameter("@QueryName", queryName));
            }

            return SqlHelper.ExecuteDataTable(sqlconn, CommandType.Text, strSql.ToString(), (sqlparams.Count > 0 ? sqlparams.ToArray() : null)).ConvertTo<QRCodeStatisticsConfigModel>();
        }
    }
}