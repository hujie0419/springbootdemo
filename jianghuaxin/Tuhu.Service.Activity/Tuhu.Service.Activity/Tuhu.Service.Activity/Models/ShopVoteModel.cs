using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class ShopVoteBaseModel: BaseModel
    {
        /// <summary>主键</summary>
        public long PKID { get; set; }
        /// <summary>
        /// 门店id
        /// </summary>
        public long ShopId { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string ShopName { get; set; }
        public string Image { get; set; }
        /// <summary>
        /// 图片URL 数组 (JSON)
        /// </summary>
        public string ImageUrls { get; set; }
        [IgnoreDataMember]
        [Obsolete("参数已过期，请使用ImageList")]
        public string[] ImageArray { get; set; }
        /// <summary>
        /// 只作为返回参数，不作为请求参数
        /// </summary>
        public List<string> ImageList { get; set; }
        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        /// <summary>
        /// 全国排名
        /// </summary>
        public int Ranking { get; set; }
        /// <summary>
        /// 省份排名
        /// </summary>
        public int ProvinceRanking { get; set; }
        /// <summary>
        /// 城市排名
        /// </summary>
        public int CityRanking { get; set; }
        /// <summary>
        /// 票数
        /// </summary>
        public int VoteNumber { get; set; }
    }

    public class ShopVoteModel:ShopVoteBaseModel
    {
       
        /// <summary>
        /// 建店时间
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 营业面积
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 员工数
        /// </summary>
        public int EmployeeCount { get; set; }
        /// <summary>
        /// 个性化介绍
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 视频介绍地址
        /// </summary>
        public string VideoUrl { get; set; }
        /// <summary>
        /// 视频缩略图
        /// </summary>
        public string VideoThumbImage { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
