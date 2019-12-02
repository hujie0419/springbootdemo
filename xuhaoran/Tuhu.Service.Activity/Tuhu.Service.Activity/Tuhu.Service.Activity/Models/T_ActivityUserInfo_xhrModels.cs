using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class T_ActivityUserInfo_xhrModels : BaseModel
    {
        /// <summary>用户ID</summary>
        [Column("UserID")]
        public int UserId { get; set; }
        /// <summary>用户名称</summary>
        public string UserName { get; set; }
        /// <summary>用户手机</summary>
        public string UserTell { get; set; }
        /// <summary>地区ID</summary>
        public int AreaID { get; set; }
        /// <summary>报名状态</summary>
        public int PassStatus { get; set; }
        /// <summary>活动ID</summary>
        public int ActID { get; set; }
        /// <summary>活动标题</summary>
        [Column("Title")]
        public string ActTitle { get; set; }
        /// <summary>用户状态</summary>
        public int UserStatus { get; set; }

    }
}
