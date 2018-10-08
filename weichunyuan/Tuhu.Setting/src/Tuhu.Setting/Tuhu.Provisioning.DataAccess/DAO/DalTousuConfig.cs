using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalTousuConfig
    {
        public static IEnumerable<OrderTypeTousuConfig> GetOrderTypeTousuConfig(string orderType, string dicValue)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Shouhou")))
            {
                //  pager.TotalItem = GetListCount(dbHelper, model, pager);
                return dbHelper.ExecuteDataTable(@"SELECT [PKID]
                                                  ,[OrderType]
                                                  ,[TopLevelTousu]
                                                  ,[SecondLevelTousu]
                                                  ,[ThirdLevelTousu]
                                                  ,[FourthLevelTousu]
                                                  ,[LastLevelTousu]
                                                  ,[IsNeedPhoto]
                                                  ,[IsChecked]
                                                  ,[CautionText] 
                                                  ,[GroupName]
                                              FROM [dbo].[OrderTypeTousuConfig] with(nolock)
                                              where (OrderType=@OrderType)
                                              and (FourthLevelTousu=@dicValue or @dicValue='')                                          
                                              order by PKID desc
                                                       	;", CommandType.Text,
                                                                                           new SqlParameter[] {
                                                                                               new SqlParameter("@OrderType",orderType),
                                                                                               new SqlParameter("@dicValue", dicValue)                                                                                               
                                                                                           }).ConvertTo<OrderTypeTousuConfig>();
            }
        }


        public static bool UpdateOrInsertOrderTypeTousuConfig(OrderTypeTousuConfig model)
        {
            bool result = false;
            IEnumerable<OrderTypeTousuConfig> ordertypeTousuConfigList;
            if (model != null)
            {
                using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Shouhou")))
                {
                     ordertypeTousuConfigList = dbHelper.ExecuteDataTable(@"SELECT [PKID]
                                                  ,[OrderType]
                                                  ,[TopLevelTousu]
                                                  ,[SecondLevelTousu]
                                                  ,[ThirdLevelTousu]
                                                  ,[FourthLevelTousu]
                                                  ,[LastLevelTousu]
                                                  ,[IsNeedPhoto]
                                                  ,[CautionText] 
                                                  ,[GroupName]
                                              FROM [dbo].[OrderTypeTousuConfig] with(nolock)
                                              where (OrderType=@OrderType)
                                              and (LastLevelTousu=@LastLevelTousu)                                            
                                              order by PKID desc
                                                       	;", CommandType.Text,
                                                                                              new SqlParameter[] {
                                                                                               new SqlParameter("@OrderType",model.OrderType),
                                                                                               new SqlParameter("@LastLevelTousu", model.LastLevelTousu)
                                                                                              }).ConvertTo<OrderTypeTousuConfig>();
                }
                using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Shouhou")))
                { 
                    if (ordertypeTousuConfigList != null && ordertypeTousuConfigList.Any())
                    {
                        using (var cmd =
                   new SqlCommand(
                       @"UPDATE [OrderTypeTousuConfig] WITH(ROWLOCK) SET IsChecked=@IsChecked,IsNeedPhoto=@IsNeedPhoto,CautionText=@CautionText,GroupName=@GroupName,LastUpdateDateTime=@LastUpdateDateTime WHERE  (OrderType=@OrderType)
                                              and (LastLevelTousu=@LastLevelTousu)")
               )
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@IsChecked", model.IsChecked);
                            cmd.Parameters.AddWithValue("@IsNeedPhoto", model.IsNeedPhoto);
                            cmd.Parameters.AddWithValue("@CautionText", model.CautionText);
                            cmd.Parameters.AddWithValue("@GroupName", model.GroupName??string.Empty);
                            cmd.Parameters.AddWithValue("@OrderType", model.OrderType);
                            cmd.Parameters.AddWithValue("@LastLevelTousu", model.LastLevelTousu);
                            cmd.Parameters.AddWithValue("@LastUpdateDateTime", DateTime.Now);
                            result = dbHelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                        }
                    }
                    else
                    {
                        using (var cmd =
                  new SqlCommand(
                      @"INSERT INTO [dbo].[OrderTypeTousuConfig] ([OrderType],[TopLevelTousu],[SecondLevelTousu],[ThirdLevelTousu],[FourthLevelTousu],[LastLevelTousu],[IsNeedPhoto],[IsChecked],[CautionText],[GroupName],[CreateDateTime],[LastUpdateDateTime])
                      VALUES(@OrderType,@TopLevelTousu,@SecondLevelTousu,@ThirdLevelTousu,@FourthLevelTousu,@LastLevelTousu,@IsNeedPhoto,@IsChecked,@CautionText,@GroupName,@CreateDateTime,@LastUpdateDateTime)")
              )
                        {
                            var datetimeNow = DateTime.Now;
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.AddWithValue("@OrderType", model.OrderType);
                            cmd.Parameters.AddWithValue("@TopLevelTousu", model.TopLevelTousu);
                            cmd.Parameters.AddWithValue("@SecondLevelTousu", model.SecondLevelTousu);
                            cmd.Parameters.AddWithValue("@ThirdLevelTousu", model.ThirdLevelTousu);
                            cmd.Parameters.AddWithValue("@FourthLevelTousu", model.FourthLevelTousu);
                            cmd.Parameters.AddWithValue("@LastLevelTousu", model.LastLevelTousu);
                            cmd.Parameters.AddWithValue("@IsNeedPhoto", model.IsNeedPhoto);
                            cmd.Parameters.AddWithValue("@IsChecked", model.IsChecked);
                            cmd.Parameters.AddWithValue("@CautionText", model.CautionText);
                            cmd.Parameters.AddWithValue("@GroupName", model.GroupName??string.Empty);
                            cmd.Parameters.AddWithValue("@CreateDateTime", datetimeNow);
                            cmd.Parameters.AddWithValue("@LastUpdateDateTime", datetimeNow);
                            result = dbHelper.ExecuteNonQuery(cmd) > 0 ? true : false;
                        }

                    }
                }
            }
            return result;
        }

        public static IEnumerable<string> GetTopTousuNameList(string orderType)
        {
            List<string> topTousuNameList = new List<string>() { };
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Shouhou")))
            {
                //  pager.TotalItem = GetListCount(dbHelper, model, pager);
                DataTable dt= dbHelper.ExecuteDataTable(@"SELECT 
                                                  DISTINCT([TopLevelTousu])                                                  
                                              FROM [dbo].[OrderTypeTousuConfig] with(nolock)
                                              where (OrderType=@OrderType) and (IsChecked=1)                                                                                                                                 
                                                       	;", CommandType.Text,
                                                                                           new SqlParameter[] {
                                                                                               new SqlParameter("@OrderType",orderType)                                                                                               
                                                                                           });
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        topTousuNameList.Add(dr["TopLevelTousu"].ToString());
                    }
                    
                }
            }
            return topTousuNameList;
        }

    }
}

