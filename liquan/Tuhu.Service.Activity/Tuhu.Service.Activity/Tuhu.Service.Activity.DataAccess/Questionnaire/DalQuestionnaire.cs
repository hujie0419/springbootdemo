using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models.Questionnaire;

namespace Tuhu.Service.Activity.DataAccess.Questionnaire
{
    public class DalQuestionnaire
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalQuestionnaire));
        /// <summary>
        /// 根据问卷编号获取问卷信息
        /// </summary>
        /// <param name="questionnaireNo"></param>
        /// <returns></returns>
        public static async Task<QuestionnaireModel> GetQuestionnaireInfoByNo (int questionnaireNo)
        {
            #region SQL

            string sql = @"SELECT Top 1 
                                  [PKID] AS QuestionnaireID
                                  ,[QuestionnaireNo]
                                  ,[QuestionnaireName]
                                  ,[QuestionnaireType]
                                  ,[IsShowRegulation]
                                  ,[Regulation]
                                  ,[CompletionInfo]
                                  ,[ParticipantsCount]
                                  ,[Sort]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                              FROM [Activity].[dbo].[Questionnaire] WITH ( NOLOCK )
                              WHERE IsDeleted=0 AND QuestionnaireNo=@QuestionnaireNo";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = new SqlParameter[] { new SqlParameter("@QuestionnaireNo", questionnaireNo) };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteFetchAsync<QuestionnaireModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetQuestionnaireByNo=>{ex.ToString()}");
                return null;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }
        /// <summary>
        /// 根据问卷类型获取问卷信息
        /// </summary>
        /// <param name="questionnaireType">问卷类型，1=售后问卷；2=售前问卷; 5=2018世界杯</param>
        /// <returns></returns>
        public static async Task<QuestionnaireModel> GetQuestionnaireInfoByType(int questionnaireType)
        {
            #region SQL

            string sql = @"SELECT Top 1 
                                  [PKID] AS QuestionnaireID
                                  ,[QuestionnaireNo]
                                  ,[QuestionnaireName]
                                  ,[QuestionnaireType]
                                  ,[IsShowRegulation]
                                  ,[Regulation]
                                  ,[ParticipantsCount]
                                  ,[Sort]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                              FROM [Activity].[dbo].[Questionnaire] WITH ( NOLOCK )
                              WHERE IsDeleted=0 AND QuestionnaireType=@QuestionnaireType
                              ORDER BY CreateDateTime DESC";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = new SqlParameter[] { new SqlParameter("@QuestionnaireType", questionnaireType) };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteFetchAsync<QuestionnaireModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetQuestionnaireInfoByType=>{ex.ToString()}");
                return null;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }

        /// <summary>
        ///     根据主键获取问卷信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<QuestionnaireModel> GetQuestionnaireInfoByPKID(long pkid)
        {
            #region SQL

            string sql = @"SELECT Top 1 
                                  [PKID] AS QuestionnaireID
                                  ,[QuestionnaireNo]
                                  ,[QuestionnaireName]
                                  ,[QuestionnaireType]
                                  ,[IsShowRegulation]
                                  ,[Regulation]
                                  ,[ParticipantsCount]
                                  ,[Sort]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                              FROM [Activity].[dbo].[Questionnaire] WITH ( NOLOCK )
                              WHERE IsDeleted=0 AND pkid=@pkid";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = new SqlParameter[] { new SqlParameter("@pkid", pkid) };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteFetchAsync<QuestionnaireModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetQuestionnaireInfoByPKID=>{ex.ToString()}");
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
