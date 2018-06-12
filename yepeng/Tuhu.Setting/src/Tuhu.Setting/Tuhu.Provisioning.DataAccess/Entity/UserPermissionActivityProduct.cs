using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class UserPermissionActivityProduct : BaseModel
    {
        public UserPermissionActivityProduct() : base() { }
        public UserPermissionActivityProduct(DataRow row) : base(row) { }
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string DisplayName { get; set; }
      
        /// <summary>
        /// 促销价
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 伪原价
        /// </summary>
        public decimal? FalseOriginalPrice { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal? cy_list_price { get; set; }



        /// <summary>
        /// 总限购数量
        /// </summary>
        public int? TotalQuantity { get; set; }
        /// <summary>
        /// 每人限购数量
        /// </summary>
        public int? MaxQuantity { get; set; }
        /// <summary>
        /// 剩余数量
        /// </summary>
        public int? SaleOutQuantity { get; set; }
       
       
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityID { get; set; }
       
        public string Channel { get; set; }
      

    }
}