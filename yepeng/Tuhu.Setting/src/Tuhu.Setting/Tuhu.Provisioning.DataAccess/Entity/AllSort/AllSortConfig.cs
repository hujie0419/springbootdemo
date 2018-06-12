using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity.AllSort
{
    public class AllSortConfig
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
        /// 主标题
        /// </summary>
        public String Maintitle { get; set; }

        /// <summary>
        /// 开始版本
        /// </summary>
        public String StartVersion { get; set; }

        /// <summary>
        /// 结束版本 
        /// </summary>
        public String EndVersion { get; set; }

        /// <summary>
        /// 优先级 
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        ///  状态 
        /// 0：禁用 
        /// 1：启用
        /// </summary>
        public int State { get; set; }

        
        /// <summary>
        /// 导航栏色值
        /// </summary>
        public String MaintitleColor { get; set; }

        /// <summary>
        /// 统计
        /// </summary>
        public String Statistics { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }


        /// <summary>
        /// 标签栏
        /// </summary>
        public AllSortTagConfig Tag { get; set; }

        /// <summary>
        /// 内容栏集合
        /// </summary>
        public IEnumerable<AllSortListConfig> ListConfig { get; set; }

        /// <summary>
        /// 配置集合
        /// </summary>
        public IEnumerable<AllSortConfig> AllConfig { get; set; }

        /// <summary>
        /// 逻辑删除标志
        /// 1：未删除
        /// 0：已删除
        /// </summary>
        public int IsAbled { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AllSortConfig()
        {
            Tag = new AllSortTagConfig();
            ListConfig = new AllSortListConfig[] { };
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            PKID = 0;
        }
    }
    
}