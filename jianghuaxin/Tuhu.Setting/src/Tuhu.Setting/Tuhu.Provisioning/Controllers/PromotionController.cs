using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.ExportImport;
using Tuhu.Provisioning.Business.Promotion;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Member;
using Tuhu.Provisioning.Common;
using Newtonsoft.Json.Linq;
using Tuhu.Provisioning.Models;
using System.Web;
using System.Web.Caching;
using Microsoft.ApplicationBlocks.Data;
using NPOI.SS.UserModel;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.Models.Push;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.Business.ProductInfomationManagement;
using Tuhu.Provisioning.Business.PromotionCodeManagerment;
using Tuhu.Provisioning.Business.DictionariesManagement;
using Tuhu.Provisioning.Business;
using Tuhu.Service.ConfigLog;
using Tuhu.Provisioning.Business.ConfigLog;

namespace Tuhu.Provisioning.Controllers
{
    public class PromotionController : Controller
    {
        private const string key = "TuhuProductCategories";
        private const string ChannelKey = "TuhuOrderChannelDictionaries";
        //
        // GET: /Promotion/
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult Index(PromotionFilterConditionModel model)
        {
            var data = PromotionManager.SelectAllPromotion(model);
            ViewBag.Condition = model;
            return View(data);
        }
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult AddPromotion(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return View();

            var PKID = Convert.ToInt32(id);
            var model = PromotionManager.SelectPromotionDetail(PKID);
            return View(model);

        }
        public ActionResult GetCouponRule(string id, string condition, PromotionFilterConditionModel model)
        {
            if (!string.IsNullOrWhiteSpace(condition))
                model = JsonConvert.DeserializeObject<PromotionFilterConditionModel>(condition);
            ViewBag.Condition = model;
            if (string.IsNullOrWhiteSpace(id))
                return View();

            var PKID = Convert.ToInt32(id);
            var result = PromotionManager.SelectGeCouponRulesByCondition(PKID, model);
            var result2 = PromotionManager.SelectGeCouponRulesByRuleID(PKID).FirstOrDefault();
            var result3 = new List<DepartmentAndUse>();
            var SettingTemp = PromotionManager.GetDepartmentUseSetting();
            if (SettingTemp != null && SettingTemp.Rows.Count > 0)
            {
                result3 = SettingTemp.Rows.OfType<DataRow>().Select(s => new DepartmentAndUse { ParentSettingId = s["ParentSettingId"].ToString(), SettingId = s["SettingId"].ToString(), DisplayName = s["DisplayName"].ToString(), Type = s["Type"].ToString() }).ToList();
                if (model != null)
                {
                    result3.ForEach(f =>
                    {
                        if (!string.IsNullOrWhiteSpace(model.DepartmentId))
                        {
                            if (f.Type == "0")
                            {
                                f.IsSelected = f.SettingId == model.DepartmentId;
                            }
                            else if (f.Type == "1")
                            {
                                f.IsSelected = f.SettingId == model.IntentionId;
                            }
                        }
                    });
                }

            }
            return View(Tuple.Create(result, result2, result3));

        }
        public ActionResult AddGetRule(int id, int? rid, string iscopy)
        {
            GetPCodeModel model = null;
            List<DepartmentAndUse> model2 = new List<DepartmentAndUse>();
            if (rid != null)
                model = PromotionManager.SelectGeCouponRulesByRuleID(id).FirstOrDefault(c => c.GETPKID == rid);
            else
            {
               var promotionInfo= PromotionManager.SelectGeCouponRulesByRuleID(id).FirstOrDefault();
                model = new GetPCodeModel() { RuleDescription = promotionInfo?.RuleDescription };
            }
            var SettingTemp = PromotionManager.GetDepartmentUseSetting();
            if (SettingTemp != null && SettingTemp.Rows.Count > 0)
            {
                model2 = SettingTemp.Rows.OfType<DataRow>().Select(s => new DepartmentAndUse { ParentSettingId = s["ParentSettingId"].ToString(), SettingId = s["SettingId"].ToString(), DisplayName = s["DisplayName"].ToString(), Type = s["Type"].ToString() }).ToList();
                if (model != null)
                {
                    model2.ForEach(f =>
                    {
                        if (f.Type == "0")
                        {
                            f.IsSelected = f.SettingId == model.DepartmentId.ToString();
                        }
                        else if (f.Type == "1")
                        {
                            f.IsSelected = f.SettingId == model.IntentionId.ToString();
                        }
                    });
                }

            }

            var model3 = DALPromotion.GetAllBusinessLines();

            return View(Tuple.Create(model, model2, model3));
        }
        [HttpPost]
        public JsonResult UploadUserFile()
        {
            //用户列表
            var cellPhones = new List<string>();
            try
            {
                var cellphonesFile = Request.Files.Count > 0 ? Request.Files[0] : null;
                if (cellphonesFile == null || cellphonesFile.ContentLength < 11)
                    return Json(new ResponseModel { IsSuccess = false, OutMessage = "上传文件中不包含手机号！~" }, "text/html", Encoding.UTF8);
                //return Content("0");

                using (var stream = new StreamReader(cellphonesFile.InputStream, Encoding.UTF8))
                {
                    string[] tmpStr = Regex.Split(stream.ReadToEnd(), @"\D");
                    var tmpInfo = tmpStr.Where(cellphone => !string.IsNullOrEmpty(cellphone)).Distinct().ToList();
                    var info = tmpInfo.Where(cellphone => Regex.IsMatch(cellphone, @"^1\d{10}$")).ToList();
                    if (tmpInfo.Count > info.Count)
                    {
                        var feifaInfo = tmpInfo.Where(cellphone => !Regex.IsMatch(cellphone, @"^1\d{10}$")).ToList();//获取非法手机号码序列
                        return Json(new ResponseModel { IsSuccess = false, OutMessage = "上传文件中包含非法手机号码！~" + string.Join(",", feifaInfo.ToArray()) }, "text/html", Encoding.UTF8);
                    }
                    if (tmpInfo.Count > info.Count)
                    {
                        return Json(new ResponseModel { IsSuccess = false, OutMessage = "上传文件中包含非法手机号码！~" }, "text/html", Encoding.UTF8);
                    }
                    foreach (var item in info)
                    {
                        cellPhones.Add(item);
                    }
                }

                if (cellPhones.Count < 1)
                    return Json(new ResponseModel { IsSuccess = false, OutMessage = "上传文件中不包含手机号！~" }, "text/html", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel { IsSuccess = false, OutMessage = ex.ToString() }, "text/html", Encoding.UTF8);
            }

            //查询 已存在的用户数
            var existsCount = DALPromotion.GetUserCountByMobilels(cellPhones);
            return Json(new { IsSuccess = true, OutMessage = string.Empty, ObjectData = cellPhones.Count.ToString(), ExistsData = existsCount }, "text/html", Encoding.UTF8);
        }
        [HttpPost]
        public ActionResult SaveGetRule(GetPCodeModel model)
        {
            if (model != null)
            {//setting,displayName
                var tableData = PromotionManager.GetDepartmentUseSettingNameBySettingId(new int[] { model.DepartmentId, model.IntentionId });
                if (tableData != null && tableData.Rows.Count > 0)
                {
                    foreach (DataRow item in tableData.Rows)
                    {
                        if (item.GetValue<int>("settingId") == model.DepartmentId)
                        {
                            model.DepartmentName = item.GetValue<string>("displayName");
                        }
                        else if (item.GetValue<int>("settingId") == model.IntentionId)
                        {
                            model.IntentionName = item.GetValue<string>("displayName");
                        }
                    }
                }

                var businessData = DALPromotion.GetAllBusinessLinesById(model.BusinessLineId);
                if (businessData != null && businessData.Rows.Count > 0)
                {
                    model.BusinessLineName = businessData.Rows[0].GetValue<string>("DisplayName");
                }
                if (model.IsPush == 1)
                {
                    if (string.IsNullOrEmpty(model.PushSetting))
                    {
                        return Json(-1);
                    }
                    var arr = model.PushSetting.Split(new[] { ",", "，" }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => int.TryParse(x, out int result)).ToArray();
                    if (arr.Length == 0)
                    {
                        return Json(-1);
                    }
                    else
                    {
                        model.PushSetting = string.Join(",", arr);
                    }
                }
                else
                {
                    model.PushSetting = null;
                }
            }

            int resultId = 0;
            var isAdd = model.GETPKID == 0;
            if (model.GETPKID > 0)
            {
                resultId = PromotionManager.UpdateGetPCodeRule(model);
            }
            else
            {
                model.Creater = User.Identity.Name;
                resultId = PromotionManager.SaveGetPCodeRule(model);
                model.GETPKID = resultId;
            }
            using (var client = new ConfigLogClient())
            {
                var response = client.InsertDefaultLogQueue("PromotionConfigLog", JsonConvert.SerializeObject(new
                {
                    ObjectId = resultId,
                    ObjectType = "SaveGetRule",
                    BeforeValue = "",
                    AfterValue = JsonConvert.SerializeObject(model),
                    Operate = isAdd ? "新增优惠券使用规则" : "修改优惠券使用规则",
                    Author = User.Identity.Name
                }));
            }
            return Json(resultId);
        }
        public ActionResult PromotionDetail(int id)
        {
            ViewBag.PKID = id;
            var result = PromotionManager.SelectPromotionDetail(id);
            if (id == 0)
                return HttpNotFound();
            return View(result);
        }
        public int CompareOid(Array a, int oid)
        {
            foreach (var i in a)
            {
                if (Convert.ToInt32(i) == oid)
                    return Convert.ToInt32(a.GetValue(a.Length - 2));
            }
            return 0;
        }

        //public IEnumerable<Category> ChildCategory(IEnumerable<Category> cate)
        //{
        //	if (cate != null && cate.Count() > 0)
        //	{
        //		foreach (var c in cate)
        //		{
        //			if (c.ChildrenCategory == null || c.ChildrenCategory.Count() <= 0)
        //				c.ChildrenCategory = c.ChildrenCategory;
        //			else
        //				c.ChildrenCategory = ChildCategory(c.ChildrenCategory);
        //		}
        //	}
        //	return cate;

        //	foreach (var category in cate)
        //	{
        //		var aaa = new List<Category>();
        //		childCategory(aaa, category);
        //		category.ChildrenCategory = aaa;
        //	}
        //}

        public void childCategory(ICollection<Category> Children, Category Category)
        {
            if (Category.ChildrenCategory == null || !Category.ChildrenCategory.Any())
                Children.Add(Category);
            else
            {
                foreach (var item in Category.ChildrenCategory)
                {
                    childCategory(Children, item);
                }
            }
        }
        public ActionResult GetCategory(string type)
        {
            var source = PromotionManager.SelectProductCategory().Where(s => s.ParentCategory == null || !s.ParentCategory.Any()).ToList();
            if (string.IsNullOrWhiteSpace(type))
                return Json(null, JsonRequestBehavior.AllowGet);
            if (type.Equals("category"))
            {
                foreach (var c in source)
                {
                    var children = new List<Category>();
                    childCategory(children, c);
                    c.ChildrenCategory = children;
                }
                return Json(source.Select(r => new
                {
                    nodeNo=r.NodeNo,
                    name = r.DisplayName,
                    open = false,
                    title = r.CategoryName,
                    children = r.ChildrenCategory.Select(c => new { nodeNo = c.NodeNo,name = c.DisplayName, title = c.CategoryName })
                }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = source.Select(r => new Category()
                {
                    DisplayName = r.DisplayName,
                    CategoryName = r.CategoryName,
                    ChildrenCategory = (r.DisplayName == "礼品" || r.DisplayName == "轮胎" || r.DisplayName == "轮毂") ? new List<Category>() : r.ChildrenCategory.Select(c => new Category() { DisplayName = c.DisplayName, CategoryName = c.CategoryName, ChildrenCategory = c.ChildrenCategory == null ? new List<Category>() : c.ChildrenCategory.Select(cc => new Category() { DisplayName = cc.DisplayName, CategoryName = cc.CategoryName, ChildrenCategory = new List<Category>() }) })
                });
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBrand(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return null;
            return Json(PromotionManager.SelectProductBrand(type).Where(c => !string.IsNullOrWhiteSpace(c.Cp_Brand)), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveInfo(string type, string PromotionModel, string action, string ParentID, string shoptypes, string shopids)
        {
            var parentID = 0;
            if (String.IsNullOrWhiteSpace(ParentID))
                parentID = 0;
            else
            {
                try
                {
                    parentID = Convert.ToInt32(ParentID);

                }
                catch
                {
                    return Json(-1);
                }
            }
            var model = JsonConvert.DeserializeObject<List<PromotionModel>>(PromotionModel);
            return Json(PromotionManager.SavePromotionInfo(model, action, parentID, shoptypes, shopids));
        }
        //[HttpPost]
        //	public ActionResult DeleteRecord(string type, string PKID)
        //	{
        //		if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(PKID))
        //			return Json(-1);
        //		try
        //		{
        //			var id = Convert.ToInt32(PKID);
        //			return Json(PromotionManager.DeleteRecord(type,id));
        //		}
        //		catch
        //		{
        //			return Json(-1);
        //		}
        //	}

        public ActionResult FetchShopNameByID(int? shopId)
        {
            if (shopId == null)
                return Json("", JsonRequestBehavior.AllowGet);
            return Json(PromotionManager.FetchShopNameByID(shopId.Value), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RefreshCache()
        {
            using (var client = new CacheClient())
            {
                var result = client.RefreshPIDCouponRulesCache();
                return Json(result.Result ? 1 : -1);
            }
        }
        /// <summary>
        /// 查询优惠券礼包列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
#if DEBUG
#else
      [PowerManage]
#endif
        public ActionResult SelectGiftBag(int pageIndex = 1)
        {
            int PageSize = 20;
            int TotalCount = 0;
            var ex = new PromotionManager();
            var rest = ex.SelectGiftBag(pageIndex, PageSize, out TotalCount);
            PagerModel pager = new PagerModel(pageIndex, PageSize);
            pager.TotalItem = TotalCount;
            ViewBag.pager = pager;
            return View(new ListModel<ExchangeCodeDetail>(pager, rest));
        }
        /// <summary>
        /// 修改优惠券礼包页面
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult UpdateOEM(int pkid = 0)
        {
            var ex = new PromotionManager();
            var rest = ex.UpdateOEM(pkid);
            var dt = new ListModel<ExchangeCodeDetail>(rest);
            return View(dt);
        }
        /// <summary>
        /// 添加优惠券礼包页面
        /// </summary>
        /// <returns></returns>
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult GetAddOEM()
        {
            return View();
        }
        /// <summary>
        /// 查询优惠券礼包详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult CreatePromotionCode(int pkid = 0)
        {
            var ex = new PromotionManager();
            var rest = ex.SelectPromotionDetails(pkid);
            ViewBag.Parentid = pkid;
            return View(new ListModel<ExchangeCodeDetail>(rest));

        }
        [PowerManage]
        public ActionResult DoGenerate(int pkid, int num)
        {
            var exm = new PromotionManager();
            //string ISActive = exm.GetIsActiveByPKID(pkid).ToString();
            DataTable dt = exm.SelectGiftByDonwLoad(pkid);
            int rows = Convert.ToInt32(dt.Rows[0]["T"]);
            if (rows > 0)
            {
                string isa = dt.Rows[0]["ISActive"].ToString();
                if (isa == "True")
                {
                    //DataTable dt = exm.JudgeGiftBagByPKID(pkid);

                    if (exm.GenerateCoupon(num, pkid) == "1")
                    {
                        return Content("ok");
                    }
                    else
                    {
                        return Content("no");
                    }


                }
                return Content("nos");
            }
            return Content("no");

        }
        /// <summary>
        /// 创建优惠券页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult AddPromotionOEM(int id = 0)
        {
            var exm = new PromotionManager();
            string CodeChannel = exm.SelectCodeChannelByAddGift(id).ToString();
            ViewBag.Parentid = id;
            ViewBag.CodeChannel = CodeChannel;
            var result3 = new List<DepartmentAndUse>();
            var SettingTemp = PromotionManager.GetDepartmentUseSetting();
            if (SettingTemp != null && SettingTemp.Rows.Count > 0)
            {
                result3 = SettingTemp.Rows.OfType<DataRow>().Select(s => new DepartmentAndUse { ParentSettingId = s["ParentSettingId"].ToString(), SettingId = s["SettingId"].ToString(), DisplayName = s["DisplayName"].ToString(), Type = s["Type"].ToString() }).ToList();
            }
            return View(result3);
        }
        /// <summary>
        /// 修改优惠券页面
        /// </summary>
        /// <param name="rest"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult UpdatePromotionOEM(int id = 0)
        {
            var ex = new PromotionManager();
            IEnumerable<ExchangeCodeDetail> rest = new ExchangeCodeDetail[] { };
            rest = ex.SelectPromotionDetailsByEdit(id);
            var dt = new ListModel<ExchangeCodeDetail>(rest);
            var result3 = new List<DepartmentAndUse>();
            var SettingTemp = PromotionManager.GetDepartmentUseSetting();
            if (SettingTemp != null && SettingTemp.Rows.Count > 0)
            {
                var temp = rest.FirstOrDefault();
                result3 = SettingTemp.Rows.OfType<DataRow>().Select(s => new DepartmentAndUse { ParentSettingId = s["ParentSettingId"].ToString(), SettingId = s["SettingId"].ToString(), DisplayName = s["DisplayName"].ToString(), Type = s["Type"].ToString() }).ToList();
                if (temp != null)
                {
                    result3.ForEach(f =>
                    {
                        if (temp.DepartmentId > 0)
                        {
                            if (f.Type == "0")
                            {
                                f.IsSelected = f.SettingId == temp.DepartmentId.ToString();
                            }
                            else if (f.Type == "1")
                            {
                                f.IsSelected = f.SettingId == temp.IntentionId.ToString();
                            }
                        }
                    });
                }

            }
            return View(Tuple.Create(dt, result3));

        }
        /// <summary>
        /// 优惠券类型列表
        /// </summary>
        /// <returns></returns>
        [PowerManage]
        public ActionResult SelectDropDownList()
        {
            var exm = new PromotionManager();
            var dt = exm.SelectDropDownList();
            if (dt.Rows.Count > 0)
            {
                string List = JsonConvert.SerializeObject(dt);
                return Content(List);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 添加优惠券礼包
        /// </summary>
        /// <param name="rest"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult DoAddGiftBag(string rest = null)
        {
            var exm = new PromotionManager();
            ExchangeCodeDetail ecd = JsonConvert.DeserializeObject<ExchangeCodeDetail>(rest);
            ecd.ExChangeEndTime = ecd.ExChangeEndTime + " 23:59:59";
            if (exm.AddOEM(ecd) > 0)
            {
                return Content("ok");
            }
            return View();
        }
        /// <summary>
        /// 修改优惠券礼包
        /// </summary>
        /// <param name="rest"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult DoUpdateOEM(string rest = null)
        {
            var exm = new PromotionManager();
            var ecd = JsonConvert.DeserializeObject<ExchangeCodeDetail>(rest);
            ecd.ExChangeEndTime = ecd.ExChangeEndTime + " 23:59:59";
            if (exm.DoUpdateOEM(ecd) > 0)
            {
                return Content("ok");
            }
            return View();
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult Download(int pkid)
        {
            var exm = new PromotionManager();
            //DataTable dt = exm.JudgeGiftBagByPKID(pkid);
            //int dt = exm.SelectDownloadByPKID(pkid);
            //string ISActive = exm.GetIsActiveByPKID(pkid).ToString();
            DataTable dt = exm.SelectGiftByDonwLoad(pkid);
            int rows = exm.SelectCountByDownLoad(pkid);
            if (rows > 0)
            {
                string isa = dt.Rows[0]["ISActive"].ToString();
                if (isa == "True")
                {

                    //导出
                    var strPath = "ExportExcel?pkid=" + pkid;
                    return Content(strPath);
                }
                else
                {
                    return Content("nos");
                }
            }
            else
            {
                return Content("no");
            }
        }

        /// <summary>
        /// 删除礼包
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult DeleteGift(int pkid)
        {
            var exm = new PromotionManager();
            int count = exm.SelectPromoCodeCount(pkid);
            if (count < 1)
            {
                if (exm.DeleteGift(pkid) > 0)
                {
                    return Content("ok");
                }
                else
                {
                    return Content("no");
                }
            }
            return Content("noc");
        }
        [PowerManage]
        public void ExportExcel(int pkid)
        {
            var exm = new PromotionManager();
            DataTable dt = exm.CreateExcel(pkid);
            var workbook = new XSSFWorkbook();
            ExportImportFactory.ExportSheet(workbook, "GenerateCoupon" + pkid, dt);
            var sheet = workbook.GetSheetAt(0);
            sheet.ShiftRows(1, sheet.LastRowNum, -1);
            using (var stream = new MemoryStream())
            {
                workbook.Write(stream);
                ExportImportUtil.ExportExcel(HttpContext, "GenerateCoupon" + pkid + ".xlsx", stream);
            }
        }
        /// <summary>
        /// 创建优惠券
        /// </summary>
        /// <param name="rest"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult AddPromotionNew(ExchangeCodeDetail rest)
        {
            var exm = new PromotionManager();
            rest.Creater = User.Identity.Name;
            rest.Issuer = User.Identity.Name;
            if (exm.CreeatePromotion(rest) > 0)
            {
                return Content("ok");
            }
            return View();
        }
        /// <summary>
        /// 修改优惠券
        /// </summary>
        /// <param name="rest"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult UpdatePromotion(ExchangeCodeDetail rest)
        {
            var exm = new PromotionManager();
            rest.Issuer = User.Identity.Name;
            if (exm.UpdatePromotionDetailsByOK(rest) > 0)
            {
                return Content("ok");
            }
            return View();
        }
        /// <summary>
        /// 删除优惠券
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult DeletePromoCode(int pkid)
        {
            var exm = new PromotionManager();
            if (exm.DeletePromoCode(pkid) > 0)
            {
                return Content("ok");
            }
            return Content("no");
        }

        /// <summary>
        /// 发券详情
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult SelectExchangeCodeDetailByPage(int pageIndex = 1)
        {
            int PageSize = 20;
            int TotalCount = 0;
            var rest = PromotionManager.SelectExchangeCodeDetailByPage(pageIndex, PageSize, out TotalCount).Rows.OfType<DataRow>().Select(x => new ExchangeCodeDetail(x));
            PagerModel pager = new PagerModel(pageIndex, PageSize);
            pager.TotalItem = TotalCount;
            ViewBag.pager = pager;
            return View(new ListModel<ExchangeCodeDetail>(pager, rest));
        }
        #region CouponDepartmentUseSetting优惠券使用部门及用途配置
        public ActionResult CouponDepartmentUseSetting()
        {
            var SettingTemp = PromotionManager.GetDepartmentUseSettingByParentId(0);
            if (SettingTemp != null && SettingTemp.Rows.Count > 0)
            {
                var temp = SettingTemp.Rows.OfType<DataRow>().Select(s => new DepartmentAndUseModel { DisplayName = s["DisplayName"].ToString(), ParentSettingId = Convert.ToInt32(s["ParentSettingId"]), SettingId = Convert.ToInt32(s["SettingId"]), Type = Convert.ToInt32(s["Type"]), SunItems = GetChildDepartmentUseSetting(Convert.ToInt32(s["SettingId"])) });
                return View(temp);
            }
            return View();
        }



        private IEnumerable<DepartmentAndUseModel> GetChildDepartmentUseSetting(int parentId)
        {
            var SettingTemp = PromotionManager.GetDepartmentUseSettingByParentId(parentId);
            if (SettingTemp != null && SettingTemp.Rows.Count > 0)
            {
                var temp = SettingTemp.Rows.OfType<DataRow>().Select(s => new DepartmentAndUseModel { DisplayName = s["DisplayName"].ToString(), ParentSettingId = Convert.ToInt32(s["ParentSettingId"]), SettingId = Convert.ToInt32(s["SettingId"]), Type = Convert.ToInt32(s["Type"]) });
                return temp;
            }
            return null;
        }
        /// <summary>
        /// 部门和用途详情信息
        /// </summary>
        /// <param name="settingId"></param>
        /// <param name="type"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public ActionResult AddCouponDepartUseSetting(int settingId, int type, int parentId)
        {
            DepartmentAndUseModel result = new DepartmentAndUseModel() { };
            if (settingId <= 0)
            {
                if (type == 1)
                {
                    var temp1 = PromotionManager.GetDepartmentUseSettingNameBySettingId(parentId);
                    var temp2 = temp1.Rows.OfType<DataRow>().Select(s => new DepartmentAndUseModel { DisplayName = s["DisplayName"].ToString(), ParentSettingId = Convert.ToInt32(s["ParentSettingId"]), SettingId = Convert.ToInt32(s["SettingId"]), Type = Convert.ToInt32(s["Type"]) }).FirstOrDefault();
                    result.ParentName = temp2.DisplayName;
                    result.Type = 1;
                    result.ParentSettingId = temp2.SettingId;
                }
                return View(result);
            }
            else
            {
                var temp = PromotionManager.GetDepartmentUseSettingNameBySettingId(settingId);
                result = temp.Rows.OfType<DataRow>().Select(s => new DepartmentAndUseModel { DisplayName = s["DisplayName"].ToString(), ParentSettingId = Convert.ToInt32(s["ParentSettingId"]), SettingId = Convert.ToInt32(s["SettingId"]), Type = Convert.ToInt32(s["Type"]) }).FirstOrDefault();
                if (type == 1)
                {
                    var temp1 = PromotionManager.GetDepartmentUseSettingNameBySettingId(parentId);
                    var temp2 = temp1.Rows.OfType<DataRow>().Select(s => new DepartmentAndUseModel { DisplayName = s["DisplayName"].ToString(), ParentSettingId = Convert.ToInt32(s["ParentSettingId"]), SettingId = Convert.ToInt32(s["SettingId"]), Type = Convert.ToInt32(s["Type"]) }).FirstOrDefault();
                    result.ParentName = temp2.DisplayName;
                }
            }
            return View(result);
        }
        public ActionResult DeleteCouponDepartUseSetting(int settingId)
        {
            var result = PromotionManager.DeleteDepartmentUseSettingNameBySettingId(settingId, User.Identity.Name);
            return Json(result);
        }
        /// <summary>
        /// 保存部门和用途
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SaveCouponDepartUseSetting(DepartmentAndUseModel model)
        {
            bool result = false;
            if (model.SettingId > 0)//修改
                result = PromotionManager.UpdateDepartmentUseSetting(model, User.Identity.Name);
            else//新增
                result = PromotionManager.InsertDepartmentUseSettingName(model, User.Identity.Name);
            return Json(result);
        }
        #endregion

        #region 业务线 配置
        public ActionResult BusinessLineSetting()
        {
            var businessLines = DALPromotion.GetAllBusinessLines();
            var temp = businessLines.Rows.OfType<DataRow>().Select(x => new PromotionBusinessLineModel()
            {
                DisplayName = x["DisplayName"].ToString(),
                PKID = int.Parse(x["PKID"].ToString())
            });
            return View(temp);
        }

        public JsonResult SaveBusinessLine(PromotionBusinessLineModel model)
        {
            model.Operater = User.Identity.Name;
            var result = DALPromotion.SaveBusinessLine(model);
            return Json(new
            {
                Code = result
            });
        }

        public JsonResult DeleteBusinessLine(int pkid)
        {
            var result = DALPromotion.DeleteBusinessLine(pkid, User.Identity.Name);
            return Json(new
            {
                Code = result
            });
        }

        #endregion

        #region 优惠券使用规则新接口
        public ActionResult EditPromotionNew(string id)
        {
            var model = new PromotionModel();
            if (string.IsNullOrWhiteSpace(id))
                return View("EditPromotionNew", model);
            int pkid = 0;
            if (!int.TryParse(id, out pkid))
                return View("EditPromotionNew", model);
            model = PromotionManager.GetPromotionDetail(pkid);
            return View("EditPromotionNew", model);
        }
        [HttpPost]
        public ActionResult SaveCouponRuleInfo(string type, string PromotionModel, string action, string shoptypes, string shopids, string categorys, string brands, string pids)
        {
            try
            {
                var categoryDic = JsonConvert.DeserializeObject<string[]>(categorys);
                var brandsDic = JsonConvert.DeserializeObject<string[]>(brands);
                var pidsDic = JsonConvert.DeserializeObject<string[]>(pids);
                var shopType = JsonConvert.DeserializeObject<string[]>(shoptypes);
                var shopId = JsonConvert.DeserializeObject<string[]>(shopids);
                var model = JsonConvert.DeserializeObject<List<PromotionModel>>(PromotionModel).FirstOrDefault();
                var isAdd = model.PKID == 0;
                var result = PromotionManager.SaveCouponRuleInfo(model, action, shopType?.Distinct().ToArray(),
                    shopId?.Distinct().ToArray(), categoryDic?.Distinct().ToArray(), brandsDic?.Distinct().ToArray(),
                    pidsDic?.Select(x => x.Trim()).Distinct().ToArray());
                if (result > 0) //修改了主券，刷新缓存
                {
                    using (var client = new CacheClient())
                    {
                        client.RefreshPIDCouponRulesCache();
                    }
                    if (model != null)
                    {
                        model.PKID = result;
                        using (var client = new ConfigLogClient())
                        {
                            client.InsertDefaultLogQueue("PromotionConfigLog", JsonConvert.SerializeObject(new
                            {
                                ObjectId = result,
                                ObjectType = "SaveCouponRuleInfo",
                                BeforeValue = "",
                                AfterValue = JsonConvert.SerializeObject(new
                                {
                                    model,
                                    categoryDic,
                                    brandsDic,
                                    pidsDic,
                                    shopType,
                                    shopId
                                }),
                                Operate = isAdd ? "新增优惠券领取规则" : "修改优惠券领取规则",
                                Author = User.Identity.Name
                            }));
                        }
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(-2);
            }
        }
        [HttpPost]
        /// <summary>
        /// PID批量导入功能
        /// </summary>
        public ActionResult ImportPIDs(string PIDs)
        {
            try
            {
                var pids = PIDs.Trim(new char[] { '\"' }).Split(new string[] { "\\r\\n" }, StringSplitOptions.RemoveEmptyEntries);
                var productModels = PromotionManager.GetProductsByPIDs(pids.Select(s => Convert.ToString(s)).ToArray());
                return Json(productModels);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        public ActionResult UploadPids(string shopIds)
        {
            try
            {
                var shopids = shopIds.Trim(new char[] { '\"' }).Split(new string[] { "\\r\\n" }, StringSplitOptions.RemoveEmptyEntries);
                var ids = shopids.Select(s => Convert.ToInt32(s)).ToArray();
                var shopModels = PromotionManager.GetShopsByShopIds(ids);
                return Json(shopModels);
            }
            catch (Exception ex)
            {
                return Json(new { ex.Message, ex.Source });
            }
        }
        public ActionResult PromotionDetailNew(int id)
        {
            ViewBag.Disabled = "disabled";
            ViewBag.OnlyView = "display:none;";
            if (id == 0)
                return HttpNotFound();
            return EditPromotionNew(id.ToString());
        }
        #endregion

        public DataTable GetCouponRules()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                connection.Open();

                return (SqlHelper.ExecuteDataTable(connection, CommandType.Text, @"
SELECT	CR.PKID, Type, CR.Name 
FROM	Activity..tbl_CouponRules AS CR WITH ( NOLOCK ) 
WHERE	CR.ParentID = 0 ORDER BY CR.PKID;"));
            }
        }

        [HttpPost]
        public ActionResult Cellphone(PromotionCellphoneSendModel model)
        {
            if (model.StartDate > model.EndDate)
                return Json(-2);

            if (model.RuleID != 42 && model.RuleID != 44 && model.RuleID != 46 && model.RuleID != 87 && (model.Discount <= 0 || model.MinMoney <= 0))
                return Json(-3);

            if (model.Discount > model.MinMoney)
                return Json(-4);

            var cellphones = new DataTable();
            cellphones.Columns.Add("Cellphone");

            if (!model.AllUser)
            {
                var cellphonesFile = Request.Files.Count > 0 ? Request.Files[0] : null;
                if (cellphonesFile == null || cellphonesFile.ContentLength < 11)
                    return Json(-5);

                using (var stream = new StreamReader(cellphonesFile.InputStream, Encoding.UTF8))
                {
                    foreach (var item in Regex.Split(stream.ReadToEnd(), @"\D")
                        .Where(cellphone => !string.IsNullOrEmpty(cellphone) && Regex.IsMatch(cellphone, @"^1\d{10}$"))
                        .Distinct())
                    {
                        var row = cellphones.NewRow();
                        cellphones.Rows.Add(row);

                        row[0] = item;
                    }
                }

                if (cellphones.Rows.Count < 1)
                    return Json(-5);
            }

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                connection.Open();

                var sqlparamters = new List<SqlParameter>()
                {
                    new SqlParameter("@StartDate", model.StartDate),
                    new SqlParameter("@EndDate", model.EndDate),
                    new SqlParameter("@Type", null),
                    new SqlParameter("@RuleId", model.RuleID),
                    new SqlParameter("@Description", model.Description),
                    new SqlParameter("@Discount", model.Discount.GetValueOrDefault()),
                    new SqlParameter("@MinMoney", model.MinMoney.GetValueOrDefault()),
                    new SqlParameter("@CreateUser", User.Identity.Name),
                    new SqlParameter("@Channels", "业务系统")
                };

                if (model.AllUser || cellphones.Rows.Count < 1000)
                {
                    sqlparamters.Insert(0, new SqlParameter("@Cellphones", cellphones));

                    cellphones = SqlHelper.ExecuteDataTable2(connection, CommandType.StoredProcedure, "PromotionCode_CreatePromotionCode_Cellphones", sqlparamters.ToArray());
                }
                else
                {
                    using (var tran = connection.BeginTransaction())
                    {
                        SqlHelper.ExecuteNonQueryV2(tran, CommandType.Text, @"SELECT	CAST('' AS VARCHAR(100)) AS Cellphone INTO	#C WHERE	0 != 0;");

                        cellphones.TableName = "#C";
                        SaveToDatabase(tran, cellphones);

                        cellphones = SqlHelper.ExecuteDataTable2(tran, CommandType.Text, @"DECLARE	@Cellphones Cellphone;
INSERT	INTO @Cellphones SELECT	* FROM	#C;
DROP TABLE #C;
EXEC Gungnir..PromotionCode_CreatePromotionCode_Cellphones @Cellphones, @StartDate, @EndDate, @Type, @RuleId, @Description, @Discount, @MinMoney, @CreateUser, @Channels;", sqlparamters.ToArray());

                        tran.Commit();
                    }
                }

                var dic = new Dictionary<string, object>();
                dic["Result"] = cellphones.Rows.Count;
                if (model.AllUser)
                    dic["Cellphones"] = string.Join(Environment.NewLine, cellphones.AsEnumerable().Select(row => row[0].ToString()));

                return JavaScript(JsonConvert.SerializeObject(dic));
            }
        }

        /// <summary>
        /// 批量复制
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="sourceTable">数据源</param>
        /// <param name="destinationTable">目标表名</param>
        public static void SaveToDatabase(SqlTransaction transaction, DataTable sourceTable)
        {
            using (var sbc = new SqlBulkCopy(transaction.Connection, SqlBulkCopyOptions.KeepIdentity, transaction))
            {
                sbc.BatchSize = 1000;
                sbc.BulkCopyTimeout = 300;

                //将DataTable表名作为待导入库中的目标表名
                sbc.DestinationTableName = sourceTable.TableName;

                //将数据集合和目标服务器库表中的字段对应\
                foreach (DataColumn col in sourceTable.Columns)
                {
                    //列映射定义数据源中的列和目标表中的列之间的关系
                    sbc.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }

                sbc.WriteToServer(sourceTable);
            }
        }


        /// <summary>
        /// 查询优惠券礼包列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult SelectAllPromotion(int pageIndex = 1)
        {

            int PageSize = 20;
            int TotalCount = 0;
            var ex = new PromotionCodeManager();
            var rest = ex.SelectGiftBag(pageIndex, PageSize, out TotalCount);
            PagerModel pager = new PagerModel(pageIndex, PageSize);
            pager.TotalItem = TotalCount;
            ViewBag.pager = pager;
            return View(new ListModel<ExchangeCodeDetail>(pager, rest));
        }

        /// <summary>
        /// 创建优惠券
        /// </summary>
        /// <param name="rest"></param>
        /// <returns></returns>
        public ActionResult AddPromotion(ExchangeCodeDetail rest)
        {

            var exm = new PromotionCodeManager();

            if (exm.CreeatePromotion(rest) > 0)
            {
                return Content("ok");
            }
            return Content("");

        }

        public JsonResult SelectRegionInfoById(int? id)
        {
            if (id == null) return Json(new ResponseModel { IsSuccess = false, OutMessage = "参数不能为空！" }, JsonRequestBehavior.AllowGet);
            ResponseModel responseModel = new ResponseModel();
            try
            {
                responseModel.ObjectData = new RegionManager().SelectAllProvince(id.Value);
                responseModel.IsSuccess = true;
            }
            catch (Exception ex)
            {
                responseModel.IsSuccess = false;
            }
            return Json(responseModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductCategories(int oid)
        {
            ResponseModel responseModel = new ResponseModel();
            try
            {
                var result = GetAllProductCategories();

                if (result != null)
                {
                    result = result.Where(o => o.ParentOid == oid).ToList();
                    responseModel.ObjectData = result;
                    responseModel.IsSuccess = result.Count != 0;
                }
            }
            catch (Exception ex)
            {
                responseModel.IsSuccess = false;
            }

            return Json(responseModel, JsonRequestBehavior.AllowGet);
        }

        private List<SKUProductCategory> GetAllProductCategories()
        {
            if (HttpRuntime.Cache[key] == null)
            {
                var manager = new ProductInfomationManager();
                var categories = manager.GetAllProductCategories();
                HttpRuntime.Cache.Insert(key, categories, null, DateTime.Now.AddHours(2), Cache.NoSlidingExpiration);
            }

            return HttpRuntime.Cache[key] as List<SKUProductCategory>;
        }
        /// <summary>
        /// 创建优惠券任务
        /// </summary>
        /// <param name="id">优惠券任务编号</param>
        /// <returns></returns>
#if DEBUG

#else
        [PowerManage]
#endif
        public ActionResult CreatePromotionTask(int? id)
        {
            //只能特定人员才可以创建优惠券任务
            //if (!cellphoneCreateUsers.Contains(User.Identity.Name, StringComparer.CurrentCultureIgnoreCase))
            //    return HttpNotFound();
            var model = new CreatePromotionTaskModel();
            model.ListProvince = new RegionManager().SelectAllProvince(0).OrderBy(item => item.PKID).ToList();
            model.ListRegion = new RegionManager().SelectAllRegions();

            model.CategoryOne = this.GetAllProductCategories().Where(o => o.ParentOid == -1).ToList();
            model.CategoryOne.RemoveAll(p => { return !"轮胎/车品/保养".Contains(p.DisplayName); });
            model.CategoryTwo = this.GetAllProductCategories().Where(o => o.ParentOid == 1).ToList();

            //var orderChannelList = new BizDictionaryManager().SelectDeliveryType("OrderChannel");
            var orderChannelList = this.GetAllOrderChannel();
            model.ListOrderChannel = orderChannelList;

            model.ListOrderType = new DictionariesManager().SelectDeliveryType("OrderType");
            model.ListOrderStatus = new DictionariesManager().SelectDeliveryType("OrderStatus");
            model.BrandInfos = new VehicleTypeManager().GetAllVehicleBrands();
            if (model.BrandInfos.Any())
            {
                model.VehicleInfos = new VehicleTypeManager().GetVehicleSeries(model.BrandInfos[0]);
            }
            model.RuleData = GetCouponRules();
            model.BrandData = DalPromotionJob.GetAllProductBrands();
            model.FinanceMarkData = new DictionariesManager().GetAllFinanceMark();
            #region MyRegion
            var result3 = new List<DepartmentAndUse>();
            var SettingTemp = DalPromotionJob.GetDepartmentUseSetting();
            if (SettingTemp != null && SettingTemp.Rows.Count > 0)
            {
                result3 = SettingTemp.Rows.OfType<DataRow>().Select(s => new DepartmentAndUse { ParentSettingId = s["ParentSettingId"].ToString(), SettingId = s["SettingId"].ToString(), DisplayName = s["DisplayName"].ToString(), Type = s["Type"].ToString() }).ToList();
            }
            model.DepartmentAndUseData = result3;
            IEnumerable<int> containIds = null;
            if (id != null && id > 0)
            {
                containIds = new List<int>() { id.Value };
            }
            model.PromotionTaskActivityData = DalPromotionJob.GetAllBiActivity(containIds);
            model.BusinessData = DALPromotion.GetAllBusinessLines();

            #endregion
            if (id == null || id < 0)
            {
                return View(model);
            }
            model.PromotionTask = DalPromotionJob.SelectPromotionTaskInfoByIdNew(id.Value);
            model.FilterItemJson = GetFilterItemsJson(model.PromotionTask, model.ListRegion, model.ListOrderChannel, model.VehicleInfos);

            return View(model);
        }

        /// <summary>
        /// 合并包含“安卓”“手机”的ITEM、合并包含“IOS”的ITEM
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Dictionary> HandlerListOrderChannel(List<Dictionary> list)
        {
            if (!list.Any()) return list;
            list.RemoveAll(item => item.DicValue.Contains("安卓") || item.DicValue.Contains("手机") || item.DicValue.Contains("IOS"));
            list.Add(new Dictionary { DicType = "OrderChannel", DicKey = "8安卓", DicValue = "安卓" });
            list.Add(new Dictionary { DicType = "OrderChannel", DicKey = "JIOS", DicValue = "IOS" });
            return list;
        }

        public JsonResult GetPromotionTaskActivityUsersCount(int promotionTaskActivityId)
        {
            var result = DalPromotionJob.GetActivityUsersCount(promotionTaskActivityId);
            return Json(new
            {
                Code = 1,
                Count = result
            });
        }


        /// <summary>
        /// 获取人工选择选项->过滤条件
        /// </summary>
        /// <param name="promotionTask"></param>
        /// <param name="listRegion"></param>
        /// <param name="listOrderChannel"></param>
        /// <param name="vehicleInfos"></param>
        /// <returns></returns>
        private string GetFilterItemsJson(PromotionTask promotionTask, List<Region> listRegion, List<Dictionary> listOrderChannel,
            IDictionary<string, string> vehicleInfos)
        {
            //拼接前端操作filter Item 所需JSON对象
            List<FilterItem> filterItems = new List<FilterItem>();
            if (promotionTask != null)
            {
                //订单时间
                if (promotionTask.FilterStartTime != null && promotionTask.FilterEndTime != null)
                {
                    filterItems.Add(new FilterItem
                    {
                        Title = "订单时间：" +
                                promotionTask.FilterStartTime.Value.ToString("yyyy-MM-dd HH:mm") + "," +
                                promotionTask.FilterEndTime.Value.ToString("yyyy-MM-dd HH:mm"),
                        ElementName = "ordersTime",
                        ElementValue =
                            promotionTask.FilterStartTime.Value.ToString("yyyy-MM-dd HH:mm") + "," +
                            promotionTask.FilterEndTime.Value.ToString("yyyy-MM-dd HH:mm"),
                        Attributes = null
                    });
                }
                //品牌
                if (!string.IsNullOrWhiteSpace(promotionTask.Brand))
                {
                    var brands = promotionTask.Brand.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    brands.ForEach(item =>
                    {
                        filterItems.Add(new FilterItem
                        {
                            Title = "品牌：" + item,
                            ElementName = "Brand_" + item,
                            ElementValue = item,
                            Attributes = null
                        });
                    });
                }
                //类别
                if (!string.IsNullOrWhiteSpace(promotionTask.Category))
                {
                    List<SKUProductCategory> listCategory = this.GetAllProductCategories();
                    var areaArr = promotionTask.Category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    areaArr.ForEach(item =>
                    {
                        var data = listCategory.FirstOrDefault(info => info.Oid.ToString().Equals(item));
                        if (data != null)
                        {
                            filterItems.Add(new FilterItem
                            {
                                Title = "产品类别：" + data.DisplayName,
                                ElementName = "Category",
                                ElementValue = item
                            });
                        }
                    });
                }
                //PID
                if (!string.IsNullOrWhiteSpace(promotionTask.Pid))
                {
                    var pids = promotionTask.Pid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    pids.ForEach(item =>
                    {
                        filterItems.Add(new FilterItem
                        {
                            Title = "PID：" + item,
                            ElementName = "Pid_" + item,
                            ElementValue = item
                        });
                    });
                }
                //购买金额
                if (promotionTask.SpendMoney != null)
                {
                    filterItems.Add(new FilterItem
                    {
                        Title = "购买金额：" + promotionTask.SpendMoney.ToString(),
                        ElementName = "SpendMoney",
                        ElementValue = promotionTask.SpendMoney.ToString()
                    });
                }
                //购买件数
                if (promotionTask.PurchaseNum != null)
                {
                    filterItems.Add(new FilterItem
                    {
                        Title = "购买件数：" + promotionTask.PurchaseNum.ToString(),
                        ElementName = "PurchaseNum",
                        ElementValue = promotionTask.PurchaseNum.ToString()
                    });
                }
                //地区
                if (!string.IsNullOrWhiteSpace(promotionTask.Area))
                {
                    var areaArr = promotionTask.Area.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    areaArr.ForEach(item =>
                    {
                        var data = listRegion.FirstOrDefault(info => info.PKID.ToString().Equals(item));
                        if (data != null)
                        {
                            filterItems.Add(new FilterItem
                            {
                                Title = "地区：" + data.RegionName,
                                ElementName = "Area_" + item,
                                ElementValue = item
                            });
                        }
                    });
                }
                //渠道
                if (!string.IsNullOrWhiteSpace(promotionTask.Channel))
                {
                    var channelArr = promotionTask.Channel.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    channelArr.ForEach(item =>
                    {
                        filterItems.Add(new FilterItem
                        {
                            Title = "渠道：" + item,
                            ElementName = "Channel_" + item,
                            ElementValue = item
                        });
                    });
                }
                if (promotionTask.InstallType != null)
                {
                    filterItems.Add(new FilterItem
                    {
                        Title = "安装类型：" + (promotionTask.InstallType == 1 ? "到店安装" : "非到店安装"),
                        ElementName = "InstallType",
                        ElementValue = promotionTask.InstallType.ToString(),
                        Attributes = null
                    });

                }
                //订单状态
                if (promotionTask.OrderStatus != null)
                {
                    var statuStr = promotionTask.OrderStatus == "Installed" ? "已安装" :
                        (promotionTask.OrderStatus == "Paid" ? "已付款" : "订单已完成");
                    filterItems.Add(new FilterItem
                    {
                        Title = "订单状态：" + statuStr,
                        ElementName = "OrderStatus",
                        ElementValue = promotionTask.OrderStatus
                    });
                }

                //车型 型号
                if (!string.IsNullOrWhiteSpace(promotionTask.Vehicle))
                {
                    var vehicleArr = promotionTask.Vehicle.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    vehicleArr.ForEach(item =>
                    {
                        var data = vehicleInfos.FirstOrDefault(info => info.Key.ToString().Equals(item));
                        filterItems.Add(new FilterItem
                        {
                            Title = "车型：" + data.Value,
                            ElementName = "Vehicle_" + item,
                            ElementValue = item
                        });
                    });
                }
            }
            return JsonConvert.SerializeObject(filterItems);
        }
#if DEBUG

#else
        [PowerManage]
#endif
        public JsonResult SelectPromotionTaskPromotionListById(int taskPromotionListId, int couponRulesId)
        {
            ResponseModel responseModel = new ResponseModel();
            try
            {
                var info = DalPromotionJob.SelectPromotionTaskPromotionListByIdNew(taskPromotionListId, couponRulesId);
                responseModel.IsSuccess = true;
                responseModel.ObjectData = new PromotionTaskCouponRuleModel
                {
                    CouponRulesId = info.CouponRulesId,
                    RuleName = info.RuleName,
                    PromotionDescription = info.PromotionDescription,
                    StartTime = info.StartTime?.ToString("yyyy-MM-dd") ?? "",
                    EndTime = info.EndTime?.ToString("yyyy-MM-dd") ?? "",
                    MinMoney = info.MinMoney,
                    DiscountMoney = info.DiscountMoney,
                    FinanceMarkName = info.FinanceMarkName,
                    DepartmentName = info.DepartmentName,
                    IntentionName = info.IntentionName,
                    BusinessLineName = info.BusinessLineName,
                    Number = info.Number,
                    IsRemind = info.IsRemind,
                    IsPush = info.IsPush,
                    PushSetting = info.PushSetting
                };
            }
            catch (Exception ex)
            {
                responseModel.IsSuccess = false;
            }
            return Json(responseModel);
        }

#if DEBUG

#else
        [PowerManage]
#endif
        public ActionResult SearchPromotion(int? promotionTaskId, string taskName, DateTime? createTime,
            int? taskStatus, int? taskType, int? couponRulesId, int? pageNo)
        {
            int sumNum;
            var pageSize = 20;
            if (pageNo == null) pageNo = 1;

            var model = new PromotionTaskListModel();

            var taskList = new List<SearchPromotionByCondition>();
            taskList = DalPromotionJob.SearchPromotionTaskByCondition(pageNo.Value, pageSize, promotionTaskId, string.IsNullOrWhiteSpace(taskName) ? null : taskName, createTime,
                taskStatus == -1 ? null : taskStatus, taskType == -1 ? null : taskType, couponRulesId, out sumNum);

            model.SumNum = sumNum;
            model.PromotionTaskList = taskList;

            if (sumNum > 1)
            {
                double tmp = sumNum / double.Parse(pageSize.ToString());
                int pageNum = (int)Math.Ceiling(tmp);
                string tmpUrl = string.Format("/promotion/SearchPromotion?pageno={0}&PromotionTaskId={1}&TaskName={2}&CreateTime={3}&TaskStatus={4}&TaskType={5}&CouponRulesId={6}",
                    "{0}", promotionTaskId, taskName, createTime, taskStatus, taskType, couponRulesId);
                model.PagerStr = PromotionPager.GetListPager(tmpUrl, pageNo.Value, pageNum);
                if (pageNum == 1) { model.PagerStr = string.Empty; }
            }
            model.SearchInfo = new PromotionSearchInfoModel
            {
                PromotionTaskId = promotionTaskId,
                TaskName = taskName,
                CreateTime = createTime,
                TaskStatus = taskStatus,
                TaskType = taskType,
                CouponRulesId = couponRulesId,
                PageNo = pageNo
            };

            return View(model);
        }

        /// <summary>
        /// 用户优惠券查询
        /// </summary>
        /// <param name="userCellPhone"></param>
        /// <param name="promotionCodeStatus">0 未使用 1已使用 2未激活</param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult UserPromotionCheck(string userCellPhone, int? promotionCodeStatus, int? pageNo)
        {
            int sumNum;
            var pageSize = 3;

            if (pageNo == null) pageNo = 1;

            var model = new UserPromotionSearchPageModel();

            var taskList = new List<SelectPromotionCodeByUserCellPhonePager>();
            if (promotionCodeStatus != null && promotionCodeStatus == -1) promotionCodeStatus = null;
            taskList = DalPromotionJob.SelectPromotionCodeByUserCellPhonePager(userCellPhone, promotionCodeStatus, pageNo.Value, pageSize, out sumNum);

            model.SumNum = sumNum;
            model.UserPromotionList = taskList;

            if (sumNum > 1)
            {
                double tmp = sumNum / double.Parse(pageSize.ToString());
                int pageNum = (int)Math.Ceiling(tmp);
                string tmpUrl = string.Format("/promotion/UserPromotionCheck?pageno={0}&userCellPhone={1}&promotionCodeStatus={2}",
                    "{0}", userCellPhone, promotionCodeStatus);
                model.PagerStr = PromotionPager.GetListPager(tmpUrl, pageNo.Value, pageNum);
                if (pageNum == 1) { model.PagerStr = string.Empty; }
            }
            model.UserCellPhone = userCellPhone;
            model.PromotionCodeStatus = promotionCodeStatus == null ? -1 : promotionCodeStatus.Value;
            model.PageNo = pageNo.Value;
            return View(model);
        }


        /// <summary>
        /// 优惠券任务审核
        /// </summary>
        /// <param name="id">任务编号</param>
        /// <param name="opType">1 审核 2关闭</param>
        /// <returns></returns>
#if DEBUG

#else
        [PowerManage]
#endif
        [HttpPost]
        public JsonResult PromotionTaskShenhe(int id, string opType)
        {
            ResponseModel info = new ResponseModel();
            info.IsSuccess = false;

            if (opType != "shenhe" && opType != "guanbi")
            {
                info.OutMessage = "参数异常！";
                return Json(info);
            }

            var taskStatus = PromotionConsts.PromotionTaskStatusEnum.PendingAudit;
            if (opType == "shenhe")
            {
                taskStatus = PromotionConsts.PromotionTaskStatusEnum.Executed;
            }
            if (opType == "guanbi")
            {
                taskStatus = PromotionConsts.PromotionTaskStatusEnum.Closed;
            }

            try
            {
                var promotionTask = DalPromotionJob.GetPromotionTaskById(id);
                if (promotionTask != null)
                {
                    PromotionManager.UpdatePromotionTaskStatus(id, taskStatus, ThreadIdentity.Identifier?.Name);
                    info.IsSuccess = true;
                    if (opType == "shenhe") info.OutMessage = "审核成功";
                    if (opType == "guanbi") info.OutMessage = "关闭成功";

                    if (taskStatus == PromotionConsts.PromotionTaskStatusEnum.Closed && promotionTask.SelectUserType == 3)
                    {
                        DalPromotionJob.ResetPromotionTaskActivity(promotionTask.PromotionTaskActivityId, promotionTask.PromotionTaskId);
                    }
                    // 针对单次任务功能，每次审核通过后，发邮件通知相关运营人员
                    //if (promotionTask.TaskType == (int)PromotionTaskType.Once && taskStatus == PromotionConsts.PromotionTaskStatusEnum.Executed)
                    //{
                    //    try
                    //    {
                    //        var emailProcess = new BizEmailProcess
                    //        {
                    //            url = string.Empty,
                    //            ToMail = "zhanglingjia@tuhu.cn,zhuzhiyuan@tuhu.cn",
                    //            CC = null,
                    //            Status = "New",
                    //            Type = "Email",
                    //            Subject = "优惠券单次任务审核通过",
                    //            Body = DalPromotionJob.GetEmailBody(promotionTask),
                    //            OrderNo = string.Empty
                    //        };
                    //        new OprLogManager().AddEmailProcess(emailProcess);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        info.OutMessage = "操作成功。邮件发送失败！";
                    //        return Json(info);
                    //    }

                    //}
                    info.OutMessage = "操作成功";
                    return Json(info);
                }
                else
                {
                    info.OutMessage = "这条优惠券任务不存在！";
                    return Json(info);
                }

            }
            catch (Exception ex)
            {
                info.OutMessage = "操作异常！~请与管理员联系";
            }
            return Json(info);
        }

#if !DEBUG
        [PowerManage]
#endif
        [HttpPost]
        public JsonResult DelPromotionTaskPromotionListById(int? taskPromotionListId)
        {
            ResponseModel info = new ResponseModel();
            info.IsSuccess = false;

            if (taskPromotionListId == null)
            {
                info.OutMessage = "参数异常！";
                return Json(info);
            }
            try
            {
                DalPromotionJob.DelPromotionTaskPromotionListById(taskPromotionListId.Value);
                info.IsSuccess = true;
                info.OutMessage = "删除成功";
            }
            catch (Exception ex)
            {
                info.OutMessage = "操作异常！~请与管理员联系";
            }
            return Json(info);
        }
#if DEBUG

#else
        [PowerManage]
#endif
        [HttpPost]
        public JsonResult CreateOrUpdatePromotionTaskPromotionList(PromotionTaskCouponRule paramsInfo)
        {

            ////判断指定的优惠券类型，优惠金额和最少使用金额是否正确
            //if (promotionTaskParams.PromotionTask.PromotionRuleId != 42 && promotionTaskParams.PromotionTask.PromotionRuleId != 44
            //    && promotionTaskParams.PromotionTask.PromotionRuleId != 46 &&
            //    promotionTaskParams.PromotionTask.PromotionRuleId != 87
            //    && (promotionTaskParams.PromotionTask.DiscountMoney <= 0
            //    || promotionTaskParams.PromotionTask.UseMoney <= 0))
            //    return Json(-3);

            ////判断优惠金额和最少使用金额是否正确
            //if (promotionTaskParams.PromotionTask.DiscountMoney > promotionTaskParams.PromotionTask.UseMoney)
            //    return Json(-4);

            ResponseModel info = new ResponseModel();
            info.IsSuccess = false;

            try
            {
                //判断输入的优惠券规则是否存在
                var manager = new PromotionCodeManager();
                if (paramsInfo.CouponRulesId != null)
                {
                    var promotionRuleName = manager.GetPromotionRuleNameById(paramsInfo.CouponRulesId.Value);
                    if (string.IsNullOrWhiteSpace(promotionRuleName))
                    {
                        info.OutMessage = "优惠券RuleId不存在！请重新填写~";
                        return Json(info);
                    }
                }
                var dicMamager = new DictionariesManager().GetAllFinanceMark();
                if (dicMamager != null && dicMamager.Rows != null && dicMamager.Rows.Count > 0)
                {
                    foreach (DataRow row in dicMamager.Rows)
                    {
                        int temp;
                        int.TryParse(row[0].ToString(), out temp);
                        if (temp == paramsInfo.FinanceMarkId)
                        {
                            paramsInfo.FinanceMarkName = row[1].ToString();
                        }
                    }
                }
                var department = DalPromotionJob.GetDepartmentUseSetting();
                if (department != null && department.Rows != null && department.Rows.Count > 0)
                {
                    var departmentTemp = department.Rows.OfType<DataRow>().Select(s => new DepartmentAndUse { ParentSettingId = s["ParentSettingId"].ToString(), SettingId = s["SettingId"].ToString(), DisplayName = s["DisplayName"].ToString(), Type = s["Type"].ToString() }).ToList();
                    var tempModel1 = departmentTemp.Where(w => w.SettingId == paramsInfo.DepartmentId.ToString()).FirstOrDefault();
                    if (tempModel1 != null)
                    {
                        paramsInfo.DepartmentName = tempModel1.DisplayName;
                    }
                    var tempModel2 = departmentTemp.Where(w => w.SettingId == paramsInfo.IntentionId.ToString()).FirstOrDefault();
                    if (tempModel2 != null)
                    {
                        paramsInfo.IntentionName = tempModel2.DisplayName;
                    }
                }
                var businessLines = DALPromotion.GetAllBusinessLinesById(paramsInfo.BusinessLineId);
                if (businessLines != null && businessLines.Rows.Count > 0)
                {
                    paramsInfo.BusinessLineName = businessLines.Rows[0].GetValue<string>("DisplayName");
                }
                if (paramsInfo.IsPush == 1)
                {
                    if (string.IsNullOrEmpty(paramsInfo.PushSetting))
                    {
                        return Json(-1);
                    }
                    var arr = paramsInfo.PushSetting.Split(new[] { ",", "，" }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => int.TryParse(x, out int result)).ToArray();
                    if (arr.Length == 0)
                    {
                        info.IsSuccess = false;
                        info.OutMessage = "优惠券提前提醒天数格式不正确";
                        return Json(info);
                    }
                    else
                    {
                        paramsInfo.PushSetting = string.Join(",", arr);
                    }
                }
                else
                {
                    paramsInfo.PushSetting = null;
                }

                paramsInfo.Creater = User.Identity.Name;
                paramsInfo.Issuer = User.Identity.Name;
                var taskPromotionListId = DalPromotionJob.CreateOrUpdatePromotionTaskPromotionListNew(paramsInfo);
                info.IsSuccess = true;
                info.OutMessage = "添加成功";
                info.ObjectData = taskPromotionListId;
            }
            catch (Exception ex)
            {
                info.OutMessage = "操作异常！~请与管理员联系";
            }
            return Json(info);
        }

        [HttpPost]
        public JsonResult FilterUserByCondition(PromotionTaskFilterInfo paramsInfo)
        {
            ResponseModel info = new ResponseModel { IsSuccess = false };

            try
            {
                DataTable dt = DalPromotionJob.FilterUserByCondition(paramsInfo);
                info.ObjectData = (dt == null ? 0 : dt.Rows.Count);
            }
            catch (Exception)
            {
                info.OutMessage = "操作异常！~请与管理员联系";
            }
            return Json(info);
        }
        [HttpPost]
        public ActionResult DownLoadFileFilterUser(PromotionTaskFilterInfo paramsInfo)
        {
            var modellist = DalPromotionJob.FilterUserByCondition(paramsInfo).ConvertTo<ThBiz.DataAccess.Entity.NotifyUserModel>();
            string[] headers = new string[3];
            headers[0] = "订单号";
            headers[1] = "用户ID";
            headers[2] = "手机号";
            MemoryStream stream = GenerateXls(headers, modellist);
            byte[] bytearray = stream.ToArray();
            stream.Close();
            Response.AppendHeader("Content-Disposition", "attachment;filename=FilterUsers.xlsx");
            Response.AddHeader("Content-Length", bytearray.Length.ToString());
            return File(bytearray, "application/octet-stream");
        }
        private MemoryStream GenerateXls(string[] fileheaders, IEnumerable<ThBiz.DataAccess.Entity.NotifyUserModel> models)
        {
            MemoryStream stream = new MemoryStream();
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("筛选出的用户列表");

            IRow headerrow = sheet.CreateRow(0);
            for (int i = 0; i < fileheaders.Length; i++)
            {
                ICell cell = headerrow.CreateCell(i);
                cell.SetCellType(CellType.String);
                cell.SetCellValue(fileheaders[i]);
                if (i == 1)
                {
                    sheet.SetColumnWidth(i, 40 * 256);
                }
                else
                {
                    sheet.SetColumnWidth(i, 20 * 256);
                }
            }
            int index = 1;
            foreach (var model in models)
            {
                IRow row = sheet.CreateRow(index);
                ICell[] cells = new ICell[fileheaders.Length];
                for (int i = 0; i < fileheaders.Length; i++)
                {
                    cells[i] = row.CreateCell(i);
                    cells[i].SetCellType(CellType.String);
                }
                cells[0].SetCellValue(model.OrderNo);
                cells[1].SetCellValue(model.UserID);
                cells[2].SetCellValue(model.MobileNum);
                index++;
            }
            workbook.Write(stream);
            return stream;
        }

        /// <summary>
        /// 提交优惠券任务
        /// </summary>
        /// <param name="promotionTaskParams">优惠券任务对象</param>
        /// <returns></returns>
#if !DEBUG
        [PowerManage]
#endif
        [HttpPost]
        public ActionResult CreatePromotionTask(PromotionTaskParamsModel promotionTaskParams)
        {
            try
            {
                //if (!cellphoneCreateUsers.Contains(User.Identity.Name, StringComparer.CurrentCultureIgnoreCase))
                //    return HttpNotFound();

                if (promotionTaskParams.TaskStartTime > promotionTaskParams.TaskEndTime)
                    return Json(new { Result = -2 });

                //用户列表
                var cellPhones = new List<string>();
                //如果是上传文件，则得到上传文件的数据
                if (promotionTaskParams.SelectUserType == PromotionConsts.SelectUserTypeEnum.UploadFile)
                {
                    var cellphonesFile = Request.Files.Count > 0 ? Request.Files[0] : null;
                    if (cellphonesFile == null || cellphonesFile.ContentLength < 11)
                        return Json(new { Result = -5 });

                    using (var stream = new StreamReader(cellphonesFile.InputStream, Encoding.UTF8))
                    {
                        string[] tmpStr = Regex.Split(stream.ReadToEnd(), @"\D");
                        var tmpInfo = tmpStr.Where(cellphone => !string.IsNullOrEmpty(cellphone)).Distinct().ToList();
                        var info = tmpInfo.Where(cellphone => Regex.IsMatch(cellphone, @"^1\d{10}$")).ToList();

                        if (tmpInfo.Count > info.Count)
                        {
                            return Json(new { Result = -6 });
                        }
                        cellPhones.AddRange(info);
                    }

                    if (cellPhones.Count < 1)
                        return Json(new { Result = -5 });
                }
                if (promotionTaskParams.SmsId == 0)
                {
                    promotionTaskParams.SmsParam = null;
                }
                if (!string.IsNullOrEmpty(promotionTaskParams.SmsParam))
                {
                    try
                    {
                        JsonConvert.DeserializeObject<List<string>>(promotionTaskParams.SmsParam);
                    }
                    catch (Exception e)
                    {
                        return Json(new { Result = -7 });
                    }
                }
                var manager = new PromotionCodeManager();
                var promotionTask = new PromotionTask()
                {
                    PromotionTaskId = promotionTaskParams.PromotionTaskId,
                    PromotionListIds = promotionTaskParams.PromotionListIds,
                    TaskName = promotionTaskParams.TaskName,
                    TaskType = (int)promotionTaskParams.TaskType,
                    TaskStartTime = promotionTaskParams.TaskStartTime,
                    TaskEndTime = promotionTaskParams.TaskEndTime,
                    ExecPeriod = promotionTaskParams.ExecPeriod,
                    SelectUserType = (int)promotionTaskParams.SelectUserType,
                    FilterStartTime = promotionTaskParams.FilterStartTime,
                    FilterEndTime = promotionTaskParams.FilterEndTime,
                    Brand = promotionTaskParams.Brand?.Trim().Replace("，", ","),
                    Category = promotionTaskParams.Category?.Trim().Replace("，", ","),
                    Pid = promotionTaskParams.Pid?.Trim().Replace("，", ","),
                    SpendMoney = promotionTaskParams.SpendMoney,
                    PurchaseNum = promotionTaskParams.PurchaseNum,
                    Area = promotionTaskParams.Area?.Trim().Replace("，", ","),
                    Channel = promotionTaskParams.Channel?.Trim().Replace("，", ","),
                    OrderType = promotionTaskParams.OrderType,
                    InstallType = promotionTaskParams.InstallType,
                    OrderStatus = promotionTaskParams.OrderStatus,
                    Vehicle = promotionTaskParams.Vehicle,
                    IsLimitOnce = promotionTaskParams.IsLimitOnce == 1,
                    IsImmediately = 0,
                    SmsId = promotionTaskParams.SmsId,
                    SmsParam = promotionTaskParams.SmsParam,
                    PromotionTaskActivityId = promotionTaskParams.PromotionTaskActivityId
                };
                //判断是否是立即塞券
                if (promotionTaskParams.IsImmediately == 1)
                {
                    if (promotionTaskParams.SelectUserType == PromotionConsts.SelectUserTypeEnum.UploadFile && cellPhones.Count <= 200)
                    {
                        promotionTask.IsImmediately = 1;
                    }
                    else if (promotionTaskParams.SelectUserType == PromotionConsts.SelectUserTypeEnum.BiActivity && promotionTaskParams.PromotionTaskActivityId != null)
                    {
                        var count = DalPromotionJob.GetActivityUsersCount(promotionTaskParams.PromotionTaskActivityId
                            .Value);
                        if (count <= 200)
                        {
                            //同步bi数据库的数据到临时发送的表里
                            promotionTask.IsImmediately = 1;
                        }
                    }
                }
                if (promotionTask.IsImmediately == 1)
                {
                    promotionTask.TaskStartTime = System.DateTime.MaxValue;
                }

                //执行创建优惠券任务
                List<string> taskPromotionListIds = promotionTaskParams.TaskPromotionListIds.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                manager.CreatePromotionTask(promotionTask, taskPromotionListIds, cellPhones);

                //创建成功
                var dic = new Dictionary<string, object> { ["Result"] = cellPhones.Count };

                //发送邮件通知相关人员  优惠券塞券系统增加监控邮件功能
                if (promotionTaskParams.TaskType == PromotionTaskType.Once && cellPhones.Count > 50000)
                {
                    try
                    {
                        var emailProcess = new BizEmailProcess
                        {
                            url = string.Empty,
                            ToMail = "wangminyou@tuhu.cn,liuchao1@tuhu.cn,zhuzhiyuan@tuhu.cn",
                            CC = null,
                            Status = "New",
                            Type = "Email",
                            Subject = "优惠券塞券：单次任务上传手机号超过5W",
                            Body = string.Format($"优惠券塞券=>任务名称：{promotionTaskParams.TaskName}；单次任务上传手机号个数：{cellPhones.Count}；操作人：{User.Identity.Name}；操作时间：{DateTime.Now}"),
                            OrderNo = string.Empty
                        };
                        new OprLogManager().AddEmailProcess(emailProcess);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return JavaScript(JsonConvert.SerializeObject(dic));
            }
            catch (Exception ex)
            {
                return Json(new { Result = -1 });
            }
        }
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult GetPromotionRuleName(int promotionRuleId)
        {
            var manager = new PromotionCodeManager();
            var promotionRuleName = manager.GetPromotionRuleNameById(promotionRuleId);
            return Content(promotionRuleName);
        }

        public JsonResult GetVehicleBrands()
        {
            var manager = new VehicleTypeManager();

            var data = manager.GetAllVehicleBrands();

            if (data != null && data.Count() > 0)
            {
                return Json(new { status = "success", data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail", data = data }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetVehicleSeries(string brand)
        {
            var manager = new VehicleTypeManager();

            var data = manager.GetVehicleSeries(brand);

            if (data != null && data.Count() > 0)
            {
                return Json(new { status = "success", data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail", data = data }, JsonRequestBehavior.AllowGet);
            }
        }
        private List<Dictionary> GetAllOrderChannel()
        {
            if (HttpRuntime.Cache[ChannelKey] == null)
            {
                var manager = new DalPromotionJob();
                var categories = DalPromotionJob.GetAllOrderChannel();
                HttpRuntime.Cache.Insert(ChannelKey, categories, null, DateTime.Now.AddHours(2), Cache.NoSlidingExpiration);
            }

            return HttpRuntime.Cache[ChannelKey] as List<Dictionary>;
        }

        public ActionResult SendPromotionTaskMQ()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PostSendPromotionTaskMQ(SendPromotionTaskMQModel model)
        {
            TuhuNotification.SendNotification(model.RouteKey, new
            {
                model.PromotionTaskId,
                model.MinPkid,
                model.OrderNo,
                model.ExcuteType
            }, 10000);
            return Json(new { Code = 1 });
        }

        public ActionResult OrderChannelDetail(string channel)
        {
            return PartialView(DalPromotionJob.GetOrderChannelChildren(channel));
        }

        public ActionResult HistoryActions(string objectId, string objectType)
        {
            var content = "";
            var logs = PromotionManager.GetPromotionLog(objectId, objectType);
            System.Text.StringBuilder sbr = new System.Text.StringBuilder();
            sbr.AppendFormat(@"<table><tr><td>序号</td><td>操作</td><td>操作人账号</td><td>操作时间</td><td>查看详细 </td></tr>");
            int k = logs.Rows.Count;
            for (var i = 0; i < logs.Rows.Count; i++)
            {
                DataRow h = logs.Rows[i];
                string beforeJson = "";
                if ((i + 1) < logs.Rows.Count)
                {
                    DataRow b = logs.Rows[i + 1];
                    beforeJson = b.GetValue<string>("AfterValue");
                }
                sbr.Append(
                        $@"<tr><td>{k}</td><td>{h.GetValue<string>("Operate")}</td><td>{h.GetValue<string>("Author")}</td>
                    <td>{h.GetValue<DateTime>("CreateTime")}</td>
                    <td><a href='javascript:void(0)' data-type='viewPromotionLogDetail' data-beforejson='{beforeJson}' data-json='{h.GetValue<string>("AfterValue")}'>查看修改详细</a></td></tr>");
                k--;
            }
            sbr.AppendFormat("</table>");
            content = sbr.ToString();
            return Content(content);
        }

        public ActionResult SelectByGiftAndIndent()
        {
            return View();
        }

        public ActionResult SelectAllByPhoneNumbers(string Phone)
        {
            if (Phone != null && Phone != "")
            {
                var exm = new PromotionCodeManager();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                int TabIndex = 0;
                var ds = exm.SelectByPhoneNumk(Phone, TabIndex = 0);
                var ds1 = exm.SelectByPhoneNumk(Phone, TabIndex = 1).Rows.OfType<DataRow>().Select(x => new { PKID = x["PKID"], Code = x["Code"], Type = x["Type"], RuleID = x["RuleID"], Description = x["Description"], Status = x["Status"], OrderId = x["OrderId"], StartTime = Convert.ToDateTime(x["StartTime"]).ToString("yyyy-MM-dd"), EndTime = Convert.ToDateTime(x["EndTime"]).ToString("yyyy-MM-dd"), CreateTime = Convert.ToDateTime(x["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss") });

                var ds2 = exm.SelectByPhoneNumk(Phone, TabIndex = 2).Rows.OfType<DataRow>().Select(x => new { PKID = x["PKID"], Code = x["Code"], Type = x["Type"], RuleID = x["RuleID"], Description = x["Description"], Status = x["Status"], OrderId = x["OrderId"], StartTime = Convert.ToDateTime(x["StartTime"]).ToString("yyyy-MM-dd"), EndTime = Convert.ToDateTime(x["EndTime"]).ToString("yyyy-MM-dd"), CreateTime = Convert.ToDateTime(x["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss") });

                var ds3 = exm.SelectByPhoneNumk(Phone, TabIndex = 3).Rows.OfType<DataRow>().Select(x => new { PKID = x["PKID"], Code = x["Code"], Type = x["Type"], RuleID = x["RuleID"], Description = x["Description"], OrderNo = x["OrderNo"], Status = x["Status"], SumMoney = x["SumMoney"], SumPaid = x["SumPaid"], OrderProducts = x["OrderProducts"], Remark = x["Remark"], PayStatus = x["PayStatus"], OrderDatetime = Convert.ToDateTime(x["OrderDatetime"]).ToString("yyyy-MM-dd HH:mm:ss") });
                dic.Add("ds", ds);
                dic.Add("ds1", ds1);
                dic.Add("ds2", ds2);
                dic.Add("ds3", ds3);
                var result = JsonConvert.SerializeObject(dic);
                return Content(result);
            }
            return View();
        }

        /// <summary>
        /// 优惠券延期
        /// </summary>
        /// <param name="promoCode"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExtendCouponEndTime(int promoCode, Guid userId, DateTime endDate)
        {
            var request = new Service.Member.Request.UpdateUserPromotionCodeEndTimeRequest()
            {
                Channel = "SelectPromotionByPhone",
                EndTime = endDate,
                OperationAuthor = ThreadIdentity.Operator.Name,
                PromotionCodeId = promoCode,
                UserID = userId
            };
            using (var client = new PromotionClient())
            {
                var response = client.UpdateUserPromotionCodeEndTime(request);
                response.ThrowIfException(true);
            }
            using (var client = new ConfigLogClient())
            {
                var response = client.InsertDefaultLogQueue("PromotionConfigLog", JsonConvert.SerializeObject(new
                {
                    ObjectId = promoCode,
                    ObjectType = "SelectPromotionByPhone",
                    BeforeValue = "",
                    AfterValue = JsonConvert.SerializeObject(request),
                    Operate = "修改优惠券到期时间",
                    Author = ThreadIdentity.Operator.Name
                }));
                response.ThrowIfException(true);
            }
            return Json(new { Code = 1 });
        }

        /// <summary>
        /// 优惠券作废
        /// </summary>
        /// <param name="promoCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExtendCouponStatus(int promoCode, Guid userId)
        {
            var request = new Service.Member.Request.UpdateUserPromotionCodeStatusRequest()
            {
                Channel = "SelectPromotionByPhone",
                UserID = userId,
                OperationAuthor = ThreadIdentity.Operator.Name,
                Status = 3,
                PromotionCodeId = promoCode
            };
            using (var client = new PromotionClient())
            {
                var response = client.UpdateUserPromotionCodeStatus(request);
                response.ThrowIfException(true);
            }
            using (var client = new ConfigLogClient())
            {
                var response = client.InsertDefaultLogQueue("PromotionConfigLog", JsonConvert.SerializeObject(new
                {
                    ObjectId = promoCode,
                    ObjectType = "SelectPromotionByPhone",
                    BeforeValue = "",
                    AfterValue = JsonConvert.SerializeObject(request),
                    Operate = "作废优惠券",
                    Author = ThreadIdentity.Operator.Name
                }));
                response.ThrowIfException(true);
            }
            return Json(new { Code = 1 });
        }
    }
}
