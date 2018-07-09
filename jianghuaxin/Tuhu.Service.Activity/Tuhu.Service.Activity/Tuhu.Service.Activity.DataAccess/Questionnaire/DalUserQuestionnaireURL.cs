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
    public class DalUserQuestionnaireURL
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalUserQuestionnaireURL));
        /// <summary>
        /// 获取用户的问卷链接信息
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static async Task<UserQuestionnaireURLModel> GetUserQuestionnaireURLInfo(Guid pageId)
        {
            #region SQL

            string sql = @"SELECT [PKID]
                              ,[PageID]
                              ,[QuestionnaireNo]
                              ,[OrderID]
                              ,[ComplaintsID]
                              ,[ComplaintsType]
                              ,[IsAtStore]
                              ,[Department]
                              ,[UserID]
                              ,[UserPhone]
                              ,[StaffEmail]
                              ,[ShortURL]
                              ,[OriginalURL]
                              ,[CreateDateTime]
                              ,[LastUpdateDateTime]
                              ,[IsDeleted]
                          FROM [Activity].[dbo].[UserQuestionnaireURL] WITH(NOLOCK)
                          WHERE IsDeleted=0
                                AND PageID=@PageID";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = new SqlParameter[] { new SqlParameter("@PageID", pageId) };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteFetchAsync<UserQuestionnaireURLModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetUserQuestionnaireURLInfo=>{ex.ToString()}");
                return null;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }
        /// <summary>
        /// 创建用户的问卷链接信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<int> AddUserQuestionnaireURL(UserQuestionnaireURLModel model)
        {
            #region SQL

            string sql = @" INSERT INTO [Activity].[dbo].[UserQuestionnaireURL] WITH(ROWLOCK)
                                       ([PageID]
                                       ,[QuestionnaireNo]
                                       ,[OrderID]
                                       ,[ComplaintsID]
                                       ,[ComplaintsType]
                                       ,[IsAtStore]
                                       ,[Department]
                                       ,[UserID]
                                       ,[UserPhone]
                                       ,[StaffEmail]
                                       ,[ShortURL]
                                       ,[OriginalURL])
                                 VALUES
                                       (@PageID
                                       ,@QuestionnaireNo
                                       ,@OrderID
                                       ,@ComplaintsID
                                       ,ISNULL(@ComplaintsType,'')
                                       ,@IsAtStore
                                       ,ISNULL(@Department,'')
                                       ,@UserID
                                       ,ISNULL(@UserPhone,'')
                                       ,ISNULL(@StaffEmail,'')
                                       ,@ShortURL
                                       ,@OriginalURL) ";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;

            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@PageID",model.PageID),
                new SqlParameter("@QuestionnaireNo",model.QuestionnaireNo),
                new SqlParameter("@OrderID",model.OrderID),
                new SqlParameter("@ComplaintsID",model.ComplaintsID),
                new SqlParameter("@ComplaintsType",model.ComplaintsType),
                new SqlParameter("@IsAtStore",model.IsAtStore),
                new SqlParameter("@Department",model.Department),
                new SqlParameter("@UserID",model.UserID),
                new SqlParameter("@UserPhone",model.UserPhone),
                new SqlParameter("@StaffEmail",model.StaffEmail),
                new SqlParameter("@ShortURL",model.ShortURL),
                new SqlParameter("@OriginalURL",model.OriginalURL)
            };
            try
            {
                dbHelper = DbHelper.CreateDbHelper(false);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras);
                return await dbHelper.ExecuteNonQueryAsync(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>AddUserQuestionnaireURL=>{ex.ToString()}");
                return 0;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }

        }
        /// <summary>
        /// 根据pageId删除用户的问卷链接信息
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static async Task<int> DeleteUserQuestionnaireURL(Guid pageId)
        {
            #region SQL

            string sql = @" UPDATE [Activity].[dbo].[UserQuestionnaireURL] WITH(ROWLOCK) SET IsDeleted=1,LastUpdateDateTime=GETDATE() WHERE PageID=@PageID ";
            #endregion
            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;

            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@PageID",pageId)
            };
            try
            {
                dbHelper = DbHelper.CreateDbHelper(false);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras);
                return await dbHelper.ExecuteNonQueryAsync(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>AddUserQuestionnaireURL=>{ex.ToString()}");
                return 0;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }
        /// <summary>
        /// 获取用户的问卷链接信息
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static async Task<UserQuestionnaireURLModel> GetUserQuestionnaireURLInfo(int complaintsID)
        {
            #region SQL

            string sql = @"SELECT [PKID]
                              ,[PageID]
                              ,[QuestionnaireNo]
                              ,[OrderID]
                              ,[ComplaintsID]
                              ,[ComplaintsType]
                              ,[IsAtStore]
                              ,[Department]
                              ,[UserID]
                              ,[UserPhone]
                              ,[StaffEmail]
                              ,[ShortURL]
                              ,[OriginalURL]
                              ,[CreateDateTime]
                              ,[LastUpdateDateTime]
                              ,[IsDeleted]
                          FROM [Activity].[dbo].[UserQuestionnaireURL] WITH(NOLOCK)
                          WHERE  IsDeleted=0
                                 AND ComplaintsID=@ComplaintsID";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = new SqlParameter[] { new SqlParameter("@ComplaintsID", complaintsID) };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteFetchAsync<UserQuestionnaireURLModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetUserQuestionnaireURLInfo=>{ex.ToString()}");
                return null;
            }
            finally
            {
                dbHelper?.Dispose();
                sqlCmd?.Dispose();
            }
        }
        /// <summary>
        /// 获取用户的问卷链接信息
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static async Task<UserQuestionnaireURLModel> GetUserQuestionnaireURLInfoByOrderID(int orderID)
        {
            #region SQL

            string sql = @"SELECT [PKID]
                              ,[PageID]
                              ,[QuestionnaireNo]
                              ,[OrderID]
                              ,[ComplaintsID]
                              ,[ComplaintsType]
                              ,[IsAtStore]
                              ,[Department]
                              ,[UserID]
                              ,[UserPhone]
                              ,[StaffEmail]
                              ,[ShortURL]
                              ,[OriginalURL]
                              ,[CreateDateTime]
                              ,[LastUpdateDateTime]
                              ,[IsDeleted]
                          FROM [Activity].[dbo].[UserQuestionnaireURL] WITH(NOLOCK)
                          WHERE  IsDeleted=0
                                 AND OrderID=@OrderID";
            #endregion

            BaseDbHelper dbHelper = null;
            SqlCommand sqlCmd = null;
            try
            {
                SqlParameter[] paras = new SqlParameter[] { new SqlParameter("@OrderID", orderID) };
                dbHelper = DbHelper.CreateDbHelper(true);
                sqlCmd = new SqlCommand(sql);
                sqlCmd.Parameters.AddRange(paras.ToArray());
                return await dbHelper.ExecuteFetchAsync<UserQuestionnaireURLModel>(sqlCmd);
            }
            catch (Exception ex)
            {
                Logger.Error($"DB异常=>GetUserQuestionnaireURLInfoByOrderID=>{ex.ToString()}");
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
