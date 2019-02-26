using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalArticle
    {

        public static List<Article> SelectBy(SqlConnection connection, string KeyStr, int? PageSize, int? PageIndex, int? type)
        {
            int _PageSize = PageSize.GetValueOrDefault(20);
            int _PageIndex = PageIndex.GetValueOrDefault(1);
            string _AddSql = string.IsNullOrEmpty(KeyStr) ? string.Empty : " And SmallTitle like @KeyStr";
            if (type != -1)
            {
                _AddSql += " AND Type = @type";
            }

            var sqlcmd = @"SELECT  *,
									(SELECT COUNT(1)
									 FROM   Marketing..tbl_Comment WITH (NOLOCK)
									 WHERE  T.PKID = PKID
									) AS CommentCount,
                                    (SELECT COUNT(1)
									 FROM   Marketing..tbl_MyRelatedArticle WITH (NOLOCK)
									 WHERE  T.PKID = PKID And (convert(varchar(10),OperateTime,120)=convert(varchar(10),DateAdd(day, -1, getdate()),120)) And Type=3 
									) AS YesterdayClickCount,
                                    --(SELECT  Count(1) 
                                    --FROM Marketing..tbl_ArticleNewList  WITH(NOLOCK) 
                                    --WHERE  convert(varchar(10),CreateTime,120)=convert(varchar(10),DateAdd(day, -1, getdate()),120) And T.PKID = ArticleId And Type = 1
                                    --) As SkipLocalUrlCount,
                                    --(SELECT  Count(1) 
                                    --FROM Marketing..tbl_ArticleNewList  WITH(NOLOCK) 
                                    --WHERE  convert(varchar(10),CreateTime,120)=convert(varchar(10),DateAdd(day, -1, getdate()),120) And T.PKID = ArticleId And Type = 2
                                    --) As SkipTaoBaoUrlCount,
                                    C.CategoryName AS CategoryName
						FROM    (SELECT *,
										ROW_NUMBER() OVER (ORDER BY PublishDateTime DESC, PKID DESC) AS RowNumber
								 FROM   Marketing.dbo.tbl_Article WITH (NOLOCK) WHERE 1=1 " + _AddSql + @"
								) AS T left join (select * from Marketing..tbl_NewCategoryList WITH (NOLOCK)) as C on T.Category = C.id
						WHERE   T.RowNumber BETWEEN (@PageIndex - 1) * @PageSize + 1
											AND     @PageIndex * @PageSize
						ORDER BY T.RowNumber";
            var sqlParamters = new[]
            {
                new SqlParameter("@KeyStr","%"+KeyStr+"%"),
                new SqlParameter("@PageIndex",_PageIndex),
                new SqlParameter("@PageSize",_PageSize),
                new SqlParameter("@type",type)
            };
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sqlcmd, sqlParamters).ConvertTo<Article>().ToList();
        }

        public static int YesterdaySumCount(SqlConnection connection)
        {
            const string sql = @"SELECT  Count(1) 
                                    FROM Marketing..tbl_MyRelatedArticle  WITH(NOLOCK) 
                                    WHERE convert(varchar(10),OperateTime,120)=convert(varchar(10),DateAdd(day, -1, getdate()),120) And Type = 3";
            return (int)SqlHelper.ExecuteScalar(connection, CommandType.Text, sql);
        }
        public static List<Article> SelectArticle(SqlConnection connection, string KeyStr, string StartTime, string EndTime, string CategoryTag,int? PageSize, int? PageIndex, int? type)
        {
            int _PageSize = PageSize.GetValueOrDefault(20);
            int _PageIndex = PageIndex.GetValueOrDefault(1);
            string _AddSql = "";
            if (!string.IsNullOrEmpty(KeyStr))
            {
                _AddSql = type == 3 ? " And Content like @KeyStr " : " And SmallTitle like @KeyStr ";
            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                _AddSql += string.Format(" And PublishDateTime > '{0}' ", StartTime);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                _AddSql += string.Format(" And PublishDateTime < '{0}' ", DateTime.Parse(EndTime).AddDays(1));
            }
            if (!string.IsNullOrEmpty(CategoryTag))
            {
                _AddSql += " And CategoryTags like @CategoryTags ";
            }
            //string _AddSql = string.IsNullOrEmpty(KeyStr) ? string.Empty : " And SmallTitle like @KeyStr";
            if (type != -1)
            {
                _AddSql += " AND Type = @type";
            }

            var sqlcmd = @"SELECT  PKID,
                                    (
                                    SELECT COUNT(1) FROM   Marketing..tbl_Comment WITH (NOLOCK) WHERE  T.PKID = PKID
                                    ) AS CommentCount,
                                   SmallImage,
                                   SmallTitle,
                                    Image,
                                   BigTitle,
                                   PublishDateTime,
                                   ContentUrl,
                                   CategoryName,
                                   Category,
                                   CommentIsActive,
                                   Type,
                                   Content,
                                   IsShow,
                                   CreatorID,
                                   CreatorInfo,         
                                   C.CategoryName AS CategoryName
						FROM    (SELECT *,
										ROW_NUMBER() OVER (ORDER BY PublishDateTime DESC, PKID DESC) AS RowNumber
								 FROM   Marketing.dbo.tbl_Article WITH (NOLOCK) WHERE 1=1 and IsDelete=0  " + _AddSql + @"
								) AS T LEFT JOIN  (SELECT * FROM  Marketing..tbl_NewCategoryList WITH (NOLOCK)) as C on T.Category = C.id
						WHERE   T.RowNumber BETWEEN (@PageIndex - 1) * @PageSize + 1
											AND     @PageIndex * @PageSize
						ORDER BY T.RowNumber";
            var sqlParamters = new[]
            {
                new SqlParameter("@KeyStr","%"+KeyStr+"%"),
                new SqlParameter("@PageIndex",_PageIndex),
                new SqlParameter("@PageSize",_PageSize),
                new SqlParameter("@CategoryTags","%"+CategoryTag+"%"),
                new SqlParameter("@type",type)
            };
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sqlcmd, sqlParamters).ConvertTo<Article>().ToList();
        }
        /// <summary>
        /// 重载分页查询方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public static List<Article> SelectBy(SqlConnection connection, int? PageSize, int? PageIndex)
        {
            int _PageSize = PageSize.GetValueOrDefault(30);
            int _PageIndex = PageIndex.GetValueOrDefault(1);
            var sqlcmd = @"SELECT  *
                            FROM    ( SELECT    A.CurrentOperateTime ,
                                                A.CurrentClickCount ,
                                                ROW_NUMBER() OVER ( ORDER BY A.CurrentOperateTime DESC ) AS RowNumber
                                      FROM      ( SELECT    CONVERT(VARCHAR(10), CreateTime, 120) AS CurrentOperateTime ,
                                                            COUNT(1) AS CurrentClickCount
                                                  FROM      Marketing..[HD_BrowseHistory]  WITH ( NOLOCK )
                                                  WHERE     Type = 3
                                                  GROUP BY  CONVERT(VARCHAR(10), CreateTime, 120)
                                                ) AS A
                                    ) AS B
                            WHERE   RowNumber BETWEEN ( @PageIndex - 1 ) * @PageSize + 1
                                              AND     @PageIndex * @PageSize";
            var sqlParamters = new[]
            {
                new SqlParameter("@PageIndex",_PageIndex),
                new SqlParameter("@PageSize",_PageSize)
            };
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sqlcmd, sqlParamters).ConvertTo<Article>().ToList();
        }

        public static List<Article> SelectAll(SqlConnection connection)
        {
            var sql = "SELECT * FROM Marketing.dbo.tbl_Article WITH (NOLOCK)";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<Article>().ToList();
        }
        public static void Delete(SqlConnection connection, int PKID)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",PKID)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM Marketing.dbo.tbl_Article WHERE PKID=@PKID", sqlParamters);
        }

        public static void Add(SqlConnection connection, Article article, out string contentUrl, out int id, string locationAddress)
        {
            const string sql = @"SET @ContentUrl = @LocationAddress + convert(varchar(200),NEWID())
            IF (@PKID = 0)
	            BEGIN
		            INSERT INTO Marketing.dbo.tbl_Article
		            (
			            [Catalog] ,
			            [Image] ,
			            SmallImage ,
			            SmallTitle ,
			            BigTitle ,
			            TitleColor ,
			            Brief ,
			            Content ,
			            ContentUrl ,
			            [Source] ,
			            PublishDateTime ,
			            CreateDateTime ,
			            LastUpdateDateTime ,
			            RedirectUrl ,
			            Vote ,
			            Category ,
			            CommentIsActive ,
			            ArticleBanner ,
			            SmallBanner,
			            Type,
			            IsShow,
			            CategoryTags,
			            ShowImages,
			            ShowType,
			            IsDescribe,
			            ContentHtml,
                        IsShowTouTiao
		            )
		            VALUES
		            (
			            @Catalog ,
			            @Image ,
			            @SmallImage ,
			            @SmallTitle ,
			            @BigTitle ,
			            @TitleColor ,
			            @Brief ,
			            @Content ,
			            @ContentUrl + '_' +CONVERT(VARCHAR(200),IDENT_CURRENT('Marketing.dbo.tbl_Article')) + '.html',
			            @Source ,
			            @PublishDateTime ,
			            @CreateDateTime ,
			            @LastUpdateDateTime ,
			            @RedirectUrl ,
			            @Vote ,
			            @Category ,
			            @CommentIsActive ,
			            @ArticleBanner ,
			            @SmallBanner,
			            @Type,
			            @IsShow,
			            @CategoryTags,
			            @ShowImages,
			            @ShowType,
			            @IsDescribe,
			            @ContentHtml ,
                        @IsShowTouTiao
		            )
		            SET @ReturnUrl = @ContentUrl + '_' +CONVERT(VARCHAR(200),IDENT_CURRENT('Marketing.dbo.tbl_Article')) + '.html'
		            SET @ReturnID = IDENT_CURRENT('Marketing.dbo.tbl_Article')
	            END
            ELSE
	            BEGIN
		            UPDATE  Marketing.dbo.tbl_Article
		            SET     [Catalog] = @Catalog ,
				            [Image] = @Image ,
				            SmallImage = @SmallImage ,
				            SmallTitle = @SmallTitle ,
				            BigTitle = @BigTitle ,
				            TitleColor = @TitleColor ,
				            Brief = @Brief ,
				            Content = @Content ,
				            ContentUrl = @ContentUrl + '_' + CONVERT(VARCHAR(2000),@PKID) + '.html' ,
				            [Source] = @Source ,
				            PublishDateTime = @PublishDateTime ,
				            LastUpdateDateTime = @LastUpdateDateTime ,
				            RedirectUrl = @RedirectUrl ,
				            Vote = @Vote ,
				            Category = @Category ,
				            CommentIsActive = @CommentIsActive ,
				            ArticleBanner = @ArticleBanner ,
				            SmallBanner = @SmallBanner,
				            Type=@Type,
				            IsShow=@IsShow,
				            CategoryTags=@CategoryTags,
				            ShowImages=@ShowImages,
				            ShowType=@ShowType,
				            IsDescribe=@IsDescribe,
				            ContentHtml=@ContentHtml ,
                            IsShowTouTiao=@IsShowTouTiao
		            WHERE   PKID = @PKID
		            SET @ReturnUrl = @ContentUrl + '_' + CONVERT(VARCHAR(2000),@PKID) + '.html'
		            SET @ReturnID = @PKID
	            END";
            try
            {
                SqlParameter[] sqlParamters = new SqlParameter[]
                {
                    new SqlParameter("@PKID" ,SqlDbType.Int),
                    new SqlParameter("@Catalog" ,SqlDbType.Int),
                    new SqlParameter("@Image" ,SqlDbType.VarChar),
                    new SqlParameter("@SmallImage" ,SqlDbType.VarChar),
                    new SqlParameter("@SmallTitle", SqlDbType.NVarChar),
                    new SqlParameter("@BigTitle" ,SqlDbType.NVarChar),
                    new SqlParameter("@TitleColor", SqlDbType.Char),
                    new SqlParameter("@Brief" ,SqlDbType.NVarChar),
                    new SqlParameter("@Content" ,SqlDbType.NVarChar),
                    new SqlParameter("@ContentUrl", SqlDbType.VarChar),
                    new SqlParameter("@Source" ,SqlDbType.NVarChar),
                    new SqlParameter("@PublishDateTime", SqlDbType.SmallDateTime),
                    new SqlParameter("@CreateDateTime", SqlDbType.DateTime),
                    new SqlParameter("@LastUpdateDateTime", SqlDbType.DateTime),
                    new SqlParameter("@RedirectUrl" ,SqlDbType.NVarChar),
                    new SqlParameter("@Vote", SqlDbType.Int),
                    new SqlParameter("@Category", SqlDbType.NVarChar ),
                    new SqlParameter("@CommentIsActive" ,SqlDbType.Bit),
                    new SqlParameter("@ArticleBanner", SqlDbType.VarChar),
                    new SqlParameter("@SmallBanner", SqlDbType.VarChar),
                    new SqlParameter("@ReturnUrl" ,SqlDbType.VarChar,2000),
                    new SqlParameter("@ReturnID",SqlDbType.Int),
                    new SqlParameter("@LocationAddress",SqlDbType.VarChar),
                    new SqlParameter("@Type",SqlDbType.Int),
                    new SqlParameter("@IsShow",SqlDbType.Int),
                    new SqlParameter("@CategoryTags",SqlDbType.NVarChar),
                    new SqlParameter("@ShowImages",SqlDbType.NVarChar),
                    new SqlParameter("@ShowType",SqlDbType.Int),
                    new SqlParameter("@IsDescribe",SqlDbType.Bit),
                    new SqlParameter("@ContentHtml" ,SqlDbType.NVarChar),
                    new SqlParameter("@IsShowTouTiao",SqlDbType.Int)
                };
                sqlParamters[0].Value = article.PKID;
                sqlParamters[1].Value = article.Catalog;
                sqlParamters[2].Value = article.Image ?? string.Empty;
                sqlParamters[3].Value = article.SmallImage ?? string.Empty;
                sqlParamters[4].Value = article.SmallTitle ?? string.Empty;
                sqlParamters[5].Value = article.BigTitle ?? string.Empty;
                sqlParamters[6].Value = article.TitleColor ?? string.Empty;
                sqlParamters[7].Value = article.Brief ?? string.Empty;
                sqlParamters[8].Value = article.Content ?? string.Empty;
                sqlParamters[9].Value = article.ContentUrl ?? string.Empty;
                sqlParamters[10].Value = article.Source ?? string.Empty;
                sqlParamters[11].Value = article.PublishDateTime;
                sqlParamters[12].Value = DateTime.Now;
                sqlParamters[13].Value = DateTime.Now;
                sqlParamters[14].Value = article.RedirectUrl ?? string.Empty;
                sqlParamters[15].Value = article.Vote;
                sqlParamters[16].Value = article.Category;
                sqlParamters[17].Value = article.CommentIsActive;
                sqlParamters[18].Value = article.ArticleBanner ?? string.Empty;
                sqlParamters[19].Value = article.SmallBanner ?? string.Empty;
                sqlParamters[20].Direction = ParameterDirection.Output;
                sqlParamters[21].Direction = ParameterDirection.Output;
                sqlParamters[22].Value = locationAddress ?? string.Empty;
                sqlParamters[23].Value = article.Type;
                sqlParamters[24].Value = article.IsShow;
                sqlParamters[25].Value = article.CategoryTags;
                sqlParamters[26].Value = article.ShowImages;
                sqlParamters[27].Value = article.ShowType;
                sqlParamters[28].Value = article.IsDescribe;
                sqlParamters[29].Value = article.ContentHtml;
                sqlParamters[30].Value = article.IsShowTouTiao;

                SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlParamters);
                contentUrl = sqlParamters[20].Value == DBNull.Value ? "" : (string)sqlParamters[20].Value;
                id = sqlParamters[21].Value == DBNull.Value ? 0 : (int)sqlParamters[21].Value;
            }
            catch (Exception ex)
            {
                contentUrl = "";
                id = 0;
            }
        }

        public static void Add(SqlConnection connection, Article article)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@Catalog",article.Catalog),
                new SqlParameter("@Image",article.Image??string.Empty),
                new SqlParameter("@SmallImage",article.SmallImage??string.Empty),
                new SqlParameter("@SmallTitle",article.SmallTitle??string.Empty),
                new SqlParameter("@BigTitle",article.BigTitle??string.Empty),
                new SqlParameter("@TitleColor",article.TitleColor??string.Empty),
                new SqlParameter("@Brief",article.Brief??string.Empty),
                new SqlParameter("@Content",article.Content??string.Empty),
                new SqlParameter("@ContentUrl",article.ContentUrl??string.Empty),
                new SqlParameter("@Source",article.Source??string.Empty),
                new SqlParameter("@PublishDateTime",article.PublishDateTime),
                new SqlParameter("@CreateDateTime",DateTime.Now),
                new SqlParameter("@LastUpdateDateTime",DateTime.Now),
                new SqlParameter("@RedirectUrl",article.RedirectUrl??string.Empty),
                new SqlParameter("@Vote",article.Vote),
                new SqlParameter("@Category",article.Category),
                new SqlParameter("@CommentIsActive",article.CommentIsActive),
                new SqlParameter("@ArticleBanner",article.ArticleBanner??string.Empty),
                new SqlParameter("@SmallBanner",article.SmallBanner??string.Empty),
                new SqlParameter("@ShowImages",article.ShowImages??string.Empty),
                new SqlParameter("@ShowType",article.ShowType),
                new SqlParameter("@IsDescribe",article.IsDescribe)
            };

            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                                        @"INSERT  INTO Marketing.dbo.tbl_Article
                                ( Catalog ,
                                  Image ,
                                  SmallImage ,
                                  SmallTitle ,
                                  BigTitle ,
                                  TitleColor ,
                                  Brief ,
                                  Content ,
                                  ContentUrl ,
                                  Source ,
                                  PublishDateTime ,
                                  CreateDateTime ,
                                  LastUpdateDateTime ,
                                  RedirectUrl ,
                                  Vote ,
                                  Category ,
                                  CommentIsActive ,
                                  ArticleBanner ,
                                  SmallBanner,
                                  ShowImages,
                                  ShowType,
                                  IsDescribe
                                )
                        VALUES  ( @Catalog ,
                                  @Image ,
                                  @SmallImage ,
                                  @SmallTitle ,
                                  @BigTitle ,
                                  @TitleColor ,
                                  @Brief ,
                                  @Content ,
                                  @ContentUrl ,
                                  @Source ,
                                  @PublishDateTime ,
                                  @CreateDateTime ,
                                  @LastUpdateDateTime ,
                                  @RedirectUrl ,
                                  @Vote ,
                                  @Category ,
                                  @CommentIsActive ,
                                  @ArticleBanner ,
                                  @SmallBanner,
                                  @ShowImages,
                                  @ShowType,
                                  @IsDescribe
                                )"
                , sqlParamters);
        }
        public static void Update(SqlConnection connection, Article article)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",article.PKID),
                new SqlParameter("@Catalog",article.Catalog),
                new SqlParameter("@Image",article.Image??string.Empty),
                new SqlParameter("@SmallImage",article.SmallImage??string.Empty),
                new SqlParameter("@SmallTitle",article.SmallTitle??string.Empty),
                new SqlParameter("@BigTitle",article.BigTitle??string.Empty),
                new SqlParameter("@TitleColor",article.TitleColor??string.Empty),
                new SqlParameter("@Brief",article.Brief??string.Empty),
                new SqlParameter("@Content",article.Content??string.Empty),
                new SqlParameter("@ContentUrl",article.ContentUrl??string.Empty),
                new SqlParameter("@Source",article.Source??string.Empty),
                new SqlParameter("@PublishDateTime",article.PublishDateTime),
                new SqlParameter("@LastUpdateDateTime",DateTime.Now),
                new SqlParameter("@RedirectUrl",article.RedirectUrl??string.Empty),
                new SqlParameter("@Vote",article.Vote),
                new SqlParameter("@Category",article.Category),
                new SqlParameter("@CommentIsActive",article.CommentIsActive),
                new SqlParameter("@ArticleBanner",article.ArticleBanner??string.Empty),
                new SqlParameter("@SmallBanner",article.SmallBanner??string.Empty)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                            @"UPDATE  Marketing.dbo.tbl_Article
                            SET     Catalog = @Catalog ,
                                    Image = @Image ,
                                    SmallImage = @SmallImage ,
                                    SmallTitle = @SmallTitle ,
                                    BigTitle = @BigTitle ,
                                    TitleColor = @TitleColor ,
                                    Brief = @Brief ,
                                    Content = @Content ,
                                    ContentUrl = @ContentUrl ,
                                    Source = @Source ,
                                    PublishDateTime = @PublishDateTime ,
                                    LastUpdateDateTime = @LastUpdateDateTime ,
                                    RedirectUrl = @RedirectUrl ,
                                    Vote = @Vote ,
                                    Category = @Category ,
                                    CommentIsActive = @CommentIsActive ,
                                    ArticleBanner = @ArticleBanner ,
                                    SmallBanner = @SmallBanner
                            WHERE   PKID = @PKID"
                                            , sqlParamters);
        }

        public static Article GetArticleEntity(SqlConnection connection, int id)
        {
            var dp = new DynamicParameters();
            dp.Add("@PKID", id);
            var a= connection
                .Query<Article>(
                    "SELECT * FROM [Marketing].[dbo].[tbl_Article]  (NOLOCK) WHERE PKID=@PKID",
                    dp)
                .FirstOrDefault(); ;
            return a;
        }

        public static List<Article> GetErrorImgUrlData(SqlConnection conn, int pageSize)
        {
            const string sql = @" SELECT PKID,
									Catalog,
									Image,
									SmallImage,
									SmallTitle,
									BigTitle,
									TitleColor,
									Brief,
									Content,
									ContentUrl,
									Source,
									PublishDateTime,
									RedirectUrl,
									Vote,
									Category,
									CommentIsActive,
                                    ArticleBanner,
                                    SmallBanner,
                                    Bestla,
                                    Type,
                                    IsShow,
                                    CategoryTags,
                                    ShowImages,
                                    ShowType,
                                    IsDescribe ,
                                    IsShowTouTiao
            FROM    Marketing..tbl_Article with  ( NOLOCK )  
            WHERE   Content LIKE '%7sboir.com2%' ORDER BY PKID
			OFFSET (@PageIndex-1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY ";
            return conn.Query<Article>(sql, new { PageIndex = 1, PageSize = pageSize } ,commandType: CommandType.Text).ToList();
        }

        public static Article GetByPKID(SqlConnection connection, int PKID)
        {
            Article _Article = null;
            var parameters = new[]
            {
                new SqlParameter("@PKID", PKID)
            };
            var sqlcmd = @"SELECT TOP 1 
									PKID,
									Catalog,
									Image,
									SmallImage,
									SmallTitle,
									BigTitle,
									TitleColor,
									Brief,
									Content,
									ContentUrl,
									Source,
									PublishDateTime,
									RedirectUrl,
									Vote,
									Category,
									CommentIsActive,
                                    ArticleBanner,
                                    SmallBanner,
                                    Bestla,
                                    Type,
                                    IsShow,
                                    CategoryTags,
                                    ShowImages,
                                    ShowType,
                                    IsDescribe ,
                                    IsShowTouTiao
					FROM Marketing.dbo.tbl_Article WHERE PKID=@PKID";
            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, sqlcmd, parameters))
            {
                if (_DR.Read())
                {
                    _Article = new Article();
                    _Article.PKID = _DR.GetTuhuValue<int>(0);
                    _Article.Catalog = _DR.GetTuhuValue<int>(1);
                    _Article.Image = _DR.GetTuhuString(2);
                    _Article.SmallImage = _DR.GetTuhuString(3);
                    _Article.SmallTitle = _DR.GetTuhuString(4);
                    _Article.BigTitle = _DR.GetTuhuString(5);
                    _Article.TitleColor = _DR.GetTuhuString(6);
                    _Article.Brief = _DR.GetTuhuString(7);
                    _Article.Content = _DR.GetTuhuString(8);
                    _Article.ContentUrl = _DR.GetTuhuString(9);
                    _Article.Source = _DR.GetTuhuString(10);
                    _Article.PublishDateTime = _DR.GetTuhuValue<DateTime>(11);
                    _Article.RedirectUrl = _DR.GetTuhuString(12);
                    _Article.Vote = _DR.GetTuhuValue<int>(13);
                    _Article.Category = _DR.GetTuhuString(14);
                    _Article.CommentIsActive = _DR.GetTuhuValue<bool>(15);
                    _Article.ArticleBanner = _DR.GetTuhuString(16);
                    _Article.SmallBanner = _DR.GetTuhuString(17);
                    _Article.Bestla = _DR.GetTuhuValue<bool>(18);
                    _Article.Type = _DR.GetTuhuValue<int>(19);
                    _Article.IsShow = _DR.GetTuhuValue<int>(20);
                    _Article.CategoryTags = _DR.GetTuhuString(21);
                    _Article.ShowImages = _DR.GetTuhuString(22);
                    _Article.ShowType = _DR.GetTuhuValue<int>(23);
                    _Article.IsDescribe = _DR.GetTuhuValue<bool>(24);
                    _Article.IsShowTouTiao = _DR.GetTuhuValue<bool>(25) ? 1 : 0;
                    _Article.IsFaxianChannel = _Article.Type == 99 ? 0 : 1;//type 为 99 代表不用于发现频道 
                }
            }
            return _Article;
        }

        public static List<SeekKeyWord> GetSeekKeyWord(SqlConnection connection, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"  SELECT  *
                        FROM    ( SELECT    *
                                  FROM      ( SELECT    ROW_NUMBER() OVER ( ORDER BY KeyWord DESC ) AS ROWNUMBER ,
		                                                KeyWord ,
                                                        COUNT(KeyWord) AS Num
                                              FROM      [Marketing].[dbo].[tbl_SeekKeyWord]
                                              WHERE     1 = 1
                                              GROUP BY KeyWord
                                            ) AS T
                                ) AS PG
                           WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                                     AND     STR(@PageIndex * @PageSize)
                                                         ";
            string sqlCount = @"SELECT  COUNT(*)
                            FROM    ( SELECT    KeyWord ,                                            
                                                COUNT(KeyWord) AS Num
                                      FROM      [Marketing].[dbo].[tbl_SeekKeyWord]
                                      WHERE     1 = 1
                                      GROUP BY  KeyWord
                                              
                                    ) AS T";
            recordCount = (int)SqlHelper.ExecuteScalar(connection, CommandType.Text, sqlCount);

            var sqlPrams = new SqlParameter[]
            {
                    new SqlParameter("@PageSize",pageSize),
                      new SqlParameter("@PageIndex",pageIndex),
            };

            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, sqlPrams).ConvertTo<SeekKeyWord>().ToList();
        }

        public static bool DeleteSeekKeyWord(SqlConnection conn, string keyWord)
        {

            string sql = @"DELETE FROM  [Marketing].[dbo].[tbl_SeekKeyWord] WHERE KeyWord = @KeyWord";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@KeyWord", keyWord)) > 0;
        }
        public static List<Article> GetArticle(SqlConnection connection, string KeyStr, int? PageSize, int? PageIndex)
        {
            int _PageSize = PageSize.GetValueOrDefault(20);
            int _PageIndex = PageIndex.GetValueOrDefault(1);

            var sqlcmd = @"SELECT  *,
									(SELECT COUNT(1)
									 FROM   Marketing..tbl_Comment WITH (NOLOCK)
									 WHERE  T.PKID = PKID
									) AS CommentCount
						FROM    (SELECT *,
										ROW_NUMBER() OVER (ORDER BY PublishDateTime DESC, PKID DESC) AS RowNumber
								 FROM   Marketing.dbo.tbl_Article WITH (NOLOCK)" + KeyStr + @"
								) AS T
						WHERE   T.RowNumber BETWEEN (@PageIndex - 1) * @PageSize + 1
											AND     @PageIndex * @PageSize
						ORDER BY T.RowNumber";
            var sqlParamters = new[]
            {
                new SqlParameter("@PageIndex",_PageIndex),
                new SqlParameter("@PageSize",_PageSize)
            };
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sqlcmd, sqlParamters).ConvertTo<Article>().ToList();
        }

        public static bool UpdateHotArticles(SqlConnection conn, int pkid, bool hotArticles)
        {
            string sql = @"  UPDATE [Marketing].[dbo].[tbl_Article] SET HotArticles=@HotArticles WHERE PKID=@PKID";
            var sqlPrams = new SqlParameter[]
            {
              new SqlParameter("@HotArticles",hotArticles),
              new SqlParameter("@PKID",pkid)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;

        }
        public static bool UpdateTheme(SqlConnection conn, int pkid, Article model)
        {
            string sql = @"UPDATE [Marketing].[dbo].[tbl_Article] SET SmallBanner=@SmallBanner  , ArticleBanner =@ArticleBanner, Bestla=@Bestla , LastUpdateDateTime=@LastUpdateDateTime WHERE  PKID=@PKID";
            var sqlPrams = new SqlParameter[]
            {
              new SqlParameter("@SmallBanner",model.SmallBanner??string.Empty),
                new SqlParameter("@ArticleBanner",model.ArticleBanner??string.Empty),
                 new SqlParameter("@LastUpdateDateTime",DateTime.Now),
                  new SqlParameter("@Bestla",model.Bestla),
              new SqlParameter("@PKID",pkid)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;

        }

        public static List<Article> GetSmallTilte(SqlConnection conn)
        {
            string sql = @"SELECT PKID,SmallTitle FROM   Marketing.dbo.tbl_Article WITH (NOLOCK) ORDER BY LastUpdateDateTime DESC";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<Article>().ToList();

        }

        public static string GetContentUrlById(SqlConnection conn, int id)
        {
            string sql = @"SELECT ContentUrl FROM   Marketing.dbo.tbl_Article WITH (NOLOCK) WHERE PKID=" + id;
            object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            return obj == null ? "" : obj.ToString();
        }

        public static List<SeekHotWord> GetSeekKeyWord(SqlConnection connection, string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @" SELECT  *
                        FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY id DESC ) AS ROWNUMBER ,
                                             [id]
                                  ,[HotWord]
                                  ,[CreateTime]
                                  ,[OnOff]
                              FROM [Marketing].[dbo].[tbl_HotWord]  WITH (NOLOCK) WHERE 1=1  " + sqlStr + @") AS PG
                        WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                             AND     STR(@PageIndex * @PageSize) ";
            string sqlCount = @"SELECT  COUNT(*)  FROM [Marketing].[dbo].[tbl_HotWord] WITH (NOLOCK) ";
            recordCount = (int)SqlHelper.ExecuteScalar(connection, CommandType.Text, sqlCount);

            var sqlPrams = new SqlParameter[]
            {              
                    new SqlParameter("@PageSize",pageSize),
                      new SqlParameter("@PageIndex",pageIndex),
            };

            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, sqlPrams).ConvertTo<SeekHotWord>().ToList();
        }

        public static bool DeleleSeekKeyWordById(SqlConnection conn, int id)
        {
            string sql = @"DELETE FROM  [Marketing].[dbo].[tbl_HotWord]  WHERE id = @id";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@id", id)) > 0;

        }

        public static bool InsertSeekKeyWord(SqlConnection conn, SeekHotWord model)
        {

            string sql = @"  INSERT INTO [Marketing].dbo.tbl_HotWord
                                      (  HotWord, CreateTime, OnOff
                                      )
                              VALUES  (
                                        @HotWord, GETDATE() ,@OnOff
                                      )";
            var sqlPrarms = new SqlParameter[]
            {
                new SqlParameter("@HotWord",model.HotWord),
                new SqlParameter("@OnOff",model.OnOff)
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrarms) > 0;

        }

        public static bool IsRepeatSeekKeyWord(SqlConnection conn, string keyword)
        {
            string sqlstr = @"SELECT COUNT(1) FROM [Marketing].[dbo].[tbl_HotWord] WITH (NOLOCK) WHERE [HotWord]=@HotWord";
            object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlstr, new SqlParameter("@HotWord", keyword));
            if ((int)obj > 0)
            {
                return true;
            }
            return false;
        }

        public static bool IsRepeatSeekKeyWord(SqlConnection conn, SeekHotWord model)
        {
            string sqlstr = @"SELECT COUNT(1) FROM [Marketing].[dbo].[tbl_HotWord] WITH (NOLOCK) WHERE [HotWord]=@HotWord AND id !=@id";

            var sqlPrams = new SqlParameter[]
            {
              new SqlParameter("@HotWord",model.HotWord),
              new SqlParameter("@id",model.id)

            };
            object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlstr, sqlPrams);
            if ((int)obj > 0)
            {
                return true;
            }
            return false;
        }

        public static bool UpdateSeekKeyWord(SqlConnection conn, SeekHotWord model, int id)
        {
            string sql = @"UPDATE  [Marketing].[dbo].[tbl_HotWord] SET [HotWord]=@keyWord , CreateTime= GETDATE(),OnOff=@OnOff  WHERE id=@id";

            var sqlPrams = new SqlParameter[]
            {
              new SqlParameter("@keyWord",model.HotWord),
              new SqlParameter("@OnOff",model.OnOff),
              new SqlParameter("@id",id)

            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;

        }

        public static SeekHotWord GetSeekHotWordById(SqlConnection conn, int id)
        {

            string sql = @"SELECT [id]
                                  ,[HotWord]
                                  ,[CreateTime]
                                  ,[OnOff]
                              FROM [Marketing].[dbo].[tbl_HotWord]  WITH (NOLOCK) WHERE  id=" + id;
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<SeekHotWord>().FirstOrDefault();
        }

        //文章类目表(tbl_NewCategoryList)相关操作（增，删，改，查）
        /// <summary>
        /// 查询（所有数据）
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<ArticleCategory> SelectCategory(SqlConnection connection)
        {
            string sql = @"select * from [Marketing].[dbo].[tbl_NewCategoryList] WITH (NOLOCK)";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<ArticleCategory>().ToList();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="connection"></param>
        public static bool AddCategory(SqlConnection connection, ArticleCategory categoryModel)
        {
            string sql = @"insert into [Marketing].[dbo].[tbl_NewCategoryList](CategoryName) values (@CategoryName)";
            var sqlPrarms = new SqlParameter[]
            {
                new SqlParameter("@CategoryName",categoryModel.CategoryName)
            };
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlPrarms) > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="categoryName">类目名称</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public static bool UpdateCategory(SqlConnection connection, string categoryName, int id)
        {
            string sql = @"update [Marketing].[dbo].[tbl_NewCategoryList] set CategoryName=@CategoryName where id=@id";
            var sqlPrams = new SqlParameter[]
            {
              new SqlParameter("@CategoryName",categoryName),
              new SqlParameter("@id",id)
            };
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlPrams) > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public static bool DeleteCategory(SqlConnection connection, int id)
        {
            string sql = @"delete from [Marketing].[dbo].[tbl_NewCategoryList] where id = @id";
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, new SqlParameter("@id", id)) > 0;
        }

        /// <summary>
        /// 修改/添加
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pwModel"></param>
        /// <returns></returns>
        public static bool EditCategory(SqlConnection connection, string sqlStr, SqlParameter[] sqlParams)
        {
            if (!string.IsNullOrEmpty(sqlStr) && sqlParams != null)
            {

                var result = SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sqlStr, sqlParams);
                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        //表Marketing..tbl_ArticleNewList相关操作（增，删，改，查）
        /// <summary>
        /// 查询（按条件查询）
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<ArticleNewList> SelectArticleNewList(SqlConnection connection, string sqlStr, SqlParameter[] sqlParams)
        {
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sqlStr, sqlParams).ConvertTo<ArticleNewList>().ToList();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="connection"></param>
        public static bool AddArticleNewList(SqlConnection connection, ArticleNewList articleModel)
        {
            string sql = @"insert into [Marketing].[dbo].[tbl_ArticleNewList]
                        (ArticleId,
                        ArticleUrl,
                        ProductId,
                        [Type],
                        CreateTime,
                        Field_1,
                        Field_2,
                        Field_3,
                        Field_4) 
                        values 
                        (@ArticleId,
                         @ArticleUrl,
                         @ProductId,
                         @Type,
                         @CreateTime,
                         @Field_1,
                         @Field_2,
                         @Field_3,
                         @Field_4)";
            var sqlPrarms = new SqlParameter[]
            {
                new SqlParameter("@ArticleId",articleModel.ArticleId),
                new SqlParameter("@ArticleUrl",articleModel.ArticleUrl),
                new SqlParameter("@ProductId",articleModel.ProductId),
                new SqlParameter("@Type",articleModel.Type),
                new SqlParameter("@CreateTime",articleModel.CreateTime),
                new SqlParameter("@Field_1",articleModel.Field_1),
                new SqlParameter("@Field_2",articleModel.Field_2),
                new SqlParameter("@Field_3",articleModel.Field_3),
                new SqlParameter("@Field_4",articleModel.Field_4)
            };
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlPrarms) > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="articleModel">对象</param>
        /// <returns></returns>
        public static bool UpdateArticleNewList(SqlConnection connection, ArticleNewList articleModel)
        {
            string sql = @"update [Marketing].[dbo].[tbl_ArticleNewList] set ArticleId =@ArticleId, 
                        ArticleUrl = @ArticleUrl,
                        ProductId = @ProductId,
                        [Type] = @Type,   
                        CreateTime = @CreateTime,
                        Field_1 = @Field_1,
                        Field_2 = @Field_2,
                        Field_3 = @Field_3,
                        Field_4 = @Field_4 where Id=@Id";
            var sqlPrams = new SqlParameter[]
            {
              new SqlParameter("@ArticleId",articleModel.ArticleId),
              new SqlParameter("@ArticleUrl",articleModel.ArticleUrl),
              new SqlParameter("@ProductId",articleModel.ProductId),
              new SqlParameter("@Type",articleModel.Type),
              new SqlParameter("@CreateTime",articleModel.CreateTime),
              new SqlParameter("@Field_1",articleModel.Field_1),
              new SqlParameter("@Field_2",articleModel.Field_2),
              new SqlParameter("@Field_3",articleModel.Field_3),
              new SqlParameter("@Field_4",articleModel.Field_4),
              new SqlParameter("@Id",articleModel.Id)
            };
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlPrams) > 0;
        }

        public static bool UpdateArticleNewList(SqlConnection connection, string sqlStr, SqlParameter[] sqlParams)
        {
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sqlStr, sqlParams) > 0;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="Id">Id</param>
        /// <returns></returns>
        public static bool DeleteArticleNewList(SqlConnection connection, int Id)
        {
            string sql = @"delete from [Marketing].[dbo].[tbl_ArticleNewList] where Id = @Id";
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, new SqlParameter("@Id", Id)) > 0;
        }

        public static int DeleteQuestion(SqlConnection conn,int PKID)
        {
            string sql = @"UPDATE  [Marketing].[dbo].[tbl_Article] SET [IsDelete]=1 WHERE PKID=@PKID";
            var sqlPrams = new SqlParameter[] { new SqlParameter("@PKID", PKID) };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams);
        }

        public static Article GetByUrl(SqlConnection connection, string url)
        {
            var dp = new DynamicParameters();
            dp.Add("@ContentUrl", url);
            return connection
                .Query<Article>(
                    "SELECT * FROM [Marketing].[dbo].[tbl_Article]  (NOLOCK) WHERE ContentUrl=@ContentUrl ",
                    dp)
                .FirstOrDefault();
            
        }
    }
}
