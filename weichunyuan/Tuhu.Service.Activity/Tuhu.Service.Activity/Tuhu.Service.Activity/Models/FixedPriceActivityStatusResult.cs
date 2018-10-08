using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class FixedPriceActivityStatusResult
    {
        /// <summary>
        /// NotStart, End, Ing, WaitNext, OutOfStock, UserEnd
        /// </summary>
        public string ActivityStatus { get; set; }

        /// <summary>
        /// 活动价格
        /// </summary>
        public decimal ActivityPrice { get; set; }

        /// <summary>
        /// 下次活动时间或者活动开始的时间
        /// </summary>
        public DateTime ActivityTime { get; set; }

        /// <summary>
        /// 提示文字颜色
        /// </summary>
        public string TipTextColor { get; set; }

        /// <summary>
        /// 主按钮背景颜色
        /// </summary>
        public string ButtonBackgroundColor { get; set; }

        /// <summary>
        /// 主按钮字体颜色
        /// </summary>
        public string ButtonTextColor { get; set; }

        /// <summary>
        /// 背景图片
        /// </summary>
        public string BackgroundImg { get; set; }

        public string OngoingButtonText { get; set; }
    }
}
