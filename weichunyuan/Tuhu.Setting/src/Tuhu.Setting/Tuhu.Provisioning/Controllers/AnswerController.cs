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

namespace Tuhu.Provisioning.Controllers
{
    public class AnswerController : Controller
    {
        // GET: Answer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddExport()
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
                list = db.GetEntityList<AnswerInfoListEntity>(p => p.Answer.Contains(keyword), pagination);
            }
            else
            {
                list = db.GetEntityList<AnswerInfoListEntity>((en) => en.IsEnabled == (showEnabledType == 1) && en.Answer.Contains(keyword), pagination);
            }

            return Content(JsonConvert.SerializeObject(new
            {
                rows = list,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            }));
        }

        public ActionResult DeleteAnswer(List<int> pkids)
        {
            string msg = "ok";
            try
            {
                RepositoryManager db = new RepositoryManager();
                db.Delete<AnswerInfoListEntity>(p => pkids.Contains(p.PKID));
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

        public JsonResult UpdateIsEnabled(string ids, bool enabled)
        {
            RepositoryManager manager = new RepositoryManager();
            using (var db = manager.BeginTrans())
            {
                var list = db.FindList<AnswerInfoListEntity>($"SELECT * FROM Configuration.dbo.AnswerInfoList WITH(NOLOCK) WHERE PKID IN ({ids})");
                foreach (var item in list)
                {
                    item.IsEnabled = enabled;
                    db.Update(item);
                }
                db.Commit();
            }
            return Json(new { code = 1 });
        }
    }
}
