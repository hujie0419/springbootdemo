using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{


    public class BigBrandPageConfigBrandEntity : EntityTypeConfiguration<BigBrandPageConfigBrandEntity>
    {

        public BigBrandPageConfigBrandEntity()
        {
            this.ToTable("dbo.BigBrandPageConfigBrand");
            this.HasKey(_ => _.PKID);
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }
        
        /// <summary>
        /// 大翻盘活动pkid
        /// </summary>
        public int FPKID { get; set; }

        /// <summary>
        /// 大翻牌背景图
        /// </summary>
        public string BigBrandBackImgUrl { get; set; }

        /// <summary>
        /// 大翻牌中心按钮图
        /// </summary>
        public string BigBrandCenterBtnImgUrl { get; set; }

        /// <summary>
        /// 大翻牌播报框外框图
        /// </summary>
        public string BroadCastOutImgUrl { get; set; }

        /// <summary>
        /// 大翻牌播报框内框图
        /// </summary>
        public string BroadCastInnerImgUrl { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string CreateUser { get; set; }

        public string UpdateUser { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool IsDelete { get; set; }

    }

    

}
