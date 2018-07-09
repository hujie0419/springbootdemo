using Common.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tuhu.Provisioning.Business.ActivityCalendar;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Comment.Request;

namespace Tuhu.Provisioning.Business.ZeroActivity
{
    public static class ZeroActivityManager
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ZeroActivityManager));

        /// <summary>
        /// 查询0元购活动详情
        /// </summary>
        /// <returns></returns>
        public static List<ZeroActivityDetail> SelectZeroActivityDetail()
        {
            return DalZeroActivity.SelectZeroActivityDetail();
        }


        /// <summary>
        /// 选取所有的已申请0元购的详情
        /// </summary>
        /// <returns></returns>
        public static List<ZeroActivityApply> SelectAllZeroActivity()
        {
            return DalZeroActivity.SelectAllZeroActivityApply();
        }

        /// <summary>
        /// 审核通过试用报告
        /// </summary>
        /// <param name="OrderID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static int UpdateZAAStatus(int OrderID, string UserID)
        {
            return DalZeroActivity.UpdateZAAStatus(OrderID, UserID);
        }
        /// <summary>
        /// 0元购活动申请 成功获得轮胎的处理
        /// </summary>
        /// <param name="Period"></param>
        /// <param name="UserID"></param>
        /// <param name="UserMobileNumber"></param>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public static int ZeroAward(int Period, string UserID, string UserMobileNumber, int OrderID)
        {
            return DalZeroActivity.ZeroAward(Period, UserID, UserMobileNumber, OrderID);
        }
        /// <summary>
        /// 0元购活动申请详情  以及筛选
        /// </summary>
        /// <param name="Period"></param>
        /// <param name="OrderQuantity"></param>
        /// <param name="Succeed"></param>
        /// <param name="ReportStatus"></param>
        /// <returns></returns>
        public static List<ZeroActivityApply> ZeroConditionFilter(ZeroActivityApply filtermodel, int CurrentPage, int PageSize, out int TotalCount)
        {
            return DalZeroActivity.ZeroConditionFilter(filtermodel, CurrentPage, PageSize, out TotalCount);
        }
        /// <summary>
        /// 0元购活动配置
        /// </summary>
        /// <param name="Zadetail"></param>
        /// <returns></returns>
        public static int ZAConfigureAct(ZeroActivityDetail Zadetail)
        {
            return DalZeroActivity.ZAConfigureAct(Zadetail);
        }
        public static int ZAConfigureDelete(int period)
        {
            return DalZeroActivity.ZAConfigureDelete(period);
        }

        public static void SelectDataForActivityFromZaConfigures()
        {
            string pkidStr = string.Empty;
            //获取活动表中已有的套餐信息
            List<Tuhu.Provisioning.DataAccess.Entity.ActivityCalendar> listAc = new ActivityCalendarManager().SelectActivityByCondition(string.Format(" and DataFrom='{0}'", Tuhu.Provisioning.DataAccess.Entity.ActivityObject.TblZeroActivity.ToString()));
            //拼接已录入的活动信息,将来在套餐信息表中排除
            if (listAc.Any())
            {
                pkidStr = listAc.Where(_ => _.DataFromId != null).Aggregate(pkidStr, (current, item) => current + (item.DataFromId.ToString() + ','));
                pkidStr = pkidStr.Substring(0, pkidStr.Length - 1);
            }
            //获取套餐信息表中新增的套餐信息
            var listZaConfigue = SelectZeroActivityDetail().FindAll(delegate (ZeroActivityDetail info)
            {
                if (!pkidStr.Split(',').Contains(info.Period.ToString(CultureInfo.InvariantCulture)))
                {
                    return true;
                }
                return false;
            });
            #region 没有的数据要添加
            //为更新准备字符穿
            var dataFromIdStr = SelectZeroActivityDetail().Select(n => n.Period).ToList();
            //向活动日历信息表添加数据
            foreach (var item in listZaConfigue)
            {
                var modelAc = new Tuhu.Provisioning.DataAccess.Entity.ActivityCalendar();
                modelAc.BeginDate = item.StartDateTime;
                modelAc.EndDate = item.EndDateTime;
                modelAc.ActivityTitle = "0元购";
                modelAc.ActivityContent = item.ProductID + ",提供数量：" + item.Quantity;
                modelAc.CreateDate = item.CreateDateTime;
                modelAc.CreateBy = "SYSTEM";
                modelAc.DataFrom = Tuhu.Provisioning.DataAccess.Entity.ActivityObject.TblZeroActivity.ToString();
                modelAc.DataFromId = item.Period;
                modelAc.IsActive = true;
                new ActivityCalendarManager().AddActivityCalendar(modelAc);

            }

            #endregion

            #region 更新活动日历数据状态

            var updateList = listAc.FindAll(delegate (Tuhu.Provisioning.DataAccess.Entity.ActivityCalendar model)
            {
                if (!dataFromIdStr.Contains(model.Pkid))
                {
                    return true;
                }
                return false;
            });
            foreach (var item in updateList)
            {
                new ActivityCalendarManager().UpdateIsActivity(item.DataFromId.HasValue ? item.DataFromId.Value : 0, Tuhu.Provisioning.DataAccess.Entity.ActivityObject.TblZeroActivity.ToString(), false);
            }

            #endregion
        }

        public static List<ZeroActivityApply> SelectZeroActivityApplyByPeriod(int period)
        {
            return DalZeroActivity.SelectZeroActivityApplyByPeriod(period);
        }

        public static int UpdateStatusByPeriod(int period)
        {
            return DalZeroActivity.UpdateStatusByPeriod(period);
        }

        public static Tuple<bool, string> CreateProductComment(Guid userId, int orderId, int pkid, string title,
            string content, string imgStr)
        {
            var result = false;
            var msg = string.Empty;
            try
            {
                if (pkid > 0)
                {
                    var info = DalZeroActivity.SelectZeroActivityDetail(pkid);
                    if (info != null && info.UserID == userId && info.OrderId == orderId)
                    {
                        var orderList = OrderService.FetchOrderAndListByOrderId(orderId);
                        var getResult = CommentService.CreateProductComment(new CreateProductCommentRequest()
                        {
                            ProductFamilyId = info.PID.Split('|').FirstOrDefault(),
                            ProductId = info.PID,
                            CommentImages = imgStr.TrimStart(';'),
                            CommentContent = content,
                            CommentType = 3,
                            OrderListId = orderList?.OrderListModel?.FirstOrDefault()?.OrderListId ?? 0,
                            OrderId = info.OrderId,
                            CommentR1 = 5,
                            CommentR2 = 5,
                            CommentR3 = 5,
                            CommentR4 = 5,
                            CommentR5 = 5,
                            CommentStatus = 1,
                            UserName = info.UserName,
                            UserId = userId,
                            SingleTitle = title
                        });
                        result = getResult.IsSuccess;
                        msg = getResult.ErrorMessage;
                    }
                    else
                    {
                        msg = "信息异常";
                    }
                }
            }
            catch (Exception ex)
            {
                msg = "系统异常";
                logger.Error(ex);
            }
            return Tuple.Create(result, msg);
        }
    }
}
