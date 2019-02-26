using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 数据访问-SE_GiftManageConfigDAL   
    /// </summary>
    public partial class SE_GiftManageConfigDAL
    {

        private static readonly Common.Logging.ILog logger = LogManager.GetLogger<SE_GiftManageConfigDAL>();

        public static IEnumerable<SE_GiftManageConfigModel> SelectPages(SqlConnection connection, int activtyType = 1, int pageIndex = 1, int pageSize = 20, string strWhere = "")
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT * ,COUNT(1) OVER () as TotalCount FROM Configuration.dbo.SE_GiftManageConfig WITH(NOLOCK) WHERE 1=1And activityType=@activtyType  {0}
                                ORDER BY Id desc
                              OFFSET (@pageIndex - 1) * @pageSize ROW
				FETCH NEXT @pageSize ROW ONLY; ";

                if (!string.IsNullOrWhiteSpace(strWhere))
                    sql = string.Format(sql, strWhere);
                else
                    sql = string.Format(sql, "", "");

                return conn.Query<SE_GiftManageConfigModel>(sql, new { pageIndex = pageIndex, pageSize = pageSize, activtyType = activtyType });
            }
        }

        public static SE_GiftManageConfigModel GetEntity(SqlConnection connection, int Id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT TOP 1 * FROM Configuration.dbo.SE_GiftManageConfig WITH(NOLOCK) WHERE Id = @Id ";
                return conn.Query<SE_GiftManageConfigModel>(sql, new { Id = Id })?.FirstOrDefault();
            }
        }

        public static int SetDisabled(SqlConnection connection)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"UPDATE  Configuration..SE_GiftManageConfig SET State =0 WHERE State=1 AND DATEADD (day ,7,ValidTimeEnd) <GETDATE() ";
                return conn.Execute(sql);
            }

        }
        public static bool InsertLog(SqlConnection connection, SE_DictionaryConfigModel model)
        {
            try
            {
                using (IDbConnection conn = connection)
                {
                    string sql = @" 
                                INSERT INTO Configuration.dbo.SE_DictionaryConfig
								(
									ParentId,
									[Key],
									[Value],
									Describe,
									Sort,
									State,
									Url,
									Images,
									CreateTime,
									UpdateTime,
									Extend1,
									Extend2,
									Extend3,
									Extend4,
									Extend5
								)
                                VALUES
                                (
									@ParentId,
									@Key,
									@Value,
									@Describe,
									@Sort,
									@State,
									@Url,
									@Images,
									@CreateTime,
									@UpdateTime,
									@Extend1,
									@Extend2,
									@Extend3,
									@Extend4,
									@Extend5
								)";
                    return conn.Execute(sql, model) > 0;
                }

            }
            catch (Exception ex)
            {

                //Logger.Log(Level.Info, $"赠品sql执行错误{ex}-{ex.InnerException}-{ex.StackTrace}");
                //Logger.Log(Level.Error, $"赠品sql执行错误{ex}-{ex.InnerException}-{ex.StackTrace}");
                throw ex;


            }

        }
        public static int Insert(SqlConnection connection, SE_GiftManageConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" 
                                INSERT INTO Configuration.dbo.SE_GiftManageConfig
								(
									Name,
									State,
									Limit,
									DonateWay,
									Describe,
									Visible,
									OrdersWay,
									ValidTimeBegin,
									ValidTimeEnd,
									Channel,
									Type,
									ConditionSize,
                                    TireType,
									Size,
									B_Categorys,
									B_Brands,
									B_PID,
									B_PID_Type,
									P_PID,
									GiftCondition,
									GiftNum,
									GiftMoney,
									GiftUnit,
									GiftType,
									GiftProducts,
									GiftDescribe,
									Creater,
									Mender,
									CreateTime,
									UpdateTime,
                                    TireSizeCondition,
                                    TireSize,
                                    CateGory,
                                    IsPackage,
                                    ActivityType,
                                    TagDisplay,
                                    GiveAway,
                                    PictureUrl,
                                    PictureUrl4Detail
								)
                                VALUES
                                (
									@Name,
									@State,
									@Limit,
									@DonateWay,
									@Describe,
									@Visible,
									@OrdersWay,
									@ValidTimeBegin,
									@ValidTimeEnd,
									@Channel,
									@Type,
									@ConditionSize,
                                    @TireType,
									@Size,
									@B_Categorys,
									@B_Brands,
									@B_PID,
									@B_PID_Type,
									@P_PID,
									@GiftCondition,
									@GiftNum,
									@GiftMoney,
									@GiftUnit,
									@GiftType,
									@GiftProducts,
									@GiftDescribe,
									@Creater,
									@Mender,
									@CreateTime,
									@UpdateTime,
                                    @TireSizeCondition,
                                    @TireSize,
                                    @Category,
                                    @IsPackage,
                                    @ActivityType,
                                    @TagDisplay,
                                    @GiveAway,
                                    @PictureUrl,
                                    @PictureUrl4Detail
								)SELECT @@IDENTITY";
                return conn.ExecuteScalar<int>(sql, model);
            }
        }

        public static bool Update(SqlConnection connection, SE_GiftManageConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" UPDATE  Configuration.dbo.SE_GiftManageConfig
                                SET	Name = @Name,									
									ValidTimeBegin = @ValidTimeBegin,
									ValidTimeEnd = @ValidTimeEnd,
									Channel = @Channel,
									GiftProducts = @GiftProducts,
									Creater = @Creater,
									Mender = @Mender,
									CreateTime = @CreateTime,
									UpdateTime = @UpdateTime,
                                    TagDisplay = @TagDisplay,
                                    GiveAway = @GiveAway,
                                    PictureUrl = @PictureUrl,
                                    PictureUrl4Detail = @PictureUrl4Detail
								WHERE Id = @Id ";
                return conn.Execute(sql, model) > 0;
            }
        }

        public static bool Delete(SqlConnection connection, int Id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = " DELETE Configuration.dbo.SE_GiftManageConfig WHERE Id = @Id ";
                return conn.Execute(sql, new { Id = Id }) > 0;
            }
        }


        #region 关联功能函数

        /// <summary>
        /// 获取支付方式渠道集合
        /// </summary>
        public static IEnumerable<U_ChannelPayModel> GetU_ChannelPayList(SqlConnection connection)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT * FROM Gungnir..U_ChannelPay WITH(NOLOCK) WHERE TYPE IN (3, 4, 5, 6) ORDER BY TYPE ";
                return conn.Query<U_ChannelPayModel>(sql, null);
            }
        }
        public static IEnumerable<ChannelDictionariesModel> GetU_ChannelPayListNew(SqlConnection connection)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT ChannelType,ChannelKey,ChannelValue FROM Gungnir..tbl_ChannelDictionaries WITH(NOLOCK) where ChannelType=N'H5合作渠道' or ChannelType=N'其他'  or ChannelType=N'第三方平台' or ChannelType=N'自有渠道' ORDER BY ChannelKey ";
                return conn.Query<ChannelDictionariesModel>(sql, null);
            }
        }

        /// <summary>
        /// 检测PID是否存在
        /// </summary>
        public static bool CheckPID(SqlConnection connection, string pid)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT top 1 oid FROM [Tuhu_productcatalog].[dbo].[vw_Products] WITH(NOLOCK) where PID = @PID ";
                return conn.ExecuteScalar<int>(sql, new { PID = pid }) > 0;
            }
        }

        /// <summary>
        /// 查询产品信息
        /// </summary>
        public static VW_ProductsModel GetVW_ProductsModel(SqlConnection connection, string pid)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT top 1 oid,PID,DisplayName,CY_List_Price FROM [Tuhu_productcatalog].[dbo].[vw_Products] WITH(NOLOCK) where PID = @PID and i_ClassType IN(2, 4)";
                return conn.Query<VW_ProductsModel>(sql, new { PID = pid })?.FirstOrDefault();
            }
        }


        /// <summary>
        /// 批量查询查询产品信息
        /// </summary>
        public static List<VW_ProductsModel> GetVW_ProductsModels(SqlConnection connection, List<string> pids)
        {
            using (IDbConnection conn = connection)
            {
               var pidStr= string.Join(",", pids.Select(t => $"'{t}'"));
                string sql = $@"SELECT  oid,PID,DisplayName,CY_List_Price FROM [Tuhu_productcatalog].[dbo].[vw_Products] WITH(NOLOCK) 
                where   i_ClassType IN(2, 4) and PID in ({pidStr}) ";
                //string sqlwhere = "";
                //var paras = new List<object>();
                ////if (pids.Any())
                //{
                //    for (int i = 0; i < pids.Count; i++)
                //    {
                //        sqlwhere +=  $" or PID =@PID{i} ";
                //        paras.Add(new SqlParameter("@PID" + i, pids[i]));
                //    }
                //}
                return conn.Query<VW_ProductsModel>(sql, null).ToList();
            }
        }

        /// <summary>
        /// 查询库存信息
        /// </summary>
        public static int? GetGiftProductStock(SqlConnection connection, string pid)
        {
            using (var cmd = new SqlCommand(@" SELECT top 1 Stock FROM Configuration..GiftStockManagerConfig WITH(NOLOCK) where PID = @PID and status =1"))
            {
                cmd.Parameters.AddWithValue("@Pid", pid);
                return DbHelper.ExecuteScalar(cmd) as int?;
            }
        }


        /// <summary>
        /// 插入库存信息
        /// </summary>
        public static int IntsertGiftProductStock(string pid, int stock)
        {
            using (var cmd = new SqlCommand(@"
                        INSERT INTO  Configuration..GiftStockManagerConfig (pid,Stock,Status,CreateDateTime,LastUpdateDateTime) VALUES(@Pid,@Stock,1,getdate(),getdate())"))
            {
                cmd.Parameters.AddWithValue("@Pid", pid);
                cmd.Parameters.AddWithValue("@Stock", stock);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 更新库存信息
        /// </summary>
        public static int UpdateGiftProductStock(string pid, int stock)
        {
            using (var cmd = new SqlCommand(@"
                        update  Configuration..GiftStockManagerConfig set Stock=@Stock,LastUpdateDateTime=getdate() where pid=@Pid"))
            {
                cmd.Parameters.AddWithValue("@Pid", pid);
                cmd.Parameters.AddWithValue("@Stock", stock);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// mergeinto库存信息
        /// </summary>
        public static int MergeIntoGiftProductStock(string pid, int? stock, int ruleId)
        {
            using (var cmd = new SqlCommand(@"
                              MERGE INTO Configuration..GiftStockManagerConfig AS T
          USING
            ( SELECT    @RuleId AS GiftId ,
                        @Pid AS Pid
            ) AS s
          ON T.GiftRuleId = s.GiftId
            AND T.Pid = s.Pid
          WHEN MATCHED THEN
            UPDATE SET
                    T.Stock = @Stock ,
                    T.LastUpdateDateTime = GETDATE()
          WHEN NOT MATCHED THEN
            INSERT ( Pid ,
                     Stock ,
                     Status ,
                     GiftRuleId ,
                     CreateDateTime ,
                     LastUpdateDateTime
                   )
            VALUES ( @Pid ,
                     @Stock ,
                     1 ,
                     @RuleId,
                     GETDATE() ,
                     GETDATE()
                   );"))
            {
                cmd.Parameters.AddWithValue("@Pid", pid);
                cmd.Parameters.AddWithValue("@Stock", stock);
                cmd.Parameters.AddWithValue("@RuleId", ruleId);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        #region 赠品信息维护到表中
        public static int DeleteGiftProductConfig(int ruleId)
        {
            using (var cmd = new SqlCommand(@"
                        Delete from Configuration..GiftProductConfig  where RuleId=@RuleId"))
            {
                cmd.Parameters.AddWithValue("@RuleId", ruleId);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static int InsertGiftProductConfig(GiftStockModel2 model)
        {
            using (var cmd = new SqlCommand(@"
                   INSERT  INTO Configuration..GiftProductConfig
                    ( RuleId ,
                      Pid ,
                      Num ,
                      Describe ,
                      Stock ,
                      IsRetrieve
                    )
            VALUES  ( @RuleId ,
                      @Pid ,
                      @Num ,
                      @Describe,
                      @Stock ,
                      @IsRetrieve
                    )"))
            {
                cmd.Parameters.AddWithValue("@RuleId", model.RuleId);
                cmd.Parameters.AddWithValue("@Pid", model.Pid);
                cmd.Parameters.AddWithValue("@Num", model.Num);
                cmd.Parameters.AddWithValue("@Describe", model.Describe);
                cmd.Parameters.AddWithValue("@Stock", model.Stock);
                cmd.Parameters.AddWithValue("@IsRetrieve", model.IsRetrieve);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        #endregion
        /// <summary>
        /// 删除库存信息
        /// </summary>
        public static int DeleteGiftProductStock(string pid)
        {
            using (var cmd = new SqlCommand(@"
                        Delete from Configuration..GiftStockManagerConfig  where pid=@Pid"))
            {
                cmd.Parameters.AddWithValue("@Pid", pid);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }


        public static int UpdateGiftLeveL(int pkid, int sort, string group)
        {
            using (var cmd = new SqlCommand(@"update  Configuration..SE_GiftManageConfig set [Group]=@Group,UpdateTime=getdate(),Sort=@Sort where Id=@PKid"))
            {
                cmd.Parameters.AddWithValue("@PKid", pkid);
                cmd.Parameters.AddWithValue("@Sort", sort);
                cmd.Parameters.AddWithValue("@Group", group);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static IEnumerable<SE_GiftManageConfigModel> SelectGiftLeveL(SqlConnection connection, string group)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"Select * from   Configuration..SE_GiftManageConfig where [Group]=@Group ";

                return conn.Query<SE_GiftManageConfigModel>(sql, new { Group = group });
            }
        }

        public static IEnumerable<SE_GiftManageConfigModel> SelectGiftLeveLs(SqlConnection connection)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"Select * from   Configuration..SE_GiftManageConfig where [Group] is not null ";

                return conn.Query<SE_GiftManageConfigModel>(sql);
            }
        }

        public static int DeleteGiftLeveLs(string group)
        {
            using (var cmd = new SqlCommand(
                        @"update  Configuration..SE_GiftManageConfig set [Group]=null,UpdateTime=getdate(),Sort=null where [Group]= @Group"))
            {
                cmd.Parameters.AddWithValue("@Group", group);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        public static int SelectGiftStock(string pid)
        {
            using (var cmd = new SqlCommand(@"SELECT Stock
                                              FROM    Configuration..GiftStockManagerConfig AS GS WITH ( NOLOCK ) WHERE GS.Status = 1 AND GS.Pid=@pid"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@pid", pid);
                return Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
            }
        }
        #endregion

        public static string GetPids(SqlConnection connection, int id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT P_PID FROM Configuration..SE_GiftManageConfig with(nolock) where id=@ID ";
                return conn.Query<string>(sql, new { ID = id })?.FirstOrDefault();
            }
        }

        public static IEnumerable<SE_GiftManageConfigModel> SelectAllGiftManageConfigModels(SqlConnection connection)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"
                    SELECT  *
                    FROM    Configuration..SE_GiftManageConfig WITH ( NOLOCK )
                    WHERE   ActivityType = 1
                            AND State = 1";
                return conn.Query<SE_GiftManageConfigModel>(sql);
            }
        }

        public static List<string> GetByAllNodes(SqlConnection connection)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"
             SELECT cc.NodeNo
             FROM   Tuhu_productcatalog..vw_Products AS P WITH ( NOLOCK )
                    JOIN Tuhu_productcatalog..[CarPAR_CatalogHierarchy] (NOLOCK)
                    AS cc ON P.oid = cc.child_oid
             WHERE  cc.NodeNo LIKE '28656.%'";
                return conn.Query<string>(sql).ToList();
            }
        }

        /// <summary>
        /// 更新模块顺序
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="modules"></param>
        /// <returns></returns>
        public bool BatchUpdateTime(SqlConnection connection, IEnumerable<SE_GiftManageConfigModel> modules)
        {
            var trans = connection.BeginTransaction();
            try
            {
                foreach (var item in modules)
                    connection.Execute("UPDATE Configuration.dbo.SE_GiftManageConfig SET ValidTimeBegin = @ValidTimeBegin, ValidTimeEnd = @ValidTimeEnd WHERE ID = @ID ", item, trans);
                trans.Commit();
                return true;
            }
            catch (Exception)
            {
                trans.Rollback();
                return false;
            }
        }

        /// <summary>
        /// 查询进行中活动所有配置买三送一的数据
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IEnumerable<SE_GiftManageConfigModel> SelectGiveAwayList(SqlConnection connection, int id = 0)
        {
            try
            {
                using (IDbConnection conn = connection)
                {
                    var sql = @"SELECT  P_PID,Id
                                    FROM    Configuration..SE_GiftManageConfig
                                    WHERE   GiveAway = 1
                                            AND State = 1
		                                    AND Type = 4
                                            AND ValidTimeEnd > GETDATE()
                                            AND ValidTimeBegin < GETDATE()";
                    if (id > 0)
                    {
                        sql += $"AND Id != {id}";
                    }
                    return conn.Query<SE_GiftManageConfigModel>(sql);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"SelectGiveAwayList:{ex.Message}", ex);
                return new List<SE_GiftManageConfigModel>();
            }
        }
    }
}
