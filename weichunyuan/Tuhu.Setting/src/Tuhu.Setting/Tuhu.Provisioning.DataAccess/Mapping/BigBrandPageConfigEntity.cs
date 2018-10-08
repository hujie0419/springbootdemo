using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class BigBrandPageConfigeEntity : EntityTypeConfiguration<BigBrandPageConfigeEntity>
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

        public BigBrandPageConfigeEntity()
        {
            this.ToTable("dbo.BigBrandPageConfig");
            this.HasKey(_=>_.PKID);
        }



        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }


        public int FKPKID { get; set; }


        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 页面类型：0-摇奖机 
        /// </summary>
        public int ActivityType { get; set; }

        /// <summary>
        /// 全局分享按钮
        /// </summary>
        public bool IsShare { get; set; }

        /// <summary>
        /// 背景图1
        /// </summary>
        public string HomeBgImgUri { get; set; }

        /// <summary>
        /// 背景图2
        /// </summary>
        public string HomeBgImgUri2 { get; set; }

        /// <summary>
        /// 结果页面图片
        /// </summary>
        public string ResultImgUri { get; set; }
        /// <summary>
        /// 摇奖机图片
        /// </summary>
        public string DrawMachineImgUri { get; set; }

        /// <summary>
        /// 跑马灯 是否开启
        /// </summary>
        public int MarqueeLampIsOn { get; set; }
    }
}
