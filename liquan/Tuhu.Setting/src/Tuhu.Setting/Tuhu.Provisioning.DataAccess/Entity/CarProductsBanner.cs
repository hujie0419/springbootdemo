using System;
using System.ComponentModel;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>  
    /// 车品频道banner数据表  
    /// <author>  
    ///     <name>xiaozhongbao@tuhu.cn</name>  
    ///     <date>2018.10.16</date>  
    /// </author>  
    /// </summary>  
    [Description("车品频道banner数据表")]
    public class CarProductsBanner
    {
        /// <summary>  
        /// 主键ID  
        /// </summary>  
        /// <returns></returns>  
        [Description("主键ID")]
        public int? PKID { get; set; }

        /// <summary>  
        /// 关联楼层表主键ID  
        /// </summary>  
        /// <returns></returns>  
        [Description("关联楼层表主键ID")]
        public int? FKFloorID { get; set; }

        /// <summary>  
        /// banner名称  
        /// </summary>  
        /// <returns></returns>  
        [Description("banner名称")]
        public string Name { get; set; }

        /// <summary>  
        /// 图片地址  
        /// </summary>  
        /// <returns></returns>  
        [Description("图片地址")]
        public string ImgUrl { get; set; }

        /// <summary>  
        /// 有效开始时间  
        /// </summary>  
        /// <returns></returns>  
        [Description("有效开始时间")]
        public DateTime? StartTime { get; set; }

        /// <summary>  
        /// 有效结束时间  
        /// </summary>  
        /// <returns></returns>  
        [Description("有效结束时间")]
        public DateTime? EndTime { get; set; }

        /// <summary>  
        /// 是否启用 1是 0否  
        /// </summary>  
        /// <returns></returns>  
        [Description("是否启用 1是 0否")]
        public bool? IsEnabled { get; set; }

        /// <summary>  
        /// 显示位置 正序  
        /// </summary>  
        /// <returns></returns>  
        [Description("显示位置 正序")]
        public int? Sort { get; set; }

        /// <summary>  
        /// 1:H5活动页/2:车品详情页/3:搜索结果页  
        /// </summary>  
        /// <returns></returns>  
        [Description("1:H5活动页/2:车品详情页/3:搜索结果页")]
        public int? LinkType { get; set; }

        /// <summary>  
        ///   
        /// </summary>  
        /// <returns></returns>  
        [Description("")]
        public string Link { get; set; }

        /// <summary>  
        /// 1:头部banner 2.坑位图 3:弹框广告 4:楼层banner,Floor_ID关联楼层表  
        /// </summary>  
        /// <returns></returns>  
        [Description("1:头部banner 2.坑位图 3:弹框广告 4:楼层banner,Floor_ID关联楼层表")]
        public int? Type { get; set; }

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
