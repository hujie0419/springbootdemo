#region Generate Code
/*
* The code is generated automically by codesmith. Please do NOT change any code.
* Generate time：2015/3/24 星期二 21:03:07
*/
#endregion

using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    ///<summary>
    /// The entity class for DB table tbl_Region.
    ///</summary>
    public class Region
    {
        public int PKID { get; set; }
        public string RegionName { get; set; }
        public string Zipcode { get; set; }
        public string Phone { get; set; }
        public bool Disabled { get; set; }
        public int ProvinceID { get; set; }
        public int CityID { get; set; }
        public int DistrictID { get; set; }
        public bool? IsBusiness { get; set; }
        public int? IsInstall { get; set; }
        public string Tag { get; set; }
        public int? ParentID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime LastUpdateTime { get; set; }
        public int? TuhuStockID { get; set; }
        public int? LogisticStockID { get; set; }
        
        //非到店快递
        public string ExpCompany { get; set; }
        //物流公司
        public string LogisticCo { get; set; }
        //到店快递
        public string ArriveShopExpCo { get; set; }
        public int? BYTuhuStockID { get; set; }
        public int? BYLogisticStockID { get; set; } 
        public string BYExpCompany { get; set; }
        public string BYLogisticCo { get; set; }
        public string BYArriveShopExpCo { get; set; }
    }
}