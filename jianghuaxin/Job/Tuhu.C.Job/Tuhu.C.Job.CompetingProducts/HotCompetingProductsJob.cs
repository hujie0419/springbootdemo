using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.CompetingProducts.Models;

namespace Tuhu.C.Job.CompetingProducts
{
    [DisallowConcurrentExecution]
    public class HotCompetingProductsJob : CompetingProductsJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HotCompetingProductsJob));

        public override void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始同步热销竞品价格");

            //获取热销前100产品
            DataTable dtHotProduct = null;
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = dbHelper.CreateCommand(@"SELECT TOP 100  p.pid FROM Tuhu_bi.dbo.dm_Product_SalespredictData p WITH(NOLOCK) ORDER BY p.num_threemonth DESC"))
                {
                    dtHotProduct = dbHelper.ExecuteQuery(cmd, _ => _);
                }
            }
            //查询所有监控竞品
            DataTable dtProduct = null;
            using (var cmd = new SqlCommand(@"
                                                SELECT  PKID ,
                                                    ShopCode ,
                                                    ItemID ,
                                                    SkuID ,
                                                    Pid
                                            FROM    Tuhu_productcatalog..CompetingProductsMonitor WITH(NOLOCK)
                                            WHERE Is_Deleted=0;"))
                dtProduct = DbHelper.ExecuteQuery(cmd, _ => _);

            if (dtProduct == null || dtProduct.Rows.Count == 0)
                return;
            //datatable关联过滤掉非Hot100数据
            var query = from hotProduct in dtHotProduct.AsEnumerable()
                        join product in dtProduct.AsEnumerable()
                        on hotProduct.Field<string>("pid") equals product.Field<string>("Pid")
                        select product;
            DataTable dt = query.CopyToDataTable();
            if (dt == null || dt.Rows.Count == 0)
                return;

            Task.WaitAll(dt.AsEnumerable().ParallelSelect(row => Task.Run(async () =>
            {
                try
                {
                    var shopCode = row["ShopCode"].ToString();

                    if (shopCode == "养车无忧")
                    {
                        Logger.InfoFormat("开始同步{0}的产品{1}的价格", row.GetValue("ItemCode"));
                    }
                    else
                    {
                        Logger.InfoFormat("开始同步{0}的产品{1}的价格", shopCode, Convert.ToInt64(row["SkuID"]) < 1 ? Convert.ToInt64(row["ItemID"]) : Convert.ToInt64(row["SkuID"]));
                    }

                    var price = await SyncThirdPartyPrice(long.Parse(row["PKID"].ToString()), shopCode, row["Pid"].ToString(), Convert.ToInt64(row["ItemID"]), Convert.ToInt64(row["SkuID"]), row.GetValue("ItemCode"));

                }
                catch (Exception ex)
                {
                    //Logger.Error($"{row[0]}: {row["ItemID"]} {row["SkuID"]}", ex);
                    Logger.Info(new LogModel() { Message = ex.Message, RefNo = $"{row[0]}: {row["ItemID"]} {row["SkuID"]}" });
                }
            }), 10).ToArray());

            Logger.Info("完成同步热销竞品价格");
        }
    }
}

