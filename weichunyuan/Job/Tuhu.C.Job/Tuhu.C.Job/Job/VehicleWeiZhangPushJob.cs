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
    public class VehicleWeiZhangPushJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(VehicleWeiZhangPushJob));

        private static readonly HttpClient CheXingYiClient = new HttpClient
        {
            BaseAddress = new Uri(ConfigurationManager.AppSettings["CheXingYiHost"])
        };

        private static readonly string Env = ConfigurationManager.AppSettings["Env"];
        private static readonly Dictionary<string, Tuple<string, string>> UserPassword =
            new Dictionary<string, Tuple<string, string>>()
            {
                ["dev"] = Tuple.Create("tuhuychtest", "QrnT0QDcf5wsNhNmHufEgw=="),
                ["pro"] = Tuple.Create("tuhu2016", "Ktbzuy3fKl+DmEJWSSR3Mw=="),
            };
        private static IEnumerable<Province> Provinces = null;
        private static readonly int WeiZhang = 845;//违章查询的模板id
        private static readonly int NoWeiZhang = 842;//没有违章的模板id
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Provinces = GetProvinceCache();

                Logger.Info("VehicleWeiZhangPushJob 开始执行");
                int pageIndex = 1;
                int pageSize = 100;
                //每次查询一百个人
                while (true)
                {
                    var data = GetPushUsers(pageIndex, pageSize);
                    if (data == null || !data.Any()) break;

                    Send(data);

                    pageIndex++;
                    System.Threading.Thread.Sleep(100);
                }

                Logger.Info("VehicleWeiZhangPushJob 执行结束");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
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
                WriteLog(item, data);
                if (!string.IsNullOrEmpty(data.ErrMessage))
                {
                    Logger.Error(data.ErrMessage);
                }
                else
                {
                    return data;
                }

            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }

            return null;
        }

        private void WriteLog(CarDetail item, QueryResult result)
        {
            using (var helper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = helper.CreateCommand(@"INSERT Tuhu_log..PeccancyLog
        ( PlateNumber ,
          UserID ,
          City_code ,
          Engineno ,
          Classno ,
          Status ,
          CreateTime ,
          Channel
        )
        VALUES(
          @plateNumber,
          @userId,
          @city_code,
          @engineno,
          @classno,
          @status,
          GETDATE(),
          'APP'
        )"))
                {
                    cmd.Parameters.Add(new SqlParameter("@plateNumber", item.CarNumber));
                    cmd.Parameters.Add(new SqlParameter("@userId", item.UserId));
                    cmd.Parameters.Add(new SqlParameter("@city_code", item.SearchCity));//车牌所在城市暂时存 查询城市
                    cmd.Parameters.Add(new SqlParameter("@engineno", item.EngineNo));
                    cmd.Parameters.Add(new SqlParameter("@classno", item.VinCode));
                    cmd.Parameters.Add(new SqlParameter("@status", result?.ErrorCode));
                    helper.ExecuteNonQuery(cmd);
                }
            }
        }

        private void Send(IEnumerable<CarDetail> data)
        {
            using (var client = new Service.Push.TemplatePushClient())
            {
                foreach (var item in data)
                {
                    var result = GetResponse(item);
                    if (result != null)
                    {
                        if (result.HasData)
                        {
                            Notification(item, WeiZhang, client);
                        }
                        else
                        {
                            Notification(item, NoWeiZhang, client);
                        }

                    }
                }
            }
        }

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

            var response = client.PushByUserIDAndBatchID(new List<string> { item.UserId.ToString() }, templateId,
                new Service.Push.Models.Push.PushTemplateLog()
                {
                    Replacement = replacement
                });
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

        /// <summary>
        /// 获取需要违章提醒的人（开启了违章提醒）
        /// </summary>
        /// <returns></returns>
        private IEnumerable<CarDetail> GetPushUsers(int pageIndex, int pageSize)
        {
            DataSet ds = new DataSet();
            using (var helper = DbHelper.CreateDbHelper(true))
            {
                using (var cmd = helper.CreateCommand($@"SELECT
                                                        VTCI.Vehicle_license_img AS ImageUrl, 
                                                        VTCI.Status,
                                                        VTCI.LastUpdateDateTime AS LastChangedDate,
                                                        VTCI.CreateDateTime AS CreatedDate,
                                                        VTCI.certified_time,
                                                        VTCI.Channel,
                                                        VTCI.SearchCity,
                                                        CO.UserId,
                                                        CO.u_carno AS CarNumber,
                                                            CO.CarID,
                                                        CO.Brand,
                                                        CO.SalesName AS CarName,
                                                        CO.Brand AS CarBrand,
                                                        CO.Engineno,
                                                        CO.u_PaiLiang,
                                                        CO.u_Nian,
                                                        CO.SalesName,
                                                        CO.Classno AS VinCode,
                                                        CO.TID,
                                                        CO.Classno,
                                                        CO.Vehicle AS VehicleName,
                                                        CO.Carno_City,
                                                        CO.Carno_Province,
                                                        CO.IsDefaultCar
                                                        FROM tuhu_profiles..VehicleTypeCertificationInfo AS VTCI WITH(NOLOCK)
                                                        INNER JOIN Tuhu_profiles..CarObject AS CO WITH(NOLOCK) ON VTCI.CarID = CO.CarID
                                                        WHERE VTCI.IsAutoLicenseSearch=1 AND CO.IsDeleted=0
                                                        ORDER BY VTCI.PKID ASC
                                                        OFFSET {pageSize * (pageIndex - 1)} ROWS
                                                        FETCH NEXT {pageSize} ROWS ONLY; "))
                {
                    return helper.ExecuteSelect<CarDetail>(cmd);
                }
            }
        }

        class CarDetail
        {
            public CarDetail()
            {

            }

            /// <summary>车型编号</summary>
            public Guid CarId { get; set; }
            /// <summary>会员编号</summary>
            public Guid UserId { get; set; }
            /// <summary>昵称</summary>
            public string Nickname { get; set; }
            /// <summary>车型描述</summary>
            public string CartypeDescription { get; set; }
            public string CartypeOther { get; set; }
            /// <summary> 购车年份</summary>
            public string BuyYear { get; set; }
            /// <summary>购车月份</summary>
            public string BuyMonth { get; set; }
            /// <summary>认证成功的时间</summary>
            public DateTime certified_time { get; set; }
            /// <summary>创建记录时间</summary>
            public DateTime CreatedDate { get; set; }
            /// <summary>最后修改记录时间</summary>
            public DateTime LastChangedDate { get; set; }
            /// <summary>是否默认车型</summary>
            public bool IsDefaultCar { get; set; }
            /// <summary>VIN码</summary>
            public string VinCode { get; set; }
            /// <summary>爱车的当前行驶里程 </summary>
            public int TotalMileage { get; set; }
            /// <summary>最近一次保养里程 </summary>
            public int LastBaoYangKm { get; set; }
            /// <summary>最近一次保养时间</summary>
            public DateTime LastBaoYangTime { get; set; }
            /// <summary>新手上路时间/// </summary>
            ///
            ///             BuyYear+BuyMonth
            public string OnRoadMonth { get; set; }
            /// <summary>全部轮胎尺寸/// </summary>
            public string TireSize { get; set; }
            /// <summary>标准尺寸/// </summary>
            public string StandardTireSize { get; }
            /// <summary>特殊轮胎尺寸/// </summary>
            public string SpecialTireSize { get; }
            /// <summary>单个轮胎尺寸/// </summary>
            public string TireSizeForSingle { get; set; }
            /// <summary> 修改TotalMileage的时间</summary>
            public DateTime? OdometerUpdatedTime { get; set; }
            /// <summary> 车牌号 </summary>
            public string CarNumber { get; set; }
            /// <summary>车牌所在的省份</summary>
            public string CarNoProvince { get; set; }
            /// <summary>车牌所在的城市</summary>
            public string CarNoCity { get; set; }
            /// <summary>车架号 </summary>
            public string CarFrameNo { get; set; }
            /// <summary>发动机号 </summary>
            public string EngineNo { get; set; }
            /// <summary>用户备注</summary>
            public string UserRemark { get; set; }
            /// <summary>五级属性集合</summary>
            public List<VehicleProperty> CarProperties { get; set; }
            /// <summary>保险到期时间</summary>
            public DateTime? InsureExpireDate { get; set; }
            /// <summary>保险公司</summary>
            public string InsuranceCompany { get; set; }
            /// <summary>车主姓名</summary>
            public string OwnerName { get; set; }
            /// <summary>车主身份证</summary>
            public string OwnerIdentityId { get; set; }
            /// <summary>投保城市</summary>
            public string InsuranceCity { get; set; }
            /// <summary>投保城市编码</summary>
            public string InsuranceRegionCode { get; set; }
            /// <summary>投保城市全称</summary>
            public string InsuranceRegionName { get; set; }
            /// <summary>一年内是否过户</summary>
            public bool IsTransferInOneYear { get; set; }
            /// <summary>注册时间</summary>
            public DateTime? Registrationtime { get; set; }
            /// <summary>发证日期</summary>
            public DateTime? IssueDate { get; set; }
            /// <summary>是否可保养</summary>
            public bool IsBaoyang { get; set; }
            /// <summary>地址</summary>
            public string Adress { get; set; }
            /// <summary>照片</summary>
            public string ImageUrl { get; set; }
            /// <summary>用户手机号/用来Setting查询呈现用</summary>
            public string Mobile { get; set; }
            /// <summary>认证渠道</summary>
            public string Channel { get; set; }
            /// <summary>使用性质</summary>
            public string UseProperty { get; set; }
            /// <summary>来源</summary>
            public string Source { get; set; }
            /// <summary>车型认证状态-1:未认证，0审核中，1已审核，2，未审核</summary>
            public int? Status { get; set; }

            public string CarName { get; set; }
            public string CarBrand { get; set; }
            public string CarPlace { get; set; }
            public string Carno_City { get; set; }
            public string Carno_Province { get; set; }
            /// <summary>
            /// 认证状态
            /// </summary>
            public int CertificationStatus { get; set; }
            public string SearchCity { get; set; }
            public string Classno { get; set; }
            public string Engineno { get; set; }
            public bool IsDelete { get; set; } = false;
            public string VehicleName { get; set; }

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
    }

}
