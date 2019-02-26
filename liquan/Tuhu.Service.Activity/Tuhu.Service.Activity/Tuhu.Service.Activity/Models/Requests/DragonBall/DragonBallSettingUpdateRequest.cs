using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     七龙珠 - 更新设置
    /// </summary>
    public class DragonBallSettingUpdateRequest
    {
        /// <summary>
        ///     任务大翻盘
        /// </summary>
        public string MissionBigBrand { get; set; }

        /// <summary>
        ///     召唤神龙大翻盘
        /// </summary>
        public string SummonBigBrand { get; set; }

        /// <summary>
        ///     购买任务 商品ID
        /// </summary>
        public string Pids { get; set; }
    }
}
