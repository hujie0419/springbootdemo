using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{


    public class BigBrandRewardListEntity:EntityTypeConfiguration<BigBrandRewardListEntity>
    {

        public BigBrandRewardListEntity()
        {
            this.ToTable("dbo.BigBrandRewardList");
            this.HasKey(_ => _.PKID);
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }

        public string HashKeyValue { get; set; }

        public DateTime? CreateDateTime { get; set; }


        public DateTime? LastUpdateDateTime { get; set; }

        public string Title { get; set; }

        public int PreTimes { get; set; }

        public int CompletedTimes { get; set; }


        public int? NeedIntegral { get; set; }

        public int BigBrandType { get; set; }


        public int Period { get; set; }

        public int PeriodType { get; set; }


        public string CreateUserName { get; set; }


        public string UpdateUserName { get; set; }

        /// <summary>
        /// 周期开始时间
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// 后置登录，抽奖验证依赖类型,1.微信授权
        /// </summary>
        public Int16? AfterLoginType { get; set; }
        /// <summary>
        /// 后置登录，抽奖验证依赖值1.（WX_APP_OfficialAccount|WX_OA_NewCarMagazine）多个用逗号隔开
        /// </summary>
        public string AfterLoginValue { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Is_Deleted { get; set; }

    }

    /// <summary>
    /// 抽奖轮数对应的奖池信息
    /// </summary>
    public class BigBrandWheelEntity:EntityTypeConfiguration<BigBrandWheelEntity>
    {
        public BigBrandWheelEntity()
        {
            this.ToTable("dbo.BigBrandWheel");
            this.HasKey(_=>_.PKID);
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }

        public int TimeNumber { get; set; }

        /// <summary>
        /// 组合信息
        /// </summary>
        [NotMapped]
        public string FKPoolPKIDText { get; set; }


        public int FKPoolPKID { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }

        public string CreateUserName { get; set; }

        public string UpdateUserName { get; set; }

        public int FKBigBrand { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Is_Deleted { get; set; }

    }

}
