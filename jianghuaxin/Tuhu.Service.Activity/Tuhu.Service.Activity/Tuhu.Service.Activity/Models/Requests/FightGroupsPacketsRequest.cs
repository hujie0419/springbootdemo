using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 更新红包人请求参数
    /// </summary>
   public class FightGroupsPacketsUpdateRequest
    {
       // int pkid, Guid userId,Guid fightGroupsIdentity

        /// <summary>
        /// 对应红包的PKID
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 用户的UserId
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 红包组
        /// </summary>
        public Guid FightGroupsIdentity { get; set; }

    }
}
