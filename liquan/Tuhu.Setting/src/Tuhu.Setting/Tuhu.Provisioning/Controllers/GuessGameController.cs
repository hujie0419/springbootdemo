using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Activity;
using Tuhu.Service.ConfigLog;

namespace Tuhu.Provisioning.Controllers
{
    public class GuessGameController : Controller
    {
        // GET: GuessGame
        public ActionResult Index()
        {
            return View();
        }

        [PowerManage(IwSystem = "OperateSys")]
        public async Task<JsonResult> GetQuestionList(string endTime, int QuestionConfirm, int pageIndex = 1, int pageSize = 20)
        {
            bool useNowTime = true;
            GuessGameManager manager = new GuessGameManager();
            DateTime enddatetime = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                enddatetime = DateTime.Parse(endTime);
                useNowTime = false;
            }
            var list = manager.GetQuestionList(enddatetime, QuestionConfirm, pageSize, pageIndex, useNowTime);
            int totalCount = manager.GetQuestionCount(enddatetime, QuestionConfirm, useNowTime);
            return Json(new { data = list, totalCount = totalCount }, JsonRequestBehavior.AllowGet);
        }


        [PowerManage(IwSystem = "OperateSys")]
        public async Task<JsonResult> GetActivityPrizeList(string prizeName, int OnSale, int pageIndex = 1, int pageSize = 20)
        {
            GuessGameManager manager = new GuessGameManager();
           
            var list = manager.GetActivityPrizeList(prizeName, OnSale, pageSize, pageIndex);
            int totalCount = manager.GetActivityPrizeCount(prizeName, OnSale);
            return Json(new { data = list, totalCount = totalCount }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetQuestionAnswerList(string endTime)
        {
            GuessGameManager manager = new GuessGameManager();
            DateTime enddatetime = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                enddatetime = DateTime.Parse(endTime);
            }
            var list = manager.GetQuestionAnswerList(enddatetime);
            return Json(new { data = list}, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetQuestionWithOptionList(string endTime,bool isdeleted=false)
        {
            GuessGameManager manager = new GuessGameManager();
            DateTime enddatetime = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                enddatetime = DateTime.Parse(endTime);
            }
            var list = manager.GetQuestionWithOptionList(enddatetime,isdeleted);
            return Json(new { data = list}, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UpdateQuestionList(Question question1, Question question2, Question question3)
        {
            GuessGameManager manager = new GuessGameManager();
            List<Question> questionList = new List<Question>() { question1, question2, question3 };

           
            bool updatesuccess = manager.UpdateQuestionList(questionList);
            if (updatesuccess)
            {
                using (var client = new ConfigLogClient())
                {
                    foreach (var question in questionList)
                    {
                        var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId =question.PKID ,
                            ObjectType = "WorldCupConfig",
                            BeforeValue = "",
                            AfterValue = "",
                            Remark = "更新题目竞猜结果",
                            Creator = User.Identity.Name,
                        }));
                    }
                }
            }
            return Json(updatesuccess ? "" : "操作失败", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SaveQuestionWithOptionList(QuestionWithOption question1, QuestionWithOption question2, QuestionWithOption question3)
        {
            string errmsg = string.Empty;
            GuessGameManager manager = new GuessGameManager();
            
            question1.StartTime =  DateTime.Parse(question1.StartTime).ToString("yyyy-MM-dd 11:00:00");
            question1.EndTime = DateTime.Parse(question1.EndTime).ToString("yyyy-MM-dd 11:00:00");
            question2.StartTime = question1.StartTime;
            question2.EndTime = question1.EndTime;
            question3.StartTime = question1.StartTime;
            question3.EndTime = question1.EndTime;
            DateTime starttimedate = DateTime.Parse(question1.StartTime);
            DateTime endtimedate = DateTime.Parse(question1.EndTime);
            IEnumerable<Question> questionlist = manager.GetALLQuestionList();
            if (questionlist.Any())
            {
                foreach (var question in questionlist.Where(p=>p!=null).ToList())
                {
                    if (!(starttimedate >= DateTime.Parse(question.EndTime) || endtimedate <= DateTime.Parse(question.StartTime)))
                    {
                        return Json("题目设置时间与线上题目时间段:(" + question.StartTime + "-" + question.EndTime + ")有重叠,请重设", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            List<QuestionWithOption> questionList = new List<QuestionWithOption>() { question1, question2, question3 };

            bool savesuccess = manager.SaveQuestionWithOptionList(questionList);
            if (savesuccess)
            {
                using (var client = new ConfigLogClient())
                {
                    foreach (var question in questionList)
                    {
                        var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = question.PKID,
                            ObjectType = "WorldCupConfig",
                            BeforeValue = "",
                            AfterValue = "",
                            Remark = "新增题目及竞猜项",
                            Creator = User.Identity.Name,
                        }));
                    }
                }
            }
            return Json(savesuccess ? "" : "操作失败", JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> UpdateQuestionWithOptionList(QuestionWithOption question1, QuestionWithOption question2, QuestionWithOption question3)
        {
            GuessGameManager manager = new GuessGameManager();

            question1.StartTime = DateTime.Parse(question1.StartTime).ToString("yyyy-MM-dd 11:00:00");
            question1.EndTime = DateTime.Parse(question1.EndTime).ToString("yyyy-MM-dd 11:00:00");
            question2.StartTime = question1.StartTime;
            question2.EndTime = question1.EndTime;
            question3.StartTime = question1.StartTime;
            question3.EndTime = question1.EndTime;

            DateTime starttimedate = DateTime.Parse(question1.StartTime);
            DateTime endtimedate = DateTime.Parse(question1.EndTime);

            IEnumerable<Question> questionlist = manager.GetALLQuestionList();
            List<long> removePKIDlist = new List<long>() { question1.PKID, question2.PKID, question3.PKID };
            var allquestion = questionlist.ToList();
            allquestion.RemoveAll(s => removePKIDlist.Any(a=>a==s.PKID));
            if (allquestion.Any())
            {
                foreach (var question in allquestion.Where(p => p != null).ToList())
                {
                    if (!(starttimedate >= DateTime.Parse(question.EndTime) || endtimedate <= DateTime.Parse(question.StartTime)))
                    {
                        return Json("题目设置时间与线上题目时间段:(" + question.StartTime + "-" + question.EndTime + ")有重叠,请重设", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            List<QuestionWithOption> questionList = new List<QuestionWithOption>() { question1, question2, question3 };

            bool savesuccess = manager.UpdateQuestionWithOptionList(questionList);
            if (savesuccess)
            {
                using (var client = new ConfigLogClient())
                {
                    foreach (var question in questionList)
                    {
                        var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = question.PKID,
                            ObjectType = "WorldCupConfig",
                            BeforeValue = "",
                            AfterValue = "",
                            Remark = "修改题目及竞猜项",
                            Creator = User.Identity.Name,
                        }));
                    }
                }
            }
            return Json(savesuccess ? "" : "操作失败", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UpdateActivityPrize(ActivityPrize activityPrize)
        {
            GuessGameManager manager = new GuessGameManager();
            bool savesuccess = manager.UpdateActivityPrize(activityPrize);
            if (savesuccess)
            {
                using (var client = new ConfigLogClient())
                {
                    
                        var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = activityPrize.PKID,
                            ObjectType = "WorldCupConfigPrize",
                            BeforeValue = "",
                            AfterValue = "",
                            Remark = "修改兑换物信息",
                            Creator = User.Identity.Name,
                        }));
                    }
                
            }
            return Json(savesuccess ? "" : "操作失败", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> InsertActivityPrize(ActivityPrize activityPrize)
        {
            GuessGameManager manager = new GuessGameManager();
            bool savesuccess = manager.SaveActivityPrize(activityPrize);
            if (savesuccess)
            {
                using (var client = new ConfigLogClient())
                {

                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = activityPrize.PKID,
                        ObjectType = "WorldCupConfigPrize",
                        BeforeValue = "",
                        AfterValue = "",
                        Remark = "新增兑换物",
                        Creator = User.Identity.Name,
                    }));
                }
            }
            return Json(savesuccess ? "" : "操作失败", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> DeleteQuestionWithOptionList(string endTime)
        {
            GuessGameManager manager = new GuessGameManager();
            DateTime enddatetime = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                enddatetime = DateTime.Parse(endTime);
            }
            IEnumerable<Question> questionList = manager.GetQuestionAnswerList(enddatetime);
            bool savesuccess = manager.DeleteQuestionWithOptionList(enddatetime);
            if (savesuccess)
            {
                using (var client = new ConfigLogClient())
                {
                    foreach (var question in questionList)
                    {
                        var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = question.PKID,
                            ObjectType = "WorldCupConfig",
                            BeforeValue = "",
                            AfterValue = "",
                            Remark = "删除题目及竞猜项",
                            Creator = User.Identity.Name,
                        }));
                    }
                }
            }
            return Json(savesuccess ? "" : "操作失败", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetActivityPrize(long pkid)
        {
            GuessGameManager manager = new GuessGameManager();           
            var result = manager.GetActivityPrizeByPKID(pkid);          
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> DeleteActivityPrize(long pkid)
        {
            GuessGameManager manager = new GuessGameManager();
            var result = manager.DeleteActivityPrize(pkid);
            if (result)
            {
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = pkid,
                        ObjectType = "WorldCupConfigPrize",
                        BeforeValue = "",
                        AfterValue = "",
                        Remark = "删除兑换物",
                        Creator = User.Identity.Name,
                    }));
                }
            }
            return Json(result? "" : "操作失败", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UpdateActivityPrizeSale(int onsale, long pkid)
        {
            GuessGameManager manager = new GuessGameManager();
            var result = manager.UpdateActivityPrizeSale(onsale,pkid);
            if (result)
            {
                using (var client = new ConfigLogClient())
                {
                    var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = pkid,
                        ObjectType = "WorldCupConfigPrize",
                        BeforeValue = "",
                        AfterValue = "",
                        Remark = "修改兑换物上下架状态",
                        Creator = User.Identity.Name,
                    }));
                }
            }
            return Json(result? "" : "操作失败", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> RefreshQuestion()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var result = client.RefreshActivityQuestionCache(activity.Result.PKID);
                return Json(result.Result ? "" : "刷新缓存失败", JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> RefreshPrize()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var result = client.RefreshActivityPrizeCache(activity.Result.PKID);
                return Json(result.Result? "" : "刷新缓存失败", JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> ReleaseQuestionList(string endTime)
        {
            GuessGameManager manager = new GuessGameManager();
            DateTime enddatetime = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                enddatetime = DateTime.Parse(endTime);
            }
            var result = manager.ReleaseQuestionList(enddatetime);
            IEnumerable<Question> questionList = manager.GetQuestionAnswerList(enddatetime);
            if (result)
            {
                using (var client = new ConfigLogClient())
                {
                    foreach (var question in questionList)
                    {
                        var response = client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = question.PKID,
                            ObjectType = "WorldCupConfig",
                            BeforeValue = "",
                            AfterValue = "",
                            Remark = "公布答案",
                            Creator = User.Identity.Name,
                        }));
                    }
                }
            }
            return Json(result ? "" : "操作失败", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetLastestQustion()
        {
            GuessGameManager manager = new GuessGameManager();
            string endtime = string.Empty;
            var result = manager.GetLastestQustion();
            if (result != null)
            {
                DateTime datetime = DateTime.Parse(result.EndTime);
                endtime = datetime.ToString("yyyy-MM-dd");
            }
            return Json(endtime, JsonRequestBehavior.AllowGet);
        }
    }
}