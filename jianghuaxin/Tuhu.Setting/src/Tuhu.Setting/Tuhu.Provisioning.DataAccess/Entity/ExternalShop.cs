#region Generate Code
/*
* The code is generated automically by codesmith. Please do NOT change any code.
* Generate time：2015/3/11 星期三 17:59:43
*/
#endregion

using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class ExternalShop
    {
        public int PKID { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public int ShopType { get; set; }
        public string Creator { get; set; }
        public DateTime LastUpdatetime { get; set; }
        public string EmailAddress { get; set; }
        public int? CPShopId { get; set; }

        /// <summary>
        /// 自己管理快递
        /// </summary>
        public bool IsExpressSelfHosted { get; set; }
    }
}