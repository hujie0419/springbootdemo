using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalPattern
    {
        #region 花纹测评
        public static DataTable SelectPatternsByBrand(string brand)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT DISTINCT
                                                   		VP.CP_Tire_Pattern AS Pattern
                                                   FROM	Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
                                                   WHERE	VP.PID LIKE 'TR-%'
                                                   		AND ISNULL(VP.CP_Tire_Pattern, '') <> ''
                                                   		AND VP.CP_Brand=@Brand
                                                   		AND VP.OnSale = 1
                                                   		AND VP.stockout = 0;", CommandType.Text, new SqlParameter("@Brand", brand));
            }
        }

        public static bool CanShow(TirePatternModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var OBJ = dbHelper.ExecuteScalar(@"SELECT	COUNT(1)
                                                   FROM	Tuhu_productcatalog.dbo.PatternArticle AS PA WITH ( NOLOCK )
                                                   WHERE	PA.Pattern = @Pattern
                                                   		AND PA.Brand = @Brand
                                                        AND PKID<>@PKID
                                                   		AND PA.IsShow = 1", CommandType.Text, new SqlParameter[] { new SqlParameter("@Brand", model.Brand), new SqlParameter("@Pattern", model.Pattern), new SqlParameter("@PKID", model.PKID) });
                return Convert.ToInt32(OBJ) > 0;
            }
        }

        public static string SelectPIDByPattern(string pattern)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var OBJ = dbHelper.ExecuteScalar(@"SELECT TOP 1
									VP.PID
							FROM	Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
							WHERE	VP.CP_Tire_Pattern = @Pattern", CommandType.Text, new SqlParameter[] {  new SqlParameter("@Pattern", pattern) });
                return OBJ == null || OBJ == DBNull.Value ? null : OBJ.ToString();
            }
        }

        public static IEnumerable<PatternArticleModel> SelectPatternForCache(string Pattern)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT	PKID,
                            		PA.Image,
                            		PA.Title,
                            		PA.Describe,
                            		PA.Author,
                            		PA.Date,
                            		PA.ArticleLink,
                            		PA.IsShow,
                            		PA.Brand,
                            		PA.Pattern,
                            		PA.CreateTime,
                            		PA.UpdateTime
                            FROM	Tuhu_productcatalog.dbo.PatternArticle AS PA WITH(NOLOCK)
                            WHERE	PA.IsActive = 1 AND PA.Pattern=@Pattern", CommandType.Text, new SqlParameter[] { new SqlParameter("@Pattern", Pattern) }).ConvertTo<PatternArticleModel>();
            }
        }
        public static TirePatternModel FetchByPKID(int pKID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  * FROM  Tuhu_productcatalog.dbo.[PatternArticle] AS [PA] WITH(NOLOCK) WHERE PA.PKID=@PKID", CommandType.Text, new SqlParameter("@PKID", pKID)).ConvertTo<TirePatternModel>().FirstOrDefault();
            }
        }

        public static int DeletePatternArticle(int pKID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"DELETE Tuhu_productcatalog.dbo.PatternArticle  WHERE PKID=@PKID", CommandType.Text, new SqlParameter("@PKID", pKID));
            }
        }

        public static int SaveUpdateOrAdd(TirePatternModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var result = -99;

                if (model.PKID > 0)
                    result = Update(dbHelper, model);
                else
                    result = Insert(dbHelper, model);
                return result;
            }
        }
        public static int SaveAddMany(TirePatternModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                dbHelper.BeginTransaction();
                var result = -99;
                foreach (var brand in model.Patterns)
                {
                    foreach (var pattern in brand.Patterns.Split(','))
                    {
                        TirePatternModel tempModel = new TirePatternModel()
                        {
                            Image = model.Image,
                            Title = model.Title,
                            Describe = model.Describe,
                            Author = model.Author,
                            Date = model.Date,
                            ArticleLink = model.ArticleLink,
                            IsShow = false,
                            IsActive = model.IsActive,
                            Brand = brand.Brand,
                            Pattern = pattern
                        };
                        result = Insert(dbHelper, tempModel);
                        if (result <= 0)
                            dbHelper.Rollback();
                    }
                }
                dbHelper.Commit();
                return result;

            }
        }

        public static int Insert(SqlDbHelper dbHelper, TirePatternModel model)
        {
            return dbHelper.ExecuteNonQuery(@"INSERT	INTO Tuhu_productcatalog.dbo.PatternArticle
					(
					  Image,
					  Title,
					  Describe,
					  Author,
					  Date,
					  ArticleLink,
					  IsShow,
					  Brand,
					  Pattern,
                      IsActive)
			VALUES	(
					  @Image,
					  @Title,
					  @Describe,
					  @Author,
					  @Date,
					  @ArticleLink,
					  @IsShow,
					  @Brand,
					  @Pattern,
                      @IsActive)", CommandType.Text, new SqlParameter[] {
                                                new SqlParameter("@Image",model.Image),
                                                new SqlParameter("@Title",model.Title),
                                                new SqlParameter("@Describe",model.Describe),
                                                new SqlParameter("@Author",model.Author),
                                                new SqlParameter("@Date",model.Date.Value),
                                                new SqlParameter("@ArticleLink",model.ArticleLink),
                                                new SqlParameter("@IsShow",model.IsShow??false),
                                                new SqlParameter("@Brand",model.Brand),
                                                new SqlParameter("@Pattern",model.Pattern),
                                                new SqlParameter("@IsActive",model.IsActive)
            });
        }
        public static int Update(SqlDbHelper dbHelper, TirePatternModel model)
        {
            return dbHelper.ExecuteNonQuery(@"UPDATE	Tuhu_productcatalog.dbo.PatternArticle
                                              SET Image = @Image,
                                              		Title = @Title,
                                              		Describe = @Describe,
                                              		Author = @Author,
                                              		Date = @Date,
                                              		ArticleLink = @ArticleLink,
                                              		IsShow = @IsShow,
                                                    IsActive=@IsActive,
                                              		Brand = @Brand,
                                              		Pattern = @Pattern,
                                              		UpdateTime = GETDATE()
                                              WHERE	PKID = @PKID;
                                              ", CommandType.Text, new SqlParameter[] {
                                                new SqlParameter("@Image",model.Image),
                                                new SqlParameter("@Title",model.Title),
                                                new SqlParameter("@Describe",model.Describe),
                                                new SqlParameter("@Author",model.Author),
                                                new SqlParameter("@Date",model.Date.Value),
                                                new SqlParameter("@ArticleLink",model.ArticleLink),
                                                new SqlParameter("@IsShow",model.IsShow??false),
                                                new SqlParameter("@Brand",model.Brand),
                                                new SqlParameter("@Pattern",model.Pattern),
                                                new SqlParameter("@PKID",model.PKID),
                                                new SqlParameter("@IsActive",model.IsActive)
            });
        }

        public static IEnumerable<TirePatternModel> SelectList(TirePatternModel model, PagerModel pager)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                pager.TotalItem = GetListCount(dbHelper, model);
                return dbHelper.ExecuteDataTable(@"SELECT	T.Brand,
                                                       		T.Pattern,
                                                       		PA.Image,
                                                       		PA.Title,
                                                       		PA.Describe,
                                                       		PA.Author,
                                                       		PA.Date,
                                                       		PA.ArticleLink,
                                                       		PA.IsShow,
                                                            PA.IsActive,
                                                            PA.PKID
                                                       FROM	( SELECT DISTINCT
                                                       					VP.CP_Brand AS Brand,
                                                       					VP.CP_Tire_Pattern AS Pattern
                                                       		  FROM		Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
                                                       		  WHERE		VP.PID LIKE 'TR-%'
                                                       					AND ISNULL(VP.CP_Brand, '') <> ''
                                                       					AND ISNULL(VP.CP_Tire_Pattern, '') <> ''
                                                   					    AND VP.OnSale=1
							                                            AND VP.stockout=0
                                                       		) AS T
                                                       LEFT  JOIN Tuhu_productcatalog.dbo.PatternArticle AS PA WITH ( NOLOCK )
                                                       		ON PA.Brand = T.Brand
                                                       		   AND PA.Pattern = T.Pattern
                                                       WHERE	(
                                                       		  T.Brand = @Brand
                                                       		  OR @Brand IS NULL )
                                                       		AND (
                                                       			  T.Pattern = @Pattern
                                                       			  OR @Pattern IS NULL )
                                                       		AND (
                                                       			  PA.Title = @Title
                                                       			  OR @Title IS NULL )
                                                       		AND (
                                                       			  PA.Author = @Author
                                                       			  OR @Author IS NULL )
                                                       		AND (
                                                       			  @Count = 0
                                                       			  OR @Count = 1
                                                       			  AND PA.PKID > 0
                                                       			  OR @Count = 2
                                                       			  AND PA.PKID IS NULL )
                                                       		AND (
                                                       			  @ShowCount = 0
                                                       			  OR @ShowCount = 1
                                                       			  AND T.Pattern IN ( SELECT	PAA.Pattern
                                                       								 FROM	Tuhu_productcatalog.dbo.PatternArticle AS PAA WITH ( NOLOCK )
                                                       								 WHERE	PAA.IsShow = 1 )
                                                       			  OR @ShowCount = 2
                                                       			  AND T.Pattern NOT  IN ( SELECT	PAA.Pattern
                                                       									  FROM		Tuhu_productcatalog.dbo.PatternArticle AS PAA WITH ( NOLOCK )
                                                       									  WHERE		PAA.IsShow = 1 )
                                                       			  AND PA.PKID > 0 )
                                                       ORDER BY T.Pattern
                                                       		OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
                                                                                                          FETCH NEXT @PageSize ROWS ONLY;", CommandType.Text,
                                                                                           new SqlParameter[] {
                                                                                               new SqlParameter("@Brand", String.IsNullOrWhiteSpace(model.Brand)?null:model.Brand),
                                                                                               new SqlParameter("@Pattern", String.IsNullOrWhiteSpace(model.Pattern)?null:model.Pattern),
                                                                                               new SqlParameter("@Title", String.IsNullOrWhiteSpace(model.Title)?null:model.Title),
                                                                                               new SqlParameter("@Author", String.IsNullOrWhiteSpace(model.Author)?null:model.Author),
                                                                                               new SqlParameter("@Count", model.Count),
                                                                                               new SqlParameter("@ShowCount", model.ShowCount),
                                                                                               new SqlParameter("@PageIndex", pager.CurrentPage),
                                                                                               new SqlParameter("@PageSize", pager.PageSize),
                                                                                           }).ConvertTo<TirePatternModel>();
            }
        }

        public static int GetListCount(SqlDbHelper dbHelper, TirePatternModel model)
        {

            var OBJ = dbHelper.ExecuteScalar(@"SELECT	COUNT(1)
                                                FROM	( SELECT DISTINCT
                                                					VP.CP_Brand AS Brand,
                                                					VP.CP_Tire_Pattern AS Pattern
                                                		  FROM		Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
                                                		  WHERE		VP.PID LIKE 'TR-%'
                                                					AND ISNULL(VP.CP_Brand, '') <> ''
                                                					AND ISNULL(VP.CP_Tire_Pattern, '') <> ''
                                                                    AND VP.OnSale=1
							                                        AND VP.stockout=0
                                                		) AS T
                                                LEFT  JOIN Tuhu_productcatalog.dbo.PatternArticle AS PA WITH ( NOLOCK )
                                                		ON PA.Brand = T.Brand
                                                		   AND PA.Pattern = T.Pattern
                                                WHERE	(
                                                		  T.Brand = @Brand
                                                		  OR @Brand IS NULL )
                                                		AND (
                                                			  T.Pattern = @Pattern
                                                			  OR @Pattern IS NULL )
                                                		AND (
                                                			  PA.Title = @Title
                                                			  OR @Title IS NULL )
                                                		AND (
                                                			  PA.Author = @Author
                                                			  OR @Author IS NULL )
                                                		AND (
                                                			  @Count = 0
                                                			  OR @Count = 1
                                                			  AND PA.PKID > 0
                                                			  OR @Count = 2
                                                			  AND PA.PKID IS NULL )
                                                		AND (
                                                			  @ShowCount = 0
                                                			  OR @ShowCount = 1
                                                			  AND T.Pattern IN ( SELECT	PAA.Pattern
                                                								 FROM	Tuhu_productcatalog.dbo.PatternArticle AS PAA WITH ( NOLOCK )
                                                								 WHERE	PAA.IsShow = 1 )
                                                			  OR @ShowCount = 2
                                                			  AND T.Pattern NOT  IN ( SELECT	PAA.Pattern
                                                									  FROM		Tuhu_productcatalog.dbo.PatternArticle AS PAA WITH ( NOLOCK )
                                                									  WHERE		PAA.IsShow = 1 )
                                                			  AND PA.PKID > 0 )",
                                                       CommandType.Text,
                                                       new SqlParameter[] {
                                                               new SqlParameter("@Brand", String.IsNullOrWhiteSpace(model.Brand)?null:model.Brand),
                                                               new SqlParameter("@Pattern", String.IsNullOrWhiteSpace(model.Pattern)?null:model.Pattern),
                                                               new SqlParameter("@Title", String.IsNullOrWhiteSpace(model.Title)?null:model.Title),
                                                               new SqlParameter("@Author", String.IsNullOrWhiteSpace(model.Author)?null:model.Author),
                                                               new SqlParameter("@Count", model.Count),
                                                               new SqlParameter("@ShowCount", model.ShowCount),
                                                       });
            return Convert.ToInt32(OBJ);
        }


        #endregion

        #region 天降神券黑名单管理
        public static IEnumerable<CouponBlackItem> GetCouponBlackList(PagerModel pager,string phoneNum)
        {
            const string sql = @"SELECT  PhoneNum ,
        MAX(CreateDateTime) AS CreateDateTime
FROM    Configuration..CouponBlackList WITH ( NOLOCK )
WHERE   @phonenum IS NULL
        OR PhoneNum = @phonenum
GROUP BY PhoneNum
ORDER BY CreateDateTime DESC
        OFFSET @begin ROWS FETCH NEXT @step ROWS ONLY;";
            pager.TotalItem = CouponBlackList(phoneNum);
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@phonenum",string.IsNullOrWhiteSpace(phoneNum)?null:phoneNum),
                    new SqlParameter("@begin",(pager.CurrentPage-1)*pager.PageSize),
                    new SqlParameter("@step",pager.PageSize)
                }).ConvertTo<CouponBlackItem>();
            }
        }


        private static int CouponBlackList(string phoneNum)
        {
            const string sql = @"SELECT  COUNT(1)
FROM    ( SELECT DISTINCT
                    PhoneNum
          FROM      Configuration..CouponBlackList WITH ( NOLOCK )
          WHERE   @phonenum IS NULL
                OR PhoneNum = @phonenum
        ) AS T;";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return Convert.ToInt32(dbHelper.ExecuteScalar(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@phonenum",string.IsNullOrWhiteSpace(phoneNum)?null:phoneNum)
                }));
            }
        }
        public static int AddCouponBlackList(string phoneNums)
        {
            Regex reg = new Regex(@"^[0-9]{11}$");
            var phonelist = phoneNums.Split(',').Where(g => reg.IsMatch(g)).Distinct();
            if (phonelist.Any())
            {
                string sql = @"INSERT INTO Configuration..CouponBlackList(PhoneNum)
VALUES('" + string.Join("'),('", phonelist) + "')";
                using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
                {
                    return dbHelper.ExecuteNonQuery(sql, CommandType.Text);
                }
            }
            return 0;
        }
        
        public static int DeleteCouponBlackList(string phoneNum)
        {
            string sql = @"DELETE FROM Configuration..CouponBlackList WHERE PhoneNum = @phonenum;";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@phonenum",phoneNum)
                });
            }
        }
		#endregion

	    public static IEnumerable<TireBlackListItem> GetTireBlackList(string blackNumber, int type, PagerModel pager)
	    {
			string sqlStr = @"SELECT  BlackNumber ,
        Type ,
        CreateDateTime ,
        EndTime
FROM    Configuration..CommonBlackList WITH ( NOLOCK )
WHERE   ( @type = 0
          OR @type = Type
        )
        AND IIF(@blackNumber IS NULL, BlackNumber, @blackNumber) = BlackNumber
ORDER BY CreateDateTime DESC
        OFFSET @begin ROWS FETCH NEXT @step ROWS ONLY;";
		    pager.TotalItem = GetTireBlackListCount(blackNumber, type);

			using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
			{
				return dbHelper.ExecuteDataTable(sqlStr, CommandType.Text, new SqlParameter[]
				{
					new SqlParameter("@type", type),
					new SqlParameter("@blackNumber", string.IsNullOrWhiteSpace(blackNumber) ? null : blackNumber),
					new SqlParameter("@begin", pager.PageSize * (pager.CurrentPage - 1)),
					new SqlParameter("@step", pager.PageSize)
				}).ConvertTo<TireBlackListItem>();
			}

		}

	    private static int GetTireBlackListCount(string blackNumber, int type)
	    {
		    string sqlStr = @"SELECT  COUNT(1)
FROM    Configuration..CommonBlackList WITH ( NOLOCK )
WHERE   ( @type = 0
          OR @type = Type
        )
        AND IIF(@blackNumber IS NULL, BlackNumber, @blackNumber) = BlackNumber;";


			using (var cmd = new SqlCommand(sqlStr))
			{
				cmd.Parameters.AddWithValue("@type", type);
				cmd.Parameters.AddWithValue("@blackNumber", string.IsNullOrWhiteSpace(blackNumber) ? null : blackNumber);
				cmd.CommandType = CommandType.Text;
				return Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
			}

		}

		public static int DeleteTireBlackListItem(string BlackNumber,int Type)
		{
			string sqlStr = @"DELETE FROM Configuration..CommonBlackList WHERE BlackNumber=@blackNumber AND Type=@type";
			using(var cmd=new SqlCommand(sqlStr))
			{
                cmd.Parameters.AddWithValue("@type", Type);
                cmd.Parameters.AddWithValue("@blackNumber", BlackNumber);
				return DbHelper.ExecuteNonQuery(cmd);
			}
		}

        public static bool CheckTireBlackListItem(string BlackNumber, int Type)
        {
            string sqlStr = @"SELECT COUNT(1) FROM Configuration..CommonBlackList WITH(NOLOCK) WHERE BlackNumber=@blacknumber AND Type = @type";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@blacknumber", BlackNumber);
                cmd.Parameters.AddWithValue("@type", Type);
                var dat = DbHelper.ExecuteScalar(cmd);
                var result = 0;
                if (Int32.TryParse(dat?.ToString(), out result))
                {
                    return result > 0;
                }
                return false;
            }
        }

        public static bool AddTireBlackListItem(string BlackNumber, int Type)
        {
            string sqlStr = @"INSERT INTO Configuration..CommonBlackList(BlackNumber, Type) VALUES (@blacknumber, @type)";
            var value = BlackNumber;
            if (Type == 2)
            {
                value = new Guid(BlackNumber).ToString("D");
            }
            using(var cmd=new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@blackNumber", value);
                cmd.Parameters.AddWithValue("@type", Type);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool AddTireBlackListLog(string BlackNumber, string Operator, int Type)
        {
            string sqlStr = @"INSERT INTO Tuhu_log..TireBlackListLog(BlackNumber, Operator, OperationType)
VALUES  (@blacknumber, @operator, @type)";
            using(var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@blacknumber", BlackNumber);
                    cmd.Parameters.AddWithValue("@operator", Operator);
                    cmd.Parameters.AddWithValue("@type", Type);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }            
        }
        public static IEnumerable<TireBlackListLog> BlackListItemHistory(string BlackNumber)
        {
            string sqlStr = @"SELECT BlackNumber,OperationType AS Type,Operator,CreateDateTime FROM Tuhu_log..TireBlackListLog WITH(NOLOCK) WHERE BlackNumber=@blacknumber ORDER BY CreateDateTime DESC;";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@blacknumber", BlackNumber);
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<TireBlackListLog>();
                }
            }
        }
        public static IEnumerable<TireBlackListLog> SelectTireBlackListLog(string blackNumber, PagerModel pager)
        {
            string sqlStr = @"SELECT  BlackNumber ,
        OperationType AS Type ,
        Operator ,
        CreateDateTime
FROM    Tuhu_log..TireBlackListLog WITH ( NOLOCK )
WHERE   IIF(@blackNumber IS NULL, BlackNumber, @blackNumber) = BlackNumber
ORDER BY BlackNumber ,
        CreateDateTime DESC
        OFFSET @begin ROWS FETCH NEXT @step ROWS ONLY;";
            pager.TotalItem = SelectTireBlackListLogCount(blackNumber);
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@blackNumber", string.IsNullOrWhiteSpace(blackNumber) ? null : blackNumber);
                    cmd.Parameters.AddWithValue("@begin", pager.PageSize * (pager.CurrentPage - 1));
                    cmd.Parameters.AddWithValue("@step", pager.PageSize);
                    cmd.CommandType = CommandType.Text;
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<TireBlackListLog>();
                }
            }
        }
        private static int SelectTireBlackListLogCount(string blackNumber)
        {
            string sqlStr = @"SELECT COUNT(1)
FROM    Tuhu_log..TireBlackListLog WITH ( NOLOCK )
WHERE   IIF(@blackNumber IS NULL, BlackNumber, @blackNumber) = BlackNumber;";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@blackNumber", string.IsNullOrWhiteSpace(blackNumber) ? null : blackNumber);
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(dbHelper.ExecuteScalar(cmd));
                }
            }
        }
    }
}
