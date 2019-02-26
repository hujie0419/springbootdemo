using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class tbl_LogModel
    {
        #region tbl_Log

        public int PKID { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastUpdate { get; set; }

        public Guid OwnerGuid { get; set; }

        public string Content { get; set; }

        public int Action { get; set; }

        public string ActionText1 { get; set; }

        public string ActionText2 { get; set; }

        public string ActionText3 { get; set; }

        public Guid CaseGuid { get; set; }

        public Guid FormGuid { get; set; }

        public Guid EndUserGuid { get; set; }

        public Guid OrgGuid { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public DateTime NextContactTime { get; set; }

        public DateTime CreateDate { get; set; }

        #endregion
    }
}
