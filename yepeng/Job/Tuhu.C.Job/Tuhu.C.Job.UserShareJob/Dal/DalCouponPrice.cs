using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.UserShareJob.Model;
namespace Tuhu.C.Job.UserShareJob.Dal
{
    public class DalCouponPrice
    {
        private static ILog DalCouponPriceLogger = LogManager.GetLogger<DalCouponPrice>();
        public static IEnumerable<CouponPriceModel> GetCouponPrice()
        {
            const string sql = @"SELECT  PCPC.PID,DPSD.jingdongself_price AS  JDPrice,DPSD.qccr_retailprice AS QCCRPrice,DPSD.cost CostPrice, DPSD.num_week WeekSaleCount,DPSD.num_month MonthSaleCount,DPSD.totalstock Stock FROM [Tuhu_bi].[dbo].[dm_TireSales_Monthly]  AS PCPC  WITH(nolock)   
LEFT JOIN Tuhu_bi..dm_Product_SalespredictData AS DPSD WITH(nolock)  
ON DPSD.PID=PCPC.PID";
            List<CouponPriceModel> result = null;
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = dbhelper.CreateCommand(sql))
                {
                    result = dbhelper.ExecuteSelect<CouponPriceModel>(cmd).ToList();
                }
            }
            result = result?.Distinct()?.ToList();
            if (result != null && result.Any())
            {
                string gw_sql = @"SELECT  PID,cy_list_price AS GWPrice ,DisplayName,CP_Brand  Brand,OnSale FROM   Tuhu_productcatalog.dbo.vw_Products  WITH ( NOLOCK ) WHERE PID IN('{0}')";
                string execute_sql = string.Format(gw_sql, string.Join("','", result.Select(s => s.PID)));
                Dictionary<string, Tuple<decimal?, string, string, bool>> GwPriceList = null;
                Func<DataTable, Dictionary<string, Tuple<decimal?, string, string, bool>>> action = delegate (DataTable dt)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        GwPriceList = new Dictionary<string, Tuple<decimal?, string, string, bool>>();
                        foreach (DataRow item in dt.Rows)
                        {
                            if (!GwPriceList.ContainsKey(item.GetValue<string>("PID")))
                            {
                                GwPriceList.Add(item.GetValue<string>("PID"), Tuple.Create(item.GetValue<decimal?>("GWPrice"), item.GetValue<string>("DisplayName"), item.GetValue<string>("Brand"), item.GetValue<bool>("OnSale")));
                            }
                        }
                    }
                    return GwPriceList;
                };
                using (var cmd = new SqlCommand(execute_sql))
                {
                    DbHelper.ExecuteQuery(true, cmd, action);
                }
                if (GwPriceList != null && GwPriceList.Any())
                {
                    foreach (var item in result)
                    {
                        if (GwPriceList.ContainsKey(item.PID))
                        {
                            item.GWPrice = GwPriceList[item.PID].Item1;
                            item.DisplayName = GwPriceList[item.PID].Item2;
                            item.Brand = GwPriceList[item.PID].Item3;
                            item.OnSale = GwPriceList[item.PID].Item4;
                        }
                    }
                }



            }
            return result?.Where(w => w.OnSale);
        }


        public static bool UpdateOrInsertCouponPrice(string pid, decimal newPrice, bool update = false)
        {
            const string sql_update = @"UPDATE [Tuhu_productcatalog].[dbo].[tbl_CouponPrice] WITH(ROWLOCK)  SET  NewPrice=@NewPrice,LastUpdateDateTime=GETDATE(),UpdateDateTime=GETDATE() WHERE PID=@PID";
            const string sql_insert = @"Insert into [Tuhu_productcatalog].[dbo].[tbl_CouponPrice]  VALUES(@PID,@NewPrice,GETDATE(),GETDATE(),GETDATE())";
            var sql = update ? sql_update : sql_insert;
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@NewPrice", newPrice);
                cmd.Parameters.AddWithValue("@PID", pid);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static Tuple<bool, decimal?> IsExistCouponPrice(string pid)
        {
            const string sql = @"SELECT TOP 1  1 Exist,NewPrice FROM  [Tuhu_productcatalog].[dbo].[tbl_CouponPrice] WITH(NOLOCK)  WHERE PID=@PID";
            Func<DataTable, Tuple<bool, decimal?>> action = delegate (DataTable dt)
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Tuple.Create(dt.Rows[0].GetValue<bool>("Exist"), dt.Rows[0].GetValue<decimal?>("NewPrice"));
                }
                return Tuple.Create(false, default(decimal?));
            };
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PID", pid);
                return DbHelper.ExecuteQuery(true, cmd, action);
            }
        }

        public static bool InsertApproveProcess(string pid, string brand, decimal? costPrice, int stockCount, int weekSaleCount, int monthSaleCount, decimal? guideprice, decimal? jdPrice, decimal? carPrice, string poductName,
           decimal? nowPrice, decimal newPrice, decimal? gwPrice)
        {
            const string sql = @"INSERT  INTO [Configuration].[dbo].[PriceApply] VALUES(@PID
      ,@Brand
      ,@CostPrice
      ,Null
      ,@StockCount
      ,@WeekSaleCount
      ,@MonthSaleCount
      ,@GuidePrice
      ,@JDPrice
      ,@CarPrice
      ,@GrossProfit
      ,@PoductName
      ,@NowPrice
      ,@NewPrice
      ,@ApplyReason
      ,@ApplyDateTime
      ,@ApplyPerson
      ,@ApplyStatus
      ,@CreateDateTime
      ,@LastUpdateDateTime
      ,@TuhuPrice)";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PID", pid);
                cmd.Parameters.AddWithValue("@Brand", (brand ?? ""));
                cmd.Parameters.AddWithValue("@CostPrice", costPrice);
                cmd.Parameters.AddWithValue("@StockCount", stockCount);
                cmd.Parameters.AddWithValue("@WeekSaleCount", weekSaleCount);
                cmd.Parameters.AddWithValue("@MonthSaleCount", monthSaleCount);
                cmd.Parameters.AddWithValue("@GuidePrice", guideprice);
                cmd.Parameters.AddWithValue("@JDPrice", jdPrice);
                cmd.Parameters.AddWithValue("@CarPrice", carPrice);
                cmd.Parameters.AddWithValue("@GrossProfit", newPrice - costPrice);
                cmd.Parameters.AddWithValue("@PoductName", (poductName ?? ""));
                cmd.Parameters.AddWithValue("@NowPrice", nowPrice);
                cmd.Parameters.AddWithValue("@NewPrice", newPrice);
                cmd.Parameters.AddWithValue("@ApplyReason", "系统自动更新");
                cmd.Parameters.AddWithValue("@ApplyDateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@ApplyPerson", "系统自动更新");
                cmd.Parameters.AddWithValue("@ApplyStatus", 0);
                cmd.Parameters.AddWithValue("@TuhuPrice", gwPrice);
                cmd.Parameters.AddWithValue("@CreateDateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@LastUpdateDateTime", DateTime.Now);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }

        }

        public static bool InsertCouponPriceHistory(decimal? oldPrice, decimal newPrice, string pid)
        {
            const string sql = @"INSERT INTO  [Tuhu_log].[dbo].[tbl_CouponPriceHistory] VALUES
(
 @PID
 ,@OldPrice
 ,@NewPrice
 ,@ChangeDateTime
 ,@ChangeUser
 ,@ChangeReason
 ,@CreateDateTime
 ,@LastUpdateDateTime
)";
            using (var db = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PID", pid);
                    cmd.Parameters.AddWithValue("@OldPrice", oldPrice);
                    cmd.Parameters.AddWithValue("@NewPrice", newPrice);
                    cmd.Parameters.AddWithValue("@ChangeDateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ChangeUser", "系统自动更新");
                    cmd.Parameters.AddWithValue("@ChangeReason", "系统自动更新");
                    cmd.Parameters.AddWithValue("@CreateDateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@LastUpdateDateTime", DateTime.Now);
                    return db.ExecuteNonQuery(cmd) > 0;
                }
            }

        }

        public static bool UpdateApproveProcess(string pid, string brand, decimal? costPrice, int stockCount, int weekSaleCount, int monthSaleCount, decimal? guideprice, decimal? jdPrice, decimal? carPrice, string poductName,
         decimal? nowPrice, decimal newPrice, decimal? gwPrice)
        {
            const string sql = @"Update [Configuration].[dbo].[PriceApply] 
SET
      Brand=@Brand
      ,CostPrice=@CostPrice
      ,StockCount=@StockCount
      ,WeekSaleCount=@WeekSaleCount
      ,MonthSaleCount=@MonthSaleCount
      ,GuidePrice=@GuidePrice
      ,JDPrice=@JDPrice
      ,CarPrice=@CarPrice
      ,GrossProfit=@GrossProfit
      ,PoductName=@PoductName
      ,NowPrice=@NowPrice
      ,NewPrice=@NewPrice
      ,ApplyReason=@ApplyReason
      ,ApplyDateTime=@ApplyDateTime
      ,LastUpdateDateTime=@LastUpdateDateTime
      ,TuhuPrice=@TuhuPrice
WHERE PID=@PID  AND ApplyPerson=N'系统自动更新'  AND ApplyStatus=0";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PID", pid);
                cmd.Parameters.AddWithValue("@Brand", (brand ?? ""));
                cmd.Parameters.AddWithValue("@CostPrice", costPrice);
                cmd.Parameters.AddWithValue("@StockCount", stockCount);
                cmd.Parameters.AddWithValue("@WeekSaleCount", weekSaleCount);
                cmd.Parameters.AddWithValue("@MonthSaleCount", monthSaleCount);
                cmd.Parameters.AddWithValue("@GuidePrice", guideprice);
                cmd.Parameters.AddWithValue("@JDPrice", jdPrice);
                cmd.Parameters.AddWithValue("@CarPrice", carPrice);
                cmd.Parameters.AddWithValue("@GrossProfit", newPrice - costPrice);
                cmd.Parameters.AddWithValue("@PoductName", (poductName ?? ""));
                cmd.Parameters.AddWithValue("@NowPrice", nowPrice);
                cmd.Parameters.AddWithValue("@NewPrice", newPrice);
                cmd.Parameters.AddWithValue("@ApplyReason", "系统自动更新");
                cmd.Parameters.AddWithValue("@ApplyDateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@TuhuPrice", gwPrice);
                cmd.Parameters.AddWithValue("@LastUpdateDateTime", DateTime.Now);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }

        }

        public static bool IsExistSystemApproveProcess(string PID)
        {
            const string sql = @"Select TOP  1 1 FROM  [Configuration].[dbo].[PriceApply] WITH(NOLOCK)
WHERE PID=@PID  AND ApplyPerson=N'系统自动更新'  AND ApplyStatus=0";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PID", PID);
                cmd.Parameters.AddWithValue("@LastUpdateDateTime", DateTime.Now);
                return DbHelper.ExecuteScalar(cmd) != null;
            }

        }

        public static bool CleanSystemApproveProcess()
        {
            const string sql = @"DELETE FROM  [Configuration].[dbo].[PriceApply] WHERE ApplyPerson=N'系统自动更新'  AND ApplyStatus=0";
            using (var cmd = new SqlCommand(sql))
            {
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
        private static bool CleanCouponPrice()
        {
            const string sql = @"DELETE FROM  [Tuhu_productcatalog].[dbo].[tbl_CouponPrice] ";
            using (var cmd = new SqlCommand(sql))
            {
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
        public static bool CleanProductCache(bool isDelete = true)
        {
            const string sql = @"SELECT PID FROM  [Tuhu_productcatalog].[dbo].[tbl_CouponPrice] WITH(NOLOCK)";
            Dictionary<string, object> PIDS = new Dictionary<string, object>();
            Func<DataTable, Dictionary<string, object>> action = delegate (DataTable dt)
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        if (!PIDS.ContainsKey(item.GetValue<string>("PID")))
                        {
                            PIDS.Add(item.GetValue<string>("PID"), "UpsertProduct");
                        }
                    }
                }
                return PIDS;
            };
            using (var cmd = new SqlCommand(sql))
            {
                DbHelper.ExecuteQuery(true, cmd, action);
            }
            if (isDelete)
            {
                CleanCouponPrice();
            }
            var result = SendCleanProductCacheMsg(PIDS);
            return result;
        }
        private static bool SendCleanProductCacheMsg(Dictionary<string, object> PIDS)
        {
            if (PIDS != null && PIDS.Any())
            {
                try
                {
                    TuhuNotification.SendNotification("ProductModify", PIDS, 5000);
                }
                catch (Exception ex)
                {
                    DalCouponPriceLogger.Error($"清除产品缓存失败：{ex}");
                }
            }
            return true;
        }
    }
}

