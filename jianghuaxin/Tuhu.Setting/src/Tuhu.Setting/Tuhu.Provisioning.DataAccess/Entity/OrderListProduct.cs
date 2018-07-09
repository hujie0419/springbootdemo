using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class OrderListProduct
	{
		public int PKID { get; set; }
		public int OrderID { get; set; }
		public string OrderNo { get; set; }
		public string PID { get; set; }
		public int? Poid { get; set; }
		public string Category { get; set; }
		public string Name { get; set; }
		public string Size { get; set; }
		public string Remark { get; set; }
		public string CarCode { get; set; }
		public string CarName { get; set; }
		public int Num { get; set; }
		public decimal MarkedPrice { get; set; }
		public decimal Discount { get; set; }
		public decimal Price { get; set; }
		public decimal TotalDiscount { get; set; }
		public decimal TotalPrice { get; set; }
		public decimal PurchasePrice { get; set; }
		public decimal Cost { get; set; }
		public decimal InstallFee { get; set; }
		public DateTime? LastUpdateTime { get; set; }
		public string Vendor { get; set; }
		public bool? IsPaid { get; set; }
		public string PaidVia { get; set; }
		public DateTime? InstockDate { get; set; }
		public DateTime? PaidDate { get; set; }
		public string PurchaseStatus { get; set; }
		public bool? IsInstallFeePaid { get; set; }
		public DateTime? InstallFeePaidDate { get; set; }
		public bool? Deleted { get; set; }
		public DateTime? CreateDate { get; set; }
		public float? Commission { get; set; }
		public int WareHouseID { get; set; }

		/// <summary>
		/// 已占用采购数量
		/// </summary>
		public int OrderedNum { get; set; }

		/// <summary>
		/// 已占用仓库数量
		/// </summary>
		public int StockNum { get; set; }

        /// <summary>
        /// 产品周期
        /// </summary>
        public string WeekYear { get; set; }
        /// <summary>
        /// 保存产品雨保养产品的关联关系
        /// </summary>
        public string FUPID { get; set; }
	}


    public class OrderProductList
    {
        private int pkid;
        private int orderID;
        private string orderNo;
        private string pid;
        private int poid;
        private string category;
        private string name;
        private string size;
        private string remark;
        private string carCode;
        private string carName;
        private int num;
        private decimal markedPrice;
        private decimal discount;
        private decimal price;
        private decimal totalDiscount;
        private decimal totalPrice;
        private System.DateTime lastUpdateTime;
        private decimal purchasePrice;
        private decimal cost;
        private decimal installFee;
        private string vendor;
        private bool isPaid;
        private string paidVia;
        private System.DateTime instockDate;
        private System.DateTime paidDate;
        private string purchaseStatus;
        private bool isInstallFeePaid;
        private System.DateTime installFeePaidDate;
        private bool deleted;
        private System.DateTime createDate;
        private double commission;
        private int hCNum;
        private int origProdId;
        private int productType;
        private int parentID;
        private string weekYear;
        private string refID;

        /// <summary>
        /// 
        /// </summary>
        public int Pkid
        {
            get { return pkid; }
            set { pkid = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OrderNo
        {
            get { return orderNo; }
            set { orderNo = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Pid
        {
            get { return pid; }
            set { pid = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Poid
        {
            get { return poid; }
            set { poid = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Category
        {
            get { return category; }
            set { category = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Size
        {
            get { return size; }
            set { size = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CarCode
        {
            get { return carCode; }
            set { carCode = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CarName
        {
            get { return carName; }
            set { carName = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Num
        {
            get { return num; }
            set { num = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal MarkedPrice
        {
            get { return markedPrice; }
            set { markedPrice = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal Discount
        {
            get { return discount; }
            set { discount = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalDiscount
        {
            get { return totalDiscount; }
            set { totalDiscount = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public System.DateTime LastUpdateTime
        {
            get { return lastUpdateTime; }
            set { lastUpdateTime = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal PurchasePrice
        {
            get { return purchasePrice; }
            set { purchasePrice = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal Cost
        {
            get { return cost; }
            set { cost = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal InstallFee
        {
            get { return installFee; }
            set { installFee = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Vendor
        {
            get { return vendor; }
            set { vendor = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsPaid
        {
            get { return isPaid; }
            set { isPaid = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PaidVia
        {
            get { return paidVia; }
            set { paidVia = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public System.DateTime InstockDate
        {
            get { return instockDate; }
            set { instockDate = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public System.DateTime PaidDate
        {
            get { return paidDate; }
            set { paidDate = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PurchaseStatus
        {
            get { return purchaseStatus; }
            set { purchaseStatus = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsInstallFeePaid
        {
            get { return isInstallFeePaid; }
            set { isInstallFeePaid = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public System.DateTime InstallFeePaidDate
        {
            get { return installFeePaidDate; }
            set { installFeePaidDate = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Deleted
        {
            get { return deleted; }
            set { deleted = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public System.DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public double Commission
        {
            get { return commission; }
            set { commission = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int HCNum
        {
            get { return hCNum; }
            set { hCNum = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int OrigProdId
        {
            get { return origProdId; }
            set { origProdId = value; }
        }
        /// <summary>
        /// 1：轮胎轮毂；2：保养产品；4：车品；8：美容；16：赠品；32：套装；
        /// </summary>
        public int ProductType
        {
            get { return productType; }
            set { productType = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int ParentID
        {
            get { return parentID; }
            set { parentID = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string WeekYear
        {
            get { return weekYear; }
            set { weekYear = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string RefID
        {
            get { return refID; }
            set { refID = value; }
        }
    }
}
