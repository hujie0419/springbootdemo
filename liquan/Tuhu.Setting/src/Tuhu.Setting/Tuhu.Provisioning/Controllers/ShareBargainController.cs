using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business.ShareBargain;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Activity;
using Tuhu.Component.Framework.Identity;
using Tuhu.Service.Activity.Models;
using System.Collections.Generic;
using Tuhu.Component.Log;
using BargainGlobalConfigModel = Tuhu.Provisioning.DataAccess.Entity.BargainGlobalConfigModel;
using Tuhu.Models;
using System.Linq;
using Tuhu.Service.Utility.Request;
using Tuhu.Provisioning.Common;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.Controllers
{
    public class ShareBargainController : Controller
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("ShareBargainController");

        public JsonResult UploadImage()
        {
            var resultStatus = false;
            string resultMsg = string.Empty;
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    var uploadResult = buffer.UpdateLoadImage();
                    resultStatus = uploadResult.Item1;
                    resultMsg = uploadResult.Item2;
                }
                catch (Exception exp)
                {
                    WebLog.LogException(exp);
                }
            }
            return Json(new
            {
                Status = resultStatus, 
                ImgUrl = resultMsg
            });
        }

        // GET: ShareBargain
        public ActionResult ShareBargain()
        {
            return View(ShareBargainManager.GetBackgroundTheme());
        }

        /// <summary>
        /// 获取砍价列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PartialViewResult ShareBargainList(BargainProductRequest request)
        {
            var pager = new Component.Common.Models.PagerModel(request.PageIndex, request.PageSize);
            ViewBag.request = request;
            var data = ShareBargainManager.SelectBargainProductList(request, pager);
            return PartialView(new ListModel<ShareBargainItemModel>() { Pager = pager, Source = data });
        }

        [HttpGet]
        public JsonResult FetchBargainProductById(int apId)
        {
            if (apId == 0)
            {
                return Json(new
                {
                    Code = 0,
                    Message = "参数不符合要求"
                }, JsonRequestBehavior.AllowGet);
            }

            var dat = ShareBargainManager.FetchBargainProductById(apId);
            if (dat == null)
            {
                return Json(new
                {
                    Code = 2,
                    Message = "未找到详细信息"
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Code = 1,
                Result = dat
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckPid(string PID, DateTime BeginTime, DateTime EndTime)
        {
            if (string.IsNullOrWhiteSpace(PID))
            {
                return Json(new CheckPidResult()
                {
                    Code = 0,
                    Info = "PID不能为空"
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(ShareBargainManager.CheckPid(PID, BeginTime, EndTime), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 检查优惠券id是否可用
        /// </summary>
        /// <param name="PID">优惠券id</param>
        /// <param name="BeginTime">砍价产品开始时间</param>
        /// <param name="EndTime">砍价产品结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CheckCouponPid(string PID, DateTime BeginTime, DateTime EndTime)
        {
            if (string.IsNullOrWhiteSpace(PID))
            {
                return Json(new CheckPidResult()
                {
                    Code = 0,
                    Info = "PID不能为空"
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(ShareBargainManager.CheckCouponPid(PID, BeginTime, EndTime), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加砍价商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddSharBargainProduct(ShareBargainProductModel request)
        {
            request.ProductType = 1;
            if (request.EndDateTime < request.BeginDateTime)
            {
                return Json(new { Code = 0, Info = "下架时间必须大于上架时间" });
            }
            if (request.ShowBeginTime > request.BeginDateTime)
            {
                return Json(new { Code = 0, Info = "开始显示时间必须小于等于上架时间" });
            }
            if (request.BigCutBeforeCount != null && request.BigCutPriceRate != null)
            {
                if ((request.BigCutBeforeCount == null && request.BigCutPriceRate != null) || (request.BigCutBeforeCount != null && request.BigCutPriceRate == null))
                {
                    return Json(new { Code = 0, Info = "请填写完整的金额分布人数和百分比" });
                }
                if (request.BigCutBeforeCount <= 0 || request.BigCutPriceRate <= 0 || request.BigCutPriceRate >= 100)
                {
                    return Json(new { Code = 0, Info = "请填写正确的金额分布人数和百分比" });
                }
                if (request.Times <= request.BigCutBeforeCount)
                {
                    return Json(new { Code = 0, Info = "金额分布人数应小于砍价次数" });
                }
            }

            try
            {
                var data = ShareBargainManager.CheckPid(request.PID, request.BeginDateTime, request.EndDateTime);
                if (data.Code == 2)
                {
                    return Json(new { Code = 0, Info = "该时间段不能配置该商品" });
                }
                var addResultPKID = ShareBargainManager.AddSharBargainProduct(request, ThreadIdentity.Operator.Name);
                if (addResultPKID > 0)
                {
                    AddSharBargainLog(addResultPKID);
                    using (var client = new ShareBargainClient())
                    {
                        var refreshResult = client.RefreshShareBargainCache();
                        if (!refreshResult.Success)
                        {
                            //缓存刷新失败补偿一次
                            var repeatRefreshResult = client.RefreshShareBargainCache();
                            if (!repeatRefreshResult.Success)
                            {
                                Logger.Log(Level.Warning, $"AddSharBargainProduct,刷新缓存失败，ErrorMessage：{repeatRefreshResult.ErrorMessage}");
                                return Json(new { Code = 1, Info = "数据保存成功,但刷新缓存失败，请手动刷新缓存" });
                            }
                        }
                    }
                    return Json(new { Code = 1, Info = "添加成功" });
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"AddSharBargainProduct异常，ex{ex}");
            }
            return Json(new { Code = 0, Info = "添加失败，请稍后重试" });
        }

        /// <summary>
        /// 添加砍价优惠券信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddSharBargainCoupon(ShareBargainProductModel request)
        {
            request.ProductType = 2;
            if (request.EndDateTime < request.BeginDateTime)
            {
                return Json(new { Code = 0, Info = "下架时间必须大于上架时间" });
            }
            if (request.ShowBeginTime > request.BeginDateTime)
            {
                return Json(new { Code = 0, Info = "开始显示时间必须小于等于上架时间" });
            }
            if (request.BigCutBeforeCount != null && request.BigCutPriceRate != null)
            {
                if ((request.BigCutBeforeCount == null && request.BigCutPriceRate != null) || (request.BigCutBeforeCount != null && request.BigCutPriceRate == null))
                {
                    return Json(new { Code = 0, Info = "请填写完整的金额分布人数和百分比" });
                }
                if (request.BigCutBeforeCount <= 0 || request.BigCutPriceRate <= 0 || request.BigCutPriceRate >= 100)
                {
                    return Json(new { Code = 0, Info = "请填写正确的金额分布人数和百分比" });
                }
                if (request.Times <= request.BigCutBeforeCount)
                {
                    return Json(new { Code = 0, Info = "金额分布人数应小于砍价次数" });
                }
            }

            //验证商品详情页图片:至少有一张
            if(string.IsNullOrWhiteSpace(request.ProductDetailImg1) && string.IsNullOrWhiteSpace(request.ProductDetailImg2) &&
                string.IsNullOrWhiteSpace(request.ProductDetailImg3) && string.IsNullOrWhiteSpace(request.ProductDetailImg4) &&
                string.IsNullOrWhiteSpace(request.ProductDetailImg5))
            {
                return Json(new { Code = 0, Info = "优惠券类型商品至少添加一张商品详情图" });
            }

            try
            {
                var data = ShareBargainManager.CheckCouponPid(request.PID, request.BeginDateTime, request.EndDateTime);
                if (data.Code == 2)
                {
                    return Json(new { Code = 0, Info = "该时间段不能配置该优惠券" });
                }
                var addResultPKID = ShareBargainManager.AddSharBargainCoupon(request, ThreadIdentity.Operator.Name);
                if (addResultPKID > 0)
                {
                    AddSharBargainLog(addResultPKID);
                    using (var client = new ShareBargainClient())
                    {
                        var refreshResult = client.RefreshShareBargainCache();
                        if (!refreshResult.Success)
                        {
                            //缓存刷新失败补偿一次
                            var repeatRefreshResult = client.RefreshShareBargainCache();
                            if (!repeatRefreshResult.Success)
                            {
                                Logger.Log(Level.Warning, $"AddSharBargainCoupon,刷新缓存失败，ErrorMessage：{repeatRefreshResult.ErrorMessage}");
                                return Json(new { Code = 1, Info = "添加成功,但刷新缓存失败，请手动刷新缓存" });
                            }
                        }
                    }
                    return Json(new { Code = 1, Info = "添加成功" });
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"AddSharBargainCoupon异常，ex{ex}");
            }
            return Json(new { Code = 0, Info = "添加失败，请稍后重试" });
        }

        private void AddSharBargainLog(int PKID)
        {
            try
            {
                var operationLogModel = new SalePromotionActivityLogModel()
                {
                    ReferId = "ShareBarginProduct_" + PKID.ToString(),
                    ReferType = "ShareBargin",
                    OperationLogType = "ShareBargin_InsertShareBargin",
                    CreateDateTime = DateTime.Now.ToString(),
                    CreateUserName = this.User.Identity.Name,
                    //日志详情
                    LogDetailList = new List<SalePromotionActivityLogDetail>()
                };
                SetOperationLog(operationLogModel, "AddSharBargainLog");
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Warning, $"AddSharBargainLog操作日志记录异常{ex}");
            }
        }

        /// <summary>
        /// 修改全局配置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public bool UpdateGlobalConfig(BargainGlobalConfigModel request)
        {
            var oldGlobal = new BargainGlobalConfigModel();
            try
            {
                oldGlobal = ShareBargainManager.FetchBargainProductGlobalConfig();
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"全局配置修改前获取数据失败ex:{ex}");
            }

            request.QAData = JsonConvert.SerializeObject(request.BargainRule);
            var result = ShareBargainManager.UpdateGlobalConfig(request);
            if (result)
            {
                using (var client = new ShareBargainClient())
                {
                    var refreshResult = client.RefreshShareBargainCache();
                    if (!refreshResult.Success)
                    {
                        Logger.Log(Level.Warning, $"UpdateGlobalConfig,刷新缓存失败，ErrorMessage：{refreshResult.ErrorMessage}");
                    }
                }

                //操作日志
                UpdateGlobalConfiguctLog(oldGlobal, request);
            }
            return result;
        }

        /// <summary>
        /// 修改全局配置记录操作日志
        /// </summary>
        /// <param name="oldModel"></param>
        /// <param name="newModel"></param>
        private void UpdateGlobalConfiguctLog(BargainGlobalConfigModel oldModel, BargainGlobalConfigModel newModel)
        {
            try
            {
                oldModel = oldModel == null ? new BargainGlobalConfigModel() : oldModel;
                var operationLogModel = new SalePromotionActivityLogModel()
                {
                    ReferId = "ShareBargin_GlobalConfig",
                    ReferType = "ShareBargin",
                    OperationLogType = "ShareBargin_UpdateGlobalConfig",
                    CreateDateTime = DateTime.Now.ToString(),
                    CreateUserName = this.User.Identity.Name,
                    LogDetailList = new List<SalePromotionActivityLogDetail>()
                };
                if (oldModel.QAData != newModel.QAData)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "问答组",
                        OldValue = GetQALogString(oldModel.BargainRule),
                        NewValue = GetQALogString(newModel.BargainRule)
                    });
                }
                if (oldModel.RulesCount != newModel.RulesCount)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "活动规则数量",
                        OldValue = oldModel.RulesCount.ToString(),
                        NewValue = newModel.RulesCount.ToString()
                    });
                }
                if (oldModel.SliceShowText != newModel.SliceShowText)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "砍价成功轮播文案",
                        OldValue = oldModel.SliceShowText,
                        NewValue = newModel.SliceShowText,
                    });
                }
                if (oldModel.WXAPPListShareText != newModel.WXAPPListShareText)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "小程序列表分享文案",
                        OldValue = oldModel.WXAPPListShareText,
                        NewValue = newModel.WXAPPListShareText,
                    });
                }
                if (oldModel.WXAPPListShareImg != newModel.WXAPPListShareImg)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "小程序列表分享图片",
                        OldValue = oldModel.WXAPPListShareImg,
                        NewValue = newModel.WXAPPListShareImg,
                    });
                }
                if (oldModel.APPListShareTag != newModel.APPListShareTag)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "APP列表页分享标签",
                        OldValue = oldModel.APPListShareTag,
                        NewValue = newModel.APPListShareTag,
                    });
                }
                if (oldModel.WXAPPDetailShareText != newModel.WXAPPDetailShareText)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "小程序详情分享文案",
                        OldValue = oldModel.WXAPPDetailShareText,
                        NewValue = newModel.WXAPPDetailShareText
                    });
                }
                if (oldModel.AppDetailShareTag != newModel.AppDetailShareTag)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "APP详情分享标签",
                        OldValue = oldModel.AppDetailShareTag,
                        NewValue = newModel.AppDetailShareTag
                    });
                }

                SetOperationLog(operationLogModel, "UpdateGlobalConfiguctLog");
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Warning, $"UpdateGlobalConfiguctLog操作日志异常:{ex}");
            }
        }

        /// <summary>
        /// 获取操作日志问答拼接字符串
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        private string GetQALogString(List<DataAccess.Entity.BargainRules> rules)
        {
            string resultStr = "";
            int i = 0;
            if ((bool)rules?.Any())
            {
                foreach (var item in rules)
                {
                    i++;
                    resultStr += $"{i}.问:{item.Question}";
                    resultStr += $"答:{item.Answer}";
                }
            }

            return resultStr;
        }

        /// <summary>
        /// 获取全局设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FetchBargainProductGlobalConfig()
        {
            return Json(ShareBargainManager.FetchBargainProductGlobalConfig(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改砍价商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateBargainProductById(ShareBargainProductModel request)
        {
            if (request.PKID == null ||
                request.CurrentStockCount > request.TotalStockCount
                || request.EndDateTime < request.BeginDateTime
                || request.ShowBeginTime > request.BeginDateTime)
            {
                return Json(new { Status = false, Msg = "保存失败" });
            }
            try
            {
                //保存前获取旧数据
                var oldInfo = ShareBargainManager.FetchBargainProductById((int)request.PKID);

                //检查是否能重新上架 || 未上架
                //if ((oldInfo.EndDateTime < DateTime.Now && request.EndDateTime >= DateTime.Now) 
                //    || request.BeginDateTime > DateTime.Now)
                //{
                //    var checkBackOn = ShareBargainManager.CheckProductBackOn((int)request.PKID);
                //    if (!checkBackOn.Item1)
                //    {
                //        var backOnTime = checkBackOn.Item2 == default(DateTime) ?
                //            oldInfo.EndDateTime.AddHours(48) : (DateTime)checkBackOn.Item2;

                //        string msg = $"存在用户砍价未完成且发起砍价未满48小时,可重新上架时间:{backOnTime}";
                //        if (request.BeginDateTime > DateTime.Now)
                //        {
                //            msg = $"存在用户砍价未完成且发起砍价未满48小时,无法改为未上架(可提前下架),可修改时间:{backOnTime}";
                //        }

                //        return Json(new { Status = false, Msg = msg });
                //    }
                //}

                //保存
                var saveResult = ShareBargainManager.UpdateBargainProductById(request);
                if (saveResult)
                {
                    //记录操作日志
                    UpdateBargainProductLog(oldInfo, request);
                    using (var client = new ShareBargainClient())
                    {
                        var refreshResult = client.RefreshShareBargainCache();
                        if (!refreshResult.Success)
                        {
                            //缓存刷新失败补偿一次
                            var repeatRefreshResult = client.RefreshShareBargainCache();
                            if (!repeatRefreshResult.Success)
                            {
                                Logger.Log(Level.Warning, $"UpdateBargainProductById,刷新缓存失败，ErrorMessage：{repeatRefreshResult.ErrorMessage}");
                                return Json(new { Status = true, Msg = "数据保存成功,但刷新缓存失败，请手动刷新缓存" });
                            }
                        }
                    }
                }
                else
                {
                    return Json(new { Status = false, Msg = "保存失败" });
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"UpdateBargainProductById异常，ex{ex}");
                return Json(new { Status = false, Msg = $"程序异常:{ex.Message},{ex.StackTrace}" });
            }
            return Json(new { Status = true, Msg = "保存成功" });
        }

        /// <summary>
        /// 修改砍价优惠券配置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult UpdateBargainCouponById(ShareBargainProductModel request)
        {
            if (request.PKID == null)
            {
                return Json(new { Status = false, Msg = "保存失败" });
            }

            if (request.CurrentStockCount > request.TotalStockCount
                || request.EndDateTime < request.BeginDateTime
                || request.ShowBeginTime > request.BeginDateTime)
            {
                return Json(new { Status = false, Msg = "上下架时间或显示时间不正确" });
            }

            //验证商品详情页图片:至少有一张
            if (string.IsNullOrWhiteSpace(request.ProductDetailImg1) && string.IsNullOrWhiteSpace(request.ProductDetailImg2) &&
                string.IsNullOrWhiteSpace(request.ProductDetailImg3) && string.IsNullOrWhiteSpace(request.ProductDetailImg4) &&
                string.IsNullOrWhiteSpace(request.ProductDetailImg5))
            {
                return Json(new { Status = false, Msg = "优惠券类型商品至少添加一张商品详情图" });
            } 

            try
            {
                //保存前获取旧数据
                var oldInfo = ShareBargainManager.FetchBargainProductById((int)request.PKID);

                //检查是否能重新上架 || 未上架
                //if ((oldInfo.EndDateTime < DateTime.Now && request.EndDateTime >= DateTime.Now)
                //    || request.BeginDateTime > DateTime.Now)
                //{
                //    var checkBackOn = ShareBargainManager.CheckProductBackOn((int)request.PKID);
                //    if (!checkBackOn.Item1)
                //    {
                //        var backOnTime = checkBackOn.Item2 == default(DateTime) ?
                //            oldInfo.EndDateTime.AddHours(48) : (DateTime)checkBackOn.Item2;

                //        string msg = $"存在用户砍价未完成且发起砍价未满48小时,可重新上架时间:{backOnTime}";
                //        if (request.BeginDateTime > DateTime.Now)
                //        {
                //            msg = $"存在用户砍价未完成且发起砍价未满48小时,无法改为未上架(可提前下架),可修改时间:{backOnTime}";
                //        }

                //        return Json(new { Status = false, Msg = msg });
                //    }
                //}

                //保存
                var saveResult = ShareBargainManager.UpdateBargainCouponById(request);
                if (saveResult)
                {
                    //记录操作日志
                    UpdateBargainProductLog(oldInfo, request);
                    using (var client = new ShareBargainClient())
                    {
                        var refreshResult = client.RefreshShareBargainCache();
                        if (!refreshResult.Success)
                        {
                            //缓存刷新失败补偿一次
                            var repeatRefreshResult = client.RefreshShareBargainCache();
                            if (!repeatRefreshResult.Success)
                            {
                                Logger.Log(Level.Warning, $"UpdateBargainCouponById,刷新缓存失败，ErrorMessage：{repeatRefreshResult.ErrorMessage}");
                                return Json(new { Status = true, Msg = "数据保存成功,但刷新缓存失败，请手动刷新缓存" });
                            }
                        }
                    }
                }
                else
                {
                    return Json(new { Status = false, Msg = "保存失败" });
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"UpdateBargainProductById异常，ex{ex}");
                return Json(new { Status = false, Msg = $"程序异常:{ex.Message},{ex.StackTrace}" });
            }
            return Json(new { Status = true, Msg = "保存成功" });
        }

        /// <summary>
        /// 修改砍价配置操作日志
        /// </summary>
        /// <param name="oldModel"></param>
        /// <param name="newModel"></param>
        private void UpdateBargainProductLog(ShareBargainProductModel oldModel, ShareBargainProductModel newModel)
        {
            try
            {
                var operationLogModel = new SalePromotionActivityLogModel()
                {
                    ReferId = "ShareBarginProduct_" + newModel.PKID.ToString(),
                    ReferType = "ShareBargin",
                    OperationLogType = "ShareBargin_UpdateShareBargin",
                    CreateDateTime = DateTime.Now.ToString(),
                    CreateUserName = this.User.Identity.Name,
                    //日志详情
                    LogDetailList = new List<SalePromotionActivityLogDetail>()
                };
                if (oldModel.BeginDateTime != newModel.BeginDateTime)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "上架时间",
                        OldValue = oldModel.BeginDateTime.ToString(),
                        NewValue = newModel.BeginDateTime.ToString(),
                    });
                }
                if (oldModel.EndDateTime != newModel.EndDateTime)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "下架时间",
                        OldValue = oldModel.EndDateTime.ToString(),
                        NewValue = newModel.EndDateTime.ToString(),
                    });
                }
                if (oldModel.ShowBeginTime != newModel.ShowBeginTime)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "商品开始显示时间",
                        OldValue = oldModel.ShowBeginTime.ToString(),
                        NewValue = newModel.ShowBeginTime.ToString(),
                    });
                }
                if (oldModel.SimpleDisplayName != newModel.SimpleDisplayName)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "商品简称",
                        OldValue = oldModel.SimpleDisplayName,
                        NewValue = newModel.SimpleDisplayName
                    });
                }
                if (oldModel.OriginalPrice != newModel.OriginalPrice)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "商品售价",
                        OldValue = oldModel.OriginalPrice.ToString("0.00"),
                        NewValue = newModel.OriginalPrice.ToString("0.00")
                    });
                }
                if (oldModel.FinalPrice != newModel.FinalPrice)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "商品活动价",
                        OldValue = oldModel.FinalPrice.ToString("0.00"),
                        NewValue = newModel.FinalPrice.ToString("0.00")
                    });
                }
                if (oldModel.TotalStockCount != newModel.TotalStockCount)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "商品库存上限",
                        OldValue = oldModel.TotalStockCount.ToString(),
                        NewValue = newModel.TotalStockCount.ToString()
                    });
                }
                if (oldModel.Sequence != newModel.Sequence)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "商品显示顺序",
                        OldValue = oldModel.Sequence.ToString(),
                        NewValue = newModel.Sequence.ToString()
                    });
                }
                if (oldModel.Image1 != newModel.Image1
                    && (!string.IsNullOrWhiteSpace(oldModel.Image1) || !string.IsNullOrWhiteSpace(newModel.Image1)))
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "商品宣传图片",
                        OldValue = oldModel.Image1,
                        NewValue = newModel.Image1
                    });
                }
                if (oldModel.WXShareTitle != newModel.WXShareTitle)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "小程序分享标题",
                        OldValue = oldModel.WXShareTitle,
                        NewValue = newModel.WXShareTitle
                    });
                }
                if (oldModel.APPShareId != newModel.APPShareId)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "APP分享ID",
                        OldValue = oldModel.APPShareId,
                        NewValue = newModel.APPShareId
                    });
                }
                if (oldModel.ProductDetailImg1 != newModel.ProductDetailImg1)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "商品详情图片1",
                        OldValue = oldModel.ProductDetailImg1,
                        NewValue = newModel.ProductDetailImg1
                    });
                }
                if (oldModel.ProductDetailImg2 != newModel.ProductDetailImg2)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "商品详情图片2",
                        OldValue = oldModel.ProductDetailImg2,
                        NewValue = newModel.ProductDetailImg2
                    });
                }
                if (oldModel.ProductDetailImg3 != newModel.ProductDetailImg3)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "商品详情图片3",
                        OldValue = oldModel.ProductDetailImg3,
                        NewValue = newModel.ProductDetailImg3
                    });
                }
                if (oldModel.ProductDetailImg4 != newModel.ProductDetailImg4)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "商品详情图片4",
                        OldValue = oldModel.ProductDetailImg4,
                        NewValue = newModel.ProductDetailImg4
                    });
                }
                if (oldModel.ProductDetailImg5 != newModel.ProductDetailImg5)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "商品详情图片5",
                        OldValue = oldModel.ProductDetailImg5,
                        NewValue = newModel.ProductDetailImg5
                    });
                }

                SetOperationLog(operationLogModel, "UpdateBargainProductById");
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Warning, $"UpdateBargainProductById操作日志异常:{ex}");
            }
        }

        /// <summary>
        /// 删除砍价商品
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public ActionResult DeleteBargainProductById(int PKID)
        {
            if (PKID < 1)
            {
                return Json(new { Status = false, Msg = "删除失败" });
            }
            try
            {
                var delResult = ShareBargainManager.DeleteBargainProductById(PKID);
                if (delResult)
                {
                    DeleteLog(PKID);
                    using (var client = new ShareBargainClient())
                    {
                        var refreshResult = client.RefreshShareBargainCache();
                        if (!refreshResult.Success)
                        {
                            //缓存刷新失败补偿一次
                            var repeatRefreshResult = client.RefreshShareBargainCache();
                            if (!repeatRefreshResult.Success)
                            {
                                Logger.Log(Level.Warning, $"DeleteBargainProductById,刷新缓存失败，ErrorMessage：{repeatRefreshResult.ErrorMessage}");
                                return Json(new { Status = false, Msg = "数据保存成功,但刷新缓存失败，请手动刷新缓存" });
                            }
                        }
                    }
                }
                else
                {
                    return Json(new { Status = false, Msg = "删除失败" });
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"DeleteBargainProductById异常，ex{ex}");
            }
            return Json(new { Status = true, Msg = "删除成功" });
        }

        /// <summary>
        /// 删除操作日志记录
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        private void DeleteLog(int PKID)
        {
            try
            {
                var operationLogModel = new SalePromotionActivityLogModel()
                {
                    ReferId = "ShareBarginProduct_" + PKID.ToString(),
                    ReferType = "ShareBargin",
                    OperationLogType = "ShareBargin_Delete",
                    CreateDateTime = DateTime.Now.ToString(),
                    CreateUserName = this.User.Identity.Name,
                    //日志详情
                    LogDetailList = new List<SalePromotionActivityLogDetail>()
                };
                SetOperationLog(operationLogModel, "DeleteBargainProductById");
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Warning, $"DeleteBargainProductById操作日志记录异常{ex}");
            }
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public bool RefreshShareBargain()
        {
            using (var client = new ShareBargainClient())
            {
                var dat = client.RefreshShareBargainCache();
                return dat.Success;
            }
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="operationLogModel"></param>
        /// <param name="funNameString"></param>
        private void SetOperationLog(SalePromotionActivityLogModel operationLogModel, string funNameString)
        {
            try
            {
                using (var logClient = new SalePromotionActivityLogClient())
                {
                    var logResult = logClient.InsertAcitivityLogAndDetail(operationLogModel);
                    if (!(logResult.Success && logResult.Result))
                    {
                        Logger.Log(Level.Warning, $"{funNameString}操作日志记录失败ErrorMessage:{logResult.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"{funNameString},操作日志记录异常，ex{ex}");
            }
        }

        /// <summary>
        /// 获取活动的操作日志列表
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult ShareBargainLogList(BargainProductRequest request)
        {
            if (string.IsNullOrEmpty(request.PID))
            {
                request.PID = Request.QueryString["PKID"];
            }
            ViewBag.request = request;
            var result = new PagedModel<SalePromotionActivityLogModel>();
            using (var client = new SalePromotionActivityLogClient())
            {
                var listResult = client.GetOperationLogList(request.PID, request.PageIndex, request.PageSize);
                if (listResult.Success && listResult.Result != null)
                {
                    result = listResult.Result;
                }
            }
            ViewBag.pageSize = request.PageSize;
            ViewBag.pageIndex = request.PageIndex;
            ViewBag.totalRecords = result.Pager.Total;
            ViewBag.totalPage = result.Pager.Total % request.PageSize == 0
                ? result.Pager.Total / request.PageSize : (result.Pager.Total / request.PageSize) + 1;
            return View(result.Source);
        }

        /// <summary>
        /// 获取日志详情
        /// </summary>
        /// <param name="FPKID"></param>
        /// <returns></returns>
        public ActionResult GetOperationLogDetailList(string PKID)
        {
            var result = new List<SalePromotionActivityLogDetail>();
            using (var client = new SalePromotionActivityLogClient())
            {
                var listResult = client.GetOperationLogDetailList(PKID);
                if (listResult.Success && listResult.Result != null)
                {
                    result = listResult.Result.ToList();
                }
            }
            if (result?.Count() > 0)
            {
                foreach (var item in result)
                {
                    item.NewValue = item.NewValue == null ? "" : item.NewValue;
                    item.OldValue = item.OldValue == null ? "" : item.OldValue;
                }
            }
            return Json(result);
        }

    }
}