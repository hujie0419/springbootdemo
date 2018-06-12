using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
   public class tbl_UserTransportation:BaseModel
    {
        public tbl_UserTransportation() : base() { }


        public tbl_UserTransportation(DataRow row) : base(row) { }


        public int? ID { get; set; }


        public int? Rank { get; set; }

        public decimal? TransMoney { get; set; }

        public decimal? Discount { get; set; }

        public decimal? SaleMoney { get; set; }


    }
}
