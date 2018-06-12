using Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tuhu.CarArchiveService.DataAccess;
using Tuhu.CarArchiveService.Models;
using Tuhu.CarArchiveService.Utils;
namespace Tuhu.CarArchiveService.Utils
{
    public abstract class CarArchiveHelperBase
    {
        protected abstract ILog logger { get; }

        protected abstract string baseUrl { get; }
        protected string Company_code { get; set; }
        protected string Company_password { get; set; }

        protected abstract void GetAccessToken();
        public abstract Tuple<bool, string> PushCarArchiveRecords(object model);

        protected static string Post(string url, Dictionary<string, object> data)
        {
            using (var httpClient = new HttpClient())
            {
                HttpContent reqMsg = new StringContent(JsonConvert.SerializeObject(data),Encoding.UTF8);
                reqMsg.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                var result = httpClient.PostAsync(url, reqMsg);
                result.Wait(3000);
                return result.Result.Content.ReadAsStringAsync().Result;
            }
        }
        protected static object Get(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var task = httpClient.GetAsync(url);
                task.Wait(3000);
                return task.Result.Content.ReadAsAsync<object>().Result;
            }
        }
    }
    public class CarArchiveHelperForChengDu : CarArchiveHelperBase
    {
        protected override ILog logger => LogManager.GetLogger<CarArchiveHelperForChengDu>();
        protected override string baseUrl => "http://record.acb360.cn/";
        //protected override string baseUrl => "http://182.150.22.222:8081/";

        private static Dictionary<int, string> Tokens = new Dictionary<int, string>();

        protected int Current_ShopId { get; set; }

        public CarArchiveHelperForChengDu(int shopId)
        {
            Current_ShopId = shopId;
            if (!Tokens.ContainsKey(Current_ShopId))
                GetAccessToken();
        }

        public bool CompanyRegister(ChengDuCarArchiveRegisterModel model)
        {
            if (model == null)
                return false;
            string registerUrl = $"{baseUrl}carhealth/register";
            var time = GetServiceTime();
            var header = new Dictionary<string, object>
            {
                { "date",time.Item1},
                { "time",time.Item2}
            };
            var body = new Dictionary<string, object>() {
                { "reqInfo",new Dictionary<string, object>()
                            {
                              { "companyname",model.CompanyName},
                              { "companypassword",model.CompanyPassword},
                              { "companyroadtransportationlicense",model.CompanyRoadTransportationLicense},
                              { "companyunifiedsocialcreditidentifier",model.CompanyUnifiedSocialCreditidentifier},
                              { "companyaddress",model.CompanyAddress},
                              { "companypostalcode",model.CompanyPostalcode},
                              { "companyeconomiccategory",model.CompanyEconomicCategory},
                              { "companycategory",model.CompanyCategory},
                              { "companylinkmanname",model.CompanyLinkmanname},
                              { "companylinkmantel",model.CompanyLinkmantel},
                              { "companysuperintendentname",model.CompanySuperintendentname},
                              { "companysuperintendenttel",model.CompanySuperintendenttel},
                              { "companybusinessscope",model.CompanyBusinessscope},
                              { "roadtransportationlicensestartdate",model.RoadTransportationLicenseStartdate},
                              { "roadtransportationlicenseenddate",model.RoadTransportationLicenseEnddate},
                              { "companyoperationstate",model.CompanyOperationState},
                              { "companyadministrativedivisioncode",model.CompanyAdministrativedivisioncode},
                              { "companyemail",model.CompanyEmail}
                            }
                }
            };
            var transaction = new Dictionary<string, object>()
            {
                {"header",header },
                {"body",body }
            };
            var data = new Dictionary<string, object>()
            {
                {"transaction",transaction },
            };
            var result = Post(registerUrl, data);
            var result_obj = JObject.Parse(result);
            logger.Info($"ShopId={Current_ShopId}:Result={result},Request={data}");
            if (result_obj.GetValue("respInfo")["code"].ToString() == "1")
            {
                if (!DalDefault.InsertShopCodeDic_All(Current_ShopId, result_obj.GetValue("respInfo")["companycode"].ToString(), model.CompanyPassword.AesString(), Models.PushToEnum.ChengDu))
                    logger.Error($"InsertShopCodeDic_All:注册插入数据库失败：{Current_ShopId},{result_obj.GetValue("respInfo")["companycode"]},{model.CompanyPassword}");
                Company_code = result_obj.GetValue("respInfo")["companycode"].ToString();
                Company_password = model.CompanyPassword.AesString();
                GetAccessToken();
                return true;
            }
            else
            {
                logger.Error($"CompanyRegister:Result={result},Request={JsonConvert.SerializeObject(data)}");
                return false;
            }
        }
        protected override void GetAccessToken()
        {
            if (string.IsNullOrEmpty(Company_code) || string.IsNullOrEmpty(Company_password))
                return;
            string getTokenUrl = $"{baseUrl}carhealth/token";
            var reqInfo = new Dictionary<string, object>() {
                { "companycode",Company_code},
                { "companypassword",Company_password.UnAesString()},
            };
            var body = new Dictionary<string, object>
            {
                { "reqInfo",reqInfo}
            };
            var time = GetServiceTime();
            var header = new Dictionary<string, object>
            {
                { "date",time.Item1 },
                { "time",time.Item2}
            };
            var transaction = new Dictionary<string, object>
            {
                { "header",header},
                { "body",body}
            };
            var result = JObject.Parse(Post(getTokenUrl,
                  new Dictionary<string, object>
              {
                { "transaction",transaction},
              }));
            if (result.GetValue("respInfo")["code"].ToString() == "1")
                Tokens.Add(Current_ShopId, result.GetValue("respInfo")["access_token"].ToString());
            else
            {
                logger.Error($"GetAccessToken:成都获取token异常=>companycode:{Company_code}");
            }
        }
        public void Init(string companycode,string password)
        {
            if (string.IsNullOrEmpty(companycode) || string.IsNullOrEmpty(password))
                throw new ArgumentNullException("companycode,password不能为空");
            Company_code = companycode;
            Company_password = password;
            GetAccessToken();
        }
        /// <summary>
        /// token集合重置
        /// </summary>
        public static void TokenReset()
        {
            Tokens = new Dictionary<int, string>();
        }
        public override Tuple<bool, string> PushCarArchiveRecords(object model)
        {
            if (model == null)
                return Tuple.Create(false, "model参数不能为空");
            if (!Tokens.ContainsKey(Current_ShopId))
                return Tuple.Create(false, $"token不存在=》{Current_ShopId}");

            var push_model = model as BaoYangRecordModel;
            if (push_model == null)
                return Tuple.Create(false, $"model转换为BaoYangRecordModel失败");

            string pushRecordUrl = $"{baseUrl}carhealth/record";

            var vehiclepartslist = new List<Dictionary<string, object>>();
            push_model.PartList.ForEach(o =>
            {
                var temp = new Dictionary<string, object>() {
                {"partsname", o.ProductName.Substring(0, o.ProductName.Length > 50 ? 50 : o.ProductName.Length) },
                {"partscode", o.PartCode },
                { "partsquantity", o.Num },
                 };
                vehiclepartslist.Add(temp);
            });

            var repairprojectlist = new List<Dictionary<string, object>>();
            push_model.ProjectList.ForEach(o =>
            {
                var temp = new Dictionary<string, object>() {
                {"repairproject", o.Name },
                { "workinghours", o.Price },
                };
                repairprojectlist.Add(temp);
            });

            var time = GetServiceTime();
            var header = new Dictionary<string, object>
            {
                { "date",time.Item1 },
                { "time",time.Item2}
            };
            var reqInfo = new Dictionary<string, object>() {
                { "access_token",Tokens[Current_ShopId]},

                { "basicInfo",new Dictionary<string, object>() {
                    { "vehicleplatenumber",push_model.PlateNumber},
                    {"vin", push_model.VinCode },
                    {"companyname", push_model.InstallShopName},
                    {"repairdate", push_model.InstallDatetime.ToString("yyyyMMdd")},
                    {"repairmileage", push_model.Distance},
                    {"faultdescription", "车主自行保养"},
                    {"settledate", push_model.InstallDatetime.ToString("yyyyMMdd")},
                    { "costlistcode", push_model.OrderId}
                }},

                { "vehiclepartslist",vehiclepartslist},

                { "repairprojectlist",repairprojectlist}
            };
            var body = new Dictionary<string, object>()
            {
                { "reqInfo",reqInfo}
            };
            var transaction = new Dictionary<string, object>()
            {
                { "header",header},
                { "body",body}

            };
            var data = new Dictionary<string, object>()
            {
                { "transaction",transaction}
            };

            try
            {
                logger.Info(JsonConvert.SerializeObject(data));
                var result = Post(pushRecordUrl, data);
                var result_obj = JObject.Parse(result);
                if (result_obj.GetValue("respInfo")["code"].ToString() == "1")
                    return Tuple.Create(true, result);
                return Tuple.Create(false, result);
            }
            catch (Exception ex)
            {
                logger.Error("PushCarArchiveRecords", ex);
                return Tuple.Create(false, ex.Message);
            }
        }

        private Tuple<string, string> GetServiceTime()
        {
            string getTimeUrl = $"{baseUrl}time";
            double time = 0;
            var get_Result = Get(getTimeUrl).ToString();
            if (!double.TryParse(get_Result, out time))
            {
                var now = DateTime.Now;
                return Tuple.Create($"{now.Year}{now.Month.ToString("00")}{now.Day.ToString("00")}", $"{now.Hour.ToString("00")}{now.Minute.ToString("00")}{now.Second.ToString("00")}");
            }
            var Now = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(time);
            return Tuple.Create($"{Now.Year}{Now.Month.ToString("00")}{Now.Day.ToString("00")}", $"{Now.Hour.ToString("00")}{Now.Minute.ToString("00")}{Now.Second.ToString("00")}");
        }

    }
}
