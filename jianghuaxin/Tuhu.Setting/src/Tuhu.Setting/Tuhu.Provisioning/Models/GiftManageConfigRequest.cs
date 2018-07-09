using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class GiftManageConfigRequest
    {
        public int pageIndex { get; set; } = 1;
        public int pageSize { get; set; } = 30;
        public string Name { get; set; }
        public string ValidTimeBegin { get; set; }
        public string ValidTimeEnd { get; set; }
        public string Creater { get; set; }
        public string Mender { get; set; }
        public int state { get; set; } = 4;
        public string productPid { get; set; }
        public string giftPid { get; set; }
        public string giftName { get; set; }
        public string channel { get; set; }
        public string Group { get; set; }
        public int IsPackage { get; set; } = 2;
    }
}