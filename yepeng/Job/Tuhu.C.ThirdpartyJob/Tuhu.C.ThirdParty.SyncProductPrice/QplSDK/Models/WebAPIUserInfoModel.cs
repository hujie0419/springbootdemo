using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Models
{
    [Serializable]
    public class WebAPIUserInfoModel
    {

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        public string ShopName { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 联系信息
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string ProvinceId { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string CityId { get; set; }
        public string TownId { get; set; }
        public string StreetId { get; set; }
        public string Address { get; set; }
        /// 备注
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// 客户类别 1门店2销售3商家 4平台
        /// </summary>
        public int CustomerType { get; set; }
    }
}
