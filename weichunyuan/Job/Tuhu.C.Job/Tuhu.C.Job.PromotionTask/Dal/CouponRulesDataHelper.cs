using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.PromotionTask.Model;

namespace Tuhu.C.Job.PromotionTask.Dal
{
    public class CouponRulesDataHelper
    {
        private static readonly ILog CouponRulesDataHelperLogger = LogManager.GetLogger<CouponRulesDataHelper>();
        private List<CouponRulesConfig> _InsertProductData, _InsertShopData;
        private static object _lockObj = new object();
        private static CouponRulesDataHelper _Instanse;
        public static CouponRulesDataHelper Instanse
        {
            get
            {
                if (_Instanse == null)
                {
                    lock (_lockObj)
                    {
                        if (_Instanse == null)
                        {
                            _Instanse = new CouponRulesDataHelper();
                        }
                    }
                }
                return _Instanse;
            }
        }
        private CouponRulesDataHelper()
        {
            _InsertProductData = new List<CouponRulesConfig>();
            _InsertShopData = new List<CouponRulesConfig>();
        }

        public bool AddData(CouponRulesConfig model, int ConfigType)
        {
            try
            {
                if (ConfigType == 0)
                {
                    _InsertProductData.Add(model);
                }
                else
                {
                    _InsertShopData.Add(model);
                }
                return true;
            }
            catch (Exception ex)
            {
                CouponRulesDataHelperLogger.Warn(ex);
                return false;
            }
        }
        /// <summary>
        /// ConfigType=0 product;ConfigType=1 shop;
        /// </summary>
        /// <param name="ConfigType"></param>
        /// <returns></returns>
        public int ExcuteDatas()
        {
            var result = 0;
            if (_InsertProductData != null && _InsertProductData.Any())
            {
                DalCouponRules.CreateCouponRuleProductConfigsData(_InsertProductData.Distinct(), "tbl_CouponRules_ConfigProduct");
                _InsertProductData = new List<CouponRulesConfig>();
                result++;
            }
            if (_InsertShopData != null && _InsertShopData.Any())
            {
                DalCouponRules.CreateCouponRuleProductConfigsData(_InsertShopData.Distinct(), "tbl_CouponRules_ConfigShop");
                _InsertShopData = new List<CouponRulesConfig>();
                result++;
            }
            return result;
        }

    }
}
