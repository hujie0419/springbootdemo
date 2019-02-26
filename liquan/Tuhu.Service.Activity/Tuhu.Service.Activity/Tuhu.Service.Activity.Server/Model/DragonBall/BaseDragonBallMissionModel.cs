namespace Tuhu.Service.Activity.Server.Model.DragonBall
{
    /// <summary>
    ///     游戏类型定义MODEL
    /// </summary>
    public abstract class BaseDragonBallMissionModel
    {
        protected BaseDragonBallMissionModel(DragonBallMissionEnumModel missionEnumModel, string missionName,
            int missionDragonBall, int isCanFinishCount, int? dayFinishCount = null)
        {
            MissionEnumModel = missionEnumModel;
            MissionName = missionName;
            MissionDragonBall = missionDragonBall;
            IsCanFinishCount = isCanFinishCount;
            DayFinishCount = dayFinishCount;
        }

        #region Define_Prototype

        /// <summary>
        ///     游戏任务
        /// </summary>
        public DragonBallMissionEnumModel MissionEnumModel { get; }

        /// <summary>
        ///     游戏任务名称
        /// </summary>
        public string MissionName { get; }

        /// <summary>
        ///     完成任务可以获得的龙珠数
        /// </summary>
        public int MissionDragonBall { get; }

        /// <summary>
        ///     可以完成次数
        /// </summary>
        public int IsCanFinishCount { get; }

        /// <summary>
        ///     每天可以完成的次数
        /// </summary>
        protected int? DayFinishCount { get; }

        #endregion
    }



    /// <summary>
    ///     七龙珠 任务：关注【途虎养车】公众号
    /// </summary>
    public class DragonBallMissionFollowOfficalAccountModel : BaseDragonBallMissionModel
    {
        public DragonBallMissionFollowOfficalAccountModel() : base(DragonBallMissionEnumModel.FollowOfficalAccount,
            "关注【途虎养车】公众号", 1, 1)
        {
        }
    }
    /// <summary>
    ///     七龙珠 任务：绑定个人微信号
    /// </summary>
    public class DragonBallMissionBindPersonalWechatModel : BaseDragonBallMissionModel
    {
        public DragonBallMissionBindPersonalWechatModel() : base(DragonBallMissionEnumModel.BindPersonalWechat,
            "绑定个人微信号", 1, 1)
        {
        }
    }
    /// <summary>
    ///     七龙珠 任务：完成车型认证
    /// </summary>
    public class DragonBallMissionFinishCarCertificationModel : BaseDragonBallMissionModel
    {
        public DragonBallMissionFinishCarCertificationModel() : base(DragonBallMissionEnumModel.FinishCarCertification,
            "完成车型认证", 2, 1)
        {
        }
    }
    /// <summary>
    ///     七龙珠 任务：每日分享游戏
    /// </summary>
    public class DragonBallMissionDailyShareModel : BaseDragonBallMissionModel
    {
        public DragonBallMissionDailyShareModel() : base(DragonBallMissionEnumModel.DailyShare, "每日分享游戏", 1, 5, 1)
        {
        }
    }
    /// <summary>
    ///     七龙珠 任务：购买指定商品
    /// </summary>
    public class DragonBallMissionBuyProductModel : BaseDragonBallMissionModel
    {
        public DragonBallMissionBuyProductModel() : base(DragonBallMissionEnumModel.BuyProduct, "购买指定商品", 3, 1)
        {
        }
    }

    /// <summary>
    ///     七龙珠 任务：邀请新用户
    /// </summary>
    public class DragonBallMissionInviteNewUserModel : BaseDragonBallMissionModel
    {
        public DragonBallMissionInviteNewUserModel() : base(DragonBallMissionEnumModel.InviteNewUser, "邀请新用户", 2, 5)
        {
        }
    }

    /// <summary>
    ///     七龙珠 任务：邀请新用户下单
    /// </summary>
    public class DragonBallMissionInviteNewUserBuyModel : BaseDragonBallMissionModel
    {
        public DragonBallMissionInviteNewUserBuyModel() : base(DragonBallMissionEnumModel.InviteNewUserBuy, "邀请新用户下单",
            3, 5)
        {
        }
    }
}
