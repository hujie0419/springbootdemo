using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    public class QuestionnaireService : IQuestionnaireService
    {
        /// <summary>
        /// 获取问卷信息
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public async Task<OperationResult<GetQuestionnaireInfoResponse>> GetQuestionnaireInfoAsync(Guid pageId)
        {
            if (pageId == Guid.Empty)
                return OperationResult.FromError<GetQuestionnaireInfoResponse>(nameof(Resource.ParameterError_PageID_NotNull), Resource.ParameterError_PageID_NotNull);

            bool isAnswered = await QuestionnaireManager.CheckIsAnswered(pageId);
            if (isAnswered)
                return OperationResult.FromError<GetQuestionnaireInfoResponse>(nameof(Resource.Questionnaire_IsAnswered), Resource.Questionnaire_IsAnswered);

            return await QuestionnaireManager.GetQuestionnaireInfoAsync(pageId);
        }
        /// <summary>
        /// 获取用户的问卷链接信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OperationResult<string>> GetQuestionnaireURLAsync(Models.Requests.GetQuestionnaireURLRequest model)
        {
            if (model.QuestionnaireType < 2)//售后问卷
            {
                if (model.ComplaintsID <= 0)
                    return OperationResult.FromError<string>(nameof(Resource.ParameterError_ComplaintsID_NotNull), Resource.ParameterError_ComplaintsID_NotNull);
                if (string.IsNullOrEmpty(model.Department))
                    return OperationResult.FromError<string>(nameof(Resource.ParameterError_Department_NotNull), Resource.ParameterError_Department_NotNull);
            }else if (model.QuestionnaireType == 2)//售前问卷
            {
                if (model.OrderID <= 0)
                    return OperationResult.FromError<string>(nameof(Resource.ParameterError_OrderID_NotNull), Resource.ParameterError_OrderID_NotNull);
                if(string.IsNullOrEmpty(model.UserPhone))
                    return OperationResult.FromError<string>(nameof(Resource.ParameterError_UserPhone_NotNull), Resource.ParameterError_UserPhone_NotNull);
                if (string.IsNullOrEmpty(model.StaffEmail))
                    return OperationResult.FromError<string>(nameof(Resource.ParameterError_StaffEmail_NotNull), Resource.ParameterError_StaffEmail_NotNull);

            }
            if (model.UserID == Guid.Empty)
                return OperationResult.FromError<string>(nameof(Resource.ParameterError_UserID_NotNull), Resource.ParameterError_UserID_NotNull);
            return await QuestionnaireManager.GetQuestionnaireURLAsync(model);
        }
        /// <summary>
        /// 提交问卷
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> SubmitQuestionnaireAsync(Models.Requests.SubmitQuestionnaireRequest model)
        {
            if (model.PageID == Guid.Empty)
                return OperationResult.FromError<bool>(nameof(Resource.ParameterError_PageID_NotNull), Resource.ParameterError_PageID_NotNull);
            if (model.QuestionnaireID <= 0)
                return OperationResult.FromError<bool>(nameof(Resource.ParameterError_QuestionnaireID_NotNull), Resource.ParameterError_QuestionnaireID_NotNull);
            if (model.QuestionList == null || !model.QuestionList.Any())
                return OperationResult.FromError<bool>(nameof(Resource.ParameterError_QuestionList_NotNull), Resource.ParameterError_QuestionList_NotNull);

            bool isAnswered = await QuestionnaireManager.CheckIsAnswered(model.PageID);
            if (isAnswered)
                return OperationResult.FromError<bool>(nameof(Resource.Questionnaire_IsAnswered), Resource.Questionnaire_IsAnswered);

            return await QuestionnaireManager.SubmitQuestionnaireAsync(model);
        }
    }
}
