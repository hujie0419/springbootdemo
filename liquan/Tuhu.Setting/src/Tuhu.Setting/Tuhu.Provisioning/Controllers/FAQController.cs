using NPOI.SS.UserModel;
using Qiniu.IO;
using Qiniu.RS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class FAQController : Controller
    {
        private readonly IFAQManager manager = new FAQManager();
        public ActionResult FAQImport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FAQImportTo()
        {
            string _ReturnStr = string.Empty;
            try
            {
                var _FAQList = AllFAQList;
                HttpPostedFileBase file = Request.Files[0];
                string _Path = Path.Combine(HttpContext.Server.MapPath("~/Content/Upload/"), "FAQImport.xls");
                file.SaveAs(_Path);
                int _SuccessCount = 0;
                int _AllCount = 0;
                using (FileStream fs = new FileStream(_Path, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook wb = WorkbookFactory.Create(fs);
                    ISheet sheet1 = wb.GetSheetAt(0);
                    _AllCount = sheet1.LastRowNum;
                    for (int i = 1; i < sheet1.LastRowNum + 1; i++)
                    {
                        try
                        {
                            IRow _Row = sheet1.GetRow(i);
                            FAQ _FAQ = new FAQ
                            {
                                Orderchannel = _Row.GetCell(1).ToString(),
                                CateOne = _Row.GetCell(2).ToString(),
                                CateTwo = _Row.GetCell(3).ToString(),
                                CateThree = _Row.GetCell(4).ToString(),
                                Question = _Row.GetCell(5).ToString(),
                                Answer = _Row.GetCell(6).ToString()
                            };
                            //if (_FAQList.Where(p =>
                            //	p.Orderchannel.Equals(_FAQ.Orderchannel) &&
                            //	p.CateOne.Equals(_FAQ.CateOne) &&
                            //	p.CateTwo.Equals(_FAQ.CateTwo) &&
                            //	p.CateThree.Equals(_FAQ.CateThree) &&
                            //	p.Question.Equals(_FAQ.Question)
                            //	)
                            //	.FirstOrDefault() == null)
                            //{
                            //	manager.Add(_FAQ);
                            //	_SuccessCount++;
                            //}

                            manager.Add(_FAQ);
                            _SuccessCount++;
                        }
                        catch { }
                    }
                    System.IO.File.Delete(_Path);
                    System.Web.Caching.Cache objCache = System.Web.HttpRuntime.Cache;
                    if (objCache["AllFAQList"] != null)
                    {
                        objCache.Remove("AllFAQList");
                    }
                    _ReturnStr = "导入成功！总条数：" + _AllCount + ",有效条数：" + _SuccessCount;
                }
            }
            catch (Exception ex)
            {
                _ReturnStr = "导入失败！原因：" + ex.Message + ":请参照订单模板数据！";
            }
            return Content("<script>alert('" + _ReturnStr + "');location='/FAQ/FAQManage'</script>");
        }

        public ActionResult FAQManage()
        {
            IEnumerable<FAQ> _AllFAQList = AllFAQList;
            ViewBag.OrderchannelList = _AllFAQList.Select(p => p.Orderchannel).Distinct().Select(p => new SelectListItem { Text = p.ToString(), Value = p.ToString() });
            ViewBag.CateOneList = _AllFAQList.Select(p => p.CateOne).Distinct().Select(p => new SelectListItem { Text = p.ToString(), Value = p.ToString() });
            ViewBag.CateTwoList = _AllFAQList.Select(p => p.CateTwo).Distinct().Select(p => new SelectListItem { Text = p.ToString(), Value = p.ToString() });
            ViewBag.CateThreeList = _AllFAQList.Select(p => p.CateThree).Distinct().Select(p => new SelectListItem { Text = p.ToString(), Value = p.ToString() });
            return View();
        }

        [HttpPost]
        public JsonResult FAQManage(string Orderchannel, string CateOne, string CateTwo, string CateThree, string Question, int? PageIndex)
        {
            IEnumerable<FAQ> _AllFAQList = AllFAQList;
            if (!string.IsNullOrEmpty(Orderchannel))
            {
                _AllFAQList = _AllFAQList.Where(p => p.Orderchannel == Orderchannel);
            }
            if (!string.IsNullOrEmpty(CateOne))
            {
                _AllFAQList = _AllFAQList.Where(p => p.CateOne == CateOne);
            }
            if (!string.IsNullOrEmpty(CateTwo))
            {
                _AllFAQList = _AllFAQList.Where(p => p.CateTwo == CateTwo);
            }
            if (!string.IsNullOrEmpty(CateThree))
            {
                _AllFAQList = _AllFAQList.Where(p => p.CateThree == CateThree);
            }
            if (!string.IsNullOrEmpty(Question))
            {
                string[] str = Question.Split(' ');
                foreach (var item in str)
                {
                    _AllFAQList = _AllFAQList.Where(m => (m.Question ?? string.Empty).Contains(item));
                }
            }
            var cou = _AllFAQList.Count();
            if (cou > 0)
            {
                int _PageIndex = PageIndex.GetValueOrDefault(1);
                int _PageSize = 30;
                _AllFAQList = _AllFAQList.Skip(_PageSize * (_PageIndex - 1)).Take(_PageSize);
            }
            return Json(new { FAQList = _AllFAQList }, JsonRequestBehavior.AllowGet);
        }
        private object lockobj = new object();
        public List<FAQ> AllFAQList
        {
            get
            {
                List<FAQ> _AllFAQList = new List<FAQ>();
                System.Web.Caching.Cache objCache = System.Web.HttpRuntime.Cache;
                if (objCache["AllFAQList"] == null)
                {
                    lock (lockobj)
                    {
                        try
                        {
                            _AllFAQList = manager.SelectAll();
                            DateTime _ExpirationTime = DateTime.Now.AddMonths(1);
                            objCache.Insert("AllFAQList", _AllFAQList, null, _ExpirationTime, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, null);
                        }
                        catch { }
                    }
                }
                else
                {
                    _AllFAQList = (List<FAQ>)objCache["AllFAQList"];
                }
                return _AllFAQList;
            }
        }
        [HttpPost]
        public ActionResult DeleteFAQ(int PKID)
        {
            try
            {
                manager.Delete(PKID);
                System.Web.Caching.Cache objCache = System.Web.HttpRuntime.Cache;
                if (objCache["AllFAQList"] != null)
                {
                    objCache.Remove("AllFAQList");
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddFAQ(int? PKID)
        {
            int _PKID = PKID.GetValueOrDefault(0);
            if (_PKID != 0)
            {
                ViewBag.Title = "修改FAQ";
                return View(manager.GetByPKID(_PKID));
            }
            else
            {
                ViewBag.Title = "新增FAQ";
                FAQ _Model = new FAQ()
                {
                    PKID = 0
                };
                return View(_Model);
            }
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddFAQ(FAQ fAQ)
        {
            if (fAQ.PKID == 0)
            {
                manager.Add(fAQ);
            }
            else
            {
                manager.Update(fAQ);
            }
            System.Web.Caching.Cache objCache = System.Web.HttpRuntime.Cache;
            if (objCache["AllFAQList"] != null)
            {
                objCache.Remove("AllFAQList");
            }
            return Redirect("FAQManage");
        }

        [HttpPost]
        public ActionResult MultiOperate(string opstr)
        {
            try
            {
                if (string.IsNullOrEmpty(opstr))
                {
                    return Json(new { IsSuccess = false, ReturnStr = "输入的参数不能为空" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string[] _Oparr = opstr.Split(',');
                    foreach (string Paras in _Oparr)
                    {
                        try
                        {
                            int _PKID = int.Parse(Paras);
                            manager.Delete(_PKID);
                        }
                        catch { }
                    }
                    System.Web.Caching.Cache objCache = System.Web.HttpRuntime.Cache;
                    if (objCache["AllFAQList"] != null)
                    {
                        objCache.Remove("AllFAQList");
                    }
                }
                return Json(new { IsSuccess = true, ReturnStr = "批量删除成功" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { IsSuccess = false, ReturnStr = "保存失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult AddFile(string prefix)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    try
                    {
                        string _FileExt = ".jpg";
                        try
                        {
                            var _KFileName = file.FileName;
                            _FileExt = _KFileName.Substring(_KFileName.LastIndexOf("."), _KFileName.Length - _KFileName.LastIndexOf("."));
                        }
                        catch { }
                        string fileName = prefix + "_" + DateTime.Now.ToString("yyMMddHHmmssfff") + _FileExt;
                        var stream = file.InputStream;
                        int retryTimes = 5;
                        while (retryTimes > 0)
                        {
                            //重试5次
                            retryTimes--;
                            stream.Seek(0, SeekOrigin.Begin);
                            IOClient _IOClient = new IOClient();
                            var _PutPolicy = new PutPolicy(WebConfigurationManager.AppSettings["Qiniu:comment_scope"], 3600).Token();
                            var _Result = _IOClient.Put(_PutPolicy, fileName, stream, new PutExtra());
                            if (_Result.OK)
                                return Json(new { error = 0, url = WebConfigurationManager.AppSettings["Qiniu:comment_url"] + fileName });
                        }
                    }
                    catch { }
                    //catch (Exception ex) { }
                }
            }
            return Json(new { error = 1, url = "" });

        }


        #region  投诉知识库操作

        public ActionResult TousuFAQManage()
        {
            IEnumerable<FAQ> _AllFAQList = TousuAllFAQList;
            ViewBag.OrderchannelList = _AllFAQList.Select(p => p.Orderchannel).Distinct().Select(p => new SelectListItem { Text = p.ToString(), Value = p.ToString() });
            ViewBag.CateOneList = _AllFAQList.Select(p => p.CateOne).Distinct().Select(p => new SelectListItem { Text = p.ToString(), Value = p.ToString() });
            ViewBag.CateTwoList = _AllFAQList.Select(p => p.CateTwo).Distinct().Select(p => new SelectListItem { Text = p.ToString(), Value = p.ToString() });
            ViewBag.CateThreeList = _AllFAQList.Select(p => p.CateThree).Distinct().Select(p => new SelectListItem { Text = p.ToString(), Value = p.ToString() });
            return View();
        }
        [HttpPost]
        public JsonResult TousuFAQManage(string Orderchannel, string CateOne, string CateTwo, string CateThree, string Question, int? PageIndex)
        {
            IEnumerable<FAQ> _AllFAQList = TousuAllFAQList;
            if (!string.IsNullOrEmpty(Orderchannel))
            {
                _AllFAQList = _AllFAQList.Where(p => p.Orderchannel == Orderchannel);
            }
            if (!string.IsNullOrEmpty(CateOne))
            {
                _AllFAQList = _AllFAQList.Where(p => p.CateOne == CateOne);
            }
            if (!string.IsNullOrEmpty(CateTwo))
            {
                _AllFAQList = _AllFAQList.Where(p => p.CateTwo == CateTwo);
            }
            if (!string.IsNullOrEmpty(CateThree))
            {
                _AllFAQList = _AllFAQList.Where(p => p.CateThree == CateThree);
            }
            if (!string.IsNullOrEmpty(Question))
            {
                string[] str = Question.Split(' ');
                foreach (var item in str)
                {
                    _AllFAQList = _AllFAQList.Where(m => (m.Question ?? string.Empty).Contains(item));
                }
            }
            var cou = _AllFAQList.Count();
            if (cou > 0)
            {
                int _PageIndex = PageIndex.GetValueOrDefault(1);
                int _PageSize = 30;
                _AllFAQList = _AllFAQList.Skip(_PageSize * (_PageIndex - 1)).Take(_PageSize);
            }
            return Json(new { FAQList = _AllFAQList }, JsonRequestBehavior.AllowGet);
        }

        public List<FAQ> TousuAllFAQList
        {
            get
            {
                List<FAQ> _AllFAQList = new List<FAQ>();
                System.Web.Caching.Cache objCache = System.Web.HttpRuntime.Cache;
                if (objCache["AllTousuFAQList"] == null)
                {
                    lock (lockobj)
                    {
                        try
                        {
                            _AllFAQList = manager.TousuFaqSelectAll();
                            DateTime _ExpirationTime = DateTime.Now.AddMonths(1);
                            objCache.Insert("AllTousuFAQList", _AllFAQList, null, _ExpirationTime, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, null);
                        }
                        catch { }
                    }
                }
                else
                {
                    _AllFAQList = (List<FAQ>)objCache["AllTousuFAQList"];
                }
                return _AllFAQList;
            }
        }
        [HttpPost]
        public ActionResult TousuDeleteFAQ(int PKID)
        {
            try
            {
                manager.TousuFaqDelete(PKID);
                System.Web.Caching.Cache objCache = System.Web.HttpRuntime.Cache;
                if (objCache["AllTousuFAQList"] != null)
                {
                    objCache.Remove("AllTousuFAQList");
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult TousuAddFAQ(int? PKID)
        {
            int _PKID = PKID.GetValueOrDefault(0);
            ViewBag.Orderchannel = new[] { new SelectListItem() { Text = "-组内FAQ-", Value = "组内FAQ" }, new SelectListItem() { Text = "-产品FAQ-", Value = "产品FAQ" } };
            if (_PKID != 0)
            {
                ViewBag.Title = "修改FAQ";
                return View(manager.TousuFaqGetByPKID(_PKID));
            }
            else
            {
                ViewBag.Title = "新增FAQ";
                FAQ _Model = new FAQ()
                {
                    PKID = 0
                };
                return View(_Model);
            }
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult TousuAddFAQ(FAQ fAQ)
        {
            if (fAQ.PKID == 0)
            {
                manager.TousuFaqAdd(fAQ);
            }
            else
            {
                manager.TousuFaqUpdate(fAQ);
            }
            System.Web.Caching.Cache objCache = System.Web.HttpRuntime.Cache;
            if (objCache["AllTousuFAQList"] != null)
            {
                objCache.Remove("AllTousuFAQList");
            }
            return Redirect("TousuFAQManage");
        }

        [HttpPost]
        public ActionResult TousuMultiOperate(string opstr)
        {
            try
            {
                if (string.IsNullOrEmpty(opstr))
                {
                    return Json(new { IsSuccess = false, ReturnStr = "输入的参数不能为空" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string[] _Oparr = opstr.Split(',');
                    foreach (string Paras in _Oparr)
                    {
                        try
                        {
                            int _PKID = int.Parse(Paras);
                            manager.TousuFaqDelete(_PKID);
                        }
                        catch { }
                    }
                    System.Web.Caching.Cache objCache = System.Web.HttpRuntime.Cache;
                    if (objCache["AllTousuFAQList"] != null)
                    {
                        objCache.Remove("AllTousuFAQList");
                    }
                }
                return Json(new { IsSuccess = true, ReturnStr = "批量删除成功" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { IsSuccess = false, ReturnStr = "保存失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        /// <summary>
        /// 加载活动信息列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ActivityIntroduction()
        {
            return View();
        }
        public ActionResult ActivityIntroductionList(string activityName, int pageIndex = 1)
        {
            string filter = "";
            if (!string.IsNullOrWhiteSpace(activityName))
            {
                filter = " and ActivityName LIKE N'%" + activityName + @"%'";
            }
            List<ActivityIntroductionModel> list =
                new FAQManager().GetAllActivityIntroductionList(filter, pageIndex, 20);
            return View(list);
        }

        /// <summary>
        /// 添加活动介绍
        /// </summary>
        /// <returns></returns>
        [PowerManage]
        public ActionResult AddActivityIntroduction(int ID)
        {
            ActivityIntroductionModel activity = new ActivityIntroductionModel();
            if (ID > 0)
            {
                activity = new FAQManager().GetActivityIntroductionById(ID);
            }
            if (activity == null)
            {
                activity = new ActivityIntroductionModel();
            }
            return View(activity);
        }

        public ActionResult ActivityLook(int ID)
        {
            ActivityIntroductionModel activity = new ActivityIntroductionModel();
            activity = new FAQManager().GetActivityIntroductionById(ID);
            return View(activity);
        }

        /// <summary>
        /// 修改/新增活动信息
        /// </summary>
        /// <param name="activityName"></param>
        /// <param name="orderChannel"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="activityContent"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult AddOrUpdateActivityIntroduction(string activityName, string orderChannel, string startTime, string endTime, string activityContent, int ID)
        {
            ActivityIntroductionModel activity = new ActivityIntroductionModel();
            activity.ID = ID;
            activity.ActivityName = System.Web.HttpUtility.UrlDecode(activityName);
            activity.ActivityChannel = System.Web.HttpUtility.UrlDecode(orderChannel);
            activity.ActivityContent = System.Web.HttpUtility.UrlDecode(activityContent);
            activity.StartTime = Convert.ToDateTime(startTime);
            activity.EndTime = Convert.ToDateTime(endTime);
            activity.CreateUser = User.Identity.Name;
            int returnVal = new FAQManager().AddOrUpActivityIntroduction(activity, "");
            return Json(returnVal);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult DeleteActivityIntroduction(int ID)
        {
            int returnVal = new FAQManager().DeleteActivityIntroductionById(ID);
            return Json(returnVal);
        }
    }
}