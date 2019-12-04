using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.QueryModel
{
   public class GetUserActivityApplyListQueryModel
    {
        /// <summary>
      /// 页长
      /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActivityID { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
    }
}
