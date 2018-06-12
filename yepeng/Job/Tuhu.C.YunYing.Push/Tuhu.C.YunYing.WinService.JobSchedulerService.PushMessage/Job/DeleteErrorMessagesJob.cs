using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Nest;
using Quartz;
using Tuhu.Service.Push.Models.MessageBox;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.BLL;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    public class DeleteErrorMessagesJob : IJob
    {
        private const string UserMessageBoxRelationIndex = "usermessageboxtelationindex";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DeleteErrorMessagesJob));
        private static IElasticClient ElasticSearchClient
        {
            get
            {
                var client = ElasticsearchHelper.CreateClient();
#if DEBUG
                ElasticsearchHelper.EnableDebug();
#endif
                return client;
            }
        }
        
        public void Execute(IJobExecutionContext context)
        {
            var isopen = PushBussiness.CheckIsOpenByNameFromCache("DeleteErrorMessages");
            if (!isopen)
            {
                Logger.Info("开关已关 return ");
                return;
            }
            try
            {
                Logger.Info($"job开始执行");
                string ids = "18,5556,22945,26744,44331,48149,66658,70361,86538,88913,97254,99181,110572,114171,127320,130543,145728,149038,164618,169081,183530,186619,198954,201443,212227,215253,226516,231134,245328,249989,250479";
#if DEBUG
                ids = "3,4,16,17";
#endif
                var messageids = ids.Split(',');
                var result =
                    ElasticSearchClient.Search<UserMessageBoxInfoRelationC>(s => s.Index(UserMessageBoxRelationIndex)
                        .Type<UserMessageBoxInfoRelationC>().Query(
                            qbm => qbm.Term(
                                       qbmt => qbmt.Field(x => x.SendType).Value(1)
                                   )
                                   && qbm.Terms(t => t.Field(tm => tm.MessageID).Terms(messageids))

                        ).Source(false).Take(1000));
                if (result.IsValid)
                {
                    var pkids = result.Hits.Select(x => x.Id);
                    Logger.Info($"收到{pkids.Count()} 个");
                    var operations = new List<IBulkOperation>();
                    foreach (var pkid in pkids)
                    {
                        operations.Add(new BulkDeleteOperation<UserMessageBoxInfoRelationC>(pkid));
                    }
                    var bulkRequest = new BulkRequest(UserMessageBoxRelationIndex)
                    {
                        Operations = operations.ToArray()
                    };
                    var responseOne = ElasticSearchClient.Bulk(bulkRequest);
                    var errorcount = responseOne.ItemsWithErrors.Count();
                    Logger.Info($"结束删除.errorcount:{errorcount}");
                }

            }
            catch (System.Exception ex)
            {
                Logger.Warn(ex);
            }
            Logger.Info($"job结束执行");
        }
        [ElasticsearchType(IdProperty = "UserIDMsgId", Name = "UserMessageBoxInfoRelation")]
        public class UserMessageBoxInfoRelationC : UserMessageBoxInfoRelation
        {
            public string UserIDMsgId => UserID.ToString("N") + "|" + MessageID;

            [String(Index = FieldIndexOption.NotAnalyzed)]
            public Guid UserIDForES => UserID;
        }

    }
}
