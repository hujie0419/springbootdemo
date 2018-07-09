using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class ActivePageListEntity : EntityTypeConfiguration<ActivePageListEntity>
    {

        public ActivePageListEntity()
        {
            this.ToTable("dbo.ActivePageList");
            this.HasKey(_ => _.PKID);

        }

        #region Model

        /// <summary>
        /// 
        /// </summary>
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PKID
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public Guid? PKGuid
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string H5Uri
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string WWWUri
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartDate
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDate
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string BgImageUrl
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string BgColor
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string TireBrand
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? ActivityType
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string DataParames
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? MenuType
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsShowDate
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string SelKeyImage
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string SelKeyName
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? IsTireSize
        {
            set;
            get;
        }


        public string CreateorUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateDateTime
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateDateTime
        {
            set;
            get;
        }


        public string PersonCharge { get; set; }


        public string RuleDesc { get; set; }
        /// <summary>
        /// 悬浮窗 0无悬浮窗 1 静图悬浮窗 2 动图悬浮窗
        /// </summary>
        public int FloatWindow { get; set; }
        /// <summary>
        /// 悬浮窗图片
        /// </summary>
        public string FloatWindowImageUrl { get; set; }
        /// <summary>
        /// 0弹窗1链接
        /// </summary>
        public int FloatWindowJump { get; set; }
        /// <summary>
        /// 弹窗图片
        /// </summary>
        public string AlertTabImageUrl { get; set; }
        public string AlertJumpApp { get; set; }
        public string AlertJumpWxApp { get; set; }

        #endregion Model

        public string HashKey { get; set; }

        public int? CustomerService { get; set; }
        public bool IsNeedLogIn { get; set; }


    }
}
