using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuhu.Service.Activity.Models.Requests;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.UnitTest
{
    [TestClass]
    public class QuestionnaireUnitTest
    {
        [TestMethod]
        public void GetQuestionnaireURL()
        {
            using (var client = new QuestionnaireClient())
            {
                var requestModel = new GetQuestionnaireURLRequest();

                #region 售后
                requestModel.OrderID = 21184723;
                requestModel.ComplaintsID = 1033258;
                requestModel.ComplaintsType = "";
                //requestModel.IsAtStore = 2;
                requestModel.Department = "顾客主观";
                requestModel.UserID = new Guid("3B62F7E5-DD0D-4159-B577-70D795BEDDAF");
                requestModel.UserPhone = "95013600052417";
                requestModel.StaffEmail = "hanbaolin@tuhu.cn";
                #endregion

                #region 售前
                //requestModel.OrderID = 9999999;
                //requestModel.UserID = new Guid("B1315556-872E-4C83-BA2E-64C4C937268D");
                //requestModel.UserPhone = "95013600052417";
                //requestModel.StaffEmail = "hanbaolin@tuhu.cn";
                //requestModel.QuestionnaireType = 2;
                #endregion

                var result = client.GetQuestionnaireURL(requestModel);
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void GetQuestionnaireInfo()
        {
            using (var client = new QuestionnaireClient())
            {
                Guid pageId = new Guid("F28A2405-AD4C-47E8-97BC-C47A765C3777");
                var result = client.GetQuestionnaireInfo(pageId);
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void SubmitQuestionnaire()
        {
            using (var client = new QuestionnaireClient())
            {
                var model = new SubmitQuestionnaireRequest();
                model.PageID = new Guid("803BCF2F-6433-4AE9-B023-4BB730FFA5FE");
                model.QuestionnaireID = 1;

                //问题1
                var question1 = new Question();
                question1.QuestionID = 1;
                var questionOption1 = new AnswerOption();
                questionOption1.AnswerOptionID = 1;
                question1.AnswerOptionList = new List<AnswerOption>();
                question1.AnswerOptionList.Add(questionOption1);

                //问题2
                var question3 = new Question();
                question3.QuestionID = 4;
                var questionOption3 = new AnswerOption();
                questionOption3.AnswerOptionID = 11;
                var questionOption3A = new AnswerOption();
                questionOption3A.AnswerOptionID = 13;
                question3.AnswerOptionList = new List<AnswerOption>();
                question3.AnswerOptionList.Add(questionOption3);
                question3.AnswerOptionList.Add(questionOption3A);

                //问题3
                var question2 = new Question();
                question2.QuestionID = 8;
                var questionOption2 = new AnswerOption();
                questionOption2.AnswerOptionID = 24;
                questionOption2.AnswerText = "服务很好，很满意！";
                question2.AnswerOptionList = new List<AnswerOption>();
                question2.AnswerOptionList.Add(questionOption2);

                model.QuestionList = new List<Question>();
                model.QuestionList.Add(question1);
                model.QuestionList.Add(question2);
                model.QuestionList.Add(question3);

                var result = client.SubmitQuestionnaire(model);
                Assert.IsNotNull(result.Result);
            }
        }
    }
}
