using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Service.Activity.Models
{
    public class BigBrandPoolModel
    {
        public int PKID { get; set; }

        public int Count { get; set; }

    }

    public class BigBrandRewardLogModel
    {
        /*
           PKID INT PRIMARY KEY
               IDENTITY(1, 1) ,
      CreateDateTime DATETIME NOT NULL ,--抽奖的时间
      LastUpdateDateTime DATETIME NOT NULL DEFAULT(GETDATE()) ,
	  Refer VARCHAR(3000),--请求地址
	  FKPKID INT NOT NULL,--抽中的奖励包的PKID
	  ChanceType INT ,--1:正常抽奖 2：分享
	  UserId UNIQUEIDENTIFIER,--用户的UserId
	  Phone VARCHAR(20),--手机号
	  [Status] BIT ,--抽奖的状态 1:有效 0：作废
	  Channel VARCHAR(20),  --渠道信息
	  DeviceSerialNumber VARCHAR(100),--设备编号
	  Remark VARCHAR(100) --备注信息
         */

        public DateTime CreateDateTime { get; set; }

        public bool Status { get; set; }

        public int PKID { get; set; }

        public string Refer { get; set; }

        /// <summary>
        /// 抽中的奖励包的PKID
        /// </summary>
        public int FKPKID { get; set; }

        /// <summary>
        /// 1:正常抽奖 2：分享 3：错题分享，4：单纯增加抽奖次数，支持无限次抽奖
        /// </summary>
        public int ChanceType { get; set; }

        /// <summary>
        /// 用户的UserId
        /// </summary>
        public Guid UserId { get; set; }
        public string UnionId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        public string Channel { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceSerialNumber { get; set; }

        public string Remark { get; set; }

        public int FKBigBrandPkid { get; set; }

        /// <summary>
        /// 领取优惠券的codeId
        /// </summary>
        public string PromotionCodePKIDs { get; set; }

    }

    public class BigBrandPackerModel
    {
        public string Name { get; set; }

        public string Phone { get; set; }
    }

    public class BigBrandPackerLog
    {
        /// <summary>
        /// 奖励池的PKID
        /// </summary>
        public int FKPKID { get; set; }

        /// <summary>
        /// 领取总数                 
        /// </summary>
        public int Number { get; set; }
    }

}
