using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.CommentStatisticsJob.Model
{
    /// <summary>
    /// 门店评论类型
    /// </summary>
    public enum ShopStatisticsType
    {
        TR = 0,
        BY = 1,
        MR = 2,
        PQ = 3,
        FW = 4,
    }

    /// <summary>
    /// 公共 变量
    /// </summary>
    public class CommonUtilsModel
    {
        /// <summary>
        /// 可评论
        /// </summary>
        public static int CanComment = 6;//月数
        /// <summary>
        /// 可回评
        /// </summary>
        public static int CanReply = 3;//月数
    }

}
