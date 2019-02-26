using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tuhu.Component.Common.Models;
using System.Data.SqlClient;
using Tuhu.Component.Common;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tuhu.Provisioning.Models
{
    public class ProductMappingModel : BaseModel
    {
        [Required]
        public string ShopCode { get; set; }
        [Required]
        public string Pid { get; set; }
        public string DisplayName { get; set; }
        [Required]
        public long? ItemID { get; set; }
        public long SkuID { get; set; }
        public decimal? Price { get; set; }
        public string Title { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }

        /// <summary>
        /// 新加字段 给养车无忧使用
        /// </summary>
        public string ItemCode { get; set; }

        public ProductMappingModel() : base() { }

        public ProductMappingModel(DataRow row) : base(row) { }
    }

}