using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Battery
{
    public class BaoYangBatteryCoverArea
    {
        public Int64 PKID { get; set; }

        public string ProvinceName { get; set; }

        public string CityName { get; set; }

        public int ProvinceId { get; set; }

        public int CityId { get; set; }

        public string Brand { get; set; }

        public string Channels { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreateDatetime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        public int Total { get; set; }
    }
}
