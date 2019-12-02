using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class T_ActivityManagerUserInfo_xhrModel
    {
        /// <summary>管理员ID</summary>
        public int ManagerId { get; set; }
        /// <summary>账号</summary>
        public string Name { get; set; }
        /// <summary>密码盐</summary>
        public string PassWordsSalt { get; set; }
        /// <summary>密码</summary>
        public string PassWords { get; set; }
    }
}
