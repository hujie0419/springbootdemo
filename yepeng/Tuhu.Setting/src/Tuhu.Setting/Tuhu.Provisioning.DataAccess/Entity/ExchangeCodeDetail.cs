using System.Data;
using Tuhu.Component.Common.Models;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ExchangeCodeDetail : BaseModel
    {
        public ExchangeCodeDetail() : base() { }
        public ExchangeCodeDetail(DataRow dr) : base(dr) { }
        public int PKID { get; set; }
        public int ParentID { get; set; }
        public string Name { get; set; }
        public int CodeType { get; set; }
        public int Number { get; set; }
        public double Money { get; set; }
        public double MinMoney { get; set; }

        public string CodeStartTime { get; set; }
        public string CodeEndTime { get; set; }
        public string ExChangeStartTime { get; set; }
        public string ExChangeEndTime { get; set; }
        public bool IsActive { get; set; }
        public int Validity { get; set; }
        public int DetailsID { get; set; }
        public string CodeChannel { get; set; }
        public int RuleId { get; set; }

        public string Code { get; set; }
        public int FaFangNum { get; set; }
        public int YiDuiHuan { get; set; }
        public int YiShiYong { get; set; }
        public int WeiDuiHuan { get; set; }
        public string CreateTime { get; set; }
        public string Channels { get; set; }
        public int CreateNumber { get; set; }
        public string QName { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public int DepartmentId { get; set; }
        /// <summary>
        /// 用途id
        /// </summary>
        public int IntentionId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creater { get; set; }
        /// <summary>
        /// 发放人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 仅新用户使用
        /// </summary>
        public bool OnlyForNewUser { get; set; }
        /// <summary>
        /// 限领数量
        /// </summary>
        public int? LimitNum { get; set; }
    }
}