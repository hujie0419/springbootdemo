using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.DataAccess;

namespace Tuhu.C.SyncProductPriceJob.Job
{
    [DisallowConcurrentExecution]
    public class HotProductMonitor : SyncProductPriceJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HotProductMonitor));

        public override void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始热销产品价格");

            var dt = Products.QueryHotProduct();

            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }

            var result = new Dictionary<string, IList<Tuple<decimal, DataRow>>>();
            Task.WaitAll(dt.AsEnumerable().ParallelSelect(row => Task.Run(async () =>
            {
                try
                {
                    var shopCode = row["ShopCode"].ToString();

                    if (shopCode == "养车无忧")
                    {
                        Logger.InfoFormat("开始同步{0}的产品{1}的价格", shopCode, row.GetValue("ItemCode"));
                    }
                    else
                    {
                        Logger.InfoFormat("开始同步{0}的产品{1}的价格", shopCode, Convert.ToInt64(row["SkuID"]) < 1 ? Convert.ToInt64(row["ItemID"]) : Convert.ToInt64(row["SkuID"]));
                    }

                    var price = await SyncThirdPartyPrice(shopCode, row["Pid"].ToString(), Convert.ToInt64(row["ItemID"]), Convert.ToInt64(row["SkuID"]), row.GetValue("ItemCode"));
                    if (price > 0 && price.Value != Convert.ToDecimal(row["Price"]))
                    {
                        if (!result.ContainsKey(shopCode))
                            result[shopCode] = new List<Tuple<decimal, DataRow>>();

                        result[shopCode].Add(Tuple.Create(price.Value, row));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"{row[0]}: {row["ItemID"]} {row["SkuID"]}", ex);
                }
            }), 10).ToArray());

            SendEmail(result);

            Logger.Info("完成热销产品价格");
        }

        public static void SendEmail(IDictionary<string, IList<Tuple<decimal, DataRow>>> result)
        {
            if (result == null || result.Count == 0)
                return;

            var sb = new StringBuilder();
            sb.Append(@"<table border=""1"" style=""border-collapse: collapse;"">
	<thead>
		<tr>
			<th>变动的商品名</th>
			<th>变动前的价格</th>
			<th>变动后的价格</th>
			<th>途虎自营的价格</th>
			<th>途虎天猫的价格</th>
		</tr>
	</thead>
");

            foreach (var shop in result)
            {
                sb.Append(@"<tbody>
		<tr><td colspan=""5""><h3>");
                sb.Append(shop.Key);
                sb.Append(@"</h3></td></tr>");
                foreach (var row in shop.Value)
                {
                    sb.Append(@"
		<tr>");

                    sb.AppendFormat(@"
			<td>{0}</td>", row.Item2["DisplayName"]);
                    sb.AppendFormat(@"
			<td>{0:￥0.00}</td>", row.Item2["Price"]);
                    sb.AppendFormat(@"
			<td><a target=""_blank"" href=""{0}"">{1:￥0.00}</a></td>", GetUrl(shop.Key, Convert.ToInt64(row.Item2["ItemID"]), Convert.ToInt64(row.Item2["SkuID"]), row.Item2.GetValue("ItemCode")), row.Item1);
                    sb.AppendFormat(@"
			<td><a target=""_blank"" href=""{0}"">{1:￥0.00}</a></td>", GetSiteProductUrl(row.Item2["Pid"].ToString()), row.Item2["SitePrice"]);
                    sb.AppendFormat(@"
			<td><a target=""_blank"" href=""http://detail.tmall.com/item.htm?id={0}"">{1:￥0.00}</a></td>", row.Item2["TmallItemID"], row.Item2["TmallPrice"]);

                    sb.Append(@"
		</tr>");
                }
                sb.Append(@"
	</tbody>");
            }

            sb.Append(@"
</table>");

            TuhuMessage.SendEmail($"竞品价格监测波动邮件通知({DateTime.Now:yyyy-MM-dd HH}点钟)",
                "徐健<xujian@tuhu.cn>;平台运营<pingtaiyunying@tuhu.cn>;",
                sb.ToString());
        }

        public static string GetSiteProductUrl(string pid)
        {
            if (string.IsNullOrWhiteSpace(pid))
                return "#";

            var arr = pid.Split(new[] { '|' }, StringSplitOptions.None);
            return "http://item.tuhu.cn/Products/" + arr[0] + (arr.Length > 1 ? "/" + arr[1] : "") + ".html";
        }

        public static string GetUrl(string shopCode, long itemId, long skuId, string itemCode)
        {
            switch (shopCode)
            {
                case "麦轮胎官网":
                    return "http://www.mailuntai.cn/product/" + itemId + ".html";

                case "京东自营":
                    return "http://item.jd.com/" + skuId + ".html";

                case "养车无忧":
                    return "http://item.yangche51.com/p-" + itemCode + ".html";

                case "汽车超人零售":
                    return "http://www.qccr.com/detail/" + itemId + ".html";

                case "汽车超人批发":
                    return "https://pch.qccr.com/html/goods/itemdetail.html?id=" + itemId;

                default:
                    return "http://detail.tmall.com/item.htm?id=" + itemId;
            }
        }
    }
}
