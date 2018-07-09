using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class tbl_PotentialFormModel
    {
        #region tbl_PotentialForm
        public int PKID { get; set; }

        public Guid FormGuid { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime CloseTime { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public Guid CreatorGuid { get; set; }

        public Guid LastModifierGuid { get; set; }

        public string Type { get; set; }

        public string Memo { get; set; }

        public bool isClosed { get; set; }

        public int DataID { get; set; }

        public string CarNo { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string PostCode { get; set; }

        public string TelePhone { get; set; }

        public string CarBrand { get; set; }

        public string Gender { get; set; }

        public string Age { get; set; }

        public DateTime Birthday { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public string Products { get; set; }

        #endregion
    }
}