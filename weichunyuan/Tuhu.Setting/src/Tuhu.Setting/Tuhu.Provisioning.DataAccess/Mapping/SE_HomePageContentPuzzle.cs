using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class SE_HomePageContentPuzzle: EntityTypeConfiguration<SE_HomePageContentPuzzle>
    {
        public SE_HomePageContentPuzzle()
        {
            this.ToTable("SE_HomePageContentPuzzle");
            this.HasKey(o => o.PKID);
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }

        public int FKID { get; set; }


        public int PriorityLevel { get; set; }

        public string GroupID { get; set; }

        public int? FKModuleID { get; set; }

        public int? FKModuleHelperID { get; set; }

        /// <summary>
        /// 图片链接
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 合成图片地址
        /// </summary>
        public string ImagePuzzleUrl { get; set; }

        /// <summary>
        /// 合成图片的宽度
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// 合成图片的高度
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// 左上角
        /// </summary>
        public int? UpperLeftX { get; set; }

        /// <summary>
        /// 左上角
        /// </summary>
        public int? UpperLeftyY{ get; set; }

        /// <summary>
        /// 右下角
        /// </summary>
        public int? LowerRightX { get; set; }

        /// <summary>
        /// 右下角
        /// </summary>
        public int? LowerRightY { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }


    }

    public class DicPuzzleSerializeModel
    {
        public int Type { get; set; } 

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public DicPuzzleSerializeDetailModel First { get; set; }
        public DicPuzzleSerializeDetailModel Second { get; set; }
        public DicPuzzleSerializeDetailModel Third { get; set; }
        public DicPuzzleSerializeDetailModel Fourth { get; set; }


    }

    public class DicPuzzleSerializeDetailModel
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public int PriorityLevel { get; set; }
    }

}
