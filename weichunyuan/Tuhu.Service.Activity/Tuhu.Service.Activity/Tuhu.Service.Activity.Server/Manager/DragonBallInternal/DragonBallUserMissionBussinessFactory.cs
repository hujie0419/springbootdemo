using System;
using System.Collections.Generic;
using Tuhu.Service.Activity.DataAccess.Models.DragonBall;
using Tuhu.Service.Activity.Server.Model.DragonBall;


namespace Tuhu.Service.Activity.Server.Manager.DragonBallInternal
{
    /// <summary>
    ///     用户任务业务Factory
    /// </summary>
    internal static class DragonBallUserMissionBussinessFactory
    {

        internal static IBaseDragonBallUserMissionBussinessModel Create(BaseDragonBallMissionModel baseDragonBallMissionModel, IList<DragonBallUserMissionModel> userMissionModels)
        {
            var type = baseDragonBallMissionModel.GetType();
            if (type == typeof(DragonBallMissionDailyShareModel))
            {
                return new DragonBallUserMissionDailyShareBussinessModel(baseDragonBallMissionModel,userMissionModels);
            }
            else
            {
                return new DefaultDragonBallUserMissionBussinessModel(baseDragonBallMissionModel, userMissionModels);
            }

        }


    }
}
