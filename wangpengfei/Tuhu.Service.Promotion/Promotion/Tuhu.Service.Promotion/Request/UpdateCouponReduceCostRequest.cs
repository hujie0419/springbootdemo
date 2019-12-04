using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 更新优惠券价格
    /// </summary>
    public class UpdateCouponReduceCostRequest : BaseValidRequest
    {
        /// <summary>
        /// 来源
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 操作原因
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 优惠券所属人
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// 优惠券id
        /// </summary>
        public int PromotionPKID { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public Decimal ReduceCost { get; set; }

        protected override bool IsValid(out string errorMsg)
        {
            StringBuilder sbr = new StringBuilder();
            if (UserID == Guid.Empty)
            {
                sbr.Append("UserID 不能为空。");
            }
            if (PromotionPKID <= 0)
            {
                sbr.Append("PromotionCodeId 必须大于0");
            }
            if (string.IsNullOrWhiteSpace(Channel))
            {
                sbr.Append("Channel 不能为空");
            }
            if (string.IsNullOrWhiteSpace(Author))
            {
                sbr.Append("OperationAuthor 不能为空");
            }
            if (string.IsNullOrWhiteSpace(Message))
            {
                sbr.Append("Message 不能为空");
            }
            if (ReduceCost < 0)
            {
                sbr.Append($"ReduceCost 金额不能小于0");
            }
            errorMsg = sbr.ToString();
            return string.IsNullOrWhiteSpace(errorMsg);
        }
    }
}
