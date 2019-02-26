using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{

    public class SelectSalePromotionActivityLogModel
    {

    }

    /// <summary>
    /// 促销活动操作日志表实体
    /// </summary>
   public class SalePromotionActivityLogModel
    {
        public string PKID { get; set; } 

        public string ReferId { get; set; }

        public string ReferType { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperationLogType { get; set; }

        public string OperationLogDescription { get; set; }
        
        /// <summary>
        /// 日志详情列表
        /// </summary>
        public List<SalePromotionActivityLogDetail> LogDetailList { get; set; } 

        public string CreateDateTime { get; set; }

        public string CreateUserName { get; set; }

        public int DetailCount { get; set; }

        public SalePromotionActivityLogModel()
        {
            LogDetailList = new List<SalePromotionActivityLogDetail>(); 
        }

    }

    /// <summary>
    /// 操作日志详情表实体
    /// </summary>
    public class SalePromotionActivityLogDetail
    {
         public string FPKID { get; set; } 

        public string OperationLogType { get; set; }

        public string Property { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

    }

    /// <summary>
    /// 操作日志类型描述表实体
    /// </summary>
    public class SalePromotionActivityLogDescription
    {

        public string OperationLogType { get; set; }

        public string OperationLogDescription { get; set; }

        public string Remark { get; set; }

        public string CreateDateTime { get; set; }

        public string CreateUserName { get; set; }

        public string LastUpdateDateTime { get; set; } 

        public string LastUpdateUserName { get; set; }

        //public string SearchOperationLogType { get; set; }

    }

}
