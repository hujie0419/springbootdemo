using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.WebSite.Web.Activity.Models;

namespace Tuhu.WebSite.Web.Activity.DataAccess
{
    public static class ArticleComment
    {

        public static DataTable SelectArticleCommentByPKID(int PKID, string UserID, int PageIndex = 1, int PageSize = 10)
        {
            using (var cmd = new SqlCommand("Marketing..[Marketing_SelectArticleCommentByPKID]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                #region AddParameters
                cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                #endregion

                DataTable dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                return dt;
            }
        }


        public static DataTable GetArticleCommentByPKID(int PKID, string UserID, out int TotalCount, int PageIndex = 1, int PageSize = 10)
        {
            using (var cmd = new SqlCommand("Marketing..[Marketing_SelectArticleCommentByPKID]"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                #region AddParameters
                cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.Int
                });
                #endregion

                DataTable dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                TotalCount = Convert.ToInt32(cmd.Parameters["@TotalCount"].Value);
                return dt;
            }
        }

        public static string GetUserNameByID(int id)
        {
            string sql = @"SELECT  isnull(UserName ,isnull(RealName,ISNULL(PhoneNum,NULL))) FROM Marketing.dbo.tbl_Comment with(nolock) WHERE ID=@id";

            var userName = DbHelper.ExecuteScalar(true, sql, CommandType.Text, new SqlParameter("@id", id));
            return userName == null ? "途虎用户" : userName.ToString();
        }

        public static int GetIsPraise(int comment, string userId)
        {
            string sql = @" SELECT  VoteState FROM Marketing.dbo.[CommentPraise] with(nolock) WHERE CommentId=@CommentId and UserId=@UserId";
            var sqlParms = new SqlParameter[]
                {
                    new SqlParameter("@UserId", userId),
                     new SqlParameter("@CommentId", comment)

                };
            return Convert.ToInt32(DbHelper.ExecuteScalar(true, sql, CommandType.Text, sqlParms));
        }

        public static DataTable GetArticleCommentTop3(int PKID, string UserID)
        {
            string sql = @"SELECT TOP 3
		                A.[ID],
		                A.[PKID],
		                A.[Category],
		                A.[Title],
		                A.[PhoneNum],
		                A.[UserID],
		                A.[UserHead],
		                A.[CommentContent],
		                A.[CommentTime],
		                A.[AuditStatus],
		                A.[AuditTime],
		                A.[UserName],
		                A.[UserGrade],
		                A.[Sort],
		                A.[ParentID],
		                A.[RealName],
		                A.[Sex],
		                COUNT(B.CommentId) AS DianZanNum,
		                ROW_NUMBER() OVER (ORDER BY (COUNT(B.CommentId)) DESC) AS RowNumber
                FROM	Marketing.dbo.tbl_Comment AS A WITH (NOLOCK),
		                [Marketing].[dbo].[CommentPraise] AS B WITH (NOLOCK)
                WHERE	PKID = @PKID
		                AND (AuditStatus = 2
			                 OR (A.UserID = @UserID
				                 AND AuditStatus = 0
				                )
			                )
		                AND A.ID = B.CommentId
                GROUP BY A.[ID],
		                A.[PKID],
		                A.[Category],
		                A.[Title],
		                A.[PhoneNum],
		                A.[UserID],
		                A.[UserHead],
		                A.[CommentContent],
		                A.[CommentTime],
		                A.[AuditStatus],
		                A.[AuditTime],
		                A.[UserName],
		                A.[UserGrade],
		                A.[Sort],
		                A.[ParentID],
		                A.[RealName],
		                A.[Sex]";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;

                #region AddParameters
                cmd.Parameters.AddWithValue("@PKID", PKID);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                #endregion

                DataTable dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                return dt;
            }
        }

        public static int InsertCommentPraise(CommentPraise model)
        {
            string sql = @"  INSERT INTO [Marketing].[dbo].[CommentPraise]
                               (
		                       UserId,
		                       CommentId,
                               CreateTime,
                               UserHead,
                               UserName,
                               RealName,
                               Sex,
                               PhoneNum,
                               VoteState
		                       )
		                       VALUES
		                       (
                               @UserId,
		                       @CommentId,
                               GETDATE(),
                               @UserHead,
                               @UserName,
                               @RealName,
                               @Sex,
                               @PhoneNum,
                               1
		                       )";
            string countSql = @"SELECT COUNT(0) FROM [Marketing].[dbo].[CommentPraise] WHERE UserId = @UserId  AND CommentId = @CommentId ";

            var sqlParmsCount = new SqlParameter[]
                {
                    new SqlParameter("@UserId", model.UserId??string.Empty),
                     new SqlParameter("@CommentId", model.CommentId)
                };

            var sqlParms = new SqlParameter[]
                {
                    new SqlParameter("@UserId", model.UserId??string.Empty),
                     new SqlParameter("@CommentId", model.CommentId),
                      new SqlParameter("@UserHead", model.UserHead??string.Empty),
                       new SqlParameter("@UserName", model.UserName??string.Empty),
                        new SqlParameter("@RealName", model.RealName??string.Empty),
                         new SqlParameter("@Sex", model.Sex??string.Empty),
                            new SqlParameter("@PhoneNum", model.PhoneNum??string.Empty)

                };

            if ((int)DbHelper.ExecuteScalar(countSql, CommandType.Text, sqlParmsCount) > 0)
            {
                return 200;
            }
            else
            {
                return DbHelper.ExecuteNonQuery(sql, CommandType.Text, sqlParms);
            }
        }

        /// <summary>
        /// 评论点赞取消点赞
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Status">返回值 true 点赞  false 取消</param>
        /// <returns></returns>
        public static int InsertCommentPraiseNew(CommentPraise model, out int Status, out int articleId)
        {
            using (var cmd = new SqlCommand("Marketing..InsertCommentPraiseNew"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", model.UserId);
                cmd.Parameters.AddWithValue("@CommentId", model.CommentId);
                cmd.Parameters.AddWithValue("@UserHead", model.UserHead);
                cmd.Parameters.AddWithValue("@UserName", model.UserName);
                cmd.Parameters.AddWithValue("@RealName", model.RealName);
                cmd.Parameters.AddWithValue("@Sex", model.Sex);
                cmd.Parameters.AddWithValue("@PhoneNum", model.PhoneNum);
                cmd.Parameters.AddWithValue("@VoteState", model.VoteState);
                cmd.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Status", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@ArticleId", DbType.Int16).Direction = ParameterDirection.Output;
                DbHelper.ExecuteScalar(cmd);
                Status = Convert.ToInt32(cmd.Parameters["@Status"].Value);
                articleId = cmd.Parameters["@ArticleId"].Value == DBNull.Value ? 0 : Convert.ToInt16(cmd.Parameters["@ArticleId"].Value);
                return Convert.ToInt32(cmd.Parameters["@Result"].Value);
            }
        }


        public static int CountComment(int id)
        {
            string sql = @"SELECT COUNT(0) FROM Marketing.dbo.tbl_Comment with(nolock) WHERE ParentID = @Id";
            return (int)DbHelper.ExecuteScalar(true, sql, CommandType.Text, new SqlParameter("@Id", id));
        }

        public static int CountPraise(int id)
        {
            string sql = @"SELECT COUNT(0) FROM Marketing.dbo.[CommentPraise] with(nolock) WHERE CommentId = @Id  AND VoteState=1";
            return (int)DbHelper.ExecuteScalar(true, sql, CommandType.Text, new SqlParameter("@Id", id));
        }


        public static int GetCountCommentBYId(int PKID, string UserID)
        {
            string sql = @"SELECT COUNT(0) FROM Marketing.dbo.tbl_Comment with(nolock)
                           WHERE   PKID = @PKID
                           AND (AuditStatus = 2
                             OR (UserID = @UserId
                                 AND AuditStatus = 0)) ";

            var sqlParms = new SqlParameter[]
                {
                    new SqlParameter("@UserId", UserID),
                     new SqlParameter("@PKID", PKID),


                };
            return (int)DbHelper.ExecuteScalar(true, sql, CommandType.Text, sqlParms);
        }
        /// <summary>
        /// 发现站点提供给败家的RSS数据源
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<BaiJiaCommentModel>> SelectOrderCommentByBaiJia()
        {
            using (var dbHelper = Tuhu.DbHelper.CreateDbHelper(System.Configuration.ConfigurationManager.ConnectionStrings["GungnirUser"].ConnectionString))
            using (var cmd = new SqlCommand(@"SELECT	T.*,
		                                                VP.DisplayName,
		                                                VP.CP_Brand,
		                                                VP.CP_Tire_Pattern,
		                                                VP.TireSize,
		                                                VP.CP_Tire_LoadIndex + VP.CP_Tire_SpeedRating AS SpeedRating
                                                FROM	( SELECT TOP 200
					                                                C2.CommentExtAttr,
					                                                C2.CommentContent,
					                                                C2.CommentType,
					                                                C2.SingleTitle,
					                                                C2.CommentProductId,
					                                                C2.CommentId,
					                                                C2.CommentImages
		                                                  FROM		Gungnir..tbl_Comment AS C2 ( NOLOCK )
		                                                  WHERE		C2.CommentStatus = 2
					                                                AND C2.CommentImages IS NOT NULL
					                                                AND C2.CommentImages <> ''
					                                                AND C2.CommentContent <> N'暂无评价'
		                                                  ORDER BY	C2.CommentId DESC
		                                                ) AS T
                                                JOIN	Tuhu_productcatalog..vw_Products AS VP ( NOLOCK )
		                                                ON T.CommentProductId = VP.PID COLLATE Chinese_PRC_CI_AS;"))
            {
                return await dbHelper.ExecuteSelectAsync<BaiJiaCommentModel>(cmd);
            }
        }

        public static async Task<string> SelectCommentCarInfoByCarID(string carid)
        {
            using (var dbHelper = new SqlDbHelper(System.Configuration.ConfigurationManager.ConnectionStrings["GungnirUser"].ConnectionString))
            using (var cmd=new SqlCommand("SELECT CO.Vehicle FROM Tuhu_profiles..CarObject AS CO (NOLOCK) WHERE CO.CarID=@CarID"))
            {
                cmd.Parameters.AddWithValue("@CarID", carid);
                var result=await dbHelper.ExecuteScalarAsync(cmd);
                return result?.ToString();
            }
        }
    }
}
