using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.SqlServer.Server;
using Nest;
using Quartz;
using Tuhu.C.Job.DAL;
using Tuhu.Service.Push.Models.MessageBox;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class DeleteExpiredMessageBoxJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<DeleteExpiredMessageBoxJob>();
        public const string UserMessageBoxRelationIndex = "umbrindex";
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
            try
            {
                string runtimename = "DeleteExpiredMessageBox";
                var allMessageIds = SelectDeleteMessageIds();
                if (allMessageIds != null)
                {
                    foreach (var messageInfos in allMessageIds)
                    {
                        var runtimeResult = DalMessageBox.CheckIsOpenWithDescription(runtimename);
                        if (!runtimeResult.Item1)
                        {
                            Logger.Info("开关已关,return");
                            return;
                        }
                        if (messageInfos != null && messageInfos.Item1 != null && messageInfos.Item1.Any())
                        {
                            var messageids = messageInfos.Item1;
                            if (messageInfos.Item2)
                            {
                                var deleteResult = DalMessageBox.DeleteBroadcastMessageInfo(messageInfos.Item1);
                                Logger.Info($"删除广播消息盒子数据,deleteResult:{deleteResult},messageids:{string.Join(",", messageInfos.Item1)}");
                            }
                            var relationResults = SelectUserMessageBoxInfoRelationByMessageIds(messageids);
                            if (relationResults != null)
                            {
                                foreach (var relations in relationResults)
                                {
                                    runtimeResult = DalMessageBox.CheckIsOpenWithDescription(runtimename);
                                    if (!runtimeResult.Item1)
                                    {
                                        Logger.Info("开关已关,return");
                                        return;
                                    }
                                    DeleteEsData(relations);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.Warn("删除消息盒子过期数据出错,ex:" + ex);
            }
        }
        private bool DeleteEsData(IEnumerable<UserMessageBoxInfoRelation> relations)
        {
            if (relations != null && relations.Any())
            {
                var operations = new List<IBulkOperation>();
                var exlogs = ConvertMessageLogToEsLog(relations);
                foreach (var log in exlogs)
                {
                    operations.Add(new BulkDeleteOperation<UserMessageBoxInfoRelationC>(log.Id)
                    {
                        Routing = log.UserId
                    });
                }
                var bulkRequest = new BulkRequest(UserMessageBoxRelationIndex)
                {
                    Operations = operations.ToArray()
                };
                var client = ElasticsearchHelper.CreateClient();
#if DEBUG
                ElasticsearchHelper.EnableDebug();
#endif
                var responseOne = client.Bulk(bulkRequest);
                var errorcount = responseOne.ItemsWithErrors.Count();
                DalMessageBox.WriteSyncLogs(relations);
                return errorcount <= 0;
            }
            return false;
        }
        private IEnumerable<IEnumerable<UserMessageBoxInfoRelation>> SelectUserMessageBoxInfoRelationByMessageIds(IEnumerable<int> messageids)
        {
            int pagesize = 1000;
            foreach (var mids in messageids.Split(10).Select(x => x.ToList()))
            {
                int maxpkid = 0;
                int totalCount = DalMessageBox.SelectMessageRelationCountByMessageId(mids);
                if (totalCount > 0)
                {
                    int totalPage = (totalCount / pagesize) + 1;
                    for (int i = 0; i <= totalPage; i++)
                    {
                        Logger.Info($"relation--开始获取第{i}页关系数据,共{totalPage}页");
                        var relations = DalMessageBox.SelectMessageRelationByMessageId(mids, maxpkid, pagesize);
                        if (relations != null && relations.Any())
                        {
                            maxpkid = relations.Max(x => x.PKID);
                            yield return relations;
                        }
                    }
                }
            }
        }

        IEnumerable<UserMessageBoxInfoRelationC> ConvertMessageLogToEsLog(
            IEnumerable<UserMessageBoxInfoRelation> logs)
        {
            var result = logs?.Select(x => new UserMessageBoxInfoRelationC()
            {
                SendType = x.SendType,
                CreateDateTime = x.CreateDateTime,
                IsDelete = x.IsDelete,
                IsRead = x.IsRead,
                MessageId = x.MessageID,
                MessageNavigationTypeId = x.MessageNavigationTypeId,
                PKID = x.PKID,
                MessageType = x.MessageType,
                UserId = Convert.ToBase64String(x.UserID.ToByteArray())
            });
            return result ?? new List<UserMessageBoxInfoRelationC>().AsEnumerable();
        }



        private static IEnumerable<Tuple<IEnumerable<int>, bool>> SelectDeleteMessageIds()
        {
            int pagesize = 1000;
            var allbroadcastids = DalMessageBox.SelectExpireBroadcastMarketingMessageIds();
            Logger.Info($"count--删除广播消息个数:{allbroadcastids.Count()}");
            yield return Tuple.Create(allbroadcastids, true);
            int totalcount = DalMessageBox.SelectExpireSingleMarketingMessageCount();
            Logger.Info($"count--删除单播消息个数:{totalcount}");
            if (totalcount > 0)
            {
                int maxpkid = 0;
                int totalPage = (totalcount / pagesize) + 1;
                for (int i = 0; i <= totalPage; i++)
                {
                    Logger.Info($"messageinfo--开始获取第{i}页消息,总页数:{totalPage}");
                    var singleResult = DalMessageBox.SelectExpireSingleMarketingMessageIds(maxpkid, pagesize);
                    if (singleResult != null && singleResult.Any())
                    {
                        maxpkid = singleResult.Max();
                        yield return Tuple.Create(singleResult?.AsEnumerable(), false);
                    }
                }
            }
        }

    }

}
