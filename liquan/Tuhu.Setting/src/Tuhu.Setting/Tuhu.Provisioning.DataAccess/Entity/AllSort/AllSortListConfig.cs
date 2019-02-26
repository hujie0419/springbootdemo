using System;

namespace Tuhu.Provisioning.DataAccess.Entity.AllSort
{
    public class AllSortListConfig
    {
        /// <summary>
        ///     主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        ///     修改时间
        /// </summary>
        public DateTime UpdateDateTime { get; set; }

        /// <summary>
        ///     名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        ///     名称文字色值
        /// </summary>

        public string TitleColor { get; set; }

        /// <summary>
        ///     名称背景色色值
        /// </summary>
        public string TitleBgColor { get; set; }

        /// <summary>
        ///     开始版本
        /// </summary>
        public string StartVersion { get; set; }

        /// <summary>
        ///     结束版本
        /// </summary>
        public string EndVersion { get; set; }


        /// <summary>
        ///     按钮图片
        /// </summary>
        public string ButtonImage { get; set; }


        /// <summary>
        ///     Banner图片
        /// </summary>
        public string BannerImage { get; set; }

        /// <summary>
        ///    APP 链接
        /// </summary>
        public string JumpUrl { get; set; }
        /// <summary>
        ///     小程序链接
        /// </summary>
        public string XiaoJumpUrl { get; set; }
        /// <summary>
        ///     优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        ///     统计
        /// </summary>
        public string Statistics { get; set; }

        /// <summary>
        ///     开始时间
        /// </summary>
        public DateTime StartTime { get; set; }


        /// <summary>
        ///     结束时间
        /// </summary>
        public DateTime EndTime { get; set; }


        /// <summary>
        ///     外键 AllSortConfig-PKID
        /// </summary>
        public int ParentId { get; set; }


        /// <summary>
        /// 外键 AllSortConfig-Maintitle
        /// </summary>
        public String ParentMaintitle { get; set; }

        /// <summary>
        /// 逻辑删除标志
        /// 1：未删除
        /// 0：已删除
        /// </summary>
        public int IsAbled { get; set; }

        /*
        public AllSortListConfig()
        {
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            PKID = 0;
            TitleColor = "#ffffff";
            TitleBgColor = "#000000";
        }
        */
    }
}