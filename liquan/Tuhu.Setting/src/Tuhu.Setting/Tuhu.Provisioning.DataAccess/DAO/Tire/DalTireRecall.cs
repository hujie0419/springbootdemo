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
    public class DalTireRecall
    {
        public static IEnumerable<TireRecallModel> SelectList(TireRecallModel model, PagerModel pager)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                pager.TotalItem = GetListCount(dbHelper,model,pager);
                return dbHelper.ExecuteDataTable(@"SELECT [PKID]
                                                  ,[OrderNo]
                                                  ,[Mobile]
                                                  ,[CarNo]
                                                  ,[VehicleLicenseImg]
                                                  ,[TireDetailImg]
                                                  ,[TireAndLicenseImg]
                                                  ,[OrderImg]
                                                  ,[Status]
                                                  ,[LastAuthor]
                                                  ,[UpdateTime]
                                                  ,[CreateTime]
                                                  ,[Reason]
                                                  ,ISNULL([Num],0) AS Num
                                              FROM [dbo].[tbl_ProductRecall](nolock)
                                              where (OrderNo=@OrderNo or @OrderNo is null)
                                              and (Mobile=@Mobile or @Mobile is null)
                                              and (Status=@Status or @Status=-1)
                                              order by PKID desc
                                                       		OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
                                                                                                          FETCH NEXT @PageSize ROWS ONLY;", CommandType.Text,
                                                                                           new SqlParameter[] {
                                                                                               new SqlParameter("@OrderNo", String.IsNullOrWhiteSpace(model.OrderNo)?null:model.OrderNo),
                                                                                               new SqlParameter("@Mobile", String.IsNullOrWhiteSpace(model.Mobile)?null:model.Mobile),
                                                                                                new SqlParameter("@Status", model.Status),
                                                                                               new SqlParameter("@PageIndex", pager.CurrentPage),
                                                                                               new SqlParameter("@PageSize", pager.PageSize),
                                                                                           }).ConvertTo<TireRecallModel>();
            }
        }


        public static IEnumerable<TireRecallModel> SelectList(TireRecallModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
              //  pager.TotalItem = GetListCount(dbHelper, model, pager);
                return dbHelper.ExecuteDataTable(@"SELECT [PKID]
                                                  ,[OrderNo]
                                                  ,[Mobile]
                                                  ,[CarNo]
                                                  ,[VehicleLicenseImg]
                                                  ,[TireDetailImg]
                                                  ,[TireAndLicenseImg]
                                                  ,[OrderImg]
                                                  ,[Status]
                                                  ,[LastAuthor]
                                                  ,[UpdateTime]
                                                  ,[CreateTime]
                                                  ,[Reason]
                                                  ,ISNULL([Num],0) AS Num
                                              FROM [dbo].[tbl_ProductRecall](nolock)
                                              where (OrderNo=@OrderNo or @OrderNo is null)
                                              and (Mobile=@Mobile or @Mobile is null)
                                              and (Status=@Status or @Status=-1)
                                              order by PKID desc
                                                       	;", CommandType.Text,
                                                                                           new SqlParameter[] {
                                                                                               new SqlParameter("@OrderNo", String.IsNullOrWhiteSpace(model.OrderNo)?null:model.OrderNo),
                                                                                               new SqlParameter("@Mobile", String.IsNullOrWhiteSpace(model.Mobile)?null:model.Mobile),
                                                                                                new SqlParameter("@Status", model.Status)                                                                                              
                                                                                           }).ConvertTo<TireRecallModel>();
            }
        }

        public static int GetListCount(SqlDbHelper dbHelper, TireRecallModel model, PagerModel pager)
        {

            var OBJ = dbHelper.ExecuteScalar(@"select count(OrderNo) from [dbo].[tbl_ProductRecall](nolock)  where (OrderNo=@OrderNo or @OrderNo is null)
                                              and (Mobile=@Mobile or @Mobile is null)
                                              and (Status=@Status or @Status=-1)", CommandType.Text,
                new SqlParameter[] {
                    new SqlParameter("@OrderNo", String.IsNullOrWhiteSpace(model.OrderNo)?null:model.OrderNo),
                    new SqlParameter("@Mobile", String.IsNullOrWhiteSpace(model.Mobile)?null:model.Mobile),
                    new SqlParameter("@Status", model.Status)
                });

            return Convert.ToInt32(OBJ);
        }

        public static TireRecallModel FetchTireRecall(long pkid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT [PKID]
                                                  ,[OrderNo]
                                                  ,[Mobile]
                                                  ,[CarNo]
                                                  ,[VehicleLicenseImg]
                                                  ,[TireDetailImg]
                                                  ,[TireAndLicenseImg]
                                                  ,[OrderImg]
                                                  ,[Status]
                                                  ,[LastAuthor]
                                                  ,[UpdateTime]
                                                  ,[CreateTime]
                                                  ,[Num]
                                              FROM Tuhu_log.[dbo].[tbl_ProductRecall] WITH(NOLOCK) WHERE PKID=@PKID
                                              ;", CommandType.Text,
                    new SqlParameter[]
                    {
                        new SqlParameter("@PKID", pkid)
                    }).ConvertTo<TireRecallModel>().FirstOrDefault();
            }
        }

        public static int UpdateTireReallStatus(long pkid, int status,string reason)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd =
                    new SqlCommand(
                        @"UPDATE Tuhu_log.[dbo].[tbl_ProductRecall] WITH(ROWLOCK) SET Status=@Status,Reason=@Reason WHERE PKID=@PKID ")
                )
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Reason", reason);
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }

        public static Special_Bridgestone_Pidweekyear FetchSpecial_Bridgestone_Pidweekyear(int orderid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                return dbHelper.ExecuteDataTable(@"select * from tuhu_bi.dbo.Special_Bridgestone_Pidweekyear with(nolock) WHERE orderid=@orderid
                                              ;", CommandType.Text,
                    new SqlParameter[]
                    {
                        new SqlParameter("@orderid", orderid)
                    }).ConvertTo<Special_Bridgestone_Pidweekyear>().FirstOrDefault();
            }
        }

        public static bool InsertTireRecallLog(TireRecallModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd =
                    new SqlCommand(
                        @"INSERT INTO [dbo].[TireRecallLog]([RecallID],[OperateType],[CreateDateTime],[LastUpdateDateTime],[Operator],[Reason]) 
                          VALUES(@RecallID,@OperateType,@CreateDateTime,@LastUpdateDateTime,@Operator,@Reason)")
                )
                {
                    DateTime now = DateTime.Now;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@RecallID", model.PKID);
                    cmd.Parameters.AddWithValue("@OperateType",model.OperateType);
                    cmd.Parameters.AddWithValue("@CreateDateTime", now);
                    cmd.Parameters.AddWithValue("@LastUpdateDateTime", now);
                    cmd.Parameters.AddWithValue("@Operator", model.Operator);
                    cmd.Parameters.AddWithValue("@Reason", model.Reason);

                    return dbHelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                }
            }
        }

        public static IEnumerable<TireRecallLog> FetchTireRecallLog(long pkid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT [PKID]
                                                  ,[RecallID]
                                                  ,[OperateType]
                                                  ,[CreateDateTime]
                                                  ,[LastUpdateDateTime]
                                                  ,[Operator]
                                                  ,[Reason]
                                              FROM [dbo].[TireRecallLog] WITH(NOLOCK) WHERE RecallID=@RecallID
                                              Order by PKID desc
                                              ;", CommandType.Text,
                    new SqlParameter[]
                    {
                        new SqlParameter("@RecallID", pkid)
                    }).ConvertTo<TireRecallLog>(); ;
            }
        }
    }
}
