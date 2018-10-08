using Common.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.OprLog;
using Tuhu.C.Job.Tire.DAL;
using Tuhu.C.Job.Tire.Model;

namespace Tuhu.C.Job.Tire.BLL
{
    public class TireStockoutStatusWhileManager
    {
        public static IEnumerable<string> SelectTiresMatchAndSaleQuantityMoreThanEight()
            => DalTireStockoutStatusWhile.SelectTiresMatchAndSaleQuantityMoreThanEight();

        public static int TruncateTableInfo()
            => DalTireStockoutStatusWhile.TruncateTableInfo();

        public static bool JoinWhiteList(IEnumerable<string> pids, ILog logger)
        {
            int result = -99;
            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                dbhelper.BeginTransaction();
                //已存在
                var exsitPids = DalTireStockoutStatusWhile.SelectExsitWhiteList(dbhelper, pids);
                if (exsitPids.Any())
                {
                    //删
                    result = DalTireStockoutStatusWhile.DeleteExsit(dbhelper, exsitPids);
                    if (result <= 0)
                    {
                        dbhelper.Rollback();
                        return false;
                    }
                }

                //加入
                result = DalTireStockoutStatusWhile.JoinWhiteList(dbhelper, pids);
                if (result <= 0)
                {
                    dbhelper.Rollback();
                    return false;
                }
                dbhelper.Commit();

                // 发送邮件
                var data = pids.Except(exsitPids).ToList();
                SendEmail(data, logger);

                foreach (var item in exsitPids)
                    AddOprLog(item, true);

                foreach (var item in pids)
                    AddOprLog(item, false);
                return true;
            }
        }

        public static void AddOprLog(string pid, bool isDelete)
        {
            using (var client = new OprLogClient())
            {
                var result = client.AddOprLog(new Service.OprLog.Models.OprLogModel()
                {
                    Author = "Job定时更新",
                    ChangeDatetime = DateTime.Now,
                    AfterValue = pid,
                    ObjectType = "StockWhite",
                    Operation = isDelete ? "移除白名单(系统)" : "加入白名单(系统)"
                });
                result.ThrowIfException(true);
            }
        }

        public static void SendEmail(List<string> pids, ILog logger)
        {
            if (pids.Any())
            {
                var baseInfo = DalTireStockoutStatusWhile.GetBaseProductInfo(pids);
                var stockoutInfo = DalTireStockoutStatusWhile.GetStockoutStatus(pids);
                var data = baseInfo.Union(stockoutInfo).GroupBy(g => g.PID).Select(g => new TireStockProductModel
                {
                    PID = g.Key,
                    DisplayName = g.FirstOrDefault(t => !string.IsNullOrEmpty(t.DisplayName))?.DisplayName ?? "",
                    Brand = g.FirstOrDefault(t => !string.IsNullOrEmpty(t.Brand))?.Brand ?? "",
                    OnSale = g.Any(t => t.OnSale),
                    StockoutStatus = g.Max(t => t.StockoutStatus),
                    CurrentStockCount = g.Max(t => t.CurrentStockCount),
                    MonthSales = g.Max(t => t.MonthSales)
                }).ToList();
                var body = new StringBuilder(51200);
                var emailhead =
                    $@"<div style='font-size:20px;font-weight:bold;'><p>白名单变更记录:</p><p>{
                            DateTime.Now.ToString(CultureInfo.CurrentCulture)
                        }</p></div>";
                body.Append(emailhead);
                body.Append(TableMessage("以下PID被加入白名单", data));
                var dat = data.Where(g => (g.SystemStockout == 1 || g.StockoutStatus == 1) && g.OnSale).ToList();
                var message = body.Append(TableMessage("以下PID的展示状态由缺货变为有货", dat)).ToString();
                TuhuMessage.SendEmail("【Info】白名单状态变更记录",
                    "fuwenbo@tuhu.cn",
                    message);
            }
            else
            {
                logger.Warn("无新增缺货白名单");
            }
        }

        private static string TableMessage(string title, List<TireStockProductModel> pids)
        {
            var body = new StringBuilder(10000);
            var tableHead = $@"<p style='font-size:16px;font-weight:bold;margin-top:30px;'>{title}</p>
  <table border='1' cellspacing='0' cellpadding='0'>
    <thead>
      <tr>
        <th>PID</th>
        <th>品牌</th>
        <th>名称</th>
        <th>近30天销量</th>
        <th>可用库存量</th>
      </tr>
    </thead>
    <tbody>";
            body.Append(tableHead);
            foreach (var item in pids)
            {
                if (!string.IsNullOrEmpty(item.PID))
                {
                    body.Append("<tr>");
                    body.Append($"<td>{item.PID}</td>");
                    body.Append($"<td>{item.Brand}</td>");
                    body.Append($"<td>{item.DisplayName}</td>");
                    body.Append($"<td>{item.MonthSales}</td>");
                    body.Append($"<td>{item.CurrentStockCount}</td></tr>");
                }
            }
            body.Append(@"</tbody></table>");
            return body.ToString();
        }

    }
}
