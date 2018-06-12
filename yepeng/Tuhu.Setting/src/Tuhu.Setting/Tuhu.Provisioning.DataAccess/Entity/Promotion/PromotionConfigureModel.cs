using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 套餐配置活动数据
    /// </summary>
    public class PromotionConfigureModel
    {

        public int PKID { get; set; }

        public string PromotionID { get; set; }

        public string PromotionName { get; set; }

        public string GiftProductID { get; set; }

        public string GiftName { get; set; }

        public int GiftNum { get; set; }

        public int PromotionNum { get; set; }

        public bool IsCertain { get; set; }

        public string CreateBy { get; set; }

        public string LastUpdateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public bool IsActive { get; set; }

        public decimal MarketPrice { get; set; }

        public string OrderChannel { get; set; }

        public int CateOrSingle { get; set; }

        public bool GiftMethod { get; set; }

        public string GiftDescription { get; set; }

        public string InstallType { get; set; }
        public string Titile { get; set; }
    }
}
