using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.AllSort
{
    public class AllSortTagConfig
    {

        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDateTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int  State { get; set; }

        /// <summary>
        /// 主标题
        /// </summary>
        public String Maintitle { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public String Subtitle { get; set; }

        /// <summary>
        /// 副标题色值
        /// </summary>
        public String SubtitleColor { get; set; }
        
        /// <summary>
        /// 频道小程序入口跳转   
        /// </summary>
        public String XiaoChannelJumpUrl { get; set; }
        /// <summary>
        /// 频道App入口跳转   
        /// </summary>
        public String ChannelJumpUrl { get; set; }

        /// <summary>
        ///  频道入口 //是否需要跳转到别处 
        /// </summary>
        public int ChannelState { get; set; }


        /// <summary>
        /// 频道入口跳转统计 //点击数统计 
        /// </summary>
        public String Statistics { get; set; }


        /// <summary>
        /// 频道入口描述 
        /// </summary>
        public String ChannelDescription { get; set; }

        /// <summary>
        /// 外键 AllSortConfig-PKID
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
        /// <summary>
        /// 构造函数
        /// </summary>
        public AllSortTagConfig()
        {
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            PKID = 0;
        }
    }
}
