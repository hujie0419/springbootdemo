using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.Service.Activity.DataAccess.Models.DragonBall;

namespace Tuhu.Service.Activity.Server.Model.DragonBall
{

    /// <summary>
    ///     用户任务MODEL
    /// </summary>
    internal interface IBaseDragonBallUserMissionBussinessModel
    {
        /// <summary>
        ///     任务ID
        /// </summary>
        int MissionId { get; }

        /// <summary>
        ///     任务用户状态
        /// </summary>
        int MissionUserStatus { get; }

        /// <summary>
        ///     龙珠数量
        /// </summary>
        int DragonBallCount { get; }

        /// <summary>
        ///     是否可领取
        /// </summary>
        bool CanRecive();
    }



    internal abstract class BaseDragonBallUserMissionBussinessModel<T>  : IBaseDragonBallUserMissionBussinessModel where T : BaseDragonBallMissionModel
    {
        protected readonly BaseDragonBallMissionModel _missionDefineModel;
        protected readonly IList<DragonBallUserMissionModel> _userMissionModels;

        internal BaseDragonBallUserMissionBussinessModel(BaseDragonBallMissionModel missionDefineModel, IList<DragonBallUserMissionModel> userMissionModels)
        {
            _missionDefineModel = missionDefineModel;
            _userMissionModels = userMissionModels;
        }

        /// <summary>
        ///     任务ID
        /// </summary>
        public virtual int MissionId => (int)_missionDefineModel.MissionEnumModel;

        /// <summary>
        ///     任务用户状态 0 去做任务  1 可领取  2 已领取    
        /// </summary>
        public virtual int MissionUserStatus
        {
            get
            {
                // 一个任务是可领取 - 那么就是可领取
                if (_userMissionModels.Any(p=>p.MissionStatus ==1))
                {
                    return 1;
                }

                // 当前完成的任务 >= 任务最多完成数 - 那么就是 已领取
                if (_userMissionModels.Count() >= _missionDefineModel.IsCanFinishCount)
                {
                    return 2;
                }

                // 否则就是 - 去做任务
                return 0;
            }
        }

        /// <summary>
        ///     龙珠数量
        /// </summary>
        public virtual int DragonBallCount => _missionDefineModel.MissionDragonBall;

        /// <summary>
        ///  是否可领取
        /// </summary>
        /// <returns></returns>
        public bool CanRecive()
        {
             return MissionUserStatus == 1;
        }
         
    }



    internal class DefaultDragonBallUserMissionBussinessModel : BaseDragonBallUserMissionBussinessModel<BaseDragonBallMissionModel>
    {
        public DefaultDragonBallUserMissionBussinessModel(BaseDragonBallMissionModel missionDefineModel, IList<DragonBallUserMissionModel> userMissionModels) : base(missionDefineModel, userMissionModels)
        {

        }
    }

    internal class DragonBallUserMissionDailyShareBussinessModel : BaseDragonBallUserMissionBussinessModel<DragonBallMissionDailyShareModel>
    {
        public DragonBallUserMissionDailyShareBussinessModel(BaseDragonBallMissionModel missionDefineModel, IList<DragonBallUserMissionModel> userMissionModels) : base(missionDefineModel, userMissionModels)
        {
        }

        /// <summary>
        ///     任务用户状态 0 去做任务  1 可领取  2 已领取 3 今日已分享
        /// </summary>
        public override int MissionUserStatus {
            get
            {

                // 最后一个任务是可领取 - 那么就是可领取
                if (_userMissionModels.Any(p => p.MissionStatus == 1))
                {
                    return 1;
                }

                // 当前完成的任务 >= 任务最多完成数 - 那么就是 已领取
                if (_userMissionModels.Count() >= _missionDefineModel.IsCanFinishCount)
                {
                    return 2;
                }

                // 如果今天已经分享
                if (_userMissionModels.Any(p => p.CreateDatetime.Date == DateTime.Today))
                {
                    return 3;
                }
                // 否则就是 - 去做任务
                return 0;
            }
        }
    }


}
