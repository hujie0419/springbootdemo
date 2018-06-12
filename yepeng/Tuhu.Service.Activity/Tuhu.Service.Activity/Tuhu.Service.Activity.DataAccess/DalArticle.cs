using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalArticle
    {
        /// <summary>
        /// 查询我关注的标签ID
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async static Task<IEnumerable<DiscoveryArticleAnswerUser>> SelectMyCategoryId(string UserId)
        {
            using(var db=DbHelper.CreateLogDbHelper(true))
            using (var cmd = new SqlCommand(@"SELECT  DISTINCT
                                                    AC.CategoryId
                                            FROM    Marketing.dbo.tbl_AttentionCategory AS AC WITH ( NOLOCK )
                                            WHERE   AC.IsAttention = 1
                                                    AND AC.UserId = @UserId;"))
            {
                cmd.Parameters.AddWithValue("@UserId", UserId);
                return await db.ExecuteSelectAsync<DiscoveryArticleAnswerUser>(cmd);
            }
        }
        /// <summary>
        /// 查询我关注的提问ID
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async static Task<IEnumerable<DiscoveryArticleAnswerUser>> SelectMyArticleID(string UserId)
        {
            using (var db = DbHelper.CreateLogDbHelper(true))
            using (var cmd = new SqlCommand(@"SELECT  MRA.PKID
                                            FROM    Marketing..tbl_MyRelatedArticle AS MRA WITH ( NOLOCK )
                                            WHERE   Type = 4
                                                    AND MRA.Vote = 1
                                                    AND MRA.UserId = @UserId; "))
            {
                cmd.Parameters.AddWithValue("@UserId", UserId);
                return await db.ExecuteSelectAsync<DiscoveryArticleAnswerUser>(cmd);
            }
        }

        /// <summary>
        /// 查询我关注的人ID
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async static Task<IEnumerable<DiscoveryArticleAnswerUser>> SelectMyAttentionUserId(string UserId)
        {
            using (var db = DbHelper.CreateLogDbHelper(true))
            using (var cmd = new SqlCommand(@"SELECT AttentionUserId,UserIdentity
                                                    FROM    Marketing.dbo.tbl_AttentionUser AS AU WITH ( NOLOCK )
                                                    WHERE   AU.UserId = @UserId
                                                            AND AU.IsAttention = 1; "))
            {
                cmd.Parameters.AddWithValue("@UserId", UserId);
                return await db.ExecuteSelectAsync<DiscoveryArticleAnswerUser>(cmd);
            }
        }
    }
}
