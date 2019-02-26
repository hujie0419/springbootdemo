using System;
using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 会员权益奖励对象
    /// </summary>
    public class UserPromotionCodeModel:BaseModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 小图片
        /// </summary>
        public string SImage { get; set; }

        /// <summary>
        /// 大图片
        /// </summary>
        public string BImage { get; set; }

        /// <summary>
        /// 规则Id
        /// </summary>
        public int RuleID { get; set; }


        /// <summary>
        /// 用户等级（旧数据库字段，版本固定后删除）
        /// </summary>
        public string UserRank { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string CouponName { get; set; }

        /// <summary>
        /// 优惠券描述
        /// </summary>
        public string CouponDescription { get; set; }

        /// <summary>
        /// 会员等级外键(以此字段为主)
        /// </summary>
        public int MembershipsGradeId { get; set; }

        /// <summary>
        /// 奖励（礼品）名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 会员权益表外键
        /// </summary>
        public int UserPermissionId { get; set; }

        /// <summary>
        /// 权益类型
        /// </summary>
        public int PermissionType { get; set; }


        /// <summary>
        /// 奖励（礼品）类型 1 优惠券  2 积分
        /// </summary>
        public int RewardType { get; set; }

        /// <summary>
        /// 奖励类型
        /// </summary>
        public string StrRewardType
        {
            get
            {
                switch (RewardType)
                {
                    case 1:
                        return "优惠券";
                    case 2:
                        return "积分";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 奖励（礼品)Id
        /// </summary>
        public string RewardId { get; set; }

        /// <summary>
        /// 奖励值（如积分）
        /// </summary>
        public string RewardValue { get; set; }

        /// <summary>
        /// 排序序列
        /// </summary>
        public int SortIndex { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string StrLastUpdateDateTime { get { return LastUpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss"); } }
        /// <summary>
        /// 最后修改人
        /// </summary>
        public string LastUpdateBy { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
