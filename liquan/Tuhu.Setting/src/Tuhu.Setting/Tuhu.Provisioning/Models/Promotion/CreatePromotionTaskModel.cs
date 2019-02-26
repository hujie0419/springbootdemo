using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Models
{
    public class CreatePromotionTaskModel
    {
        public PromotionTask PromotionTask { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public List<Dictionary> ListOrderStatus { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public List<Dictionary> ListOrderType { get; set; }
        /// <summary>
        /// 订单渠道
        /// </summary>
        public List<Dictionary> ListOrderChannel { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public List<Region> ListProvince { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public List<Region> ListRegion { get; set; }

        public List<string> BrandInfos { get; set; }

        public List<SKUProductCategory> CategoryOne { get; set; }

        public List<SKUProductCategory> CategoryTwo { get; set; }

        public IDictionary<string, string> VehicleInfos { get; set; }

        public String FilterItemJson { get; set; }

        public DataTable RuleData { get; set; }

        public DataTable BrandData { get; set; }

        public DataTable FinanceMarkData { get; set; }

        public List<DepartmentAndUse> DepartmentAndUseData { get; set; }

        public DataTable PromotionTaskActivityData { get; set; }
        public DataTable BusinessData { get; set; }
    }
    public class DepartmentAndUse
    {
        /// <summary>
        /// 标识
        /// </summary>
        public string SettingId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 类型（0=部门，1=用途）
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected { get; set; }
        public string ParentSettingId { get; set; }
    }
}