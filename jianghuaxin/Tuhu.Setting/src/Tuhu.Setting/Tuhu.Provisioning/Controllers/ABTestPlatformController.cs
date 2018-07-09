using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Service.Config.Models;
using Tuhu.Provisioning.Business.ABTestPlatform;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.DataAccess.Entity.ABTestPlatform;

namespace Tuhu.Provisioning.Controllers
{
    public class ABTestPlatformController : Controller
    {
        // GET: ABTestPlatform
        [PowerManage]
        public ActionResult Index()
        {
            ABTestPlatformManager manager = new ABTestPlatformManager();
            var list = manager.GetAllTestList();
            if (list == null)
                return View();
            return View(list);
        }

        public ActionResult Add()
        {
            ABTestPlatformManager manager = new ABTestPlatformManager();
            var list = manager.GetAllTestName();
            ViewBag.NameList = list;
            return View();
        }

        public JsonResult AddABTest(string jsonstr, string testName, string testScale)
        {
            bool flag = false;

            ABTestDetail test = new ABTestDetail
            {
                CreateTime = DateTime.Now,
                Creator = ThreadIdentity.Operator.Name,
                GroupList = new List<ABTestGroupDetail>(),
                LastUpdateDataTime = DateTime.Now,
                Status = "Processing",
                TestGuid = Guid.NewGuid(),
                TestName = testName,
                TestScale = Convert.ToDouble(testScale) / 100
            };
            List<ABTestGroupDetail> groups = new List<ABTestGroupDetail>();

            JArray o = (JArray)JsonConvert.DeserializeObject(jsonstr);
            IList<JToken> oList = (IList<JToken>)o;
            foreach (JToken jt in oList)
            {
                JObject jo = jt as JObject;
                groups.Add(new ABTestGroupDetail
                {
                    CreateTime = DateTime.Now,
                    LastUpdateDataTime = DateTime.Now,
                    TestGuid = test.TestGuid,
                    Selected = false,
                    GroupId = jo["GroupId"].ToString(),
                    GroupName = jo["GroupName"].ToString()
                });
            }

            test.GroupNum = groups.Count();
            test.GroupList = groups;

            ABTestPlatformManager manager = new ABTestPlatformManager();
            flag = manager.CreateABTest(test);

            #region 插入日志
            ABTestEditLog log = new ABTestEditLog
            {
                TestGuid = test.TestGuid,
                TestName = test.TestName,
                Change = "新建测试",
                Operator = ThreadIdentity.Operator.Name,
                CreateTime = DateTime.Now,
                LastUpdateDataTime = DateTime.Now
            };
            if (flag)
                InsertLog(log);
            #endregion

            return Json(flag);
        }

        public ActionResult Edit(string testGuid)
        {
            ABTestPlatformManager manager = new ABTestPlatformManager();
            var test = manager.GetABTestDetailByGuid(new Guid(testGuid));
            return View(test);
        }

        public JsonResult EditABTest(string jsonstr, string testGuid, string testScale)
        {
            bool flag = false;

            ABTestPlatformManager manager = new ABTestPlatformManager();
            var oldTest = manager.GetABTestDetailByGuid(new Guid(testGuid));

            ABTestDetail test = new ABTestDetail
            {
                CreateTime = DateTime.Now,
                LastUpdateDataTime = DateTime.Now,
                TestGuid = new Guid(testGuid),
                GroupList = new List<ABTestGroupDetail>(),
                TestScale = Convert.ToDouble(testScale) / 100
            };
            List<ABTestGroupDetail> groups = new List<ABTestGroupDetail>();

            JArray o = (JArray)JsonConvert.DeserializeObject(jsonstr);
            IList<JToken> oList = (IList<JToken>)o;
            foreach (JToken jt in oList)
            {
                JObject jo = jt as JObject;
                groups.Add(new ABTestGroupDetail
                {
                    CreateTime = DateTime.Now,
                    LastUpdateDataTime = DateTime.Now,
                    TestGuid = test.TestGuid,
                    GroupId = jo["GroupId"].ToString(),
                    ExceptData = jo["ExceptData"].ToString()
                });
            }
            test.GroupNum = groups.Count();
            test.GroupList = groups;

            flag = manager.UpdateABTest(test);

            #region 插入日志
            if (flag)
            {
                var change = CompareChange(oldTest, test);
                ABTestEditLog log = new ABTestEditLog()
                {
                    TestGuid = oldTest.TestGuid,
                    TestName = oldTest.TestName,
                    Change = change,
                    Operator = ThreadIdentity.Operator.Name,
                    CreateTime = DateTime.Now,
                    LastUpdateDataTime = DateTime.Now
                };
                InsertLog(log);
            }
            #endregion

            return Json(flag);
        }

        public ActionResult Delete(string testGuid)
        {
            if (string.IsNullOrWhiteSpace(testGuid))
                return Json(0);

            ABTestPlatformManager manager = new ABTestPlatformManager();
            var oldTest = manager.GetABTestDetailByGuid(new Guid(testGuid));

            if (manager.DeleteABTest(new Guid(testGuid), oldTest.TestName))
            {
                #region 插入日志
                ABTestEditLog log = new ABTestEditLog
                {
                    TestGuid = oldTest.TestGuid,
                    TestName = oldTest.TestName,
                    Change = "删除测试",
                    Operator = ThreadIdentity.Operator.Name,
                    CreateTime = DateTime.Now,
                    LastUpdateDataTime = DateTime.Now
                };
                InsertLog(log);
                #endregion
                return Json(1);
            }
            else
                return Json(0);
        }

        public ActionResult SelectDone(string testGuid)
        {
            ABTestPlatformManager manager = new ABTestPlatformManager();
            var test = manager.GetABTestDetailByGuid(new Guid(testGuid));
            test.GroupList = test.GroupList.Where(item => item.Selected == true).ToList();
            return View(test);
        }

        public ActionResult SelectProcessing(string testGuid)
        {
            ABTestPlatformManager manager = new ABTestPlatformManager();
            var test = manager.GetABTestDetailByGuid(new Guid(testGuid));
            return View(test);
        }

        public JsonResult SelectABTestResult(string testGuid, string testName, string status, string testGroupId)
        {
            bool flag = false;
            if (string.IsNullOrWhiteSpace(testGuid) ||
                string.IsNullOrWhiteSpace(status) ||
                status != "Done" ||
                string.IsNullOrWhiteSpace(testGroupId))
                return Json(false);

            ABTestPlatformManager manager = new ABTestPlatformManager();
            flag = manager.SelectABTestResult(testGuid, testName, status, testGroupId);

            #region 插入日志
            ABTestEditLog log = new ABTestEditLog
            {
                TestGuid = new Guid(testGuid),
                TestName = testName,
                Change = "选择测试结果,关闭测试",
                Operator = ThreadIdentity.Operator.Name,
                CreateTime = DateTime.Now,
                LastUpdateDataTime = DateTime.Now
            };
            if (flag)
                InsertLog(log);
            #endregion

            return Json(flag);
        }

        public void InsertLog(ABTestEditLog log)
        {
            ABTestPlatformManager manager = new ABTestPlatformManager();
            manager.InsertLog(log);
        }

        public string CompareChange(ABTestDetail oldTest, ABTestDetail newTest)
        {
            string result = string.Empty;
            if (oldTest.TestScale != (newTest.TestScale * 100))
            {
                result += "总分配比例:" + oldTest.TestScale + "->" + newTest.TestScale * 100 + ";";
            }
            for (int i = 0; i < oldTest.GroupList.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(oldTest.GroupList[i].ExceptData) && !string.IsNullOrWhiteSpace(newTest.GroupList[i].ExceptData))
                    result +=
                        "分组" +
                        oldTest.GroupList[i].GroupId +
                        ": null->" +
                        newTest.GroupList[i].ExceptData +
                        ";";
                if (!string.IsNullOrWhiteSpace(oldTest.GroupList[i].ExceptData) && string.IsNullOrWhiteSpace(newTest.GroupList[i].ExceptData))
                    result +=
                        "分组" +
                        oldTest.GroupList[i].GroupId +
                        ": " +
                        oldTest.GroupList[i].ExceptData +
                        "->null;";
                if (!string.IsNullOrWhiteSpace(oldTest.GroupList[i].ExceptData)
                    && !string.IsNullOrWhiteSpace(newTest.GroupList[i].ExceptData)
                    && oldTest.GroupList[i].ExceptData != newTest.GroupList[i].ExceptData)
                    result +=
                        "分组" +
                        oldTest.GroupList[i].GroupId +
                        ": " +
                        oldTest.GroupList[i].ExceptData +
                        "->" +
                        newTest.GroupList[i].ExceptData + ";";
            }

            return result;
        }

        public ActionResult CheckLog(string testGuid)
        {
            ABTestPlatformManager manager = new ABTestPlatformManager();
            var log = manager.GetLog(new Guid(testGuid));
            return View(log);
        }
    }
}