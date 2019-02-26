using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class FlashSaleRecordsLog
    {
        public int PKID { get; set; }

        public Guid ActivityID { get; set; }

        public string PID { get; set; }

        public Guid UserID { get; set; }

        public string Phone { get; set; }

        public int OrderId { get; set; }

        public int OrderListId { get; set; }

        public int Quantity { get; set; }

        public string OrderStatus { get; set; }

        public DateTime CreateDate { get; set; }

        public string DeviceID { get; set; }
    }
}
