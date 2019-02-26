using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess.Models.Game
{

    /// <summary>
    /// 游戏奖品配置表实体 [Activity].[dbo].[tbl_GamePrizeSetting] 
    /// </summary>
    public class GamePrizeSettingModel
    {

        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 游戏活动id
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        /// 奖品唯一标识(券id)
        /// </summary>
        public Guid PrizeId { get; set; }

        /// <summary>
        /// 奖品的类型：CARMODEL-车模;ENGINEILOIL-机油;CINEMATICKET-电影票;FREECOUPON-免费券
        /// </summary>
        public string PrizeType { get; set; }

        /// <summary>
        /// 奖品展示名称
        /// </summary>
        public string PrizeName { get; set; }

        /// <summary>
        /// 奖品展示图片url
        /// </summary>
        public string PrizePicUrl { get; set; }

        /// <summary>
        /// 奖品总数量
        /// </summary>
        public int PrizeCount { get; set; }

        /// <summary>
        /// 已发放数量
        /// </summary>
        public int GiveCount { get; set; }

        /// <summary>
        /// 每日限量发放数量
        /// </summary>
        public int DayCount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDatetime { get; set; }

        /// <summary>
        /// 上次修改时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }

        /// <summary>
        /// 是否轮播显示 0否 1是
        /// </summary>
        public int IsBroadCastShow { get; set; }

        /// <summary>
        /// 轮播内容
        /// </summary>
        public string BroadCastTitle { get; set; }

        public int IsDeleted { get; set; }

        /// <summary>
        /// 奖品描述、备注
        /// </summary>
        public string PrizeDesc { get; set; }

    }
}
