using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CrmUserCaseModel
    {
        public DateTime NextContactTime { get; set; }
        public DateTime ForecastDate { get; set; }
        public string Nickname { get; set; }
        public string Type { get; set; }
        public string District { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set;}
        public string carno { get; set; }
        public string cartype_description { get; set; }
        public string Products { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public string TaobaoId { get; set; }
        public string Q_Name { get; set; }
        public string Q_CarNo { get; set; }
        public string Q_Telephone { get; set; }
        public string CaseInterest { get; set; }
        public string StatusID { get; set; }
        public string Lastlog { get; set; }
        public DateTime CaseLastModifiedTime { get; set; }
        public DateTime CreateTime { get; set; }
        public string CaseGuid { get; set; }
        public string EndUserGuid { get; set; }
        public bool IsClosed { get; set; }
        public string OwnerGuid { get; set; }
    }
}