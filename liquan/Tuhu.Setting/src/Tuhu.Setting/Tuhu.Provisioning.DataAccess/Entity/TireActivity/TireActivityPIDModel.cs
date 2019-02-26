using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.TireActivity
{
    public class TireActivityPIDModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 轮胎活动PKID
        /// </summary>
        public int TireActivityID { get; set; }

        /// <summary>
        /// 轮胎PID
        /// </summary>
        public string PID { get; set; }

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

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Is_Deleted { get; set; }
    }
}
