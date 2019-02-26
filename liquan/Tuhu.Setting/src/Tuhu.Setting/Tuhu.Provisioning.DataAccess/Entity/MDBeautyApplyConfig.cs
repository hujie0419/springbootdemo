using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class MDBeautyApplyConfig
    {
        public int Id { get; set; }

        public string ActivityId { get; set; }

        public int ActivityPKID { get; set; }
        public int ShopId { get; set; }

        public string ShopName { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public float Price { get; set; }

        public int Quantity { get; set; }

        public short ApplyAuditStatus { get; set; }

        public short ExitAuditStatus { get; set; }

        public string AuditReason { get; set; }

        public string ExitReason { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public int BeautyAcitivityId { get; set; }

        public string Name { get; set; }

        public DateTime? SignUpStartTime { get; set; }

        public DateTime? SignUpEndTime { get; set; }

        public DateTime? PlanStartTime { get; set; }
        public DateTime? ActivityStartTime { get; set; }

        public DateTime? ActivityEndTime { get; set; }

        public int CategoryId { get; set; }

        public string ActivityTypeName { get; set; }

        public List<RegionRelation> RegionList { get; set; }

        public float MinPrice { get; set; }

        public float MaxPrice { get; set; }

        public int EverydayQuantity { get; set; }

        public int MinShopQuantity { get; set; }

        public int VehicleGrade { get; set; }

        public int ApplicationVehicle { get; set; }

        public int ShopType { get; set; }

        public int ShopGrade { get; set; }

        public int MeiRongAppraise { get; set; }

        public string ActivityRequire { get; set; }

        public string ActivityNotification { get; set; }

        public int Status { get; set; }

        public string CreateName { get; set; }

        public string UpdateName { get; set; }

        public string StrProvince { get; set; }

        public string StrCity { get; set; }

        public string CategoryName { get; set; }

        public string ShowName { get; set; }

        public string ExitActivityReason { get; set; }

    }
}
