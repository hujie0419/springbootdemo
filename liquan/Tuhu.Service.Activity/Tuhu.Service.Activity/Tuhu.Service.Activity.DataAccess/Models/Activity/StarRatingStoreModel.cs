using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    public class StarRatingStoreModel
    {
        public int PKID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string Duty { get; set; }

        /// <summary>
        /// 省份ID
        /// </summary>
        public int ProvinceID { get; set; }

        /// <summary>
        /// 城市ID
        /// </summary>
        public int CityID { get; set; }

        /// <summary>
        /// 区/县ID
        /// </summary>
        public int DistrictID { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 区/县
        /// </summary>
        public string DistrictName { get; set; }

        /// <summary>
        /// 门店详细地址
        /// </summary>
        public string StoreAddress { get; set; }

        /// <summary>
        /// 门店地址
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 门店面积
        /// </summary>
        public decimal StoreArea { get; set; }

        /// <summary>
        /// 门面数量
        /// </summary>
        public int StoreNum { get; set; }

        /// <summary>
        /// 工位
        /// </summary>
        public int WorkPositionNum { get; set; }

        /// <summary>
        /// 维修资质
        /// </summary>
        public string MaintainQualification { get; set; }

        /// <summary>
        /// 现有门头
        /// </summary>
        public string Storefront { get; set; }


        /// <summary>
        /// 门头备注
        /// </summary>
        public string StorefrontDesc { get; set; }

        /// <summary>
        /// 门店位置
        /// </summary>
        public string StoreLocation { get; set; }

        /// <summary>
        /// 是否同意按照途虎认证店自行制作门头
        /// </summary>
        public bool IsAgree { get; set; }


        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }
}
