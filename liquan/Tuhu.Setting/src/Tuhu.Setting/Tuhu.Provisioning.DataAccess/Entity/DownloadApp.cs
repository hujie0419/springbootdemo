using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class DownloadApp
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

        public int Type { get; set; }
    }

    public class Content
    {
        public string SmallImage { get; set; }

        public string Image { get; set; }

        public string IOSLink { get; set; }

        public string AndroidLink { get; set; }

        public string PCLink { get; set; }

        public string WeiXinLink { get; set; }

        public string MLink { get; set; }
    }

    public class BatteryBanner
    {
        public int Id { get; set; }

        public string SmallImage { get; set; }

        public string Image { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public DateTime ShowTime { get; set; }

        public bool Display { get; set; }

        public bool Available { get; set; }


    }

}
