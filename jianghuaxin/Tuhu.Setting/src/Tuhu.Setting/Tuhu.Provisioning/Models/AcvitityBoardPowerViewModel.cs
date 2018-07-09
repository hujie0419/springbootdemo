using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Provisioning.DataAccess.Entity.ActivityBoard;

namespace Tuhu.Provisioning.Models
{
    public class AcvitityBoardPowerViewModel
    {
        /// <summary>
        /// 用户账户
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 模块配置
        /// </summary>
        public ModuleConfig ModuleConfig { get; set; }
    }

    public class ModuleConfig
    {
        /// <summary>
        /// 活动看板权限
        /// </summary>
        public ActivityBoardPermissionConfig TuhuActivityPower { get; set; }
        /// <summary>
        /// 第三方活动权限
        /// </summary>
        public ActivityBoardPermissionConfig ThirdPartyActivityPower { get; set; }
        /// <summary>
        /// 外部活动权限
        /// </summary>
        public ActivityBoardPermissionConfig OutSideActivityPower { get; set; }

    }
}