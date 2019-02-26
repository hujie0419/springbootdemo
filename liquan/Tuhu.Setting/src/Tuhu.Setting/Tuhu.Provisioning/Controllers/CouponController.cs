using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponManager manager = new CouponManager();
        #region CouponCategory
        [PowerManage]
        public ActionResult CouponCategory()
        {
            List<CouponCategory> _AllCouponCategory = manager.GetAllCouponCategory();
            ViewBag.CouponCategory = _AllCouponCategory;
            return View();
        }
        [HttpPost]
        public ActionResult DeleteCouponCategory(int id)
        {
            try
            {
                manager.DeleteCouponCategory(id);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 缓存下拉框数据
        /// </summary>
        /// <returns></returns>
        private DataTable SelectDropDownList()
        {
            DataTable data = null;
            if (Session["SelectDropDownList"] == null)
            {
                data = manager.SelectDropDownList();
                Session["SelectDropDownList"] = data;

                if (data != null && data.Rows.Count > 0)
                    return data;
                else
                    return null;
            }
            else
            {
                data = (DataTable)Session["SelectDropDownList"];
                if (data != null && data.Rows.Count > 0)
                    return data;
                else
                    return null;
            }
        }

        public ActionResult AddCouponCategory(int? id)
        {
            var _CouponTypeTable = SelectDropDownList();
            List<SelectListItem> _CouponTypeList = new List<SelectListItem>();
            if (_CouponTypeTable != null && _CouponTypeTable.Rows.Count > 0)
            {
                for (int i = 0; i < _CouponTypeTable.Rows.Count; i++)
                {
                    _CouponTypeList.Add(new SelectListItem { Value = _CouponTypeTable.Rows[i]["PKID"].ToString(), Text = _CouponTypeTable.Rows[i]["Name"].ToString() });
                }
            }

            ViewBag.CouponTypeList = _CouponTypeList;

            int _PKID = id.GetValueOrDefault(0);
            if (_PKID != 0)
            {
                ViewBag.Title = "修改红包分类";
                return View(manager.GetCouponCategoryByID(_PKID));
            }
            else
            {
                ViewBag.Title = "新增红包分类";
                CouponCategory _Model = new CouponCategory()
                {
                    PKID = 0,
                    ValidDays = 7,
                    IsActive = 0
                };
                return View(_Model);
            }
        }
        [HttpPost]
        public ActionResult AddCouponCategory(CouponCategory couponCategory)
        {
            string kstr = "";
            bool IsSuccess = true;
            if (string.IsNullOrEmpty(couponCategory.Name))
            {
                kstr = "名称不能为空";
                IsSuccess = false;
            }
            if (string.IsNullOrEmpty(couponCategory.EnID))
            {
                kstr = "英文标识不能为空";
                IsSuccess = false;
            }
            int _ExistPKID = manager.GetPKIDByEnID(couponCategory.EnID);
            if (_ExistPKID != 0)
            {
                if (couponCategory.PKID == 0)
                {
                    kstr = "新增失败，该英文标识已存在";
                    IsSuccess = false;
                }
                else if (couponCategory.PKID != _ExistPKID)
                {
                    kstr = "修改失败，该英文标识已存在";
                    IsSuccess = false;
                }
            }
            if (IsSuccess)
            {
                //为兼容老旧优惠券类型
                string _type = SelectDropDownList().Select("PKID = " + couponCategory.RuleID + "")[0]["Type"].ToString();

                if (string.IsNullOrEmpty(_type))
                    couponCategory.CouponType = null;
                else
                    couponCategory.CouponType = System.Convert.ToInt32(_type);

                if (couponCategory.PKID == 0)
                {
                    manager.AddCouponCategory(couponCategory);
                }
                else
                {
                    manager.UpdateCouponCategory(couponCategory);
                }
                return RedirectToAction("CouponCategory");
            }
            else
            {
                string js = "<script>alert(\"" + kstr + "\");location='CouponCategory';</script>";
                return Content(js);
            }
        }
        [HttpPost]
        public JsonResult SavePercentage(string opstr)
        {
            try
            {
                if (string.IsNullOrEmpty(opstr))
                {
                    return Json(new { IsSuccess = false, ReturnStr = "输入的参数不能为空" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string[] _Oparr = opstr.Split('*');
                    foreach (string Paras in _Oparr)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(Paras))
                            {
                                string[] _Para = Paras.Split(',');
                                if (_Para.Length == 2)
                                {
                                    string _PKID = _Para[0];
                                    string _PERC = _Para[1];
                                    if (!string.IsNullOrEmpty(_PKID) && !string.IsNullOrEmpty(_PERC))
                                    {
                                        manager.UpdateCouponCategoryPercentage(int.Parse(_PKID), int.Parse(_PERC));
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
                return Json(new { IsSuccess = true, ReturnStr = "批量保存成功" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { IsSuccess = false, ReturnStr = "保存失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        public ActionResult Coupon(int? CategoryID)
        {
            int _CategoryID = CategoryID.GetValueOrDefault(0);
            if (_CategoryID != 0)
            {
                CouponCategory _CouponCategory = manager.GetCouponCategoryByID(_CategoryID);
                if (_CouponCategory != null && _CouponCategory.PKID != 0)
                {
                    List<Coupon> _CouponList = manager.GetCouponByCategoryID(_CategoryID);
                    ViewBag.CouponCategory = _CouponCategory;
                    ViewBag.CouponList = _CouponList;
                    return View();
                }
                else
                {
                    return Content("<script>alert('该分类不存在，请从正常来源进入');location='/Coupon/CouponCategory'</script>");
                }
            }
            else
            {
                return Content("<script>alert('该分类不存在，请从正常来源进入');location='/Coupon/CouponCategory'</script>");
            }
        }
        [HttpPost]
        public ActionResult DeleteCoupon(int id)
        {
            try
            {
                manager.DeleteCoupon(id);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddCoupon(int? id, int? CategoryID)
        {
            int _PKID = id.GetValueOrDefault(0);
            if (_PKID != 0)
            {
                ViewBag.Title = "修改红包";
                return View(manager.GetCouponByID(_PKID));
            }
            else
            {
                ViewBag.Title = "新增红包";
                Coupon _Model = new Coupon()
                {
                    PKID = 0,
                    IsActive = 0,
                    CategoryID = CategoryID.GetValueOrDefault(0)
                };
                return View(_Model);
            }
        }
        [HttpPost]
        public ActionResult AddCoupon(Coupon coupon)
        {
            if (coupon.PKID == 0)
            {
                manager.AddCoupon(coupon);
            }
            else
            {
                manager.UpdateCoupon(coupon);
            }
            return Redirect("Coupon?CategoryID=" + coupon.CategoryID);
        }
    }
}