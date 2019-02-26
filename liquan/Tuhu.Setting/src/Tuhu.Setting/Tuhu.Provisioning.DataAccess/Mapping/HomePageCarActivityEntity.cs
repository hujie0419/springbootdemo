using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
   
    public class HomePageCarActivityEntity : EntityTypeConfiguration<HomePageCarActivityEntity>
    {

        public HomePageCarActivityEntity()
        {
            this.ToTable("dbo.HomePageCarActivity");
            this.HasKey(_ => _.PKID);
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PKID
        {
            set;
            get;
        }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }


        public string HashKey { get; set; }

        public string Name { get; set; }

        public string UserIDs { get; set; }

        public string URL { get; set; }

        public string StartVersion { get; set; }

        public string EndVersion { get; set; }

        public bool? Status { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public string MainTitle { get; set; }

        public string MainTitleColor { get; set; }

        /// <summary>
        /// 高亮主标题颜色
        /// </summary>
        public string MainHighlightColor { get; set; }

        /// <summary>
        /// 高亮副标题颜色
        /// </summary>
        public string SubHighlightColor { get; set; }

        public string SubTitle { get; set; }

        public string SubTitleColor { get; set; }

      
        public int? OrderBy { get; set; }

        public string JumpUrl { get; set; }


        public int? Position { get; set; }

        public string ImageUrl { get; set; }

        public string FileUrl { get; set; }


    }

    public class HomePageCarActivityRegionEntity : EntityTypeConfiguration<HomePageCarActivityRegionEntity>
    {
        public HomePageCarActivityRegionEntity()
        {
            this.ToTable("dbo.HomePageCarActivityRegion");
            this.HasKey(_ => _.PKID);
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PKID
        {
            set;
            get;
        }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }

        public int FKCarActviityPKID { get; set; }

        public int RegionID { get; set; }

        [NotMapped]
        public int ParentID { get; set; }

    }

    public class HomePageCACarInfoEntity : EntityTypeConfiguration<HomePageCACarInfoEntity>
    {
        public HomePageCACarInfoEntity()
        {
            this.ToTable("dbo.HomePageCACarInfo");
            this.HasKey(_ => _.PKID);
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PKID
        {
            set;
            get;
        }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }

        public int FKCarActviityPKID { get; set; }

        public string Brand { get; set; }

        public string Vehicle { get; set; }

        public string SalesName { get; set; }

        public string u_Nian { get; set; }

        public string u_PaiLiang { get; set; }

    }


}
