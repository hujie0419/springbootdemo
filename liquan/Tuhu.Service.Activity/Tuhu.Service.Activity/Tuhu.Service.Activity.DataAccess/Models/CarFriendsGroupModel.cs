using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess.Models
{
    /// <summary>
    /// 车友群
    /// </summary>
    public class CarFriendsGroupModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 车友群名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 车友群描述
        /// </summary>
        public string GroupDesc { get; set; }

        /// <summary>
        /// 绑定车型
        /// </summary>
        public string BindVehicleType { get; set; }

        /// <summary>
        /// 绑定车型ID
        /// </summary>
        public int BindVehicleTypeID { get; set; }

        /// <summary>
        /// 群头像
        /// </summary>
        public string GroupHeadPortrait { get; set; }

        /// <summary>
        /// 群二维码
        /// </summary>
        public string GroupQRCode { get; set; }

        /// <summary>
        /// 群类别，0=车型群，1=工厂店群，2=地区群，3=福利群，4=拼团群
        /// </summary>
        public int GroupCategory { get; set; }

        /// <summary>
        /// 群权重（数字越小，权重越高）
        /// </summary>
        public int GroupWeight { get; set; }

        /// <summary>
        /// 是否推荐
        /// </summary>
        public bool IsRecommend { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Is_Deleted { get; set; }

        /// <summary>
        /// 群创建时间
        /// </summary>
        public DateTime GroupCreateTime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime GroupOverdueTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string LastUpdateBy { get; set; }
    }
}
