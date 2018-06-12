using System;

namespace K.Domain
{
    public class DOrderToInsuranceTyre
    {
        public int PKID { get; set; }

        public int OrderId { get; set; }

        public int InsuranceType { get; set; }

        public int InsuranceStatus { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }
}
