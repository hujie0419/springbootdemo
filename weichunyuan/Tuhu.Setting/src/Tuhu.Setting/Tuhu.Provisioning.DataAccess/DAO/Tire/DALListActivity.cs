using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity.Tire;

namespace Tuhu.Provisioning.DataAccess.DAO.Tire
{
    public class DALListActivity
    {
        public static IEnumerable<ActivityItem> SelectList(ListActCondition model, PagerModel pager)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                pager.TotalItem = GetTotalCount(sqlHelper, model);
                #region sql
                var sql = @"SELECT  TT.BrandCategory ,
        TT.Brand ,
        TT.Vehicle ,
        TT.VehicleId ,
        TT.TireSize ,
        TT.MinPrice ,
        TLA.ActivityID ,
        TLA.ActivityName ,
        TLA.Status ,
        TLA.StartTime ,
        TLA.EndTime ,
        TLA.Image ,
        TLA.Image2 ,
        TLA.Icon ,
        TLA.GetRuleGUID ,
        TLA.CreateTime ,
        TLA.UpdateTime ,
        TLA.ButtonType ,
        TLA.ButtonText,
        TLA.Sort
FROM    ( SELECT    T.BrandCategory ,
                    T.Brand ,
                    T.Vehicle ,
                    T.MinPrice ,
                    T.TiresMatch ,
                    T.VehicleId ,
                    SS.Item AS TireSize ,
                    ( SELECT    COUNT(1)
                      FROM      Gungnir..tbl_Vehicle_Type AS V WITH ( NOLOCK )
                                CROSS APPLY Gungnir..SplitString(V.TiresMatch,
                                                              ';', 1) AS SS
                                JOIN Tuhu_productcatalog.dbo.vw_Products AS VP
                                WITH ( NOLOCK ) ON SS.Item = VP.PID COLLATE Chinese_PRC_CI_AS
                                                   AND VP.TireSize = TireSize COLLATE Chinese_PRC_CI_AS
                      WHERE     VP.CP_Tire_ROF = N'非防爆'
                                AND V.ProductID = T.VehicleId
                    ) NotRofCount ,
                    ( SELECT    COUNT(1)
                      FROM      Gungnir..tbl_Vehicle_Type AS V WITH ( NOLOCK )
                                CROSS APPLY Gungnir..SplitString(V.TiresMatch,
                                                              ';', 1) AS SS
                                JOIN Tuhu_productcatalog.dbo.vw_Products AS VP
                                WITH ( NOLOCK ) ON SS.Item = VP.PID COLLATE Chinese_PRC_CI_AS
                                                   AND VP.TireSize = TireSize COLLATE Chinese_PRC_CI_AS
                      WHERE     VP.CP_Tire_ROF = N'防爆'
                                AND V.ProductID = T.VehicleId
                    ) RofCount
          FROM      ( SELECT    VT.BrandCategory ,
                                VT.Brand ,
                                VT.Vehicle ,
                                VT.Tires ,
                                VT.MinPrice ,
                                VT.TiresMatch ,
                                VT.ProductID AS VehicleId
                      FROM      Gungnir.dbo.tbl_Vehicle_Type AS VT WITH ( NOLOCK )
                      WHERE     ( VT.ProductID IN (
                                  SELECT    *
                                  FROM      Gungnir..Split(@VehicleId, ';') AS S )
                                  OR @VehicleId = N'ALL'
                                )
                                AND ( VT.BrandCategory IN (
                                      SELECT    *
                                      FROM      Gungnir..Split(@Department,
                                                              ';') AS S )
                                      OR @Department = N'ALL'
                                      OR ( CHARINDEX(N'Other', @Department) > 0
                                           AND ISNULL(VT.BrandCategory, '') = ''
                                         )
                                    )
                                AND ( @PriceRange = N'ALL'
                                      OR ( CHARINDEX(N'High', @PriceRange) > 0
                                           AND VT.MinPrice >= 16
                                         )
                                      OR ( CHARINDEX(N'Middle', @PriceRange) > 0
                                           AND VT.MinPrice < 16
                                           AND VT.MinPrice >= 8
                                         )
                                      OR ( CHARINDEX(N'Low', @PriceRange) > 0
                                           AND VT.MinPrice < 8
                                           AND ISNULL(VT.MinPrice, 0) > 0
                                         )
                                    )
                                AND ( @VehicleBodyType = N'ALL'
                                      OR VT.VehicleBodyType IN (
                                      SELECT    *
                                      FROM      Gungnir..Split(@VehicleBodyType,
                                                              ';') AS S2 )
                                    )
                                AND ISNULL(VT.Tires, '') <> ''
                    ) AS T
                    CROSS APPLY Gungnir..SplitString(T.Tires, ';', 1) AS SS
          WHERE     ( @TireSize = N'ALL'
                      OR SS.Item IN (
                      SELECT    *
                      FROM      Gungnir.dbo.Split(@TireSize, ';') AS S3 )
                    )
        ) AS TT
        LEFT JOIN ( SELECT  *
                    FROM    Activity.dbo.tbl_TireListActivity AS TL WITH ( NOLOCK )
                    WHERE   TL.Status <> -1
                  ) AS TLA ON TLA.VehicleID = TT.VehicleId COLLATE Chinese_PRC_CI_AS
                              AND TLA.TireSize = TT.TireSize COLLATE Chinese_PRC_CI_AS
WHERE   ( {3} )
        AND ( {0} )
        AND ( {6} )
        AND ( {1} )
        AND ( {4} )
        AND ( {5} )
        AND ( {2} )
        AND ( {7} )
ORDER BY TT.Brand
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;";
                #endregion

                #region IngTypeCondition
                string IngTypeCondition = null;
                if (model.IngType == 1)
                {
                    IngTypeCondition = "  TLA.StartTime <= GETDATE() AND TLA.EndTime > GETDATE() ";
                }
                else if (model.IngType == -1)
                {
                    IngTypeCondition = " TLA.StartTime > GETDATE() ";
                }
                else if (model.IngType == -2)
                {
                    IngTypeCondition = "  TLA.EndTime <= GETDATE() ";
                }
                else
                {
                    IngTypeCondition = " 0=0 ";
                }
                #endregion

                #region IsActiveCondition
                string IsActiveCondition = null;
                if (model.IsActive == 1)
                {
                    IsActiveCondition = "  TLA.Status = 1 ";
                }
                else if (model.IsActive == -1)
                {
                    IsActiveCondition = "  TLA.Status = 0 ";
                }
                else
                {
                    IsActiveCondition = " 0=0 ";
                }
                #endregion

                #region IsRofCondition
                string IsRofCondition = null;
                if (model.IsRof == 1)
                {
                    IsRofCondition = " TT.RofCount > 0 ";
                }
                else if (model.IsRof == -1)
                {
                    IsRofCondition = " TT.RofCount= 0  OR TT.RofCount>0 AND TT.NotRofCount>0 ";
                }
                else
                {
                    IsRofCondition = " 0=0 ";
                }
                #endregion

                #region ShowTypeCondition
                string ShowTypeCondition = null;
                if (model.ShowType == 1)
                {
                    ShowTypeCondition = " TLA.ActivityID IS NOT NULL ";
                }
                else if (model.ShowType == -1)
                {
                    ShowTypeCondition = " TLA.ActivityID IS NULL ";
                }
                else
                {
                    ShowTypeCondition = " 0=0 ";
                }
                #endregion

                #region TimeCondition               
                var StartTimeCondition = model.StartTime != null ? $" TLA.StartTime >=@StartTime  " : " 0=0 ";
                var EndTimeCondition = model.EndTime != null ? $" TLA.EndTime<@EndTime " : " 0=0 ";
                #endregion

                #region  ActivityNameCondition
                var ActivityNameCondition = string.IsNullOrWhiteSpace(model.ActivityName) ? " 0=0 " : " TLA.ActivityName LIKE '%' + @ActivityName + '%' ";
                #endregion
                #region SortCondition

                string sortCondition = null;
                if (model.Sort != 0)
                    sortCondition = "TLA.Sort=@Sort";
                else
                {
                    sortCondition = " 1=1";
                }
                #endregion
                sql = string.Format(sql, IngTypeCondition, IsActiveCondition, IsRofCondition, ShowTypeCondition, StartTimeCondition, EndTimeCondition, ActivityNameCondition, sortCondition);
                return sqlHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@Department",GetStr(model.Department)),
                    new SqlParameter("@PriceRange",GetStr(model.PriceRange)),
                    new SqlParameter("@VehicleBodyType",GetStr(model.VehicleBodyType)),
                    new SqlParameter("@VehicleId",GetStr(model.VehicleId)),
                    new SqlParameter("@TireSize",GetStr(model.TireSize)),
                    //new SqlParameter("@ShowType",model.ShowType),
                    //new SqlParameter("@IngType",model.IngType),
                    //new SqlParameter("@IsRof",model.IsRof),
                    new SqlParameter("@ActivityName",model.ActivityName),
                    //new SqlParameter("@IsActive",model.IsActive),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@PageIndex",pager.CurrentPage),
                    new SqlParameter("@PageSize",pager.PageSize),
                    new SqlParameter("@Sort",model.Sort),
                }).ConvertTo<ActivityItem>();
            }
        }

        public static int ReplaceListActivityItem(string activityName, string image, string icon, string image2, string buttonText, string activityIDs)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return sqlHelper.ExecuteNonQuery(@"UPDATE  Activity.dbo.tbl_TireListActivity
            SET     ActivityName = ISNULL(@ActivityName, ActivityName) ,
                    Image = ISNULL(@Image, Image) ,
                    Image2 = ISNULL(@Image2,Image2) ,
                    Icon = ISNULL(@Icon, Icon) ,
                    ButtonText = ISNULL(@ButtonText, ButtonText)
            WHERE   ActivityID IN (
                    SELECT  *
                    FROM    Gungnir.dbo.Split(@ActivityIDs, ';') );", CommandType.Text, new SqlParameter[] {
                                              new SqlParameter("@ActivityName",activityName),
                                              new SqlParameter("@Image",image),
                                              new SqlParameter("@Icon",icon),
                                              new SqlParameter("@Image2",image2),
                                              new SqlParameter("@ButtonText",buttonText),
                                              new SqlParameter("@ActivityIDs",activityIDs),

                });
            }
        }

        public static IEnumerable<ActivityItem> GetListActivityByID(Guid activityID)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return sqlHelper.ExecuteDataTable(@" SELECT	TLA.ActivityName,
                                                   		TLA.Status,
                                                        TLA.Sort,
                                                   		TLA.StartTime,
                                                   		TLA.EndTime,
                                                   		TLA.Image,
                                                        TLA.Image2,
                                                   		TLA.Icon,
                                                   		TLA.ButtonType,
                                                   		TLA.GetRuleGUID,
                                                   		TLA.ButtonText,
                                                   		T.PID,
                                                   		T.Postion,
                                                   		TLA.ActivityID,
                                                   		TLA.TireSize,
                                                   		T.DisplayName,
                                                        TLA.VehicleID
                                                    FROM	Activity.dbo.tbl_TireListActivity AS TLA WITH ( NOLOCK )
                                                    LEFT JOIN ( SELECT	TLAP.PID,
                                                   					TLAP.Postion,
                                                   					VP.DisplayName,
                                                   					TLAP.ActivityID
                                                   			 FROM	Activity.dbo.tbl_TireListActivityProducts AS TLAP WITH ( NOLOCK )
                                                   			 JOIN	Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
                                                   					ON TLAP.PID = VP.PID
                                                   		   ) AS T
                                                   		ON T.ActivityID = TLA.ActivityID
                                                    WHERE	TLA.ActivityID = @ActivityID;", CommandType.Text, new SqlParameter("@ActivityID", activityID)).ConvertTo<ActivityItem>();
            }
        }

        public static int BitchOn(string activityIDs)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {

                var list = SelectVehicleTireSizesByActivityIDs(sqlHelper, activityIDs);
                var flag = true;
                if (!CheckIngActivityTimeFromReceive(sqlHelper, activityIDs))
                    return -99;
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        var result = CheckIngActivityTime(sqlHelper, item);
                        if (result < 0)
                            flag = false;
                    }
                }
                if (!flag)
                    return -99;

                return sqlHelper.ExecuteNonQuery(@"UPDATE	Activity.dbo.tbl_TireListActivity
                                                   SET		Status = 1
                                                   WHERE Status<>-1 AND	ActivityID IN ( SELECT	*
                                                   						FROM	Gungnir.dbo.Split(@ActivityIDs, ',') AS S );", CommandType.Text, new SqlParameter("@ActivityIDs", activityIDs));
            }
        }
        public static bool CheckIngActivityTimeFromReceive(SqlDbHelper sqlHelper, string activityIDs)
        {
            //var list = sqlHelper.ExecuteDataTable(@"SELECT	*
            //                                      FROM	Activity.dbo.tbl_TireListActivity AS TLA WITH ( NOLOCK )
            //                                      WHERE	TLA.ActivityID IN ( SELECT	*
            //                                      							FROM	Gungnir.dbo.Split(@ActivityIDs, ',') AS S )", CommandType.Text, new SqlParameter("@ActivityIDs", activityIDs)).ConvertTo<ActivityItem>();
            //if(list.Any())
            //{
            //  var dic=  list.GroupBy(C => C.VehicleId + "|" + C.TireSize).ToDictionary(C => C.Key, C=>C.ToList());
            //    foreach (var item in dic)
            //    {
            //        if(item.Value.Count>1)
            //        {

            //        }
            //    }
            //}

            var obj = sqlHelper.ExecuteScalar(@"WITH	T AS ( SELECT	TLA.VehicleID,
                                            						TLA.StartTime,
                                            						TLA.EndTime,
                                            						TLA.ActivityID,
                                                                    TLA.Sort
                                            			   FROM		Activity.dbo.tbl_TireListActivity AS TLA
                                            			   WHERE	TLA.ActivityID IN (SELECT * FROM Gungnir.dbo.Split(@ActivityIDs,',') AS S) 
                                            			 )
                                            	 SELECT	COUNT(1)
                                            	 FROM	T AS A
                                            	 JOIN	T AS B
                                            			ON A.ActivityID <> B.ActivityID
                                            			   AND A.VehicleID = B.VehicleID
                                                           AND A.Sort=B.Sort
                                            			   AND A.EndTime > B.StartTime
                                            			   AND A.EndTime <= B.EndTime;", CommandType.Text, new SqlParameter("@ActivityIDs", activityIDs));
            return Convert.ToInt32(obj) == 0;
        }




        public static IEnumerable<ActivityItem> SelectVehicleTireSizesByActivityIDs(SqlDbHelper sqlHelper, string activityIDs)
        {
            return sqlHelper.ExecuteDataTable(@"SELECT DISTINCT
                                                		TLA.VehicleID,
                                                		TLA.TireSize,
                                                		TLA.StartTime,
                                                        TLA.EndTime,
                                                        1 AS Status
                                                FROM	Activity.dbo.tbl_TireListActivity AS TLA WITH ( NOLOCK )
                                                WHERE	TLA.ActivityID IN ( SELECT	*
                                                							FROM	Gungnir.dbo.Split(@ActivityIDs, ',') AS S )
                                                		AND TLA.Status = 0", CommandType.Text, new SqlParameter("@ActivityIDs", activityIDs)).ConvertTo<ActivityItem>();

        }

        public static int BitchOff(string activityIDs)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return sqlHelper.ExecuteNonQuery(@"UPDATE	Activity.dbo.tbl_TireListActivity
                                                   SET		Status = 0
                                                   WHERE	Status<>-1 AND ActivityID IN ( SELECT	*
                                                   						FROM	Gungnir.dbo.Split(@ActivityIDs, ',') AS S );", CommandType.Text, new SqlParameter("@ActivityIDs", activityIDs));
            }
        }

        public static int DeleteListActivity(Guid activityID)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return sqlHelper.ExecuteNonQuery(@"UPDATE	Activity.dbo.tbl_TireListActivity
                                                   SET		Status = -1
                                                   WHERE	ActivityID = @ActivityID", CommandType.Text, new SqlParameter("@ActivityID", activityID));
            }
        }

        public static IEnumerable<ActivityProducts> SelectRelationPIDs(Guid activityID)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return sqlHelper.ExecuteDataTable(@"SELECT	*
				                             FROM	Activity.dbo.tbl_TireListActivityProducts AS TLAP WITH ( NOLOCK )
				                             WHERE	TLAP.ActivityID = @ActivityID Order By Postion", CommandType.Text, new SqlParameter("@ActivityID", activityID)).ConvertTo<ActivityProducts>();
            }
        }

        public static int CheckIngActivityTime2(SqlDbHelper sqlHelper, ActivityItem model)
        {
            if (model.Status == 0)
                return 1;
            var OBJ = sqlHelper.ExecuteScalar(@"SELECT	1
                                             FROM	Activity.dbo.tbl_TireListActivity AS TLA WITH ( NOLOCK )
                                             WHERE	TLA.VehicleID = @VehicleID
                                             		AND TLA.TireSize = @TireSize
                                                    AND TLA.Sort = @Sort
                                                    AND TLA.ActivityID <> @ActivityID
                                             		AND TLA.Status = 1
                                             		AND  (@StartTime<TLA.EndTime AND @StartTime>=TLA.StartTime OR @EndTime>TLA.StartTime AND @EndTime<TLA.EndTime OR @EndTime>=TLA.EndTime AND @StartTime<=TLA.StartTime)", CommandType.Text,
                                                    new SqlParameter[] {
                                                        new SqlParameter("@VehicleID",model.VehicleId),
                                                        new SqlParameter("@TireSize",model.TireSize),
                                                        new SqlParameter("@Sort",model.Sort),
                                                        new SqlParameter("@EndTime",model.EndTime),
                                                         new SqlParameter("@ActivityID",model.ActivityID),
                                                        new SqlParameter("@StartTime",model.StartTime),
                                                    });
            return OBJ == null || OBJ == DBNull.Value ? 1 : -1;
        }
        public static int EditActivity(ActivityItem model)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                int result = -99;

                if (model.Status == 1)
                {
                    result = CheckIngActivityTime2(sqlHelper, model);
                    if (result <= 0)
                        return -97;
                }
                sqlHelper.BeginTransaction();
                result = UpdateActivity(sqlHelper, model);
                if (result > 0)
                {
                    DeleteProductsByActivityID(sqlHelper, model.ActivityID.Value);
                    if (model.Products != null && model.Products.Any())
                    {
                        foreach (var item in model.Products)
                        {
                            result = SaveSingleProduct(sqlHelper, item, model.ActivityID.Value);
                            if (result <= 0)
                            {
                                sqlHelper.Rollback();
                                return result;
                            }
                        }
                    }
                }
                else
                {
                    sqlHelper.Rollback();
                    return result;
                }
                sqlHelper.Commit();
                return result;
            }
        }

        public static void DeleteProductsByActivityID(SqlDbHelper sqlHelper, Guid activityID)
        {
            sqlHelper.ExecuteNonQuery(@"DELETE	Activity.dbo.tbl_TireListActivityProducts
                                        WHERE	ActivityID = @ActivityID", CommandType.Text, new SqlParameter("@ActivityID", activityID));
        }

        public static int UpdateActivity(SqlDbHelper sqlHelper, ActivityItem model)
        {
            return sqlHelper.ExecuteNonQuery(@"UPDATE	Activity.dbo.tbl_TireListActivity
                                              SET     ActivityName = @ActivityName,
                                                      Status = @Status,
                                                      Sort=@Sort,
                                                      StartTime = @StartTime,
                                                      EndTime = @EndTime,
                                                      Image = @Image,
                                                      Image2=@Image2,
                                                      Icon = @Icon,
                                                      ButtonType = @ButtonType,
                                                      GetRuleGUID = @GetRuleGUID,
                                                      ButtonText = @ButtonText,
                                                      UpdateTime = GETDATE()
                                              WHERE   ActivityID = @ActivityID", CommandType.Text, new SqlParameter[] {
                                                 new SqlParameter("@ActivityName",model.ActivityName),
                                                 new SqlParameter("@Status",model.Status),
                                                 new SqlParameter("@Sort",model.Sort),
                                                 new SqlParameter("@StartTime",model.StartTime),
                                                 new SqlParameter("@EndTime",model.EndTime),
                                                 new SqlParameter("@Image",model.Image),
                                                 new SqlParameter("@Image2",model.Image2),
                                                 new SqlParameter("@Icon",model.Icon),
                                                 new SqlParameter("@ButtonType",model.ButtonType),
                                                 new SqlParameter("@GetRuleGUID",model.GetRuleGUID),
                                                 new SqlParameter("@ButtonText",model.ButtonText),
                                                 new SqlParameter("@ActivityID",model.ActivityID),
            });
        }
        public static int SaveBitchAdd(ActivityItem model, out List<Guid> activityIDS)
        {
            activityIDS = new List<Guid>();
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                int result = -99;
                sqlHelper.BeginTransaction();
                foreach (var item in model.VehicleIDTireSize)
                {
                    model.VehicleId = item.VehicleID;
                    model.TireSize = item.TireSize;
                    result = CheckIngActivityTime(sqlHelper, model);
                    if (result > 0)
                    {
                        result = SaveSingle(sqlHelper, model);
                        if (result > 0)
                        {
                            activityIDS.Add(model.ActivityID.Value);//用于记录日志
                            if (model.Products != null && model.Products.Any())
                            {
                                foreach (var product in model.Products)
                                {
                                    result = SaveSingleProduct(sqlHelper, product, model.ActivityID.Value);
                                    if (result <= 0)
                                    {
                                        sqlHelper.Rollback();
                                        return result;
                                    }
                                }
                            }
                        }
                        else
                        {
                            sqlHelper.Rollback();
                            return result;
                        }
                    }
                    else
                    {
                        sqlHelper.Rollback();
                        return -97;
                    }
                }
                sqlHelper.Commit();
                return result;
            }
        }
        public static int CheckIngActivityTime(SqlDbHelper sqlHelper, ActivityItem model)
        {
            if (model.Status == 0)
                return 1;
            var OBJ = sqlHelper.ExecuteScalar(@"SELECT	1
                                             FROM	Activity.dbo.tbl_TireListActivity AS TLA WITH ( NOLOCK )
                                             WHERE	TLA.VehicleID = @VehicleID
                                             		AND TLA.TireSize = @TireSize
                                                    AND TLA.Sort=@Sort
                                             		AND TLA.Status = 1
                                             		AND  (@StartTime<TLA.EndTime AND @StartTime>=TLA.StartTime OR @EndTime>TLA.StartTime AND @EndTime<TLA.EndTime OR @EndTime>=TLA.EndTime AND @StartTime<=TLA.StartTime)", CommandType.Text,
                                                    new SqlParameter[] {
                                                        new SqlParameter("@VehicleID",model.VehicleId),
                                                        new SqlParameter("@TireSize",model.TireSize),
                                                        new SqlParameter("@EndTime",model.EndTime),
                                                        new SqlParameter("@Sort",model.Sort),
                                                        new SqlParameter("@StartTime",model.StartTime),
                                                    });
            return OBJ == null || OBJ == DBNull.Value ? 1 : -1;
        }
        public static int SaveSingle(SqlDbHelper sqlHelper, ActivityItem model)
        {
            model.ActivityID = Guid.NewGuid();
            return sqlHelper.ExecuteNonQuery(@"INSERT	INTO Activity.dbo.tbl_TireListActivity
			                         			(
			                         			  ActivityID,
			                         			  VehicleID,
			                         			  TireSize,
			                         			  ActivityName,
			                         			  Status,
                                                  Sort,
			                         			  StartTime,
			                         			  EndTime,
			                         			  Image,
                                                  Image2,
			                         			  Icon,
			                         			  ButtonType,
			                         			  GetRuleGUID,
			                         			  ButtonText )
			                         	VALUES	(
			                         			  @ActivityID,
			                         			  @VehicleID,
			                         			  @TireSize,
			                         			  @ActivityName,
			                         			  @Status,
                                                  @Sort,
			                         			  @StartTime,
			                         			  @EndTime,
			                         			  @Image,
                                                  @Image2,
			                         			  @Icon,
			                         			  @ButtonType,
			                         			  @GetRuleGUID,
			                         			  @ButtonText )", CommandType.Text, new SqlParameter[] {
                                          new SqlParameter("@ActivityID",model.ActivityID),
                                          new SqlParameter("@VehicleID",model.VehicleId),
                                          new SqlParameter("@TireSize",model.TireSize),
                                          new SqlParameter("@ActivityName",model.ActivityName),
                                          new SqlParameter("@Status",model.Status),
                                          new SqlParameter("@Sort",model.Sort),
                                          new SqlParameter("@StartTime",model.StartTime),
                                          new SqlParameter("@EndTime",model.EndTime),
                                          new SqlParameter("@Image",model.Image),
                                          new SqlParameter("@Image2",model.Image2),
                                          new SqlParameter("@Icon",model.Icon),
                                          new SqlParameter("@ButtonType",model.ButtonType),
                                          new SqlParameter("@GetRuleGUID",model.GetRuleGUID),
                                          new SqlParameter("@ButtonText",model.ButtonText)
            });
        }

        public static int SaveSingleProduct(SqlDbHelper sqlHelper, ActivityProducts model, Guid activityID)
        {

            return sqlHelper.ExecuteNonQuery(@"INSERT	INTO Activity.dbo.tbl_TireListActivityProducts
				                               		( PID, ActivityID, Postion )
				                               VALUES	( @PID, @ActivityID, @Postion )", CommandType.Text, new SqlParameter[] {
                                          new SqlParameter("@ActivityID",activityID),
                                          new SqlParameter("@PID",model.PID),
                                          new SqlParameter("@Postion",model.Postion)
            });
        }

        public static int CheckGetRuleGUID(Guid? guid)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var obj = sqlHelper.ExecuteScalar(@"SELECT	1
			                                      FROM		Activity.dbo.tbl_GetCouponRules AS GCR WITH ( NOLOCK )
			                                      WHERE		GCR.GetRuleGUID = @GetRuleGUID
			                                     			AND (
			                                     				  GCR.Term > 0
			                                     				  OR GCR.ValiEndDate > GETDATE() )",
                                                                  CommandType.Text,
                                                                  new SqlParameter("@GetRuleGUID", guid));
                return obj == null || obj == DBNull.Value ? 0 : 1;
            }
        }

        public static int GetTotalCount(SqlDbHelper sqlHelper, ListActCondition model)
        {
            var sql = @"SELECT  COUNT(1)
FROM    ( SELECT    T.BrandCategory ,
                    T.Brand ,
                    T.Vehicle ,
                    T.MinPrice ,
                    T.TiresMatch ,
                    T.VehicleId ,
                    SS.Item AS TireSize ,
                    ( SELECT    COUNT(1)
                      FROM      Gungnir..tbl_Vehicle_Type AS V WITH ( NOLOCK )
                                CROSS APPLY Gungnir..SplitString(V.TiresMatch,
                                                              ';', 1) AS SS
                                JOIN Tuhu_productcatalog.dbo.vw_Products AS VP
                                WITH ( NOLOCK ) ON SS.Item = VP.PID COLLATE Chinese_PRC_CI_AS
                                                   AND VP.TireSize = TireSize COLLATE Chinese_PRC_CI_AS
                      WHERE     VP.CP_Tire_ROF = N'非防爆'
                                AND V.ProductID = T.VehicleId
                    ) NotRofCount ,
                    ( SELECT    COUNT(1)
                      FROM      Gungnir..tbl_Vehicle_Type AS V WITH ( NOLOCK )
                                CROSS APPLY Gungnir..SplitString(V.TiresMatch,
                                                              ';', 1) AS SS
                                JOIN Tuhu_productcatalog.dbo.vw_Products AS VP
                                WITH ( NOLOCK ) ON SS.Item = VP.PID COLLATE Chinese_PRC_CI_AS
                                                   AND VP.TireSize = TireSize COLLATE Chinese_PRC_CI_AS
                      WHERE     VP.CP_Tire_ROF = N'防爆'
                                AND V.ProductID = T.VehicleId
                    ) RofCount
          FROM      ( SELECT    VT.BrandCategory ,
                                VT.Brand ,
                                VT.Vehicle ,
                                VT.Tires ,
                                VT.MinPrice ,
                                VT.TiresMatch ,
                                VT.ProductID AS VehicleId
                      FROM      Gungnir.dbo.tbl_Vehicle_Type AS VT WITH ( NOLOCK )
                      WHERE     ( VT.ProductID IN (
                                  SELECT    *
                                  FROM      Gungnir..Split(@VehicleId, ';') AS S )
                                  OR @VehicleId = N'ALL'
                                )
                                AND ( VT.BrandCategory IN (
                                      SELECT    *
                                      FROM      Gungnir..Split(@Department,
                                                              ';') AS S )
                                      OR @Department = N'ALL'
                                      OR ( CHARINDEX(N'Other', @Department) > 0
                                           AND ISNULL(VT.BrandCategory, '') = ''
                                         )
                                    )
                                AND ( @PriceRange = N'ALL'
                                      OR ( CHARINDEX(N'High', @PriceRange) > 0
                                           AND VT.MinPrice >= 16
                                         )
                                      OR ( CHARINDEX(N'Middle', @PriceRange) > 0
                                           AND VT.MinPrice < 16
                                           AND VT.MinPrice >= 8
                                         )
                                      OR ( CHARINDEX(N'Low', @PriceRange) > 0
                                           AND VT.MinPrice < 8
                                           AND ISNULL(VT.MinPrice, 0) > 0
                                         )
                                    )
                                AND ( @VehicleBodyType = N'ALL'
                                      OR VT.VehicleBodyType IN (
                                      SELECT    *
                                      FROM      Gungnir..Split(@VehicleBodyType,
                                                              ';') AS S2 )
                                    )
                                AND ISNULL(VT.Tires, '') <> ''
                    ) AS T
                    CROSS APPLY Gungnir..SplitString(T.Tires, ';', 1) AS SS
          WHERE     ( @TireSize = N'ALL'
                      OR SS.Item IN (
                      SELECT    *
                      FROM      Gungnir.dbo.Split(@TireSize, ';') AS S3 )
                    )
        ) AS TT
        LEFT JOIN ( SELECT  *
                    FROM    Activity.dbo.tbl_TireListActivity AS TL WITH ( NOLOCK )
                    WHERE   TL.Status <> -1
                  ) AS TLA ON TLA.VehicleID = TT.VehicleId COLLATE Chinese_PRC_CI_AS
                              AND TLA.TireSize = TT.TireSize COLLATE Chinese_PRC_CI_AS
WHERE   ( @ShowType = 0
          OR @ShowType = 1
          AND TLA.ActivityID IS NOT NULL
          OR @ShowType = -1
          AND TLA.ActivityID IS NULL
        )
        AND ( @IngType = 0
              OR @IngType = -1
              AND TLA.StartTime > GETDATE()
              OR @IngType = -2
              AND TLA.EndTime <= GETDATE()
              OR @IngType = 1
              AND TLA.StartTime <= GETDATE()
              AND TLA.EndTime > GETDATE()
            )
        AND ( @ActivityName IS NULL
              OR TLA.ActivityName LIKE '%' + @ActivityName + '%'
            )
        AND ( @IsActive = 0
              OR @IsActive = 1
              AND TLA.Status = 1
              OR @IsActive = -1
              AND TLA.Status = 0
            )
        AND ( @StartTime IS NULL
              OR TLA.StartTime >= @StartTime
            )
        AND ( @EndTime IS NULL
              OR TLA.EndTime < @EndTime
            )
        AND ( @IsRof = 0
              OR @IsRof = 1
              AND TT.RofCount > 0
              AND TT.NotRofCount = 0
              OR @IsRof = -1
              AND (TT.RofCount= 0  OR TT.RofCount>0 AND TT.NotRofCount>0)
            ) AND ({0})";
            string sortCondition = null;
            if (model.Sort != 0)
                sortCondition = "TLA.Sort=@Sort";
            else
            {
                sortCondition = " 1 = 1";
            }
            var sqlwithparm = string.Format(sql, sortCondition);
            var OBJ = sqlHelper.ExecuteScalar(sqlwithparm, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@Department",GetStr(model.Department)),
                    new SqlParameter("@PriceRange",GetStr(model.PriceRange)),
                    new SqlParameter("@VehicleBodyType",GetStr(model.VehicleBodyType)),
                    new SqlParameter("@VehicleId",GetStr(model.VehicleId)),
                    new SqlParameter("@TireSize",GetStr(model.TireSize)),
                    new SqlParameter("@ShowType",model.ShowType),
                    new SqlParameter("@IngType",model.IngType),
                    new SqlParameter("@IsRof",model.IsRof),
                    new SqlParameter("@ActivityName",string.IsNullOrWhiteSpace(model.ActivityName)?null:model.ActivityName),
                    new SqlParameter("@IsActive",model.IsActive),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@Sort",model.Sort)
                });
            return Convert.ToInt32(OBJ);

        }

        static string GetStr(string str) => string.IsNullOrWhiteSpace(str) || str.Contains("ALL") ? "ALL" : str;

        public static int GetActPageOnTireChangelCount(SqlDbHelper sqlHelper, ListActCondition model)
        {
            var sql = @"    SELECT  Count(1)
                            FROM    ( SELECT    VT.Brand ,
                                                VT.Vehicle ,
                                                SS.Item AS TireSize ,
                                                VT.ProductID AS VehicleId
                                      FROM      Gungnir.dbo.tbl_Vehicle_Type AS VT WITH ( NOLOCK )
                                                CROSS APPLY Gungnir..SplitString(VT.Tires, ';', 1) AS SS
                                      WHERE     ( @TireSize = N'ALL'
                                                  OR SS.Item IN (
                                                  SELECT    *
                                                  FROM      Gungnir.dbo.Split(@TireSize,
                                                                              ';') AS S3 )
                                                )
                                                AND ( VT.ProductID IN (
                                                      SELECT    *
                                                      FROM      Gungnir..Split(@VehicleId,
                                                                               ';') AS S )
                                                      OR @VehicleId = N'ALL'
                                                    )
                                    ) AS S
                                    LEFT JOIN Configuration.dbo.TireChangedActPage AS T ON S.VehicleId = T.VehicleId COLLATE Chinese_PRC_CI_AS
                                                                                          AND S.TireSize = T.TireSize COLLATE Chinese_PRC_CI_AS
                            WHERE   ( @ShowType = 0
                                      OR @ShowType = 1
                                      AND T.HashKey IS NOT NULL
                                      OR @ShowType = -1
                                      AND T.HashKey IS NULL
                                    )
                                    AND ( @IsActive = 0
                                          OR @IsActive = 1
                                          AND T.State = 1
                                          OR @IsActive = -1
                                          AND T.State = 0
                                        )";
            var OBJ = sqlHelper.ExecuteScalar(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@VehicleId",GetStr(model.VehicleId)),
                    new SqlParameter("@TireSize",GetStr(model.TireSize)),
                    new SqlParameter("@ShowType",model.ShowType),
                    new SqlParameter("@IsActive",model.IsActive),
                });
            return Convert.ToInt32(OBJ);
        }

        public static IEnumerable<ActivityItem> SelectActPageOnTireChange(ListActCondition model, PagerModel pager)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                pager.TotalItem = GetActPageOnTireChangelCount(sqlHelper, model);
                #region sql
                var sql = @"SELECT  S.Brand ,
                                    S.Vehicle ,
                                    S.TireSize ,
                                    S.VehicleId ,
                                    T.HashKey ,
                                    T.State AS Status,
                                    T.PKId
                            FROM    ( SELECT    VT.Brand ,
                                                VT.Vehicle ,
                                                SS.Item AS TireSize ,
                                                VT.ProductID AS VehicleId
                                      FROM      Gungnir.dbo.tbl_Vehicle_Type AS VT WITH ( NOLOCK )
                                                CROSS APPLY Gungnir..SplitString(VT.Tires, ';', 1) AS SS
                                      WHERE     ( @TireSize = N'ALL'
                                                  OR SS.Item IN (
                                                  SELECT    *
                                                  FROM      Gungnir.dbo.Split(@TireSize,
                                                                              ';') AS S3 )
                                                )
                                                AND ( VT.ProductID IN (
                                                      SELECT    *
                                                      FROM      Gungnir..Split(@VehicleId,
                                                                               ';') AS S )
                                                      OR @VehicleId = N'ALL'
                                                    )
                                    ) AS S
                                    LEFT JOIN Configuration.dbo.TireChangedActPage AS T ON S.VehicleId = T.VehicleId COLLATE Chinese_PRC_CI_AS
                                                                                          AND S.TireSize = T.TireSize COLLATE Chinese_PRC_CI_AS
                            WHERE   ( {0} ) AND ( {1} ) 
                            ORDER BY S.Brand
                            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
                            FETCH NEXT @PageSize ROWS ONLY;";
                #endregion

                #region IsActiveCondition
                string IsActiveCondition = null;
                if (model.IsActive == 1)
                {
                    IsActiveCondition = "  T.State = 1 ";
                }
                else if (model.IsActive == -1)
                {
                    IsActiveCondition = "  T.State = 0 ";
                }
                else
                {
                    IsActiveCondition = " 0=0 ";
                }
                #endregion
                #region ShowTypeCondition
                string ShowTypeCondition = null;
                if (model.ShowType == 1)
                {
                    ShowTypeCondition = " T.HashKey IS NOT NULL ";
                }
                else if (model.ShowType == -1)
                {
                    ShowTypeCondition = " T.HashKey IS NULL ";
                }
                else
                {
                    ShowTypeCondition = " 0=0 ";
                }
                #endregion
                sql = string.Format(sql, IsActiveCondition, ShowTypeCondition);
                return sqlHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@VehicleId",GetStr(model.VehicleId)),
                    new SqlParameter("@TireSize",GetStr(model.TireSize)),
                    new SqlParameter("@PageIndex",pager.CurrentPage),
                    new SqlParameter("@PageSize",pager.PageSize),
                }).ConvertTo<ActivityItem>() ?? new List<ActivityItem>();
            }
        }
        public static ActivityItem FetchTireActivityById(int PkId)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return sqlHelper.ExecuteDataTable(@"SELECT  PKId ,
                                                            HashKey ,
                                                            VehicleId ,
                                                            TireSize ,
                                                            State AS Status
                                                    FROM    Configuration.[dbo].[TireChangedActPage] WITH ( NOLOCK )
                                                    WHERE   PKId = @PKId",
                    CommandType.Text, new SqlParameter("@PKId", PkId)).ConvertTo<ActivityItem>()?.FirstOrDefault() ?? new ActivityItem();
            }
        }

        public static bool UpdateTireChangedAct(ActivityItem model)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                string sql = @" UPDATE Configuration..TireChangedActPage
                                SET HashKey=@HashKey,
                                State=@State,
                                LastUpdateDateTime=GETDATE(),
                                StartDateTime=@StartDateTime,
                                EndDateTime=@EndDateTime,
                                Manager=@Manager
                                WHERE PKId=@PKId";
                var sqlParam = new[]
                {
                    new SqlParameter("@HashKey", model.HashKey),
                    new SqlParameter("@State",model.Status),
                    new SqlParameter("@PKId", model.PKId),
                    new SqlParameter("@StartDateTime", model.StartTime),
                    new SqlParameter("@EndDateTime",model.EndTime),
                    new SqlParameter("@Manager", model.Manager)
                };
                return sqlHelper.ExecuteNonQuery(sql, CommandType.Text, sqlParam) > 0 ? true : false;
            }
        }

        public static bool UpdateTireChangedActInBatch(SqlDbHelper dbHelper, ActivityItem updateModel, List<string> pkids)
        {
            var linesToBeChanged = pkids.Count();
            string sql = @" UPDATE  Configuration..TireChangedActPage
                                SET     HashKey = @HashKey ,
                                        State = @State ,
                                        LastUpdateDateTime = GETDATE() ,
                                        StartDateTime = @StartDateTime ,
                                        EndDateTime = @EndDateTime ,
                                        Manager = @Manager
                                WHERE   PKId IN ( SELECT    *
                                                  FROM      Gungnir.dbo.Split(@PKIds, ';') );";
            var sqlParam = new[]
            {
                        new SqlParameter("@HashKey", updateModel.HashKey),
                        new SqlParameter("@State",updateModel.Status),
                        new SqlParameter("@PKIds", string.Join(";", pkids)),
                        new SqlParameter("@StartDateTime", updateModel.StartTime),
                        new SqlParameter("@EndDateTime",updateModel.EndTime),
                        new SqlParameter("@Manager", updateModel.Manager)
                    };
            return dbHelper.ExecuteNonQuery(sql, CommandType.Text, sqlParam) < linesToBeChanged? false : true;
        }

        public static bool AddTireChangedAct(ActivityItem model)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                string sql = @"INSERT INTO Configuration..TireChangedActPage
									        ( VehicleId ,
									          TireSize ,
									          HashKey ,
									          StartDateTime ,
									          EndDateTime ,
									          Manager ,
									          State ,
									          CreateDateTIme ,
									          LastUpdateDateTime
									        )
									VALUES  ( @VehicleId , 
									          @TireSize , 
									          @HashKey ,
									          @StartDateTime ,
									          @EndDateTime , 
									          @Manager ,
									          @State,
									          GETDATE() , 
									          GETDATE() 
									        )";
                var sqlParam = new[]
                {
                    new SqlParameter("@VehicleId", model.VehicleId),
                    new SqlParameter("@TireSize",model.TireSize),
                    new SqlParameter("@HashKey", model.HashKey),
                    new SqlParameter("@StartDateTime", model.StartTime),
                    new SqlParameter("@EndDateTime",model.EndTime),
                    new SqlParameter("@Manager", model.Manager),
                    new SqlParameter("@State",model.Status)
                };
                return sqlHelper.ExecuteNonQuery(sql, CommandType.Text, sqlParam) > 0 ? true : false;
            }
        }

        public static bool DeleteTireChangedAct(int PKId)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return sqlHelper.ExecuteNonQuery(@"DELETE FROM Configuration..TireChangedActPage WHERE PKId = @PKId", CommandType.Text, new SqlParameter("@PKId", PKId)) > 0 ? true : false;
            }
        }

        public static IEnumerable<TireChangedActivityLog> SelectTireChangedActivityLog(string vehicleId, string tireSize)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                var sqlParam = new[]
                {
                    new SqlParameter("@VehicleId", vehicleId),
                    new SqlParameter("@TireSize", tireSize)
                };
                return sqlHelper.ExecuteDataTable(@"SELECT  VehicleId ,
                                                            TireSize ,
                                                            HashKey ,
                                                            OldHashKey ,
                                                            State ,
                                                            OldState ,
                                                            Message ,
                                                            Author ,
                                                            CreateDatetime
                                                    FROM    Tuhu_log.dbo.TireChangedActivityLog WITH ( NOLOCK )
                                                    WHERE   VehicleId = @VehicleId
                                                            AND TireSize = @TireSize",
                    CommandType.Text, sqlParam).ConvertTo<TireChangedActivityLog>();
            }
        }
    }
}
