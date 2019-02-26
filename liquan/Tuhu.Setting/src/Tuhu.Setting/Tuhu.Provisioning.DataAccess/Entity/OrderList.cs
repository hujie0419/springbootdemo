#region Generate Code
/*
* The code is generated automically by codesmith. Please do NOT change any code.
* Generate time：2014/10/9 星期四 16:14:25
*/
#endregion

using System;
using System.Collections.Generic;
namespace Tuhu.Provisioning.DataAccess.Entity
{
    ///<summary>
    /// The entity class for DB table tbl_OrderList.
    ///</summary>
    public class OrderList
    {
        public int PKID { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public string PID { get; set; }
        public int Poid { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Remark { get; set; }
        public string WeekYear { get; set; }
        public string CarCode { get; set; }
        public string CarName { get; set; }
        public int Num { get; set; }
        public decimal MarkedPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Price { get; set; }
        public decimal AdditionalFee { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal? Cost { get; set; }
        public decimal? InstallFee { get; set; }
        public decimal ServerFee { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public string Vendor { get; set; }
        public bool IsPaid { get; set; }
        public string PaidVia { get; set; }
        public DateTime InstockDate { get; set; }
        public DateTime PaidDate { get; set; }
        public string PurchaseStatus { get; set; }
        public bool IsInstallFeePaid { get; set; }
        public DateTime InstallFeePaidDate { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreateDate { get; set; }
        public double Commission { get; set; }
        public int HCNum { get; set; }
        public int OrigProdId { get; set; }
        public bool? StockOut { get; set; }
        public bool OnSale { get; set; }

        public int? ParentID { get; set; }
        public int ProductType { get; set; }
        public string FUPID { get; set; }

        public string CP_Remark { get; set; }

        public string NodeNo { get; set; }

        public bool IsDaiFa { get; set; }
    }

    public class OrderListModel
    {
        /// <summary>
        /// 套餐虚产品
        /// </summary>
        public OrderList ParentOrderList { get; set; }

        /// <summary>
        /// 套餐实产品
        /// </summary>
        public List<OrderList> ChildOrderList { get; set; }
    }

    /// <summary>
    /// 自定义排序规则
    /// </summary>
    public class OrderListComparer : IComparer<OrderList>
    {
        /// <summary>
        /// 服务类产品排序在下面显示
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(OrderList x, OrderList y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            {
                var t = x.PID.StartsWith("FU-");
                if (t) return 1;
                if (!t) return -1;
            }

            return 0;
        }
    }

}