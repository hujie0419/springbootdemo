using System;
using System.Data;
using Tuhu.WebSite.Component.SystemFramework.Models;

namespace Tuhu.WebSite.Web.Activity.Models
{
    public class ActivityBuild : BaseModel
    {
        public int id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string ActivityUrl { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public ActivityBuild() : base() { }
        public ActivityBuild(DataRow row)
            : base(row)
        { }
    }


    public class BatteryBanner : BaseModel
    {
        public int Id { get; set; }

        public string Image { get; set; }

        public DateTime ShowTime { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public bool Display { get; set; }

        public bool Available { get; set; }


        public BatteryBanner() : base() { }
        public BatteryBanner(DataRow row)
            : base(row)
        { }

    }
    public class ActivityContent
    {
        public string PID { get; set; }
        public string VID { get; set; }
        public string ActivityId { get; set; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public int Type { get; set; }
        public int OrderBy { get; set; }
        public int BigImg { get; set; }

        public string LinkUrl { get; set; }
    }

    public class DownloadApp : BaseModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string BottomContent { get; set; }

        public bool BottomStatus { get; set; }

        public string ImageContent { get; set; }

        public string TimerContent { get; set; }

        public bool TimerStatus { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public DownloadApp() : base() { }

        public DownloadApp(DataRow row)
            : base(row)
        { }
    }

}