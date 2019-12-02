using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class T_ActivityUserInfo_xhrModel
    {
        /// <summary>用户ID</summary>
        public string UserID { get; set; }
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
        public string Title { get; set; }
        /// <summary>用户状态</summary>
        public int UserStatus { get; set; }
    }
}
