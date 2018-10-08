using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 查询产品集合
    /// </summary>
    public class QueryProductsListModel
    {
        /// <summary>
        /// 产品库优惠券列表展示数据
        /// </summary>
        public List<QueryProductsModel> QueryProducts = new List<QueryProductsModel>();

        /// <summary>
        /// 产品列表符合条件的总数据行
        /// </summary>
        public int ProductsTotalCount { get; set; }

        /// <summary>
        /// 品牌展示模块数据
        /// </summary>
        public List<FilterConditionModel> CP_BrandList = new List<FilterConditionModel>();

        /// <summary>
        /// 尺寸模块展示数据
        /// </summary>
        public List<FilterConditionModel> CP_Tire_RimList = new List<FilterConditionModel>();

        /// <summary>
        /// 标签模块展示数据
        /// </summary>
        public HashSet<string> CP_tabList = new HashSet<string>();
    }
}