using System;
using Nest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.Server.Manager
{
    public static class ArticleManager
    {
        static ArticleManager()
        {
            //ElasticsearchHelper.EnableDebug();
        }
        public async static Task<PagedModel<HomePageTimeLineRequestModel>> SelectDiscoveryHomeAsync(string UserId, PagerModel page,int version)
        {
            //关注的类目ID
            List<int> CategoryIds = new List<int>();
            //关注的文章ID
            List<int> ArticleIDs = new List<int>();
            //关注的用户ID
            List<string> AttentionUserIds = new List<string>();
            //关注的用户ID 不包含技师和途虎员工
            List<string> AttentionUserIdsNoTuhu = new List<string>();

            CategoryIds.Add(0);
            ArticleIDs.Add(0);
            AttentionUserIdsNoTuhu.Add("jiadeshujuyongyuguolu");

            var cids =await DalArticle.SelectMyCategoryId(UserId);
            if (cids != null && cids.Any())
                CategoryIds = cids.Select(x => x.CategoryId).ToList();

            var atcid = await DalArticle.SelectMyArticleID(UserId);
            if (atcid != null && atcid.Any())
                ArticleIDs = atcid.Select(x => x.PKID).ToList();

            var atuid = await DalArticle.SelectMyAttentionUserId(UserId);
            if (atuid != null && atuid.Any())
            {
                AttentionUserIds = atuid.Select(x => x.AttentionUserId).ToList();
                if (atuid.Any(x => x.UserIdentity == null || x.UserIdentity == 0))
                {
                    AttentionUserIdsNoTuhu = atuid.Where(x => x.UserIdentity == null || x.UserIdentity == 0).Select(x => x.AttentionUserId).ToList();
                  }
            }

            AttentionUserIds.AddRange(new string[] { "{c69cf542-7b0c-8494-8831-c1a9a2fe6388}", "{f8479c14-3bb1-4711-9a38-2626359b8c28}", "{935b3462-9310-59ce-d742-0cbc1c276ebf}", "{81c9ed57-008c-b2ba-53c8-1cc35caf69c0}" });

            var types = new List<int> { 1, 2, 5 };
            if (version > 1)
            {
                types.Add(11);
            }
            var client = ElasticsearchHelper.CreateClient();

            var b = await client.SearchAsync<DiscoveryArticleModel>(x => x
                                                         .Index("discoveryarticle")
                                                         .Type("Discovery")
                                                         .Query(q => q.
                                                                 Bool(qb => qb.
                                                                      Should(
                                                                             qs => qs.
                                                                                         Bool(qsb => qsb.
                                                                                                 Must(qsbm => qsbm.
                                                                                                     Terms(qt => qt.
                                                                                                             Field(qf => qf.AttentionUserIds).
                                                                                                             Terms(AttentionUserIds)),
                                                                                                     qsbm => qsbm.
                                                                                                             Terms(qt => qt.
                                                                                                                     Field(qf => qf.TYPE).
                                                                                                                     Terms(types))
                                                                                                                 )
                                                                                             ),
                                                                             qs => qs.
                                                                                         Bool(qsb => qsb.
                                                                                                 Must(qsbm => qsbm.
                                                                                                     Terms(qt => qt.
                                                                                                             Field(qf => qf.AttentionUserIds).
                                                                                                             Terms(AttentionUserIdsNoTuhu)),
                                                                                                     qsbm => qsbm.
                                                                                                             Terms(qt => qt.
                                                                                                                     Field(qf => qf.TYPE).
                                                                                                                     Terms(3,4))
                                                                                                                 )
                                                                                             ),
                                                                             qs => qs.
                                                                                 Bool(qsb => qsb.
                                                                                         Must(
                                                                                             qsbm => qsbm.
                                                                                                     Terms(qt => qt.
                                                                                                         Field(qf => qf.PKID).
                                                                                                         Terms(ArticleIDs)),
                                                                                             qsbm => qsbm.
                                                                                                     Terms(qt => qt.
                                                                                                         Field(qf => qf.TYPE).
                                                                                                         Terms(6)),
                                                                                             qsbm => qsbm.Bool(qsbmb => qsbmb.
                                                                                                          Should(
                                                                                                              qsbmbs => qsbmbs.
                                                                                                                  Term(qt => qt.
                                                                                                                      Field(qf => qf.AuditStatus).
                                                                                                                          Value(2)),
                                                                                                              qsbmbs => qsbmbs.
                                                                                                                  Term(qt => qt.
                                                                                                                      Field(qf => qf.AttentionUserIds).
                                                                                                                          Value(UserId))))
                                                                                                         )
                                                                                     ),
                                                                             qs => qs.
                                                                                 Bool(qsb => qsb.
                                                                                         Must(
                                                                                             qsbm => qsbm.
                                                                                                     Terms(qt => qt.
                                                                                                         Field(qf => qf.CategoryId).
                                                                                                         Terms(CategoryIds)),
                                                                                             qsbm => qsbm.
                                                                                                     Terms(qt => qt.
                                                                                                         Field(qf => qf.TYPE).
                                                                                                         Terms(7))
                                                                                             )
                                                                                     ),
                                                                             qs => qs.
                                                                                 Bool(qsb => qsb.
                                                                                         Must(
                                                                                             qsbm => qsbm.
                                                                                                     Terms(qt => qt.
                                                                                                         Field(qf => qf.CategoryId).
                                                                                                         Terms(CategoryIds)),
                                                                                             qsbm => qsbm.
                                                                                                     Terms(qt => qt.
                                                                                                         Field(qf => qf.TYPE).
                                                                                                         Terms(8)),
                                                                                             qsbm => qsbm.Bool(qsbmb => qsbmb.
                                                                                                          Should(
                                                                                                              qsbmbs => qsbmbs.
                                                                                                                  Term(qt => qt.
                                                                                                                      Field(qf => qf.AuditStatus).
                                                                                                                          Value(2)),
                                                                                                              qsbmbs => qsbmbs.
                                                                                                                  Term(qt => qt.
                                                                                                                      Field(qf => qf.AttentionUserIds).
                                                                                                                          Value(UserId)))
                                                                                                         )
                                                                                             )
                                                                                     )
                                                                         )
                                                                     )
                                                                 )
                                                         .Sort(s => s.
                                                                 Descending(ss=>ss.OperateTime))
                                                         .Skip((page.CurrentPage - 1) * page.PageSize)
                                                         .Take(page.PageSize));
            page.Total = (int)b.Total;
            var a = b.Hits.Select(x => x.Source).Select(x =>
                new HomePageTimeLineRequestModel()
                {
                    PKID = x.PKID,
                    Type = x.TYPE,
                    DistinctId = x.DISTINCTID,
                    AnswerId = x.AnswerId,
                    AnswerContent = x.AnswerContent,
                    AttentionCount = x.AttentionCount,
                    BigTitle = x.BigTitle,
                    Brief = x.Brief,
                    VoteNum = x.VoteNUm,
                    CategoryId = x.CategoryId,
                    AttentionUserIds = x.AttentionUserIds,
                    CategoryImage = x.CategoryImage,
                    CategoryName = x.CategoryName,
                    CategoryTags = x.CategoryTags,
                    ClickCount = x.ClickCount,
                    CommentImage = x.CommentImage,
                    CommentTimes = x.CommentTimes,
                    Content = x.Content,
                    ContentUrl = x.ContentUrl,
                    CreatorInfo = x.CreatorInfo,
                    OperateTime = x.OperateTime,
                    Praise = x.Praise,
                    RelatedArticleId = x.RelatedArticleId,
                    ShowImages = x.ShowImages,
                    ShowType = x.ShowType,
                    SmallImage = x.SmallImage,
                    SmallTitle = x.SmallTitle,
                    ImagesCount=x.ImagesCount
                }).ToList();
            var groupTypeTimeLine = a.GroupBy(t => new { t.PKID, t.DistinctId, t.Type }).ToList();

            var tempTimeLineList = new List<HomePageTimeLineRequestModel>();
            //分组拼接所有关注的人
            foreach (var timeLine in groupTypeTimeLine)
            {
                var tempTimeLine = timeLine.OrderByDescending(t => t.OperateTime).FirstOrDefault();
                tempTimeLine.AttentionCount = timeLine.Select(t => t.AttentionUserIds).Distinct().Count();
                tempTimeLine.AttentionUserIds = string.Join("、", timeLine.Select(t => t.AttentionUserIds).Distinct().ToArray<string>());
                tempTimeLineList.Add(tempTimeLine);
            }
            var groupFilterTimeLine = tempTimeLineList.GroupBy(t => new { t.PKID, t.AnswerId }).ToList();

            tempTimeLineList.Clear();
            //分组去掉重复的数据
            foreach (var timeLine in groupFilterTimeLine)
            {
                var tempTimeLine = timeLine.OrderByDescending(t => t.OperateTime).FirstOrDefault();

                tempTimeLineList.Add(tempTimeLine);
            }

            tempTimeLineList = tempTimeLineList.Select(t => t).OrderByDescending(t => t.OperateTime).ToList();

            return new PagedModel<HomePageTimeLineRequestModel>(page, tempTimeLineList); ;
        }
    }
}
