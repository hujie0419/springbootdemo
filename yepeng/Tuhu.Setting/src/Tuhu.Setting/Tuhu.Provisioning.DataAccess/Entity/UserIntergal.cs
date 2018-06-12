using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 用户积分详情
    /// </summary>
    public class UserIntergalDetail
    {
        public Guid IntegralDetailID { get; set; }

        /// <summary>
        /// 积分操作描述
        /// </summary>
        public string TransactionDescribe { get; set; }

        public Guid IntegralRuleID { get; set; }

        /// <summary>
        /// 积分数量
        /// </summary>
        public int TransactionIntegral { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// 交易类型（0：收入，1：支出）
        /// </summary>
        public bool? IntegralType { get; set; }

        /// <summary>
        /// 交易状态（1：有效，0：无效）
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// 用于查看日志表中是否有记录
        /// </summary>
        public int id { get; set; }
    }

    /// <summary>
    /// 用户基本信息
    /// </summary>
    public class UserIntergalMessage
    {
        public Guid UserID { get; set; }
        public string u_mobile_number { get; set; }
        public string Integral { get; set; }
        public Guid IntegralID { get; set; }
    }
    /// <summary>
    ///查询修改日志
    /// </summary>
    public class UpdateLogModal
    {
        public string PKID { get; set; }
        public string IntegralDetailID { get; set; }
        public string Author { get; set; }
        public string ChangeResult { get; set; }
        public string UpdateTime { get; set; }
        public string Reason { get; set; }
    }

    /// <summary>
    /// 修改用户积分
    /// </summary>
    public class InsertIntergalModal
    {
        public string Author { get; set; }
        /// <summary>
        /// 数据库中的积分
        /// </summary>
        public int Integral { get; set; }

        /// <summary>
        ///用户Id
        /// </summary>
        public Guid UserId { get; set; }

        public Guid IntegralDetailID { get; set; }
        public Guid IntegralID { get; set; }

        /// <summary>
        ///积分的规则Id
        /// </summary>
        public Guid IntegralRuleID { get; set; }

        /// <summary>
        /// 增加的积分
        /// </summary>
        public int TransactionIntegral { get; set; }

        public string TransactionRemark { get; set; }

        /// <summary>
        /// 增加的积分的描述原因
        /// </summary>
        public string TransactionDescribe { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string TransactionChannel { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Versions { get; set; }
    }
}                                                                                           
