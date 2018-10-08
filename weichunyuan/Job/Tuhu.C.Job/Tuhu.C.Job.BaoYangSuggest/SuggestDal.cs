using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.C.Job.BaoYangSuggest.Model;
namespace Tuhu.C.Job.BaoYangSuggest
{
    public class SuggestDal
    {
        public static int SelectMaxId()
        {
            using(var cmd = new SqlCommand(@"SELECT  MAX(PKID) FROM    Gungnir..tbl_Order AS o WITH ( NOLOCK );"))
            {
                cmd.CommandType = CommandType.Text;
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
        }

        public static int UpdateNewData(int index)
        {
            using (var cmd = new SqlCommand(@"UPDATE  BaoYang..UserBaoYangRecords
        SET     OrderPrice = o.SumMoney ,
                InstallShopId = o.InstallShopID ,
                InstallShopName = s.CarparName
        FROM    BaoYang..UserBaoYangRecords AS re WITH ( NOLOCK ),
                Gungnir..tbl_Order AS o WITH ( NOLOCK ) ,
                Gungnir..vw_Shop AS s WITH ( NOLOCK )                
        WHERE   re.IsTuhuRecord = 1
                AND re.RelatedOrderID < @Index * 10000
                AND re.RelatedOrderID > ( @Index - 1 ) * 10000
                AND o.InstallShopID = s.PKID
                AND o.PKID = re.RelatedOrderID;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("@Index", index);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 分页获取取消的订单号
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<int> SelectCanceledOrderIds(int pageIndex, int pageSize)
        {
            using (SqlCommand cmd = new SqlCommand(@"SELECT  o.PKID
                                                    FROM    Gungnir..tbl_Order AS o WITH ( NOLOCK )
                                                    WHERE   LastUpdateTime > DATEADD(DAY, -3, GETDATE())
                                                            AND o.Status = '7Canceled'
                                                    ORDER BY o.PKID DESC
                                                            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                                            ONLY;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.CommandTimeout = 0;
                return DbHelper.ExecuteQuery<List<int>>(true, cmd, ConvertDt2List<int>);
            }
        }

        /// <summary>
        /// 分页获取更新的订单号（不含取消）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int SelectMinOrderIdInHalfYear()
        {
            using (SqlCommand cmd = new SqlCommand(@"select min(pkid)
                                                    from Gungnir..tbl_Order with(nolock)
                                                    where OrderDatetime > dateadd(day, -180, getdate())"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                return (int)DbHelper.ExecuteScalar(true, cmd);
            }
        }

        /// <summary>
        /// 分页获取更新的订单号（不含取消）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<int> SelectUpdatedOrderIdsInHalfYear(int minOrderId, int pageIndex, int pageSize)
        {
            using (SqlCommand cmd = new SqlCommand(@"select RelatedOrderID
                                                    from (select DISTINCT RelatedOrderID
                                                          from baoyang..UserBaoYangRecords with (nolock)
                                                          where RelatedOrderID > @OrderId) as t
                                                    ORDER BY t.RelatedOrderID ASC OFFSET (@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                                    ONLY;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderId", minOrderId);
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.CommandTimeout = 0;
                return DbHelper.ExecuteQuery<List<int>>(true, cmd, ConvertDt2List<int>);
            }
        }

        /// <summary>
        /// 分页获取更新的订单号（不含取消）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<int> SelectUpdatedOrderIds(int pageIndex, int pageSize)
        {
            using (SqlCommand cmd = new SqlCommand(@"SELECT  o.PKID
                                                    FROM    Gungnir..tbl_Order AS o WITH ( NOLOCK )
                                                    WHERE   (LastUpdateTime > DATEADD(DAY, -3, GETDATE()) or
                                                            InstallDatetime > DATEADD(DAY, -3, GETDATE()))
                                                            AND o.Status <> '7Canceled'
                                                    ORDER BY o.PKID ASC
                                                            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                                            ONLY;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.CommandTimeout = 0;
                return DbHelper.ExecuteQuery<List<int>>(true, cmd, ConvertDt2List<int>);
            }
        }

        public static List<UserBaoYangRecordModel> SelectBaoYangRecordsFromMaintainedData(List<int> orderIds)
        {
            using (SqlCommand cmd = new SqlCommand(@"SELECT  record.PKID ,
                                                            record.UserID ,
                                                            record.VechileID ,
                                                            record.UserCarID ,
                                                            record.BaoYangDateTime ,
                                                            record.Distance ,
                                                            record.BaoYangType ,
                                                            record.RelatedOrderID ,
                                                            record.RelatedOrderNo ,
                                                            record.Status ,
                                                            record.IsTuhuRecord ,
                                                            record.OrderPrice ,
                                                            record.InstallShopId ,
                                                            record.InstallShopName ,
                                                            record.CreatedDateTime ,
                                                            record.UpdatedDateTime ,
                                                            record.IsDeleted
                                                    FROM    BaoYang..UserBaoYangRecords AS record WITH ( NOLOCK )
                                                            JOIN Gungnir.dbo.SplitString(@OrderIds, ',', 1) AS ids ON record.RelatedOrderID = ids.Item;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderIds", string.Join(",", orderIds));
                cmd.CommandTimeout = 0;
                return DbHelper.ExecuteSelect<UserBaoYangRecordModel>(true, cmd).ToList();
            }
        }

        public static List<UserBaoYangRecordModel> SelectBaoYangRecordsFromOrder(List<int> orderIds)
        {
            using (SqlCommand cmd = new SqlCommand(@"SELECT DISTINCT
                                                                o.UserID ,
                                                                o.CarID AS UserCarID ,
                                                                case
                                                                  when o.InstallDatetime is not null then o.InstallDatetime
                                                                  else o.OrderDatetime end AS BaoYangDateTime,
                                                                ol.PID ,
                                                                ol.Category ,
                                                                o.PKID AS RelatedOrderId ,
                                                                o.OrderNo AS RelatedOrderNo ,
                                                                CASE WHEN o.Status = '3Installed'
                                                                          OR o.Status = '6Complete' THEN 1
                                                                     ELSE 2
                                                                END AS Status ,
                                                                1 AS IsTuhuRecord ,
                                                                o.SumMoney AS OrderPrice ,
                                                                s.PKID AS InstallShopId ,
                                                                s.CarparName AS InstallShopName
                                                        FROM    Gungnir.dbo.tbl_Order AS o WITH ( NOLOCK )
                                                                JOIN Gungnir..SplitString(@OrderIds, ',', 1) AS ids ON ids.Item = o.PKID
                                                                JOIN Gungnir.dbo.tbl_OrderList AS ol WITH ( NOLOCK ) ON ol.OrderID = o.PKID
                                                                LEFT JOIN Gungnir..Shops AS s WITH ( NOLOCK ) ON s.PKID = o.InstallShopID
                                                        WHERE   o.CarID IS NOT NULL AND o.Status <> '7Canceled'
                                                                AND ( o.InstallType = '1ShopInstall'
                                                                      OR o.InstallType = '4SentInstall'
                                                                    )
                                                                AND ol.Deleted = 0"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderIds", string.Join(",", orderIds));
                return DbHelper.ExecuteSelect<UserBaoYangRecordModel>(true, cmd).ToList();
            }
        }

        public static List<OrderCarModel> SelectCarsFormOrder(List<int> orderIds)
        {
            using (SqlCommand cmd = new SqlCommand(@"select o.PKID AS OrderId, oc.VehicleId, oc.Distance, oc.UserCreated
                        from Gungnir..tbl_Order as o with(nolock)
                               JOIN Gungnir..SplitString(@OrderIds, ',', 1) AS ids ON ids.Item = o.PKID
                               Join Tuhu_order..tbl_OrderCar as oc WITH (NOLOCK) on oc.OrderNo = o.OrderNo collate Chinese_PRC_CI_AS"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderIds", string.Join(",", orderIds));
                return DbHelper.ExecuteSelect<OrderCarModel>(true, cmd).ToList();
            }
        }

        public static List<UserBaoYangRecordConfig> SelectUserBaoYangRecordConfig()
        {
            using (SqlCommand cmd = new SqlCommand(@"DECLARE @xml XML;
                                                    SET @xml = ( SELECT TOP 1
                                                                        Config
                                                                 FROM   BaoYang.dbo.BaoYangConfig With(nolock)
                                                                 WHERE  ConfigName = 'UserBaoYangRecordConfig'
                                                               );
                                                    SELECT  S.value('@packageType[1]', 'nvarchar(100)') AS PackageType ,
                                                            S.value('@serviceId[1]', 'nvarchar(100)') AS ServiceId ,
                                                            S.value('@productCategory[1]', 'nvarchar(100)') AS ProductCategory
                                                    FROM    @xml.nodes('UserBaoYangRecordConfig/UserBaoYangRecordConfigItem') T ( S );"))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteSelect<UserBaoYangRecordConfig>(true, cmd).ToList();
            }
        }

        private static List<T> ConvertDt2List<T>(DataTable dt)
        {
            List<T> result = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    T item = row.IsNull(0) ? default(T) : (T)row[0];
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// 删除被取消的订单对应的保养档案
        /// </summary>
        /// <param name="orderIds"></param>
        public static void DeleteBaoYangRecord(List<int> orderIds)
        {
            using (SqlCommand cmd = new SqlCommand(@"UPDATE  BaoYang..UserBaoYangRecords
                                                    SET     IsDeleted = 1
                                                    WHERE   EXISTS ( SELECT 1
                                                                        FROM   Gungnir.dbo.SplitString(@OrderIds, ',', 1) AS Ids
                                                                        WHERE  Ids.Item = RelatedOrderID )
                                                            AND IsDeleted = 0;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderIds", string.Join(",", orderIds));
                cmd.CommandTimeout = 0;
                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static void UpdateBaoYangRecord(List<UserBaoYangRecordModel> records)
        {
            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                foreach (var record in records)
                {
                    using (var cmd = new SqlCommand(@"UPDATE  BaoYang..UserBaoYangRecords
                                            SET     UserID = @UserID,
                                                    VechileID = @VechileID,
                                                    UserCarID = @UserCarID,
                                                    BaoYangDateTime = @BaoYangDateTime,
                                                    Distance = @Distance,
                                                    RelatedOrderID = @RelatedOrderID,
                                                    RelatedOrderNo = @RelatedOrderNo,
                                                    Status = @Status,
                                                    IsDeleted = @IsDeleted,
                                                    OrderPrice = @OrderPrice,
                                                    InstallShopId = @InstallShopId,
                                                    InstallShopName = @InstallShopName
                                            WHERE   PKID = @PKID; "))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@PKID", record.PKID);
                        cmd.Parameters.AddWithValue("@UserID", record.UserID);
                        cmd.Parameters.AddWithValue("@VechileID", record.VechileID);
                        cmd.Parameters.AddWithValue("@UserCarID", record.UserCarID);
                        cmd.Parameters.AddWithValue("@BaoYangDateTime", record.BaoYangDateTime);
                        cmd.Parameters.AddWithValue("@Distance", record.Distance);
                        cmd.Parameters.AddWithValue("@RelatedOrderID", record.RelatedOrderID);
                        cmd.Parameters.AddWithValue("@RelatedOrderNo", record.RelatedOrderNo);
                        cmd.Parameters.AddWithValue("@Status", record.Status);
                        cmd.Parameters.AddWithValue("@IsDeleted", record.IsDeleted);
                        cmd.Parameters.AddWithValue("@OrderPrice", record.OrderPrice);
                        cmd.Parameters.AddWithValue("@InstallShopId", record.InstallShopId);
                        cmd.Parameters.AddWithValue("@InstallShopName", record.InstallShopName);
                        dbhelper.ExecuteNonQuery(cmd);
                    }
                }
            }
        }

        /// <summary>
        /// 删除pkid对应的保养档案
        /// </summary>
        /// <param name="orderIds"></param>
        public static void DeleteBaoYangRecordByPkids(List<int> pkids)
        {
            using (SqlCommand cmd = new SqlCommand(@"UPDATE  BaoYang..UserBaoYangRecords
                                                    SET     IsDeleted = 1
                                                    WHERE   EXISTS ( SELECT 1
                                                                        FROM   Gungnir.dbo.SplitString(@PKIDs, ',', 1) AS Ids
                                                                        WHERE  Ids.Item = PKID )
                                                            AND IsDeleted = 0;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKIDs", string.Join(",", pkids));
                cmd.CommandTimeout = 0;
                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 删除pkid对应的保养档案
        /// </summary>
        /// <param name="orderIds"></param>
        public static void InsertBaoYangRecords(List<UserBaoYangRecordModel> records)
        {
            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                foreach (var record in records)
                {
                    using (SqlCommand cmd = new SqlCommand(@"INSERT  INTO BaoYang..UserBaoYangRecords
                                                                ( UserID ,
                                                                  VechileID ,
                                                                  UserCarID ,
                                                                  BaoYangDateTime ,
                                                                  Distance ,
                                                                  BaoYangType ,
                                                                  RelatedOrderID ,
                                                                  RelatedOrderNo ,
                                                                  Status ,
                                                                  IsTuhuRecord ,
                                                                  OrderPrice,
                                                                  InstallShopId,
                                                                  InstallShopName,
                                                                  CreatedDateTime ,
                                                                  UpdatedDateTime ,
                                                                  IsDeleted
                                                                )
                                                        VALUES  ( @UserID ,
                                                                  @VechileID ,
                                                                  @UserCarID ,
                                                                  @BaoYangDateTime ,
                                                                  @Distance ,
                                                                  @BaoYangType ,
                                                                  @RelatedOrderID ,
                                                                  @RelatedOrderNo ,
                                                                  @Status ,
                                                                  1 ,
                                                                  @OrderPrice,
                                                                  @InstallShopId,
                                                                  @InstallShopName,
                                                                  GETDATE() ,
                                                                  GETDATE() ,
                                                                  0  
                                                                );"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@UserID", record.UserID);
                        cmd.Parameters.AddWithValue("@VechileID", record.VechileID);
                        cmd.Parameters.AddWithValue("@UserCarID", record.UserCarID);
                        cmd.Parameters.AddWithValue("@BaoYangDateTime", record.BaoYangDateTime);
                        cmd.Parameters.AddWithValue("@Distance", record.Distance);
                        cmd.Parameters.AddWithValue("@BaoYangType", record.BaoYangType);
                        cmd.Parameters.AddWithValue("@RelatedOrderID", record.RelatedOrderID);
                        cmd.Parameters.AddWithValue("@RelatedOrderNo", record.RelatedOrderNo);
                        cmd.Parameters.AddWithValue("@Status", record.Status);
                        cmd.Parameters.AddWithValue("@OrderPrice", record.OrderPrice);
                        cmd.Parameters.AddWithValue("@InstallShopId", record.InstallShopId);
                        cmd.Parameters.AddWithValue("@InstallShopName", record.InstallShopName);
                        dbhelper.ExecuteNonQuery(cmd);
                    }
                }
            }
        }

        public static void UpdateBaoYangRecord()
        {
            using (DbCommand cmd = new SqlCommand("BaoYang..BaoYang_MaintianUserBaoYangRecord"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static string GetBaoYangSuggestConfig()
        {
            using (
                var cmd =
                    new SqlCommand("SELECT Config FROM baoyang..BaoYangConfig WHERE ConfigName='BaoYangSuggestConfig'"))
            {
                return (string)DbHelper.ExecuteScalar(cmd);
            }
        }

        public static Tuple<bool, int> AddCarObjectRecord()
        {
            Tuple<bool, int> result = null;
            using (DbCommand cmd = new SqlCommand(@" IF ( NOT EXISTS ( SELECT    1
                      FROM      BaoYangSuggest..SuggestJobExecuteTime(NOLOCK) )
       )
        OR ( EXISTS ( SELECT    1
                      FROM      BaoYang..BaoYangConfig(NOLOCK)
                      WHERE     DATEDIFF(DAY, UpdatedTime, GETDATE()) = 1
                                AND ConfigName = 'BaoYangSuggestConfig' ) )
        BEGIN 
            SET @IsFirstTime = 1
        END  
    ELSE
        BEGIN   
            SET @IsFirstTime = 0;
		
            TRUNCATE TABLE   BaoYangSuggest..CarObjectRecord;
            DECLARE @lastTime DATETIME;
            SELECT TOP 1
                    @lastTime = ExecuteTime
            FROM    BaoYangSuggest..SuggestJobExecuteTime(NOLOCK)
            ORDER BY PKID DESC
            IF ( @lastTime = NULL )
                BEGIN
                    SET @lastTime = DATEADD(DAY, -1, GETDATE())
                END;

            SELECT DISTINCT
                    UserID ,
                    VechileID,
                    UserCarID
            INTO    #UserVechileRecordTemp
            FROM    BaoYang..UserBaoYangRecords br ( NOLOCK )
            WHERE   br.CreatedDateTime > DATEADD(MONTH,-2,GETDATE())
                    OR br.UpdatedDateTime > DATEADD(MONTH,-2,GETDATE())
                         
            INSERT  INTO BaoYangSuggest..CarObjectRecord
                    SELECT  UserID ,
                            u_cartype_pid_vid ,
                            OdometerUpdatedTime ,
                            i_km_total ,
                            i_buy_year ,
                            i_buy_month ,
                            co.CarID,
							co.u_Nian
                    FROM    Tuhu_profiles..CarObject co ( NOLOCK )
                    WHERE   OdometerUpdatedTime > @lastTime                            
                            AND co.IsDeleted = 0
                            AND i_km_total IS NOT NULL
                            AND i_km_total != 0                          
                            AND UserID IS NOT NULL
                            AND u_cartype_pid_vid IS NOT NULL 
          
                INSERT   INTO BaoYangSuggest..CarObjectRecord
                            SELECT  RT.UserID ,
                                    co.u_cartype_pid_vid ,
                                    co.OdometerUpdatedTime ,
                                    co.i_km_total ,
                                    co.i_buy_year ,
                                    co.i_buy_month ,
                                    co.CarID ,
                                    co.u_Nian
                            FROM    #UserVechileRecordTemp RT ( NOLOCK )
                                    JOIN Tuhu_profiles..CarObject co ( NOLOCK ) ON RT.UserCarID = co.CarID
                            WHERE   co.IsDeleted = 0
                                    AND co.i_km_total IS NOT NULL
                                    AND co.i_km_total != 0
                                    AND co.u_cartype_pid_vid IS NOT NULL;  
                 
                 
                                                          
          	
            DROP TABLE #UserVechileRecordTemp
        END 

    INSERT  INTO BaoYangSuggest..SuggestJobExecuteTime
    VALUES  ( GETDATE() )

    SELECT  @CarObjectRecordNum = COUNT(1)
    FROM    BaoYangSuggest..CarObjectRecord (NOLOCK)"))
            {

                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@CarObjectRecordNum", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                });
                cmd.Parameters.Add(new SqlParameter("@IsFirstTime", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                });

                DbHelper.ExecuteNonQuery(cmd);
                result = new Tuple<bool, int>(Convert.ToBoolean(cmd.Parameters["@IsFirstTime"].Value),
                    Convert.ToInt32(cmd.Parameters["@CarObjectRecordNum"].Value));
            }

            return result;
        }

        public static DataTable GetCarObjectAndBaoYangRecord(int startIndex, int endIndex)
        {
            using (DbCommand cmd = new SqlCommand(@"
        SELECT  UserID ,
                u_cartype_pid_vid ,
                OdometerUpdatedTime ,
                i_km_total ,
                i_buy_year ,
                i_buy_month ,
                CarID ,
                u_Nian
        INTO    #CarObjectTemp
        FROM    BaoYangSuggest.[dbo].CarObjectTemp co ( NOLOCK )       
        ORDER BY UserID DESC
                OFFSET @StartIndex ROWS
								FETCH NEXT @EndIndex - @StartIndex + 1 ROWS
                ONLY
                               
        SELECT  co.UserID ,
                co.CarID ,
                co.u_cartype_pid_vid ,
                co.OdometerUpdatedTime ,
                co.i_km_total ,
                co.i_buy_year ,
                co.u_Nian ,
                co.i_buy_month ,
                br.BaoYangDateTime ,
                br.BaoYangType ,
                br.Distance ,
                br.IsTuhuRecord
        FROM    #CarObjectTemp co
                LEFT JOIN BaoYang..UserBaoYangRecords AS br WITH ( NOLOCK ) ON co.UserID = br.UserID
                                                              AND co.u_cartype_pid_vid = br.VechileID
        WHERE   ( br.Status <> 0
                  OR br.Status IS NULL
                )
                AND ( br.IsDeleted = 0
                      OR br.IsDeleted IS NULL
                    )

        DROP TABLE #CarObjectTemp "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 60;
                cmd.Parameters.Add(new SqlParameter("@StartIndex", startIndex));
                cmd.Parameters.Add(new SqlParameter("@EndIndex", endIndex));
                return DbHelper.ExecuteQuery(true, cmd, (dt) => dt);
            }
        }

        public static DataTable GetCarObjectRecordAndBaoYangRecord(int startIndex, int endIndex)
        {
            using (DbCommand cmd = new SqlCommand(@"
        SELECT  UserID ,
                u_cartype_pid_vid ,
                OdometerUpdatedTime ,
                i_km_total ,
                i_buy_year ,
                i_buy_month ,
                CarId ,
                u_Nian
        INTO    #CarObjectRecordTemp
        FROM    BaoYangSuggest.dbo.CarObjectRecord (NOLOCK)
        ORDER BY UserID DESC
                OFFSET @StartIndex ROWS
								FETCH NEXT @EndIndex - @StartIndex + 1 ROWS
                ONLY
                               
        SELECT  co.UserID ,
                co.CarId ,
                co.u_cartype_pid_vid ,
                co.OdometerUpdatedTime ,
                co.i_km_total ,
                co.i_buy_year ,
                co.u_Nian ,
                co.i_buy_month ,
                br.BaoYangDateTime ,
                br.BaoYangType ,
                br.Distance ,
                br.IsTuhuRecord
        FROM    #CarObjectRecordTemp co
                LEFT JOIN BaoYang..UserBaoYangRecords AS br WITH ( NOLOCK ) ON co.UserID = br.UserID
                                                              AND co.u_cartype_pid_vid = br.VechileID
        WHERE   ( br.Status <> 0
                  OR br.Status IS NULL
                )
                AND ( br.IsDeleted = 0
                      OR br.IsDeleted IS NULL
                    )
        DROP TABLE #CarObjectRecordTemp"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@StartIndex", startIndex));
                cmd.Parameters.Add(new SqlParameter("@EndIndex", endIndex));
                return DbHelper.ExecuteQuery(true, cmd, (dt) => dt);
            }
        }

        public static void UpdateBaoYangSuggest(DataTable suggestData)
        {
            DbParameter parameter = new SqlParameter("@SuggestData", suggestData);
            DbHelper.ExecuteNonQuery("BaoYangSuggest..MergeIntoBaoYangSuggest",
                CommandType.StoredProcedure, parameter);
        }

        public static bool TemporarySaveActiveUserSuggest()
        {
            using (var cmd = new SqlCommand("BaoYangSuggest..TemporarySaveActiveUserSuggest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static List<SuggestModel> GetActiveUserSuggestModels(int startIndex, int endIndex)
        {
            using (var cmd = new SqlCommand("BaoYangSuggest..SelectActiveUserSuggest"))
            {
                cmd.Parameters.AddWithValue("@StartIndex", startIndex);
                cmd.Parameters.AddWithValue("@EndIndex", endIndex);
                cmd.CommandType = CommandType.StoredProcedure;
                return DbHelper.ExecuteQuery(cmd, (dt) => dt).AsEnumerable().Select(r => new SuggestModel()
                {
                    UserId = r["UserId"] == null ? Guid.Empty : new Guid(r["UserId"].ToString()),
                    CarId = r["CarId"] == null ? Guid.Empty : new Guid(r["CarId"].ToString()),
                    VehicleId = r["VehicleId"]?.ToString() ?? string.Empty,
                    SuggestNum = r["SuggestNum"] == null ? 0 : Int32.Parse(r["SuggestNum"].ToString()),
                    UrgentNum = r["UrgentNum"] == null ? 0 : Int32.Parse(r["UrgentNum"].ToString()),
                    VeryUrgentNum = r["VeryUrgentNum"] == null ? 0 : Int32.Parse(r["VeryUrgentNum"].ToString()),
                }).ToList();
            }
        }

        /// <summary>
        /// 获取订单里程关联数据
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public static List<OrderCarModel> SelectBaoYangRecordOrderDistance(List<int> orderIds)
        {
            var sql = @"SELECT  s.OrderId ,
                                s.Distance
                        FROM    BaoYang..BaoYangRecordOrderDistance AS s WITH ( NOLOCK )
                                INNER JOIN BaoYang..SplitString(@OrderIds, ',', 1) AS v
                                ON v.Item = s.OrderId;";
            using (SqlCommand cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderIds", string.Join(",", orderIds));
                return DbHelper.ExecuteSelect<OrderCarModel>(true, cmd).ToList();
            }
        }
    }
}
