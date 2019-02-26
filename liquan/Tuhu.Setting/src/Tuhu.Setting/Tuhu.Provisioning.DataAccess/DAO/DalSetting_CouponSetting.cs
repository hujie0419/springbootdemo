using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalSetting_CouponSetting
    {
        /// <summary>
        /// 查询单张表的所有信息
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<CouponActivityModel> SelectAllCouponActivity(SqlConnection connection)
        {
            return SqlHelper.ExecuteDataTable(connection,
                CommandType.StoredProcedure,
                "Activity..CouponActivity_SelectAllActivity")
            .ConvertTo<CouponActivityModel>().ToList();
        }

        public static List<CouponActivityModel> SelectAllCouponActivityV1(SqlConnection connection)
        {
            const string sql = @" SELECT *  FROM Activity..tbl_CouponActivity AS A WITH(NOLOCK) WHERE A.CreateDateTime>'2016-07-07'
                                ORDER BY A.ActivityID DESC";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<CouponActivityModel>().ToList();
        }

        /// <summary>
        /// 查询多张表(条件ActivityID)
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ActivityID"></param>
        /// <returns></returns>
        public static CouponActivityModel FetchCouponActivity(SqlConnection connection, Guid ActivityID)
        {
            var ds = SqlHelper.ExecuteDataset(connection,
                CommandType.StoredProcedure,
                "Activity..CouponActivity_FetchCouponActivity",
                new SqlParameter("@ActivityID", ActivityID));

            if (ds.Tables[0].Rows.Count > 0)
            {
                var model = DbHelper.Parse<CouponActivityModel>(ds.Tables[0].Rows[0]);

                model.Coupons = ds.Tables[1].ConvertTo<CouponActivity_CouponModel>();

                model.ErrorPage = DbHelper.Parse<CouponActivity_PageModel>(ds.Tables[2].Rows[0]);
                model.SuccessPage = DbHelper.Parse<CouponActivity_PageModel>(ds.Tables[2].Rows[1]);

                return model;
            }
            //.ConvertTo<CouponActivityModel>()
            //.ToList();
            return null;
        }


        /// <summary>
        /// 添加/修改（多张表）
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static bool EditSetting(SqlConnection connection, Guid? activityID, CouponActivityModel CASModel)
        {
            var tran = connection.BeginTransaction();
            try
            {
                var paras = new[]
                {
                    new SqlParameter("@ActivityID", activityID ?? Guid.NewGuid()),
                    new SqlParameter("@ActivityName", CASModel.ActivityName),
                    new SqlParameter("@ActivityChannel", CASModel.ActivityChannel),
                    new SqlParameter("@StartTime", CASModel.StartTime),
                    new SqlParameter("@EndTime", CASModel.EndTime),
                    new SqlParameter("@BannerImg", CASModel.BannerImg),
                    new SqlParameter("@ActivityRuleImg", CASModel.ActivityRuleImg),
                    new SqlParameter("@CouponNum", CASModel.CouponNum),
                    new SqlParameter("@IsNewUser", CASModel.IsNewUser),
                    new SqlParameter("@ActivityType",CASModel.ActivityType)
                };

                if (activityID == null)
                {
                    paras[0].Direction = ParameterDirection.Output;

                    SqlHelper.ExecuteNonQuery(tran,
                        CommandType.StoredProcedure,
                        "Activity..CouponActivity_InsertCouponActivity",
                        paras);

                    CASModel.ActivityID = (Guid)paras[0].Value;
                }
                else
                {
                    SqlHelper.ExecuteNonQuery(tran,
                          CommandType.StoredProcedure,
                          "Activity..CouponActivity_UpdateCouponActivity",
                          paras);

                    CASModel.ActivityID = activityID.Value;
                }

                if (activityID != null)
                {
                    SqlHelper.ExecuteNonQuery(tran,
                        CommandType.Text,
                        @"UPDATE	Activity..tbl_CouponActivity_Coupon SET Deleted = 1 WHERE	ActivityID = @ActivityID;",
                        new SqlParameter("@ActivityID", CASModel.ActivityID));
                }

                foreach (var item in CASModel.Coupons)
                {
                    if (item.PKID == 0)
                    {
                        //添加
                        SqlHelper.ExecuteNonQuery(tran,
                            CommandType.StoredProcedure,
                            "Activity..CouponActivity_InsertCouponActivity_Coupon",
                            new SqlParameter("@ActivityID", CASModel.ActivityID),
                            new SqlParameter("@CouponType", item.CouponType),
                            new SqlParameter("@Money", item.Money),
                            new SqlParameter("@MinMoney", item.MinMoney),
                            new SqlParameter("@DateWay", item.DateWay),
                            new SqlParameter("@StartDate", item.StartDate),
                            new SqlParameter("@EndDate", item.EndDate),
                            new SqlParameter("@ValidDays", item.ValidDays),
                            new SqlParameter("@Instructions", item.Instructions),
                            new SqlParameter("@CouponNum", item.CouponNum == 0 ? (object)null : item.CouponNum));
                    }
                    else
                    {
                        //更新现有的优惠券
                        SqlHelper.ExecuteNonQuery(tran,
                           CommandType.StoredProcedure,
                           "Activity..CouponActivity_UpdateCouponActivity_Coupon",
                           new SqlParameter("@ActivityID", CASModel.ActivityID),
                           new SqlParameter("@CouponType", item.CouponType),
                           new SqlParameter("@Money", item.Money),
                           new SqlParameter("@MinMoney", item.MinMoney),
                           new SqlParameter("@DateWay", item.DateWay),
                           new SqlParameter("@StartDate", item.StartDate),
                           new SqlParameter("@EndDate", item.EndDate),
                           new SqlParameter("@ValidDays", item.ValidDays),
                           new SqlParameter("@Instructions", item.Instructions),
                           new SqlParameter("@CouponNum", item.CouponNum == 0 ? (object)null : item.CouponNum),
                           new SqlParameter("@PKID", item.PKID));

                    }
                }


                SqlHelper.ExecuteNonQuery(tran,
                    CommandType.StoredProcedure,
                   activityID == null ? "Activity..CouponActivity_InsertCouponActivity_Page" : "Activity..CouponActivity_UpdateCouponActivity_Page",
                    new SqlParameter("@ActivityID", CASModel.ActivityID),
                    new SqlParameter("@PageNumber", CASModel.SuccessPage.PageNumber),
                    new SqlParameter("@TopImg", CASModel.SuccessPage.TopImg),
                    new SqlParameter("@CenterImg", CASModel.SuccessPage.CenterImg),
                    new SqlParameter("@BottomImg", CASModel.SuccessPage.BottomImg),
                    new SqlParameter("@IOSLink", CASModel.SuccessPage.IOSLink),
                    new SqlParameter("@AndroidLink", CASModel.SuccessPage.AndroidLink),
                    new SqlParameter("@JumpMillisecond", CASModel.SuccessPage.JumpMillisecond));


                SqlHelper.ExecuteNonQuery(tran,
                    CommandType.StoredProcedure,
                   activityID == null ? "Activity..CouponActivity_InsertCouponActivity_Page" : "Activity..CouponActivity_UpdateCouponActivity_Page",
                    new SqlParameter("@ActivityID", CASModel.ActivityID),
                    new SqlParameter("@PageNumber", CASModel.ErrorPage.PageNumber),
                    new SqlParameter("@TopImg", CASModel.ErrorPage.TopImg),
                    new SqlParameter("@CenterImg", CASModel.ErrorPage.CenterImg),
                    new SqlParameter("@BottomImg", CASModel.ErrorPage.BottomImg),
                    new SqlParameter("@IOSLink", CASModel.ErrorPage.IOSLink),
                    new SqlParameter("@AndroidLink", CASModel.ErrorPage.AndroidLink),
                    new SqlParameter("@JumpMillisecond", CASModel.ErrorPage.JumpMillisecond));

                tran.Commit();

                return true;
            }
            //catch
            //{
            //    return false;
            //}
            finally
            {
                tran.Dispose();
            }
        }

        /// <summary>
        /// 添加/修改（多张表）
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static bool EditSettingV1(SqlConnection connection, Guid? activityID, CouponActivityModel CASModel)
        {
            var tran = connection.BeginTransaction();
            try
            {
                var paras = new[]
                {
                    new SqlParameter("@ActivityID", activityID ?? Guid.NewGuid()),
                    new SqlParameter("@ActivityName", CASModel.ActivityName),
                    new SqlParameter("@ActivityChannel", CASModel.ActivityChannel),
                    new SqlParameter("@StartTime", CASModel.StartTime),
                    new SqlParameter("@EndTime", CASModel.EndTime),
                    new SqlParameter("@BannerImg", CASModel.BannerImg),
                    new SqlParameter("@ActivityRuleImg", CASModel.ActivityRuleImg),
                    new SqlParameter("@CouponNum", CASModel.CouponNum),
                    new SqlParameter("@IsNewUser", CASModel.IsNewUser),
                    new SqlParameter("@ActivityType",CASModel.ActivityType)
                };

                if (activityID == null)
                {
                    paras[0].Direction = ParameterDirection.Output;

                    SqlHelper.ExecuteNonQuery(tran,
                        CommandType.StoredProcedure,
                        "Activity..CouponActivity_InsertCouponActivity",
                        paras);

                    CASModel.ActivityID = (Guid)paras[0].Value;
                }
                else
                {
                    const string sql = @"UPDATE	Activity..tbl_CouponActivity
                                        SET		ActivityName = @ActivityName,
		                                        ActivityChannel = @ActivityChannel,
		                                        StartTime = @StartTime,
		                                        EndTime = @EndTime,
		                                        BannerImg = @BannerImg,
		                                        ActivityRuleImg = @ActivityRuleImg,
		                                        CouponNum = @CouponNum,
		                                        IsNewUser = @IsNewUser,
		                                        LastUpdateDateTime = GETDATE(),
		                                        ActivityType=@ActivityType
                                        WHERE	ActivityID = @ActivityID;
                                        ";
                    SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, paras);

                    CASModel.ActivityID = activityID.Value;
                }

                if (activityID != null)
                {
                    SqlHelper.ExecuteNonQuery(tran,
                        CommandType.Text,
                        @"UPDATE	Activity..tbl_CouponActivity_Coupon SET Deleted = 1 WHERE	ActivityID = @ActivityID;",
                        new SqlParameter("@ActivityID", CASModel.ActivityID));
                }

                foreach (var item in CASModel.Coupons)
                {
                    if (item.PKID == 0)
                    {
                        const string sql = @"INSERT	INTO Activity..tbl_CouponActivity_Coupon
		                            ( ActivityID,
		                              CouponType,
		                              Money,
		                              MinMoney,
		                              DateWay,
		                              StartDate,
		                              EndDate,
		                              ValidDays,
		                              Instructions ,
		                              CouponNum,
                                      CouponId
                                      )
                            VALUES	( @ActivityID, -- ActivityID - uniqueidentifier
		                              @CouponType, -- CouponType - int
		                              @Money, -- Money - int
		                              @MinMoney, -- MinMoney - int
		                              @DateWay,
		                              @StartDate, -- StartTime - datetime
		                              @EndDate, -- EndTime - datetime
		                              @ValidDays, -- ValidTime - int
		                              @Instructions, -- Instructions - nvarchar(200)
		                              @CouponNum,
                                      @CouponId
		                              );";
                        //添加
                        SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql,
                            new SqlParameter("@ActivityID", CASModel.ActivityID),
                            new SqlParameter("@CouponType", item.CouponType),
                            new SqlParameter("@Money", item.Money),
                            new SqlParameter("@MinMoney", item.MinMoney),
                            new SqlParameter("@DateWay", item.StartDate.HasValue ? 0 : 1),
                            new SqlParameter("@StartDate", item.StartDate),
                            new SqlParameter("@EndDate", item.EndDate),
                            new SqlParameter("@ValidDays", item.ValidDays),
                            new SqlParameter("@Instructions", item.Instructions),
                            new SqlParameter("@CouponNum", item.CouponNum == 0 ? (object)null : item.CouponNum),
                            new SqlParameter("@CouponId", item.CouponId));

                    }
                    else
                    {
                        const string sql = @"UPDATE  Activity..tbl_CouponActivity_Coupon
                                            SET     Deleted = 0 ,
                                                    ActivityID = @ActivityID ,
                                                    CouponType = @CouponType ,
                                                    [Money] = @Money ,
                                                    MinMoney = @MinMoney ,
                                                    DateWay = @DateWay ,
                                                    StartDate = @StartDate ,
                                                    EndDate = @EndDate ,
                                                    ValidDays = @ValidDays ,
                                                    Instructions = @Instructions ,
                                                    CouponNum = @CouponNum ,
                                                    CouponId = @CouponId
                                            WHERE   PKID = @PKID";
                        //更新现有的优惠券
                        SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql,
                           new SqlParameter("@ActivityID", CASModel.ActivityID),
                           new SqlParameter("@CouponType", item.CouponType),
                           new SqlParameter("@Money", item.Money),
                           new SqlParameter("@MinMoney", item.MinMoney),
                           new SqlParameter("@DateWay", item.StartDate.HasValue ? 0 : 1),
                           new SqlParameter("@StartDate", item.StartDate),
                           new SqlParameter("@EndDate", item.EndDate),
                           new SqlParameter("@ValidDays", item.ValidDays),
                           new SqlParameter("@Instructions", item.Instructions),
                           new SqlParameter("@CouponNum", item.CouponNum == 0 ? (object)null : item.CouponNum),
                           new SqlParameter("@PKID", item.PKID),
                           new SqlParameter("@CouponId", item.CouponId));

                    }
                }

                if (true)
                {
                    const string insertPage = @"INSERT	INTO Activity..tbl_CouponActivity_Page
		                                        ( ActivityID,
		                                          PageNumber,
		                                          TopImg,
		                                          CenterImg,
		                                          BottomImg,
		                                          IOSLink,
		                                          AndroidLink,
		                                          JumpMillisecond )
                                        VALUES	( @ActivityID, -- ActivityID - uniqueidentifier
		                                          @PageNumber, -- PageNumber - tinyint
		                                          @TopImg, -- TopImg - nvarchar(200)
		                                          @CenterImg, -- CenterImg - nvarchar(200)
		                                          @BottomImg, -- BottomImg - nvarchar(200)
		                                          @IOSLink, -- IOSLink - nvarchar(200)
		                                          @AndroidLink, -- AndroidLink - nvarchar(200)
		                                          @JumpMillisecond -- JumpMillisecond - int
		                                          );";

                    const string updatePage = @"UPDATE	Activity..tbl_CouponActivity_Page
                                                SET		TopImg = @TopImg,
		                                                CenterImg = @CenterImg,
		                                                BottomImg = @BottomImg,
		                                                IOSLink = @IOSLink,
		                                                AndroidLink = @AndroidLink,
		                                                JumpMillisecond = @JumpMillisecond,
		                                                LastUpdateDateTime = GETDATE()
                                                WHERE	ActivityID = @ActivityID
		                                                AND PageNumber = @PageNumber;";


                    var successParameters = new SqlParameter[]
                        {
                             new SqlParameter("@ActivityID", CASModel.ActivityID),
                        new SqlParameter("@PageNumber", CASModel.SuccessPage.PageNumber),
                        new SqlParameter("@TopImg", CASModel.SuccessPage.TopImg),
                        new SqlParameter("@CenterImg", CASModel.SuccessPage.CenterImg),
                        new SqlParameter("@BottomImg", CASModel.SuccessPage.BottomImg),
                        new SqlParameter("@IOSLink", CASModel.SuccessPage.IOSLink),
                        new SqlParameter("@AndroidLink", CASModel.SuccessPage.AndroidLink),
                        new SqlParameter("@JumpMillisecond", CASModel.SuccessPage.JumpMillisecond)
                        };
                    SqlHelper.ExecuteNonQuery(tran, CommandType.Text, activityID == null ? insertPage : updatePage, successParameters);

                    var errorParameters = new SqlParameter[]
                        {
                        new SqlParameter("@ActivityID", CASModel.ActivityID),
                        new SqlParameter("@PageNumber", CASModel.ErrorPage.PageNumber),
                        new SqlParameter("@TopImg", CASModel.ErrorPage.TopImg),
                        new SqlParameter("@CenterImg", CASModel.ErrorPage.CenterImg),
                        new SqlParameter("@BottomImg", CASModel.ErrorPage.BottomImg),
                        new SqlParameter("@IOSLink", CASModel.ErrorPage.IOSLink),
                        new SqlParameter("@AndroidLink", CASModel.ErrorPage.AndroidLink),
                        new SqlParameter("@JumpMillisecond", CASModel.ErrorPage.JumpMillisecond)

                        };

                    SqlHelper.ExecuteNonQuery(tran, CommandType.Text, activityID == null ? insertPage : updatePage, errorParameters);
                }
                tran.Commit();

                return true;
            }
            //catch
            //{
            //    return false;
            //}
            finally
            {
                tran.Dispose();
            }
        }

        /// <summary>
        /// 删除多张表的数据
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static bool Delete(SqlConnection connection, Guid activityID)
        {
            try
            {
                return SqlHelper.ExecuteNonQuery(connection,
                      CommandType.StoredProcedure,
                      "Activity..CouponActivity_DeleteCouponActivity",
                      new SqlParameter("@ActivityID", activityID)) > 0;
            }
            catch
            {
                return false;
            }
        }

        public static IEnumerable<Tuple<int, int?, string>> SelectAllPromotionRules(SqlConnection connection)
        {
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, @"SELECT	CR.PKID, CR.Type, CR.Name FROM	Activity..tbl_CouponRules AS CR WITH ( NOLOCK ) WHERE	CR.ParentID = 0 ORDER BY CR.PKID;")
                      .Rows.OfType<DataRow>()
                     .Select(row => Tuple.Create(Convert.ToInt32(row["PKID"]), row.IsNull("Type") ? (int?)null : Convert.ToInt32(row["Type"]), row["Name"].ToString()))
                     .ToArray();
        }
    }
}
