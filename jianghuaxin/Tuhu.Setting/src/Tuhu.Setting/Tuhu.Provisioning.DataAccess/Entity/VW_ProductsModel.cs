using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class VW_ProductsModel
    {
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool isChecked { get; set; }
        public int oid { get; set; }
        public string PID { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 1上架，0下架
        /// </summary>
        public int OnSale { get; set; }

        /// <summary>
        /// 1缺货，0正常 库存
        /// </summary>
        public int Stockout { get; set; }

        /// <summary>
        /// 途虎价
        /// </summary>
        public double CY_List_Price { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        public double CY_Marketing_Price { get; set;}

        public decimal CostPrice { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string CP_Brand { get; set; }

        /// <summary>
        /// 地域
        /// </summary>
        public string CP_Place { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string CP_Tab { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string TireSize { get; set; }

        /// <summary>
        /// 产品图片
        /// </summary>
        public string Image { get; set; }
    }
}
