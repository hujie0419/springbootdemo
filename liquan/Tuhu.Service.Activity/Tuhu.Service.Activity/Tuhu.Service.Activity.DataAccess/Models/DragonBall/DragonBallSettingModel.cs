namespace Tuhu.Service.Activity.DataAccess.Models.DragonBall
{
    /// <summary>
    ///     七龙珠 - 设置表
    /// </summary>
    public class DragonBallSettingModel
    {
        /// <summary>
        ///     完成任务的大翻盘ID
        /// </summary>
        public string MissionBigBrand { get; set; }

        /// <summary>
        ///     召唤神龙的大翻盘ID
        /// </summary>
        public string SummonBigBrand { get; set; }

        /// <summary>
        ///     产品ID
        /// </summary>
        public string Pids { get; set; }
    }
}
