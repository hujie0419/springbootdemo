using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Questionnaire;
using Tuhu.Service.Activity.Models.Questionnaire;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Utility;

namespace Tuhu.Service.Activity.Server.Manager
{
    public class QuestionnaireManager
    {
        /// <summary>
        /// 获取问卷信息
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GetQuestionnaireInfoResponse>> GetQuestionnaireInfoAsync(Guid pageId)
        {
            var response = new GetQuestionnaireInfoResponse();
            Question questionInfo = null;
            var userInfo = await DalUserQuestionnaireURL.GetUserQuestionnaireURLInfo(pageId);
            if (userInfo != null)
            {
                //有效期验证(投诉单问卷有效期为7天)
                if (userInfo.CreateDateTime.Date.AddDays(7) < DateTime.Now.Date)
                    return OperationResult.FromError<GetQuestionnaireInfoResponse>(nameof(Resource.Questionnaire_Overdue), string.Format(Resource.Questionnaire_Overdue));

                var questionnaireInfo = await DalQuestionnaire.GetQuestionnaireInfoByNo(userInfo.QuestionnaireNo);
                if (questionnaireInfo != null)
                {
                    //AutoMapper初始化配置文件
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<QuestionnaireModel, GetQuestionnaireInfoResponse>();
                        cfg.CreateMap<QuestionModel, Question>();
                        cfg.CreateMap<QuestionOptionModel, QuestionOption>();
                    });
                    var mapper = config.CreateMapper();

                    response = mapper.Map<GetQuestionnaireInfoResponse>(questionnaireInfo);
                    response.QuestionList = new List<Question>();
                    var questionList = await DalQuestion.GetQuestionList(questionnaireInfo.QuestionnaireID);
                    var optionList = await DalQuestionOption.GetQuestionOptionList(questionnaireInfo.QuestionnaireID);
                    foreach (var item in questionList)
                    {
                        questionInfo = new Question();
                        questionInfo = mapper.Map<Question>(item);
                        questionInfo.QuestionOptionList = mapper.Map<List<QuestionOption>>(optionList.Where(t => t.QuestionID == questionInfo.QuestionID).ToList());
                        response.QuestionList.Add(questionInfo);
                    }
                }
            }
            else
                return OperationResult.FromError<GetQuestionnaireInfoResponse>(nameof(Resource.Questionnaire_NotExist), string.Format(Resource.Questionnaire_NotExist));
            return OperationResult.FromResult(response);
        }
        /// <summary>
        /// 检查用户是否已提交问卷
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static async Task<bool> CheckIsAnswered(Guid pageId)
        {
            bool result = false;//未提交问卷
            var userInfo = await DalUserQuestionnaireURL.GetUserQuestionnaireURLInfo(pageId);
            if (userInfo != null)
            {
                var questionnaireInfo = await DalQuestionnaire.GetQuestionnaireInfoByNo(userInfo.QuestionnaireNo);
                if (questionnaireInfo != null)
                {
                    QuestionnaireAnswerRecordModel answerRecordInfo = null;
                    if (questionnaireInfo.QuestionnaireType == 1)       //售后问卷
                        answerRecordInfo = await DalQuestionnaireAnswerRecord.GetQuestionnaireAnswerRecordInfo(userInfo.UserID, userInfo.ComplaintsID);
                    else if (questionnaireInfo.QuestionnaireType == 2)  //售前问卷
                        answerRecordInfo = await DalQuestionnaireAnswerRecord.GetQuestionnaireAnswerRecordInfo(userInfo.UserID, userInfo.OrderID);
                    if (answerRecordInfo != null)
                        result = true;//已提交问卷
                }
            }
            return result;
        }
        /// <summary>
        /// 提交问卷
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> SubmitQuestionnaireAsync(Models.Requests.SubmitQuestionnaireRequest model)
        {
            bool result = false;
            var userInfo = await DalUserQuestionnaireURL.GetUserQuestionnaireURLInfo(model.PageID);
            if (userInfo != null)
            {
                //有效期验证(投诉单问卷有效期为7天)
                if (userInfo.CreateDateTime.Date.AddDays(7) < DateTime.Now.Date)
                    return OperationResult.FromError<bool>(nameof(Resource.Questionnaire_Overdue), string.Format(Resource.Questionnaire_Overdue));

                var questionnaireInfo = await DalQuestionnaire.GetQuestionnaireInfoByNo(userInfo.QuestionnaireNo);
                if (questionnaireInfo != null)
                {
                    var questionList = await DalQuestion.GetQuestionList(questionnaireInfo.QuestionnaireID);
                    var optionList = await DalQuestionOption.GetQuestionOptionList(questionnaireInfo.QuestionnaireID);
                    List<QuestionnaireAnswerRecordModel> answerRecordList = new List<QuestionnaireAnswerRecordModel>();
                    QuestionnaireAnswerRecordModel answerRecord = null;

                    #region 装载数据
                    foreach (var item in model.QuestionList)
                    {
                        // 验证数据
                        var questionInfo = questionList.Where(t => t.QuestionID == item.QuestionID).FirstOrDefault();
                        if (questionInfo != null)
                        {
                            var answerQuestionInfo = model.QuestionList.Where(t => t.QuestionID == item.QuestionID).FirstOrDefault();
                            if (answerQuestionInfo != null)
                            {
                                //是否必填验证
                                if (questionInfo.IsRequired == 1)
                                {
                                    if (answerQuestionInfo != null)
                                    {
                                        if (answerQuestionInfo.AnswerOptionList == null || !answerQuestionInfo.AnswerOptionList.Any())
                                            return OperationResult.FromError<bool>(nameof(Resource.ParameterError_IsRequired), string.Format(Resource.ParameterError_IsRequired, questionInfo.QuestionID));
                                    }
                                }
                                //最大字符验证
                                if (questionInfo.IsValidateMaxChar == 1)
                                {
                                    if (answerQuestionInfo != null)
                                    {
                                        foreach (var option in answerQuestionInfo.AnswerOptionList)
                                        {
                                            if (option.AnswerText.Length > questionInfo.MaxChar)
                                                return OperationResult.FromError<bool>(nameof(Resource.ParameterError_OverMaxChar), string.Format(Resource.ParameterError_OverMaxChar, questionInfo.QuestionID));
                                        }
                                    }
                                }
                                QuestionOptionModel optionInfo = null;
                                foreach (var option in item.AnswerOptionList)
                                {
                                    answerRecord = new QuestionnaireAnswerRecordModel();
                                    answerRecord.UserID = userInfo.UserID;
                                    if (questionnaireInfo.QuestionnaireType < 2)    //售后问卷
                                        answerRecord.ObjectID = userInfo.ComplaintsID;
                                    else if(questionnaireInfo.QuestionnaireType==2) //售前问卷
                                        answerRecord.ObjectID = userInfo.OrderID;
                                    answerRecord.QuestionnaireID = questionnaireInfo.QuestionnaireID;
                                    answerRecord.QuestionnaireName = questionnaireInfo.QuestionnaireName;
                                    answerRecord.QuestionID = item.QuestionID;

                                    answerRecord.QuestionName = questionInfo.QuestionTitle;
                                    answerRecord.QuestionType = questionInfo.QuestionType;
                                    answerRecord.AnswerText = option.AnswerText;

                                    optionInfo = optionList.Where(t => t.OptionID == option.AnswerOptionID).FirstOrDefault();
                                    if (optionInfo != null)
                                    {
                                        answerRecord.AnswerOptionID = optionInfo.OptionID;
                                        answerRecord.AnswerOptionContent = optionInfo.OptionContent;
                                    }
                                    answerRecordList.Add(answerRecord);
                                }
                            }
                        }
                    }
                    #endregion

                    await DalQuestionnaireAnswerRecord.SubmitQuestionnaire(answerRecordList);
                    result = true;
                }
            }
            else
                return OperationResult.FromError<bool>(nameof(Resource.Questionnaire_NotExist), string.Format(Resource.Questionnaire_NotExist));

            return OperationResult.FromResult(result);
        }
        /// <summary>
        /// 获取用户的问卷链接信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<string>> GetQuestionnaireURLAsync(Models.Requests.GetQuestionnaireURLRequest model)
        {
            //AutoMapper初始化配置文件
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Models.Requests.GetQuestionnaireURLRequest, UserQuestionnaireURLModel>();
            });
            var mapper = config.CreateMapper();
            int objectId = 0;//业务对象ID

            UserQuestionnaireURLModel userModel = mapper.Map<UserQuestionnaireURLModel>(model);
            userModel.PageID = Guid.NewGuid();
            userModel.OriginalURL = ConfigurationManager.AppSettings["QuestionnaireURL"].ToString() + "pageId=" + userModel.PageID;

            if (model.QuestionnaireType < 2)//售后问卷
            {
                //获取问卷和定责部门关系信息
                var mappingInfo = await DalQuestionnaireDptMapping.GetQuestionnaireDptMappingInfo(userModel.Department, userModel.ComplaintsType, userModel.IsAtStore);
                if (mappingInfo != null)
                    userModel.QuestionnaireNo = mappingInfo.QuestionnaireNo;
                else
                    return OperationResult.FromError<string>(nameof(Resource.Error_GetQuestionnaireURL_Failure), string.Format(Resource.Error_GetQuestionnaireURL_Failure));
                objectId = model.ComplaintsID;
            }
            else if (model.QuestionnaireType == 2)//售前问卷
            {
                var questionnaireInfo = await DalQuestionnaire.GetQuestionnaireInfoByType(model.QuestionnaireType);
                userModel.QuestionnaireNo = questionnaireInfo.QuestionnaireNo;
                objectId = model.OrderID.Value;
            }
            //获取短连接
            using (var client = new UtilityClient())
            {
                var shortUrlResult = await client.GetTuhuDwzAsync(userModel.OriginalURL, "投诉问卷");
                if (shortUrlResult.Success)
                    userModel.ShortURL = shortUrlResult.Result;
                else
                    return OperationResult.FromError<string>(nameof(Resource.Error_GetQuestionnaireURL_Failure), string.Format(Resource.Error_GetQuestionnaireURL_Failure));
            }
            //删除重复问卷答题记录和获取问卷记录
            await QuestionnaireManager.DelExistRecord(model.UserID, objectId, model.QuestionnaireType);

            int result = await DalUserQuestionnaireURL.AddUserQuestionnaireURL(userModel);
            if (result > 0)
                return OperationResult.FromResult(userModel.ShortURL);

            return OperationResult.FromError<string>(nameof(Resource.Error_GetQuestionnaireURL_Failure), string.Format(Resource.Error_GetQuestionnaireURL_Failure));
        }
        /// <summary>
        /// 删除重复问卷答题记录和获取问卷记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public static async Task DelExistRecord(Guid userId, int objectId, int questionnaireType)
        {
            //判断当前用户和投诉是否已答
            var answerRecordInfo = await DalQuestionnaireAnswerRecord.GetQuestionnaireAnswerRecordInfo(userId, objectId);
            if (answerRecordInfo != null)
                //删除之前答题记录
                await DalQuestionnaireAnswerRecord.DeleteQuestionnaireAnswerRecord(userId, objectId);

            UserQuestionnaireURLModel userInfo = null;
            if (questionnaireType < 2)//售后问卷
                userInfo = await DalUserQuestionnaireURL.GetUserQuestionnaireURLInfo(objectId);
            else if (questionnaireType == 2)//售前问卷
                userInfo = await DalUserQuestionnaireURL.GetUserQuestionnaireURLInfoByOrderID(objectId);
            if (userInfo != null)
                //删除之前问卷记录
                await DalUserQuestionnaireURL.DeleteUserQuestionnaireURL(userInfo.PageID);
        }
    }
}

