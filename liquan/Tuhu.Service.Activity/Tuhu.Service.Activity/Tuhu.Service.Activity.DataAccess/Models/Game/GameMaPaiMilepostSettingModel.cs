namespace Tuhu.Service.Activity.DataAccess.Models.Game
{
    /// <summary>
    ///     小游戏 - 马牌里程设置
    /// </summary>
    public class GameMaPaiMilepostSettingModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        ///     外键：关联 - Activity.tbl_Activity
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        ///     里程碑名称
        /// </summary>
        public string MilepostName { get; set; }

        /// <summary>
        ///     里程碑图片
        /// </summary>
        public string MilepostPicUrl { get; set; }

        /// <summary>
        ///     里程碑距离
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        ///     里程碑排序
        /// </summary>
        public int Sort { get; set; }
    }
}
