using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// 车友群列表信息
    /// </summary>
    public class CarFriendsGroupInfoResponse
    {
        /// <summary>
        /// 车友群列表
        /// </summary>
        public List<CarFriendsGroup> groupList { get; set; }

        /// <summary>
        /// 车友群数量
        /// </summary>
        public int groupCount { get; set; }

        /// <summary>
        /// 车友群类
        /// </summary>
        public class CarFriendsGroup
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
            public string BindVehicleTypeID { get; set; }

            /// <summary>
            /// 群头像
            /// </summary>
            public string GroupHeadPortrait { get; set; }

            /// <summary>
            /// 群二维码
            /// </summary>
            public string GroupQRCode { get; set; }

            /// <summary>
            /// 群权重（数字越小，权重越高）
            /// </summary>
            public int GroupWeight { get; set; }

            /// <summary>
            /// 是否推荐
            /// </summary>
            public bool IsRecommend { get; set; }

        }
    }    
}
