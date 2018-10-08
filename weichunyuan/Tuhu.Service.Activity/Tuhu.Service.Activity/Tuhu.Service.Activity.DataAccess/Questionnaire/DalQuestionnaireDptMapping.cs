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
    public class DalQuestionnaireDptMapping
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalQuestionnaireDptMapping));

        /// <summary>
        /// 获取问卷和定责部门关系信息
        /// </summary>
        /// <param name="department"></param>
        /// <param name="complaintsType"></param>
        /// <param name="isAtStore"></param>
        /// <returns></returns>
        public static async Task<QuestionnaireDptMappingModel> GetQuestionnaireDptMappingInfo(string department, string complaintsType, int isAtStore)
        {
            #region SQL

            string sql = @" SELECT  TOP 1
                                    [PKID] ,
                                    [QuestionnaireNo] ,
                                    [Department] ,
                                    [ComplaintsType] ,
                                    [IsAtStore] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime] ,
                                    [IsDeleted]
                            FROM    [Activity].[dbo].[QuestionnaireDptMapping] WITH ( NOLOCK )
                            WHERE   IsDeleted = 0
                                    AND Department = @Department
                                    AND ComplaintsType = ISNULL(@ComplaintsType,'')
                                    AND IsAtStore = ISNULL(@IsAtStore,0);";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = new SqlParameter[] {
                    new SqlParameter("@Department", department),
                    new SqlParameter("@ComplaintsType",complaintsType),
                    new SqlParameter("@IsAtStore",isAtStore)
                };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteFetchAsync<QuestionnaireDptMappingModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetQuestionnaireDptMappingInfo=>{ex.ToString()}");
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
