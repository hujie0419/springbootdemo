using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class FlashSales
    {
        public int PKID { get; set; }
        public string EnID { get; set; }
        public DateTime StartTime { get; set; }
        public int CountDown { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public int Position { get; set; }
        public int DisplayColumn { get; set; }
        public string AppValue { get; set; }
        public bool IsBanner { get; set; }
        public string BannerUrl { get; set; }
        public string BackgoundColor { get; set; }
        public string TomorrowText { get; set; }
        public bool IsTomorrowTextActive { get; set; }
        public byte Status{ get; set; }
    }
}
