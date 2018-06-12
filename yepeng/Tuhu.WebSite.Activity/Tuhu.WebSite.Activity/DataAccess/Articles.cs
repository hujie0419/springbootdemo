using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.WebSite.Component.SystemFramework.Models;
using Tuhu.WebSite.Web.Activity.Models;
using System.Threading.Tasks;

namespace Tuhu.WebSite.Web.Activity.DataAccess
{
    internal static class Articles
    {
        /// <summary>
        /// 获取图文推送
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public static DataTable SelectArticlesForApi(PagerModel pager, string Category)
        {
            using (var cmd = new SqlCommand("Marketing..Article_SelectArticlePage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                #region AddParameters
                cmd.Parameters.AddWithValue("@PageNumber", pager.CurrentPage);
                cmd.Parameters.AddWithValue("@PageSize", pager.PageSize);
                cmd.Parameters.AddWithValue("@Category", Category);
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.Int
                });
                #endregion

                DataTable dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                pager.TotalItem = Convert.ToInt32(cmd.Parameters["@TotalCount"].Value);
                return dt;
            }
        }


        /// <summary>
        /// 图文推送=>查询所有的类别
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectAllCategory()
        {
            using (var cmd = new SqlCommand(@"SELECT * FROM [Marketing]..tbl_CategoryList WITH(NOLOCK)"))
            {
                DataTable dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                return dt;
            }
        }

        /// <summary>
        /// 查询文章用户评论总数
        /// </summary>
        /// <param name="pkId"></param>
        /// AuditStatus  0、待审核  1、审核不通过  2、审核通过
        /// <returns></returns>
        public static string SelectCommentCount(string pkId)
        {
            using (var cmd = new SqlCommand(@"SELECT COUNT(1) FROM Marketing..tbl_Comment WITH (NOLOCK) AS C WHERE C.PKID=@PKID AND AuditStatus=2"))
            {
                cmd.CommandType = CommandType.Text;

                #region AddParameters
                cmd.Parameters.AddWithValue("@PKID", pkId);
                #endregion
                return DbHelper.ExecuteScalar(true, cmd).ToString();
            }
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        public static int AddComment(ArticleCommentModel ac)
        {
            using (var cmd = new SqlCommand(@"INSERT	INTO Marketing..tbl_Comment
		(PKID,
		 Category,
		 Title,
		 PhoneNum,
		 UserID,
		 UserHead,
		 CommentContent,
		 CommentTime,
		 AuditStatus,
		 AuditTime,
		 UserName,
		 UserGrade,
		 RealName,
		 ParentID,
		 Sex,
		 Type
		)
VALUES	(@PKID,
		 @Category,
		 @Title,
		 @PhoneNum,
		 @UserID,
		 @UserHead,
		 @CommentContent,
		 @CommentTime,
		 0,
		 @AuditTime,
		 @UserName,
		 @UserGrade,
		 @RealName,
		 @ParentID,
		 @Sex,
		 0
		);"))
            {
                cmd.CommandType = CommandType.Text;

                #region AddParameters
                cmd.Parameters.AddWithValue("@PKID", ac.PKID);
                cmd.Parameters.AddWithValue("@Category", ac.Category);
                cmd.Parameters.AddWithValue("@Title", ac.Title);
                cmd.Parameters.AddWithValue("@PhoneNum", ac.PhoneNum);
                cmd.Parameters.AddWithValue("@UserID", ac.UserID);
                cmd.Parameters.AddWithValue("@UserHead", ac.UserHead);
                cmd.Parameters.AddWithValue("@CommentContent", ac.CommentContent);
                cmd.Parameters.AddWithValue("@CommentTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@AuditStatus", ac.AuditStatus);
                cmd.Parameters.AddWithValue("@AuditTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@UserName", ac.UserName);
                cmd.Parameters.AddWithValue("@UserGrade", ac.UserGrade);
                cmd.Parameters.AddWithValue("@RealName", ac.RealName);
                cmd.Parameters.AddWithValue("@ParentID", ac.ParentID);
                cmd.Parameters.AddWithValue("@Sex", ac.Sex);
                #endregion

                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 每次操作完修改热度
        /// </summary>
        /// <param name="pkId"></param>
        /// <param name="Num"></param>
        /// <returns></returns>
        public static int AddHeat(int pkId, int Num)
        {
            using (var cmd = new SqlCommand(@"UPDATE Marketing.[dbo].[tbl_Article] WITH(ROWLOCK) SET Heat=Heat+@Num WHERE pkid =@pkId"))
            {
                cmd.CommandType = CommandType.Text;

                #region AddParameters
                cmd.Parameters.AddWithValue("@Num", Num);
                cmd.Parameters.AddWithValue("@pkId", pkId);
                #endregion

                return DbHelper.ExecuteNonQuery(cmd);
            }
        }



        public static DataTable SelectArticlesByWord(string keyWord)
        {
            using (var cmd = new SqlCommand(@"SELECT TOP 20 * FROM Marketing..tbl_Article WITH (NOLOCK) WHERE BigTitle LIKE N'%'+@keyWord+N'%' OR Content LIKE N'%'+@keyWord+N'%' order by PublishDateTime desc"))
            {
                cmd.CommandType = CommandType.Text;

                #region AddParameters
                cmd.Parameters.AddWithValue("@keyWord", keyWord);
                #endregion

                DataTable dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                return dt;
            }
        }

        #region  点赞，并返回目前本片文章总的点赞数
        public static Hashtable UpdateVoteAndReturnCount(int pkid, bool dec = false)
        {
            var hastable = new Hashtable();
            int resultState = 0;//操作失败
            int AllVoteCount = 0; //总的点赞数
            int updateResult = 0;//update是否成功
            string sqli = @"UPDATE Marketing..tbl_Article with (rowlock) SET Vote= ISNULL(Vote, 0) + 1 WHERE PKID=@PKID";
            string sqld = @"UPDATE Marketing..tbl_Article with (rowlock) SET Vote= ISNULL(Vote, 1) - 1 WHERE PKID=@PKID and Vote>0";
            using (var cmd = new SqlCommand(dec ? sqld : sqli))
            {
                cmd.CommandType = CommandType.Text;

                #region AddParameters
                cmd.Parameters.AddWithValue("@PKID", pkid);
                #endregion

                updateResult = DbHelper.ExecuteNonQuery(cmd);
            }
            if (updateResult > 0)//修改失败
            {
                resultState = 1;//操作成功
                AllVoteCount = Articles.AllVoteCount(pkid);
            }
            hastable["Code"] = resultState;
            hastable["AllVoteCount"] = AllVoteCount;
            return hastable;
        }
        #endregion

        public static int AllVoteCount(int pkid)
        {
            var AllVoteCount = 0;
            using (var cmd = new SqlCommand(@"SELECT Vote FROM Marketing..tbl_Article with (nolock)  WHERE PKID=@PKID"))
            {
                cmd.CommandType = CommandType.Text;

                #region AddParameters
                cmd.Parameters.AddWithValue("@PKID", pkid);
                #endregion
                AllVoteCount = Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
            return AllVoteCount;
        }

        public static int UpdateArticleClick(int PKID)
        {
            using (var cmd = new SqlCommand("UPDATE [Marketing].[dbo].[tbl_Article] WITH(ROWLOCK) SET [ClickCount] = ISNULL([ClickCount], 0) + 1 WHERE PKID=@PKID"))
            {
                cmd.CommandType = CommandType.Text;

                #region AddParameters
                cmd.Parameters.AddWithValue("@PKID", PKID);
                #endregion

                return DbHelper.ExecuteNonQuery(cmd);
            }
        }



        #region 新版图文推送
        /// <summary>
        /// 查询所有文章
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="Category"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable SelectNewArticlesForApi(int pIndex, int pSize, string Category, string userId, string version, out int totalCount)
        {
            using (var cmd = new SqlCommand("Marketing..Article_NewSelectArticle"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                #region AddParameters
                cmd.Parameters.AddWithValue("@PageNumber", pIndex);
                cmd.Parameters.AddWithValue("@PageSize", pSize);
                cmd.Parameters.AddWithValue("@Category", Category);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@version", version);
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.Int
                });
                #endregion

                DataTable dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                totalCount = Convert.ToInt32(cmd.Parameters["@TotalCount"].Value);
                return dt;
            }
        }
        public static DataTable SelectAllArticle()
        {
            using (var cmd = new SqlCommand(@"SELECT	B.[PKID],
		                                                B.[Catalog],
		                                                B.[Image],
		                                                B.[SmallImage],
		                                                B.[SmallTitle],
		                                                B.[BigTitle],
		                                                B.[Brief],
		                                                B.[ContentUrl],
		                                                B.[Source],
		                                                B.[CreateDateTime],
		                                                B.[ClickCount],
		                                                B.Vote,
		                                                B.RedirectUrl,
		                                                B.ShareWX,
		                                                B.SharePYQ
                                                FROM	[Marketing].[dbo].[tbl_Article] B WITH (NOLOCK)"))
            {
                cmd.CommandType = CommandType.Text;
                return Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);

            }
        }

        /// <summary>
        /// 根据关键字查询图文推送文章
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>

        public static DataTable SelectNewArticlesByWord(PagerModel pager, string keyWord, string version)
        {
            using (var cmd = new SqlCommand("Marketing..SelectArticlesByWord"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                #region AddParameters
                cmd.Parameters.AddWithValue("@PageNumber", pager.CurrentPage);
                cmd.Parameters.AddWithValue("@PageSize", pager.PageSize);
                cmd.Parameters.AddWithValue("@KeyWord", keyWord);
                cmd.Parameters.AddWithValue("@version", version);
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.Int
                });
                #endregion

                DataTable dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                pager.TotalItem = Convert.ToInt32(cmd.Parameters["@TotalCount"].Value);
                return dt;
            }
        }

        /// <summary>
        /// 将图文推送搜索关键词-->添加
        /// </summary>
        /// <param name="KeyWord"></param>
        /// <param name="Versions"></param>
        /// <param name="Channel"></param>
        /// <returns></returns>
        public static int AddSeekKeyWord(string KeyWord, string Versions, string Channel)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO Marketing.[dbo].[tbl_SeekKeyWord](KeyWord, Versions, Channel, CreateTime) VALUES(@KeyWord,@Versions,@Channel,@CreateTime)"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@KeyWord", KeyWord);
                cmd.Parameters.AddWithValue("@Versions", Versions);
                cmd.Parameters.AddWithValue("@Channel", Channel);
                cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                int Result = Convert.ToInt32(DbHelper.ExecuteNonQuery(cmd));
                return Result;
            }
        }

        /// <summary>
        /// 查询特推主题
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectTopics()
        {
            using (var cmd = new SqlCommand(@"Marketing..Article_SelectTopics"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                return Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
            }
        }

        /// <summary>
        /// 查询相关文章，已经置顶评论
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="Category"></param>
        /// <param name="UserId"></param>
        /// <param name="IsLikeNew"></param>
        /// <returns></returns>
        public static DataSet SelectNewsInfo(int PKID, int Category, string UserId = "", bool IsLikeNew = false)
        {
            using (var cmd = new SqlCommand("Marketing..SelectNewsInfo"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PKID", PKID);
                cmd.Parameters.AddWithValue("@Category", Category);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@IsLikeNew", IsLikeNew);
                return Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataSet(true, cmd);
            }
        }

        /// <summary>
        /// 查询当前用户是否收藏过指定文章
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="Category"></param>
        /// <param name="UserId"></param>
        /// <param name="IsLikeNew"></param>
        /// <returns></returns>
        public static bool IsNewsForLikeNews(int PKID, string UserId, int Category = 0, bool IsLikeNew = true)
        {
            using (var cmd = new SqlCommand("Marketing..SelectNewsInfo"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PKID", PKID);
                cmd.Parameters.AddWithValue("@Category", Category);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@IsLikeNew", IsLikeNew);
                return (bool)DbHelper.ExecuteScalar(cmd);
            }
        }

        /// <summary>
        /// 查询出搜索热门的前12个关键词
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectHotKeyWord()
        {
            using (var cmd = new SqlCommand(@"SELECT TOP 12
                            *
                    FROM    Marketing.[dbo].[tbl_HotWord] WITH (NOLOCK)
                    WHERE   [OnOff] = 1
                    ORDER BY [CreateTime] DESC;"))
            {
                DataTable dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                return dt;
            }
        }

        /// <summary>
        /// 查询文章URL链接地址
        /// </summary>
        /// <returns></returns>
        public static string SelectNewsUrl(int pkid)
        {
            using (var cmd = new SqlCommand(@" SELECT ContentUrl FROM Marketing..tbl_Article WITH(NOLOCK) WHERE Type IN (0,1) AND PKID = @PKID "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", pkid);
                string result = (string)Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteScalar(true, cmd) ?? "";
                return result;
            }
        }

        /// <summary>
        /// 评论后或点赞后添加文章ID和对应UserID
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PKID"></param>
        /// <param name="Type">1、我喜欢的  2、我评论的 </param>
        /// <returns></returns>
        public static int AddUserReviewOfArticles(string UserId, int PKID, int Type, string Vote, out bool Status)
        {
            using (var cmd = new SqlCommand("Marketing..Article_TakeVoteState"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@Vote", Vote);
                cmd.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Status", SqlDbType.Bit).Direction = ParameterDirection.Output;
                DbHelper.ExecuteScalar(cmd);
                Status = Convert.ToBoolean(cmd.Parameters["@Status"].Value);
                var result = Convert.ToInt32(cmd.Parameters["@Result"].Value);

                //cmd.Parameters.Clear();
                //cmd.CommandText = "UPDATE Marketing.dbo.tbl_Article  SET VOTE=VOTE=1 WHERE PKID=@PKID";
                //cmd.CommandType = CommandType.Text;
                //cmd.Parameters.AddWithValue("@PKID", PKID);
                //DbHelper.ExecuteNonQuery(cmd);
                return result;
            }
        }

        /// <summary>
        /// 查询评论消息通知
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable SelectCommentNotice(string userId, bool isIncludeAll = false, PagerModel page = null)
        {
            if (isIncludeAll)
            {
                using (var cmd = new SqlCommand("[Marketing].dbo.[Discovery_SelectMyAnswerAndReply]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@PageIndex", page.CurrentPage);
                    cmd.Parameters.AddWithValue("@PageSize", page.PageSize);
                    cmd.Parameters.AddWithValue("@Total", DbType.Int32).Direction = ParameterDirection.Output;

                    var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                    page.TotalItem = Convert.ToInt32(cmd.Parameters["@Total"].Value);
                    return dt;
                }
            }

            else
            {
                using (var cmd = new SqlCommand(@"SELECT TC1.PKID,(SELECT A.BigTitle FROM Marketing..tbl_Article AS A WITH(NOLOCK) WHERE A.PKID=tc1.PKID) AS Title,tc1.CommentContent AS MyCommentContent,tc.PhoneNum,tc.UserHead,tc.CommentTime,tc.CommentContent,tc.UserName,tc.UserGrade,tc.RealName,tc.Sex FROM Marketing..[tbl_Comment] AS tc WITH(NOLOCK)		 INNER JOIN Marketing..[tbl_Comment] AS tc1 WITH(NOLOCK) ON tc.ParentID=tc1.ID
		 WHERE tc1.UserID=@userId AND tc.AuditStatus=2 AND tc.Type=0 AND tc1.Type=0 ORDER BY tc.CommentTime DESC"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                }
            }

        }


        /// <summary>
        /// 发送已读消息通知
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public static async Task<int> ReadCommentNotic(int commentId)
        {
            using (var command = new SqlCommand("UPDATE Marketing..tbl_Comment WITH (ROWLOCK) SET ISREAD=1,COMMENTTIME=GETDATE() WHERE ID=@Id"))
            {
                command.Parameters.AddWithValue("@Id", commentId);
                var result = await DbHelper.ExecuteNonQueryAsync(command);
                return result;
            }
        }

        /// <summary>
        /// 发送已读点赞消息通知
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<int> ReadPraiseNotice(int id, int praiseType)
        {
            using (var command = new SqlCommand())
            {
                int result = 0;
                //文章点赞
                if (praiseType == 1)
                {
                    result = await ReleatedArticlePraiseNotice(id, command, result);

                    if (result == 0)
                        await ComentPraiseNotice(id, command, result);
                }
                //评论点赞
                else if (praiseType == 2)
                {
                    result = await ComentPraiseNotice(id, command, result);
                    if (result == 0)
                        result = await ReleatedArticlePraiseNotice(id, command, result);
                }
                return result;
            }
        }

        private static async Task<int> ComentPraiseNotice(int id, SqlCommand command, int result)
        {
            command.CommandText = " UPDATE Marketing..CommentPraise WITH (ROWLOCK) SET ISREAD=1 WHERE ID=@ID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", id);
            result = await DbHelper.ExecuteNonQueryAsync(command);
            return result;
        }

        private static async Task<int> ReleatedArticlePraiseNotice(int id, SqlCommand command, int result)
        {
            command.CommandText = " UPDATE Marketing..tbl_MyRelatedArticle WITH (ROWLOCK)  SET ISREAD=1 WHERE ID=@ID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@ID", id);
            result = await DbHelper.ExecuteNonQueryAsync(command);
            return result;
        }




        /// <summary>
        /// 查询评论消息通知
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable SelectVoteNotice(string userId)
        {
            using (var cmd = new SqlCommand(@"SELECT  C2.UserName, C2.RealName, C2.Sex,C2.PhoneNum,CP.CommentId,tc.CommentContent,CP.CreateTime,tc.CommentTime,CP.UserHead FROM Marketing..tbl_Comment AS tc WITH(NOLOCK) 
		 INNER JOIN Marketing..CommentPraise AS CP WITH(NOLOCK) ON tc.ID=CP.CommentId
		 left JOIN Marketing..tbl_Comment AS C2 WITH(NOLOCK) ON tc.ParentID=C2.ID
		 WHERE tc.UserID=@userId  AND CP.VoteState=1 ORDER BY CP.CreateTime DESC"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@userId", userId);
                return Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
            }
        }


        /// <summary>
        /// 查询我的点赞与感谢信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable SelectVoteNotices(string userId, PagerModel page)
        {
            using (var cmd = new SqlCommand(@"Marketing..[Discovery_SelectMyVoteNotices]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@PageIndex", page.CurrentPage);
                cmd.Parameters.AddWithValue("@PageSize", page.PageSize);
                cmd.Parameters.AddWithValue("@Total", DbType.Int32).Direction = ParameterDirection.Output;

                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                page.TotalItem = Convert.ToInt32(cmd.Parameters["@Total"].Value);
                return dt;
            }
        }



        /// <summary>
        /// 新版图文推送=>查询所有的类别
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectAllNewCategory()
        {
            using (var cmd = new SqlCommand(@"SELECT * FROM [Marketing]..tbl_NewCategoryList WITH(NOLOCK)"))
            {
                DataTable dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                return dt;
            }
        }

        /// <summary>
        /// 查询点赞状态
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static bool SelectVoteState(string UserId, int PKID)
        {
            using (var cmd = new SqlCommand("Marketing..Article_SelectVoteState"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                cmd.Parameters.Add("@Vote", SqlDbType.Bit).Direction = ParameterDirection.Output;
                DbHelper.ExecuteScalar(true, cmd);
                return Convert.ToBoolean(cmd.Parameters["@Vote"].Value);
            }
        }

        /// <summary>
        /// 查点赞数
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static string SelectVoteCount(int PKID)
        {
            using (var cmd = new SqlCommand(@"SELECT A.Vote FROM Marketing.dbo.tbl_Article AS A WITH ( NOLOCK ) WHERE A.PKID=@PKID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return Convert.ToString(DbHelper.ExecuteScalar(true, cmd));
            }
        }

        /// <summary>
        /// 查询点击数
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static int SelectClickCount(int PKID)
        {
            using (var cmd = new SqlCommand(@"SELECT ISNULL(A.ClickCount,0) AS ClickCount FROM Marketing..tbl_Article AS A WITH(NOLOCK) WHERE A.PKID=@PKID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
        }

        /// <summary>
        /// 阅读文章后插入记录
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Type"></param>
        /// <param name="Vote"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static int AddReadingRecord(string UserId, int Type, string Vote, int PKID)
        {
            using (var cmd = new SqlCommand(@"INSERT  INTO Marketing..HD_BrowseHistory(UserId,PKID, Type, CreateTime)
                    VALUES(@UserId,@PKID,@Type,@OperateTime)"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                cmd.Parameters.AddWithValue("@Type", Type);
                //cmd.Parameters.AddWithValue("@Vote", Vote);
                cmd.Parameters.AddWithValue("@OperateTime", DateTime.Now);
                int Result = Convert.ToInt32(DbHelper.ExecuteNonQuery(cmd));
                return Result;
            }
        }

        /// <summary>
        /// 插入浏览记录到 HD_BrowseHistory
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Type"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static int AddReadingRecord2(string UserId, int PKID, int Type = 3)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO Marketing..HD_BrowseHistory(PKID,UserId,Type,CreateTime) 
                                              VALUES(@PKID,@UserId,@Type,@CreateTime)"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", PKID);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                int Result = Convert.ToInt32(DbHelper.ExecuteNonQuery(cmd));
                return Result;
            }
        }

        /// <summary>
        /// 首页汽车头条
        /// </summary>
        /// <param name="isIncludeQuestion">是否包含全局问题</param>
        /// <returns></returns>
        public static async Task<DataTable> SelectCarMadeHeadlines()
        {
            var cmd = new SqlCommand(@"SELECT TOP 5
    	                                      B.[PKID],
                                              B.[Catalog],
                                              B.[Image],
                                              B.[SmallImage],
                                              B.[SmallTitle],
                                              B.[BigTitle],
                                              B.[TitleColor],
                                              B.[Brief],
                                              B.[ContentUrl],
                                              B.[Source],
                                              B.[PublishDateTime],
                                              B.[CreateDateTime],
                                              B.[LastUpdateDateTime],
                                              B.[ClickCount],
                                              B.Vote,
                                              B.Heat,
                                              B.Type,
                                              B.RedirectUrl
                                            FROM [Marketing].[dbo].[tbl_Article] B WITH ( NOLOCK )
                                            WHERE IsShow = 1
                                                  AND PublishDateTime <= GETDATE()
                                                  AND IsShowTouTiao = 1
                                            ORDER BY PublishDateTime DESC;");
            return await Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTableAsync(true, cmd);
        }

        public static DataTable SelectCarMadeHeadlinesVersion1()
        {
            var cmd = new SqlCommand(@"SELECT TOP 5
		                                        B.[PKID],
                                                B.RelatedArticleId,
		                                        B.[Catalog],
		                                        B.[Image],
		                                        B.[SmallImage],
		                                        B.[SmallTitle],
		                                        B.[BigTitle],
		                                        B.[TitleColor],
		                                        B.[Brief],
		                                        B.[ContentUrl],
		                                        B.[Source],
		                                        B.[PublishDateTime],
		                                        B.[CreateDateTime],
		                                        B.[LastUpdateDateTime],
		                                        B.[ClickCount],
		                                        B.Vote,
		                                        B.Heat,
		                                        B.RedirectUrl,
		                                        C.CategoryName,
		                                        (SELECT	COUNT(1)
		                                         FROM	[Marketing].dbo.tbl_Comment WITH (NOLOCK)
		                                         WHERE	PKID = B.PKID
				                                        AND AuditStatus = 2
		                                        ) AS CommentNum
                                        FROM	(SELECT	[PKID],
				                                        ROW_NUMBER() OVER (ORDER BY PublishDateTime DESC) AS [RowNumber]
		                                         FROM	[Marketing].[dbo].[tbl_Article] WITH (NOLOCK)
		                                         WHERE	IsShow = 1
				                                        AND PublishDateTime <= GETDATE()
				                                        AND Type <> 2 AND TYPE<>3
		                                        ) A
                                        LEFT JOIN [Marketing].[dbo].[tbl_Article] B WITH (NOLOCK)
		                                        ON A.PKID = B.PKID
		                                           AND B.Type <> 2 AND B.TYPE<>3
                                        LEFT JOIN [Marketing].[dbo].[tbl_NewCategoryList] C WITH (NOLOCK)
		                                        ON B.Category = C.id
                                        ORDER BY PublishDateTime DESC;");
            return Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
        }
        /// <summary>
        /// 发现文章小红点
        /// </summary>
        /// <returns></returns>
        public static string SelectArticleIsNew()
        {
            var cmd = new SqlCommand(@"SELECT TOP 1
                    A.[PKID]
            FROM    Marketing..tbl_Article AS A
            WHERE   A.IsShow = 1 AND (A.TYPE=0 OR A.TYPE=1)
                    AND A.PublishDateTime <= GETDATE()
            ORDER BY A.PublishDateTime DESC;");
            return DbHelper.ExecuteScalar(true, cmd).ToString();
        }

        /// <summary>
        /// 查询文章的详情以及相关文章
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="UserId"></param>
        /// <param name="IsLikeNew"></param>
        /// <returns></returns>
        public static DataSet SelectArticleAndRelated(int PKID, string UserId = "", bool IsLikeNew = false)
        {
            using (var cmd = new SqlCommand("Marketing..SelectArticleDetailAndRelated"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PKID", PKID);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@IsLikeNew", IsLikeNew);
                return Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataSet(true, cmd);
            }
        }
        #endregion
        /// <summary>
        /// 查询所有作者信息
        /// </summary>
        /// <returns></returns>
        public static async Task<List<CoverAuthorModel>> SelectCoverAuthor()
        {
            using (var cmd = new SqlCommand("SELECT AuthorName,AuthorHead,Description FROM Marketing..tbl_CoverAuthor WITH(NOLOCK)"))
            {
                return (await DbHelper.ExecuteSelectAsync<CoverAuthorModel>(true, cmd))?.ToList();
            }
        }
        /// <summary>
        /// 查询文章相关阅读
        /// </summary>
        /// <param name="categoryTagId"></param>
        /// <returns></returns>
        public static async Task<List<RelatedArticleModel>> SelectRelateArticleByCategoryTagId(int categoryTagId)
        {
            using (var cmd = new SqlCommand(@" SELECT TOP 6 A.PKID , A.SmallTitle , A.CoverTag , A.CoverImage , A.PublishDateTime , A.Type , A.ClickCount , A.Image FROM   Marketing..HD_ArticleCategoryTag AS Tag ( NOLOCK ) JOIN Marketing..tbl_Article AS A ( NOLOCK ) ON Tag.ArticleId = A.PKID WHERE  Tag.CategoryTagId = @CategoryTagId AND A.Type IN ( 0, 1, 5 ) AND (   (   A.Type = 5 AND A.Status = 'Published' ) OR A.IsShow = 1 ) AND A.PublishDateTime <= GETDATE() ORDER BY A.PublishDateTime DESC;"))
            {
                cmd.Parameters.AddWithValue("@CategoryTagId", categoryTagId);
                return (await DbHelper.ExecuteSelectAsync<RelatedArticleModel>(true, cmd))?.ToList();
            }
        }
        public static async Task<List<string>> SelectArticleLikeUserListByArticleId(int articleId)
        {
            using (var cmd = new SqlCommand("SELECT userId FROM Marketing..tbl_MyRelatedArticle (NOLOCK) WHERE PKID=@PKID AND Type=0 AND Vote=1"))
            {
                cmd.Parameters.AddWithValue("@PKID", articleId);
                return (await DbHelper.ExecuteSelectAsync<ArticleVoteModel>(true, cmd))?.Select(x => x.UserId)?.ToList();
            }
        }
        /// <summary>
        /// 根据用户和优选ID 查询对优选文章的点赞和收藏状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static async Task<YouXuanRelatedOperactionModel> SelectYouXuanRelatedOperaction(Guid userId, int articleId)
        {
            using (var cmd = new SqlCommand(@"SELECT TOP 1 PKID,UserId,ArticleId,Vote,Favorite FROM Marketing..YouXuanRelatedOperaction(NOLOCK) WHERE UserId=@UserId AND ArticleId=@ArticleId ORDER BY PKID DESC "))
            {
                cmd.Parameters.AddWithValue("@UserId", userId).SqlDbType = SqlDbType.UniqueIdentifier;
                cmd.Parameters.AddWithValue("@ArticleId", articleId);
                return await DbHelper.ExecuteFetchAsync<YouXuanRelatedOperactionModel>(true, cmd);
            }
        }
        /// <summary>
        /// 插入用户优选点赞和收藏状态
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static async Task<int> InserYouXuanRelatedOperaction(YouXuanRelatedOperactionModel info)
        {
            using (var cmd = new SqlCommand("INSERT INTO Marketing..YouXuanRelatedOperaction VALUES ( @UserId, @ArticleId, @Vote, @Favorite, GETDATE(), GETDATE());"))
            {
                cmd.Parameters.AddWithValue("@UserId", info.UserId).SqlDbType = SqlDbType.UniqueIdentifier;
                cmd.Parameters.AddWithValue("@ArticleId", info.ArticleId);
                cmd.Parameters.AddWithValue("@Vote", info.Vote);
                cmd.Parameters.AddWithValue("@Favorite", info.Favorite);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }
        /// <summary>
        /// 修改用户优选点赞和收藏状态
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static async Task<int> UpdateYouXuanRelatedOperactionStatus(YouXuanRelatedOperactionModel info)
        {
            using (var cmd = new SqlCommand("UPDATE Marketing..YouXuanRelatedOperaction WITH(ROWLOCK) SET Vote=@Vote,Favorite=@Favorite,LastUpdateDateTime=GETDATE() WHERE PKID=@PKID"))
            {
                cmd.Parameters.AddWithValue("@Vote", info.Vote);
                cmd.Parameters.AddWithValue("@Favorite", info.Favorite);
                cmd.Parameters.AddWithValue("@PKID", info.PKID);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }
        /// <summary>
        /// 查询用户收藏的优选文章ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<List<YouXuanRelatedOperactionModel>> SelectMyFavoriteYouXuanIdsByUserId(Guid userId)
        {
            using (var cmd = new SqlCommand("SELECT ArticleId FROM Marketing..YouXuanRelatedOperaction (NOLOCK) WHERE UserId=@UserId AND Favorite=1 ORDER BY PKID DESC "))
            {
                cmd.Parameters.AddWithValue("@UserId", userId).SqlDbType = SqlDbType.UniqueIdentifier;
                return (await DbHelper.ExecuteSelectAsync<YouXuanRelatedOperactionModel>(true, cmd))?.ToList();
            }
        }
        /// <summary>
        /// 根据优选文章ID 查询列表信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static async Task<List<YouXuantListModel>> SelectMyFavoriteYouXuanListByIds(List<int> ids)
        {
            using (var cmd = new SqlCommand(@"SELECT A.PKID , A.SmallTitle , A.BigTitle , A.Brief , A.Content , A.PublishDateTime , ISNULL(A.ClickCount, 0) AS ClickCount , ISNULL(A.Vote, 0) AS VoteCount , C.CoverType , C.CoverImg , C.CoverVideo FROM   Marketing..tbl_Article AS A ( NOLOCK ) JOIN Marketing..ArticleCoverConfig AS C ( NOLOCK ) ON A.PKID = C.ArticleId WHERE  A.Type = 9 AND IsDelete = 0 AND A.Status = 'Published' AND A.PublishDateTime <= GETDATE() AND A.PKID IN (   SELECT * FROM   Marketing..SplitString(@ids, ',', 1) );"))
            {
                cmd.Parameters.AddWithValue("@ids", string.Join(",", ids));
                return (await DbHelper.ExecuteSelectAsync<YouXuantListModel>(true, cmd))?.ToList();
            }
        }
        /// <summary>
        /// 查询优选文章喜欢数
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static async Task<int> SelectYouXuanFavoriteCountByArticleId(int articleId)
        {
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM   Marketing..YouXuanRelatedOperaction (NOLOCK) WHERE  Favorite = 1 AND ArticleId=@articleId; "))
            {
                cmd.Parameters.AddWithValue("@articleId", articleId);
                var data = await DbHelper.ExecuteScalarAsync(true,cmd);
                return data != null && int.TryParse(data.ToString(), out int count) ? count : 0;
            }
        }
        /// <summary>
        /// 根据优选标签查询文章列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="categoryTagId"></param>
        /// <returns></returns>
        public static async Task<List<YouXuantModel>> SelectYouXuanArticleByTagId(int pageIndex, int pageSize, string categoryTagId)
        {
            using (var cmd = new SqlCommand(@"SELECT   A.PKID , A.SmallTitle, A.BigTitle, A.Brief, A.Content, A.PublishDateTime, ISNULL(A.ClickCount, 0) AS ClickCount, ISNULL(A.Vote, 0) AS VoteCount, C.CoverType, C.CoverImg, C.CoverVideo,A.IsTopMost  FROM     Marketing..tbl_Article AS A(NOLOCK) JOIN Marketing..HD_ArticleCategoryTag AS Tag(NOLOCK) ON Tag.ArticleId = A.PKID JOIN Marketing..ArticleCoverConfig AS C(NOLOCK) ON A.PKID = C.ArticleId WHERE    A.Type = 9 AND IsDelete = 0 AND A.Status = 'Published' AND A.PublishDateTime <= GETDATE() AND Tag.CategoryTagId = @CategoryTagId AND Tag.Source = 'YouXuan' ORDER BY A.IsTopMost DESC, A.PublishDateTime DESC OFFSET(@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY; "))
            {
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.AddWithValue("@CategoryTagId", categoryTagId);
                return (await DbHelper.ExecuteSelectAsync<YouXuantModel>(true, cmd))?.ToList();
            }
        }
        public static async Task<YouXuantModel> FirstYouXuanDetailById(int id)
        {
            using (var cmd = new SqlCommand(@"SELECT A.PKID , A.SmallTitle , A.BigTitle , A.Brief , A.Content , A.PublishDateTime , ISNULL(A.ClickCount, 0) AS ClickCount , ISNULL(A.Vote, 0) AS VoteCount , C.CoverType , C.CoverImg , C.CoverVideo , c.OtherImg, A.IsTopMost FROM   Marketing..tbl_Article AS A ( NOLOCK ) JOIN Marketing..ArticleCoverConfig AS C ( NOLOCK ) ON A.PKID = C.ArticleId WHERE  A.PKID = @id AND A.Type = 9 AND A.Status = 'Published' AND IsDelete = 0;"))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return await DbHelper.ExecuteFetchAsync<YouXuantModel>(true, cmd);
            }
        }
        /// <summary>
        /// 新文章列表（包含优选文章）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ArticleListModel>> SelectArticleListAndYouXuanList(int pageIndex,int pageSize)
        {
            using(var cmd=new SqlCommand(@"SELECT   A.PKID ,
                                                    A.SmallTitle AS Title ,
                                                    A.Type ,
                                                    A.PublishDateTime ,
                                                    ISNULL(A.ClickCount, 0) AS ClickCount ,
                                                    ISNULL(C.CoverImg, A.SmallImage) AS ShowImage ,
                                                    A.CoverTag ,
                                                    A.IsTopMost
                                        FROM     Marketing..tbl_Article AS A ( NOLOCK )
                                                 LEFT JOIN Marketing..ArticleCoverConfig AS C ( NOLOCK ) ON A.PKID = C.ArticleId
                                        WHERE    A.Type IN ( 5, 9 )
                                                 AND IsDelete = 0
                                                 AND A.Status = 'Published'
                                                 AND A.PublishDateTime <= GETDATE()
                                        ORDER BY A.IsTopMost DESC ,
                                                    A.PublishDateTime DESC OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;"))
            {
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                return await DbHelper.ExecuteSelectAsync<ArticleListModel>(true,cmd);
            }
        }
    }
}
