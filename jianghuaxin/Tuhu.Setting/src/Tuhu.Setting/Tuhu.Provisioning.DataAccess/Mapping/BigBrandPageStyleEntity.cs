using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class BigBrandPageStyleEntity:EntityTypeConfiguration<BigBrandPageStyleEntity>
    {
        /*
        PKID INT IDENTITY(1,1) PRIMARY KEY,
        FKPKID INT  FOREIGN KEY REFERENCES Configuration.dbo.BigBrandRewardList(PKID) NOT NULL,--大翻牌列表的主键
        CreateDateTime DATETIME NOT NULL ,
        LastUpdateDateTime DATETIME NOT NULL ,
        PageType INT ,--页面类型
        IsShare BIT ,--全局分享按钮
        Position INT ,--位置 1:左上 2:上 3:右上 4：左 5：右 6：左下 7：下 8：右下 
        ImageUri VARCHAR(1000),--图片链接
        CreateUserName VARCHAR(100) ,--创建人
        UpdateUserName VARCHAR(100),--更新人 
        */

        public BigBrandPageStyleEntity()
        {
            this.ToTable("dbo.BigBrandPageStyle");
            this.HasKey(_=>_.PKID);
        }



        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }


        public int? FKPKID { get; set; }

        public Guid GroupGuid { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }

        public int? PageType { get; set; }

        public bool? IsShare { get; set; }

        public int? Position { get; set; }

        public string ImageUri { get; set; }
        
        /// <summary>
        /// 背景图片
        /// </summary>
        public string BImageUri { get; set; }

        public string CreateUserName { get; set; }

        public string UpdateUserName { get; set; }
        /// <summary>
        /// 弹窗样式，0.普通弹窗，1.节日弹窗
        /// </summary>
        public Int16? PromptStyle { get; set; }





    }
}
