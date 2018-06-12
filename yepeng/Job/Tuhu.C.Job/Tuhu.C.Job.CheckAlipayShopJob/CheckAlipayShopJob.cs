using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tuhu.Service.Shop;
using Tuhu.Service.ThirdParty;
using Tuhu.C.Job.CheckAlipayShopJob.BLL;

namespace Tuhu.C.Job.CheckAlipayShopJob
{
    [DisallowConcurrentExecution]
    public class CheckAlipayShopJob:IJob
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(CheckAlipayShopJob));
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("CheckAlipayShopJob开始执行");
            try
            {
               
               
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    bool result = ShopDifference();
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    if (result)
                        Logger.Info($"CheckAlipayShopJob执行成功");
                    else
                        Logger.Info($"CheckAlipayShopJob执行失败");
                    Logger.Info(string.Format("耗时：{0}s", ts.TotalSeconds));
              
            }
            catch (Exception ex)
            {
                Logger.Error($"CheckAlipayShopJob异常：{ex.ToString()}");
            }
        }


        public static bool ShopDifference()
        {
            bool syncsuccess = true;
            try
            {
                StringBuilder sb = new StringBuilder("");
                Dictionary<string, string> shopDic = GetShopDic();
                using (var thirdpartyclient = new Tuhu.Service.ThirdParty.AliPayServiceClient())
                {
                    foreach (KeyValuePair<string, string> item in shopDic)
                    {
                        var shopid = Convert.ToInt32(item.Key);
                        if (IsVailShop(shopid))
                        {
                            
                            var TuanGouList = ShopBusiness.GetTuanGouXiCheModel(shopid);
                            if (!TuanGouList.Any())
                            {

                                //   Console.WriteLine("途虎门店"+item.Key + "|" + item.Value+"不存在美容团购服务");
                                var result = thirdpartyclient.IsExistedShop(shopid);
                                if (result != null && result.Result)
                                {
                                   // var servicelist = ShopBusiness.tuangouxicheTypes.Select(p => shopid + "|" + p).ToList();
                                    foreach (var tuhupid in ShopBusiness.tuangouxicheTypes)
                                    {
                                        var pid = shopid + "|" + tuhupid;
                                        var serviceresult = thirdpartyclient.IsExistedShopService(pid);
                                        if (serviceresult != null && serviceresult.Result)
                                        {
                                            Logger.Info($"门店服务{shopid}不同步,需同步");
                                            thirdpartyclient.DeleteShopServiceFromCheZhuPlatform(shopid,tuhupid);
                                        
                                            sb.AppendLine("途虎门店" + item.Key + "|" + item.Value + "不存在美容团购服务");
                                            sb.AppendLine("车主平台" + item.Key + "|" + item.Value + "存在" + pid + "美容团购服务");
                                            //break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Logger.Info($"途虎门店{shopid}下架或者不在营业时间内");
                            if (ShopIsExistedInAliPay(shopid))
                            {
                                Logger.Info($"车主平台存在门店{shopid},需修改下架状态");
                                thirdpartyclient.UpdateShopsFlagToCheZhuPlatform(new List<int> { shopid }, null, false);
                            }
                            else
                            {
                                Logger.Info($"车主平台不存在门店{shopid},无需处理此门店");
                            }
                        }
                    }
                }
                if (sb.Length > 0)
                {
                    SendMail("门店不同步日志信息", ConfigurationManager.AppSettings["CheckAlipayShopJob:To"], sb.ToString());
                }
            }
            catch (Exception ex)
            {
                syncsuccess = false;
                Logger.Info("ShopDifference Error", ex);
            }
            return false;
        }

        public static bool TuhuShopIsExisted(int shopid)
        {
            bool isTuhuExisted = true;
            using (var shopclient = new ShopClient())
            {
                var tuhushopisExisted = shopclient.FetchShop(shopid);
                if (tuhushopisExisted != null && tuhushopisExisted.Result != null)
                {
                    isTuhuExisted = false;
                }
            }

            return isTuhuExisted;
        }

        public static  bool ShopIsExistedInAliPay(int shopId)
        {
            bool isExisted = false;
            using (var alipayclient = new AliPayServiceClient())
            {
                var shopisExistedResult = alipayclient.IsExistedShop(shopId);
                if (shopisExistedResult != null && shopisExistedResult.Result)
                {
                    isExisted = true;
                }
            }
            return isExisted;
        }

        public static bool IsVailShop(int shopId)
        {
            bool isvalid = false;
            using (var alipayclient = new AliPayServiceClient())
            {
                var shopisValidResult = alipayclient.IsValidShop(shopId);
                if (shopisValidResult != null && shopisValidResult.Result)
                {
                    isvalid = true;
                }
            }
            return isvalid;
        }

        public static Dictionary<string, string> GetShopDic()
        {
            Dictionary<string, string> districtDic = new Dictionary<string, string>();
            string[] txtData = File.ReadAllLines("ShopList.txt", System.Text.Encoding.UTF8);
            if (txtData.Any())
            {
                foreach (var item in txtData.Where(q => q != null))
                {
                    
                    string[] arr = Regex.Split(item, @"\s+");
                    if (!string.IsNullOrWhiteSpace(arr.First()) && !string.IsNullOrWhiteSpace(arr.Last()))
                    {
                        if (!districtDic.ContainsKey(arr.First()))
                        {
                            districtDic.Add(arr.First(), arr.Last());
                        }
                    }
                }
            }
            return districtDic;
        }


        public static void SendMail(string subject, string to, string body)
        {
            using (var smtp = new SmtpClient())
            using (var mail = new MailMessage())
            {
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                foreach (var a in to.Split(';'))
                {
                    mail.To.Add(a);
                }
                smtp.Send(mail);
            }
        }
    }
}
