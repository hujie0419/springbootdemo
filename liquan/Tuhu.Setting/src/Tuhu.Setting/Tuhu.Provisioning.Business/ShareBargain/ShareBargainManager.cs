using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.DAO.ShareBargain;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Activity;

namespace Tuhu.Provisioning.Business.ShareBargain
{
    public class ShareBargainManager
    {
        public static IEnumerable<BackgroundThemeModel> GetBackgroundTheme()
            => DALShareBargain.GetBackgroundTheme();

        public static IEnumerable<ShareBargainItemModel> SelectBargainProductList(BargainProductRequest request, PagerModel pager)
        {
            return DALShareBargain.SelectBargainProductList(request, pager);
        }

        public static bool UpdateGlobalConfig(BargainGlobalConfigModel request)
            => DALShareBargain.UpdateGlobalConfig(request);
        public static BargainGlobalConfigModel FetchBargainProductGlobalConfig()
            => DALShareBargain.FetchBargainProductGlobalConfig();

        public static ShareBargainProductModel FetchBargainProductById(int apId)
            => DALShareBargain.FetchBargainProductById(apId);

        public static int AddSharBargainProduct(ShareBargainProductModel request, string Operator)
            => DALShareBargain.AddSharBargainProduct(request, Operator);

        public static int AddSharBargainCoupon(ShareBargainProductModel request, string Operator)
            => DALShareBargain.AddSharBargainProduct(request, Operator);

        public static CheckPidResult CheckPid(string PID, DateTime beginDateTime, DateTime endDateTime)
        {
            var result = new CheckPidResult();
            var dat = DALShareBargain.CheckBargainProductByPid(PID, beginDateTime, endDateTime);
            if (dat)
            {
                result.Code = 2;
                result.Info = "该商品已在活动中";
            }
            else
            {
                result = DALShareBargain.CheckProductByPid(PID);
            }
            return result;
        }

        //检查优惠券id是否可用
        public static CheckPidResult CheckCouponPid(string PID, DateTime beginDateTime, DateTime endDateTime)
        {
            var result = new CheckPidResult();
            var dat = DALShareBargain.CheckBargainProductByPid(PID, beginDateTime, endDateTime);
            if (dat)
            {
                result.Code = 2;
                result.Info = "该优惠券已在活动中";
            }
            else
            {
                using (var memberClient = new Tuhu.Service.Member.PromotionClient())
                {
                    if (!Guid.TryParse(PID, out Guid rr))
                    {
                        result.Code = 4;
                        result.Info = "优惠券pid格式错误";
                        return result;
                    }

                    var couponInfo = memberClient.GetCouponRule(new Guid(PID));
                    if (!couponInfo.Success)
                    {
                        result.Code = 3;
                        result.Info = "获取优惠券信息失败";
                        return result;
                    }

                    result.Code = 1;
                    result.Info = couponInfo.Result.PromotionName;
                }
            }
            return result;
        }

        public static bool UpdateBargainProductById(ShareBargainProductModel request)
            => DALShareBargain.UpdateBargainProductById(request);

        /// <summary>
        /// 检查砍价商品是否可重新上架
        /// </summary>
        /// <param name="activityProductID"></param>
        /// <returns></returns>
        public static Tuple<bool,DateTime?> CheckProductBackOn(int activityProductID)
        {
            var result = true;
            DateTime? lastDateTime = default(DateTime);

            using (var client = new ShareBargainClient())
            {
                var request = new Service.Activity.Models.Requests.GetShareBargainUserParticipantInfoRequest() {
                    ActivityProductID = activityProductID
                };
                var clientResult = client.GetShareBargainUserParticipantInfo(request);
                if (clientResult.Success)
                {
                    if (clientResult.Result?.BargainUserCount > 0)
                    {
                        result = false;
                        lastDateTime = clientResult.Result?.LastUserEndDateTime;
                    }
                }
            }

            return new Tuple<bool, DateTime?>(result,lastDateTime) ;
        }

        public static bool UpdateBargainCouponById(ShareBargainProductModel request)
          => DALShareBargain.UpdateBargainProductById(request);

        public static bool DeleteBargainProductById(int PKID)
            => DALShareBargain.DeleteBargainProductById(PKID);
    }
}
