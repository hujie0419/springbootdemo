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
    public class DalQuestionnaireAnswerRecord
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalQuestionnaireAnswerRecord));
        /// <summary>
        /// 根据问卷编号获取问卷信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public static async Task<QuestionnaireAnswerRecordModel> GetQuestionnaireAnswerRecordInfo(Guid userId, int objectId)
        {
            #region SQL

            string sql = @"SELECT  TOP 1
                                    [PKID] ,
                                    [UserID] ,
                                    [QuestionnaireID] ,
                                    [QuestionnaireName] ,
                                    [QuestionID] ,
                                    [QuestionName] ,
                                    [QuestionType] ,
                                    [AnswerText] ,
                                    [AnswerOptionID] ,
                                    [AnswerOptionContent] ,
                                    [AnswerDate] ,
                                    [QuestionScore] ,
                                    [ObjectID] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime] ,
                                    [IsDeleted]
                            FROM    [Activity].[dbo].[QuestionnaireAnswerRecord] WITH(NOLOCK)
                            WHERE   IsDeleted = 0
                                    AND UserID = @UserID
                                    AND ObjectID = @ObjectID;";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = new SqlParameter[] {
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@ObjectID",objectId)
                };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteFetchAsync<QuestionnaireAnswerRecordModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetQuestionnaireAnswerRecordInfo=>{ex.ToString()}");
                return null;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }
        /// <summary>
        /// 批量提交问卷
        /// </summary>
        /// <param name="answerRecordList"></param>
        public static async Task SubmitQuestionnaire(List<QuestionnaireAnswerRecordModel> answerRecordList)
        {
            #region [构建表数据]
            var dt = new DataTable();
            dt.Columns.AddRange(new[]
            {
                new DataColumn("UserID", typeof(Guid)),
                new DataColumn("QuestionnaireID", typeof(long)),
                new DataColumn("QuestionnaireName", typeof(string)),
                new DataColumn("QuestionID", typeof(long)),
                new DataColumn("QuestionName", typeof(string)),
                new DataColumn("QuestionType", typeof(int)),
                new DataColumn("AnswerText", typeof(string)),
                new DataColumn("AnswerOptionID", typeof(long)),
                new DataColumn("AnswerOptionContent", typeof(string)),
                new DataColumn("ObjectID", typeof(int)),
            });

            foreach (var item in answerRecordList)
            {
                var r = dt.NewRow();
                r["UserID"] = item.UserID;
                r["QuestionnaireID"] = item.QuestionnaireID;
                r["QuestionnaireName"] = item.QuestionnaireName;
                r["QuestionID"] = item.QuestionID;
                r["QuestionName"] = item.QuestionName;
                r["QuestionType"] = item.QuestionType;
                r["AnswerText"] = item.AnswerText;
                r["AnswerOptionID"] = item.AnswerOptionID;
                r["AnswerOptionContent"] = item.AnswerOptionContent;
                r["ObjectID"] = item.ObjectID;
                dt.Rows.Add(r);
            }

            #endregion

            try
            {
                using (var db = DbHelper.CreateDbHelper())
                {
                    db.BeginTransaction();
                    dt.TableName = "[Activity].[dbo].[QuestionnaireAnswerRecord]";
                    await db.BulkCopyAsync(dt);
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>SubmitQuestionnaire=>{ex.ToString()}");
            }
        }

        /// <summary>
        ///     提交问卷 回答
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<long> SubmitQuestionnaire(BaseDbHelper helper, QuestionnaireAnswerRecordModel model)
        {
            var sql = @"
                            INSERT INTO [Activity].[dbo].[QuestionnaireAnswerRecord]
                                                           ([UserID]
                                                           ,[QuestionnaireID]
                                                           ,[QuestionnaireName]
                                                           ,[QuestionID]
                                                           ,[QuestionName]
                                                           ,[QuestionType]
                                                           ,[AnswerText]
                                                           ,[AnswerOptionID]
                                                           ,[AnswerOptionContent]
                                                           ,[AnswerDate]
                                                           ,[QuestionScore]
                                                           ,[ObjectID]
                                                           ,[CreateDateTime]
                                                           ,[LastUpdateDateTime]
                                                           ,[IsDeleted])
                                                     VALUES
                                                           (@UserID
                                                           ,@QuestionnaireID
                                                           ,@QuestionnaireName
                                                           ,@QuestionID
                                                           ,@QuestionName
                                                           ,@QuestionType
                                                           ,@AnswerText
                                                           ,@AnswerOptionID
                                                           ,@AnswerOptionContent
                                                           ,@AnswerDate
                                                           ,@QuestionScore
                                                           ,@ObjectID
                                                           ,getdate()
                                                           ,getdate()
                                                           ,0);
                                                   SELECT SCOPE_IDENTITY();
                                                    ";
            try
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.AddParameter("@UserID", model.UserID);
                    cmd.AddParameter("@QuestionnaireID", model.QuestionnaireID);
                    cmd.AddParameter("@QuestionnaireName", model.QuestionnaireName ?? "");
                    cmd.AddParameter("@QuestionID", model.QuestionID);
                    cmd.AddParameter("@QuestionName", model.QuestionName ?? "");
                    cmd.AddParameter("@QuestionType", model.QuestionType);
                    cmd.AddParameter("@AnswerText", model.AnswerText ?? "");
                    cmd.AddParameter("@AnswerOptionID", model.AnswerOptionID);
                    cmd.AddParameter("@AnswerOptionContent", model.AnswerOptionContent ?? "");
                    cmd.AddParameter("@AnswerDate", model.AnswerDate);
                    cmd.AddParameter("@QuestionScore", model.QuestionScore);
                    cmd.AddParameter("@ObjectID", model.ObjectID);

                    var result = await helper.ExecuteScalarAsync(cmd);
                    return Convert.ToInt64(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>{nameof(SubmitQuestionnaire)} =>{ex}");
                throw ex;
            }

        }

        /// <summary>
        /// 删除用户的问卷答案记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public static async Task<int> DeleteQuestionnaireAnswerRecord(Guid userId, int objectId)
        {
            #region SQL

            string sql = @" UPDATE  Activity..QuestionnaireAnswerRecord WITH(ROWLOCK)
                            SET IsDeleted = 1,LastUpdateDateTime=GETDATE() 
                            WHERE UserID = @UserID
                                    AND ObjectID = @ObjectID; ";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = new SqlParameter[] {
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@ObjectID",objectId)
                };
                dbHelper = DbHelper.CreateDbHelper(false);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteNonQueryAsync(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>DeleteQuestionnaireAnswerRecord=>{ex.ToString()}");
                return 0;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }

        /// <summary>
        ///     获取用户的回答
        /// </summary>
        /// <param name="readOnly">是否查询读库</param>
        /// <param name="userId">用户ID</param>
        /// <param name="questionnaireID">问卷的ID</param>
        /// <param name="questionId">问题ID</param>
        /// <returns></returns>
        public static async Task<IEnumerable<QuestionnaireAnswerRecordModel>> SearchQuestionnaireAnswerRecordInfo(bool readOnly
            , Guid userId
            , long questionnaireID
            , long questionId)
        {


            var sql = @" SELECT    [PKID] 
                                  ,[UserID]
                                  ,[QuestionnaireID]
                                  ,[QuestionnaireName]
                                  ,[QuestionID]
                                  ,[QuestionName]
                                  ,[QuestionType]
                                  ,[AnswerText]
                                  ,[AnswerOptionID]
                                  ,[AnswerOptionContent]
                                  ,[AnswerDate]
                                  ,[QuestionScore]
                                  ,[ObjectID]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                          FROM [Activity].[dbo].[QuestionnaireAnswerRecord] with (nolock) where userId=@userId and questionnaireID = @questionnaireID and QuestionID = @QuestionID ";
            try
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.AddParameter("@userId", userId);
                    cmd.AddParameter("@questionnaireID", questionnaireID);
                    cmd.AddParameter("@QuestionID", questionId);
                    var result = await DbHelper.ExecuteSelectAsync<QuestionnaireAnswerRecordModel>(readOnly, cmd);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>{nameof(GetQuestionnaireAnswerRecordInfo)}=> {userId:D} {questionnaireID} {questionId} =>{ex}");
                throw ex;
            }

        }

        /// <summary>
        ///     获取用户最后一次的回答
        /// </summary>
        /// <param name="readOnly">是否查询读库</param>
        /// <param name="userId">用户ID</param>
        /// <param name="questionnaireID">问卷的ID</param>
        /// <param name="isClear">是否已经清算</param>
        /// <returns></returns>
        public static async Task<QuestionnaireAnswerRecordModel> GetLastQuestionnaireAnswerRecordInfo(bool readOnly, Guid userId, long questionnaireID , int? isClear=null)
        {

            var where = @"
                    where userId=@userId and questionnaireID = @questionnaireID
                ";
            if (isClear == 1 )
            {
                where = where + " and exists ( select * from [Activity].[dbo].[QuestionnaireAnswerResult] as b where a.pkid = b.QuestionnaireAnswerID and b.AnswerResultStatus <> 0  )";
            }

            var sql = @" SELECT   top 1  a.[PKID] 
                                  ,a.[UserID]
                                  ,a.[QuestionnaireID]
                                  ,a.[QuestionnaireName]
                                  ,a.[QuestionID]
                                  ,a.[QuestionName]
                                  ,a.[QuestionType]
                                  ,a.[AnswerText]
                                  ,a.[AnswerOptionID]
                                  ,a.[AnswerOptionContent]
                                  ,a.[AnswerDate]
                                  ,a.[QuestionScore]
                                  ,a.[ObjectID]
                                  ,a.[CreateDateTime]
                                  ,a.[LastUpdateDateTime]
                                  ,a.[IsDeleted]
                          FROM [Activity].[dbo].[QuestionnaireAnswerRecord] as a with (nolock)  "+ where + @"
                          order by pkid desc
                        ";
            try
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.AddParameter("@userId", userId);
                    cmd.AddParameter("@questionnaireID", questionnaireID);
                    var result = await DbHelper.ExecuteFetchAsync<QuestionnaireAnswerRecordModel>(readOnly, cmd);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>{nameof(GetLastQuestionnaireAnswerRecordInfo)}=> {userId:D} {questionnaireID}  {isClear} =>{ex}");
                throw ex;
            }

        }

    }
}
