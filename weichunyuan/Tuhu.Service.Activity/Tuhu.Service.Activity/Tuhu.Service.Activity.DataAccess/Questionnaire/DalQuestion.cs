using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.Models.Questionnaire;

namespace Tuhu.Service.Activity.DataAccess.Questionnaire
{
    public class DalQuestion
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalQuestion));
        /// <summary>
        /// 根据问卷标识获取问题列表
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<QuestionModel>> GetQuestionList(long questionnaireId)
        {
            #region SQL

            string sql = @" SELECT [PKID] AS QuestionID
                                  ,[QuestionnaireID]
                                  ,[QuestionTitle]
                                  ,[QuestionType]
                                  ,[IsFork]
                                  ,[DefaultValue]
                                  ,[ScoreStyle]
                                  ,[MinScore]
                                  ,[MaxScore]
                                  ,[IsRequired]
                                  ,[IsValidateMinChar]
                                  ,[MinChar]
                                  ,[IsValidateMaxChar]
                                  ,[MaxChar]
                                  ,[IsValidateStartDate]
                                  ,[StartTime]
                                  ,[IsValidateEndDate]
                                  ,[EndTime]
                                  ,[IsValidatePhone]
                                  ,[Sort]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                                  ,[QuestionTextResult]
                                  ,[QuestionConfirm]
                                  ,[DeadLineTime]
                              FROM [Activity].[dbo].[Question] WITH ( NOLOCK )
                              WHERE IsDeleted=0 AND QuestionnaireID=@QuestionnaireID 
                              ORDER BY Sort ASC ";
            #endregion
            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = new SqlParameter[] { new SqlParameter("@QuestionnaireID", questionnaireId) };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteSelectAsync<QuestionModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetQuestionList=>{ex.ToString()}");
                return null;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }


        /// <summary>
        /// 根据问卷标识和时间范围获取问题列表
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<QuestionModel>> GetQuestionList(long questionnaireId, DateTime time)
        {
            #region SQL

            string sql = @" SELECT [PKID] AS QuestionID
                                  ,[QuestionnaireID]
                                  ,[QuestionTitle]
                                  ,[QuestionType]
                                  ,[IsFork]
                                  ,[DefaultValue]
                                  ,[ScoreStyle]
                                  ,[MinScore]
                                  ,[MaxScore]
                                  ,[IsRequired]
                                  ,[IsValidateMinChar]
                                  ,[MinChar]
                                  ,[IsValidateMaxChar]
                                  ,[MaxChar]
                                  ,[IsValidateStartDate]
                                  ,[StartTime]
                                  ,[IsValidateEndDate]
                                  ,[EndTime]
                                  ,[IsValidatePhone]
                                  ,[Sort]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                                  ,[QuestionTextResult]
                                  ,[QuestionConfirm]
                                  ,[DeadLineTime]
                              FROM [Activity].[dbo].[Question] WITH ( NOLOCK )
                              WHERE IsDeleted=0 AND QuestionnaireID=@QuestionnaireID  and StartTime< @time and EndTime > @time 
                              ORDER BY Sort ASC ";
            #endregion
            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = { new SqlParameter("@QuestionnaireID", questionnaireId), new SqlParameter("@time", time) };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteSelectAsync<QuestionModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetQuestionList=>{ex.ToString()}");
                return null;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }

        /// <summary>
        ///     获取正在进行中和结束的题目
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <param name="time">日期范围</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PagedModel<QuestionModel>> GetQuestionListPage(long questionnaireId
            , DateTime time
            , int pageIndex = 1
            , int pageSize = 20)
        {
            #region SQL

            var where = @"              IsDeleted=0
                                    AND QuestionnaireID=@QuestionnaireID
                                    and StartTime<@time
                                    ";

            string sql = @" SELECT [PKID] AS QuestionID
                                  ,[QuestionnaireID]
                                  ,[QuestionTitle]
                                  ,[QuestionType]
                                  ,[IsFork]
                                  ,[DefaultValue]
                                  ,[ScoreStyle]
                                  ,[MinScore]
                                  ,[MaxScore]
                                  ,[IsRequired]
                                  ,[IsValidateMinChar]
                                  ,[MinChar]
                                  ,[IsValidateMaxChar]
                                  ,[MaxChar]
                                  ,[IsValidateStartDate]
                                  ,[StartTime]
                                  ,[IsValidateEndDate]
                                  ,[EndTime]
                                  ,[IsValidatePhone]
                                  ,[Sort]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                                  ,[QuestionTextResult]
                                  ,[QuestionConfirm]
                                  ,[DeadLineTime]
                              FROM [Activity].[dbo].[Question] WITH ( NOLOCK )
                              WHERE " + where + @"
                              ORDER BY EndTime desc
                              OFFSET (@pageIndex -1) * @pageSize ROWS
                              FETCH NEXT @pageSize ROWS ONLY
                                                    ";

            var sqlCount = @" SELECT  count(*)
                              FROM [Activity].[dbo].[Question] WITH ( NOLOCK )
                              WHERE " + where;
            #endregion
            SqlCommand sqlCmd = null;
            var result = new PagedModel<QuestionModel>()
            {
                Pager = new PagerModel()
                {
                    PageSize = pageSize,
                    CurrentPage = pageIndex
                }
            };
            try
            {
                using (sqlCmd = new SqlCommand(sql))
                {
                    sqlCmd.AddParameter("@QuestionnaireID", questionnaireId);
                    sqlCmd.AddParameter("@time", time);
                    sqlCmd.AddParameter("@pageIndex", pageIndex);
                    sqlCmd.AddParameter("@pageSize", pageSize);

                    result.Source = await DbHelper.ExecuteSelectAsync<QuestionModel>(true,sqlCmd);
                }
                using (var cmd = new SqlCommand(sqlCount))
                {
                    cmd.AddParameter("@QuestionnaireID", questionnaireId);
                    cmd.AddParameter("@time", time);

                    var total = Convert.ToInt32(await DbHelper.ExecuteScalarAsync(cmd));
                    result.Pager.Total = total;
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetQuestionListPage=>{ex.ToString()}");
                return null;
            }
            finally
            {
                sqlCmd?.Dispose();
            }
        }


        /// <summary>
        ///     获取正在进行中和结束的题目
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <param name="userId">用户ID</param>
        /// <param name="showFlag"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PagedModel<QuestionModel>> GetQuestionListPage(long questionnaireId
            , Guid userId
            , int showFlag
            , int pageIndex = 1
            , int pageSize = 20)
        {
            #region SQL

            var where = "     a.IsDeleted=0  ";

            if (showFlag == 1)
            {
                where = where + @"           
                                  AND  exists (
	SELECT * FROM [Activity].[dbo].QuestionnaireAnswerRecord c WITH ( NOLOCK )
    INNER JOIN [Activity].[dbo].QuestionnaireAnswerResult d WITH ( NOLOCK )
    on  c.UserID = @userid and  c.PKID = d.QuestionnaireAnswerID and d.AnswerResultStatus <> 0 AND c.QuestionnaireID = @QuestionnaireID 
    WHERE  a.StartTime < c.AnswerDate AND a.EndTime > c.AnswerDate 
) 
                                    ";
            }
            else
            {
                where = where + @"     
                                  AND  exists (
	SELECT * FROM [Activity].[dbo].QuestionnaireAnswerRecord c WITH ( NOLOCK )
    INNER JOIN [Activity].[dbo].QuestionnaireAnswerResult d WITH ( NOLOCK )
    on  c.UserID = @userid and  c.PKID = d.QuestionnaireAnswerID  AND c.QuestionnaireID = @QuestionnaireID 
    WHERE  a.StartTime < c.AnswerDate AND a.EndTime > c.AnswerDate 
) 
                                    ";
            }

            string sql = @" SELECT a.[PKID] AS QuestionID
                                  ,a.[QuestionnaireID]
                                  ,a.[QuestionTitle]
                                  ,a.[QuestionType]
                                  ,a.[IsFork]
                                  ,a.[DefaultValue]
                                  ,a.[ScoreStyle]
                                  ,a.[MinScore]
                                  ,a.[MaxScore]
                                  ,a.[IsRequired]
                                  ,a.[IsValidateMinChar]
                                  ,a.[MinChar]
                                  ,a.[IsValidateMaxChar]
                                  ,a.[MaxChar]
                                  ,a.[IsValidateStartDate]
                                  ,a.[StartTime]
                                  ,a.[IsValidateEndDate]
                                  ,a.[EndTime]
                                  ,a.[IsValidatePhone]
                                  ,a.[Sort]
                                  ,a.[CreateDateTime]
                                  ,a.[LastUpdateDateTime]
                                  ,a.[IsDeleted]
                                  ,a.[QuestionTextResult]
                                  ,a.[QuestionConfirm]
                                  ,a.[DeadLineTime]
                              FROM [Activity].[dbo].[Question] a WITH ( NOLOCK )
                              FULL JOIN [Activity].[dbo].QuestionnaireAnswerRecord b WITH ( NOLOCK )
                              ON b.UserID = @userid  AND b.QuestionnaireID = @QuestionnaireID  AND a.PKID =b.QuestionID    
                              WHERE " + where + @"
                              ORDER BY a.EndTime desc
                              OFFSET (@pageIndex -1) * @pageSize ROWS
                              FETCH NEXT @pageSize ROWS ONLY
                                                    ";

            var sqlCount = @" SELECT  count(*)
                              FROM [Activity].[dbo].[Question] a WITH ( NOLOCK )
                              FULL JOIN [Activity].[dbo].QuestionnaireAnswerRecord b WITH ( NOLOCK )
                              ON b.UserID = @userid  AND b.QuestionnaireID = @QuestionnaireID  AND a.PKID =b.QuestionID
                              WHERE " + where;
            #endregion
            SqlCommand sqlCmd = null;
            var result = new PagedModel<QuestionModel>()
            {
                Pager = new PagerModel()
                {
                    PageSize = pageSize,
                    CurrentPage = pageIndex
                }
            };
            try
            {
                using (sqlCmd = new SqlCommand(sql))
                {
                    sqlCmd.AddParameter("@QuestionnaireID", questionnaireId);
                    sqlCmd.AddParameter("@userid", userId);

                    sqlCmd.AddParameter("@pageIndex", pageIndex);
                    sqlCmd.AddParameter("@pageSize", pageSize);

                    result.Source = await DbHelper.ExecuteSelectAsync<QuestionModel>(true, sqlCmd);
                }
                using (var cmd = new SqlCommand(sqlCount))
                {
                    cmd.AddParameter("@QuestionnaireID", questionnaireId);
                    cmd.AddParameter("@userid", userId);


                    var total = Convert.ToInt32(await DbHelper.ExecuteScalarAsync(true,cmd));
                    result.Pager.Total = total;
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetQuestionListPage=>{ex.ToString()}");
                return null;
            }
            finally
            {
                sqlCmd?.Dispose();
            }
        }

        /// <summary>
        ///     通过主键获取问题对象
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<QuestionModel> GetQuestion(long pkid)
        {
            #region SQL
            string sql = @" SELECT     [PKID] as [QuestionID]
                                      ,[QuestionnaireID]
                                      ,[QuestionTitle]
                                      ,[QuestionType]
                                      ,[IsFork]
                                      ,[DefaultValue]
                                      ,[ScoreStyle]
                                      ,[MinScore]
                                      ,[MaxScore]
                                      ,[IsRequired]
                                      ,[IsValidateMinChar]
                                      ,[MinChar]
                                      ,[IsValidateMaxChar]
                                      ,[MaxChar]
                                      ,[IsValidateStartDate]
                                      ,[StartTime]
                                      ,[IsValidateEndDate]
                                      ,[EndTime]
                                      ,[IsValidatePhone]
                                      ,[Sort]
                                      ,[CreateDateTime]
                                      ,[LastUpdateDateTime]
                                      ,[IsDeleted]
                                      ,[QuestionTextResult]
                                      ,[QuestionConfirm]
                                      ,[DeadLineTime]
                            FROM    [Activity].[dbo].[Question] WITH ( NOLOCK )
                            WHERE   IsDeleted = 0
                                    AND pkid = @pkid ";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = { new SqlParameter("@pkid", pkid) };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return (await dbHelper.ExecuteSelectAsync<QuestionModel>(sqlCmd)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>{nameof(GetQuestion)}=>{ex.ToString()}");
                return null;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }
    }
}
