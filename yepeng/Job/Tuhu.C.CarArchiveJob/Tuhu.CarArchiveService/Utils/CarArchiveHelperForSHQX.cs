using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.CarArchiveService.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace Tuhu.CarArchiveService.Utils
{
    public class CarArchiveHelperForSHQX
    {
        private static ILog logger = LogManager.GetLogger<CarArchiveHelperForSHQX>();

        private static string baseUrl = "http://api.qcda.shanghaiqixiu.org/restservices/lcipprodatarest/{0}/query";
        /// <summary>
        /// 获取解密后密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string GetDecryptionPassWord(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;
            return password.UnAesString();
        }
        /// <summary>
        /// 获取加密后密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetEncryptPassWord(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;
            return password.AesString();
        }
        /// <summary>
        /// 获取门店token
        /// </summary>
        /// <param name="companycode"></param>
        /// <param name="companypassword"></param>
        /// <returns></returns>
        public static string GetAccessToken(string companycode, string companypassword)
        {
            if (string.IsNullOrEmpty(companypassword) || string.IsNullOrEmpty(companycode))
                return null;
            companypassword = GetDecryptionPassWord(companypassword);
            string getTokenUrl = string.Format(baseUrl, "lcipprogetaccesstoken");
            var request_dic = new Dictionary<string, object>();
            request_dic.Add("companycode", companycode);
            request_dic.Add("companypassword", companypassword);
            var result = Post(getTokenUrl, request_dic);
            var resultModel = JObject.Parse(result);
            var code = resultModel.GetValue("code").ToString();
            if (code == "1")
                return resultModel.GetValue("access_token").ToString();
            else
            {
                logger.Error($"GetAccessToken:获取token失败：companycode={companycode}");
                return string.Empty;
            }
        }
        /// <summary>
        /// 新增维修记录
        /// </summary>
        /// <param name="record"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Tuple<bool, string> PushCarArchiveRecords(BaoYangRecordModel push_model, string current_token)
        {

            string pushRecordUrl = string.Format(baseUrl, "lcipprocarfixrecordadd");
            var data = new Dictionary<string, object>();
            data.Add("access_token", current_token);
            var basicinfo = new Dictionary<string, object>();
            basicinfo.Add("vehicleplatenumber", push_model.PlateNumber);
            basicinfo.Add("vin", push_model.VinCode);
            basicinfo.Add("companyname", push_model.InstallShopName);
            basicinfo.Add("repairdate", push_model.InstallDatetime.ToString("yyyyMMdd"));
            basicinfo.Add("repairmileage", push_model.Distance);
            basicinfo.Add("faultdescription", "车主自行保养");
            basicinfo.Add("settledate", push_model.InstallDatetime.ToString("yyyyMMdd"));
            basicinfo.Add("costlistcode", push_model.OrderId);
            data.Add("basicInfo", basicinfo);
            var vehiclepartslist = new List<Dictionary<string, object>>();
            push_model.PartList.ForEach(o =>
            {
                var temp = new Dictionary<string, object>();
                temp.Add("partsname", o.ProductName.Substring(0, o.ProductName.Length > 50 ? 50 : o.ProductName.Length));
                temp.Add("partscode", o.PartCode);
                temp.Add("partsquantity", o.Num);
                vehiclepartslist.Add(temp);
            });
            data.Add("vehiclepartslist", vehiclepartslist.ToArray());

            var repairprojectlist = new List<Dictionary<string, object>>();
            push_model.ProjectList.ForEach(o =>
            {
                var temp = new Dictionary<string, object>();
                temp.Add("repairproject", o.Name);
                temp.Add("workinghours", o.Price);
                repairprojectlist.Add(temp);
            });
            data.Add("repairprojectlist", repairprojectlist.ToArray());
            try
            {
                logger.Info(JsonConvert.SerializeObject(data));
                var result = Post(pushRecordUrl, data);
                if (result.Contains("新增成功"))
                    return Tuple.Create(true, result);
                return Tuple.Create(false, result);
            }
            catch (Exception ex)
            {
                logger.Error("PushCarArchiveRecords", ex);
                return Tuple.Create(false, ex.Message);
            }
        }

        private static string Post(string url, Dictionary<string, object> data)
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
