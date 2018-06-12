using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ConfigApi
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime? CreateTime { get; set; }

        public string Remark { get; set; }

        public int Type { get; set; }
    }

    public class ConfigCoupon
    {
        public int Id { get; set; }

        public string BackgroundImage { get; set; }

        public string BackgroundSmallImage { get; set; }

        public string BackgroundColor { get; set; }

        public string ActivityRule { get; set; }

        public bool LinkAvailable { get; set; }

        public string LinkImage { get; set; }

        public string LinkSamllImage { get; set; }

        public string AppLink { get; set; }

        public string WeixinLink { get; set; }

        public string Link { get; set; }

        public bool CheckLink { get; set; }

        public DateTime? CreateTime { get; set; }

    }
}
