using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Service.Activity.Models.Questionnaire;

namespace Tuhu.Service.Activity.DataAccess.Questionnaire
{
    /// <summary>
    ///     用户回答结果表
    /// </summary>
    public static class DalQuestionnaireAnswerResult
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalQuestionnaireAnswerResult));

        /// <summary>
        ///     提交问卷 回答结果
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<long> SubmitQuestionnaireAnswerResult(BaseDbHelper helper, QuestionnaireAnswerResultModel model)
        {
            var sql = @"
                            INSERT INTO [Activity].[dbo].[QuestionnaireAnswerResult]
                                                       ([QuestionnaireAnswerID]
                                                       ,[AnswerResultStatus]
                                                       ,[UseIntegral]
                                                       ,[WinCouponCount]
                                                       ,[CreateDatetime]
                                                       ,[LastUpdateDateTime])
                                                     VALUES
                                                           (@QuestionnaireAnswerID
                                                           ,@AnswerResultStatus
                                                           ,@UseIntegral
                                                           ,@WinCouponCount
                                                           ,getdate()
                                                           ,getdate());
                                                   SELECT SCOPE_IDENTITY();
                                                    ";

            try
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.AddParameter("@QuestionnaireAnswerID", model.QuestionnaireAnswerID);
                    cmd.AddParameter("@AnswerResultStatus", model.AnswerResultStatus);
                    cmd.AddParameter("@UseIntegral", model.UseIntegral);
                    cmd.AddParameter("@WinCouponCount", model.WinCouponCount);

                    var result = await helper.ExecuteScalarAsync(cmd);
                    return Convert.ToInt64(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>{nameof(SubmitQuestionnaireAnswerResult)} =>{ex}");
                throw ex;
            }

        }


        /// <summary>
        ///     获取用户的回答结果 
        /// </summary>
        /// <param name="readOnly">是否查询读库</param>
        /// <param name="questionnaireAnswerRecordId">外键：QuestionnaireAnswerRecord pkid </param>
        /// <returns></returns>
        public static async Task<QuestionnaireAnswerResultModel> GetQuestionnaireAnswerResult(bool readOnly, long questionnaireAnswerRecordId)
        {
            var sql = @" SELECT  top  1  [PKID]
                                  ,[QuestionnaireAnswerID]
                                  ,[AnswerResultStatus]
                                  ,[UseIntegral]
                                  ,[WinCouponCount]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                          FROM [Activity].[dbo].[QuestionnaireAnswerResult] with (nolock)
                          where     QuestionnaireAnswerID=@QuestionnaireAnswerID
                           ";
            try
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.AddParameter("@QuestionnaireAnswerID", questionnaireAnswerRecordId);

                    var result = await DbHelper.ExecuteFetchAsync<QuestionnaireAnswerResultModel>(readOnly, cmd);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>{nameof(GetQuestionnaireAnswerResult)}=> {questionnaireAnswerRecordId} =>{ex}");
                throw ex;
            }

        }


        /// <summary>
        ///     获取用户赢得的场次
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="questionnaireID"></param>
        /// <returns></returns>
        public static async Task<int> GetQuestionnaireAnswerUserWinCount(Guid userId, long questionnaireID)
        {
            var sql = @" SELECT  count(*)
                          FROM [Activity].[dbo].[QuestionnaireAnswerResult] as a with (nolock)
                          Inner join [Activity].[dbo].[QuestionnaireAnswerRecord] as b with (nolock)
                          on a.QuestionnaireAnswerID = b.PKID
                          where     b.UserID = @UserID and QuestionnaireID = @QuestionnaireID and b.IsDeleted = 0 and AnswerResultStatus = 1 
                           ";
            try
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.AddParameter("@UserID", userId);
                    cmd.AddParameter("@QuestionnaireID", questionnaireID);

                    var result = await DbHelper.ExecuteScalarAsync(true,cmd);
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>{nameof(GetQuestionnaireAnswerUserWinCount)}=> {userId:D} {questionnaireID} =>{ex}");
                throw ex;
            }

        }


        /// <summary>
        ///     设置用户答案结果
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="answerId"></param>
        /// <param name="couponCount"></param>
        /// <param name="flag"> 1 胜利  2 失败</param>
        /// <returns></returns>
        public static async Task<bool> UpdateUserAnswerResult(BaseDbHelper helper, long answerId, int couponCount, int flag)
        {
            var sql = @"
                                   update Activity.dbo.QuestionnaireAnswerResult
                                   set AnswerResultStatus = @AnswerResultStatus,WinCouponCount = @WinCouponCount,LastUpdateDateTime=getdate()
                                   where PKID = @PKID
                            ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@AnswerResultStatus", flag);
                cmd.Parameters.AddWithValue("@WinCouponCount", couponCount);
                cmd.Parameters.AddWithValue("@PKID", answerId);
                var result = await helper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }
    }
}
