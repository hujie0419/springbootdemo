using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class FightGroupsPacketsLogModel
    {
        /*
        [PKID] [INT] PRIMARY KEY IDENTITY(1, 1),
        [CreateDateTime] [datetime] NOT NULL,
        [LastUpdateDateTime] [datetime] NOT NULL,
        FightGroupsIdentity UNIQUEIDENTIFIER NOT NULL,--拼团组
        UserId UNIQUEIDENTIFIER ,
        GetRuleGuid UNIQUEIDENTIFIER NOT NULL,--优惠券领取的规则的GUID
        IsLeader BIT NOT NULL,--是否开团人
        PromotionPKID INT ,--发放优惠券的PKID
        OrderBy INT NOT NULL--排序
         */

        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 最后一次更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 拼团组
        /// </summary>
        public Guid FightGroupsIdentity { get; set; }

        /// <summary>
        /// 用户的UserId
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// 优惠券领取的规则的GUID
        /// </summary>
        public Guid GetRuleGuid { get; set; }

        /// <summary>
        /// 是否开团人
        /// </summary>
        public bool IsLeader { get; set; }

        /// <summary>
        /// 发放优惠券的PKID
        /// </summary>
        public int? PromotionPKID { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderBy { get; set; }
    }
}
