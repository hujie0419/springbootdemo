using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class tbl_EndUserCaseModel
    {
        #region tbl_EndUserCase
        public int PKID { get; set; }

        public Guid CaseGuid { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime CloseTime { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public Guid OwnerGuid { get; set; }

        public string Type { get; set; }

        public string Memo { get; set; }

        public int StatusID { get; set; }

        public bool isClosed { get; set; }

        public Guid EndUserGuid { get; set; }

        public Guid EndUserCarGuid { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public Guid PotentialFormGuid { get; set; }

        public DateTime NextContactTime { get; set; }

        public DateTime ForecastDate { get; set; }

        public string Creator { get; set; }

        public string CaseInterest { get; set; }

        public string Source { get; set; }

        public string Way { get; set; }

        public string Fail { get; set; }

        public string Tasks { get; set; }

        public string Orders { get; set; }

        public string Products { get; set; }

        public string Remark { get; set; }

        #endregion
    }
}