using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.ActivityBoard;

namespace Tuhu.Provisioning.DataAccess.DAO.ActivityBoard
{
    public static class DALActivityBoard
    {
        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<ActivityBoardLog> SelectOperationLog(string objectId, ActivityBoardModuleType type)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT * FROM Tuhu_log..ActivityBoardLog WITH (NOLOCK) WHERE ObjectId=@ObjectId AND Type=@Type ORDER BY CreatedTime DESC");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ObjectId", objectId);
                cmd.Parameters.AddWithValue("@Type", type.ToString());
                return dbHelper.ExecuteDataTable(cmd).ConvertTo<ActivityBoardLog>().ToList();
            }
        }

        public static List<BIActivityPageModel> SelectActvityForBI(SqlConnection conn, DateTime start, DateTime end, string activityId)
        {
            const string sql = @"SELECT clicktime_varchar ,
            app_id ,
            id ,
            PV ,
            UV ,
            ClickUV ,
            Loaddate ,
            Registrationnum ,
            PromotionUV ,
            OrederUV ,
            Ordernum ,
            Ordernum_Tire ,
            Ordernum_Maintenance ,
            Ordernum_CarProduct ,
            Ordernum_Hubbeauty ,
            Ordernum_Hubbeauty ,
            Ordernum_Others ,
            SalesAmount1_Tire ,
            SalesAmount1_Maintenance ,
            SalesAmount1_CarProduct ,
            SalesAmount1_Hubbeauty ,
            SalesAmount1_Others ,
            SalesAmount2_Tire ,
            SalesAmount2_Maintenance ,
            SalesAmount2_CarProduct ,
            SalesAmount2_Hubbeauty ,
            SalesAmount2_Others ,
            CASE WHEN UV = 0 THEN NULL
                 ELSE ClickUV * 1.00 / UV
            END ClickPercent--点击转化率
            ,
            CASE WHEN ClickUV = 0 THEN NULL
                 ELSE OrederUV * 1.00 / ClickUV
            END OrderPercent--下单转化率
            ,
            SalesPercent1_Tire--毛利率
            ,
             SalesPercent1_Maintenance ,
            SalesPercent1_CarProduct ,
             SalesPercent1_Hubbeauty ,
            SalesPercent1_Others ,
            SalesPercent2_Tire--券后毛利率
            ,
            SalesPercent2_Maintenance ,
            SalesPercent2_CarProduct ,
           SalesPercent2_Hubbeauty ,
             SalesPercent2_Others,
	        Cost_Tire,
			 Cost_Maintenance,
			 Cost_CarProduct,
			 Cost_Hubbeauty,
			 OrderUV_BY,
			 OrderNum_BY,
			 OrderPercent_BY,
			 SalesAmount1_BY,
			 SalesAmount2_BY,
			 SalesPercent1_BY,
			 SalesPercent2_BY,
			 Cost_BY
         FROM Tuhu_bi..dm_XH5_ActivityPage WITH (NOLOCK) 
            WHERE ID=@ActivityId AND  clicktime_varchar >= @StartTime AND clicktime_varchar <=@EndTime AND APP_ID IN ('h5_ios_app','h5_android_app','tuhu_web','weixin')";
            return conn.Query<BIActivityPageModel>(sql, new { StartTime = start, EndTime = end, ActivityId = activityId.ToString() }, commandType: CommandType.Text).ToList();
        }

        public static ActivityBuild GetActivityForPage(SqlConnection conn, int pkid)
        {
            const string sql = @"		 SELECT  PKID AS Id ,
                        Title ,
                        ActivityType AS ActivityConfigType ,
                        StartDate AS StartDT ,
                        EndDate AS EndDate ,
                        1 AS IsNew ,
                        HashKey,
						RuleDesc AS Content,
                        PersonCharge AS PersonWheel ,
                        CreateorUser AS CreatetorUser
                FROM    Configuration..ActivePageList WITH ( NOLOCK )
                WHERE   IsEnabled = 1 AND PKID = @PKID";
            return conn.Query<ActivityBuild>(sql, new { PKID = pkid }, commandType: CommandType.Text).SingleOrDefault();
        }
    }
}
