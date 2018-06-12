using Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tuhu.CarArchiveService.Models;

namespace Tuhu.CarArchiveService.Utils
{
    public static class CarArchiveHelper
    {
        private static readonly string _userName = ConfigurationManager.AppSettings["uname"].ToString();
        private static readonly string _password = ConfigurationManager.AppSettings["password"].ToString();
        private static readonly string _baseuri = ConfigurationManager.AppSettings["baseuri"].ToString();
        private static readonly ILog _logger = LogManager.GetLogger("CarArchiveHelper");
        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            string result = string.Empty;

            try
            {
                var data = new Dictionary<string, object>();
                data["username"] = _userName;
                data["password"] = _password;

                var postResult = Post(_baseuri + "/restservices/lciphostrest/lcipgetaccesstoken/query", data);
                var jobject = JObject.Parse(postResult);
                if (string.Equals((string)jobject.GetValue("code"), "1"))
                {
                    result = (string)jobject.GetValue("access_token");
                }
                else
                {
                    _logger.Error($"获取AccessToken失败, result:{postResult}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("获取AccessToken失败", ex);
            }

            return result;
        }
        /// <summary>
        /// 生成维修企业唯一标识
        /// </summary>
        /// <param name="shopName"></param>
        /// <param name="licence"></param>
        /// <param name="regionCode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetCompanyCode(string shopName, string licence, string regionCode)
        {
            string result = string.Empty;
            string token = GetAccessToken();
            if (string.IsNullOrEmpty(token))
                return result;
            try
            {
                var data = new Dictionary<string, object>();
                data["access_token"] = token;
                data["companyname"] = shopName;
                data["companyroadtransportationlicense"] = licence;
                data["companyadministrativedivisioncode"] = regionCode;

                var postResult = Post(_baseuri + "/restservices/lciphostrest/lcipaccountcompany/query", data);
                var jobject = JObject.Parse(postResult);
                if (string.Equals((string)jobject.GetValue("code"), "1"))
                {
                    result = (string)jobject.GetValue("companyuniquecode");
                }
                else
                {
                    _logger.Error($"获取companyuniquecode失败, result:{postResult}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("获取companyuniquecode失败", ex);
            }

            return result;
        }
        /// <summary>
        /// 新增维修记录
        /// </summary>
        /// <param name="record"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string PushRecord(BaoYangRecordModel record)
        {
            string result = string.Empty;
            string token = GetAccessToken();
            if (string.IsNullOrEmpty(token))
                return "获取Token失败";
            dynamic item = new
            {
                access_token = token,
                administrativedivisioncode = record.ShopRegionCode,
                companyuniquecode = record.ShopCode,
                basicinfo = JsonConvert.SerializeObject(new
                {
                    vehicleplatenumber = record.PlateNumber,
                    vin = record.VinCode,
                    companyname = record.InstallShopName,
                    repairdate = record.InstallDatetime.ToString("yyyyMMdd"),
                    repairmileage = record.Distance,
                    faultdescription = "车主自行保养",
                    settledate = record.InstallDatetime.ToString("yyyyMMdd"),
                    costlistcode = record.OrderId
                }),
                vehiclepartslist = JsonConvert.SerializeObject(
                    record.PartList.Select(o => new
                    {
                        partsname = o.ProductName.Substring(0, o.ProductName.Length > 50 ? 50 : o.ProductName.Length),
                        partscode = o.PartCode,
                        partsquantity = o.Num
                    })),
                repairprojectlist = JsonConvert.SerializeObject(
                    record.ProjectList.Select(o => new
                    {
                        repairproject = o.Name,
                        workinghours = o.Price
                    }))
            };

            try
            {
                var data = new Dictionary<string, object>();
                data["str"] = JsonConvert.SerializeObject(new List<dynamic>() { item });

                result = Post(_baseuri + "/restservices/lciphostrest/lcipcarfixrecordaddall/query", data);
            }
            catch (Exception ex)
            {
                result = "发生异常";
                _logger.Error("PushRecord失败", ex);
            }

            return result;
        }

        public static string Post(string url, Dictionary<string, object> data)
        {
            using (var httpClient = new HttpClient())
            {
                var task = httpClient.PostAsJsonAsync(url, data)
                                     .ContinueWith(o => o.Result.Content.ReadAsStringAsync().Result);
                task.Wait(3000);
                return task.Result;
            }
        }
    }
}
