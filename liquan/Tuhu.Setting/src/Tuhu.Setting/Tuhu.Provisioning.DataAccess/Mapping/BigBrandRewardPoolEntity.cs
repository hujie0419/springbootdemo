using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
   public class BigBrandRewardPoolEntity:EntityTypeConfiguration<BigBrandRewardPoolEntity>
    {


        public BigBrandRewardPoolEntity()
        {
            this.ToTable("dbo.BigBrandRewardPool");
            this.HasKey(_ => _.PKID);
        }

        /*
         *
      PKID INT PRIMARY KEY
               IDENTITY(1, 1) ,
      FKPKID INT
        FOREIGN KEY REFERENCES Configuration.dbo.BigBrandRewardList ( PKID ) ,--大翻盘关联的外键
	  ParentPKID INT ,--父级PKID 第一级奖励池 第二级奖励包 第三级优惠券集合或者积分或者空奖励
	  Name VARCHAR(30),--奖励池名称 
      CreateDateTime DATETIME NOT NULL ,
      LastUpdateDateTime DATETIME NOT NULL ,
      RewardType INT NOT NULL ,--奖励类型 1：优惠券 2：积分 3：无奖励 4：实物奖励 5:微信红包
      CouponGuid UNIQUEIDENTIFIER ,--优惠券GUID
      Integral INT CHECK ( Integral > 0 ) ,--积分
	  PromptType int ,--提示类型 1：弹窗 2：链接
      PromptMsg VARCHAR(100)  ,--抽中提示信息
      PromptImg VARCHAR(1000) ,--抽中提示图片
      RedirectH5 VARCHAR(300) ,--移动站跳转链接
      RedirectAPP VARCHAR(300) ,--APP跳转链接
      RedirectWXAPP VARCHAR(300) ,--小程序跳转链接
      RedirectBtnText VARCHAR(300) ,--跳转按钮的文案 
      Number INT ,--抽奖的数量
      IsDefault bit , --是否为奖池的默认抽奖项
      CreateUserName VARCHAR(100) ,--创建人
      UpdateUserName VARCHAR(100),--更新人
         */

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }

        public int? FKPKID { get; set; }

        public int? ParentPKID { get; set; }

        [NotMapped]

        public string ParentPKIDText { get; set; }


        public string Name { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }

        /// <summary>
        /// 奖励类型 1：优惠券 2：积分 3：无奖励
        /// </summary>
        public int? RewardType { get; set; }

        public Guid? CouponGuid { get; set; }

        /// <summary>
        /// 实物奖励名称
        /// </summary>
        public string RealProductName { get; set; }


        public string WxAppId { get; set; }

        public int? Integral { get; set; }
        /// <summary>
        /// 微信红包金额
        /// </summary>
        public decimal? WxRedBagAmount { get; set; }

        /// <summary>
        /// 提示类型 1：弹窗 2：链接
        /// </summary>
        public int? PromptType { get; set; }


        public string PromptMsg { get; set; }

        public string PromptImg { get; set; }

        public string RedirectH5 { get; set; }

        public string RedirectAPP { get; set; }

        public string RedirectWXAPP { get; set; }
        public string RedirectHuaWei { get; set; }
        public string RedirectBtnText { get; set; }

        /// <summary>
        /// 抽奖的数量
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// 是否为奖池的默认抽奖项
        /// </summary>
        public bool? IsDefault { get; set; }

        public string CreateUserName { get; set; }


        public string UpdateUserName { get; set; }

        public bool? Status { get; set; }


    }
}
