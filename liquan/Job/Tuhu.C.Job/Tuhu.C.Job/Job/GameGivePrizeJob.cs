using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using Tuhu.C.Job.DAL;
using Tuhu.C.Job.Models;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;

namespace Tuhu.C.Job.Job
{
    public class GameGivePrizeJob : IJob
    {

        private static readonly ILog Logger = LogManager.GetLogger<GameGivePrizeJob>();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"GameGivePrizeJob开始执行");
            Stopwatch s = new Stopwatch();
            s.Start();

            GetRankListAndGivePrize();

            Logger.Info($"GameGivePrizeJob执行结束,耗时:{s.ElapsedMilliseconds}");
        }

        private void GetRankListAndGivePrize()
        {
            //1.获取排行榜前150
            var rankList = new List<GameRankInfoModel>();

            using (var client = new GameClient())
            {
                var rankRequest = new GetRankListBeforeDayRequest()
                {
                    GameVersion = GameVersionEnum.BUMBLEBEE,
                    DayTime = DateTime.Now
                };
                var rankListResult = client.GetRankListBeforeDay(rankRequest);
                if (!rankListResult.Success)
                {
                    rankListResult = client.GetRankListBeforeDay(rankRequest);

                    if (!rankListResult.Success)
                    {
                        Logger.Warn($"GameGivePrizeJob => GetRankListBeforeDay 获取排行榜信息失败");
                        return;
                    }
                }
                rankList = rankListResult.Result?.RankList;
            }

            if (rankList?.Count > 0)
            {
                Logger.Info($"GameGivePrizeJob =>  开奖前日排行榜数据:[{rankList.Count}],{JsonConvert.SerializeObject(rankList)}");
            }
            else
            {
                Logger.Warn($"GameGivePrizeJob => GetRankListBeforeDay 排行榜无信息");
                return;
            }

            // 判断今天是否执行过:有用户领奖就是执行过
            var prizeCount = DalGame.GetBumBelbeeLastDayPrizeCount();
            if (prizeCount > 0)
            {
                Logger.Warn($"GameGivePrizeJob => GetRankListAndGivePrize 今日发放过奖励,prizeCount:{prizeCount}");
                return;
            }

            try
            {
                //记录排行数据
                var insertRankResult = DalGame.InsertGameRankList(rankList);
                if (!insertRankResult)
                {
                    Logger.Warn($"GameGivePrizeJob => insertRankResult 记录排行榜数据失败");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GameGivePrizeJob => insertRankResult异常", ex);
            }

            //发放奖品
            GivePrize(rankList);

        }

        private void GivePrize(List<GameRankInfoModel> rankList)
        {
            //1.遍历排行榜 调用接口发放奖品
            using (var client = new GameClient())
            {
                foreach (var item in rankList)
                {
                    try
                    {
                        var lootRequest = new GameUserLootRequest()
                        {
                            UserId = item.UserID,
                            PrizeId = item.Rank,
                            GameVersion = GameVersionEnum.BUMBLEBEE,
                        };
                        var userLootResult = client.GameUserLoot(lootRequest);
                        if (!userLootResult.Success)
                        {
                            Logger.Warn($"GameGivePrizeJob => GameUserLoot失败,:{JsonConvert.SerializeObject(item)},ErrorMessage:{userLootResult.ErrorMessage} ");
                        }

                        //等待
                        Thread.Sleep(50);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"GameGivePrizeJob => GameUserLoot异常,:{JsonConvert.SerializeObject(item)}, ",ex);
                    }
                }
            }
        }
    }
}
