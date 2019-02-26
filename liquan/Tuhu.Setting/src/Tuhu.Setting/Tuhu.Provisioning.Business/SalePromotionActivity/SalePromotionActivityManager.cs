using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tuhu.Component.Framework;
using Tuhu.Models;
using Tuhu.Provisioning.DataAccess.DAO.SalePromotionActivity;
using Tuhu.Provisioning.DataAccess.Entity.SalePromotionActivity;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.ModelDefinition.ProductModels;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Request;
using Tuhu.Service.ProductQuery;
using Tuhu.Service.Stock;
using Tuhu.Service.Stock.Request;
using Tuhu.Service.Stock.Response;

namespace Tuhu.Provisioning.Business.SalePromotionActivity
{
    public class SalePromotionActivityManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("SalePromotionActivityManager");
        #region 操作

        /// <summary>
        /// 新增促销活动
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool InsertActivity(SalePromotionActivityModel model, out string message)
        {
            var result = false;
            message = "操作失败,请刷新重试";
            using (var client = new SalePromotionActivityClient())
            {
                //1.新增活动
                var insertResult = client.InsertActivity(model);
                if (insertResult.Success && insertResult.Result)
                {
                    result = true;
                    #region 操作日志
                    try
                    {
                        string discontContent = string.Empty;
                        string label = "元";
                        if (model.DiscountContentList?.Count > 0)
                        {
                            if (model.DiscountContentList[0].DiscountMethod == 2)
                            {
                                label = "件";
                            }
                            foreach (var item in model.DiscountContentList)
                            {
                                discontContent += $"满{item.Condition}{label} 打{item.DiscountRate / 100}折|";
                            }
                            if (discontContent.Length > 0)
                            {
                                discontContent = discontContent.Substring(0, discontContent.Length - 1);
                            }
                        }
                        var operationLogModel = new SalePromotionActivityLogModel()
                        {
                            ReferId = model.ActivityId,
                            ReferType = SalePromotionActivityConst.SalePromotionActivity,
                            OperationLogType = SalePromotionActivityConst.Insert_Activity,
                            CreateDateTime = DateTime.Now.ToString(),
                            CreateUserName = model.CreateUserName,
                            //日志详情
                            LogDetailList = new List<SalePromotionActivityLogDetail>() {
                            new SalePromotionActivityLogDetail(){
                                OperationLogType="ActivityInfo",
                                Property="活动名称",
                                NewValue=model.Name,
                            },
                            new SalePromotionActivityLogDetail(){
                                 OperationLogType="ActivityInfo",
                                Property="活动描述",
                                NewValue=model.Description,
                            },
                            new SalePromotionActivityLogDetail(){
                                 OperationLogType="ActivityInfo",
                                Property="活动促销语",
                                NewValue=model.Banner,
                            },
                            new SalePromotionActivityLogDetail(){
                                 OperationLogType="ActivityInfo",
                                Property="活动开始时间",
                                NewValue=model.StartTime,
                            },
                            new SalePromotionActivityLogDetail(){
                                OperationLogType="ActivityInfo",
                                Property="活动终止时间",
                                NewValue=model.EndTime,
                            },
                            new SalePromotionActivityLogDetail(){
                                OperationLogType="ActivityInfo",
                                Property="标签名称",
                                NewValue=model.Is_DefaultLabel==1?"折(默认标签)":model.Label,
                            },
                            new SalePromotionActivityLogDetail(){
                                 OperationLogType="ActivityInfo",
                                Property="单品限购数",
                                NewValue=model.Is_PurchaseLimit==1?"不限购":model.LimitQuantity.ToString(),
                            },
                            new SalePromotionActivityLogDetail(){
                                 OperationLogType="ActivityInfo",
                                Property="活动类型",
                                NewValue="打折",
                            },
                            new SalePromotionActivityLogDetail(){
                                 OperationLogType="ActivityInfo",
                                Property="活动内容",
                                NewValue=discontContent,
                            },
                        }
                        };
                        SetOperationLog(operationLogModel, "InsertActivity");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(Level.Error, ex, $"InsertActivityLog新增活动操作日志记录异常:{ex}");
                    }
                    #endregion
                }
                else
                {
                    if (insertResult.ErrorCode == "1")
                    {
                        message = insertResult.ErrorMessage;
                    }
                    else
                    {
                        Logger.Log(Level.Warning, $"InsertActivity新增活动失败：errormessage:{insertResult.ErrorMessage}");
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 修改促销活动
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UpdateActivity(SalePromotionActivityModel model, out string message)
        {
            var result = false;
            message = "操作失败请刷新重试";
            using (var client = new SalePromotionActivityClient())
            {
                var oldModel = client.GetActivityInfo(model.ActivityId).Result;//更新前的活动信息
                var updateResult = client.UpdateActivity(model);
                if (updateResult.Success && updateResult.Result)
                {
                    result = true;
                    DateTime.TryParse(model.StartTime, out DateTime startTime);
                    DateTime.TryParse(model.EndTime, out DateTime endTime);
                    #region 操作日志
                    try
                    {
                        var operationLogModel = new SalePromotionActivityLogModel()
                        {
                            ReferId = model.ActivityId,
                            ReferType = SalePromotionActivityConst.SalePromotionActivity,
                            OperationLogType = SalePromotionActivityConst.Update_Activity,
                            CreateDateTime = DateTime.Now.ToString(),
                            CreateUserName = model.CreateUserName,
                            //日志详情
                            LogDetailList = new List<SalePromotionActivityLogDetail>()
                        };
                        if (oldModel.Name != model.Name)
                        {
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "活动名称",
                                OldValue = oldModel.Name,
                                NewValue = model.Name,
                            });
                        }
                        if (oldModel.Description != model.Description)
                        {
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "活动描述",
                                OldValue = oldModel.Description,
                                NewValue = model.Description,
                            });
                        }
                        if (oldModel.Banner != model.Banner)
                        {
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "促销描述",
                                OldValue = oldModel.Banner,
                                NewValue = model.Banner,
                            });
                        }
                        DateTime.TryParse(oldModel.StartTime, out DateTime oldStart);
                        DateTime.TryParse(oldModel.StartTime, out DateTime oldEnd);
                        DateTime.TryParse(model.StartTime, out DateTime newStart);
                        DateTime.TryParse(model.StartTime, out DateTime newEnd);
                        if (oldStart != newStart)
                        {
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "活动开始时间",
                                OldValue = oldModel.StartTime,
                                NewValue = startTime.ToString(),
                            });
                        }
                        if (oldEnd != newEnd)
                        {
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "活动终止时间",
                                OldValue = oldModel.EndTime,
                                NewValue = endTime.ToString(),
                            });
                        }
                        if (oldModel.Is_DefaultLabel != model.Is_DefaultLabel || oldModel.Label != model.Label)
                        {
                            string oldLabel;
                            string newLabel;
                            if (oldModel.Is_DefaultLabel == 1)
                            {
                                oldLabel = "折(默认标签)";
                            }
                            else
                            {
                                oldLabel = oldModel.Label;
                            }
                            if (model.Is_DefaultLabel == 1)
                            {
                                newLabel = "折(默认标签)";
                            }
                            else
                            {
                                newLabel = model.Label;
                            }
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "标签",
                                OldValue = oldLabel,
                                NewValue = newLabel
                            });
                        }
                        if (oldModel.Is_PurchaseLimit != model.Is_PurchaseLimit || oldModel.LimitQuantity != model.LimitQuantity)
                        {
                            string oldLimit;
                            string newLimit;
                            if (oldModel.Is_PurchaseLimit == 0)
                            {
                                oldLimit = "不限购";
                            }
                            else
                            {
                                oldLimit = oldModel.LimitQuantity.ToString();
                            }
                            if (model.Is_PurchaseLimit == 0)
                            {
                                newLimit = "不限购";
                            }
                            else
                            {
                                newLimit = model.LimitQuantity.ToString();
                            }
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "会场限购",
                                OldValue = oldLimit,
                                NewValue = newLimit,
                            });
                        }
                        if (oldModel.InstallMethod != model.InstallMethod)
                        {
                            string oldValue = "不限";
                            string newValue = "不限";
                            if (oldModel.InstallMethod == 1)
                            {
                                oldValue = "到店安装";
                            }
                            else if (oldModel.InstallMethod == 2)
                            {
                                oldValue = "上门安装";
                            }
                            else if (oldModel.InstallMethod == 3)
                            {
                                oldValue = "无需安装";
                            }
                            if (model.InstallMethod == 1)
                            {
                                newValue = "到店安装";
                            }
                            else if (model.InstallMethod == 2)
                            {
                                newValue = "上门安装";
                            }
                            else if (model.InstallMethod == 3)
                            {
                                newValue = "无需安装";
                            }
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "安装方式",
                                OldValue = oldValue,
                                NewValue = newValue
                            });
                        }
                        if (oldModel.PaymentMethod != model.PaymentMethod)
                        {
                            string oldValue = "不限";
                            string newValue = "不限";
                            if (oldModel.PaymentMethod == 1)
                            {
                                oldValue = "到店支付";
                            }
                            else if (oldModel.PaymentMethod == 2)
                            {
                                oldValue = "在线支付";
                            }
                            if (model.PaymentMethod == 1)
                            {
                                newValue = "到店支付";
                            }
                            else if (model.PaymentMethod == 2)
                            {
                                newValue = "在线支付";
                            }
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "支付方式",
                                OldValue = oldValue,
                                NewValue = newValue
                            });
                        }

                        if (oldModel.DiscountContentList?.Count > 0 && model.DiscountContentList?.Count > 0)
                        {
                            string oldValue = "全场满额折";
                            string newValue = "全场满额折";
                            string oldRuleValue = "";
                            string newRuleValue = "";
                            if (oldModel.DiscountContentList[0].DiscountMethod == 2)
                            {
                                oldValue = "单件满件折";
                                foreach (var item in oldModel.DiscountContentList)
                                {
                                    oldRuleValue = $"满{(int)item.Condition}件 享 {item.DiscountRate / 10}折 ";
                                }
                            }
                            else
                            {
                                foreach (var item in oldModel.DiscountContentList)
                                {
                                    oldRuleValue = $"满{item.Condition}元 享 {item.DiscountRate / 10}折 ";
                                }
                            }
                            if (model.DiscountContentList[0].DiscountMethod == 2)
                            {
                                newValue = "单件满件折";
                                foreach (var item in oldModel.DiscountContentList)
                                {
                                    newRuleValue = $"满{(int)item.Condition}件 享 {item.DiscountRate / 10}折 ";
                                }
                            }
                            else
                            {
                                foreach (var item in oldModel.DiscountContentList)
                                {
                                    newRuleValue = $"满{item.Condition}元 享 {item.DiscountRate / 10}折 ";
                                }
                            }
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "折扣方式",
                                OldValue = oldValue,
                                NewValue = newValue
                            });
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "折扣内容",
                                OldValue = oldRuleValue,
                                NewValue = newRuleValue
                            });
                        }
                        SetOperationLog(operationLogModel, "UpdateActivity");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(Level.Error, $"UpdateActivity修改活动信息操作日志异常:{ex}");
                    }
                    #endregion
                }
                else
                {
                    if (updateResult.ErrorCode == "1")
                    {
                        message = updateResult.ErrorMessage;
                    }
                    else
                    {
                        Logger.Log(Level.Warning, $"UpdateActivity修改活动失败：errormessage:{updateResult.ErrorMessage}");
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 审核后修改促销活动
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UpdateActivityAfterAudit(SalePromotionActivityModel model)
        {
            var result = false;
            using (var client = new SalePromotionActivityClient())
            {
                var oldModel = client.GetActivityInfo(model.ActivityId).Result;//更新前的活动信息
                var updateResult = client.UpdateActivityAfterAudit(model);
                if (updateResult.Success && updateResult.Result)
                {
                    //刷新活动下所有商品标签缓存
                    RefreshActivityProductDiscountTag(model.ActivityId, "UpdateActivityAfterAudit");

                    result = true;
                    DateTime.TryParse(model.StartTime, out DateTime startTime);
                    DateTime.TryParse(model.EndTime, out DateTime endTime);
                    #region 操作日志
                    try
                    {
                        var operationLogModel = new SalePromotionActivityLogModel()
                        {
                            ReferId = model.ActivityId,
                            ReferType = SalePromotionActivityConst.SalePromotionActivity,
                            OperationLogType = SalePromotionActivityConst.Update_Activity,
                            CreateDateTime = DateTime.Now.ToString(),
                            CreateUserName = model.CreateUserName,
                            //日志详情
                            LogDetailList = new List<SalePromotionActivityLogDetail>()
                        };
                        if (oldModel.StartTime != model.StartTime)
                        {
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "活动开始时间",
                                OldValue = oldModel.StartTime,
                                NewValue = startTime.ToString(),
                            });
                        }
                        if (oldModel.EndTime != model.EndTime)
                        {
                            operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                            {
                                Property = "活动终止时间",
                                OldValue = oldModel.EndTime,
                                NewValue = endTime.ToString(),
                            });
                        }
                        SetOperationLog(operationLogModel, "UpdateActivityAfterAudit");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(Level.Error, $"UpdateActivityAfterAudit操作日志记录异常{ex}");
                    }
                    #endregion
                }
                else
                {
                    Logger.Log(Level.Warning, $"UpdateActivityAfterAudit审核后修改活动失败：errormessage:{updateResult.ErrorMessage}");
                }
            }
            return result;
        }

        /// <summary>
        /// 提交审核/拒绝促销活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SetActivityAuditStatus(string activityId, int auditStatus, string remark, string userName)
        {
            var result = false;
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.SetActivityAuditStatus(activityId, userName, auditStatus, remark);
                if (clientResult.Success && clientResult.Result)
                {

                    //刷新活动下所有商品标签缓存
                    RefreshActivityProductDiscountTag(activityId, "SetActivityAuditStatus");

                    result = true;
                    //添加操作日志
                    try
                    {
                        if (auditStatus != 1)
                        {
                            var operationLogModel = new SalePromotionActivityLogModel()
                            {
                                ReferId = activityId,
                                ReferType = SalePromotionActivityConst.SalePromotionActivity,
                                OperationLogType = SalePromotionActivityConst.Reject_Audit,
                                CreateDateTime = DateTime.Now.ToString(),
                                CreateUserName = userName,
                            };
                            SetOperationLog(operationLogModel, "SetActivityAuditStatus");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(Level.Error, ex, "SetActivityAuditStatusLog");
                    }
                }
                else
                {
                    Logger.Log(Level.Warning, $"SetActivityAuditStatus提交审核/拒绝活动失败{clientResult.ErrorMessage}");
                }
            }
            return result;
        }

        /// <summary>
        /// 审核促销活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool PassAuditActivity(string activityId, string userName, out string message)
        {
            var result = false;
            message = "操作失败,请刷新重试";
            using (var client = new SalePromotionActivityClient())
            {
                var auditStatus = client.GetActivityAuditStatus(activityId);
                if (!auditStatus.Success || auditStatus.Result < 0)
                {
                    message = "获取活动审核状态失败";
                }
                else
                {
                    if (auditStatus.Result != 1)
                    {
                        message = "请先提交审核";
                    }
                    else
                    {
                        //审核
                        var clientResult = client.SetActivityAuditStatus(activityId, userName, 2, "");
                        if (clientResult.Success && clientResult.Result)
                        {
                            //刷新活动下所有商品标签缓存
                            RefreshActivityProductDiscountTag(activityId, "PassAuditActivity");

                            result = true;
                            //添加操作日志
                            try
                            {
                                var operationLogModel = new SalePromotionActivityLogModel()
                                {
                                    ReferId = activityId,
                                    ReferType = SalePromotionActivityConst.SalePromotionActivity,
                                    OperationLogType = SalePromotionActivityConst.PassAudit_Activity,
                                    CreateDateTime = DateTime.Now.ToString(),
                                    CreateUserName = userName,
                                };
                                SetOperationLog(operationLogModel, "PassAuditActivity");
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(Level.Error, ex, "PassAuditActivityLog");
                            }
                        }
                        else
                        {
                            Logger.Log(Level.Warning, $"PassAuditActivity审核活动失败{clientResult.ErrorMessage}");
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 下架活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UnShelveActivity(string activityId, string userName)
        {
            var result = false;
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.UnShelveActivity(activityId, userName);
                if (clientResult.Success && clientResult.Result)
                {

                    //刷新活动下所有商品标签缓存
                    RefreshActivityProductDiscountTag(activityId, "UnShelveActivity");

                    result = true;
                    #region 操作日志
                    try
                    {
                        var operationLogModel = new SalePromotionActivityLogModel()
                        {
                            ReferId = activityId,
                            ReferType = SalePromotionActivityConst.SalePromotionActivity,
                            OperationLogType = SalePromotionActivityConst.UnShelveActivity,
                            CreateDateTime = DateTime.Now.ToString(),
                            CreateUserName = userName,
                        };
                        SetOperationLog(operationLogModel, "UnShelveActivity");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(Level.Error, ex, "UnShelveActivityLog");
                    }
                    #endregion
                }
                else
                {
                    Logger.Log(Level.Warning, $"UnShelveActivity下架活动失败{clientResult.ErrorMessage}");
                }
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 分页查询活动列表和各个状态下活动数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SelectActivityListModel SelectActivityList(SalePromotionActivityModel model, int pageIndex, int pageSize)
        {
            var activityList = new SelectActivityListModel();
            using (var client = new SalePromotionActivityClient())
            {
                var selectResult = client.SelectActivityList(model, pageIndex, pageSize);
                if (selectResult.Success)
                {
                    activityList = selectResult.Result;
                }
                else
                {
                    Logger.Log(Level.Warning, $"SelectActivityList查询活动列表失败,errormessage:{selectResult.ErrorMessage}");
                }
            }
            return activityList;
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public SalePromotionActivityModel GetActivityInfo(string activityId)
        {
            var model = new SalePromotionActivityModel();
            using (var client = new SalePromotionActivityClient())
            {
                var result = client.GetActivityInfo(activityId);
                if (result.Success && result.Result != null)
                {
                    model = result.Result;
                }
                else
                {
                    Logger.Log(Level.Warning, $"GetActivityInfo查询活动信息失败,errormessage:{result.ErrorMessage}");
                }
            }
            return model;
        }

        /// <summary>
        /// 获取活动打折规则
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public List<SalePromotionActivityDiscount> GetActivityContent(string activityId)
        {
            var list = new List<SalePromotionActivityDiscount>();
            using (var client = new SalePromotionActivityClient())
            {
                var result = client.GetActivityContent(activityId);
                if (result.Success && result.Result != null)
                {
                    list = result.Result;
                }
                else
                {
                    Logger.Log(Level.Warning, $"GetActivityContent查询活动打折规则失败,errormessage:{result.ErrorMessage}");
                }
            }
            return list;
        }

        #region 活动商品

        /// <summary>
        /// 批量新增活动商品
        /// </summary>
        /// <param name="productList"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public bool InsertActivityProductList(List<SalePromotionActivityProduct> productList, string activityId, string userName)
        {
            bool result = false;
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.InsertActivityProductList(productList, activityId, userName);
                if (clientResult.Success && clientResult.Result)
                {
                    result = true;

                    //刷新活动下所有商品标签缓存
                    RefreshActivityProductDiscountTag(activityId, "InsertActivityProductList");
                    
                    #region 操作日志
                    try
                    {
                        var operationLogModel = new SalePromotionActivityLogModel()
                        {
                            ReferId = activityId,
                            ReferType = SalePromotionActivityConst.SalePromotionActivity,
                            OperationLogType = SalePromotionActivityConst.InsertProduct,
                            CreateDateTime = DateTime.Now.ToString(),
                            CreateUserName = userName,
                            LogDetailList = new List<SalePromotionActivityLogDetail>()
                        };
                        foreach (var psList in productList.Split(15))
                        {
                            StringBuilder newValueBuilder = new StringBuilder();
                            string newValue = string.Empty;
                            foreach (var item in psList)
                            {
                                newValueBuilder.Append($"商品PID:{item.Pid},限购库存:{item.LimitQuantity};");
                            }
                            if (newValueBuilder.Length > 0)
                            {
                                newValue = newValueBuilder.ToString().Substring(0, newValueBuilder.Length - 1);
                                operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                                {
                                    Property = "新增商品",
                                    NewValue = newValue,
                                });
                            }
                        }
                        SetOperationLog(operationLogModel, "InsertActivityProductList");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(Level.Error, ex, "InsertActivityProductListLog");
                    }
                    #endregion
                }
                else
                {
                    Logger.Log(Level.Warning, $"InsertActivityProductList新增活动商品失败,errormessage:{clientResult.ErrorMessage}");
                }
                return result;
            }
        }

        /// <summary>
        /// 从活动中移除商品
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="activityId"></param>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool DeleteProductFromActivity(string pid, string activityId, string userName, out string message)
        {
            var result = false;
            message = "操作失败,请刷新重试";
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.DeleteProductFromActivity(pid, activityId, userName);
                if (clientResult.Success && clientResult.Result > 0)
                {
                    //刷新标签缓存
                    var pids = new List<string>() { pid };
                    NotifyRefreshProductCommonTag(pids);
                    RefreshActivityProductDiscountTag(activityId, "DeleteProductFromActivity");

                    result = true;
                    //添加操作日志
                    try
                    {
                        var operationLogModel = new SalePromotionActivityLogModel()
                        {
                            ReferId = activityId,
                            ReferType = SalePromotionActivityConst.SalePromotionActivity,
                            OperationLogType = SalePromotionActivityConst.RemoveProduct,
                            CreateDateTime = DateTime.Now.ToString(),
                            CreateUserName = userName,
                            LogDetailList = new List<SalePromotionActivityLogDetail>() {
                            new SalePromotionActivityLogDetail(){
                                Property="撤除商品PID",
                                OldValue=pid,
                            },
                             }
                        };
                        SetOperationLog(operationLogModel, "DeleteProductFromActivity");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(Level.Error, ex, "InsertActivityProductListLog");
                    }
                }
                else
                {
                    Logger.Log(Level.Warning, $"DeleteProductFromActivity撤出商品失败,errormessage:{clientResult.ErrorMessage}");
                }
            }
            return result;
        }

        /// <summary>
        /// 获取活动商品库存信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public IList<SalePromotionActivityProduct> GetProductInfoList(string activityId, List<string> pidList = null)
        {
            var resultList = new List<SalePromotionActivityProduct>();
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.GetProductInfoList(activityId, pidList);
                if (clientResult.Success && clientResult.Result != null)
                {
                    resultList = clientResult.Result.ToList();
                }
                return resultList;
            }
        }

        /// <summary>
        /// 获取活动商品信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public PagedModel<SalePromotionActivityProduct> GetActivityProductInfoList(SelectActivityProduct condition, int pageIndex, int pageSize)
        {
            var resultList = new PagedModel<SalePromotionActivityProduct>();
            var activityList = new SelectActivityListModel();
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.SelectProductList(condition, pageIndex, pageSize);
                if (clientResult.Success && clientResult.Result != null)
                {
                    resultList = clientResult.Result;
                }
                return resultList;
            }
        }

        /// <summary>
        /// 获取已经存在活动中的商品列表
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public IList<SalePromotionActivityProduct> GetRepeatProductList(string activityId, List<string> pidList)
        {
            var resultList = new List<SalePromotionActivityProduct>();
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.GetRepeatProductList(activityId, pidList);
                if (clientResult.Success && clientResult.Result != null)
                {
                    resultList = clientResult.Result.ToList();
                }
                else
                {
                    Logger.Log(Level.Warning, $"获取活动商品信息失败,errormessage:{clientResult.ErrorMessage}");
                }
                return resultList;
            }
        }

        /// <summary>
        /// 获取特定时间内当前活动和其他活动重复的商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public IList<SalePromotionActivityProduct> GetActivityRepeatProductList(string activityId, string startTime, string endTime)
        {
            var resultList = new List<SalePromotionActivityProduct>();
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.GetActivityRepeatProductList(activityId, startTime, endTime);
                if (clientResult.Success && clientResult.Result != null)
                {
                    resultList = clientResult.Result.ToList();
                }
                return resultList;
            }
        }

        /// <summary>
        /// 设置活动商品限购库存
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <param name="stock"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int SetProductLimitStock(string activityId, List<string> pidList, int stock, string userName)
        {
            int result = 0;
            using (var client = new SalePromotionActivityClient())
            {
                //获取商品原先库存信息
                var oldList = client.GetProductInfoList(activityId, pidList)?.Result.ToList();
                var clientResult = client.SetProductLimitStock(activityId, pidList, stock, userName);
                if (clientResult.Success)
                {
                    result = clientResult.Result;

                    // 刷新活动商品标签缓存
                    RefreshActivityProductDiscountTag(activityId, "SetProductLimitStock");

                    if (result > 0)
                    {
                        #region 操作日志
                        try
                        {
                            var operationLogModel = new SalePromotionActivityLogModel()
                            {
                                ReferId = activityId,
                                ReferType = SalePromotionActivityConst.SalePromotionActivity,
                                OperationLogType = SalePromotionActivityConst.SetProductStock,
                                CreateDateTime = DateTime.Now.ToString(),
                                CreateUserName = userName,
                                LogDetailList = new List<SalePromotionActivityLogDetail>()
                            };
                            foreach (var psList in oldList.Split(8))
                            {
                                StringBuilder newValueBuilder = new StringBuilder($"修改后库存{stock},");
                                string newValue = string.Empty;
                                foreach (var item in psList)
                                {
                                    newValueBuilder.Append($"商品PID:{item.Pid},修改前库存:{item.LimitQuantity};");
                                }
                                if (newValueBuilder.Length > 0)
                                {
                                    newValue = newValueBuilder.ToString().Substring(0, newValueBuilder.Length - 1);
                                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                                    {
                                        Property = "批量修改商品库存",
                                        NewValue = newValue,
                                    });
                                }
                            }
                            SetOperationLog(operationLogModel, "DeleteProductFromActivity");
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(Level.Error, ex, "SetProductLimitStockLog");
                        }
                        #endregion 
                    }
                }
                else
                {
                    Logger.Log(Level.Warning, $"SetProductLimitStock设置商品库存失败,errormessage:{clientResult.ErrorMessage}");
                }
                return result;
            }
        }

        /// <summary>
        /// 设置商品列表牛皮癣
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <param name="urlImg"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int SetProductImage(string activityId, List<string> pidList, string urlImg, string userName)
        {
            int result = 0;
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.SetProductImage(activityId, pidList, urlImg, userName);
                if (clientResult.Success)
                {
                    result = clientResult.Result;

                    // 刷新活动所有商品标签缓存
                    RefreshActivityProductDiscountTag(activityId, "SetProductImage");

                    if (result > 0)
                    {
                        #region 操作日志
                        try
                        {
                            string newValue = string.Empty;
                            if (!(pidList?.Count > 0))
                            {
                                newValue = $"设置活动所有商品列表牛皮癣，图片地址:{urlImg}";
                            }
                            else
                            {

                                StringBuilder newValueBuilder = new StringBuilder($"修改图片地址:{urlImg}。");
                                foreach (var item in pidList)
                                {
                                    newValueBuilder.Append($"商品PID:{item},");
                                }
                                if (newValueBuilder.Length > 0)
                                {
                                    newValue = newValueBuilder.ToString().Substring(0, newValueBuilder.Length - 1);
                                }
                            }
                            var operationLogModel = new SalePromotionActivityLogModel()
                            {
                                ReferId = activityId,
                                ReferType = SalePromotionActivityConst.SalePromotionActivity,
                                OperationLogType = SalePromotionActivityConst.SetProductImage,
                                CreateDateTime = DateTime.Now.ToString(),
                                CreateUserName = userName,
                                LogDetailList = new List<SalePromotionActivityLogDetail>() {
                            new SalePromotionActivityLogDetail(){
                                Property="图片地址",
                                NewValue=newValue,
                            },
                             }
                            };
                            SetOperationLog(operationLogModel, "DeleteProductFromActivity");
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(Level.Error, ex, "SetProductImageLog");
                        }
                        #endregion
                    }
                }
                else
                {
                    Logger.Log(Level.Warning, $"SetProductImage设置商品列表牛皮癣失败,errormessage:{clientResult.ErrorMessage}");
                }
                return result;
            }
        }

        /// <summary>
        /// 设置商品详情页牛皮癣
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <param name="urlImg"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int SetDiscountProductDetailImg(string activityId, List<string> pidList, string urlImg, string userName)
        {
            int result = 0;

            using (var client = new SalePromotionActivityClient())
            {
                var request = new SetDiscountProductDetailImgRequest()
                {
                    ActivityId = activityId,
                    DetailImgUrl = urlImg,
                    Pid = pidList,
                    Operator = userName
                };

                //保存详情页牛皮癣,更新活动审核、下架状态
                var clientResult = client.SetDiscountProductDetailImg(request);
                if (!clientResult.Success)
                {
                    Logger.Log(Level.Warning,
                        $"SetDiscountProductDetailImg=>SetDiscountProductDetailImg失败:Message:{clientResult.ErrorMessage}");
                    return 0;
                }
                else
                {
                    if (clientResult.Result.ResponseCode == "0000")
                    {
                        result = clientResult.Result.ResponseRow;

                        // 刷新活动商品标签缓存
                        RefreshActivityProductDiscountTag(activityId, "SetDiscountProductDetailImg");

                        //操作日志
                        NoteSetDiscountProductDetailImgLog(activityId, pidList, urlImg, userName);
                    }
                    else
                    {
                        Logger.Log(Level.Warning,
                       $"SetDiscountProductDetailImg=>SetDiscountProductDetailImg失败:Message:{clientResult.Result.ResponseMessage}");
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 记录 设置商品详情页牛皮癣操作日志
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <param name="urlImg"></param>
        /// <param name="userName"></param>
        private void NoteSetDiscountProductDetailImgLog(string activityId, List<string> pidList,
            string urlImg, string userName)
        {
            try
            {
                string newValue = string.Empty;
                if (!(pidList?.Count > 0))
                {
                    newValue = $"设置活动所有商品详情页牛皮癣，图片地址:{urlImg}";
                }
                else
                {
                    StringBuilder newValueBuilder = new StringBuilder($"修改图片地址:{urlImg}。");
                    foreach (var item in pidList)
                    {
                        newValueBuilder.Append($"商品PID:{item},");
                    }
                    if (newValueBuilder.Length > 0)
                    {
                        newValue = newValueBuilder.ToString().Substring(0, newValueBuilder.Length - 1);
                    }
                }
                var operationLogModel = new SalePromotionActivityLogModel()
                {
                    ReferId = activityId,
                    ReferType = SalePromotionActivityConst.SalePromotionActivity,
                    OperationLogType = SalePromotionActivityConst.SetProductDetailImage,
                    CreateDateTime = DateTime.Now.ToString(),
                    CreateUserName = userName,
                    LogDetailList = new List<SalePromotionActivityLogDetail>() {
                            new SalePromotionActivityLogDetail(){
                                Property="图片地址",
                                NewValue=newValue,
                            },
                             }
                };

                SetOperationLog(operationLogModel, "SetDiscountProductDetailImg");
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SetDiscountProductDetailImg");
            }
        }

        /// <summary>
        /// 新增或删除活动商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="stock"></param>
        /// <param name="addList"></param>
        /// <param name="delList"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool AddAndDelActivityProduct(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName)
        {
            bool result = false;
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.AddAndDelActivityProduct(activityId, stock, addList, delList, userName);
                if (clientResult.Success)
                {
                    //刷新活动下所有商品标签缓存
                    RefreshActivityProductDiscountTag(activityId, "AddAndDelActivityProduct");

                    //刷新删除的pid标签缓存
                    NotifyRefreshProductCommonTag(delList);

                    result = clientResult.Result;
                    #region 操作日志 
                    try
                    {
                        var operationLogModel = new SalePromotionActivityLogModel()
                        {
                            ReferId = activityId,
                            ReferType = SalePromotionActivityConst.SalePromotionActivity,
                            OperationLogType = SalePromotionActivityConst.ChangeProduct,
                            CreateDateTime = DateTime.Now.ToString(),
                            CreateUserName = userName,
                            LogDetailList = new List<SalePromotionActivityLogDetail>()
                        };
                        string newValue = string.Empty;
                        if (addList?.Count > 0)
                        {
                            foreach (var psList in addList.Split(15))
                            {
                                StringBuilder newValueBuilder = new StringBuilder($"限购库存:{stock}。商品PID:");
                                foreach (var item in psList)
                                {
                                    newValueBuilder.Append($"{item.Pid},");
                                }
                                if (newValueBuilder.Length > 0)
                                {
                                    newValue = newValueBuilder.ToString().Substring(0, newValueBuilder.Length - 1);
                                }
                                operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                                {
                                    Property = "新增商品",
                                    NewValue = newValue,
                                });
                            }
                        }
                        string oldValue = string.Empty;
                        if (delList?.Count > 0)
                        {
                            foreach (var psList in delList.Split(10))
                            {
                                StringBuilder oldValueBuilder = new StringBuilder("商品PID:");
                                foreach (var item in psList)
                                {
                                    oldValueBuilder.Append($"{item},");
                                }
                                if (oldValueBuilder.Length > 0)
                                {
                                    oldValue = oldValueBuilder.ToString().Substring(0, oldValueBuilder.Length - 1);
                                }
                                operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                                {
                                    Property = "删除商品",
                                    OldValue = oldValue,
                                });
                            }
                        }
                        SetOperationLog(operationLogModel, "DeleteProductFromActivity");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(Level.Error, ex, "AddAndDelActivityProductLog");
                    }
                    #endregion
                }
            }
            return result;
        }

        /// <summary>
        /// 同步活动商品信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public bool RefreshProductInfo(string activityId, string userName, out string message)
        {
            bool result = false;
            message = "获取数据失败";
            var resultList = new List<SalePromotionActivityProduct>();
            try
            {
                using (var client = new SalePromotionActivityClient())
                {
                    var clientResult = client.GetProductInfoList(activityId, null);
                    if (clientResult.Success)
                    {
                        resultList = clientResult.Result.ToList();
                    }
                    else
                    {
                        Logger.Log(Level.Warning, $"RefreshProductInfo获取活动商品信息失败{clientResult.ErrorMessage}");
                    }
                }
                var pids = new List<string>();
                if (resultList.Count > 0)
                {
                    pids = resultList.Select(p => p.Pid).ToList();

                    //获取商品基本信息:售价、名称、成本价、库存(代发-999)
                    var productInfos = GetProductAllInfo(pids);

                    var newInfoProductList = productInfos?.Select(x => new SalePromotionActivityProduct()
                    {
                        Pid = x.Pid,
                        SalePrice = x.Price,
                        ProductName = x.ProductName,
                        TotalStock = x.TotalStock,
                        CostPrice = x.CostPrice
                    })?.ToList();

                    //同步数据到数据库
                    if (newInfoProductList?.Count > 0)
                    {
                        using (var client = new SalePromotionActivityClient())
                        {
                            var clientResult = client.RefreshProduct(activityId, newInfoProductList);
                            if (clientResult.Success)
                            {
                                result = clientResult.Result;
                            }
                            else
                            {
                                Logger.Log(Level.Warning, $"RefreshProductInfo同步活动商品信息失败，{clientResult.ErrorMessage}");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"RefreshProductInfo,同步活动商品信息异常,activityId：{activityId}，ex:{ex}");
            }
            return result;
        }

        /// <summary>
        /// 获取套装商品的库存和成本价
        /// </summary>
        /// <param name="packageIds"></param>
        /// <returns></returns>
        private List<ProductInfo> GetPackageProductSotckAndCost(List<string> packageIds)
        {
            var resultList = new List<ProductInfo>();

            try
            {
                //获取套装商品信息
                var packageProducts = GetPackageProducts(packageIds);
                var itemPids = packageProducts?.Select(a => a.Pid)?.ToList();//套装子商品pid

                //获取套装子商品信息
                var itemPidStocks = GetProductAllInfo(itemPids);

                //按套装pid分组
                var packagePidGroup = packageProducts.Join(itemPidStocks, x => x.Pid, y => y.Pid, (x, y) =>
                {
                    return new
                    {
                        x.PackagePid,
                        x.Pid,
                        x.Quantity,
                        y.IsDaiFa,
                        y.TotalStock,
                        y.CostPrice
                    };
                }).GroupBy(x => x.PackagePid);

                //遍历套装组：计算库存和成本价
                foreach (var group in packagePidGroup)
                {
                    var resultModel = new ProductInfo() { Pid = group.Key };

                    var daifaCount = group.Count(p => p.IsDaiFa);
                    //子产品全为代发商品
                    if (group.Count() == 0)
                    {
                        resultModel.TotalStock = 0;//子商品库存都不存在
                    }
                    else if (daifaCount == group.Count())
                    {
                        resultModel.TotalStock = -999;///-999表示代发商品不限制库存
                    }
                    else
                    {
                        //取非代发商品的最小库存(库存/套装件数)
                        resultModel.TotalStock = group.Where(p => !p.IsDaiFa).Min(x => x.TotalStock / x.Quantity);
                    }
                    //成本价：子商品的 数量*成本价 累加
                    resultModel.CostPrice = group.Sum(p => p.CostPrice * p.Quantity);
                    resultList.Add(resultModel);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"GetPackageProductSotckAndCost,获取套装和库存成本价异常,ex:{ex}");
            }
            return resultList;
        }

        /// <summary>
        /// 获取商品信息:售价、名称、库存、成本价
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        public List<ProductInfo> GetProductAllInfo(List<string> pids)
        {
            var resultProductInfos = new List<ProductInfo>();

            try
            {
                //获取商品名称、售价、是否代发、是否套装
                var productBaseInfos = GetProductPriceAndName(pids);
                //获取普通商品库存、成本价
                var stockAndCosts = GetProductStockAndCostListFromWCF(pids);

                var packagePids = productBaseInfos?.Where(x => (bool)x.IsPackageProduct);//套装商品
                var daiFaPids = productBaseInfos?.Where(x => x.IsDaiFa);//代发商品

                //未查询到商品信息的pid不返回
                foreach (var item in productBaseInfos)
                {
                    var stockAndCostModel = stockAndCosts?.FirstOrDefault(x => x.Pid == item.Pid);
                    var model = new ProductInfo()
                    {
                        Pid = item.Pid,
                        ProductName = item?.DisplayName,
                        Price = item?.Price ?? 0,
                        IsDaiFa = (bool)item?.IsDaiFa,
                        IsPackageProduct = (bool)item?.IsPackageProduct,
                        TotalStock = stockAndCostModel?.TotalStock ?? 0,
                        CostPrice = stockAndCostModel?.CostPrice ?? 0
                    };

                    if (model.IsDaiFa)
                    {
                        model.TotalStock = -999;//-999表示代发商品不限制库存
                    }

                    //套装商品计算库存和成本价
                    if (model.IsPackageProduct)
                    {
                        var packageProducts = GetPackageProductSotckAndCost(new List<string>() { item.Pid });
                        model.TotalStock = packageProducts?.FirstOrDefault(x => x.Pid == item.Pid)?.TotalStock ?? 0;
                        model.CostPrice = packageProducts?.FirstOrDefault(x => x.Pid == item.Pid)?.CostPrice ?? 0;
                    }
                    resultProductInfos.Add(model);
                }

            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"GetProductAllInfo,获取商品信息异常,ex:{ex}");
            }
            return resultProductInfos;
        }

        #endregion


        #region 查数据库

        /// <summary>
        /// 获取渠道列表
        /// </summary>
        /// <returns></returns>
        public List<ChannelVModel> GetChannelList()
        {
            using (var conn = ProcessConnection.OpenGungnir)
            {
                var channelList = DalSalePromotionActivity.GetChannelList(conn).ToList();
                return channelList;
            }
        }

        /// <summary>
        /// 获取渠道名称
        /// </summary>
        /// <param name="keyList"></param>
        /// <returns></returns>
        public IEnumerable<ChannelVModel> GetChannelValueByKey(List<string> keyList)
        {
            var channelList = new List<ChannelVModel>();
            channelList = DalSalePromotionActivity.GetChannelValueByKey(keyList)?.ToList();
            return channelList;
        }

        #endregion

        #region 外部接口

        /// <summary>
        /// 获取商品的库存和成本价
        /// </summary>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public List<ProductInfo> GetProductStockAndCostListFromWCF(List<string> pidList)
        {
            var result = new List<ProductInfo>();
            using (var client = new StockQueryClient())
            {
                var request = new SelectAvailableInventoryRequest() { Pids = pidList };
                var clientResult = client.SelectAvailableInventoryByBiPids(request);
                if (clientResult.Success)
                {
                    result = clientResult.Result?.Select(a => new ProductInfo()
                    {
                        Pid = a.PID,
                        TotalStock = a.TotalStock,
                        CostPrice = a.TotalCostPrice
                    })?.ToList();
                }
                else
                {
                    Logger.Log(Level.Warning, $"方法GetProductStockAndCostListFromWCF,获取商品的库存和成本价失败,pid:{string.Join(",", pidList)}," +
                        $"ErrorMessage:{clientResult.ErrorMessage}");
                }
                return result;
            }
        }

        /// <summary>
        /// 获取套装商品信息
        /// </summary>
        /// <param name="packageIds">套装商品pid</param>
        /// <returns></returns>
        private List<ProductPackageModel> GetPackageProducts(List<string> packageIds)
        {
            var resultList = new List<ProductPackageModel>();
            using (var client = new ProductClient())
            {
                var result = client.SelectProductPackage(packageIds);
                if (result.Success)
                {
                    if (result.Result?.Count() > 0)
                    {
                        resultList.AddRange(result.Result);
                    }
                }
                else
                {
                    Logger.Log(Level.Warning, $"GetPackageProducts，获取套装商品接口查询失败，ErrorMessage{result.ErrorMessage}");
                }
            }
            return resultList;
        }

        /// <summary>
        /// 获取类目的所有下级类目
        /// </summary>
        /// <param name="parentCategory"></param>
        /// <returns></returns>
        public List<RescueSelectModel> GetCatrgoryList(string parentCategory, out bool outResult)
        {
            outResult = false;
            var catelist = new ProductCategoryModel();
            var list = new List<RescueSelectModel>();
            using (var client = new ProductClient())
            {
                var clientResult = client.GetCategoryDetailLevelsByCategory(parentCategory);
                if (clientResult.Success)
                {
                    var cateModel = clientResult.Result;
                    foreach (var item in cateModel?.ChildCategorys)
                    {
                        if (item.ParentId == clientResult.Result.Id)
                        {
                            list.Add(new RescueSelectModel()
                            {
                                value = item.CategoryName,
                                label = item.DisplayName,
                                children = RescueSelectModel(cateModel.ChildCategorys.ToList(), item.Id)
                            });
                        }
                    }
                    outResult = true;
                }
                return list;
            }
        }

        /// <summary>
        /// 递归类目集合
        /// </summary>
        /// <param name="cateList"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private List<RescueSelectModel> RescueSelectModel(List<ChildCategoryModel> cateList, int parentId)
        {
            var list = new List<RescueSelectModel>();
            foreach (var item in cateList)
            {
                if (item.ParentId == parentId)
                {
                    list.Add(new RescueSelectModel()
                    {
                        value = item.CategoryName,
                        label = item.DisplayName,
                        children = RescueSelectModel(cateList, item.Id)
                    });
                }
            }
            return list;
        }

        /// <summary>
        ///根据类别获取品牌信息
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<string> GetBrandsByCategory(string category, out bool result)
        {
            var brandList = new List<string>();
            result = false;
            using (var client = new ProductClient())
            {
                var clientResult = client.GetBrandsByCategoryName(category);
                if (clientResult.Success)
                {
                    brandList = clientResult.Result;
                    result = true;
                }
                return brandList;
            }
        }

        /// <summary>
        /// 按条件查询商品信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="category"></param>
        /// <param name="brandList"></param>
        /// <param name="sizeList"></param>
        /// <param name="patternList"></param>
        /// <returns></returns>
        public PagedModel<string> SearchProduct(int pageIndex, int pageSize, string category, List<string> brandList, List<string> sizeList, List<string> patternList)
        {
            var request = new SearchProductRequest()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Parameters = new Dictionary<string, IEnumerable<string>>
                {
                    ["Category"] = new[] { category },
                    ["CP_Brand"] = brandList,
                    ["CP_Tire_Size"] = sizeList,
                    ["CP_Tire_Pattern"] = patternList,
                }
            };
            using (var client = new ProductSearchClient())
            {
                var searchResult = client.SearchProduct(request);
                if (!searchResult.Success)
                {
                    Logger.Log(Level.Warning, $"查询商品信息失败(名称、价格),errormessage:{searchResult.ErrorMessage}");
                    return null;
                }
                return searchResult?.Result;
            }
        }

        /// <summary>
        /// 根据品牌获取尺寸
        /// </summary>
        /// <returns></returns>
        public List<string> GetSizeAndPatternList(List<string> bandList, string category, out List<string> sizeList)
        {
            sizeList = new List<string>();
            using (var client = new ProductSearchClient())
            {
                var filter = client.QueryTireListFilterValues(new SearchProductRequest()
                {
                    Parameters = new Dictionary<string, IEnumerable<string>>
                    {
                        ["Category"] = new[] { category },
                        ["CP_Brand"] = bandList
                    }
                }, new List<string>() { "CP_Tire_Size", "CP_Tire_Pattern" });
                sizeList = filter.Result?.Where(p => p.Key == "CP_Tire_Size").SelectMany(p => p.Value.Where(pp => !string.IsNullOrWhiteSpace(pp.FilterName)).Select(pp => pp.FilterName)).Distinct().ToList();
                var patternList = filter.Result?.Where(p => p.Key == "CP_Tire_Pattern").SelectMany(p => p.Value.Where(pp => !string.IsNullOrWhiteSpace(pp.FilterName)).Select(pp => pp.FilterName)).Distinct().ToList();
                return patternList;
            }
        }

        /// <summary>
        /// 根据pids获取商品信息
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        public List<ProductBaseInfo> GetProductPriceAndName(List<string> pids)
        {
            var result = new List<ProductBaseInfo>();
            using (var client = new ProductInfoQueryClient())
            {
                var listResult = client.SelectProductBaseInfo(pids);
                if (listResult.Success && listResult.Result != null)
                {
                    result = listResult.Result;
                }
            }
            return result;
        }

        #endregion

        #region 审核权限

        /// <summary>
        /// 获取用户审核权限信息
        /// </summary>
        /// <param name="promotionType"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public SalePromotionAuditAuth GetUserAuditAuth(int promotionType, string userName)
        {
            var result = new SalePromotionAuditAuth();
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.GetUserAuditAuth(promotionType, userName);
                if (!clientResult.Success)
                {
                    Logger.Log(Level.Warning, $"GetUserAuditAuth,promotionType:{promotionType},userName:{userName}");
                }
                else
                {
                    result = clientResult.Result;
                }
            }
            return result;
        }

        /// <summary>
        /// 新增活动审核权限
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public int InsertAuditAuth(SalePromotionAuditAuth model, out string message)
        {
            message = string.Empty;
            int result = 0;
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.InsertAuditAuth(model);
                if (!clientResult.Success)
                {
                    if (clientResult.ErrorCode == "1")
                    {
                        message = clientResult.ErrorMessage;
                    }
                    else
                    {
                        Logger.Log(Level.Warning, $"InsertAuditAuth新增审核权限失败errormessage:{clientResult.ErrorMessage}");
                    }
                }
                else
                {
                    result = clientResult.Result;
                }
            }
            return result;
        }

        /// <summary>
        /// 删除审核权限
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public int DeleteAuditAuth(int PKID, string userName)
        {
            int result = 0;
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.DeleteAuditAuth(PKID);
                if (!clientResult.Success)
                {
                    Logger.Log(Level.Warning, $"DeleteAuditAuth删除审核权限失败，PKID：{PKID},ErrorMessage:{clientResult.ErrorMessage}");
                }
                else
                {
                    result = clientResult.Result;
                    if (result > 0)
                    {
                        #region 操作日志
                        try
                        {
                            var operationLogModel = new SalePromotionActivityLogModel()
                            {
                                ReferId = PKID.ToString(),
                                ReferType = SalePromotionActivityConst.SalePromotionActivity,
                                OperationLogType = SalePromotionActivityConst.DeleteAiditAuth,
                                CreateDateTime = DateTime.Now.ToString(),
                                CreateUserName = userName
                            };
                            SetOperationLog(operationLogModel, "DeleteAuditAuth");
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(Level.Error, ex, "DeleteAuditAuth");
                        }
                        #endregion 
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 查询审核权限列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagedModel<SalePromotionAuditAuth> SelectAuditAuthList(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize)
        {
            var result = new PagedModel<SalePromotionAuditAuth>();
            using (var client = new SalePromotionActivityClient())
            {
                var clientResult = client.SelectAuditAuthList(searchModel, pageIndex, pageSize);
                if (!clientResult.Success)
                {
                    Logger.Log(Level.Warning, $"SelectAuditAuthList查询审核权限失败,ErrorMessage:{clientResult.ErrorMessage}");
                }
                else
                {
                    result = clientResult.Result;
                }
            }
            return result;
        }

        #endregion

        #region 操作日志

        /// <summary>
        /// 获取活动的操作日志列表
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public PagedModel<SalePromotionActivityLogModel> GetOperationLogList(string activityId, int pageIndex, int pageSize)
        {
            var result = new PagedModel<SalePromotionActivityLogModel>();
            using (var client = new SalePromotionActivityLogClient())
            {
                var listResult = client.GetOperationLogList(activityId, pageIndex, pageSize);
                if (listResult.Success && listResult.Result != null)
                {
                    result = listResult.Result;
                }
            }
            return result;
        }

        public List<SalePromotionActivityLogDetail> GetOperationLogDetailList(string FPKID)
        {
            var result = new List<SalePromotionActivityLogDetail>();
            using (var client = new SalePromotionActivityLogClient())
            {
                var listResult = client.GetOperationLogDetailList(FPKID);
                if (listResult.Success && listResult.Result != null)
                {
                    result = listResult.Result.ToList();
                }
            }
            return result;
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="operationLogModel"></param>
        /// <returns></returns>
        private void SetOperationLog(SalePromotionActivityLogModel operationLogModel, string funNameString)
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

        #endregion

        /// <summary>
        /// 通知刷新标签缓存
        /// </summary>
        /// <param name="pidList"></param>
        private void NotifyRefreshProductCommonTag(List<string> pidList)
        {
            try
            {
                if (pidList?.Count > 0)
                {
                    var data = new
                    {
                        type = "RebuildCache",
                        pids = pidList,
                        tag = ProductCommonTag.Discount
                    };
                    TuhuNotification.SendNotification("notification.productModify.ProductCommonTag", data);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"NotifyRefreshProductCommonTag异常,pid:{string.Join(",", pidList)}", ex);
            }
        }

        /// <summary>
        /// 刷新活动下所有商品的打折标签缓存
        /// </summary>
        /// <param name="activityId"></param> 
        /// <param name="funName"></param>
        private void RefreshActivityProductDiscountTag(string activityId, string funName)
        {
            try
            {
                using (var client = new SalePromotionActivityClient())
                {
                    //获取活动下所有商品、刷新打折标签
                    var activityProductListResult = client.GetProductInfoList(activityId, null);
                    var activityProductList = activityProductListResult.Result;
                    if (activityProductListResult.Success)
                    {
                        if (activityProductList?.Count() > 0)
                        {
                            var pids = activityProductList.Select(a => a.Pid);
                            NotifyRefreshProductCommonTag(pids?.ToList());
                        }
                        else
                        {
                            Logger.Log(Level.Warning, $"{funName}=>RefreshActivityProductDiscountTag=>GetProductInfoList未获取到活动商品信息");
                        }
                    }
                    else
                    {
                        Logger.Log(Level.Warning, $"{funName}=>RefreshActivityProductDiscountTag=>GetProductInfoList失败,message:{activityProductListResult.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, $"{funName}=>RefreshActivityProductDiscountTag=>RefreshActivityProductDiscountTag刷新打折标签缓存异常");
            }
        }

    }


}
