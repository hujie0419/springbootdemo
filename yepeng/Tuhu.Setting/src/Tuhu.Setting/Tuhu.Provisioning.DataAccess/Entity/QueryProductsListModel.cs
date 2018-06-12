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
        public List<QueryProductsModel> QueryProducts = new List<QueryProductsModel>();
        public List<FilterConditionModel> CP_BrandList = new List<FilterConditionModel>();
        public List<FilterConditionModel> CP_Tire_RimList = new List<FilterConditionModel>();
        public HashSet<string> CP_tabList = new HashSet<string>();
    }
}