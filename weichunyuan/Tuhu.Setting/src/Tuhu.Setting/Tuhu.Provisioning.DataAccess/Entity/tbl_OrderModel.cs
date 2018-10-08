using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class tbl_OrderModel
    {
        #region tbl_Order

        public int PKID { get; set; }

        public string OrderNo { get; set; }

        public Guid UserID { get; set; }

        public string UserName { get; set; }

        public string UserTel { get; set; }

        public int AddressID { get; set; }

        public Guid CarID { get; set; }

        public string Status { get; set; }

        public DateTime OrderDatetime { get; set; }

        public DateTime BookDatetime { get; set; }

        public string BookPeriod { get; set; }

        public string Abst { get; set; }

        public string Remark { get; set; }

        public string Owner { get; set; }

        public string Submitor { get; set; }

        public string Auditor { get; set; }

        public DateTime AuditDatetime { get; set; }

        public int SumNum { get; set; }

        public Decimal SumMoney { get; set; }

        public Decimal SumMarkedMoney { get; set; }

        public Decimal SumDisMoney { get; set; }

        public Decimal ShippingMoney { get; set; }

        public Decimal InstallMoney { get; set; }

        public string DiscountItem { get; set; }

        public string InstallType { get; set; }

        public int InstallShopID { get; set; }

        public string InstallShop { get; set; }

        public string DeliveryCode { get; set; }

        public string PayStatus { get; set; }

        public Decimal SumPaid { get; set; }

        public string PayMothed { get; set; }

        public string PaymentType { get; set; }

        public DateTime PurchaseDatetime { get; set; }

        public string DeliveryType { get; set; }

        public string DeliveryStatus { get; set; }

        public DateTime DeliveryDatetime { get; set; }

        public string DeliveryConfirmor { get; set; }

        public DateTime InstallDatetime { get; set; }

        public string InstallStatus { get; set; }

        public DateTime PayToCPDate { get; set; }

        public string PayInfo { get; set; }

        public string InvoiceTitle { get; set; }

        public string InvoiceType { get; set; }

        public string InvoiceStatus { get; set; }

        public string InvoiceDeliveryCode { get; set; }

        public Decimal InvoiceAddTax { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public string CarPlate { get; set; }

        public Decimal Cost { get; set; }

        public Decimal InstallFee { get; set; }

        public Decimal DeliveryFee { get; set; }

        public Decimal OtherPayShop { get; set; }

        public bool IsOtherPaid { get; set; }

        public DateTime OtherPaidDate { get; set; }

        public Decimal InvAmont { get; set; }

        public string InvAddress { get; set; }

        public string InvTaxNum { get; set; }

        public string InvBank { get; set; }

        public string InvBankAccount { get; set; }

        public string InvRemark { get; set; }

        public string OrderType { get; set; }

        public string OrderChannel { get; set; }

        public string RefNo { get; set; }

        public Guid CaseId { get; set; }

        public int CommentStatus { get; set; }

        public bool Reconciliation { get; set; }

        public DateTime ReconcileTime { get; set; }

        public DateTime ReconciledTime { get; set; }

        public int ShopReconcileVoucher { get; set; }

        public int BankRecieveVoucher { get; set; }

        public string OrderAddress { get; set; }

        public DateTime OutStockDatetime { get; set; }

        public int IncomeVoucher { get; set; }

        public int CostVoucher { get; set; }

        public bool OrderProcessed { get; set; }

        public string OrderProcessor { get; set; }

        public DateTime OrderProcessTime { get; set; }

        public string OrderProducts { get; set; }

        public string DeliveryCompany { get; set; }

        public Guid DeliveryAddressID { get; set; }

        public string TranRefNum { get; set; }

        public bool CheckMark { get; set; }

        public string CheckComment { get; set; }

        public string TuHuHandler { get; set; }

        public string HandleComment { get; set; }

        public bool IsHandle { get; set; }

        public int PurchaseStatus { get; set; }

        public int RegionID { get; set; }

        public int WareHouseID { get; set; }

        public string WareHouseName { get; set; }

        public string DriverName { get; set; }

        public int RefOrderState { get; set; }

        public DateTime SubmitDate { get; set; }

        #endregion

        public string StatusToCN { get; set; }

        public string ApplyPerson { get; set; }

        public DateTime? ApplyDateTime { get; set; }

        public int TotalCount { get; set; }

        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
    }
}
