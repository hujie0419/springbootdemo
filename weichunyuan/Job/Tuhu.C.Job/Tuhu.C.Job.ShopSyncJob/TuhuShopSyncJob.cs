using Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Tuhu.Service.Shop.Models;
using Tuhu.Service.Shop.Models.Result;
using Tuhu.Service.ThirdParty;
using Tuhu.C.Job.ShopSyncJob.BLL;
using Tuhu.C.Job.ShopSyncJob.DAL;
using Tuhu.C.Job.ShopSyncJob.Models;

namespace Tuhu.C.Job.ShopSyncJob
{
    [DisallowConcurrentExecution]
    public class TuhuShopSyncJob : IJob
    {

        Dictionary<string, string> districtCodeDic = new Dictionary<string, string>();
        Dictionary<string, string> specificDistrictDic = new Dictionary<string, string>();

        List<int> successfulShopList = new List<int>();
        Dictionary<int, string> failedShopIdDic = new Dictionary<int, string>();
        public static readonly ILog Logger = LogManager.GetLogger(typeof(TuhuShopSyncJob));
        public static string GetBase64Image(string url, out bool issuccess)
        {
            issuccess = true;
            string resultstr = string.Empty;
            var webclient = new System.Net.WebClient();
            try
            {

                byte[] res = webclient.DownloadData(url);
                resultstr = Convert.ToBase64String(res);

            }
            catch (Exception ex)
            {
                issuccess = false;
                string imageurl = "https://img1.tuhu.org/Home/Image/1542A950674B44BD199996EEE823D31E.png@100w_100h_100Q.jpg";
                byte[] res = webclient.DownloadData(imageurl);
                resultstr = Convert.ToBase64String(res);
            }
            return resultstr;
        }

        void compress(Image img, string path)
        {
            EncoderParameter qualityParam =
                new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60);
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            img.Save(path, jpegCodec, encoderParams);
        }
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        public void ManualInsertData()
        {
            Dictionary<object, object> operationDic = new Dictionary<object, object>();
            operationDic.Add("operation_type", "MODIFY");
            operationDic.Add("out_product_id", "9256|FU-CARWASHING-XICHE|2");
            var shopProductObj = new ShopProduct()
            {
                out_shop_id = "9256",
                service_category_id = 4,
                product_name = "标准洗车-5座",

                product_desc = "标准洗车-5座",
                off_price = 11,
                orig_price = 11,
                status = "1"
            };
            operationDic.Add("shop_product", shopProductObj);
            var addresultstr = PostMethodResult(
                "alipay.eco.mycar.maintain.serviceproduct.update", operationDic);

        }

        public string PostMethodResult(string method, Dictionary<object, object> param)
        {
            // string gatewayUrl=
            Dictionary<string, string> paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("biz_content", JsonConvert.SerializeObject(param));
            paramDictionary.Add("app_id", ConfigurationManager.AppSettings["AliPayAppid"]);
            paramDictionary.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            paramDictionary.Add("version", "1.0");
            paramDictionary.Add("method", method);
            paramDictionary.Add("sign_type", "RSA");
            paramDictionary.Add("charset", "UTF-8");
            string signdata = RSACryptoServiceHelper.SignData(paramDictionary);
            paramDictionary.Add("sign", signdata);
            string resultstr = WebUtils.PostData(ConfigurationManager.AppSettings["AliPayGatewayUrl"], paramDictionary);
            return resultstr;
        }

        public TuhuShopSyncJob()
        {

            districtCodeDic = GetDistrictCodeDic();
            specificDistrictDic.Add("310000", "310100");
            specificDistrictDic.Add("110000", "110100");
            specificDistrictDic.Add("120000", "120100");
            specificDistrictDic.Add("500000", "500100");
        }

        public void Execute(IJobExecutionContext context)
        {
         
            Logger.Info("SyncTuhuShop启动");
            try
            {

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                bool result = SyncTuhuShop();
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                if (result)
                    Logger.Info($"SyncTuhuShop执行成功");
                else
                    Logger.Info(string.Format($"SyncTuhuShop执行失败,失败Id：{0}", string.Join(",", failedShopIdDic.Keys.Select(q => q).ToArray())));
                Logger.Info(string.Format("耗时：{0}s", ts.TotalSeconds));
            }
            catch (Exception ex)
            {
                Logger.Error($"SyncTuhuShop异常：{ex.ToString()}");
            }
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
        public bool SyncTuhuShop()
        {

            bool flag = true;
            List<ShopDetailModel> shopDetailModels = ShopBusiness.GetShopsByShopIds();
            if (!shopDetailModels.Any())
            {
                shopDetailModels = ShopBusiness.GetShopsForBaoyangAndXiChe();
            }
            if (!shopDetailModels.Any())
            {
                Logger.Info("SelectShopDetailsByRegionId出错");
                SendMail("SynTuhuShopInAliPay", ConfigurationManager.AppSettings["SEOFileJob:To"], "无门店需要同步,出错需查看");
                return false;
            }
            Logger.Info(string.Format("有{0}个门店需要同步处理", shopDetailModels.Count()));

            
            IEnumerable<int> validshopids = ShopsDAL.GetValidThirdPartyShopIds();
            IEnumerable<int> shopids = ShopsDAL.GetAllThirdPartyShopIds();
            List<int> shopDetailIds = shopDetailModels.Where(q => q != null).Select(q => q.ShopId).OrderBy(q => q).ToList();


            List<ShopDetailModel> toaddShops =
             shopDetailModels.Where(q => !validshopids.Contains(q.ShopId)).OrderBy(q => q.ShopId).ToList();
         
            List<int> todeleteShopIds = new List<int>();
            if (string.Equals(ConfigurationManager.AppSettings["InputGlobal"], "true", StringComparison.CurrentCultureIgnoreCase))
            {
                todeleteShopIds = shopids.ToList().Where(q => !shopDetailIds.Contains(q)).ToList();
            }



            try
            {
                using (var alipayclient = new AliPayServiceClient())
                {
                    #region Add

                    if (toaddShops.Any())
                    {
                        var toaddshopidlist = toaddShops.Select(p => p.ShopId).ToList();
                        foreach (var shopid in toaddshopidlist)
                        {

                            if (ShopIsExistedInAliPay(shopid))
                            {

                                alipayclient.UpdateShopsFlagToCheZhuPlatform(new List<int> { shopid }, null, true);
                            }
                            else
                            {
                                Logger.Info($"车主平台不存在门店{shopid},需导入");
                                alipayclient.InputShopsToCheZhuPlatform(new List<int> { shopid }, null);
                            }
                        }
                    }
                    #endregion

                    #region delete


                    if (todeleteShopIds.Any())
                    {
                        foreach (var shopId in todeleteShopIds)
                        {
                            if (ShopIsExistedInAliPay(shopId))
                            {
                                Logger.Info($"车主平台存在门店{shopId},需修改下架状态");
                                alipayclient.UpdateShopsFlagToCheZhuPlatform(new List<int> { shopId }, null, false);
                            }
                            else
                            {
                                Logger.Info($"车主平台不存在门店{shopId},无需处理此门店");
                            }
                        }
                    }
                    #endregion
                    //更新数据
                    #region update
                    foreach (var shopId in shopDetailIds)
                    {
                        if (ShopIsExistedInAliPay(shopId))
                        {
                            Logger.Info($"车主平台存在门店{shopId},需更新");
                            alipayclient.DeleteShopsFromCheZhuPlatform(new List<int>() { shopId }, null);
                            alipayclient.InputShopsToCheZhuPlatform(new List<int>() { shopId }, null);
                            alipayclient.InputShopServicesToCheZhuPlatform(new List<int>() { shopId }, null);
                        }
                        else
                        {
                            Logger.Info($"TuhuShopSyncJob 途虎存在却车主平台不存在的门店{shopId}");
                        }

                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                Logger.Error("TuhuShopSyncJob error",ex);
            }

            return flag;
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




        public Dictionary<string, string> GetDistrictCodeDic()
        {
            Dictionary<string, string> districtDic = new Dictionary<string, string>();
            string[] txtData = File.ReadAllLines("DistrictCode.txt", System.Text.Encoding.Default);
            if (txtData.Any())
            {
                foreach (var item in txtData.Where(q => q != null))
                {
                    // char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                    string[] arr = Regex.Split(item, @"\s+");
                    if (!string.IsNullOrWhiteSpace(arr.First()) && !string.IsNullOrWhiteSpace(arr.Last()))
                    {
                        if (!districtDic.ContainsKey(arr.Last()))
                        {
                            districtDic.Add(arr.Last(), arr.First());
                        }
                    }
                }
            }
            return districtDic;
        }

        public static List<int> GetShopIds()
        {
            List<int> shopids = new List<int>();
            string[] txtData = File.ReadAllLines("shopid.txt", System.Text.Encoding.Default);
            if (txtData.Any())
            {
                foreach (var item in txtData.Where(q => q != null))
                {
                    shopids.Add(Convert.ToInt32(item));
                }
            }
            return shopids;
        }

        public void deleteShopDetails()
        {
            List<int> shopids = new List<int>() { 18286, 17314 };

            //if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ShopIdList"]))
            //{
            //    shopids = ConfigurationManager.AppSettings["ShopIdList"].Split(new char[] { ',' }).Select(q => Convert.ToInt32(q)).ToList();
            //}

            int failedshopid = 0;


            using (StreamWriter stream = new StreamWriter(@"E:\log5.txt"))
            {

                try
                {
                    foreach (var shopid in shopids)
                    {
                        Dictionary<object, object> deleteDic = new Dictionary<object, object>();
                        failedshopid = shopid;
                        deleteDic.Add("out_shop_id", shopid.ToString());
                        // updateDic.Add("status", "0");
                        var resultstr = PostMethodResult("alipay.eco.mycar.maintain.shop.delete", deleteDic);
                        JObject modifyresult = JsonConvert.DeserializeObject(resultstr) as JObject;
                        if (modifyresult != null &&
                            modifyresult["alipay_eco_mycar_maintain_shop_delete_response"] != null &&
                            modifyresult["alipay_eco_mycar_maintain_shop_delete_response"]["code"].ToString() == "10000")
                        {
                            //Logger.Info(String.Format("门店修改成功，门店ID：{0}", shopid));
                            stream.WriteLine(String.Format("门店删除成功，门店ID：{0}", shopid));
                        }
                        else
                        {
                            stream.WriteLine(String.Format("门店删除失败，门店ID：{0},{1}", shopid, modifyresult["alipay_eco_mycar_maintain_shop_delete_response"]["sub_msg"]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    stream.WriteLine(String.Format("异常门店ID：{0},异常信息：{1}", failedshopid, ex.Message));
                }


            }
        }

        public void UpdateShopDetails()
        {
            List<int> shopids = new List<int>() { 21825 };
            //  shopids = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\FactoryShopIdList.txt").Select(s => Convert.ToInt32(s)).ToList();
            // shopids =new List<int>{16979,17796,3117,12802,1919,1646,11656,17918,11495,13329};
            shopids = shopids.OrderBy(q => q).ToList();
            int failedshopid = 0;


            using (StreamWriter stream = new StreamWriter(@"E:\log9.txt"))
            {

                try
                {

                    foreach (var shopid in shopids)
                    {
                        Dictionary<object, object> queryDictionary = new Dictionary<object, object>();
                        queryDictionary.Add("out_shop_id", shopid.ToString());

                        var queryresultstr = PostMethodResult(
                            "alipay.eco.mycar.maintain.shop.query", queryDictionary);

                        JObject queryresult = JsonConvert.DeserializeObject(queryresultstr) as JObject;
                        if (queryresult != null && queryresult["alipay_eco_mycar_maintain_shop_query_response"] != null &&
                            queryresult["alipay_eco_mycar_maintain_shop_query_response"]["code"].ToString() == "10000")
                        {
                            stream.WriteLine(shopid);
                            stream.WriteLine(queryresult["alipay_eco_mycar_maintain_shop_query_response"]["shop_id"]);
                        }


                        Dictionary<object, object> updateDic = new Dictionary<object, object>();
                        failedshopid = shopid;
                        updateDic.Add("out_shop_id", shopid.ToString());
                        //updateDic.Add("province_code", "310000");                        
                        // updateDic.Add("status", "0");
                        updateDic.Add("open_time", "08:00");
                        updateDic.Add("close_time", "23:59");
                        var resultstr = PostMethodResult("alipay.eco.mycar.maintain.shop.modify", updateDic);
                        JObject modifyresult = JsonConvert.DeserializeObject(resultstr) as JObject;
                        if (modifyresult != null &&
                            modifyresult["alipay_eco_mycar_maintain_shop_modify_response"] != null &&
                            modifyresult["alipay_eco_mycar_maintain_shop_modify_response"]["code"].ToString() == "10000")
                        {
                            //Logger.Info(String.Format("门店修改成功，门店ID：{0}", shopid));
                            stream.WriteLine(String.Format("门店修改成功，门店ID：{0}", shopid));
                        }
                        else
                        {
                            stream.WriteLine(String.Format("门店修改失败，门店ID：{0},{1}", shopid, modifyresult["alipay_eco_mycar_maintain_shop_modify_response"]["sub_msg"]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    stream.WriteLine(String.Format("异常门店ID：{0},异常信息：{1}", failedshopid, ex.Message));
                }


            }

        }

        public string GetDistrictCode(ShopDetailModel shopDetail, string districtName)
        {
            string districtCode = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(districtName))
                {
                    districtName = shopDetail.Province;
                }

                if (!string.IsNullOrEmpty(districtName))
                {
                    if (!districtCodeDic.TryGetValue(districtName, out districtCode))
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(districtName, ex);
            }                      
            Logger.Info(string.Format("DistrictName:{0},DistrictCode:{1}", districtName, districtCode));
            return districtCode;
        }

        public string DealWithWorkTime(string worktime)
        {
            string formatWorktime = worktime;
            if (formatWorktime.Contains("24小时"))
            {
                formatWorktime = "00:00-23:59";
            }
            else
            {
                formatWorktime = formatWorktime.Normalize(NormalizationForm.FormKC);
                formatWorktime = Regex.Replace(formatWorktime, @"\s+", "");
            }
            return formatWorktime;
        }

        public Tuple<double, double> transformBaiduToMars(double longitude, double latitude)
        {

            double x_pi = 3.14159265358979324 * 3000.0 / 180.0;
            double x = longitude - 0.0065;
            double y = latitude - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);
            return new Tuple<double, double>(z * Math.Sin(theta), z * Math.Cos(theta));

            //   return CLLocationCoordinate2D(latitude: z* sin(theta), longitude: z* cos(theta))
        }


        public Dictionary<object, object> GetRequestParams(ShopDetailModel shopdetail, out bool result, bool insertPic = true)
        {
            bool issuccess = true;
            result = true;
            Dictionary<object, object> paramDic = new Dictionary<object, object>();
            try
            {
                if (insertPic)
                {
                    if (shopdetail.Images != null && shopdetail.Images.FirstOrDefault() != null)
                    {
                        string imagepath = shopdetail.Images[0];
                        string imgtype = imagepath.Split(new char[] { '.' })[1];
                        var baseimagepath = "https://img3.tuhu.org/Images/Marketing/Shops/" +
                                            imagepath + "@100w_100h_100Q.jpg";

                        Dictionary<object, object> picDic = new Dictionary<object, object>();
                        picDic.Add("img_content", GetBase64Image(baseimagepath,out issuccess));
                        picDic.Add("img_type", issuccess?imgtype:"png");
                        var picresultstr = PostMethodResult(
                            "alipay.eco.mycar.image.upload", picDic);
                        JObject picresult = JsonConvert.DeserializeObject(picresultstr) as JObject;
                        if (picresultstr != null && picresult["alipay_eco_mycar_image_upload_response"] != null &&
                            picresult["alipay_eco_mycar_image_upload_response"]["code"]?.ToString() == "10000")
                        {
                            paramDic.Add("main_image",
                                picresult["alipay_eco_mycar_image_upload_response"]["img_url"]);

                        }
                        else
                        {
                            result = false;
                            if (picresult["alipay_eco_mycar_image_upload_response"]["msg"] != null)
                            {
                                if (!failedShopIdDic.ContainsKey(shopdetail.ShopId))
                                {
                                    failedShopIdDic.Add(shopdetail.ShopId,
                                        picresult["alipay_eco_mycar_image_upload_response"]["msg"].ToString());
                                }
                            }
                        }
                    }
                }
                List<string> industry_category_id = new List<string>();
                List<string> industry_app_category_id = new List<string>();
                if ((shopdetail.ServiceType & 4) == 4)
                {
                    industry_category_id.Add("2016062900190125");
                    industry_app_category_id.Add("15");
                }
                if ((shopdetail.ServiceType & 2) == 2)
                {
                    industry_category_id.Add("2016062900190217");
                    industry_app_category_id.Add("16");
                }
                if (!industry_app_category_id.Any())
                {
                    result = false;
                }

                paramDic.Add("out_shop_id", shopdetail.ShopId.ToString());
                paramDic.Add("shop_name", shopdetail.SimpleName);
                Logger.Info(shopdetail.Province);
                // string provinceCode=GetDistrictCode(shopdetail, shopdetail.Province);
                string provinceCode = GetDistrictCode(shopdetail, shopdetail.Province);
                paramDic.Add("province_code", provinceCode);
                string city_code = "310100";
                if (specificDistrictDic.TryGetValue(provinceCode, out city_code))
                {
                    paramDic.Add("city_code", city_code);
                    paramDic.Add("district_code", city_code);
                    Logger.Info(string.Format("门店:{0},CityCode:{1}", shopdetail.SimpleName, city_code));
                }
                else
                {
                    string citycode = GetDistrictCode(shopdetail, shopdetail.City);
                    paramDic.Add("city_code", citycode);
                    paramDic.Add("district_code", citycode);
                    Logger.Info(string.Format("门店:{0},CityCode:{1}", shopdetail.SimpleName, citycode));
                }
                paramDic.Add("address", shopdetail.AddressBrief);
                var formatworktime = DealWithWorkTime(shopdetail.WorkTime);


                DateTime specificstarttime = DateTime.Parse("08:00");
                DateTime specificendtime = DateTime.Parse("18:00");
                DateTime starttime = DateTime.Parse("08:00");
                DateTime endtime = DateTime.Parse("18:00");

                if (formatworktime.Split(new char[] { '-', '—', '~' }).Count() >= 2)
                {
                    var tempendtime = Regex.Replace(formatworktime.Split(new char[] { '-', '—', '~' })[1], @"24:(.*)$", "23:59");
                    if (DateTime.TryParse(formatworktime.Split(new char[] { '-', '—', '~' })[0], out starttime))
                    {
                        paramDic.Add("open_time", string.Format("{0:HH:mm}", starttime));
                    }
                    else
                    {
                        paramDic.Add("open_time", string.Format("{0:HH:mm}", specificstarttime));
                    }
                    if (DateTime.TryParse(tempendtime, out endtime))
                    {
                        paramDic.Add("close_time", string.Format("{0:HH:mm}", endtime));
                    }
                    else
                    {
                        paramDic.Add("close_time", string.Format("{0:HH:mm}", specificendtime));
                    }
                }
                else
                {
                    paramDic.Add("open_time", string.Format("{0:HH:mm}", specificstarttime));
                    paramDic.Add("close_time", string.Format("{0:HH:mm}", specificendtime));
                }


                paramDic.Add("status", "1");
                Tuple<double, double> coordinate = transformBaiduToMars(
                               Convert.ToDouble(shopdetail.Position[0]), Convert.ToDouble(shopdetail.Position[1]));
                paramDic.Add("lon",
                    coordinate.Item1.ToString().Length > 14
                        ? coordinate.Item1.ToString().Substring(0, 15)
                        : coordinate.Item1.ToString());
                paramDic.Add("lat",
                    coordinate.Item2.ToString().Length > 14
                        ? coordinate.Item2.ToString().Substring(0, 15)
                        : coordinate.Item2.ToString());

                paramDic.Add("shop_type", GetShopClassification(shopdetail.ShopClassification));
                paramDic.Add("shop_tel", "4001118868");
                paramDic.Add("industry_app_category_id", industry_app_category_id);
                paramDic.Add("industry_category_id", industry_category_id);

            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error(ex);

            }

            return paramDic;
        }

        public bool TestImage(string imagepath)
        {
            bool flag = true;
            string imgtype = imagepath.Split(new char[] { '.' })[1];
            //var baseimagepath = "https://img0-tuhu-cn.alikunlun.com/Images/Marketing/Shops/" +
            //                    imagepath + "@100w_100h_100Q.jpg";


            var baseimagepath = "https://img2.tuhu.org/Images/Marketing/Shops/" +
                                imagepath + "@100w_100h_100Q.jpg";
            Dictionary<object, object> picDic = new Dictionary<object, object>();
            bool issuccess = true;
            picDic.Add("img_content", GetBase64Image(baseimagepath,out issuccess));
            picDic.Add("img_type", imgtype);
            var picresultstr = PostMethodResult(
                "alipay.eco.mycar.image.upload", picDic);
            JObject picresult = JsonConvert.DeserializeObject(picresultstr) as JObject;
            if (picresultstr != null && picresult["alipay_eco_mycar_image_upload_response"] != null &&
                picresult["alipay_eco_mycar_image_upload_response"]["code"].ToString() == "10000")
            {               
                flag = true;
            }
            return flag;
        }

        public string GetShopClassification(string type)
        {
            string shopClassification = "shop_type_maintenance";
            if (type.Contains("4S店"))
                shopClassification = "shop_type_4s";
            else if (type.Contains("快修店"))
            {
                shopClassification = "shop_type_repair";
            }

            return shopClassification;
        }

        public void SyncCarWashingService(List<int> shopidList)
        {
            if (shopidList != null && shopidList.Any())
            {
                foreach (var shopId in shopidList)
                {

                    bool shopIsExisted = ShopIsExisted(shopId);
                    if (!shopIsExisted)
                    {
                        return;
                    }
                    Dictionary<object, object> operationDic = new Dictionary<object, object>();
                    Dictionary<object, object> updateDic = new Dictionary<object, object>();
                    Dictionary<object, object> queryDictionary = new Dictionary<object, object>();
                    var shopservicemodelList = ShopBusiness.GetShopServiceModelListByShopId(shopId);


                    if (shopservicemodelList.Any())
                    {
                        foreach (var shopservicemodel in shopservicemodelList)
                        {
                            try
                            {
                                #region 查询更新

                                queryDictionary.Add("out_product_id", shopservicemodel.ShopId + "|" + shopservicemodel.ServiceId);
                                queryDictionary.Add("operation_type", "QUERY");
                                var queryresultstr = PostMethodResult(
                         "alipay.eco.mycar.maintain.serviceproduct.update", queryDictionary);

                                JObject queryresult = JsonConvert.DeserializeObject(queryresultstr) as JObject;
                                if (queryresult != null &&
                                    queryresult["alipay_eco_mycar_maintain_serviceproduct_update_response"] != null &&
                                    queryresult["alipay_eco_mycar_maintain_serviceproduct_update_response"]["code"]?
                                        .ToString() == "10000")
                                {

                                    //更新
                                    updateDic.Add("operation_type", "MODIFY");
                                    updateDic.Add("out_product_id",
                                        shopservicemodel.ShopId + "|" + shopservicemodel.ServiceId);
                                    var shopProductObj = new ShopProduct()
                                    {
                                        out_shop_id = shopservicemodel.ShopId.ToString(),
                                        service_category_id = GetXiCheType(shopservicemodel.ServiceId),
                                        product_name =
                                            shopservicemodel.ServersName ?? string.Empty,
                                        product_desc = string.IsNullOrWhiteSpace(shopservicemodel.ServiceRemark) ? shopservicemodel.ServersName : shopservicemodel.ServiceRemark,
                                        off_price = shopservicemodel.Price,
                                        orig_price = shopservicemodel.Price,
                                        status = "1"
                                    };

                                    updateDic.Add("shop_product", shopProductObj);
                                    var updateresultstr = PostMethodResult(
                                        "alipay.eco.mycar.maintain.serviceproduct.update", updateDic);
                                    JObject update_result = JsonConvert.DeserializeObject(updateresultstr) as JObject;
                                    if (update_result != null &&
                                        update_result["alipay_eco_mycar_maintain_serviceproduct_update_response"] != null &&
                                        update_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["code"]?
                                            .ToString() == "10000")
                                    {

                                        Logger.Info(String.Format("此产品:{0} 已更新到门店:{1}", shopservicemodel.ServiceId,
                                            shopservicemodel.ShopId));
                                    }
                                    else
                                    {

                                        Logger.Info(String.Format("此产品:{0} 未能更新到门店:{1},失败原因:{2}", shopservicemodel.ServiceId,
                                            shopservicemodel.ShopId,update_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["sub_msg"]?
                                            .ToString()));
                                    }

                                }
                                else
                                {

                                    #endregion
                                    #region 添加产品                                                                
                                    operationDic.Add("operation_type", "INSERT");
                                    operationDic.Add("out_product_id",
                                        shopservicemodel.ShopId + "|" + shopservicemodel.ServiceId);
                                    var shopProductObj = new ShopProduct()
                                    {
                                        out_shop_id = shopservicemodel.ShopId.ToString(),
                                        service_category_id = GetXiCheType(shopservicemodel.ServiceId),
                                        product_name =
                                            shopservicemodel.ServersName ?? string.Empty,
                                        // product_desc = "整车泡沫清洗",
                                        product_desc = string.IsNullOrWhiteSpace(shopservicemodel.ServiceRemark) ? shopservicemodel.ServersName : shopservicemodel.ServiceRemark,
                                        off_price = shopservicemodel.Price,
                                        orig_price = shopservicemodel.Price,
                                        status = "1"
                                    };

                                    operationDic.Add("shop_product", shopProductObj);
                                    var addresultstr = PostMethodResult(
                                        "alipay.eco.mycar.maintain.serviceproduct.update", operationDic);
                                    JObject add_result = JsonConvert.DeserializeObject(addresultstr) as JObject;
                                    if (add_result != null &&
                                        add_result["alipay_eco_mycar_maintain_serviceproduct_update_response"] != null &&
                                        add_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["code"]?
                                            .ToString() == "10000")
                                    {

                                        Logger.Info(String.Format("此产品:{0} 插入到门店:{1}", shopservicemodel.ServiceId,
                                            shopservicemodel.ShopId));
                                    }
                                    else
                                    {
                                        Logger.Info(String.Format("此产品:{0} 未能插入到门店:{1},失败原因:{2}", shopservicemodel.ServiceId,
                                            shopservicemodel.ShopId, add_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["sub_msg"]?.ToString()));                                        
                                    }

                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                Logger.InfoException(ex);
                            }

                            finally
                            {
                                operationDic.Clear();
                                updateDic.Clear();
                                queryDictionary.Clear();
                            }
                        }


                    }
                }



            }
        }

        public void SyncNoShanghaiAreaService(List<int> shopidList)
        {
            if (shopidList != null && shopidList.Any())
            {
                foreach (var shopId in shopidList)
                {
                    try
                    {

                        bool shopIsExisted = ShopIsExisted(shopId);
                        if (!shopIsExisted)
                        {
                            return;
                        }

                        Logger.Info($"更新在支付宝平台的途虎门店ID:{shopId}服务");
                        using (var alipayclient = new AliPayServiceClient())
                        {
                            alipayclient.DeleteShopsFromCheZhuPlatform(new List<int>() { shopId }, null);
                            alipayclient.InputShopsToCheZhuPlatform(new List<int>() { shopId }, null);
                            alipayclient.InputShopServicesToCheZhuPlatform(new List<int>() { shopId }, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"更新在支付宝平台的途虎门店ID:{shopId}服务出错,错误原因:{ex.Message}");
                    }
                }



            }
        }

        public void SyncMeiRongService(List<int> shopidList)
        {
            Logger.Info($"SyncMeiRongService有{shopidList.Count()}个门店需要同步");
            if (shopidList != null && shopidList.Any())
            {
                foreach (var shopId in shopidList)
                {
                    try
                    {

                        bool shopIsExisted = ShopIsExisted(shopId);
                        if (!shopIsExisted)
                        {
                            Logger.Info($"在支付宝平台不存在途虎门店ID:{shopId}");
                            continue;
                        }

                        Logger.Info($"更新在支付宝平台的途虎门店ID:{shopId}服务");
                       
                        #region new code 
                        
                        Dictionary<object, object> operationDic = new Dictionary<object, object>();
                        Dictionary<object, object> updateDic = new Dictionary<object, object>();
                        Dictionary<object, object> queryDictionary = new Dictionary<object, object>();
                        List<ShopBeautyProductResultModel> tuangouxicheList = ShopBusiness.GetTuanGouXiCheModel(shopId);


                        if (tuangouxicheList != null && tuangouxicheList.Any())
                        {
                            foreach (var model in tuangouxicheList)
                            {
                                try
                                {
                                    #region 查询更新

                                    queryDictionary.Add("out_product_id", shopId + "|" + model.PID);
                                    queryDictionary.Add("operation_type", "QUERY");
                                    var queryresultstr = PostMethodResult(
                             "alipay.eco.mycar.maintain.serviceproduct.update", queryDictionary);


                                    JObject queryresult = JsonConvert.DeserializeObject(queryresultstr) as JObject;
                                    if (queryresult != null &&
                                        queryresult["alipay_eco_mycar_maintain_serviceproduct_update_response"] != null &&
                                        queryresult["alipay_eco_mycar_maintain_serviceproduct_update_response"]["code"]?
                                            .ToString() == "10000")
                                    {

                                        //更新
                                        updateDic.Add("operation_type", "MODIFY");
                                        updateDic.Add("out_product_id",
                                            shopId + "|" + model.PID);
                                        var shopProductObj = new ShopProduct()
                                        {
                                            out_shop_id = shopId.ToString(),
                                            service_category_id = ShopBusiness.GetTuanGouXiCheType(model.PID),
                                            product_name =
                                                model.ProductName ?? string.Empty,
                                            product_desc = model.Description,
                                            off_price = model.DefaultPrice,
                                            orig_price = model.DefaultPrice,
                                            status = "1"
                                        };

                                        updateDic.Add("shop_product", shopProductObj);
                                        var updateresultstr = PostMethodResult(
                                            "alipay.eco.mycar.maintain.serviceproduct.update", updateDic);
                                        JObject update_result = JsonConvert.DeserializeObject(updateresultstr) as JObject;
                                        if (update_result != null &&
                                            update_result["alipay_eco_mycar_maintain_serviceproduct_update_response"] != null &&
                                            update_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["code"]?
                                                .ToString() == "10000")
                                        {

                                            Logger.Info(String.Format("此产品:{0} 已更新到门店:{1}", model.PID,
                                                shopId));
                                        }
                                        else
                                        {                                       
                                            Logger.Info(String.Format("此产品:{0} 未能更新到门店:{1},失败原因:{2}", model.PID,
                                                shopId, update_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["sub_msg"]?
                                                 .ToString()));
                                        }

                                    }
                                    else
                                    {

                                        #endregion
                                        #region 添加产品                                                                
                                        operationDic.Add("operation_type", "INSERT");
                                        operationDic.Add("out_product_id",
                                            shopId + "|" + model.PID);
                                        var shopProductObj = new ShopProduct()
                                        {
                                            out_shop_id = shopId.ToString(),
                                            service_category_id = ShopBusiness.GetTuanGouXiCheType(model.PID),
                                            product_name =
                                                model.ProductName ?? string.Empty,
                                            product_desc = model.Description,
                                            off_price = model.DefaultPrice,
                                            orig_price = model.DefaultPrice,
                                            status = "1"
                                        };

                                        operationDic.Add("shop_product", shopProductObj);
                                        var addresultstr = PostMethodResult(
                                            "alipay.eco.mycar.maintain.serviceproduct.update", operationDic);
                                        JObject add_result = JsonConvert.DeserializeObject(addresultstr) as JObject;
                                        if (add_result != null &&
                                            add_result["alipay_eco_mycar_maintain_serviceproduct_update_response"] != null &&
                                            add_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["code"]?
                                                .ToString() == "10000")
                                        {

                                            Logger.Info(String.Format("此产品:{0} 插入到门店:{1}", model.PID,
                                                shopId));
                                        }
                                        else
                                        {

                                            Logger.Info(String.Format("此产品:{0} 未能插入到门店:{1},失败原因:{2}", model.PID,
                                                shopId, add_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["sub_msg"]?.ToString()));
                                        }

                                    }
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    Logger.InfoException(ex);
                                }

                                finally
                                {
                                    operationDic.Clear();
                                    updateDic.Clear();
                                    queryDictionary.Clear();
                                }
                            }


                        }
                       
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"更新在支付宝平台的途虎门店ID:{shopId}服务出错,错误原因:{ex.Message}");
                    }
                }



            }
        }

        public  bool ShopIsExisted(int shopId)
        {
            bool isExisted = false;
            Dictionary<object, object> queryDictionary = new Dictionary<object, object>();
            queryDictionary.Add("out_shop_id", shopId.ToString());

            var queryresultstr = PostMethodResult(
                "alipay.eco.mycar.maintain.shop.query", queryDictionary);

            JObject queryresult = JsonConvert.DeserializeObject(queryresultstr) as JObject;
            if (queryresult != null && queryresult["alipay_eco_mycar_maintain_shop_query_response"] != null &&
                queryresult["alipay_eco_mycar_maintain_shop_query_response"]["code"].ToString() == "10000" && queryresult["alipay_eco_mycar_maintain_shop_query_response"]["status"].ToString()=="1")
            {
                isExisted = true;
            }            
            return isExisted;
        }

        public void SyncXBYService(List<int> shopidList)
        {
            if (shopidList != null && shopidList.Any())
            {
                foreach (var shopId in shopidList)
                {
                    bool shopIsExisted = ShopIsExisted(shopId);
                    if (!shopIsExisted)
                    {
                        Logger.Info($"在支付宝平台不存在途虎门店ID:{shopId}");
                        continue;
                    }

                    Dictionary<object, object> operationDic = new Dictionary<object, object>();
                    Dictionary<object, object> updateDic = new Dictionary<object, object>();
                    Dictionary<object, object> queryDictionary = new Dictionary<object, object>();
                    var shopservicemodelList = ShopBusiness.GetXBYShopServiceModelListByShopId(shopId);


                    if (shopservicemodelList.Any())
                    {
                        foreach (var shopservicemodel in shopservicemodelList)
                        {
                            try
                            {
                                #region 查询更新

                                queryDictionary.Add("out_product_id", shopservicemodel.ShopId + "|" + shopservicemodel.ServiceId);
                                queryDictionary.Add("operation_type", "QUERY");
                                var queryresultstr = PostMethodResult(
                         "alipay.eco.mycar.maintain.serviceproduct.update", queryDictionary);

                                JObject queryresult = JsonConvert.DeserializeObject(queryresultstr) as JObject;
                                if (queryresult != null &&
                                    queryresult["alipay_eco_mycar_maintain_serviceproduct_update_response"] != null &&
                                    queryresult["alipay_eco_mycar_maintain_serviceproduct_update_response"]["code"]?
                                        .ToString() == "10000")
                                {
                                    //更新

                                    updateDic.Add("operation_type", "MODIFY");
                                    updateDic.Add("out_product_id",
                                        shopservicemodel.ShopId + "|" + shopservicemodel.ServiceId);
                                    var shopProductObj = new ShopProduct()
                                    {
                                        out_shop_id = shopservicemodel.ShopId.ToString(),
                                        service_category_id = 12,
                                        product_name =
                                            shopservicemodel.ServersName ?? string.Empty,
                                        product_desc = string.IsNullOrWhiteSpace(shopservicemodel.ServiceRemark) ? shopservicemodel.ServersName : shopservicemodel.ServiceRemark,
                                        off_price = shopservicemodel.Price,
                                        orig_price = shopservicemodel.Price,
                                        status = "1"
                                    };

                                    updateDic.Add("shop_product", shopProductObj);
                                    var updateresultstr = PostMethodResult(
                                        "alipay.eco.mycar.maintain.serviceproduct.update", updateDic);
                                    JObject update_result = JsonConvert.DeserializeObject(updateresultstr) as JObject;
                                    if (update_result != null &&
                                        update_result["alipay_eco_mycar_maintain_serviceproduct_update_response"] != null &&
                                        update_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["code"]?
                                            .ToString() == "10000")
                                    {
                                        Logger.Info(String.Format("此产品:{0} 更新到门店:{1}", shopservicemodel.ServiceId,
                                            shopservicemodel.ShopId));
                                    }
                                    else
                                    {
                                        Logger.Info(String.Format("此产品:{0} 未能更新到门店:{1},失败原因:{}", shopservicemodel.ServiceId,
                                            shopservicemodel.ShopId, update_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["sub_msg"]?
                                            .ToString()));
                                    }

                                }
                                else
                                {
                                    #endregion

                                    #region 添加产品
                                    operationDic.Add("operation_type", "INSERT");
                                    operationDic.Add("out_product_id",
                                        shopservicemodel.ShopId + "|" + shopservicemodel.ServiceId);
                                    var shopProductObj = new ShopProduct()
                                    {
                                        out_shop_id = shopservicemodel.ShopId.ToString(),
                                        service_category_id = 12,
                                        product_name =
                                            shopservicemodel.ServersName ?? string.Empty,
                                        product_desc = string.IsNullOrWhiteSpace(shopservicemodel.ServiceRemark) ? shopservicemodel.ServersName : shopservicemodel.ServiceRemark,
                                        off_price = shopservicemodel.Price,
                                        orig_price = shopservicemodel.Price,
                                        status = "1"
                                    };

                                    operationDic.Add("shop_product", shopProductObj);
                                    var addresultstr = PostMethodResult(
                                        "alipay.eco.mycar.maintain.serviceproduct.update", operationDic);
                                    JObject add_result = JsonConvert.DeserializeObject(addresultstr) as JObject;
                                    if (add_result != null &&
                                        add_result["alipay_eco_mycar_maintain_serviceproduct_update_response"] != null &&
                                        add_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["code"]?
                                            .ToString() == "10000")
                                    {
                                        Logger.Info(String.Format("此产品:{0} 插入到门店:{1}", shopservicemodel.ServiceId,
                                            shopservicemodel.ShopId));
                                    }
                                    else
                                    {
                                        Logger.Info(String.Format("此产品:{0} 未能插入到门店:{1},失败原因:{2}", shopservicemodel.ServiceId,
                                            shopservicemodel.ShopId, add_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["sub_msg"]?
                                            .ToString()));
                                    }

                                }
                                #endregion
                                #region

                                /*
                                 operationDic.Add("operation_type","QUERY");
                                 operationDic.Add("out_product_id",shopservicemodel.ServiceId);

                                 var shopProduct = new ShopProduct()
                                 {
                                    out_shop_id = shopservicemodel.ShopId.ToString()
                                 };

                                 var resultstr = PostMethodResult(UrlConfig.AliPayRequestUrl, "2016102601257298", "alipay.eco.mycar.maintain.serviceproduct.update", operationDic);

                                 JObject result = JsonConvert.DeserializeObject(resultstr) as JObject;
                                 if (result != null && result["alipay_eco_mycar_maintain_serviceproduct_update_response"] != null && result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["code"].ToString() == "10000")
                                 {
                                     Logger.Info(String.Format("门店:{0}存在此产品:{1}", shopservicemodel.ShopId, shopservicemodel.ServiceId));
                                     //update product
                                     operationDic.Clear();

                                     operationDic.Add("operation_type", "MODIFY");
                                     operationDic.Add("out_product_id", shopservicemodel.ServiceId);
                                     Dictionary<object,object> addparamDic=new Dictionary<object, object>();
                                     operationDic.Add("out_shop_id", shopservicemodel.ShopId.ToString());
                                     operationDic.Add("service_category_id", "4");
                                     operationDic.Add("product_name", shopservicemodel.ServersName);
                                     operationDic.Add("product_desc", shopservicemodel.AccessoriesName);
                                     operationDic.Add("off_price", shopservicemodel.Price);
                                     operationDic.Add("orig_price", shopservicemodel.Price);
                                     operationDic.Add("privilege_price", shopservicemodel.Price);
                                     operationDic.Add("privilege_tags", "限时抢购");
                                     operationDic.Add("privilege_close_time", DateTime.MaxValue.ToLongTimeString());
                                     operationDic.Add("status", 1);
                                     //operationDic.Add("shop_product",JsonConvert.SerializeObject(addparamDic));

                                     var  updateresultstr = PostMethodResult(UrlConfig.AliPayRequestUrl, "2016102601257298", "alipay.eco.mycar.maintain.serviceproduct.update", operationDic);

                                     JObject update_result = JsonConvert.DeserializeObject(updateresultstr) as JObject;
                                     if (update_result != null && update_result["alipay_eco_mycar_maintain_serviceproduct_update_response"] != null && update_result["alipay_eco_mycar_maintain_serviceproduct_update_response"]["code"].ToString() == "10000")
                                     {
                                         Logger.Info(String.Format("门店:{0} 下的此产品:{1} 更新成功", shopservicemodel.ShopId, shopservicemodel.ServiceId));
                                     }
                                     else
                                     {
                                         Logger.Info(String.Format("门店:{0} 下的此产品:{1} 更新失败,失败原因:{2}", shopservicemodel.ShopId, shopservicemodel.ServiceId, update_result["msg"].ToString()));
                                     }
                                 }
                                 else
                                 {
                                 */

                                #endregion
                            }
                            catch (Exception ex)
                            {
                                Logger.InfoException(ex);
                            }
                            finally
                            {
                                operationDic.Clear();
                                updateDic.Clear();
                                queryDictionary.Clear();
                            }
                        }
                    }
                }


            }
        }


        public static Dictionary<object, object> SetShopStatus(int shopid, bool flag)
        {
            Dictionary<object, object> paramDic = new Dictionary<object, object>();
            paramDic.Add("out_shop_id", shopid.ToString());
            paramDic.Add("status", Convert.ToInt32(flag).ToString());

            return paramDic;
        }

        public static void ShopDifference()
        {
            StringBuilder sb = new StringBuilder("");
            Dictionary<string, string> shopDic = GetShopDic();
            using (var thirdpartyclient = new Tuhu.Service.ThirdParty.AliPayServiceClient())
            {
                foreach (KeyValuePair<string, string> item in shopDic)
                {
                    var shopid = Convert.ToInt32(item.Key);
                    var TuanGouList = ShopBusiness.GetTuanGouXiCheModel(shopid);
                    if (!TuanGouList.Any())
                    {

                        //   Console.WriteLine("途虎门店"+item.Key + "|" + item.Value+"不存在美容团购服务");
                        var result = thirdpartyclient.IsExistedShop(shopid);
                        if (result != null && result.Result)
                        {
                            var servicelist = ShopBusiness.tuangouxicheTypes.Select(p => shopid + "|" + p).ToList();
                            foreach (var service in servicelist)
                            {
                                var serviceresult = thirdpartyclient.IsExistedShopService(service);
                                if (serviceresult != null && serviceresult.Result)
                                {
                                    sb.AppendLine("途虎门店" + item.Key + "|" + item.Value + "不存在美容团购服务");
                                    sb.AppendLine("车主平台" + item.Key + "|" + item.Value + "存在" + serviceresult + "美容团购服务");
                                    Console.WriteLine(item.Key + "|" + item.Value + "不同步");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            using (FileStream stream = new FileStream("Info.txt", FileMode.OpenOrCreate))
            {

                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(sb.ToString());
                }
            }
        }

        public static Dictionary<string, string> GetShopDic()
        {
            Dictionary<string, string> districtDic = new Dictionary<string, string>();
            string[] txtData = File.ReadAllLines("ShopList.txt", System.Text.Encoding.Default);
            if (txtData.Any())
            {
                foreach (var item in txtData.Where(q => q != null))
                {
                    // char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
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


        public int GetXiCheType(string typeName)
        {


            int type = 4;
            if (!string.IsNullOrWhiteSpace(typeName))
            {
                if (typeName.Equals("FU-CARWASHING-XICHE|9", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = 5;
                }
                else if (typeName.Equals("FU-CARWASHING-XICHE|2", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = 6;
                }
                else if (typeName.Equals("FU-CARWASHING-XICHE|10", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = 7;
                }
            }

            return type;
        }


        public static List<int> GetShopIdList()
        {
            string[] txtData = File.ReadAllLines(@"C:\Users\tuhuadmin\Desktop\ShopList.txt", System.Text.Encoding.UTF8);
            string[] txtData1 = File.ReadAllLines(@"C:\Users\tuhuadmin\Desktop\ShopList1.txt", System.Text.Encoding.UTF8);
            List<int> shopids = new List<int>();
            List<int> disableshopids = new List<int>();
            //sb.AppendLine("insert into Configuration..SEOMetaData( SEOKey ,MetaKeyword , MetaDescription ,Status ,CreatedTime ,UpdatedTime ,RuleKey)");
            foreach (var txt in txtData)
            {
                try
                {
                    int shopid = Convert.ToInt32(txt.Split(new char[] { '：' })[1]);
                    shopids.Add(shopid);
                }
                catch
                {
                    continue;
                }
            }
            foreach (var txt in txtData1)
            {
                try
                {
                    int disableshopid = Convert.ToInt32(txt.Split(new char[] { '：' })[1]);
                    disableshopids.Add(disableshopid);
                }
                catch
                {
                    continue;
                }
            }

            return shopids.Intersect(disableshopids).ToList();
        }
    }
}
