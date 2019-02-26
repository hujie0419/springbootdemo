using Dapper;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity.TireActivity;

namespace Tuhu.Provisioning.DataAccess.DAO.TireActivity
{
    public class DalTireActivity
    {
        #region 小保养套餐优惠价格

        /// <summary>
        /// 获取小保养套餐优惠价格列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public static List<MaintenancePackageOnSaleModel> GetMaintenancePackageOnSaleList(out int recordCount, int pageSize = 20, int pageIndex = 1)
        {
            string sql= @"SELECT    [PKID] ,
                                    [PID] ,
                                    [Price] ,
                                    [OnetirePrice] ,
                                    [TwotirePrice] ,
                                    [ThreetirePrice] ,
                                    [FourtirePrice] ,
                                    [UpdateID] ,
                                    [CreateDatetime] ,
                                    [LastUpdateDateTime] ,
                                    [CreateBy] ,
                                    [LastUpdateBy]
                            FROM    Activity.[dbo].[MaintenancePackageOnSale] WITH ( NOLOCK )
                            WHERE   Status = 1
                            ORDER BY [PKID] DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                    ONLY;
                            ";
            string sqlCount = @"SELECT  COUNT(*)
                                FROM    Activity.[dbo].[MaintenancePackageOnSale] WITH ( NOLOCK )
                                WHERE   Status = 1;";

            using (var conn= new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlCount, conn);
                recordCount = (int)cmd.ExecuteScalar();
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@PageSize", pageSize);
                dp.Add("@PageIndex", pageIndex);
                return conn.Query<MaintenancePackageOnSaleModel>(sql, dp).ToList();
            }
        }

        /// <summary>
        /// 获取有效的小保养套餐PID集合
        /// </summary>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static List<MaintenancePackageOnSaleModel> GetMaintenancePackagePidList(List<string> pidList)
        {
            string sql= @"SELECT PID from Tuhu_productcatalog.dbo.[CarPAR_zh-CN] with(nolock) where PID in ('{0}')";
            string sqlResult = string.Format(sql, string.Join("','", pidList));
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sqlResult).ConvertTo<MaintenancePackageOnSaleModel>().ToList();
            }
        }

        /// <summary>
        /// 下载每一次上传的excel
        /// </summary>
        /// <param name="updateID"></param>
        /// <returns></returns>
        public static List<MaintenancePackageOnSaleModel> GetEachMaintenancePackageList(int updateID)
        {
            string sql = @"
                        SELECT  *
                        FROM    Activity.dbo.MaintenancePackageOnSale
                        WHERE   UpdateID = @UpdateID
                        ORDER BY PKID DESC;";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@UpdateID", updateID);
                return conn.Query<MaintenancePackageOnSaleModel>(sql, dp).ToList();
            }
        }

        #region 导入excel-小保养套餐优惠价格数据

        /// <summary>
        /// 获取小保养套餐优惠价格表中最大的更新ID
        /// </summary>
        /// <returns></returns>
        public static List<MaintenancePackageOnSaleModel> GetMaxUpdateID()
        {
            string sql = @"SELECT CASE WHEN MAX(UpdateID) IS NULL THEN 0 ELSE  MAX(UpdateID) END AS UpdateID  FROM Activity.dbo.MaintenancePackageOnSale with (nolock)";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<MaintenancePackageOnSaleModel>().ToList();
            }
        }

        /// <summary>
        /// 将有效的小保养套餐设置为无效的
        /// </summary>
        /// <returns></returns>
        public static int UpdateMaintenancePackageState()
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                string sql = @"
                            UPDATE  Activity.[dbo].[MaintenancePackageOnSale] WITH ( ROWLOCK )
                            SET     Status = 0
                            WHERE   Status = 1
                            ";
                var cmd = new SqlCommand(sql);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 根据PID获取小保养套餐优惠价格数据
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<MaintenancePackageOnSaleModel> GetMaintenancePackageOnSaleModelByPid(string pid)
        {

            string sql = @"SELECT [PKID]
                              ,[PID]
                              ,[Price]
                              ,[OnetirePrice]
                              ,[TwotirePrice]
                              ,[ThreetirePrice]
                              ,[FourtirePrice]
                              ,[UpdateID]
                              ,[CreateDatetime]
                              ,[LastUpdateDateTime]
                              ,[CreateBy]
                              ,[LastUpdateBy]
                      FROM Activity.[dbo].[MaintenancePackageOnSale] WITH(NOLOCK) 
                        WHERE PID=@PID";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@PID", pid);
                return conn.Query<MaintenancePackageOnSaleModel>(sql, dp).ToList();
            }
        }

        /// <summary>
        /// 逻辑删除小保养套餐优惠价格数据
        /// </summary>
        /// <param name="pkidList"></param>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public static int UpdateMaintenancePackageOnSale(List<int> pkidList,string lastUpdateBy)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                string sql = @"UPDATE Activity.[dbo].[MaintenancePackageOnSale] WITH(ROWLOCK) SET Is_Deleted=@Is_Deleted,LastUpdateBy=@LastUpdateBy,LastUpdateDateTime=GETDATE() where PKID in ('{0}')";
                string sqlResult = string.Format(sql, string.Join("','", pkidList));
                var cmd = new SqlCommand(sqlResult);
                cmd.Parameters.AddWithValue("@Is_Deleted", 1);
                cmd.Parameters.AddWithValue("@LastUpdateBy", lastUpdateBy);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 添加小保养套餐优惠价格数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="maxUpdateID"></param>
        /// <returns></returns>
        public static int AddMaintenancePackageOnSale(MaintenancePackageOnSaleModel model,int maxUpdateID)
        {
            var sql = @"INSERT INTO Activity.[dbo].[MaintenancePackageOnSale]
                                ([PID]
                               ,[Price]
                               ,[OnetirePrice]
                               ,[TwotirePrice]
                               ,[ThreetirePrice]
                               ,[FourtirePrice]
                               ,[UpdateID]
                               ,[Status]
                               ,[CreateBy]
                               ,[LastUpdateBy]
                               ,[CreateDatetime]
                               ,[LastUpdateDateTime]
                                )
                        VALUES  ( @PID , 
                                  @Price,
                                  @OnetirePrice,
                                  @TwotirePrice,
                                  @ThreetirePrice,
                                  @FourtirePrice,
                                  @UpdateID,
                                  @Status,
                                  @CreateBy,
                                  @LastUpdateBy,
                                  GETDATE() ,
                                  GETDATE()
                                 )
                        SELECT  SCOPE_IDENTITY();";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var parameters = new[]
            {
                new SqlParameter("@PID", model.PID),
                new SqlParameter("@Price", model.Price),
                new SqlParameter("@OnetirePrice", model.OnetirePrice),
                new SqlParameter("@TwotirePrice", model.TwotirePrice),
                new SqlParameter("@ThreetirePrice", model.ThreetirePrice),
                new SqlParameter("@FourtirePrice", model.FourtirePrice),
                new SqlParameter("@UpdateID", maxUpdateID),
                new SqlParameter("@Status", model.Status),
                new SqlParameter("@CreateBy", model.CreateBy),
                new SqlParameter("@LastUpdateBy", model.LastUpdateBy)
            };
                return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
            }
        }

        #endregion

        #endregion

        #region 轮保定价

        /// <summary>
        /// 获取轮胎活动列表
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public static List<TireActivityModel> GetTireActivityList(out int recordCount, int pageSize = 20, int pageIndex = 1)
        {
            string sql = @"SELECT [PKID]
                                  ,[PlanNumber]
                                  ,[PlanName]
                                  ,[PlanDesc]
                                  ,[PIDNum]
                                  ,[Status]
                                  ,[BeginDatetime]
                                  ,[EndDatetime]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                                  ,[CreateBy]
                                  ,[LastUpdateBy]
                      FROM Activity.[dbo].[TireActivity] WITH(NOLOCK) 
                        order by [CreateDatetime] DESC OFFSET (@PageIndex -1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";
            string sqlCount = @"SELECT COUNT(*) FROM Activity.[dbo].[TireActivity] WITH(NOLOCK) ";

            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlCount, conn);
                recordCount = (int)cmd.ExecuteScalar();
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@PageSize", pageSize);
                dp.Add("@PageIndex", pageIndex);
                return conn.Query<TireActivityModel>(sql, dp).ToList();
            }
        }

        /// <summary>
        /// 根据pkid获得轮胎活动计划数据
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static TireActivityModel GetTireActivityModel(int pkid)
        {
            string sql = @"SELECT [PKID]
                                  ,[PlanNumber]
                                  ,[PlanName]
                                  ,[PlanDesc]
                                  ,[PIDNum]
                                  ,[Status]
                                  ,[UpdateID]
                                  ,[BeginDatetime]
                                  ,[EndDatetime]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                                  ,[CreateBy]
                                  ,[LastUpdateBy]
                      FROM Activity.[dbo].[TireActivity] WITH(NOLOCK) where PKID=@PKID";

            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@PKID", pkid);
                return conn.Query<TireActivityModel>(sql, dp).ToList().FirstOrDefault();
            }
        }

        /// <summary>
        /// 停止轮胎活动计划
        /// </summary>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public static int UpdateTireActivityStatus(int pkid,string lastUpdateBy)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                string sql = @"UPDATE Activity.[dbo].[TireActivity] WITH(ROWLOCK) SET Status=@Status,LastUpdateBy=@LastUpdateBy,LastUpdateDateTime=GETDATE() where PKID=@PKID";
                var cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@Status", -1);
                cmd.Parameters.AddWithValue("@LastUpdateBy", lastUpdateBy);
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 轮胎活动列表-下载excel
        /// </summary>
        /// <returns></returns>
        public static List<TireActivityPIDModel> GetTireActivityPIDList(int tireActivityID)
        {
            string sql = @"SELECT TireActivityID,PKID,PID FROM Activity.dbo.TireActivityPID WITH(NOLOCK) WHERE TireActivityID=@TireActivityID";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@TireActivityID", tireActivityID);
                return conn.Query<TireActivityPIDModel>(sql, dp).ToList();
            }
        }

        /// <summary>
        /// 获取有效的轮胎产品PID集合
        /// </summary>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static List<TireActivityPIDModel> GetValidTirePid(List<string> pidList)
        {
            string sql = @"SELECT  Pid2 as PID  FROM Tuhu_productcatalog.dbo.CarPAR_CatalogProducts WITH(NOLOCK) WHERE Pid2 IN ('{0}')";
            string sqlResult = string.Format(sql, string.Join("','", pidList));
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sqlResult).ConvertTo<TireActivityPIDModel>().ToList();
            }
        }

        /// <summary>
        /// 获取轮胎活动数据
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<TireActivityPIDModel> GetTireActivityPidByPid(string pid)
        {
            string sql = @"SELECT P.PKID ,P.TireActivityID
                        FROM Activity.dbo.TireActivity AS T  with(nolock) inner JOIN Activity.dbo.TireActivityPID AS P with (nolock) ON T.PKID=P.TireActivityID where p.PID=@PID  AND GETDATE()<=T.EndDatetime AND T.Status>0 AND p.Is_Deleted=0";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@PID", pid);
                return conn.Query<TireActivityPIDModel>(sql, dp).ToList();
            }
        }

        /// <summary>
        /// 逻辑删除轮胎
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="LastUpdateBy"></param>
        /// <returns></returns>
        public static int UpdateTireActivityPid(int pkid,string LastUpdateBy)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                string sql = @"UPDATE Activity.[dbo].[TireActivityPID] WITH(ROWLOCK) SET Is_Deleted=@Is_Deleted,LastUpdateBy=@LastUpdateBy,LastUpdateDateTime=GETDATE() where PKID=@PKID";
                var cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@Is_Deleted", 1);
                cmd.Parameters.AddWithValue("@LastUpdateBy", LastUpdateBy);
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 轮胎活动计划的PID数量减1
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="LastUpdateBy"></param>
        /// <returns></returns>
        public static int UpdateTireActivityPidNum(int pkid, string LastUpdateBy)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                string sql = @"UPDATE Activity.dbo.TireActivity WITH(ROWLOCK) SET PIDNum=(CASE WHEN PIDNum=0 THEN 0 ELSE PIDNum-1 end), LastUpdateBy=@LastUpdateBy,LastUpdateDateTime=GETDATE() WHERE PKID=@PKID";
                var cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@PKID", pkid);
                cmd.Parameters.AddWithValue("@LastUpdateBy", LastUpdateBy);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 获取轮胎活动计划最大更新ID
        /// </summary>
        /// <returns></returns>
        public static List<TireActivityModel> GetMaxTireActivityUpdateID()
        {
            string sql = @"SELECT CASE WHEN MAX(UpdateID) IS NULL THEN 0 ELSE  MAX(UpdateID) END AS UpdateID  FROM Activity.dbo.TireActivity with (nolock)";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<TireActivityModel>().ToList();
            }
        }

        /// <summary>
        /// 获取重复轮胎PID个数与计划
        /// </summary>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static List<TireActivityModel> GetRepeatTirePids(List<string> pidList)
        {
            string sql = @"SELECT COUNT(*) AS repeatPidCount,a.PlanNumber  FROM 
                    (SELECT P.PID,T.Status,T.PKID, t.PlanNumber
                    FROM Activity.dbo.TireActivity AS T  with(nolock) inner JOIN Activity.dbo.TireActivityPID AS P with (nolock) ON T.PKID=P.TireActivityID
                    WHERE P.PID IN ('{0}') AND GETDATE()<=T.EndDatetime AND T.Status>0 AND P.Is_Deleted=0
                    ) a GROUP BY a.PlanNumber";
            string sqlResult = string.Format(sql, string.Join("','", pidList));
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                conn.Open();
                return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sqlResult).ConvertTo<TireActivityModel>().ToList();
            }
        }

        /// <summary>
        /// 添加轮胎活动计划
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddTireActivity(TireActivityModel model)
        {
            var sql = @"INSERT INTO Activity.[dbo].[TireActivity]
                                ([PlanNumber]
                                   ,[PlanName]
                                   ,[PlanDesc]
                                   ,[PIDNum]
                                   ,[Status]
                                   ,[UpdateID]
                                   ,[BeginDatetime]
                                   ,[EndDatetime]
                                   ,[CreateDatetime]
                                   ,[LastUpdateDateTime]
                                   ,[CreateBy]
                                   ,[LastUpdateBy]
                                )
                        VALUES  ( @PlanNumber , 
                                  @PlanName,
                                  @PlanDesc,
                                  @PIDNum,
                                  @Status,
                                  @UpdateID,
                                  @BeginDatetime,
                                  @EndDatetime,
                                  GETDATE() ,
                                  GETDATE(),
                                  @CreateBy,
                                  @LastUpdateBy
                                 )
                        SELECT  SCOPE_IDENTITY();";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var parameters = new[]
            {
                new SqlParameter("@PlanNumber", model.PlanNumber),
                new SqlParameter("@PlanName", model.PlanName),
                new SqlParameter("@PlanDesc", model.PlanDesc),
                new SqlParameter("@PIDNum", model.PIDNum),
                new SqlParameter("@Status", model.Status),
                new SqlParameter("@UpdateID", model.UpdateID),
                new SqlParameter("@BeginDatetime", model.BeginDatetime),
                new SqlParameter("@EndDatetime", model.EndDatetime),
                new SqlParameter("@CreateBy", model.CreateBy),
                new SqlParameter("@LastUpdateBy", model.LastUpdateBy)
            };
                return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
            }
        }

        /// <summary>
        /// 添加轮胎数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddTireActivityPid(TireActivityPIDModel model)
        {
            var sql = @"INSERT INTO Activity.[dbo].[TireActivityPID]
                                ([TireActivityID]
                                   ,[PID]
                                   ,[Is_Deleted]
                                   ,[CreateDatetime]
                                   ,[LastUpdateDateTime]
                                   ,[CreateBy]
                                   ,[LastUpdateBy]
                                )
                        VALUES  ( @TireActivityID , 
                                  @PID,
                                  @Is_Deleted,
                                  GETDATE() ,
                                  GETDATE(),
                                  @CreateBy,
                                  @LastUpdateBy
                                 )
                        SELECT  SCOPE_IDENTITY();";
            using (var conn = new SqlConnection(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var parameters = new[]
            {
                new SqlParameter("@TireActivityID", model.TireActivityID),
                new SqlParameter("@PID", model.PID),
                new SqlParameter("@Is_Deleted", model.Is_Deleted),
                new SqlParameter("@CreateBy", model.CreateBy),
                new SqlParameter("@LastUpdateBy", model.LastUpdateBy)
            };
                return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
            }
        }

        #endregion
    }
}
