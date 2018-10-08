using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.CommonEnum;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BeautyProductModel
    {
        /// <summary>
        /// 美容产品Id
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 美容产品PID
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 美容产品名字
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 美容产品描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 美容产品分类Id
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 美容产品分类名
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public Decimal Commission { get; set; }
        /// <summary>
        /// 美容产品的适配车型
        /// </summary>
        public MrVehicleType RestrictVehicleType { get; set; }
    }
}
