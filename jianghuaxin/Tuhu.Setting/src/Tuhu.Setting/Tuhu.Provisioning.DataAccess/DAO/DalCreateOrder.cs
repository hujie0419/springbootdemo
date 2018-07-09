using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalCreateOrder
    {
        public static IEnumerable<TireCreateOrderOptionsConfigModel> SelectOrderOptions()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Configuration")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT Pkid as Id,Type,Status,Pid,ProductName,ProductPrice,IsAuto,HasFreight,ServicePid,ServicePrice FROM Configuration.dbo.TireCreateOrderOptionsConfig WITH (NOLOCK) where Status=1 order by Pkid ").ConvertTo<TireCreateOrderOptionsConfigModel>();
            }
        }

        public static TireCreateOrderOptionsConfigModel SelectOrderOptionById(int id)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Configuration")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT Pkid as Id,Type,Status,Pid,ProductName,ProductPrice,IsAuto,HasFreight,ServicePid,ServicePrice FROM Configuration.dbo.TireCreateOrderOptionsConfig WITH (NOLOCK) where Status=1 and PKID=@id order by Pkid ", CommandType.Text,
                    new SqlParameter[]
                {
                    new SqlParameter("@id", id)
                }).ConvertTo<TireCreateOrderOptionsConfigModel>().FirstOrDefault();
            }
        }
        public static IEnumerable<OrderOptionReferProductModel> SelectOrderOptionReferProducts(int orderOptionId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Configuration")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT * FROM Configuration.dbo.OrderOptionReferProduct WITH (NOLOCK) where Status=1 and orderOptionId=@orderOptionId order by Pkid ", CommandType.Text,
                        new SqlParameter[]
                        {
                            new SqlParameter("@OrderOptionId", orderOptionId)
                        }).ConvertTo<OrderOptionReferProductModel>();
            }
        }

        public static int InsertOrderOptionReferProducts(OrderOptionReferProductModel refer)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Configuration")))
            {
                return dbHelper.ExecuteNonQuery(
                        @"Insert into  Configuration..OrderOptionReferProduct(OrderOptionId,Pid,Num,Price,Status)values(@OrderOptionId,@Pid,@Num,@Price,1)",
                        CommandType.Text,
                        new SqlParameter[]
                        {
                            new SqlParameter("@OrderOptionId", refer.OrderOptionId),
                            new SqlParameter("@Price", refer.Price),
                            new SqlParameter("@PID", refer.Pid),
                            new SqlParameter("@Num", refer.Num)
                        });
            }
        }

        public static int DelOrderOptionReferProducts(int orderOptionId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Configuration")))
            {
                return dbHelper.ExecuteNonQuery(
                        @"Delete from  Configuration..OrderOptionReferProduct where OrderOptionId=@OrderOptionId",
                        CommandType.Text,
                        new SqlParameter[]
                        {
                            new SqlParameter("@OrderOptionId", orderOptionId)
                        });
            }
        }
        public static int InsertOrderOption(TireCreateOrderOptionsConfigModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Configuration")))
            {
                return dbHelper.ExecuteNonQuery(
                        @"INSERT INTO Configuration..TireCreateOrderOptionsConfig
						        ( Type ,
						          Status ,
						          PID ,
						          ProductName ,
						          ProductPrice ,
						          IsAuto ,
						          CreateDateTime ,
						          LastUpdateDateTime ,
						          HasFreight ,
						          ServicePid ,
						          ServicePrice
						        )
						VALUES  ( @Type , -- Type - int
						          @Status , -- Status - bit
						          @Pid , -- PID - nvarchar(50)
						          @ProductName , -- ProductName - nvarchar(100)
						          @ProductPrice , -- ProductPrice - money
						          @IsAuto , -- IsAuto - bit
						          GETDATE() , -- CreateDateTime - datetime
						          GETDATE() , -- LastUpdateDateTime - datetime
						          @HasFreight , -- HasFreight - int
						          @ServicePid , -- ServicePid - nvarchar(100)
						          @ServicePrice  -- ServicePrice - money
						        )",
                        CommandType.Text,
                        new SqlParameter[]
                        {
                            new SqlParameter("@Type", model.Type),
                            new SqlParameter("@Status", model.Status),
                            new SqlParameter("@Pid", model.Pid),
                            new SqlParameter("@ProductName", model.ProductName),
                            new SqlParameter("@ProductPrice", model.ProductPrice),
                            new SqlParameter("@IsAuto", model.IsAuto),
                            new SqlParameter("@HasFreight", model.HasFreight),
                            new SqlParameter("@ServicePid", model.ServicePid),
                            new SqlParameter("@ServicePrice", model.ServicePrice)
                        });
            }
        }

        public static int UpdateOrderOption(TireCreateOrderOptionsConfigModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Configuration")))
            {
                return dbHelper.ExecuteNonQuery(
                        @"UPDATE  Configuration..TireCreateOrderOptionsConfig
                                SET     Type = @Type ,
                                        Status = @Status ,
                                        PID = @PID ,
                                        ProductName = @ProductName ,
                                        ProductPrice = @ProductPrice ,
                                        IsAuto = @IsAuto ,
                                        HasFreight = @HasFreight ,
                                        ServicePid = @ServicePid ,
                                        ServicePrice = @ServicePrice Where PKid=@Id",
                        CommandType.Text,
                        new SqlParameter[]
                        {
                             new SqlParameter("@Type", model.Type),
                            new SqlParameter("@Status", model.Status),
                            new SqlParameter("@Pid", model.Pid),
                            new SqlParameter("@ProductName", model.ProductName),
                            new SqlParameter("@ProductPrice", model.ProductPrice),
                            new SqlParameter("@IsAuto", model.IsAuto),
                            new SqlParameter("@HasFreight", model.HasFreight),
                            new SqlParameter("@ServicePid", model.ServicePid),
                            new SqlParameter("@ServicePrice", model.ServicePrice),
                            new SqlParameter("@Id", model.Id)
                        });
            }
        }
    }
}
