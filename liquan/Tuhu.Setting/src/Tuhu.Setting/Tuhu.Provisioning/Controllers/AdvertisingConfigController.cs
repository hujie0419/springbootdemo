using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using swc = System.Web.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Utility.Request;
using Tuhu.Service.ConfigLog;
using System.Threading;

namespace Tuhu.Provisioning.Controllers
{
    public class AdvertisingConfigController : Controller
    {

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            return View();
        }
        /// <summary>
        /// 复制/编辑广告位配置
        /// </summary>
        /// <param name="pkId"></param>
        /// <param name="isCopy"></param>
        /// <returns></returns>
        public ActionResult Edit(long pkId = 0, int isCopy = 0)
        {
            if (pkId > 0)
            {
                var manager = new AdvertisingConfigManager();
                var advertisingConfigInfo = manager.GetAdvertisingConfigInfo(pkId);
                string domain = swc.WebConfigurationManager.AppSettings["DoMain_image"];
                if (!string.IsNullOrEmpty(advertisingConfigInfo.MobileIconUrl))
                    advertisingConfigInfo.DomainMobileIconUrl = domain + advertisingConfigInfo.MobileIconUrl;
                if (!string.IsNullOrEmpty(advertisingConfigInfo.PcIconUrl))
                    advertisingConfigInfo.DomainPcIconUrl = domain + advertisingConfigInfo.PcIconUrl;
                if (!string.IsNullOrEmpty(advertisingConfigInfo.MobileImageUrl))
                    advertisingConfigInfo.DomainMobileImageUrl = domain + advertisingConfigInfo.MobileImageUrl;
                if (!string.IsNullOrEmpty(advertisingConfigInfo.PcImageUrl))
                    advertisingConfigInfo.DomainPcImageUrl = domain + advertisingConfigInfo.PcImageUrl;


                if (isCopy == 1)//复制时，将主键赋值成0
                    advertisingConfigInfo.PKID = 0;
                return View(advertisingConfigInfo);
            }
            var model = new AdvertisingConfigModel();
            model.AdType = 1;   //文字广告
            return View(model);
        }
        /// <summary>
        /// 获取广告位配置列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult GetAdvertisingConfigList(Pagination pagination)
        {
            var manager = new AdvertisingConfigManager();
            var advertisingConfigList = manager.GetAdvertisingConfigList(pagination);
            return Content(JsonConvert.SerializeObject(new
            {
                rows = advertisingConfigList,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            }));
        }
        /// <summary>
        /// 新增/复制/编辑广告
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult AddAdvertisingConfig(AdvertisingConfigModel model)
        {
            var manager = new AdvertisingConfigManager();
            if (!string.IsNullOrEmpty(model.StartVersion))
            {
                Version startVersion;
                if (!Version.TryParse(model.StartVersion, out startVersion))
                {
                    return Content(JsonConvert.SerializeObject(new
                    {
                        state = "failure",
                        message = "开始版本的类型有误",
                        data = ""
                    }));
                }
            }
            if (!string.IsNullOrEmpty(model.EndVersion))
            {
                Version endVersion;
                if (!Version.TryParse(model.EndVersion, out endVersion))
                {
                    return Content(JsonConvert.SerializeObject(new
                    {
                        state = "failure",
                        message = "结束版本的类型有误",
                        data = ""
                    }));
                }
            }
            //根据区域信息和产品线判断配置是否存在
            var exsitConfigInfo = manager.GetAdvertisingConfigInfo(model.ProvinceID, model.CityID, model.ProductLine, model.AdType);
            if (exsitConfigInfo != null && exsitConfigInfo.PKID != model.PKID)
            {
                exsitConfigInfo.Status = 0;//禁用
                manager.UpdateAdvertisingConfig(exsitConfigInfo);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.PKID,
                        ObjectType = "AdvertisingConfig",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(exsitConfigInfo),
                        Remark = "禁用广告配置",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
            }
            //新增
            if (model.PKID == 0)
            {
                model.AdLocation = 1;//下单完成页
                model.Status = 1;
                model.Creator = User.Identity.Name;
                manager.AddAdvertisingConfig(model);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.PKID,
                        ObjectType = "AdvertisingConfig",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = "新增广告配置",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
            }
            //编辑
            else
            {
                var configInfo = manager.GetAdvertisingConfigInfo(model.PKID);
                model.AdLocation = configInfo.AdLocation; ;//下单完成页
                model.Status = 1;//启用
                model.Creator = configInfo.Creator;
                manager.UpdateAdvertisingConfig(model);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.PKID,
                        ObjectType = "AdvertisingConfig",
                        BeforeValue = JsonConvert.SerializeObject(configInfo),
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = "修改广告配置",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
            }
            //等待1秒，写库同步到读库
            Thread.Sleep(1000);
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));
        }
        /// <summary>
        /// 启用广告位配置
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult EnableAdvertisingConfig(long pkid)
        {
            var manager = new AdvertisingConfigManager();
            var configInfo = manager.GetAdvertisingConfigInfo(pkid);

            //根据区域信息和产品线判断配置是否存在
            var exsitConfigInfo = manager.GetAdvertisingConfigInfo(configInfo.ProvinceID, configInfo.CityID, configInfo.ProductLine, configInfo.AdType);
            if (exsitConfigInfo != null)
            {
                if (exsitConfigInfo.PKID != configInfo.PKID)
                {
                    exsitConfigInfo.Status = 0;  //禁用
                    manager.UpdateAdvertisingConfig(exsitConfigInfo);
                    #region 日志记录
                    using (var client = new ConfigLogClient())
                    {
                        var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = exsitConfigInfo.PKID,
                            ObjectType = "AdvertisingConfig",
                            BeforeValue = "",
                            AfterValue = JsonConvert.SerializeObject(exsitConfigInfo),
                            Remark = "禁用广告配置",
                            Creator = User.Identity.Name,
                        }));
                    }
                    #endregion
                }
            }
            bool result = false;
            if (configInfo != null && configInfo.Status == 0)
            {
                configInfo.Status = 1;  //启用
                result = manager.UpdateAdvertisingConfig(configInfo);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = configInfo.PKID,
                        ObjectType = "AdvertisingConfig",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(configInfo),
                        Remark = "启用广告配置",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
            }
            else
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "failure",
                    message = "当前已是启用状态"
                }));
            //等待1秒，写库同步到读库
            Thread.Sleep(1000);
            if (result)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功"

                }));
            else
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "failure",
                    message = "操作失败"
                }));
        }
        /// <summary>
        /// 删除广告配置
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult DeleteAdvertisingConfig(long pkid)
        {
            var manager = new AdvertisingConfigManager();
            var configInfo = manager.GetAdvertisingConfigInfo(pkid);
            bool result = manager.DeleteAdvertisingConfig(pkid);
            if (result)
            {
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = pkid,
                        ObjectType = "AdvertisingConfig",
                        BeforeValue = JsonConvert.SerializeObject(configInfo),
                        AfterValue = "",
                        Remark = "删除广告配置",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
                //等待1秒，写库同步到读库
                Thread.Sleep(1000);
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功"

                }));
            }
            else
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "failure",
                    message = "操作失败"

                }));
        }
        /// <summary>
        /// 图片上传地址
        /// </summary>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="identifying">标识</param>
        /// <param name="urlStr">地址</param>
        /// <returns></returns>
        public ActionResult ImageUploadToAli(string from)
        {
            string _BImage = string.Empty;
            string _SImage = string.Empty;
            string _ImgGuid = Guid.NewGuid().ToString();
            Exception ex = null;
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    using (var client = new Tuhu.Service.Utility.FileUploadClient())
                    {
                        string dirName = swc.WebConfigurationManager.AppSettings["UploadDoMain_image"];

                        var result = client.UploadImage(new ImageUploadRequest(dirName, buffer));
                        result.ThrowIfException(true);
                        if (result.Success)
                        {
                            _BImage = result.Result;
                            //_SImage= ImageHelper.GetImageUrl(result.Result, 100);
                        }
                    }
                }
                catch (Exception error)
                {
                    ex = error;
                }
            }
            string imgUrl = swc.WebConfigurationManager.AppSettings["DoMain_image"] + _BImage;

            if (from == "content")
            {
                return Json(new { error = 0, url = imgUrl });
            }
            return Json(new
            {
                BImage = _BImage,
                SImage = imgUrl,
                Msg = ex == null ? "上传成功" : ex.Message
            }, "text/html");
        }
        /// <summary>
        /// 根据provinceId、cityId、productLine和adType获取广告信息
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="productLine"></param>
        /// <param name="adType"></param>
        /// <returns></returns>
        public ActionResult GetAdvertisingConfigInfo(int provinceId, int cityId, string productLine, int adType, int pkId)
        {
            var manager = new AdvertisingConfigManager();
            var exsitConfigInfo = manager.GetAdvertisingConfigInfo(provinceId, cityId, productLine, adType);
            var result = false;
            if (exsitConfigInfo != null)
            {
                if (exsitConfigInfo.PKID != pkId)
                    result = true;
            }
            return Content(JsonConvert.SerializeObject(new { result }));
        }
    }
}