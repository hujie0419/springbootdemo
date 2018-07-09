using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class TEnrollInfoModel: BaseModel
    {
        /// <summary> Id  </summary>
        public int Id { get; set; }

        /// <summary>
        /// 报名用户姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 区域信息
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        ///报名状态
        /// </summary>
        public int EnrollStatus { get; set; }

        /// <summary>
        /// 新增报名信息时间
        /// </summary>
        public DateTime CreatTime { get; set; }

        /// <summary>
        /// 修改报名信息时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 活动id
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
    }
}
