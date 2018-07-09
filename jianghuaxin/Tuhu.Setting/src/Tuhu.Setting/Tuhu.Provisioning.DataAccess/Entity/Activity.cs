using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using System.Data;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShareParams
    {
        public string Type { get; set; }
        public string URL { get; set; }
        public string shareImage { get; set; }
        public string shareTitle { get; set; }
        public string shareDescrip { get; set; }
        public string shareUrl { get; set; }
    }
    public class ActivityPageConfig
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int DefaultUrlId { get; set; }

        public string ShareParameters { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public string CreateName { get; set; }

        public string UpdateName { get; set; }


    }

    public class ActivityPageUrlConfig
    {
        public int Id { get; set; }

        public int ActivityPageId { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public string CreateName { get; set; }

        public string UpdateName { get; set; }

        public List<ActivityPageRegionConfig> RegionList { get; set; }

        public object RegionString { get; set; }

        public bool IsDefault { get; set; }
    }

    public class ActivityPageRegionConfig
    {
        public int Id { get; set; }

        public int UrlId { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public DateTime CreateTime { get; set; }
        public string CreateName { get; set; }
    }

    /// <summary>
    /// 活动菜单导航配置
    /// </summary>
    public class ActivityMenu : BaseModel
    {
        public ActivityMenu(DataRow row) : base(row)
        {

        }

        public int ID { get; set; }

        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActivityID { get; set; }

        public string MenuName { get; set; }

        /// <summary>
        /// 开始值
        /// </summary>
        public string MenuValue { get; set; }

        /// <summary>
        /// 结束值
        /// </summary>
        public string MenuValueEnd { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        public string Color { get; set; }

        public int MenuType { get; set; }

    }


    public class ActivityHome : BaseModel
    {

        public ActivityHome(DataRow row) : base(row) { }

        public int ID { get; set; }

        public string ActivityID { get; set; }

        public string Icon { get; set; }

        public string Name { get; set; }
        public string AppUrl { get; set; }

        public string WXUrl { get; set; }

        public string WwwUrl { get; set; }

        public string Sort { get; set; }


    }


}
