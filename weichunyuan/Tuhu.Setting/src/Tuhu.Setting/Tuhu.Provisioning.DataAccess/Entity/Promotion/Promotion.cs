using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class Promotion
    {
        /// <summary>
        /// 优惠券类型
        /// </summary>
        public enum PromotionType
        {
            /// <summary>
            /// 轮胎保养优惠券
            /// </summary>
            TireBaoYang = 6,

            /// <summary>
            /// 轮胎优惠券
            /// </summary>
            Tire = 1,

            /// <summary>
            /// 保养优惠券
            /// </summary>
            BaoYang = 2,

            /// <summary>
            /// 洗车
            /// </summary>
            WashCar = 3,

            /// <summary>
            /// 全品通用券，丁丁说跟轮胎保养券一样
            /// </summary>
            All = 0,

            /// <summary>
            /// 保养免单券
            /// </summary>
            BaoYangFree = 11,

            /// <summary>
            /// 途虎现金券
            /// </summary>
            TuHuCash = 12,
            /// <summary>
            /// 保养和车品都支持
            /// </summary>
            BaoYangOrChePin = 13
        }

        /// <summary>
        /// 优惠券状态
        /// </summary>
        public enum PromotionStatus
        {
            /// <summary>
            /// 未使用
            /// </summary>
            UnUserd = 0,

            /// <summary>
            /// 已使用
            /// </summary>
            Userd = 1,

            /// <summary>
            /// 未激活
            /// </summary>
            UnActived = 2
        }
    }
}
