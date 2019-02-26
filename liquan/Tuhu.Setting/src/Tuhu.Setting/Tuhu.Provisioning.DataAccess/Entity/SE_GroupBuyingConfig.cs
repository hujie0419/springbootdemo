using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SE_GroupBuyingConfig
    {
        public int ID { get; set; }


        public string PID { get; set; }

        /// <summary>
        /// 限时抢购活动ID
        /// </summary>
        public Guid? FalshSaleGuid { get; set; }


        public decimal? ActivityPrice { get; set; }


        public int? LimitSaleNumber { get; set; }


        public int? LimitGroupNumber { get; set; }

        public bool IsGroupBuy
        {
            get
            {
                return true;
            }
        }

        public int  OrderBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string ProductName { get; set; }

        public int SaleOutQuantity { get; set; }
    }
}
