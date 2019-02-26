using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
  public  class SalePromotionAuditAuth
    {
        public int PKID { get; set; }

        /// <summary>
        /// 权限id
        /// </summary>
        public string AuthId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// 促销方式 1.打折
        /// </summary>
        public int PromotionType { get; set; }

        /// <summary>
        /// 角色类型  SuperManeger.超级管理员 Maneger.管理员
        /// </summary>
        public string RoleType { get; set; }

        public string CreateDateTime { get; set; }

        public string CreateUserName { get; set; }

        public string LastUpdateDateTime { get; set; }

        public string LastUpdateUserName { get; set; }

    }
}
