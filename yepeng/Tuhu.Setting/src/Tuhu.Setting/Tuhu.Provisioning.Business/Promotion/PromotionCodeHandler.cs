using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ThBiz.Common.Entity;
using ThBiz.Common.OrderEnum;
using TheBiz.Common.Promotion;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.BeautyService;
using Tuhu.Provisioning.Business.Monitor;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Order.Request;

namespace Tuhu.Provisioning.Business.PromotionCodeManagerment
{
    internal class PromotionCodeHandler
    {
        #region Private Fields

        //private readonly IConnectionManager _connectionManager;
        private readonly IDBScopeManager dbManager;

        #endregion

        #region Ctor

        
        internal PromotionCodeHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        #endregion

        #region Public Method

        public List<BizPromotionCode> SelectPromotionCodesByUserId(string userId)
        {
            ParameterChecker.CheckNull(userId, "UserId");
            
            return dbManager.Execute(connection => DalPromotionJob.SelectPromotionCodesByUserId(userId));
        }

        /// <summary>
        /// 根据订单号查询优惠券
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<BizPromotionCode> SelectPromotionByOrderId(int OrderId)
        {

            ParameterChecker.CheckNull(OrderId, "OrderId");
            return dbManager.Execute(connection => DalPromotionJob.SelectPromotionByOrderId(OrderId));
        }
        /// <summary>
        /// 获得一个订单使用的优惠券数量
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public int GetOrderUsedPromtionCodeNumByOrderId(int orderId)
        {
            ParameterChecker.CheckNull(orderId, "OrderId");
            
            return dbManager.Execute(connection => DalPromotionJob.GetOrderUsedPromtionCodeNumByOrderId(connection, orderId));
        }

        #endregion

        public DataTable SelectExchangeCodeDetailByPage(int PageNumber, int PageSize, out int TotalCount)
        {
            return DalPromotionJob.SelectExchangeCodeDetailByPage(PageNumber, PageSize, out TotalCount);
        }

        public DataTable SelectGiftBag(int PageNumber, int PageSize, out int TotalCount)
        {
            return DalPromotionJob.SelectGiftBag(PageNumber, PageSize, out TotalCount);
        }

        public DataTable SelectGiftBagByPKID(int pkid)
        {
            return DalPromotionJob.SelectGiftBagByPKID(pkid);
        }
        public DataTable SelectGiftByDonwLoad(int pkid)
        {
            return DalPromotionJob.SelectGiftByDonwLoad(pkid);
        }


        public int SelectDownloadByPKID(int pkid)
        {
            return DalPromotionJob.SelectDownloadByPKID(pkid);
        }

        public DataTable GetEdit(int pkid)
        {
            return DalPromotionJob.GetEdit(pkid);
        }

        public DataTable UpdateOEM(int pkid)
        {
            return DalPromotionJob.UpdateOEM(pkid);
        }
        public int UpdateGift(ExchangeCodeDetail ecd)
        {
            return DalPromotionJob.UpdateGift(ecd);
        }

        public int DoUpdateOEM(ExchangeCodeDetail ecd)
        {
            return DalPromotionJob.DoUpdateOEM(ecd);
        }

        public int AddGift(ExchangeCodeDetail ecd)
        {
            return DalPromotionJob.AddGift(ecd);
        }

        public object SelectCodeChannelByAddGift(int id)
        {
            return DalPromotionJob.SelectCodeChannelByAddGift(id);
        }

        public int AddOEM(ExchangeCodeDetail ecd)
        {
            return DalPromotionJob.AddOEM(ecd);
        }

        public string GenerateCoupon(int Number, int DetailsID)
        {
            return DalPromotionJob.GenerateCoupon(Number, DetailsID);
        }

        public DataTable CreateExcel(int pkid)
        {
            return DalPromotionJob.CreateExcel(pkid);
        }

        public int DeletePromoCode(int pkid)
        {
            return DalPromotionJob.DeletePromoCode(pkid);
        }
        public int SelectPromoCodeCount(int pkid)
        {
            return DalPromotionJob.SelectPromoCodeCount(pkid);
        }

        public int DeleteGift(int pkid)
        {
            return DalPromotionJob.DeleteGift(pkid);
        }

        public DataSet SelectByPhoneNum(string PhoneNum)
        {
            return DalPromotionJob.SelectByPhoneNum(PhoneNum);
        }


        /// <summary>
        /// 释放优惠券券（根据优惠券PKID），清空订单的PromotionMoney,清空OrderList PromotionCode.PromotionMoney
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="codePkid"></param>
        public void ReleasePromtionByPromotionId(int orderId, int codePkid)
        {
            using (var client = new PromotionClient())
            {
                var result = client.ReleasePromtionByOrderId(orderId, ThreadIdentity.Operator.Name);
                if (!result.Success)
                {
                    ExceptionMonitor.AddNewMonitor("Order", orderId.ToString(), result.Exception.ToString(),
                         ThreadIdentity.Operator.Name, "调用ReleasePromtionByPromotionId方法报错", MonitorLevel.Critial, MonitorModule.Order);
                }
            }

            new OprLogManager().AddOprLog("Order", orderId, codePkid + "已使用", codePkid + "未使用", "释放优惠卷，优惠券PKID为：" + codePkid);
        }

        /// <summary>
        /// 查询优惠券下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable SelectDropDownList()
        {
            return DalPromotionJob.SelectDropDownList();
        }

        /// <summary>
        /// 创建优惠券
        /// </summary>
        /// <param name="ecd"></param>
        /// <returns></returns>
        public int CreeatePromotion(ExchangeCodeDetail ecd)
        {
            return DalPromotionJob.CreeatePromotion(ecd);
        }

        /// <summary>
        /// 优惠券列表详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public DataTable SelectPromotionDetails(int pkid)
        {
            return DalPromotionJob.SelectPromotionDetails(pkid);
        }

        /// <summary>
        /// 查询优惠券详情-->修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable SelectPromotionDetailsByEdit(int id)
        {
            return DalPromotionJob.SelectPromotionDetailsByEdit(id);
        }

        /// <summary>
        /// 修改优惠券
        /// </summary>
        /// <param name="ecd"></param>
        /// <returns></returns>
        public int UpdatePromotionDetailsByOK(ExchangeCodeDetail ecd)
        {
            return DalPromotionJob.UpdatePromotionDetailsByOK(ecd);
        }


        /// <summary>
        /// 创建优惠券执行任务
        /// </summary>
        /// <param name="promotionTask">优惠券任务对象</param>
        /// <param name="operateBy">操作者</param>
        /// <param name="cellPhones">需要发券的用户列表</param>
        /// <returns></returns>
        public int CreatePromotionTask(PromotionTask promotionTask, string operateBy, List<string> TaskPromotionListIds = null, List<string> cellPhones = null)
        {
            return DalPromotionJob.CreateOrUpdatePromotionTask(promotionTask, operateBy, TaskPromotionListIds, cellPhones);
        }

        /// <summary>
        /// 根据优惠券ID查询优惠券名称
        /// </summary>
        /// <param name="promotionRuleId"></param>
        /// <returns></returns>
        public string GetPromotionRuleNameById(int promotionRuleId)
        {
            return DalPromotionJob.GetPromotionRuleNameById(promotionRuleId);
        }


        public BizPromotionCode ConvertPromotionCode(PromotionOrderEntity entity)
        {
            var promotionCode = new BizPromotionCode()
            {
                UserId = entity.UserId.ToString(),
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Status = 0,
                Description = entity.Description,
                Discount = entity.Discount,
                MinMoney = entity.MinMoney,
                CodeChannel = entity.CodeChannel,
                RuleID = entity.RuleID,
                RuleName = entity.PromotionName,
                OrderId = entity.OrderId,
                CouponRulesId = entity.CouponRulesId
            };

            return promotionCode;

        }

        /// <summary>
        /// 根据原优惠券，创建新优惠券，并且插入日志
        /// </summary>
        /// <param name="promotionId"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private string CreateNewPromotionByOldPromotion(int promotionId, string operation = "部分取消")
        {
            if (promotionId > 0)
            {
                var oldPromotionCode = dbManager.Execute(connection => DalPromotionJob.FetchPromotionCodeByPromotionCode(connection, promotionId));
                oldPromotionCode.RuleName = oldPromotionCode.PromtionName;
                if (oldPromotionCode != null)
                {
                    oldPromotionCode.Status = 0;
                    oldPromotionCode.CodeChannel = oldPromotionCode.CodeChannel + "(取消订单)";
                    CreatePromotionModel createPromotionModel = new CreatePromotionModel()
                    {
                        Author = ThreadIdentity.Operator.Name,
                        Channel = "取消订单",
                        Operation = "原优惠券生成新优惠券",
                        PromotionPKID = oldPromotionCode.PkId
                    };
                    using (var client = new PromotionClient())
                    {
                        var result = client.CopyPromotionCode(createPromotionModel);
                        if (result.Success)
                        {
                            new OprLogManager().AddOprLog<BizPromotionCode>("Order", oldPromotionCode.OrderId, operation + "原优惠券不释放，生成一张一样的优惠券，优惠券PKID：" + result.Result, null, null);
                        }
                        else
                        {
                            throw new Exception("根据原优惠券生成新优惠券失败，订单号：" + oldPromotionCode.OrderId);
                        }
                    }
                    //var newPromotionCode = dbManager.Execute(connection => DalPromotionJob.CreatePromotionCode(connection, oldPromotionCode));
                    //new OprLogManager().AddOprLog<BizPromotionCode>("Order", oldPromotionCode.OrderId, operation + "原优惠券不释放，生成一张一样的优惠券，优惠券PKID：" + newPromotionCode, null, null);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 调用服务，查询优惠券
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public PromotionCodeModel FetchPromotionCodeByOrderIdForService(int orderId)
        {
            //订单取消，不释放优惠券
            using (var client = new PromotionClient())
            {
                var result = client.FetchPromotionCodeByOrderId(orderId);

                if (!result.Success)
                {
                    throw result.Exception;
                }
                return result.Result;
            }
        }

    }
}
