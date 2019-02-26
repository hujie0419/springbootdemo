using System;
using System.ComponentModel;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>  
    /// 车品模块首页配置表  
    /// <author>  
    ///     <name>xiaozhongbao@tuhu.cn</name>  
    ///     <date>2018.10.16</date>  
    /// </author>  
    /// </summary>  
    [Description("车品模块首页配置表")]
    public class CarProductsHomePageModuleConfig
    {
        /// <summary>  
        /// 主键ID  
        /// </summary>  
        /// <returns></returns>  
        [Description("主键ID")]
        public int? PKID { get; set; }

        /// <summary>  
        /// 关联配置表主键ID  
        /// </summary>  
        /// <returns></returns>  
        [Description("关联配置表主键ID")]
        public int? FKHomePageID { get; set; }

        /// <summary>  
        /// 配置名称  
        /// </summary>  
        /// <returns></returns>  
        [Description("配置名称")]
        public string ModuleName { get; set; }

        /// <summary>  
        /// 模块类型 1.Banner 2.中心坑位 3.天天秒杀 4.玩车改装 5.车品楼层 6.热销榜 7.精选晒单  
        /// </summary>  
        /// <returns></returns>  
        [Description("模块类型 1.Banner 2.中心坑位 3.天天秒杀 4.玩车改装 5.车品楼层 6.热销榜 7.精选晒单")]
        public int? ModuleType { get; set; }

        /// <summary>  
        /// 是否启用 1是 0否  
        /// </summary>  
        /// <returns></returns>  
        [Description("是否启用 1是 0否")]
        public bool? IsEnabled { get; set; }

        /// <summary>  
        /// 显示顺序 正序排列  
        /// </summary>  
        /// <returns></returns>  
        [Description("显示顺序 正序排列")]
        public int? Sort { get; set; }

        /// <summary>  
        /// 创建时间  
        /// </summary>  
        /// <returns></returns>  
        [Description("创建时间")]
        public DateTime? CreateTime { get; set; }

        /// <summary>  
        /// 更新时间  
        /// </summary>  
        /// <returns></returns>  
        [Description("更新时间")]
        public DateTime? UpdateTime { get; set; }
    }
}
