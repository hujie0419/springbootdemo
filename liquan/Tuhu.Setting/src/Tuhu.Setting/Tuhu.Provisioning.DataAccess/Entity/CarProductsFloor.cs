using System;
using System.ComponentModel;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>  
    /// 车品频道楼层表  
    /// <author>  
    ///     <name>xiaozhongbao@tuhu.cn</name>  
    ///     <date>2018.10.16</date>  
    /// </author>  
    /// </summary>  
    [Description("车品频道楼层表")]
    public class CarProductsFloor
    {
        /// <summary>  
        /// 主键ID  
        /// </summary>  
        /// <returns></returns>  
        [Description("主键ID")]
        public int? PKID { get; set; }

        /// <summary>  
        /// 楼层显示名称  
        /// </summary>  
        /// <returns></returns>  
        [Description("楼层显示名称")]
        public string DisplayName { get; set; }

        /// <summary>  
        /// 二级车品分类名称  
        /// </summary>  
        /// <returns></returns>  
        [Description("二级车品分类名称")]
        public string Name { get; set; }

        /// <summary>  
        /// 二级车品编码  
        /// </summary>  
        /// <returns></returns>  
        [Description("二级车品编码")]
        public string Code { get; set; }

        /// <summary>  
        /// 显示顺序  正序排序  
        /// </summary>  
        /// <returns></returns>  
        [Description("显示顺序  正序排序")]
        public int? Sort { get; set; }

        /// <summary>  
        /// 状态 1启用 0禁用  
        /// </summary>  
        /// <returns></returns>  
        [Description("是否禁用 1是 0否")]
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

        /// <summary>
        /// Banner图片
        /// </summary>
        public string ImgUrl { get; set; }

    }

    public class Categorys
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
    }
}
