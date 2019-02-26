using Tuhu.Service.Activity.Server.Utils;

namespace Tuhu.Service.Activity.Server.Model
{
    public enum CreateOrderErrorCode
    {
        ActivitySatisfied = 1000,
        /// <summary>
        /// 活动太火爆了
        /// </summary>
        ServerBusy = -1001,
        /// <summary>
        /// 已超过活动限购数量
        /// </summary>
        ActivityLimit = -1002,
        /// <summary>
        /// 已超过个人最大限购数量
        /// </summary>
        UserLimit = -1003,
        /// <summary>
        /// 活动已结束
        /// </summary>
        ActivityExpired = -1004,
        /// <summary>
        /// 活动还未开始
        /// </summary>
        ActivityNotStart = -1005,
        /// <summary>
        /// 本期活动已结束，请等待后续活动开始
        /// </summary>
        WaitingNext = -1006,
        /// <summary>
        /// 产品验证失败，请返回重试
        /// </summary>
        ProductValidateFailed = -1007,
    }

    public enum CheckFlashSaleStatus
    {
        /// <summary>
        /// 验证通过
        /// </summary>
        [EnumExtension.Remark("验证通过")]
        Succeed = 1,
        /// <summary>
        /// 活动不存在或时间不对或活动不包含此PID
        /// </summary>
        [EnumExtension.Remark("抱歉，您想参加的活动飞走了")]
        NoExist = -3,

        /// <summary>
        /// 库存不足
        /// </summary>
        [EnumExtension.Remark("抱歉，您关注的商品被抢光了")]
        NoEnough = -4,

        /// <summary>
        /// 个人限购
        /// </summary>
        [EnumExtension.Remark("抱歉，您已达到购买上限")]
        MaxQuantityLimit = -5,

        /// <summary>
        /// 会场限购
        /// </summary>
        [EnumExtension.Remark("抱歉，您已参加过此活动")]
        PlaceQuantityLimit = -6,

        /// <summary>
        /// 下单失败
        /// </summary>
        [EnumExtension.Remark("交易失败，请咨询客服 400-111-8868")]
        CreateOrderFailed = -100,

        /// <summary>
        /// 普通商品单限购(每单限购)
        /// </summary>
        [EnumExtension.Remark("亲，您的购买数量超过限制啦，请修改后重新提交")]
        NormalProductLimit = -17,  

    }

    public class CreateOrderMessageDic
    {
        public static string GetMessage(CreateOrderErrorCode code)
        {
            string result = string.Empty;
            switch (code)
            {
                case CreateOrderErrorCode.ActivityExpired:
                    result = "活动已结束";
                    break;
                case CreateOrderErrorCode.ActivityLimit:
                    result = "活动太火爆，都抢光了～";
                    break;
                case CreateOrderErrorCode.ActivityNotStart:
                    result = "活动还未开始";
                    break;
                case CreateOrderErrorCode.ActivitySatisfied:
                    result = "满足活动条件";
                    break;
                case CreateOrderErrorCode.ServerBusy:
                    result = "活动太火爆了";
                    break;
                case CreateOrderErrorCode.UserLimit:
                    result = "您买的已经很多啦，下次再买吧～";
                    break;
                case CreateOrderErrorCode.WaitingNext:
                    result = "本期活动已结束，请等待后续活动开始";
                    break;
                case CreateOrderErrorCode.ProductValidateFailed:
                    result = "产品验证失败，请返回重试";
                    break;
                default:
                    result = "活动已结束";
                    break;
            }

            return result;
        }

        public static string GetFlashSaleErrorMessage(string code)
        {
            switch (code)
            {
                case "-3":
                    return "抱歉，您想参加的活动飞走了";
                case "-4":
                    return "抱歉，您关注的商品被抢光了";
                case "-5":
                    return "抱歉，您已达到购买上限";
                case "-6":
                    return "抱歉，您已参加过此活动";
                case "-7":
                    return "仅限新用户购买！";
                default:
                    return "交易失败，请咨询客服 400-111-8868";

            }
        }
    }
    public enum LimitType
    {
        PersonalLimit = 3,
        PlaceLimit = 2,
        AllPlaceLimit=4
    }
    public enum CounterKeyType
    {
        UserIdKey = 1,
        DeviceIdKey = 2,
        UserTelKey=3
    }
}
