using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class YLHUserVipCardInfoModel
    {
        #region YLH_UserVipCardInfo
        public string u_user_id { get; set; }

        public string CarNumber { get; set; }

        public string CarFactory { get; set; }

        public string CarType { get; set; }

        public double VehicleAge { get; set; }

        public string VipCardNumber { get; set; }

        public string Display_Card_NBR { get; set; }

        public bool VipCardStatus { get; set; }

        public string VipCardType { get; set; }

        public string RegisterPhone { get; set; }

        public string RegisterAddress { get; set; }

        public DateTime RegisterDate { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime UpdatedTime { get; set; }
        #endregion
    }
}
