using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Mapping;
using System.Data;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Framework;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Controllers
{
    public class AnswerController : Controller
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("AnswerController");

        // GET: Answer
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> AddExport()
        {
            var file = Request.Files[0];

            if (file.ContentType != "application/vnd.ms-excel" && file.ContentType != "application/x-xls")
                return Content(JsonConvert.SerializeObject(new { Status = -1, Error = "请上传.xlsx文件或者.xls文件" }), "text/html");

            var excel = new Controls.ExcelHelper(file.InputStream, file.FileName);
            var dt = excel.ExcelToDataTable("问答题库", true);
            if (dt != null && dt?.Rows?.Count > 0)
            {
                if (dt.Columns.Count != 8)
                    return Content(JsonConvert.SerializeObject(new { Status = -1, Error = "请确认文档的列是否只有【编码】【标签】【题目】【选项A】【选项B】【选项B】【选项C】【选项D】【正确选项】这几列" }), "text/html");

                RepositoryManager manager = new RepositoryManager();
                using (var db = manager.BeginTrans())
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (string.IsNullOrWhiteSpace(dr[2].ToString().Trim()))//过滤 答题  为空的 行
                        {
                            continue;
                        }
                        AnswerInfoListEntity entity = new AnswerInfoListEntity()
                        {
                            Tip = dr[1].ToString(),
                            Answer = dr[2].ToString(),
                            OptionsA = dr[3].ToString(),
                            OptionsB = dr[4].ToString(),
                            OptionsC = dr[5].ToString(),
                            OptionsD = dr[6].ToString(),
                            OptionsReal = dr[7].ToString(),
                            CreateDateTime = DateTime.Now,
                            LastUpdateDateTime = DateTime.Now,
                            IsEnabled = true
                        };
                        db.Insert<AnswerInfoListEntity>(entity);
                    }
                    db.Commit();
                }

                //读写分离 做延时
                await Task.Delay(2000);

                #region  更新问答题库缓存
                RefreshQuestionInfoListCache();
                #endregion

                return Content(JsonConvert.SerializeObject(new { Status = 1, Error = "导入成功" }), "text/html");

            }
            else
            {
                return Content(JsonConvert.SerializeObject(new { Status = -1, Error = "请确认xls的sheet名称是否为：问答题库;是否有内容" }), "text/html");
            }
        }

        public ActionResult GetGridJson(Pagination pagination, int showEnabledType = -1, string keyword = "")
        {
            keyword = keyword.Trim();
            RepositoryManager db = new RepositoryManager();
            List<AnswerInfoListEntity> list = new List<AnswerInfoListEntity>();

            pagination.sord = "desc";
            if (showEnabledType == -1)
            {
                list = db.GetEntityList<AnswerInfoListEntity>(p => p.Answer.Contains(keyword) && p.Is_Deleted==false, pagination);
            }
            else
            {
                list = db.GetEntityList<AnswerInfoListEntity>((en) => en.IsEnabled == (showEnabledType == 1) && en.Answer.Contains(keyword) && en.Is_Deleted==false, pagination);
            }

            return Content(JsonConvert.SerializeObject(new
            {
                rows = list,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            }));
        }

        public async Task<ActionResult> DeleteAnswer(List<int> pkids)
        {
            string msg = "ok";
            try
            {
                RepositoryManager db = new RepositoryManager();
                RepositoryManager manager = new RepositoryManager();
                var answerInfoList = manager.GetEntityList<AnswerInfoListEntity>(_ => pkids.Contains(_.PKID)&&_.Is_Deleted==false);
                foreach (var item in answerInfoList)
                {
                    item.Is_Deleted = true;
                    item.LastUpdateDateTime = DateTime.Now;
                    manager.Update<AnswerInfoListEntity>(item);
                }

                //读写分离 做延时
                await Task.Delay(2000);

                #region  更新问答题库缓存
                RefreshQuestionInfoListCache();
                #endregion
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { Status = msg == "ok" ? 1 : 0, Error = msg });
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        public ActionResult Reload()
        {
            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.UpdateQuestionInfoList();
                if (result.Success && result.Result)
                    return Content(JsonConvert.SerializeObject(new
                    {
                        Code = 1,
                        Msg = "刷新成功"
                    }));
                else
                    return Content(JsonConvert.SerializeObject(new
                    {
                        Code = 0,
                        Msg = "刷新失败",
                        Error = result.Exception.Message
                    }));
            }
        }

        public async Task<JsonResult> UpdateIsEnabled(string ids, bool enabled)
        {
            RepositoryManager manager = new RepositoryManager();
            using (var db = manager.BeginTrans())
            {
                var list = db.FindList<AnswerInfoListEntity>($"SELECT * FROM Configuration.dbo.AnswerInfoList WITH(NOLOCK) WHERE PKID IN ({ids}) AND Is_Deleted=0");
                foreach (var item in list)
                {
                    item.IsEnabled = enabled;
                    db.Update(item);
                }
                db.Commit();
            }

            //读写分离 做延时
            await Task.Delay(2000);

            #region  更新问答题库缓存
            RefreshQuestionInfoListCache();
            #endregion

            return Json(new { code = 1 });
        }

        /// <summary>
        /// 更新问答题库缓存
        /// </summary>
        /// <param name="pkid"></param>
        public void RefreshQuestionInfoListCache()
        {
            try
            {
                using (var client = new Tuhu.Service.Activity.BigBrandClient())
                {
                    var result = client.UpdateQuestionInfoList();
                    if (!result.Success)
                    {
                        Logger.Log(Level.Warning, $"更新问题库缓存失败", result.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"AnswerController -> RefreshQuestionInfoListCache ,异常信息：{ex.Message}，堆栈异常：{ex.StackTrace}");
            }
        }
    }
}
