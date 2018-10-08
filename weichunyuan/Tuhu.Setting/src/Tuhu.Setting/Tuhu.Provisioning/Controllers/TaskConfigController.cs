using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Component.Framework.Extension;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Member;
using Tuhu.Service.Push;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;
namespace Tuhu.Provisioning.Controllers
{
    public class TaskConfigController : Controller
    {
        // GET: TaskConfig
        public ActionResult Index()
        {
            var data = TaskConfigManager.GetTaskListList();
            var data2 = TaskConfigManager.GetActionList();
            return View(Tuple.Create(data, data2));
        }
        public ActionResult TaskConfig()
        {
            var data = TaskConfigManager.GetTaskListList();
            //var data2 = TaskConfigManager.GetActionList();
            return View(data);
        }
        public JsonResult FetchTaskDetail(Guid TaskId)
        {
            var data = TaskConfigManager.FetchTaskDetail(TaskId);
            if (data == null)
                return Json(new
                {
                    Code = 0,
                    Info = "未找到该任务"
                });
            return Json(new
            {
                Code = 1,
                Data = data
            });
        }
        public JsonResult EditTaskConfig(TaskConfigModel request)
        {
            var actionList = new List<string>
            {
                "0SignOn",
                "1FirstOrder",
                "2LuckyMoney",
                "4BindWX",
                "5Follow",
                "6AddCar",
                "7Authentication",
                "8Comment",
                "9TireInsurance"
            };
            var code = 1;
            var info = "操作完成";
            if (request.ConditionList.Distinct(g => g.ActionName).Count() > 1 && actionList.Intersect(request.ConditionList.Select(g => g.ActionName)).Any())
            {
                code = 0;
                info = "任务完成条件不符合要求";
            }
            else
            {
                var result = TaskConfigManager.EditTaskConfig(request, ThreadIdentity.Operator.Name);
                if (result == 0)
                {
                    code = 0;
                    info = "任务完成条件不符合要求";
                }
                if (result == 2)
                {
                    code = 2;
                    info = "更新数据库失败";
                }
                if (result == 3)
                {
                    code = 3;
                    info = "操作失败，缓存刷新失败";
                }
            }
            return Json(new
            {
                Code = code,
                Info = info
            });
        }
        public JsonResult RefreshUserCache(Guid userId)
        {
            using (var client = new TaskClient())
            {
                var result = client.RefreshTaskCache(userId);
                if (result.Success && result.Result)
                    return Json(new { Code = 1 });
                return Json(new { Code = 0 });
            }
        }

        public JsonResult GetTaskList(Guid? TaskId) => Json(TaskConfigManager.GetTaskList(TaskId));

        public JsonResult GetTaskActionList() => Json(TaskConfigManager.GetActionList());

        public JsonResult RefreshTaskCache()
        {
            using (var client = new TaskClient())
            {
                var result = client.RefreshTaskCache(Guid.Empty);
                if (result.Success && result.Result)
                    return Json(new { Code = 1 });
                return Json(new { Code = 0 });
            }
        }
        public JsonResult UploadImage()
        {
            var result = "";
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    using (var client = new FileUploadClient())
                    {
                        var res = client.UploadImage(new ImageUploadRequest()
                        {
                            Contents = buffer,
                            DirectoryName = "TaskConfig",
                            MaxHeight = 1920,
                            MaxWidth = 1920
                        });
                        if (res.Success && res.Result != null)
                        {
                            result = ImageHelper.GetImageUrl(res.Result);
                        }
                    }
                }
                catch (Exception exp)
                {
                    WebLog.LogException(exp);
                }
            }
            return Json(result);
        }

        public JsonResult DeleteTaskConfig(Guid TaskId)
        {
            if (TaskId == Guid.Empty)
            {
                return Json(new { Code = -1, Info = "参数错误" });
            }
            var result = TaskConfigManager.DeleteTaskConfig(TaskId);
            if (result > 0)
                TaskConfigManager.TaskOprLog(TaskId, ThreadIdentity.Operator.Name, "Delete", TaskId.ToString("D"));
            return Json(new
            {
                Code = result
            });
        }
        public JsonResult GetTaskOprLog(Guid TaskId)
        {
            if (TaskId == Guid.Empty) return Json(new { Code = 0, Info = "参数异常" });
            var data = TaskConfigManager.GetTaskOprLog(TaskId);
            return Json(new
            {
                Code = 1,
                Data = data
            });
        }

        public ActionResult ConfigOrderRule() => View(TaskConfigManager.GetRuleInfo());


        public JsonResult GetCategory()
        {
            var source = TaskConfigManager.SelectProductCategory().Where(s => s.ParentCategory == null || !s.ParentCategory.Any()).ToList();
            foreach (var c in source)
            {
                var children = new List<Category>();
                TaskConfigManager.childCategory(children, c);
                c.ChildrenCategory = children;
            }
            return Json(source.Select(r => new
            {
                name = r.DisplayName,
                open = false,
                title = r.CategoryName,
                url = r.NodeNo,
                children = r.ChildrenCategory.Select(c => new { name = c.DisplayName, title = c.CategoryName, url = c.NodeNo })
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditOrderRule(OrderRuleDetailModel request)
        {
            var result = TaskConfigManager.EditOrderRule(request, ThreadIdentity.Operator.Name);
            return Json(new
            {
                Code = result ? 1 : 0,
                Indo = request
            });
        }

        public JsonResult FetchOrderRuleInfo(int RuleId)
        {
            if (RuleId < 1) return Json(new { Code = 0, Info = "参数不合法" });
            var result = TaskConfigManager.FetchOrderRuleInfo(RuleId);
            var pidList = new List<OrderRuleProductModel>();
            if (result.RuleNo != 0 && result.ConditionType == 1)
            {
                var pids = result.PIDS.Split(';').Where(g => !string.IsNullOrEmpty(g));
                foreach (var pid in pids)
                {
                    var pidItem = TaskConfigManager.FetchProductNameByPID(pid);
                    if (pidItem != null) pidList.Add(pidItem);
                }
            }
            return Json(new { Code = 1, Data = result, Pids = pidList });
        }
        public JsonResult FetchProductNameByPID(string PID)
        {
            if (string.IsNullOrEmpty(PID))
            {
                return Json(new { Code = 0, Info = "未发现该PID对应产品;" });
            }
            var result = TaskConfigManager.FetchProductNameByPID(PID);
            if (result == null)
            {
                return Json(new
                {
                    Code = 0,
                    Info = "未发现该PID对应产品;"
                });
            }
            return Json(new { Code = 1, Data = result.DisplayName });
        }
        public JsonResult GetBrandsByCategory(string Categorys = "1")
        {
            var paras = Categorys.Split(';').Where(g => !string.IsNullOrEmpty(g));
            var result = new List<string>();
            if (paras != null)
            {
                foreach (var item in paras)
                {
                    var data = ProductLibraryConfigController.QueryProducts(new SeachProducts() { Category = item })?.CP_BrandList.Select(g => g.Name)?.ToList() ?? new List<string>();
                    if (data.Any()) result.AddRange(data);
                }
            }
            result = result.Where(g => !string.IsNullOrEmpty(g)).Distinct()?.ToList() ?? new List<string>();
            return Json(new { Code = result.Any() ? 1 : 0, Data = result });
        }

        public JsonResult GetAppIdList()
        {
            using (var client = new WeiXinPushClient())
            {
                var result = client.SelectWxConfigs();
                if (result.Success && result.Result != null)
                {
                    return Json(
                        new
                        {
                            Code = 1,
                            Data = result.Result.Where(g => g.Type == "WX_APP").Select(g => new
                            {
                                AppId = g.appId,
                                Name = g.name
                            })
                        });
                }
                else
                {
                    return Json(new
                    {
                        Code = 0,
                        Info = "APPID信息获取失败"
                    });
                }
            }
        }
    }
}