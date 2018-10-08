using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Server.Manager.SalePromotion
{
    public class ProductHitFailCode
    {
        public const string UserBeLimited = "超过个人限购数量";
        public const string NotEnoughStock = "折扣库存不足";
        public const string NotEnoughNum = "数量不满足折扣门槛";
        public const string NotEnoughMoney = "金额不满足折扣门槛";
        public const string MustShopPay = "到店支付专享";
        public const string MustOnlinePay = "在线支付专享";
        public const string MustShopInstall = "到店安装专享";
        public const string MustHomeInstall = "上门安装专享";
        public const string MustNoneInstall = "无需安装专享";
        public const string NoneActivityDiscount = "无打折活动";
        /// <summary>
        /// 获取安装方式描述
        /// </summary>
        /// <param name="InstallMethod"></param>
        /// <returns></returns>
        public static Tuple<string, string> GetFailCodeAndDescriptionOfInstall(int InstallMethod)
        {
            var code = string.Empty;
            var description = string.Empty;
            switch (InstallMethod)
            {
                case 1:
                    code = nameof(MustShopInstall);
                    description = MustShopInstall;
                    break;
                case 2:
                    code = nameof(MustHomeInstall);
                    description = MustHomeInstall;
                    break;
                case 3:
                    code = nameof(MustNoneInstall);
                    description = MustNoneInstall;
                    break;
            }
            return new Tuple<string, string>(code,description) ;
        }
        /// <summary>
        /// 1 到店
        /// 2 在线
        /// </summary>
        public static Tuple<string, string> GetFailCodeAndDescriptionOfPay(int payMethod)
        {
            var code = string.Empty;
            var description = string.Empty;
            switch (payMethod)
            {
                case 1:
                    code = nameof(MustShopPay);
                    description = MustShopPay;
                    break;
                case 2:
                    code = nameof(MustOnlinePay);
                    description = MustOnlinePay;
                    break;
            }
            return new Tuple<string, string>(code, description);
        }
    }
}
