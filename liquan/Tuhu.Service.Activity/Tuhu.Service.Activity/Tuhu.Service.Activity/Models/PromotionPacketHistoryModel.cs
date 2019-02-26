using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
   public class PromotionPacketHistoryModel:BaseModel
    {
        public int PKID { set; get; }
        public string UserID { set; get; }
        public int EntityID { set; get; }
        public bool IsGet { set; get; }
        public int Type { set; get; }
        public string CreateDateTime { set; get; }
    }
}
