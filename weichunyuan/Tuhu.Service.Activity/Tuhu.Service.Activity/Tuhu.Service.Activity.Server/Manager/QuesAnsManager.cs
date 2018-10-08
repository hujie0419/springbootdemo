using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Nosql;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.DataAccess;

namespace Tuhu.Service.Activity.Server.Manager
{
   public static class QuesAnsManager
    {
        /// <summary>
        /// 问答题库名称
        /// </summary>
        private static readonly string QuesAnsCacheName = "QuesAnsCacheName";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ZeroActivityManager));

        /// <summary>
        /// 获取问答题库信息
        /// </summary>
        /// <returns></returns>
        public async static Task<List<AnswerInfoListModel>> GetAnswerInfoList()
        {
            using (var client = Tuhu.Nosql.CacheHelper.CreateCacheClient(QuesAnsCacheName))
            {
                var result = await client.GetOrSetAsync(QuesAnsCacheName, async () => await DalQuesAns.GetAnswerInfoList(), TimeSpan.FromDays(30));
                return result?.Value;
            }
        }

        /// <summary>
        /// 插入试卷信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool InsertBigBrandQues(List<BigBrandQuesList> list)
        {
            return DalQuesAns.InsertBigBrandQues(list);
        }

        /// <summary>
        /// 获取试卷信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public async static Task<List<BigBrandQuesList>> GetBigBrandQuesList(Guid userId, string hashKey,bool isReady=true)
        {
            return await DalQuesAns.GetBigBrandQuesList(userId, hashKey, isReady);
        }

        /// <summary>
        /// 获取问题信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public async static Task<BigBrandQuesList> GetBigBrandQuesEntity(int pkid)
        {
            return await DalQuesAns.GetBigBrandQuesEntity(pkid);
        }

        /// <summary>
        /// 统计回答问题的得分
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public static async Task<int> GetGrade(Guid userId, string hashKey)
        {
            int number = 0;
            var list = await QuesAnsManager.GetBigBrandQuesList(userId, hashKey);
            list.ForEach(_ => {
                if (_.ResValue == _.OptionsReal)
                    number++;
            });
            return number;
        }

        /// <summary>
        /// 更新问答试题
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="realValue"></param>
        /// <returns></returns>
        public async static Task<bool> UpdateBigBrandQues(int pkid, string realValue)
        {
            if (pkid == 0)
                return false;
            if (string.IsNullOrWhiteSpace(realValue))
                return false;
            return await DalQuesAns.UpdateBigBrandQues(pkid, realValue);
        }

        /// <summary>
        /// 更新题库信息
        /// </summary>
        /// <returns></returns>
        public async static Task<bool> UpdateAnswerInfoList()
        {
            using (var client = Tuhu.Nosql.CacheHelper.CreateCacheClient(QuesAnsCacheName))
            {
                var result = await client.SetAsync(QuesAnsCacheName, await DalQuesAns.GetAnswerInfoList(), TimeSpan.FromDays(30));
                return result.Success;
            }
        }


    }
}
