using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 获取用户的问卷链接信息请求类
    /// </summary>
    public class GetQuestionnaireURLRequest
    {
        /// <summary>
        /// 订单标识（售前问卷必填）
        /// </summary>
        public int? OrderID { get; set; }
        /// <summary>
        /// 投诉标识（售后问卷必填）
        /// </summary>
        public int ComplaintsID { get; set; }
        /// <summary>
        /// 投诉类型，多级用英文逗号分隔
        /// </summary>
        public string ComplaintsType { get; set; }
        /// <summary>
        /// 是否到店，1=到家；2=到店（责任部门是物流时必填）
        /// </summary>
        public int? IsAtStore { get; set; }
        /// <summary>
        /// 定责部门（售后问卷必填）
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 用户标识
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// 用户手机号（售前问卷必填）
        /// </summary>
        public string UserPhone { get; set; }
        /// <summary>
        /// 员工邮箱（售前问卷必填）
        /// </summary>
        public string StaffEmail { get; set; }
        /// <summary>
        /// 问卷类型，1=售后问卷；2=售前问卷（售前问卷必填）
        /// </summary>
        public int QuestionnaireType { get; set; }
    }
}
