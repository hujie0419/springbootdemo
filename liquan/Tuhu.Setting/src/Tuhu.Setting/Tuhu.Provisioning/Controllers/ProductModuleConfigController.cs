using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.Business.ProductDescModule;
using Tuhu.Provisioning.Business.Promotion;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class ProductModuleConfigController : Controller
    {
        private readonly Lazy<LoggerManager> lazy = new Lazy<LoggerManager>();

        private LoggerManager LoggerManager
        {
            get { return lazy.Value; }

        }
        [PowerManage]
        public ActionResult Index(string pid, string moduleName, int isActive = -1, int isAdvert = -1, int pageIndex = 1)
        {
            List<ProductDescriptionModel> data = null;
            var totalCount = 0;

            if (!string.IsNullOrEmpty(pid))
            {
                pid = pid.Replace('/', '|');
            }

            data = ProductModuleConfigManager.SelectAllProductConfig(5, pageIndex, pid, isActive, isAdvert, moduleName, out totalCount).ToList();

            ProductDescriptionViewModel model = new ProductDescriptionViewModel()
            {
                productDescriptionList = ProductDescriptionViewModel.ConvertToList(data),
            };

            ViewBag.IsAdvert = isAdvert;
            ViewBag.PID = pid;
            ViewBag.IsActive = isActive;
            ViewBag.ModuleName = moduleName;

            var list = new OutData<List<ProductDescriptionViewModel>, int>(model.productDescriptionList, totalCount);
            var pager = new PagerModel(pageIndex, 5)
            {
                TotalItem = totalCount
            };

            if (model == null)
                return View();
            return this.View(new ListModel<ProductDescriptionViewModel>(list.ReturnValue, pager));
        }

        [PowerManage]
        public ActionResult AddProductDescModule(string id)
        {
            ProductDescriptionViewModel model = new ProductDescriptionViewModel();
            if (string.IsNullOrWhiteSpace(id))
            {
                return View(new ProductDescriptionViewModel());
            }
            var pkid = Convert.ToInt32(id);
            var result = ProductModuleConfigManager.GetProductDescModuleDetail(pkid).ToList();

            model = ProductDescriptionViewModel.ConvertToList(result).FirstOrDefault();

            return View(model);
        }

        [PowerManage]
        public ActionResult InsertOrUpdateProductConfig(string productModel, string platformConfig, string categoryData, string pid,
            string brand, string parentID)
        {
            var result = -1;

            var productModuleModel = JsonConvert.DeserializeObject<ProductDescriptionModel>(productModel);
            var categoryList = JsonConvert.DeserializeObject<List<ProductDescriptionModel>>(categoryData);
            var pidList = JsonConvert.DeserializeObject<List<ProductDescriptionModel>>(pid);
            var platformList = JsonConvert.DeserializeObject<List<ProductDescriptionModel>>(platformConfig);
            var brandList = JsonConvert.DeserializeObject<List<ProductDescriptionModel>>(brand);
            var userName = HttpContext.User.Identity.Name;
            var value = productModel + categoryData + pid + brand;

            foreach (var item in platformList)
            {
                item.ModuleContent = Server.UrlDecode(item.ModuleContent);
                item.ModuleContent = item.ModuleContent.Replace('&', '"');
            }
            productModuleModel.ModuleContent = Server.UrlDecode(productModuleModel.ModuleContent);

            if (String.IsNullOrWhiteSpace(parentID))
            {
                var moduleID = 0;
                result = ProductModuleConfigManager.InsertProductConfig(productModuleModel, platformList, categoryList, pidList,
                    brandList, userName, out moduleID);
                if (result > 0 && moduleID > 0)
                {
                    var log = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                    {
                        ObjectID = moduleID,
                        ObjectType = "ProConfig",
                        BeforeValue = "New",
                        AfterValue = value,
                        Author = userName,
                        Operation = "添加商品配置信息"
                    };
                    new OprLogManager().AddOprLog(log);
                }
            }
            else
            {
                result = ProductModuleConfigManager.UpdateProductConfig(productModuleModel, platformList, categoryList, pidList, brandList, Convert.ToInt32(parentID), userName);
                if (result > 0)
                {
                    var log = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                    {
                        ObjectID = Convert.ToInt32(parentID),
                        ObjectType = "ProConfig",
                        BeforeValue = "Update",
                        AfterValue = value,
                        Author = userName,
                        Operation = "更改商品配置信息"
                    };
                    new OprLogManager().AddOprLog(log);
                }
            }

            return Json(result);
        }

        [PowerManage]
        public ActionResult DeleteProductAllInfo(int PKID)
        {
            var userName = HttpContext.User.Identity.Name;
            var result = -1;

            result = ProductModuleConfigManager.DeleteProductAllInfo(PKID, userName);
            if (result > 0)
            {
                var log = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                {
                    ObjectID = PKID,
                    ObjectType = "ProConfig",
                    BeforeValue = "Delete",
                    AfterValue = "",
                    Author = userName,
                    Operation = "删除商品配置信息"
                };
                new OprLogManager().AddOprLog(log);
            }
            return Json(result);
        }

        public ActionResult SelectOprLogByPKID(int id)
        {
            var result = LoggerManager.SelectOprLogByParams("ProConfig", id.ToString());

            return result.Count() > 0 && result != null
                ? Json(new { status = "success", data = result }, JsonRequestBehavior.AllowGet)
                : Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ImageUpload()
        {
            byte[] uploadFileBytes = null;
            string uploadFileName = null;
            string _BImage = string.Empty;
            string _ImgGuid = Guid.NewGuid().ToString();
            Exception ex = null;
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    uploadFileName = Imgfile.FileName;
                    uploadFileBytes = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(uploadFileBytes, 0, uploadFileBytes.Length);
                    var _BytToImg = BytToImg(uploadFileBytes);
                    if (_BytToImg != null && _BytToImg.Count > 0)
                    {
                        _ImgGuid = string.Format(_ImgGuid + "${0}w_{1}h", _BytToImg["Width"], _BytToImg["Height"]);
                    }

                    var pathFormat = WebConfigurationManager.AppSettings["UploadDoMain_image"];

                    _BImage = ProductModuleConfigManager.ImageUploadFile(pathFormat, uploadFileBytes, uploadFileName, 100);
                }
                catch (Exception error)
                {
                    ex = error;
                }
            }
            return Json(new
            {
                BImage = WebConfigurationManager.AppSettings["DoMain_image"] + _BImage,
                SImage = WebConfigurationManager.AppSettings["DoMain_image"] + _BImage,
                Msg = ex == null ? "上传成功" : ex.Message
            }, "text/html");
        }

        public static Dictionary<string, object> BytToImg(byte[] byt)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(byt))
                {
                    using (Image img = Image.FromStream(ms))
                    {
                        Dictionary<string, object> dicToImg = new Dictionary<string, object>();
                        dicToImg.Add("Width", img.Width);
                        dicToImg.Add("Height", img.Height);
                        return dicToImg;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public ActionResult GetCategory(string type)
        {
            var source= PromotionManager.SelectProductCategoryCategoryNameAndDisplayName().Where(s => s.ParentCategory == null || !s.ParentCategory.Any()).ToList();
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
                    name = r.DisplayName,
                    open = false,
                    title = r.CategoryName,
                    children = r.ChildrenCategory.Select(c => new { name = c.DisplayName, title = c.CategoryName })
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
    }
}