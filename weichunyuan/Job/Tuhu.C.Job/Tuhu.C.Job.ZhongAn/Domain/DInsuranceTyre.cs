using System;

namespace K.Domain
{
    public class DInsuranceTyre
    {
        public string PID { get; set; }
        public int PKID { get; set; }
        public string orderNo { get; set; }
        public string orderDate { get; set; }
        public string customerName { get; set; }
        public string customerPhoneNo { get; set; }
        public string plateNumber { get; set; }
        public string storeAddress { get; set; }
        public string storeName { get; set; }
        public string tyreType { get; set; }
        public string tyrePrice { get; set; }
        public string idType { get; set; }
        public string idNo { get; set; }
        public string type { get; set; }
        public string tyreBatchNo { get; set; }
        public string tyreId { get; set; }
        public byte state { get; set; }
        public DateTime SentTime { get; set; }
        public int Num { get; set; }
        public string OrderListPkid { get; set; }
    }
}
