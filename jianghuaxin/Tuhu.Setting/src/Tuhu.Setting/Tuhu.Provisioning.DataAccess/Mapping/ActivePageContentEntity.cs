using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class ActivePageContentEntity : EntityTypeConfiguration<ActivePageContentEntity>
    {

        public ActivePageContentEntity()
        {
            this.ToTable("dbo.ActivePageContent");
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
        public int? FKActiveID
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string GROUP
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? Type
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Channel
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsUploading
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? DisplayWay
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string PID
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string ProductName
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public Guid? ActivityID
        {
            set;
            get;
        }

        public string HashKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? ActivityPrice
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string TireSize
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public Guid? CID
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Image
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? RowType
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string LinkUrl
        {
            set;
            get;
        }

        /// <summary>
        /// 小程序链接
        /// </summary>
        public string WXAPPUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string APPUrl
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string PCUrl
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? OrderBy
        {
            set;
            get;
        }
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
        #endregion Model

        public bool? IsRecommended { get; set; }

        public bool? IsLogin { get; set; }

        public bool? IsTireStandard { get; set; }

        public bool? IsTireSize { get; set; }

        public bool? IsHiddenTtile { get; set; }

        public string ByService { get; set; }

        public string ByActivityID { get; set; }

        public string APPUri { get; set; }

        public int? Vehicle { get; set; }

        public int? OthersType { get; set; }

        /// <summary>
        /// 0:轮胎 1：车品 2：轮毂
        /// </summary>
        public int? ProductType { get; set; }

        /// <summary>
        /// 2 :一行两列 3:一行三列
        /// </summary>
        public int? ColumnNumber { get; set; }

        /// <summary>
        /// 适配过滤 1：是 0：否
        /// </summary>
        public bool? IsAdapter { get; set; }

        /// <summary>
        /// 1:适配 2分期 3赠品 4安装服务 （以逗号分割）
        /// </summary>
        public string Tips { get; set; }

        /// <summary>
        /// 小程序的appId
        /// </summary>
        public string WxAppId { get; set; }
        /// <summary>
        /// 拼团group id
        /// </summary>
        public string ProductGroupId { get; set; }

        /// <summary>
        /// 活动类型
        /// </summary>
        public int? ActivityType { get; set; } = 1;
        /// <summary>
        /// 是否推荐商品
        /// </summary>
        public bool IsReplace { get; set; }

        public int ImageType { get; set; } = 1;

        public string FileUrl { get; set; }
        /// <summary>
        /// 活动文案
        /// </summary>
        public string ActiveText { get; set; }
        /// <summary>
        /// 1,深色，2，浅色
        /// </summary>
        public int? CountDownStyle { get; set; }

        public string JsonContent { get; set; }

        public bool IsVehicle { get; set; }
        public int RowLimit { get; set; }
        public int VehicleLevel { get; set; }
        public string Brand { get; set; }
    }
}
