using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class FlashSalesProductTwo
    {
        public int PKID { get; set; }
        public string PID { get; set; }
        public int Position { get; set; }
        public string ActivityID { get; set; }
        public decimal Price { get; set; }//促销价
        public int TotalQuantity { get; set; }//总数量（总限制数量）
        public int SaleOutQuantity { get; set; }//已卖出的数量
        public int MaxQuantity { get; set; }//用户限购
        public int RestQuantity { get { return TotalQuantity - SaleOutQuantity; } }//剩余数量=总数-卖出数
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public string ProductName { get; set; }
        public string DisplayName { get; set; }
        public decimal cy_list_price { get; set; }//原价
        public string Label { get; set; }

        public string InstallAndPay { get; set; }

        public bool IsUsePcode { get; set; }
    }
}
