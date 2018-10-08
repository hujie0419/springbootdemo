using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tuhu.Provisioning.DataAccess.Entity.VipBaoYangPackage
{
    /// <summary>
    /// 单次保养套餐DB Model
    /// </summary>
    public class VipBaoYangPackageViewModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// PID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 套餐名称
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// 大客户UserId
        /// </summary>
        public Guid VipUserId { get; set; }

        public List<BaoYangPackageOilConfig> OilConfigs { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 机油升数
        /// </summary>
        public double? Volume { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        public string SettlementMethod { get; set; }

        public VipBaoYangPackageViewModel(VipBaoYangPackageDbModel package)
        {
            if (package != null)
            {

                this.OilConfigs = package?.OilConfigs.OrderBy(x => x.PKID).Select(x => new BaoYangPackageOilConfig
                {
                    Brand = x.Brand,
                    Grade = x.Grade,
                    Series = x.Series,
                }).ToList() ?? new List<BaoYangPackageOilConfig>();
                this.PKID = package.PKID;
                this.PID = package.PID;
                this.PackageName = package.PackageName;
                this.CreateUser = package.CreateUser;
                this.Price = package.Price;
                this.SettlementMethod = package.SettlementMethod;
                this.VipUserId = package.VipUserId;
                this.Volume = package.Volume;
            }
        }

        public VipBaoYangPackageViewModel() { }

        #region Other Property

        public string VipUserName { get; set; }

        #endregion

        /// <summary>
        /// success:
        ///     string.Empty 
        /// </summary>
        /// <returns></returns>
        public string GetValidationResults()
        {
            #region PackageName

            if (string.IsNullOrWhiteSpace(this.PackageName))
            {
                return "套餐名称不能为空";
            }
            this.PackageName = this.PackageName.Trim();

            #endregion

            #region SettlementMethod

            SettlementMethod method;
            if (!Enum.TryParse(this.SettlementMethod, out method))
            {
                return "结算方式不存在";
            }
            this.SettlementMethod = method.ToString();

            #endregion

            #region OilConfigs

            var gradesTmpl = new List<string> { "矿物油", "半合成", "全合成" };
            this.OilConfigs = this.OilConfigs?.Where(config => !string.IsNullOrEmpty(config.Brand) &&
                (string.IsNullOrEmpty(config.Grade) || gradesTmpl.Contains(config.Grade))).ToList();
            if (this.OilConfigs == null || !this.OilConfigs.Any())
            {
                return "品牌不能为空";
            }

            var count = this.OilConfigs.Count;
            var distinctCount = this.OilConfigs.Select(x => new { x.Brand, x.Series, x.Grade }).Distinct().Count();

            if (count != distinctCount)
            {
                return "品牌配置不能重复";
            }

            var groups = this.OilConfigs.GroupBy(x => x.Brand);
            if (groups.Any(g => g.Any(x => string.IsNullOrEmpty(x.Grade)) && g.Any(x => !string.IsNullOrEmpty(x.Grade))))
            {
                return "等级限制不能为空";
            }

            #endregion

            return string.Empty;
        }
    }

    /// <summary>
    /// 保养套餐机油配置
    /// </summary>
    public class BaoYangPackageOilConfig
    {
        public string Brand { get; set; }

        public string Grade { get; set; }

        public string Series { get; set; }
    }

    [Obsolete]
    public class BaoYangPackageOilBrand
    {
        public string Brand { get; set; }

        public List<int> Grades { get; set; }
    }

}
