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
    public class DalQuestionOption
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalQuestionOption));
        /// <summary>
        /// 根据问卷标识获取问题的所有选项
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<QuestionOptionModel>> GetQuestionOptionList(long questionnaireId)
        {
            #region SQL
            string sql = @" SELECT  [PKID] AS OptionID ,
                                    [QuestionnaireID] ,
                                    [QuestionID] ,
                                    [OptionContent] ,
                                    [OptionPicSrc] ,
                                    [NoOptionScore] ,
                                    [YesOptionScore] ,
                                    [IsRightValue] ,
                                    [ForkQuestionID] ,
                                    [Sort] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime] ,
                                    [IsDeleted],
                                    ISNULL([IsAdditionalInfo],0) AS [IsAdditionalInfo],
                                    ISNULL([IsShowAdditionalInfo],0) AS [IsShowAdditionalInfo],
                                    [QuestionParentID],
                                    [UseIntegral],
                                    [WinCouponCount]
                            FROM    [Activity].[dbo].[QuestionOption] WITH ( NOLOCK )
                            WHERE   IsDeleted = 0
                                    AND QuestionnaireID = @QuestionnaireID
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
                return await dbHelper.ExecuteSelectAsync<QuestionOptionModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetQuestionOptionList=>{ex.ToString()}");
                return null;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }


        /// <summary>
        ///     根据问题获取问题的所有选项
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<QuestionOptionModel>> GetQuestionOptionListByQuestionId(long questionId)
        {
            #region SQL
            string sql = @" SELECT  [PKID] AS OptionID ,
                                    [QuestionnaireID] ,
                                    [QuestionID] ,
                                    [OptionContent] ,
                                    [OptionPicSrc] ,
                                    [NoOptionScore] ,
                                    [YesOptionScore] ,
                                    [IsRightValue] ,
                                    [ForkQuestionID] ,
                                    [Sort] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime] ,
                                    [IsDeleted],
                                    ISNULL([IsAdditionalInfo],0) AS [IsAdditionalInfo],
                                    ISNULL([IsShowAdditionalInfo],0) AS [IsShowAdditionalInfo],
                                    [QuestionParentID],
                                    [UseIntegral],
                                    [WinCouponCount]
                            FROM    [Activity].[dbo].[QuestionOption] WITH ( NOLOCK )
                            WHERE   IsDeleted = 0
                                    AND QuestionID = @QuestionID
                                    ORDER BY Sort ASC ,PKID asc";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = new SqlParameter[] { new SqlParameter("@QuestionID", questionId) };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteSelectAsync<QuestionOptionModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>{nameof(GetQuestionOptionListByQuestionId)}=>{ex.ToString()}");
                return null;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }


        /// <summary>
        ///     根据问卷ID获取问题的所有选项
        /// </summary>
        /// <param name="questionnaireID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<QuestionOptionModel>> GetQuestionOptionListByQuestionnaireId(long questionnaireID)
        {
            #region SQL
            string sql = @" SELECT  [PKID] AS OptionID ,
                                    [QuestionnaireID] ,
                                    [QuestionID] ,
                                    [OptionContent] ,
                                    [OptionPicSrc] ,
                                    [NoOptionScore] ,
                                    [YesOptionScore] ,
                                    [IsRightValue] ,
                                    [ForkQuestionID] ,
                                    [Sort] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime] ,
                                    [IsDeleted],
                                    ISNULL([IsAdditionalInfo],0) AS [IsAdditionalInfo],
                                    ISNULL([IsShowAdditionalInfo],0) AS [IsShowAdditionalInfo],
                                    [QuestionParentID],
                                    [UseIntegral],
                                    [WinCouponCount]
                            FROM    [Activity].[dbo].[QuestionOption] WITH ( NOLOCK )
                            WHERE   IsDeleted = 0
                                    AND QuestionnaireID = @QuestionnaireID
                                    ORDER BY Sort ASC,PKID asc ";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = { new SqlParameter("@QuestionnaireID", questionnaireID) };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteSelectAsync<QuestionOptionModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>{nameof(GetQuestionOptionListByQuestionnaireId)}=>{ex.ToString()}");
                return null;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }

        /// <summary>
        ///     通过主键 获取 问题选项
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<QuestionOptionModel> GetQuestionOptionById(long pkid)
        {
            #region SQL
            string sql = @" SELECT  [PKID] AS OptionID ,
                                    [QuestionnaireID] ,
                                    [QuestionID] ,
                                    [OptionContent] ,
                                    [OptionPicSrc] ,
                                    [NoOptionScore] ,
                                    [YesOptionScore] ,
                                    [IsRightValue] ,
                                    [ForkQuestionID] ,
                                    [Sort] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime] ,
                                    [IsDeleted],
                                    ISNULL([IsAdditionalInfo],0) AS [IsAdditionalInfo],
                                    ISNULL([IsShowAdditionalInfo],0) AS [IsShowAdditionalInfo],
                                    [QuestionParentID],
                                    [UseIntegral],
                                    [WinCouponCount]
                            FROM    [Activity].[dbo].[QuestionOption] WITH ( NOLOCK )
                            WHERE   IsDeleted = 0
                                    AND pkid = @pkid  ";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = new SqlParameter[] { new SqlParameter("@pkid", pkid) };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return (await dbHelper.ExecuteSelectAsync<QuestionOptionModel>(sqlCmd)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>{nameof(GetQuestionOptionById)}=>{ex.ToString()}");
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
