using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.UserShareJob.Model
{
    public class ShareUserModel
    {
        public Guid UserId { get; set; }

    }
    public class UserShareAwardmodel
    {
        //RNU.RecommendUserId UserId,RIO.Integral,RNU.IsCompleteOrder,RNU.IsGetPlace,RIO.PromotionMoney
        /// <summary>
        /// 分享者
        /// </summary>
        public Guid UserId { get; set; }
        public Guid? RecommendUserId { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public int? Integral { get; set; }
        /// <summary>
        /// 优惠券金额
        /// </summary>
        public decimal? PromotionMoney { get; set; }
        /// <summary>
        /// 是否老表里的数据
        /// </summary>
        public bool IsFromOldTalbe { get; set; }
        /// <summary>
        /// 订单是否完成
        /// </summary>
        public bool IsCompleteOrder { get; set; }
        /// <summary>
        /// 是否获取注册积分
        /// </summary>
        public bool IsGetPlace { get; set; }
        /// <summary>
        /// 奖励
        /// </summary>
        public decimal Award { get; set; }
    }

    public class UserShareRankingModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string UserNickName { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserHeadImg { get; set; }
        /// <summary>
        /// 总奖励
        /// </summary>
        public decimal TotalReward { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public class SendCodeForUserGroupModel
    {
        public int ID { get; set; }
        public int GroupId { get; set; }
        public Guid UserId { get; set; }
        public string SendCode { get; set; }
        public Guid? GetUserId { get; set; }
        public string GetUserName { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public string UserPhone { get; set; }
    }
}
