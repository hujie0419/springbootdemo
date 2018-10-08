using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class WMSUserObject
    {
        public int PKID { get; set; }
        public string EmailAddress { get; set; }
        public int ShopID { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdate { get; set; }

        public int ShopsNo { get; set; }

        public string ShopsName { get; set; }

        public bool IsDefault { get; set; }
    }
}
