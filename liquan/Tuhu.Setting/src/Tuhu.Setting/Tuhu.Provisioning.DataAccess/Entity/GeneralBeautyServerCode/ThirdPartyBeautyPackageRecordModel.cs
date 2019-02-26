using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.GeneralBeautyServerCode
{
    public class ThirdPartyBeautyPackageRecordModel : ThirdPartyBeautyPackageConfigModel
    {
        public int PKID { get; set; }
        /// <summary>
        /// 服务包Id
        /// </summary>
        public Guid PackageId { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedDateTimeString
        {
            get
            {
                return CreatedDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedDateTime { get; set; }
        /// <summary>
        /// 状态（0=默认,1=已发放，2=已作废）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 2B订单id
        /// </summary>
        public int OrderId { get; set; }
    }

    public class ThirdPartyBeautyPackageProductRegionConfigModel
    {
        /// <summary>
        /// 包产品限购地区主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 包产品id
        /// </summary>
        public int PackageProductId { get; set; }
        /// <summary>
        /// 省id
        /// </summary>
        public int ProvinceId { get; set; }
        /// <summary>
        /// 城市id  逗号分隔
        /// </summary>
        public string CityIds { get; set; }
        public IEnumerable<int> CityIdArray
        {
            get
            {
                IEnumerable<int> result = new List<int>();
                if (!string.IsNullOrEmpty(CityIds))
                    result = CityIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToInt32(s));
                return result;
            }
        }
        /// <summary>
        /// 是否所有城市
        /// </summary>
        public bool IsAllCitys { get; set; }

    }
}
