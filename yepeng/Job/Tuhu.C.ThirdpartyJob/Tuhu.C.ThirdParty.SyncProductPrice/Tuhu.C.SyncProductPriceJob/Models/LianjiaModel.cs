using System.Collections.Generic;

namespace Tuhu.C.SyncProductPriceJob.Models
{
    public abstract class BaseLianjiaModel
    {
        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }

    public class LianjiaCityModel : BaseLianjiaModel
    {
        /// <summary>
        /// 是否有小区模块
        /// </summary>
        public bool HasXiaoqu { get; set; }

        /// <summary>
        /// Districts
        /// </summary>
        public IReadOnlyCollection<LianjiaDistrictModel> Districts { get; set; }
    }

    public class LianjiaDistrictModel : BaseLianjiaModel
    {
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        public List<LianjiaXiaoquModel> XiaoquList { get; set; } = new List<LianjiaXiaoquModel>(512);
    }

    public class LianjiaXiaoquModel : BaseLianjiaModel
    {
        /// <summary>
        /// 小区id
        /// </summary>
        public long XiaoquId { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 建筑年代
        /// </summary>
        public string Age { get; set; }

        /// <summary>
        /// 建筑类型
        /// </summary>
        public string BuildingType { get; set; }

        /// <summary>
        /// 物业费用
        /// </summary>
        public string WuyeFee { get; set; }

        /// <summary>
        /// 物业公司
        /// </summary>
        public string WuyeCompany { get; set; }

        /// <summary>
        /// 开发商
        /// </summary>
        public string Developer { get; set; }

        /// <summary>
        /// 楼栋总数
        /// </summary>
        public string BuildingNum { get; set; }

        /// <summary>
        /// 房屋总数
        /// </summary>
        public string HouseNum { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string Longtitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// 价格（每平米）
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 小区所属区域
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 地标
        /// </summary>
        public string Remark1 { get; set; }
    }
}
