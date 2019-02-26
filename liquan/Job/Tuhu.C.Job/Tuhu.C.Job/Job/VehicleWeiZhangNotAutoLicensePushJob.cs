using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using Tuhu.Nosql;
using Tuhu.Service.Vehicle.Model;

namespace Tuhu.C.Job.Job
{
    /// <summary>
    /// 车辆违章自动提醒
    /// </summary>
    [DisallowConcurrentExecution]
    public class VehicleWeiZhangNotAutoLicensePushJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(VehicleWeiZhangNotAutoLicensePushJob));

        private static readonly HttpClient CheXingYiClient = new HttpClient
        {
            BaseAddress = new Uri(ConfigurationManager.AppSettings["CheXingYiHost"])
        };
        private static IEnumerable<Province> Provinces = null;
        private static readonly string Env = ConfigurationManager.AppSettings["Env"];
        private static readonly Dictionary<string, Tuple<string, string>> UserPassword =
            new Dictionary<string, Tuple<string, string>>()
            {
                ["dev"] = Tuple.Create("tuhuychtest", "QrnT0QDcf5wsNhNmHufEgw=="),
                ["pro"] = Tuple.Create("tuhu2016", "Ktbzuy3fKl+DmEJWSSR3Mw=="),
            };
        private static readonly int WeiZhang = 2788;//违章查询的模板id
        private static readonly int NoWeiZhang = 2787;//没有违章的模板id

        //private static readonly int NoWeiZhang = 842;//没有违章的模板id
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("VehicleWeiZhangNotAutoLicensePushJob 开始执行");
                Provinces = GetProvinceCache();
                //记录推送的历史数据
                Dictionary<string, PushHistory> _PushHistoryList = new Dictionary<string, PushHistory>();

                int maxId = GetPeccancyLogMaxPkid();
                int totalCount = GetPeccancyLogCount(maxId);

                Logger.Info("VehicleWeiZhangNotAutoLicensePushJob totalCount =" + totalCount);
                int pageIndex = 1;
                int pageSize = 100;
                //每次查询一百条日志
                while (pageIndex <= Math.Ceiling(totalCount / (pageSize * 1.0)))
                {
                    var datas = GetPeccancyLog(pageIndex, pageSize, maxId);
                    if (datas == null || !datas.Any()) break;
                    foreach (var item in datas)
                    {
                        if (item.UserID == Guid.Empty)
                        {
                            continue;
                        }
                        CarDetail _CarDetail = PushCheck(item);
                        if (_CarDetail==null)
                        {
                            continue;
                        }
                        else
                        {
                            //避免重复推送
                            string key = item.UserID.ToString() + item.PlateNumber;
                            var check = _PushHistoryList.ContainsKey(key);
                            if (!check)
                            {
                                _PushHistoryList.Add(key, new PushHistory() { PlateNumber = item.PlateNumber, UserID = item.UserID });
                                Send(_CarDetail);
                            }
                        }
                    }
                    pageIndex++;
                    System.Threading.Thread.Sleep(100);
                }
                Logger.Info("VehicleWeiZhangNotAutoLicensePushJob 执行结束 推送的数据数 _PushHistoryList=" + _PushHistoryList.Count);
            }
            catch (Exception e)
            {
                Logger.Error("VehicleWeiZhangNotAutoLicensePushJob Execute Exception=" + e.Message, e);
            }

        }


        private void Send(CarDetail _CarDetail)
        {
            using (var client = new Service.Push.TemplatePushClient())
            {
               
                if (_CarDetail != null)
                {
                    var result = GetResponse(_CarDetail);
                    if (result != null)
                    {
                        if (result.HasData)
                        {
                            Notification(_CarDetail, WeiZhang, client);
                        }
                        else
                        {
                            Notification(_CarDetail, NoWeiZhang, client);
                        }
                    }
                }
            }
        }

        private CarDetail PushCheck(PeccancyLog data)
        {
            CarDetail car = new CarDetail();
            try
            {
                if (data == null )
                {
                    return null;
                }
                else if (data.Status != 0 && data.Status != 1)
                {
                    return null;
                }

                //验证是否是微信用户
                if (!CheckWeiXinUser(data.UserID))
                {
                    return null;
                }
                //获取车型信息 carID
                var carObject = GetCarObject(data.UserID, data.PlateNumber);
                if (carObject == null)
                {
                    return null;
                }
                //是否开通了自动查违章提醒
                VehicleTypeCertificationInfo _VehicleTypeCertificationInfo = GetVehicleTypeCertificationInfo(carObject.CarId);
                if (_VehicleTypeCertificationInfo != null && _VehicleTypeCertificationInfo.IsAutoLicenseSearch == 1)
                {
                    return null;
                }
                car.CarId = carObject.CarId;
                car.CarNumber = data.PlateNumber;
                car.UserId = data.UserID;
                car.VinCode = carObject.VinCode;
                car.EngineNo = carObject.Engineno;
                car.Carno_Province = carObject.Carno_Province;
                car.Carno_City = carObject.Carno_City;
                return car;
            }
            catch (Exception ex)
            {
                Logger.Info("VehicleWeiZhangNotAutoLicensePushJob PushCheck  ex=" + ex.Message);
                return null;
            }
        }

        private QueryResult GetResponse(CarDetail item)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("userid", UserPassword[Env].Item1);
            dic.Add("userpwd", UserPassword[Env].Item2);
            dic.Add("carnumber", item.CarNumber);
            dic.Add("carcode", item.VinCode);
            dic.Add("cardrivenumber", item.EngineNo);
            var carNoCity = GetCityId(item);
            if (carNoCity > 0)
            {
                dic.Add("cityid", carNoCity);
            }

            try
            {
                var data = JsonConvert.DeserializeObject<QueryResult>(CheXingYiClient.GetStringAsync("CFTQueryindex.aspx?" + dic.ToHtmlString()).Result);
                //WriteLog(item, data);
                if (!string.IsNullOrEmpty(data.ErrMessage))
                {
                    Logger.Error(data.ErrMessage);
                }
                else
                {
                    return data;
                }

            }
            catch (Exception ex)
            {
                Logger.Info("VehicleWeiZhangNotAutoLicensePushJob GetResponse  ex=" + ex.Message);
            }
            return null;
        }

        private IEnumerable<Province> GetProvinceCache()
        {
            using (var client = CacheHelper.CreateCacheClient())
            {
                var response = client.GetOrSet<IEnumerable<Province>>("Provinces", GetProvince, TimeSpan.FromDays(1));
                if (response.Success)
                {
                    return response.Value;
                }
                else
                {
                    return GetProvince();
                }
            }
        }
        private IEnumerable<Province> GetProvince()
        {
            try
            {
                return JsonConvert.DeserializeObject<IEnumerable<Province>>(CheXingYiClient.GetStringAsync("InputsCondition.aspx?from=" + UserPassword[Env].Item1).Result);

            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }

            return new List<Province>();
        }

        private int GetCityId(CarDetail item)
        {
            if (Provinces == null) return 0;
            var provinceItem = Provinces.FirstOrDefault(x =>
            {
                if (!string.IsNullOrEmpty(item.Carno_Province) && item.Carno_Province.Contains(x.ProvinceName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
            if (provinceItem == null || provinceItem.Cities == null || !provinceItem.Cities.Any()) return 0;
            var cityItem = provinceItem.Cities.FirstOrDefault(x =>
            {
                if (!string.IsNullOrEmpty(item.Carno_City) && item.Carno_City.Contains(x.Name))
                    return true;
                else
                    return false;
            });
            if (cityItem == null) return 0;
            return cityItem.CityID;
        }

        //推送
        private void Notification(CarDetail item, int templateId, Service.Push.ITemplatePushClient client)
        {
            if (item.UserId == Guid.Empty) return;
            //无论违章否，都发通知
            var replacement = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                //["{{type}}"] = "customCar",
                ["{{carID}}"] = System.Web.HttpUtility.UrlEncode(item.CarId.ToString()),
                ["{{CarNo}}"] = item.CarNumber

            });
            //Logger.Info("VehicleWeiZhangNotAutoLicensePushJob 推送的数据数 CarDetail=" + JsonConvert.SerializeObject(item));

            var response = client.PushByUserIDAndBatchID(new List<string> { item.UserId.ToString() }, templateId,
                new Service.Push.Models.Push.PushTemplateLog()
                {
                    Replacement = replacement
                });
            Logger.Info("VehicleWeiZhangPushJob 推送的数据数 CarDetail=" + JsonConvert.SerializeObject(item) + "||结果= " + response.Success);
        }

        /// <summary>
        /// 获取车型的信息 
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="PlateNumber"></param>
        /// <returns></returns>
        public static CarObject GetCarObject(Guid UserID, string PlateNumber)
        {
            const string sqlStr = @"SELECT TOP (1)
                                    UserID ,CarId ,u_carno,Classno AS VinCode,Engineno,Carno_Province,Carno_City 
                                    FROM    Tuhu_profiles..CarObject WITH ( NOLOCK )
                                    WHERE   UserID = @UserID and u_carno=@PlateNumber
                                            AND IsDeleted = 0
                                    ORDER BY CarId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@PlateNumber", PlateNumber);
                return DbHelper.ExecuteSelect<CarObject>(true, cmd).FirstOrDefault();
            }
        }

        /// <summary>
        /// 验证是否是微信用户
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static bool CheckWeiXinUser(Guid UserID)
        {
            const string sql = @" SELECT count(*) FROM Tuhu_notification..WXUserAuth WITH(NOLOCK) 
                                     WHERE Channel='WX_APP_OfficialAccount' AND BindingStatus='Bound'
                                     and UserId=@UserID
                                     ";

            using (var helper = DbHelper.CreateLogDbHelper(false))
            {
                using (var cmd = helper.CreateCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    SqlParameter p = new SqlParameter("@UserID", SqlDbType.UniqueIdentifier);
                    p.Value = UserID;
                    cmd.Parameters.Add(p);
                    var result = (int)cmd.ExecuteScalar()>0?true:false;
                    return result;
                }
            }
        }


        /// <summary>
        /// 获取查询违章提醒的 日志
        /// </summary>
        /// <returns></returns>
        private IEnumerable<PeccancyLog> GetPeccancyLog(int pageIndex, int pageSize,int maxId)
        {
            DataSet ds = new DataSet();
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                string sql = $@"SELECT
                                T.PKID , 
                                T.PlateNumber,
                                T.UserID,
                                T.City_code,
                                T.Engineno,
                                T.Classno,
                                T.Status,
                                T.CreateTime  
                                FROM Tuhu_log..PeccancyLog AS T WITH(NOLOCK) WHERE PKID<={maxId}
                                ORDER BY T.PKID ASC
                                OFFSET {pageSize * (pageIndex - 1)} ROWS
                                FETCH NEXT {pageSize} ROWS ONLY; ";
                using (var cmd = helper.CreateCommand(sql))
                {
                    return helper.ExecuteSelect<PeccancyLog>(cmd);
                }
            }
        }
        private int GetPeccancyLogMaxPkid()
        {
            DataSet ds = new DataSet();
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                string sql = $@"SELECT MAX(PKID) FROM Tuhu_log..PeccancyLog WITH(nolock)";
                using (var cmd = helper.CreateCommand(sql))
                {
                    return int.Parse(helper.ExecuteScalar(cmd).ToString());
                }
            }
        }

        private int GetPeccancyLogCount(int maxId)
        {
            DataSet ds = new DataSet();
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                string sql = $@"SELECT
                                count(1) 
                                FROM Tuhu_log..PeccancyLog AS T WITH(NOLOCK) WHERE PKID<={maxId}";
                using (var cmd = helper.CreateCommand(sql))
                {
                    return (int)helper.ExecuteScalar(cmd);
                }
            }
        }


        /// <summary>
        /// 获取 自动查违章提醒
        /// </summary>
        /// <returns></returns>
        private VehicleTypeCertificationInfo GetVehicleTypeCertificationInfo(Guid CarID)
        {
            DataSet ds = new DataSet();
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                string sql = $@"SELECT Top(1)
                                T.IsAutoLicenseSearch ,
                                T.CarID
                                FROM Tuhu_profiles..VehicleTypeCertificationInfo AS T WITH(NOLOCK)
                                where CarID = '{CarID}'
                                ; ";

                using (var cmd = helper.CreateCommand(sql))
                {
                    return helper.ExecuteSelect<VehicleTypeCertificationInfo>(cmd).FirstOrDefault();
                }
            }
        }


        #region Model

        class PeccancyLog
        {
            public long PKID { get; set; }
            /// <summary>车牌号</summary>
            public string PlateNumber { get; set; }
            /// <summary>昵称</summary>
            public Guid UserID { get; set; }
            /// <summary>车型描述</summary>
            public string City_code { get; set; }
            public string Engineno { get; set; }
            /// <summary> 购车年份</summary>
            public string Classno { get; set; }
            /// <summary>状态</summary>
            public int Status { get; set; }
            /// <summary>创建时间</summary>
            public DateTime CreateTime { get; set; }
        }

        class PushHistory
        {
            public string PlateNumber { get; set; }
            public Guid UserID { get; set; }
        }


        class CarDetail
        {
            public Guid UserId { get; set; }
            public Guid CarId { get; set; }
            public string CarNumber { get; set; }

            public string VinCode { get; set; }
            public string EngineNo { get; set; }

            public string Carno_Province { get; set; }
            public string Carno_City { get; set; }

        }

        public class CarObject
        {
            public string u_carno { get; set; }
            public Guid UserID { get; set; }
            public Guid CarId { get; set; }
            public string VinCode { get; set; }
            public string Engineno { get; set; }
            public string Carno_Province { get; set; }
            public string Carno_City { get; set; }
        }

        public class VehicleTypeCertificationInfo
        {
            public Guid CarId { get; set; }
            public int Status { get; set; }

            public int IsAutoLicenseSearch { get; set; }

        }

        class QueryResult
        {
            public bool Success { get; set; }
            public int ErrorCode { get; set; }
            public string ErrMessage { get; set; }
            /// <summary>
            /// 是否有违章
            /// </summary>
            public bool HasData { get; set; }
        }

        public class Province
        {
            public Province()
            {
            }

            public int ProvinceID { get; set; }
            public string ProvinceName { get; set; }
            public string ProvincePrefix { get; set; }
            public IEnumerable<City> Cities { get; set; }
        }

        public class City
        {
            public City()
            {
            }

            public int CarCodeLen { get; set; }
            public int CarEngineLen { get; set; }
            public string CarNumberPrefix { get; set; }
            public int CarOwnerLen { get; set; }
            public int CityID { get; set; }
            public string CityName { get; set; }
            public string Name { get; set; }
            public int ProxyEnable { get; set; }
        }

        #endregion


    }

}
