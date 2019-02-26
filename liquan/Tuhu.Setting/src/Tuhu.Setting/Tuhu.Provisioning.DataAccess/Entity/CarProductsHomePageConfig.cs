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
    public class CarProductsHomePageConfig
    {
        /// <summary>  
        /// 主键ID  
        /// </summary>  
        /// <returns></returns>  
        [Description("主键ID")]
        public int? PKID { get; set; }

        /// <summary>  
        /// 配置名称  
        /// </summary>  
        /// <returns></returns>  
        [Description("配置名称")]
        public string HomePageName { get; set; }

        /// <summary>  
        /// 是否启用 1是 0否  
        /// </summary>  
        /// <returns></returns>  
        [Description("是否启用 1是 0否")]
        public bool? IsEnabled { get; set; }

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
