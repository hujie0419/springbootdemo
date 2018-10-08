using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.TireActivity
{
    public class TireActivityModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 计划编号
        /// </summary>
        public string PlanNumber { get; set; }

        /// <summary>
        /// 计划名称
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// 计划说明
        /// </summary>
        public string PlanDesc { get; set; }

        /// <summary>
        /// PID个数
        /// </summary>
        public int PIDNum { get; set; }

        /// <summary>
        /// 状态。0=未开始，1=运行中，3=已过期， -1=暂停
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 计划开始时间
        /// </summary>
        public DateTime BeginDatetime { get; set; }

        /// <summary>
        /// 计划结束时间
        /// </summary>
        public DateTime EndDatetime { get; set; }

        /// <summary>
        ///创建时间
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


        /// <summary>
        /// 更新ID
        /// </summary>
        public int UpdateID { get; set; }

        /// <summary>
        /// 重复的pid个数
        /// </summary>
        public int repeatPidCount { get; set; }
    }
}
