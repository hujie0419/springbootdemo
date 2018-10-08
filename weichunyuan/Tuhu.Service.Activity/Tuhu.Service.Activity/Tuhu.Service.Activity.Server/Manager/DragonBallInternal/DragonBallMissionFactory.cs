using System.Collections.Generic;
using System.Linq;
using Tuhu.Service.Activity.Server.Model.DragonBall;

namespace Tuhu.Service.Activity.Server.Manager.DragonBallInternal
{
    /// <summary>
    ///     七龙珠 - 任务工厂
    /// </summary>
    internal static class DragonBallMissionFactory
    {
        private static readonly IList<BaseDragonBallMissionModel> DragonBallMissionCollection = new List<BaseDragonBallMissionModel>();
        static DragonBallMissionFactory()
        {
            // 生成任务列表
            DragonBallMissionCollection.Add(new DragonBallMissionFollowOfficalAccountModel());
            DragonBallMissionCollection.Add(new DragonBallMissionBindPersonalWechatModel());
            DragonBallMissionCollection.Add(new DragonBallMissionFinishCarCertificationModel());
            DragonBallMissionCollection.Add(new DragonBallMissionDailyShareModel());
            DragonBallMissionCollection.Add(new DragonBallMissionBuyProductModel());
            DragonBallMissionCollection.Add(new DragonBallMissionInviteNewUserModel());
            DragonBallMissionCollection.Add(new DragonBallMissionInviteNewUserBuyModel());
        }

        /// <summary>
        ///     获取所有的任务
        /// </summary>
        /// <returns></returns>
        internal static IList<BaseDragonBallMissionModel> GetAllMissions()
        {
            return DragonBallMissionCollection;
        }

        /// <summary>
        ///     通过任务ID获得对应的任务
        /// </summary>
        /// <param name="missionId"></param>
        /// <returns></returns>
        internal static BaseDragonBallMissionModel GetByMissionId(int missionId)
        {
            return DragonBallMissionCollection.FirstOrDefault(p => (int) p.MissionEnumModel == missionId);
        }
    }
}
