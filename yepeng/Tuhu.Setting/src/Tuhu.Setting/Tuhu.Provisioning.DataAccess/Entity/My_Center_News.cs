using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class My_Center_News
    {
        public int PKID { get; set; }
        public string UserObjectID { get; set; }
        public string News { get; set; }
        public string Type { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Titel { get; set; }
        public string OrderNo { get; set; }
    }
}
