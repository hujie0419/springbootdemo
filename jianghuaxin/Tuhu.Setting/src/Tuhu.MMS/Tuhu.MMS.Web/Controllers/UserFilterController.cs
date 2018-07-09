using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Common.Logging;
using Tuhu.MMS.Web.Domain.UserFilter;
using Tuhu.MMS.Web.Service;

namespace Tuhu.MMS.Web.Controllers
{
    public class UserFilterController : BaseController
    {
        private readonly ILog logger;

        public UserFilterController(ILog logger)
        {
            this.logger = logger;
        }
        // GET: UserFilter
        public async Task<ActionResult> Index()
        {
            var jobs = await Service.UserFilterService.SelectUserFilterRuleJobsAsync();
            return View(jobs);
        }

        public async Task<ActionResult> SelectFilterRuleOutResult(int? jobid)
        {
            if (!jobid.HasValue)
            {
                var job = new UserFilterRuleJob()
                {
                    JobName = "会员筛选任务",
                    JobStatus = JobStatus.Created,
                    Description = "会员筛选任务",
                    CreateUser = CurrentUser.Name,
                    ModifyUser = CurrentUser.Name,
                    PreviewStatus = PreviewStatus.Created
                };
                var pkid = await UserFilterService.InsertUserFilterRuleJobAsync(job);
                return RedirectToAction("SelectFilterRuleOutResult", new { jobid = pkid });
            }
            var userselectconfigs = await UserFilterService.SelectUserFilterResultConfigAsync(jobid.Value);
            ViewBag.userselectconfigs = userselectconfigs;
            ViewData["jobid"] = jobid.Value;
            return View();
        }
        public async Task<ActionResult> EditUserFilterRule(int? jobid)
        {
            if (!jobid.HasValue)
            {
                var temp = new UserFilterRuleJob()
                {
                    JobName = "会员筛选任务",
                    JobStatus = JobStatus.Created,
                    Description = "会员筛选任务",
                    CreateUser = CurrentUser.Name,
                    ModifyUser = CurrentUser.Name,
                    PreviewStatus = PreviewStatus.Created
                };
                var pkid = await UserFilterService.InsertUserFilterRuleJobAsync(temp);
                return RedirectToAction("EditUserFilterRule", new { jobid = pkid });
            }
            var configs = await UserFilterService.SelectAllUserFilterValueConfigAsync();
            var job = await Service.UserFilterService.SelectUserFilterRuleJobsAsync(jobid.Value);
            ViewBag.configs = configs;
            ViewBag.job = job;
            ViewBag.jobid = jobid.Value;
            return View();
        }

        private string GetJoinTypeDescriptionValue(JoinType type)
        {
            switch (type)
            {
                case JoinType.And:
                    return "且";
                case JoinType.Except:
                    return "排除";
                case JoinType.Or:
                    return "或";
                default:
                    return "";
            }
        }

        private string GetCompareTypeDescriptionValue(CompareType type)
        {
            switch (type)
            {
                case CompareType.DateFromToday:
                    return "距今天天数(不含今天)";
                case CompareType.Equal:
                    return "等于";
                case CompareType.Greater:
                    return "大于";
                case CompareType.GreaterOrEqual:
                    return "大于等于";
                case CompareType.Less:
                    return "小于";
                case CompareType.LessOrEqual:
                    return "小于等于";
                default:
                    return "";
            }
        }

        private string GetTableColName(string basecategory, string secondcategory, string colname)
        {
            var config = UserFilterService.UserFilterColConfigs.FirstOrDefault(x =>
                x.BaseCategory == basecategory && x.SecnodCategory == secondcategory);
            var value = config?.TableColConfig?.FirstOrDefault(x => x.ColName == colname)?.TableColName;
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
            return colname;
        }

        private string GetTableColName(string basecategory, string colname)
        {
            var config = UserFilterService.UserFilterColConfigs.FirstOrDefault(x =>
                 x.BaseCategory == basecategory && x.TableColConfig.Any(s => s.ColName == colname));
            var value = config?.TableColConfig?.FirstOrDefault(x => x.ColName == colname)?.TableColName;
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
            return colname;
        }

        public async Task<JsonResult> SetJobStartRunAsync(int jobid, string forceupdate = null)
        {
            JsonResult jr = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var job = await Service.UserFilterService.SelectUserFilterRuleJobsAsync(jobid);
            if (job.JobStatus == JobStatus.Created || string.Equals(forceupdate, "forceupdate"))
            {
                var result = await UserFilterService.SaveJobRunStatusAsync(jobid, JobStatus.WaittingForRun);
                jr.Data = result ? new { code = 1, msg = "提交成功" } : new { code = 0, msg = "提交失败" };
            }
            else
            {
                jr.Data = new { code = 0, msg = "该job已经开始执行或等待执行" };
            }
            return jr;
        }
        public async Task<JsonResult> SelectFilterRuleJobDetailsAsync(int? jobid)
        {
            JsonResult jr = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            if (jobid.HasValue)
            {
                var details = await Service.UserFilterService.SelectUserFilterRuleJobDetailsAsync(jobid.Value);
                if (details != null && details.Any())
                {
                    var datas = details.GroupBy(x => new { x.BasicAttribute, x.SecondAttribute, x.JoinType, x.BatchID }).Select(x => new
                    {
                        category = x.Key,
                        datas = x
                    });
                    List<object> returndata = new List<object>();
                    foreach (var data in datas)
                    {
                        string values = string.Join(",  ", data.datas.Select(x => $"{GetTableColName(data.category.BasicAttribute, data.category.SecondAttribute, x.SearchKey)}-{GetCompareTypeDescriptionValue(x.CompareType)}-{x.SearchValue}"));
                        returndata.Add(new
                        {
                            BasicAttribute = UserFilterService.FirstCategoryNameMap.ContainsKey(data.category.BasicAttribute) ? UserFilterService.FirstCategoryNameMap[data.category.BasicAttribute] : "",
                            SecondAttribute = UserFilterService.SecondCategoryNameMap.ContainsKey(data.category.SecondAttribute) ? UserFilterService.SecondCategoryNameMap[data.category.SecondAttribute] : "",
                            JoinType = GetJoinTypeDescriptionValue(data.category.JoinType),
                            values = values,
                            BatchID = data.category.BatchID
                        });
                    }
                    jr.Data = new
                    {
                        code = 1,
                        datas = returndata
                    };
                    return jr;
                }
                jr.Data = new
                {
                    code = -1,
                };
                return jr;


            }
            jr.Data = new
            {
                code = 0,
                msg = "没有数据"
            };
            return jr;
        }


        public async Task<JsonResult> DeleteFilterRuleJobDetailAsync(string batchid)
        {
            JsonResult jr = new JsonResult();
            var result = await UserFilterService.DeleteFilterRuleJobDetailAsync(batchid);
            jr.Data = new { code = result > 0 ? 1 : 0, msg = result > 0 ? "删除成功" : "删除失败.." };
            return jr;
        }
        public async Task<JsonResult> SubmitJobDetail(string FirstCategory, string SecondCategory, string TableName, JoinType JoinType, int JobId)
        {
            JsonResult jr = new JsonResult();
            try
            {
                await Service.UserFilterService.InsertUserFilterRuleDetailFromWebAsync(FirstCategory, SecondCategory,
                    JoinType, TableName, Request.Form, JobId);
                jr.Data = new { code = 1, msg = "保存成功" };
            }
            catch (System.Exception ex)
            {
                logger.Error(ex);
                jr.Data = new { code = 0, msg = ex.Message };
            }
            return jr;
        }

        public async Task<ContentResult> SelectUserFilterValueConfigsAsync(string type, string value)
        {

            IEnumerable<UserFilterValueConfig> data = new List<UserFilterValueConfig>();
            if (type == "colname")
            {
                data = await UserFilterService.SelectUserFilterValueConfigByColNameAsync(value);
            }
            if (type == "parentvalue")
            {
                data = await UserFilterService.SelectUserFilterValueConfigByParentValueAsync(value);
            }
            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            var result = new ContentResult
            {
                Content = serializer.Serialize(data),
                ContentType = "application/json"
            };
            return result;
        }

        public async Task<JsonResult> SubmitUserFilterResultConfigAsync(string basecategory, string tablename, string colname, int jobid, bool ischecked)
        {
            JsonResult jr = new JsonResult();
            if (ischecked)
            {
                var count = await UserFilterService.SelectUserFilterResultConfigCountAsync(jobid);
                if (count >= 6)
                {
                    jr.Data = new { code = 0, msg = "筛选结果字段不能大于6个" };
                    return jr;
                }
            }
            var config = new UserFilterResultConfig()
            {
                BasicAttribute = basecategory,
                ColName = colname,
                TableName = tablename,
                JobId = jobid
            };
            var result = await UserFilterService.InsertOrUpdateUserFilterResultConfigAsync(config);
            jr.Data = new { code = 1, msg = "保存成功", data = result };
            return jr;
        }


        public async Task<JsonResult> SaveJobDescriptionAsync(int jobid, string description)
        {
            JsonResult jr = new JsonResult();
            var result = await UserFilterService.SaveJobDescriptionAsync(jobid, description);
            jr.Data = new { code = 1, msg = "保存成功", data = result };
            return jr;
        }
        public async Task<JsonResult> SelectUserFilterResultConfigAsync(int jobid)
        {
            JsonResult jr = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var result = await UserFilterService.SelectUserFilterResultConfigAsync(jobid);
            if (result != null && result.Any())
            {
                var datas = result.Select(x => new
                {
                    x.ColName,
                    x.BasicAttribute,
                    x.JobId,
                    x.PKID,
                    x.TableName,
                    TableColName = GetTableColName(x.BasicAttribute, x.ColName)
                }).ToList();
                jr.Data = new { code = 1, msg = "", data = datas };
                return jr;
            }
            jr.Data = new { code = 1, msg = "", data = result };
            return jr;
        }
    }
}