using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalZeroActivity
    {
        /// <summary>
        /// 查询0元购活动详情
        /// </summary>
        /// <returns></returns>
        public static List<ZeroActivityDetail> SelectZeroActivityDetail()
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var cmd = new SqlCommand(@"SELECT * FROM Activity..tbl_ZeroActivity  AS ZA WITH (NOLOCK) ORDER BY Period DESC");
                cmd.CommandType = CommandType.Text;
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<ZeroActivityDetail>().ToList();
            }
        }
        /// <summary>
        ///查询0元购的数据
        /// </summary>
        public static List<ZeroActivityApply> SelectAllZeroActivityApply()
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var cmd = new SqlCommand(@"select Z.*,((SELECT R.RegionName FROM Gungnir..tbl_Region AS R WHERE R.PKID=Z.ProvinceID)) AS ProvinceName,(SELECT R.RegionName FROM Gungnir..tbl_Region AS R WHERE R.PKID=Z.CityID) AS CityName FROM Activity..tbl_ZeroActivity_Apply AS Z WITH(NOLOCK) ORDER BY Z.Period DESC,Z.ApplyDateTime DESC");
                cmd.CommandType = CommandType.Text;
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<ZeroActivityApply>().ToList();
            }
        }
        public static int UpdateZAAStatus(int OrderID, string UserID)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var cmd = new SqlCommand(@"UPDATE Activity..tbl_ZeroActivity_Apply SET ReportStatus=3 WHERE OrderId=@OrderID AND UserID=@UserID");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderID", OrderID);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 0元购活动申请 成功获得轮胎的处理
        /// </summary>
        /// <param name="Period"></param>
        /// <param name="UserID"></param>
        /// <param name="UserMobileNumber"></param>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public static int ZeroAward(int Period, string UserID, String UserMobileNumber, int OrderID)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
               //var result= CheckZeroAward(dbhelper,Period, OrderID);
               // if (result == -99)
               //     return result;
                var cmd = new SqlCommand(@"UPDATE Activity..tbl_ZeroActivity_Apply SET Succeed=1,Status=1,OrderId=@OrderID,LastUpdateDateTime=GETDATE() WHERE Period=@Period AND UserMobileNumber=@UserMobileNumber AND UserID=@UserID and @OrderID NOT IN(SELECT OrderID FROM Activity..tbl_ZeroActivity_Apply where OrderId>0)");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderID", DbHelper.GetDbValue(OrderID));
                cmd.Parameters.AddWithValue("@Period", DbHelper.GetDbValue(Period));
                cmd.Parameters.AddWithValue("@UserMobileNumber", DbHelper.GetDbValue(UserMobileNumber));
                cmd.Parameters.AddWithValue("@UserID", DbHelper.GetDbValue(UserID));
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static int CheckZeroAward(SqlDbHelper dbHelper,int Period,int OrderID)
        {
            var OBJ = dbHelper.ExecuteScalar(@"	SELECT	1
	FROM	Activity.dbo.tbl_ZeroActivity AS ZA WITH (NOLOCK)
	WHERE	ZA.Period = @Period
			AND ZA.ProductID + '|' + ZA.VariantID COLLATE Chinese_PRC_CI_AS IN ( SELECT	OL.PID
																				 FROM	Gungnir.dbo.tbl_OrderList AS OL  WITH (NOLOCK)
																				 WHERE	OL.PID NOT LIKE 'FU-%'
																						AND OL.OrderID = @OrderID )", CommandType.Text, new SqlParameter[] {
                                                 new SqlParameter("@Period",Period),
                                                 new SqlParameter("@OrderID",OrderID)
            });
            return OBJ == null || OBJ == DBNull.Value ? -99 : Convert.ToInt32(OBJ);
        }
        /// <summary>
        /// 0元购活动申请详情  以及筛选
        /// </summary>
        /// <param name="Period"></param>
        /// <param name="OrderQuantity"></param>
        /// <param name="Succeed"></param>
        /// <param name="ReportStatus"></param>
        /// <returns></returns>
        public static List<ZeroActivityApply> ZeroConditionFilter(ZeroActivityApply filtermodel, int CurrentPage, int PageSize, out int TotalCount)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var cmd = new SqlCommand("[Activity].dbo.[Activity_ZeroActivityApply_Filter]");

                cmd.CommandType = CommandType.StoredProcedure;
                if (filtermodel.Period == 0)
                    cmd.Parameters.AddWithValue("@Period", null);
                else
                    cmd.Parameters.AddWithValue("@Period", filtermodel.Period);
                cmd.Parameters.AddWithValue("@Succeed", filtermodel.Succeed);
                cmd.Parameters.AddWithValue("@OrderQuantity", filtermodel.UserOrderQuantity);
                cmd.Parameters.AddWithValue("@ReportStatus", filtermodel.ReportStatus);
                cmd.Parameters.AddWithValue("@UserMobileNumber", filtermodel.UserMobileNumber);
                cmd.Parameters.AddWithValue("@CurrentPage", CurrentPage);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@TotalCount",
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output
                });
                DataTable dt = dbhelper.ExecuteDataTable(cmd);
                TotalCount = Convert.ToInt32(cmd.Parameters["@TotalCount"].Value);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<ZeroActivityApply>().ToList();
            }

        }
        public static int ZAConfigureAct(ZeroActivityDetail Zadetail)
        {
            const string sql = @" IF EXISTS ( SELECT  1
                FROM    Activity..tbl_ZeroActivity WITH (NOLOCK)
                WHERE   Period = @Period )
        BEGIN
            UPDATE  Activity..tbl_ZeroActivity
            SET     ProductID = @ProductID ,
                    VariantID = @VariantID ,
                    SucceedQuota = @SucceedQuota ,
                    Quantity = @Quantity ,
                    StartDateTime = @StartDateTime ,
                    EndDateTime = @EndDateTime ,
                    Description = @Description,
					ImgUrl = @ImgUrl ,
					Pid = @Pid
            WHERE   Period = @Period;
        END;
    ELSE
        BEGIN
            IF NOT EXISTS ( SELECT  *
                            FROM    Activity..tbl_ZeroActivity WITH (NOLOCK)
                            WHERE   Period = @Period
                                    AND ProductID = @ProductID
                                    AND VariantID =  @VariantID )
                BEGIN
                    INSERT  INTO Activity..tbl_ZeroActivity
                            ( Period ,
                              ProductID ,
                              VariantID ,
                              SucceedQuota ,
                              Quantity ,
                              StartDateTime ,
                              EndDateTime ,
                              CreateDateTime ,
                              [Description],
							  ImgUrl ,
							  Pid 
                            )
                    VALUES  ( @Period ,
                              @ProductID ,
                              @VariantID ,
                              @SucceedQuota ,
                              @Quantity ,
                              @StartDateTime ,
                              @EndDateTime ,
                              GETDATE() ,
                              @Description ,
							  @ImgUrl ,
							  @Pid
                            );
                END;
        END;";
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var cmd = new SqlCommand(sql);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Period", Zadetail.Period);
                cmd.Parameters.AddWithValue("@ProductID", Zadetail.ProductID);
                cmd.Parameters.AddWithValue("@VariantID", Zadetail.VariantID ?? string.Empty);
                cmd.Parameters.AddWithValue("@SucceedQuota", Zadetail.SucceedQuota);
                cmd.Parameters.AddWithValue("@Quantity", Zadetail.Quantity);
                cmd.Parameters.AddWithValue("@StartDateTime", Zadetail.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@EndDateTime", Zadetail.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@Description", Zadetail.Description);
                cmd.Parameters.AddWithValue("@ImgUrl", Zadetail.ImgUrl);
                cmd.Parameters.AddWithValue("@Pid", $"{Zadetail.ProductID}|{Zadetail.VariantID ?? string.Empty}");
                var n = dbhelper.ExecuteNonQuery(cmd);
                return n;
            }
        }
        public static int ZAConfigureDelete(int period)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var cmd = new SqlCommand(@"Delete from Activity..tbl_ZeroActivity where Period=@period");

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@period", period);
                return dbhelper.ExecuteNonQuery(cmd);
            }
        }

        public static List<ZeroActivityApply> SelectZeroActivityApplyByPeriod(int period)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var cmd = new SqlCommand(@"SELECT * FROM Activity..tbl_ZeroActivity_Apply WITH (NOLOCK)　WHERE Period=@Period");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Period", period);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<ZeroActivityApply>().ToList();
            }
        }

        public static int UpdateStatusByPeriod(int period)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var cmd = new SqlCommand(@"UPDATE Activity..tbl_ZeroActivity_Apply SET Status=-1 ,LastUpdateDateTime=GETDATE() WHERE Period=@Period AND(Succeed=0 OR Succeed IS NULL) AND Status=0");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Period", period);
                return dbhelper.ExecuteNonQuery(cmd); 
            }
        }

        public static ZeroActivityApply SelectZeroActivityDetail(int pkid)
        {
            using (var dbhelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var cmd = new SqlCommand(@"SELECT  * FROM    Activity..tbl_ZeroActivity_Apply WITH (NOLOCK) WHERE PKID=@PKID");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<ZeroActivityApply>().SingleOrDefault();
            }
        }
    }
}
