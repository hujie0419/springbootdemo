using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Business.Push
{
    public class PushTemplateModifyLog
    {
        public int PKID { get; set; }
        public string User { get; set; }
        public string OriginTemplate { get; set; }
        public string NewTemplate { get; set; }
        public int TemplateID { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
    }
}
