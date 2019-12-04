using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 延期优惠券
    /// </summary>
    public class DelayCouponEndTimeRequest : BaseValidRequest
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
        /// 优惠券到期时间
        /// </summary>
        public DateTime EndTime { get; set; }

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
            int delayDay = 30;
            if ((EndTime.Date - DateTime.Now.Date).TotalDays > delayDay)
            {
                sbr.Append($"EndTime  延期时间不能大于{delayDay}天");
            }
            errorMsg = sbr.ToString();
            return string.IsNullOrWhiteSpace(errorMsg);
        }
    }
}
