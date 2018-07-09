using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalArticleComment
    {
        private static string CreateWhere(List<string> sl)
        {
            StringBuilder sb = new StringBuilder();
            var filtersl = sl.Where(s => !string.IsNullOrWhiteSpace(s));
            foreach (var s in filtersl)
            {
                if (sb.Length == 0)
                {
                    sb.Append(" WHERE ");
                    sb.Append(s);
                }
                else
                {
                    sb.Append(" AND ");
                    sb.Append(s);
                }
            }
            return sb.ToString();
        }
        public static List<ArticleComment> SelectBy(SqlConnection connection, int? PageSize, int? PageIndex,
            DateTime CommentTime, string Category, string Title, string CommentContent, string PhoneNum, int? AuditStatus, int Index = 1)
        {
            int _PageSize = PageSize.GetValueOrDefault(20);
            int _PageIndex = Index;
            var sqlParamters = new[]
            {
                new SqlParameter("@PageIndex",_PageIndex),
                new SqlParameter("@PageSize",_PageSize)
            }.ToList();

            string CommentTimeStr = string.Empty;
            string CategoryStr = string.Empty;
            string TitleStr = string.Empty;
            string CommentContentStr = string.Empty;
            string PhoneNumStr = string.Empty;
            string AuditStatusStr = string.Empty;
            if (CommentTime != null && CommentTime != DateTime.MinValue)
            {
                CommentTimeStr = " CommentTime BETWEEN @CommentTime AND (@CommentTime + 1) ";
                sqlParamters.Add(new SqlParameter("@CommentTime", CommentTime));
            }
            //if (!string.IsNullOrWhiteSpace(Category))
            //{
            //    CategoryStr = " Type = @Type ";
            //    sqlParamters.Add(new SqlParameter("@Type", Category));
            //}
            if (!string.IsNullOrWhiteSpace(Category))
            {
                switch (Category)
                {
                    case "0":   //文章评论
                        CategoryStr = " PType = @Type ";
                        break;
                    case "1":   //专题相关
                        CategoryStr = " PType = @Type OR (PType = 2 AND (ISNULL(ParentID,0) > 0 OR ISNULL(TOPID,0) > 0)) ";
                        break;
                    case "2":   //说说
                        CategoryStr = " PType = @Type AND (ISNULL(ParentID,0) = 0 AND ISNULL(TOPID,0) = 0)  ";
                        break;
                    case "3":   //问题相关
                        CategoryStr = " PType = @Type ";
                        break;
                    case "10":   //晒图评论
                        CategoryStr = " PType = @Type ";
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrWhiteSpace(Category))
                    sqlParamters.Add(new SqlParameter("@Type", Category));
            }
            if (!string.IsNullOrWhiteSpace(Title))
            {
                TitleStr = " Title LIKE '%'+@Title+'%' ";
                sqlParamters.Add(new SqlParameter("@Title", Title));
            }
            if (!string.IsNullOrWhiteSpace(CommentContent))
            {
                TitleStr = " CommentContent LIKE '%'+@CommentContent+'%' ";
                sqlParamters.Add(new SqlParameter("@CommentContent", CommentContent));
            }
            if (!string.IsNullOrWhiteSpace(PhoneNum))
            {
                PhoneNumStr = " PhoneNum = @PhoneNum ";
                sqlParamters.Add(new SqlParameter("@PhoneNum", PhoneNum));
            }
            if (AuditStatus.HasValue)
            {
                AuditStatusStr = " AuditStatus=@AuditStatus ";
                sqlParamters.Add(new SqlParameter("@AuditStatus", AuditStatus.Value));
            }
            var whereStr = CreateWhere(new List<string>() { CommentTimeStr, CategoryStr, TitleStr, PhoneNumStr, AuditStatusStr });

            var sqlcmd = @"SELECT * FROM
                           (
	                            SELECT ROW_NUMBER() OVER (ORDER BY CommentTime DESC, PKID DESC) AS RowNumber,* FROM 
	                            (
		                            SELECT *,'PType' = ISNULL((SELECT TOP 1 tab1.Type FROM Marketing..tbl_Comment AS tab1 WITH (NOLOCK) WHERE tab1.id = ISNULL(T.TopID,T.ParentID)),T.Type)
		                            FROM   Marketing.dbo.tbl_Comment AS T WITH (NOLOCK)
	                            ) AS TT " + whereStr + @"
                            )AS TTT
                            WHERE TTT.RowNumber BETWEEN (@PageIndex - 1) * @PageSize + 1 AND  @PageIndex * @PageSize
                            ORDER BY TTT.RowNumber";

            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sqlcmd, sqlParamters.ToArray()).ConvertTo<ArticleComment>().ToList();
        }
        public static List<ArticleComment> SelectAll(SqlConnection connection)
        {
            var sql = "SELECT * FROM Marketing.dbo.tbl_Comment WITH (NOLOCK)";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<ArticleComment>().ToList();
        }
        /// <summary>
        /// 评论置顶
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int UpdateSort(SqlConnection connection, int ID)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@ID",ID)
            };
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "UPDATE Marketing.dbo.[tbl_Comment] SET Sort = 1 WHERE ID = @ID", sqlParamters);
        }

        /// <summary>
        /// 评论批量置顶
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int UpdateSortBatch(SqlConnection connection, IEnumerable<int> IDs, int sort = 0)
        {
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "UPDATE Marketing.dbo.[tbl_Comment] SET Sort = " + sort + " WHERE ID IN (" + string.Join(",", IDs) + ")");
        }

        public static int Delete(SqlConnection connection, int ID)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@ID",ID)
            };
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM Marketing.dbo.tbl_Comment WHERE ID=@ID", sqlParamters);
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ID"></param>
        public static int DeleteBatch(SqlConnection connection, IEnumerable<int> IDs)
        {
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM Marketing.dbo.tbl_Comment WHERE ID IN (" + string.Join(",", IDs) + ")");
        }
        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int Pass(SqlConnection connection, int ID)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@ID",ID)
            };
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"UPDATE  Marketing.dbo.tbl_Comment
					SET     AuditStatus = 2,AuditTime=GETDATE()
					WHERE   ID = @ID");
        }
        /// <summary>
        /// 审核不通过
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int UnPass(SqlConnection connection, int ID)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@ID",ID)
            };
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"UPDATE  Marketing.dbo.tbl_Comment
					SET     AuditStatus = 1,AuditTime=GETDATE()
					WHERE   ID = @ID");
        }
        /// <summary>
        /// 批量审核通过
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public static int PassBatch(SqlConnection connection, IEnumerable<int> IDs)
        {
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"UPDATE  Marketing.dbo.tbl_Comment
					SET     AuditStatus = 2 ,AuditTime=GETDATE()
					WHERE   ID  IN (" + string.Join(",", IDs) + ")");
        }
        /// <summary>
        /// 批量审核不通过
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public static int UnPassBatch(SqlConnection connection, IEnumerable<int> IDs)
        {
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"UPDATE  Marketing.dbo.tbl_Comment
					SET     AuditStatus = 1,AuditTime=GETDATE()
					WHERE   ID  IN (" + string.Join(",", IDs) + ")");
        }
        /// <summary>
        /// 获取评论
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static ArticleComment GetByID(SqlConnection connection, int ID)
        {
            ArticleComment _ArticleComment = null;
            var parameters = new[]
            {
                new SqlParameter("@ID", ID)
            };
            var sqlcmd = @"SELECT TOP 1 
									[ID],
									[PKID],
									[Category],
									[Title],
									[PhoneNum],
									[UserID],
									[UserHead],
									[CommentContent],
									[CommentTime],
									[AuditStatus],
									[AuditTime],
                                    [Sort]
					FROM Marketing.dbo.[tbl_Comment] WITH (NOLOCK) WHERE ID=@ID";
            using (var dt = SqlHelper.ExecuteDataTable(connection, CommandType.Text, sqlcmd, parameters))
            {
                _ArticleComment = dt.Rows.Cast<DataRow>().Select(row =>
                {
                    return new ArticleComment(row);
                }).FirstOrDefault();
            }
            return _ArticleComment;
        }

        /// <summary>
        /// 修改文章表修改问题评论
        /// </summary>
        public static bool UpdateArticleToComment(SqlConnection connection, int pkid, int type, int isShow)
        {
            var sqlcmd = @"
                    BEGIN
                         UPDATE Marketing..tbl_Article
                         SET IsShow = @IsShow
                         WHERE PKID = @PKID

                         IF(@Type = 3 AND @IsShow = 0)
                         BEGIN
	                         UPDATE Marketing.dbo.tbl_Comment
	                         SET AuditStatus = 1
	                         WHERE ID IN
	                         (
		                         SELECT ID FROM Marketing..tbl_Comment WITH(NOLOCK)WHERE PKID = @PKID AND ISNULL(ParentID,0) = 0 AND ISNULL(TopID,0) = 0 
	                         )
                         END     
                    END ";

            SqlParameter[] sqlparams = new SqlParameter[] {
                new SqlParameter("@PKID",pkid),
                new SqlParameter("@Type",type),
                new SqlParameter("@IsShow",isShow)
            };

            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sqlcmd, sqlparams) > 0;
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static IEnumerable<ArticleComment> GetByIDs(SqlConnection connection, IEnumerable<int> IDs)
        {
            try
            {
                IEnumerable<ArticleComment> _ArticleComment = new List<ArticleComment>();

                var sqlcmd = @"SELECT [ID],
									  [PKID],
									  [Category],
									  [Title],
									  [PhoneNum],
									  [UserID],
									  [UserHead],
									  [CommentContent],
									  [CommentTime],
									  [AuditStatus],
									  [AuditTime],
                                      [Sort],
                                      [Type],
                                      [UserName],
                                      [RealName],
                                      [ParentID] = ISNULL(ParentID,0),
                                      [TopID] = ISNULL(TopID,0),
                                      'PKIDType' = (SELECT TOP 1 tab1.Type FROM Marketing..[tbl_Article] AS tab1 WITH(NOLOCK) WHERE tab1.PKID = tab2.PKID),
                                      'PType' = ISNULL((SELECT TOP 1 tab1.Type FROM Marketing..tbl_Comment AS tab1 WITH (NOLOCK) WHERE tab1.id = ISNULL(tab2.TopID,tab2.ParentID)),tab2.Type),
                                      'UserInfos' =
                                       CASE
                                       WHEN (SELECT TOP 1 tab1.Type FROM Marketing..[tbl_Article] AS tab1 WITH(NOLOCK) WHERE tab1.PKID = tab2.PKID) = 3 AND ISNULL(tab2.ParentID,0) = 0
	                                      THEN  '1|' +(SELECT TOP 1 CreatorID + '|' + CreatorInfo FROM Marketing..[tbl_Article] WITH(NOLOCK) WHERE PKID = tab2.PKID)
                                       ELSE
	                                      '2|' + (SELECT TOP 1 UserID +'|'+UserHead+'|'+RealName FROM Marketing..[tbl_Comment] WITH(NOLOCK) WHERE ID = tab2.ParentID)
                                       END
					                  FROM Marketing.dbo.[tbl_Comment] AS tab2 WITH (NOLOCK) WHERE ID in(" + string.Join(",", IDs) + ") ";
                using (var dt = SqlHelper.ExecuteDataTable(connection, CommandType.Text, sqlcmd, null))
                {
                    _ArticleComment = dt.Rows.Cast<DataRow>().Select(row =>
                    {
                        return new ArticleComment(row);
                    });
                }
                return _ArticleComment;
            }
            catch (Exception ex)
            {
                return null;
            } 
        }

        /// <summary>
        /// 获取文章对应所有评论
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static IEnumerable<ArticleComment> GetByPKID(SqlConnection connection, int PKID, string Type)
        {
            IEnumerable<ArticleComment> _ArticleComments = new List<ArticleComment>();
            var parameters = new[]
            {
                new SqlParameter("@PKID", PKID)
            };
            var sqlcmd = @"SELECT 	[ID],
									[PKID],
                                    [ParentID],
                                    [Type],
									[Category],
									[Title],
									[PhoneNum],
									[UserID],
									[UserHead],
									[CommentContent],
									[CommentTime],
									[AuditStatus],
									[AuditTime],
                                    [Sort],
                                    [UserName],
                                    [CommentImage],
                                    'PType' = ISNULL((SELECT TOP 1 tab1.Type FROM Marketing..tbl_Comment AS tab1 WITH (NOLOCK) WHERE tab1.id = ISNULL(T.TopID,T.ParentID)),T.Type)
					FROM Marketing.dbo.[tbl_Comment] AS T WITH (NOLOCK) WHERE PKID=@PKID ";
            sqlcmd += string.IsNullOrEmpty(Type) ? " AND Type!=10" : " AND Type=" + Type;
            using (var _Dt = SqlHelper.ExecuteDataTable(connection, CommandType.Text, sqlcmd, parameters))
            {
                _ArticleComments = _Dt.Rows.Cast<DataRow>().Select(row =>
                {
                    return new ArticleComment(row);
                });
            }
            return _ArticleComments;
        }

        /// <summary>
        /// 插入评论
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Insert(SqlConnection connection, ArticleComment model)
        {
            var sqlParams = new[] {
             new SqlParameter("@PKID",model.PKID),
             new SqlParameter("@ParentID",model.ParentID),
             new SqlParameter("@PhoneNum",model.PhoneNum),
             new SqlParameter("@UserID",model.UserID),
             new SqlParameter("@UserHead",model.UserHead),
             new SqlParameter("@CommentContent",model.CommentContent),
             new SqlParameter("@CommentTime",model.CommentTime),
             new SqlParameter("@AuditStatus",model.AuditStatus),
             new SqlParameter("@AuditTime",model.AuditTime),
             new SqlParameter("@UserName",model.UserName),
             new SqlParameter("@UserGrade",model.UserGrade),
             new SqlParameter("@Sort",model.Sort),
             new SqlParameter("@RealName",model.RealName),
             new SqlParameter("@Sex",model.Sex),
             new SqlParameter("@Type",model.Type),
             new SqlParameter("@Praise",model.Praise),
             new SqlParameter("@TopID",model.TopID)
            };

            var strSql = @"
            INSERT INTO Marketing.dbo.tbl_Comment
                    ( PKID,
                      ParentID,
                      PhoneNum,
                      UserID,
                      UserHead,
                      CommentContent,
                      CommentTime,
                      AuditStatus,
                      AuditTime,
                      UserName,
                      UserGrade,
                      Sort,
                      RealName,
                      Sex,
                      Type,
                      Praise,
                      TopID
                    )
            VALUES  ( @PKID,
                      @ParentID,
                      @PhoneNum,
                      @UserID,
                      @UserHead,
                      @CommentContent,
                      @CommentTime,
                      @AuditStatus,
                      @AuditTime,
                      @UserName,
                      @UserGrade,
                      @Sort,
                      @RealName,
                      @Sex,
                      @Type,
                      @Praise,
                      @TopID
                    )";

            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, strSql, sqlParams) > 0 ? true : false;
        }

        public static bool UpdateShImgCommentCount(SqlConnection conn, int PKID,string op)
        {

            if (op == "-1")
            {
                string countSql = "SELECT TOP 1 commentCount FROM Marketing..tbl_ShareImagesStatistics(NOLOCK) WHERE infoId=" + PKID;
                object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, countSql);
                int commentCount = obj != null ? Convert.ToInt32(obj) : 0;
                if (commentCount > 0)
                {
                    string Sql = "UPDATE Marketing..tbl_ShareImagesStatistics SET commentCount=commentCount-1 WHERE infoId=" + PKID;
                    return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, Sql) > 0;
                }
                else
                    return true;
            }
            else if (op == "+1")
            {
                string Sql = "UPDATE Marketing..tbl_ShareImagesStatistics SET commentCount=commentCount+1 WHERE infoId=" + PKID;
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, Sql) > 0;
            }
            else
                return true;          
        }

    }
}