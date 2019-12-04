using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    public class ObsoleteCouponsByChannelRequest : BaseValidRequest
    {
        /// <summary>
        /// 作废来源
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
        /// 优惠券渠道 
        /// </summary>
        public string CodeChannel { get; set; }

        /// <summary>
        /// 优惠券最小id
        /// </summary>
        public int MinPromotionCodePKID { get; set; }

        /// <summary>
        /// 优惠券最大id
        /// </summary>
        public int MaxPromotionCodePKID { get; set; }



        protected override bool IsValid(out string errorMsg)
        {
            StringBuilder sbr = new StringBuilder();
            if (string.IsNullOrWhiteSpace(Channel))
            {
                sbr.Append("Channel 不能为空");
            }
            if (string.IsNullOrWhiteSpace(Author))
            {
                sbr.Append("Author 不能为空");
            }
            if (string.IsNullOrWhiteSpace(Message))
            {
                sbr.Append("Message 不能为空");
            }
            if (string.IsNullOrWhiteSpace(CodeChannel))
            {
                sbr.Append("CodeChannel 不能为空");
            }
            if (MinPromotionCodePKID <=0)
            {
                sbr.Append("MinPromotionCodePKID 必须大于0");
            }
            if (MaxPromotionCodePKID <= 0)
            {
                sbr.Append("MaxPromotionCodePKID 必须大于0");
            }
            errorMsg = sbr.ToString();
            return string.IsNullOrWhiteSpace(errorMsg);
        }
    }
}
