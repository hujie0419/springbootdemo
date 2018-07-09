using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Activity.Models
{
    public class ShareBargainModel
    {
        public Guid IdKey { get; set; }
        public decimal FinalPrice { get; set; }
        public string ProductName { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool IsOver { get; set; }
        public Guid OwnerId { get; set; }
        public string PID { get; set; }
        public int ActivityProductId { get; set; }
        public decimal OriginalPrice { get; set; }
        public string SimpleDisplayName { get; set; }
    }
}
