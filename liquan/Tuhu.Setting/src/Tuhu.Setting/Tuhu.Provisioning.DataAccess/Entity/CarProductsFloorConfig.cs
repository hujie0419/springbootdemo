using System;
using System.ComponentModel;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>  
    /// 车品频道楼层配置表  
    /// <author>  
    ///     <name>xiaozhongbao@tuhu.cn</name>  
    ///     <date>2018.10.16</date>  
    /// </author>  
    /// </summary>  
    [Description("车品频道楼层配置表")]
    public class CarProductsFloorConfig
    {
        /// <summary>  
        /// 主键ID  
        /// </summary>  
        /// <returns></returns>  
        [Description("主键ID")]
        public int? PKID { get; set; }

        /// <summary>  
        /// 关联车品楼层表主键ID  
        /// </summary>  
        /// <returns></returns>  
        [Description("关联车品楼层表主键ID")]
        public int? FKFloorID { get; set; }

        /// <summary>  
        /// 三级车品名称  
        /// </summary>  
        /// <returns></returns>  
        [Description("三级车品名称")]
        public string Name { get; set; }

        /// <summary>  
        /// 三级车品代码  
        /// </summary>  
        /// <returns></returns>  
        [Description("三级车品代码")]
        public string Code { get; set; }

        /// <summary>  
        /// 取产品数量  
        /// </summary>  
        /// <returns></returns>  
        [Description("取产品数量")]
        public int? PidCount { get; set; }

        /// <summary>  
        /// 自定义PID ','号分隔  
        /// </summary>  
        /// <returns></returns>  
        [Description("自定义PID ','号分隔")]
        public string PIDS { get; set; }

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

        /// <summary>  
        /// 是否禁用 1是 0否  
        /// </summary>  
        /// <returns></returns>  
        [Description("是否禁用 1是 0否")]
        public bool? IsEnabled { get; set; }

    }
}
