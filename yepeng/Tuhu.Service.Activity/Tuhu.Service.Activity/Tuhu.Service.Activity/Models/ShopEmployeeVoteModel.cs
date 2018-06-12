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

    public class ShopEmployeeVoteBaseModel:BaseModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long PKID { get; set; }
        /// <summary>
        /// 门店I的
        /// </summary>
        public long ShopId { get; set; }
        public string ShopName { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 技师头像
        /// </summary>
        public string EmployeeAvatar { get; set; }
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
        /// <summary>
        /// 技师id
        /// </summary>
        public long EmployeeId { get; set; }
        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        /// <summary>
        /// 全国排名
        /// </summary>
        public int Ranking { get; set; }
        /// <summary>
        /// 省份排名
        /// </summary>
        public int ProvinceRanking{ get; set; }
        /// <summary>
        /// 城市排名
        /// </summary>
        public int CityRanking { get; set; }
        /// <summary>
        /// 票数
        /// </summary>
        public int VoteNumber { get; set; }
    }

    public class ShopEmployeeVoteModel: ShopEmployeeVoteBaseModel
    {
        
       
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 从业年数
        /// </summary>
        public int YearsEmployed { get; set; }
        /// <summary>
        /// 兴趣爱好
        /// </summary>
        public string Hobby { get; set; }
        /// <summary>
        /// 专长车型
        /// </summary>
        public string ExpertiseModels { get; set; }
        /// <summary>
        /// 专长项目
        /// </summary>
        public string ExpertiseProjects { get; set; }
        /// <summary>
        /// 个性化介绍
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 视频URL
        /// </summary>
        public string VideoUrl { get; set; }
        /// <summary>
        /// 视频缩略图
        /// </summary>
        public string VideoThumbImage { get; set; }
        /// <summary>
        /// 30天内订单
        /// </summary>
        public int ThirtyDaysOrderNum { get; set; }
        /// <summary>
        /// 服务好评率
        /// </summary>
        public decimal ServiceFavRate { get; set; }
        /// <summary>
        /// 服务态度
        /// </summary>
        public decimal ServiceAttitude { get; set; }
        /// <summary>
        /// 技术能力
        /// </summary>
        public decimal TechnicalCapacity { get; set; }
        public string Comments { get; set; }
        public List<ShopEmployeVoteCommentModel> CommentArray { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    public class ShopEmployeVoteCommentModel:BaseModel
    {
        public long PKID { get; set; }
        public long ShopId { get; set; }
        public long EmployeeId { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public double Score { get; set; }
        public DateTime CreateTime { get; set; }
        public string Content { get; set; }
        public string ImageUrls { get; set; }
        [IgnoreDataMember]
        [Obsolete]
        public List<string> ImageArray { get; set; }
    }
}
