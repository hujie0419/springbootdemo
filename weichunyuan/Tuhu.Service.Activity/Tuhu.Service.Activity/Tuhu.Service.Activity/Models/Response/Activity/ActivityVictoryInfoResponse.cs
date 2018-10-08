using System;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     活动胜利信息
    /// </summary>  
    public class ActivityVictoryInfoResponse
    {
        /// <summary>
        ///     胜利次数
        /// </summary>
        public int VictoryNumber { get; set; }

        /// <summary>
        ///     称号 （足球小将）
        /// </summary>
        public string VictoryTitle { get; set; }
    }
}
