using Microsoft.ApplicationBlocks.Data;
using Qiniu.IO;
using Qiniu.RS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{

    public class AdvertiseController : Controller
    {
        IAutoSuppliesManager manager = new AutoSuppliesManager();
        #region 汽车用品模块设置
		[PowerManage]
        public ActionResult SuppliesModule()
        {
            List<Advertise> _AllAdvertise = manager.GetSuppliesModule();
            ViewBag.SuppliesModule = _AllAdvertise.Where(p => p.AdColumnID == "wx-01").OrderBy(p => p.Position).ToList();
            ViewBag.WwwAdvertise = _AllAdvertise.Where(p => "www-01,www-02,www-03,www-04".Contains(p.AdColumnID)).OrderBy(p => p.Position).ToList();
            ViewBag.WWW05 = _AllAdvertise.Where(p => p.AdColumnID.Equals("www-05")).OrderBy(p => p.Position).ToList();
			ViewBag.WWW06 = _AllAdvertise.Where(p => p.AdColumnID.Equals("www-06")).OrderBy(p => p.Position).ToList();
			ViewBag.WWW07 = _AllAdvertise.Where(p => p.AdColumnID.Equals("www-07")).OrderBy(p => p.Position).ToList();
			ViewBag.WWW08 = _AllAdvertise.Where(p => p.AdColumnID.Equals("www-08")).OrderBy(p => p.Position).ToList();
			ViewBag.WWW_ShanGou = _AllAdvertise.Where(p => p.AdColumnID.Equals("www-shangou")).OrderBy(p => p.Position).ToList();
            ViewBag.Android001List = _AllAdvertise.Where(p => p.AdColumnID.Equals("android-001")).OrderBy(p => p.Position).ToList();
            ViewBag.Android002List = _AllAdvertise.Where(p => p.AdColumnID.Equals("android-002")).OrderBy(p => p.Position).ToList();
            ViewBag.Android003List = _AllAdvertise.Where(p => p.AdColumnID.Equals("android-003")).OrderBy(p => p.Position).ToList();
            ViewBag.IOS001List = _AllAdvertise.Where(p => p.AdColumnID.Equals("ios-001")).OrderBy(p => p.Position).ToList();
            ViewBag.IOS002List = _AllAdvertise.Where(p => p.AdColumnID.Equals("ios-002")).OrderBy(p => p.Position).ToList();
            ViewBag.IOS003List = _AllAdvertise.Where(p => p.AdColumnID.Equals("ios-003")).OrderBy(p => p.Position).ToList();
            ViewBag.BatteryBanner = _AllAdvertise.Where(p => p.AdColumnID.Equals("www-BatteryBanner")).OrderBy(p => p.Position).ToList();
            ViewBag.QuickBanner = _AllAdvertise.Where(p => p.AdColumnID.Equals("www-QuickBanner")).OrderBy(p => p.Position).ToList();

            return View();
        }
        public ActionResult InitSupplyModule(string col, string name, int? pos)
        {
            if (DateTime.Now > new DateTime(2015, 5, 11))
            {
                return Content("Expired");
            }
            IConnectionManager cm =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
            using (var conn = cm.OpenConnection())
            {
                SqlParameter parcol = new SqlParameter("@col", col);
                SqlParameter parnam = new SqlParameter("@name", name);
                SqlParameter parpos = new SqlParameter("@pos", pos.HasValue ? pos.Value : 0);
                var sql = "insert into gungnir..tbl_advertise(adcolumnid,name,position,begindatetime,EndDateTime,Image,Url,ShowType,State,CreateDateTime,LastUpdateDateTime,Platform,FunctionID,TopPicture,AdType,ProductID) values(@col,@name,@pos,'2015-01-16', '2015-01-16','http://wxbanner.qiniudn.com/wxbanner_775615a0-127a-4123-a266-4053b6932d6c.png','http://huodong.tuhu.cn/by/details.html',0,1,'2015-01-16','2015-01-16',4,'cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsWebViewUI|cn.TuHu.Activity.WashShopUI','',0,'')";
                var n = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parcol, parnam, parpos);
                WebLog.LogInfo("Add Index Config: " + User.Identity.Name);
                return Content("ok " + n.ToString());
            }
        }
        [HttpPost]
        public ActionResult DeleteItem(int id)
        {
            try
            {
                manager.DeleteAdvertise(id);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddSuppliesModule(int? id, string adColumnID)
        {
            if (id.HasValue)
            {
                ViewBag.Title = "修改模块";
                return View(manager.GetAdvertiseByID(id.Value));
            }
            else
            {
                if (!string.IsNullOrEmpty(adColumnID))
                {
                    ViewBag.Title = "新增模块";
                    Advertise _Model = new Advertise()
                    {
                        PKID = 0,
                        AdColumnID = adColumnID,//此处设置无用，直接在DAL里边直接添加了
                        Name = "",
                        Position = 0,
                        BeginDateTime = DateTime.Now.AddDays(1),
                        EndDateTime = DateTime.Now.AddMonths(1),
                        Image = "",
                        Url = "",
                        ActivityID="",
                        ShowType = 2,
                        State = 0,//默认0停用
                        Platform = 0,//默认0
                        FunctionID = "",
                        TopPicture = "",
                        AdType = 0,
                        ProductID = "",
                        CreateDateTime = DateTime.Now,
                        LastUpdateDateTime = DateTime.Now,
                        IsSendStamps = false,
                        ActivityKey = ""
                    };
                    return View(_Model);
                }
                else { return Content("请选择要添加模块的类型"); }
            }
        }
        #region 缩小图片
        /// <summary>缩小图片</summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <returns>缩小后的图片</returns>
        public static Image Reduces(Image rawImage, int width, int height)
        {
            return Reduces(rawImage, width, height, false);
        }

        /// <summary>缩小图片</summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="needFiller">是否需要补白，默认不补白</param>
        /// <returns>缩小后的图片</returns>
        public static Image Reduces(Image rawImage, int width, int height, bool needFiller)
        {
            if (width < 1 || height < 1)
                throw new ArgumentException();

            var rawWidth = rawImage.Width;
            var rawHeight = rawImage.Height;
            var newWidth = width;
            var newHeight = height;
            if (rawWidth <= width && rawHeight <= height)
            {
                newWidth = rawWidth;
                newHeight = rawHeight;
            }
            else if (width / height > rawWidth / rawHeight)
                newWidth = rawWidth * height / rawHeight;
            else
                newHeight = rawHeight * width / rawWidth;


            int startX = 0;
            int startY = 0;
            Bitmap bitmap;
            if (needFiller)
            {
                startX = (width - newWidth) / 2;
                startY = (height - newHeight) / 2;
                bitmap = new Bitmap(width, height);
            }
            else
                bitmap = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                //插值算法的质量
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.Clear(Color.White);

                graphics.FillRectangle(Brushes.White, startX, startY, newWidth, newHeight);
                graphics.DrawImage(rawImage, startX, startY, newWidth, newHeight);
                return bitmap;
            }
        }
        #endregion
        [HttpPost]
        public ActionResult AddSuppliesModule(Advertise advertise)
        {
            string kstr = "";
            bool IsSuccess = true;
            if (string.IsNullOrEmpty(advertise.Name))
            {
                kstr = "模块名称不能为空";
                IsSuccess = false;
            }
            if (string.IsNullOrEmpty(advertise.Image))
            {
                kstr = "图片不能为空";
                IsSuccess = false;
            }
            if (IsSuccess)
            {
                if (advertise.PKID == 0)
                {
                    advertise.CreateDateTime = DateTime.Now;
                    advertise.LastUpdateDateTime = DateTime.Now;
                    manager.AddAdvertise(advertise);
                }
                else
                {
                    advertise.LastUpdateDateTime = DateTime.Now;
                    manager.UpdateAdvertise(advertise);
                }
                return RedirectToAction("SuppliesModule");
            }
            else
            {
                string js = "<script>alert(\"" + kstr + "\");location='SuppliesModule';</script>";
                return Content(js);
            }
        }
        [HttpPost]
        public ActionResult AddWxBannerImg()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {

                    try
                    {
                        Image bitmap;
                        ImageFormat rawFormt;
                        using (var rawImage = Bitmap.FromStream(file.InputStream))
                        {
                            rawFormt = rawImage.RawFormat;
                            bitmap = Reduces(rawImage, 2000, 600);
                        }

                        using (var stream = new MemoryStream())
                        {
                            using (bitmap)
                            {
                                bitmap.Save(stream, rawFormt);
                            }
                            string _FileExt = ".png";
                            try
                            {
                                var _KFileName = file.FileName;
                                _FileExt = _KFileName.Substring(_KFileName.LastIndexOf("."), _KFileName.Length - _KFileName.LastIndexOf("."));
                            }
                            catch { }
                            string fileName = "wx_" + Guid.NewGuid().ToString() + _FileExt;
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
                                    return Content(WebConfigurationManager.AppSettings["Qiniu:comment_url"] + fileName);
                            }
                        }
                    }
                    catch { }
                    //catch (Exception ex) { }
                }
            }
            return Content("false");
        }
        #endregion

        #region ADProducts
        /// <summary>
        /// 获取模块下边所有产品
        /// </summary>
        /// <param name="AdvertiseID"></param>
        /// <returns></returns>
        public ActionResult ModuleProducts(int? AdvertiseID)
        {
            if (AdvertiseID.HasValue)
            {
                int _AdvertiseID = AdvertiseID.Value;
                Advertise _Advertise = manager.GetAdvertiseByID(_AdvertiseID);
                if (_Advertise != null)
                {
                    List<Tuhu.Provisioning.Models.AdProductItem> _AdProList = manager.GetAdProListByAdID(_AdvertiseID).Select(
                        p => new Tuhu.Provisioning.Models.AdProductItem
                        {
                            AdvertiseID = p.AdvertiseID,
                            PID = p.PID,
                            ProductName = _Advertise.AdColumnID == "www-01" ? manager.GetCateNameByCateID(p.PID) : manager.GetProductNameByPID(p.PID),
                            Position = p.Position,
                            State = p.State,
                            StateName = "<b>" + (p.State == 1 ? "<font color=\"#006600\">显示中" : "<font color=\"#FF0000\">已禁止") + "</font></b>",
                            PromotionPrice = p.PromotionPrice,
                            PromotionNum = p.PromotionNum
                        }
                        ).ToList();
                    ViewBag.Advertise = _Advertise;
                    ViewBag.AdProList = _AdProList;
                    return View();
                }
                else
                {
                    return Content("<script>alert('该模块不存在，请确认该模块是否被删除');location='/Advertise/SuppliesModule';</script>");
                }
            }
            else
            {
                return Content("<script>alert('该模块不存在，请从正常来源进入');location='/Advertise/SuppliesModule'</script>");
            }
        }

        [HttpPost]
        public ActionResult OperateProduct(int? AdvertiseID, string PID, string NewPID, byte? Position, byte? State, int? Cate, decimal? PromotionPrice, int? PromotionNum, string adColumnID)
        {
            try
            {
                if (!AdvertiseID.HasValue || !Cate.HasValue)
                {
                    return Json("请确保模块编号和操作类型不为空", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //删除产品
                    if (Cate == 1)
                    {
                        manager.DeleteAdProduct(AdvertiseID.GetValueOrDefault(0), PID);
                    }
                    //设置为已禁止
                    else if (Cate == 2)
                    {
                        manager.ChangeState(AdvertiseID.GetValueOrDefault(0), PID, State.GetValueOrDefault(0));
                    }
                    //保存产品
                    else if (Cate == 3)
                    {
                        manager.UpdateAdProduct(AdvertiseID.GetValueOrDefault(0), PID, NewPID, Position.GetValueOrDefault(0), PromotionPrice.GetValueOrDefault(0), PromotionNum.GetValueOrDefault(0));
                        if (adColumnID=="www-01")
                        {
                            if (manager.IsExistsxAdColumnID("www-" + NewPID))
                            {
                                Advertise _Model = new Advertise()
                                {
                                    AdColumnID = "www-" + NewPID,
                                    Name = manager.GetCateNameByCateID(NewPID),
                                    Position = 0,
                                    BeginDateTime = DateTime.Now.AddDays(1),
                                    EndDateTime = DateTime.Now.AddMonths(1),
                                    Image = "",
                                    Url = "",
                                    ShowType = 4,
                                    State = 0,
                                    Platform = 0,
                                    FunctionID = "",
                                    TopPicture = "",
                                    AdType = 0,
                                    ProductID = "",
                                    CreateDateTime = DateTime.Now,
                                    LastUpdateDateTime = DateTime.Now
                                };
                                manager.AddAdvertise(_Model);

                            }
                        }
                    }
                    //新增产品
                    else if (Cate == 4)
                    {
                        if (!Position.HasValue || !State.HasValue)
                        {
                            return Json("排序顺序和状态不能为空", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            manager.AddAdProduct(AdvertiseID.GetValueOrDefault(0), PID, Position.GetValueOrDefault(0), State.GetValueOrDefault(0), PromotionPrice.GetValueOrDefault(0), PromotionNum.GetValueOrDefault(0));
                            if (adColumnID == "www-01")
                            {
                                if (manager.IsExistsxAdColumnID("www-" + PID))
                                {
                                    Advertise _Model = new Advertise()
                                     {
                                       AdColumnID = "www-"+PID,
                                       Name = manager.GetCateNameByCateID(PID),
                                       Position = 0,
                                       BeginDateTime = DateTime.Now.AddDays(1),
                                       EndDateTime = DateTime.Now.AddMonths(1),
                                       Image = "",
                                       Url = "",
                                       ShowType = 4,
                                       State = 0,
                                       Platform = 0,
                                       FunctionID = "",
                                       TopPicture = "",
                                       AdType = 0,
                                       ProductID = "",
                                       CreateDateTime = DateTime.Now,
                                       LastUpdateDateTime = DateTime.Now
                                     };
                                    manager.AddAdvertise(_Model);

                                }
                            }
                        }
                    }
                    return Json("OK", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json("保存失败" + (Cate.HasValue && (Cate.Value == 3 || Cate.Value == 4) ? "请确认产品编号不要重复和顺序号为两位内整数" : ""), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult GetProductNamebyPID(string PID, int? Cate)
        {
            try
            {
                if (Cate.GetValueOrDefault(1) == 1)
                {
                    return Content(manager.GetProductNameByPID(PID));
                }
                else
                {
                    return Content(manager.GetCateNameByCateID(PID));
                }
            }
            catch
            {
                return Content("");
            }
        }

        [HttpPost]
        public ActionResult AllOperate(string opstr, int? AdvertiseID, int? Cate)
        {
            try
            {
                if (string.IsNullOrEmpty(opstr) || !AdvertiseID.HasValue || !Cate.HasValue)
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
                                if (_Para.Length == 5)
                                {
                                    string _PID = _Para[0];
                                    string _NewPID = _Para[1];
                                    byte _Position = byte.Parse(_Para[2]);
                                    decimal _PromotionPrice = decimal.Parse(_Para[3]);
                                    int _PromotionNum = int.Parse(_Para[4]);
                                    if (!string.IsNullOrEmpty(_PID) && !string.IsNullOrEmpty(_NewPID))
                                    {
                                        //删除
                                        if (Cate == 1)
                                        {
                                            manager.DeleteAdProduct(AdvertiseID.Value, _PID);
                                        }
                                        //保存
                                        else if (Cate == 2)
                                        {
                                            manager.UpdateAdProduct(AdvertiseID.Value, _PID, _NewPID, _Position, _PromotionPrice, _PromotionNum);
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
                return Json(new { IsSuccess = true, ReturnStr = Cate.Value == 1 ? "批量删除成功" : "批量保存成功" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { IsSuccess = false, ReturnStr = "保存失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ActionsetTab
        public ActionResult ActionsetTabs()
        {
            List<BizActionsetTab> _AllActionsetTabs = manager.GetAllActionsetTab();
            ViewBag.AllActionsetTabs = _AllActionsetTabs;
            return View();
        }
        [HttpPost]
        public ActionResult DeleteActionsetTab(int id)
        {
            try
            {
                manager.DeleteActionsetTab(id);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddActionsetTab(int? id)
        {
            if (id.HasValue && id.Value != 0)
            {
                ViewBag.Title = "修改模块";
                return View(manager.GetActionsetTabByID(id.Value));
            }
            else
            {
                ViewBag.Title = "新增模块";
                BizActionsetTab _Model = new BizActionsetTab()
                {
                    id = 0,
                    action_bgimg = "",
                    action_adproimg = "",
                    action_explain = "",
                    action_rule = "",
                    action_label = "",
                    action_proname = "",
                    action_proID = "",
                    action_proimg = "",
                    action_needfriendcount = 1,
                    action_totaljoinperson = 1,
                    action_totaldownprice = 1,
                    action_endTime = DateTime.Now.AddDays(1),
                    action_dealmoney = 1,
                    action_static = false,
                    action_introurl = "",
                    action_endurl = ""
                };
                return View(_Model);
            }
        }
        [HttpPost]
        public ActionResult AddActionsetTab(BizActionsetTab actionsetTab)
        {
            string kstr = "";
            bool IsSuccess = true;
            if (string.IsNullOrEmpty(actionsetTab.action_proID))
            {
                kstr = "所选产品不能为空";
                IsSuccess = false;
            }
            if (actionsetTab.action_dealmoney <= 0)
            {
                kstr = "产品价格必须大于0";
                IsSuccess = false;
            }
            if (IsSuccess)
            {
                if (actionsetTab.id == 0)
                {
                    actionsetTab.action_submitTime = DateTime.Now;
                    actionsetTab.action_updateTime = DateTime.Now;
                    manager.AddActionsetTab(actionsetTab);
                }
                else
                {
                    actionsetTab.action_updateTime = DateTime.Now;
                    manager.UpdateActionsetTab(actionsetTab);
                }
                return RedirectToAction("ActionsetTabs");
            }
            else
            {
                string js = "<script>alert(\"" + kstr + "\");location='ActionsetTabs';</script>";
                return Content(js);
            }
        }

        [HttpPost]
        public ActionResult AddActionImg()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    try
                    {
                        string _FileExt = ".png";
                        try
                        {
                            var _KFileName = file.FileName;
                            _FileExt = _KFileName.Substring(_KFileName.LastIndexOf("."), _KFileName.Length - _KFileName.LastIndexOf("."));
                        }
                        catch { }
                        string fileName = Guid.NewGuid().ToString() + _FileExt;
                        //string _Path = @"\\169.254.28.220\resource.9_9\";
                        string _Path = System.Configuration.ConfigurationManager.AppSettings["action99Path"];
                        file.SaveAs(_Path + fileName);
                        return Content("/images/9.9/" + fileName);
                        //file.SaveAs(@"E:\项目文档\201501\活动配置\测试上传图\" + fileName);
                        //return Content(@"file:\\\E:\项目文档\201501\活动配置\测试上传图\" + fileName);
                    }
                    catch (Exception ex)
                    {
                        return Content("2false" + ex.Message);
                    }
                }
            }
            return Content("1false");
        }
        #endregion

        /// <summary>
        /// 配置广告位类别下边的产品,根据类别的PID获得ColumnID
        /// </summary>
        public ActionResult GetPIDByColumnID(string ColumnID)
        {
            List<Advertise> _AllAdvertise = manager.GetSuppliesModule();
            var adproduct = _AllAdvertise.Where(p => ColumnID.Equals(p.AdColumnID,StringComparison.CurrentCultureIgnoreCase)).OrderBy(p => p.Position).FirstOrDefault();
            if (adproduct != null)
                return Json(adproduct.PKID);
            else
                return Json("false");
        }
    }
}