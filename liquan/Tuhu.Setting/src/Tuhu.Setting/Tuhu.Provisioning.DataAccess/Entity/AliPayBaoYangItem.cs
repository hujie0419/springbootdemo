using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class AliPayBaoYangItem
    {
        public int PKID { get; set; }
        public string KeyWord { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }

    public class AliPayBaoYangActivity
    {
        public int PKID { get; set; }

        public string Name { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

       
    }

    public class AliPayBaoYangSetting
    {
        public AliPayBaoYangActivity Activity { get; set; }
        public List<AliPayBaoYangItem> AliPayBaoYangItems { get; set; }
    }
  
}
