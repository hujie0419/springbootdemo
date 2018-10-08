using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.ConfigLog;
using System.Threading;

namespace Tuhu.Provisioning.Controllers
{
    public class PaymentPageConfigController : Controller
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [PowerManage]
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
        /// 复制/编辑下单完成页
        /// </summary>
        /// <param name="pkId"></param>
        /// <param name="isCopy"></param>
        /// <returns></returns>
        public ActionResult Edit(long pkId = 0, int isCopy = 0)
        {
            if (pkId > 0)
            {
                var manager = new PaymentPageConfigManager();
                var paymentPageConfigInfo = manager.GetPaymentPageConfigInfo(pkId);
                if (isCopy == 1)//复制时，将主键赋值成0
                    paymentPageConfigInfo.PKID = 0;
                return View(paymentPageConfigInfo);
            }
            return View(new PaymentPageConfigModel());
        }
        /// <summary>
        /// 获取下单完成页配置列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult GetPaymentPageConfigList(Pagination pagination)
        {
            var manager = new PaymentPageConfigManager();
            var paymentPageConfigList = manager.GetPaymentPageConfigList(pagination);
            return Content(JsonConvert.SerializeObject(new
            {
                rows = paymentPageConfigList,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            }));
        }
        /// <summary>
        /// 新增/复制/编辑下单完成页配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult AddPaymentPageConfig(PaymentPageConfigModel model)
        {
            var manager = new PaymentPageConfigManager();
            //根据区域信息和产品线判断配置是否存在
            var exsitConfigInfo = manager.GetPaymentPageConfigInfo(model.ProvinceID, model.CityID, model.ProductLine);
            if (exsitConfigInfo != null && exsitConfigInfo.PKID != model.PKID)
            {
                exsitConfigInfo.Status = 0;//禁用
                manager.UpdatePaymentPageConfig(exsitConfigInfo);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = exsitConfigInfo.PKID,
                        ObjectType = "PaymentPageConfig",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(exsitConfigInfo),
                        Remark = "禁用下单完成页配置",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
            }
            //新增
            if (model.PKID == 0)
            {
                model.Status = 1;
                model.Creator = User.Identity.Name;
                manager.AddPaymentPageConfig(model);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.PKID,
                        ObjectType = "PaymentPageConfig",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = "新增下单完成页配置",
                        Creator = User.Identity.Name,
                    }));
                }
                #endregion
            }
            //编辑
            else
            {
                var configInfo = manager.GetPaymentPageConfigInfo(model.PKID);
                model.Status = 1;//启用
                model.Creator = configInfo.Creator;
                manager.UpdatePaymentPageConfig(model);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.PKID,
                        ObjectType = "PaymentPageConfig",
                        BeforeValue = JsonConvert.SerializeObject(configInfo),
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = "修改下单完成页配置",
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
        /// 启用下单完成页配置
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult EnablePaymentPageConfig(long pkid)
        {
            var manager = new PaymentPageConfigManager();
            var configInfo = manager.GetPaymentPageConfigInfo(pkid);

            //根据区域信息和产品线判断配置是否存在
            var exsitConfigInfo = manager.GetPaymentPageConfigInfo(configInfo.ProvinceID, configInfo.CityID, configInfo.ProductLine);
            if (exsitConfigInfo != null)
            {
                if (exsitConfigInfo.PKID != configInfo.PKID)
                {
                    exsitConfigInfo.Status = 0;  //禁用
                    manager.UpdatePaymentPageConfig(exsitConfigInfo);
                    #region 日志记录
                    using (var client = new ConfigLogClient())
                    {
                        var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = exsitConfigInfo.PKID,
                            ObjectType = "PaymentPageConfig",
                            BeforeValue = "",
                            AfterValue = JsonConvert.SerializeObject(exsitConfigInfo),
                            Remark = "禁用下单完成页配置",
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
                result = manager.UpdatePaymentPageConfig(configInfo);
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = configInfo.PKID,
                        ObjectType = "PaymentPageConfig",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(configInfo),
                        Remark = "启用下单完成页配置",
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
            {
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
        /// 删除下单完成页配置
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult DeletePaymentPageConfig(long pkid)
        {
            var manager = new PaymentPageConfigManager();
            var configInfo = manager.GetPaymentPageConfigInfo(pkid);
            bool result = manager.DeletePaymentPageConfig(pkid);
            if (result)
            {
                #region 日志记录
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = pkid,
                        ObjectType = "PaymentPageConfig",
                        BeforeValue = JsonConvert.SerializeObject(configInfo),
                        AfterValue = "",
                        Remark = "删除下单完成页配置",
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
        /// 根据provinceId、cityId和productLine获取配置信息
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="productLine"></param>
        /// <returns></returns>
        public ActionResult GetPaymentPageConfigInfo(int provinceId, int cityId, string productLine, int pkid)
        {
            var manager = new PaymentPageConfigManager();
            var exsitConfigInfo = manager.GetPaymentPageConfigInfo(provinceId, cityId, productLine);
            var result = false;
            if (exsitConfigInfo != null)
            {
                if (exsitConfigInfo.PKID != pkid)
                    result = true;
            }
            return Content(JsonConvert.SerializeObject(new { result }));
        }
    }
}