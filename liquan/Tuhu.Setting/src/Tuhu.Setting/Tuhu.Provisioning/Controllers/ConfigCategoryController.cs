using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.FileUpload;
using Tuhu.Provisioning.Business.ConfigCategory;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using swc = System.Web.Configuration;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using Tuhu.Provisioning.Business.ProductVehicleType;
using System.Text;
using System.Data;
using Tuhu.Provisioning.Business.Promotion;
using System.Threading.Tasks;
using Tuhu.Service.Product.Models.New;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Service.Utility.Request;
using Tuhu.Component.Framework.Extension;
using System.Web.Configuration;
using Tuhu.Provisioning.Common;

namespace Tuhu.Provisioning.Controllers
{
    public class ConfigCategoryController : Controller
    {
        public ActionResult ChangeCategoryList()
        {
            IEnumerable<GaiZhuangCategoryModel> list = ConfigCategoryManager.SelectChangeCategoryList();
            return View(list);
        }

        public ActionResult ChangeCategoryItem(string pkid = null)
        {
            GaiZhuangCategoryModel model = new GaiZhuangCategoryModel();
            model.IsShow = true;
            if (pkid != null)
            {
                model = ConfigCategoryManager.GetChangeCategoryModelByPKID(Int32.Parse(pkid));
            }
            return View("ChangeCategoryItem", model);
        }
        public ActionResult ChildChangeCategoryItem(string pkid, string parentPkid)
        {
            GaiZhuangCategoryModel model = new GaiZhuangCategoryModel();
            if (parentPkid != null)
            {
                model.ParentCategoryID = Convert.ToInt32(parentPkid);
                model.IsShow = true;
            }
            if (pkid != null)
            {
                model = ConfigCategoryManager.GetChangeCategoryModelByPKID(Int32.Parse(pkid));
                //if (model.ImageURL != null)
                //{
                //MemoryStream ms = new MemoryStream(model.ImageURL);
                //model.Images = Image.FromStream(ms);
                //}
            }
            return View("ChildChangeCategoryItem", model);
        }

        /// <summary>
        /// 添加或修改类目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertUpdateChangeCategoryItem(GaiZhuangCategoryModel model)
        {
            //HttpPostedFileBase file = Request.Files["files"];
            //if (model != null && file != null && file.ContentLength > 0)
            //{
            //    string fileName = Path.GetFileName(file.FileName);
            //    string mimeType = file.ContentType;
            //    using (Stream inputStream = file.InputStream)
            //    {
            //        MemoryStream memoryStream = inputStream as MemoryStream;
            //        if (memoryStream == null)
            //        {
            //            memoryStream = new MemoryStream();
            //            inputStream.CopyTo(memoryStream);
            //        }
            //        model.ImageURL = memoryStream.ToArray();
            //    }
            //}
            if (model.PKID == null)
            {
                if (model.ParentCategoryID == null)
                {
                    model.CategoryLevel = 0;
                    model.CategoryType = 0;
                }
                else
                {
                    model.CategoryLevel = 1;
                    model.CategoryType = 1;
                }
                bool result = ConfigCategoryManager.InsertChangeCategoryModel(model);
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "添加类目:" + model.CategoryName, ObjectType = "GaiZhuang", ObjectID = "0" });
                return Json(result);
            }
            else
            {
                bool result = ConfigCategoryManager.UpdateChangeCategoryModel(model);
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "修改类目:" + model.CategoryName, ObjectType = "GaiZhuang", ObjectID = model.PKID.ToString() });
                return Json(result);
            }
        }

        public ActionResult InsertCategoryItem(string categoryName, string sort, bool isShow)
        {
            GaiZhuangCategoryModel model = new GaiZhuangCategoryModel();
            model.CategoryName = categoryName;
            model.Sort = Convert.ToInt32(sort);
            model.IsShow = isShow;
            model.CategoryLevel = 0;
            model.CategoryType = 0;
            bool result = ConfigCategoryManager.InsertChangeCategoryModel(model);
            OprLogManager oprLog = new OprLogManager();
            DataAccess.Entity.OprLog modelLog = new DataAccess.Entity.OprLog();
            modelLog.Author = User.Identity.Name;
            modelLog.ObjectID = 0;
            modelLog.ObjectType = "GaiZhuang";
            modelLog.Operation = "添加类目:" + model.CategoryName;
            try
            {
                oprLog.AddOprLog(modelLog);
            }
            catch
            {
            }
            //LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "添加类目:" + model.CategoryName, ObjectType = "GaiZhuang", ObjectID = "0" });
            return Json(result);
        }
        public ActionResult UpdateCategoryItem(int pkid, string categoryName, string subTitle, string sort, bool isShow, int? showNum, string remark)
        {
            GaiZhuangCategoryModel model = new GaiZhuangCategoryModel();
            model.PKID = pkid;
            model.CategoryName = categoryName;
            model.Sort = Convert.ToInt32(sort);
            model.SubTitle = subTitle;
            model.ShowNum = showNum;
            model.IsShow = isShow;
            model.IsActive = true;
            bool result = ConfigCategoryManager.UpdateChangeCategoryModel(model);
            OprLogManager oprLog = new OprLogManager();
            DataAccess.Entity.OprLog modelLog = new DataAccess.Entity.OprLog();
            modelLog.Author = User.Identity.Name;
            modelLog.ObjectID = pkid;
            modelLog.ObjectType = "GaiZhuang";
            modelLog.Operation = "修改类目:" + model.CategoryName;
            try
            {
                oprLog.AddOprLog(modelLog);
            }
            catch
            {
            }
            //LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "修改类目:" + model.CategoryName, ObjectType = "GaiZhuang", ObjectID = model.PKID.ToString() });
            return Json(result);
        }

        public ActionResult InsertChildCategoryItem(string categoryName, int? parentCategoryID, string subTitle, string sort, int? showNum, bool isShow)
        {
            GaiZhuangCategoryModel model = new GaiZhuangCategoryModel();
            model.CategoryName = categoryName;
            model.ParentCategoryID = parentCategoryID;
            model.SubTitle = subTitle;
            model.Sort = Convert.ToInt32(sort);
            model.IsShow = isShow;
            model.ShowNum = showNum;
            model.CategoryLevel = 1;
            model.CategoryType = 1;
            bool result = ConfigCategoryManager.InsertChangeCategoryModel(model);
            OprLogManager oprLog = new OprLogManager();
            DataAccess.Entity.OprLog modelLog = new DataAccess.Entity.OprLog();
            modelLog.Author = User.Identity.Name;
            modelLog.ObjectID = 0;
            modelLog.ObjectType = "GaiZhuang";
            modelLog.Operation = "添加类目:" + model.CategoryName;
            try
            {
                oprLog.AddOprLog(modelLog);
            }
            catch
            {
            }
            //LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "添加类目:" + model.CategoryName, ObjectType = "GaiZhuang", ObjectID = "0" });
            return Json(result);
        }
        public ActionResult UpdateChildCategoryItem(int pkid, string categoryName, int? parentCategoryID, string subTitle, string sort, int? showNum, bool isShow)
        {
            GaiZhuangCategoryModel model = new GaiZhuangCategoryModel();
            model.PKID = pkid;
            model.CategoryName = categoryName;
            model.Sort = Convert.ToInt32(sort);
            model.SubTitle = subTitle;
            model.ShowNum = showNum;
            model.IsShow = isShow;
            model.IsActive = true;
            bool result = ConfigCategoryManager.UpdateChangeCategoryModel(model);
            OprLogManager oprLog = new OprLogManager();
            DataAccess.Entity.OprLog modelLog = new DataAccess.Entity.OprLog();
            modelLog.Author = User.Identity.Name;
            modelLog.ObjectID = pkid;
            modelLog.ObjectType = "GaiZhuang";
            modelLog.Operation = "修改类目:" + model.CategoryName;
            try
            {
                oprLog.AddOprLog(modelLog);
            }
            catch
            {
            }
            //LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "修改类目:" + model.CategoryName, ObjectType = "GaiZhuang", ObjectID = model.PKID.ToString() });
            return Json(result);
        }

        /// <summary>
        /// 删除类目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteChangeCategoryItem(int pkid)
        {
            bool result = ConfigCategoryManager.DeleteChangeCategoryModelByPkid(pkid);
            return Json(result);
        }


        /// <summary>
        /// 删除已关联商品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteRelateProductByPkid(int pkid)
        {
            bool result = ConfigCategoryManager.DeleteRelateProductByPkid(pkid);
            OprLogManager oprLog = new OprLogManager();
            DataAccess.Entity.OprLog modelLog = new DataAccess.Entity.OprLog();
            modelLog.Author = User.Identity.Name;
            modelLog.ObjectID = pkid;
            modelLog.ObjectType = "SelectedRelateProduct";
            modelLog.Operation = "删除已关联商品:" + pkid;
            try
            {
                oprLog.AddOprLog(modelLog);
            }
            catch
            {
            }
            //LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "删除已关联商品:" + pkid, ObjectType = "SelectedRelateProduct", ObjectID = "0" });
            return Json(result);
        }
        [HttpPost]
        public ActionResult DeleteRelateProductByPkids(string pkids)
        {
            //LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "批量删除已关联商品:" + pkids, ObjectType = "SelectedRelateProduct", ObjectID = "0" });
            bool result = ConfigCategoryManager.DeleteRelateProductByPkids(pkids);
            OprLogManager oprLog = new OprLogManager();
            DataAccess.Entity.OprLog modelLog = new DataAccess.Entity.OprLog();
            modelLog.Author = User.Identity.Name;
            modelLog.ObjectID = 0;
            modelLog.ObjectType = "SelectedRelateProduct";
            modelLog.Operation = "批量删除已关联商品:" + pkids;
            try
            {
                oprLog.AddOprLog(modelLog);
            }
            catch
            {
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetPromotionModelByRuleID(string promotionId)
        {
            GetPCodeModel model = ConfigCategoryManager.GetPromotionModelByRuleID(promotionId);
            var data = Json(model);
            return Json(model);
        }

        public ActionResult AdvertCategoryList(string categoryid)
        {
            IEnumerable<AdvertCategoryModel> list = ConfigCategoryManager.SelectAdvertCategoryList(categoryid);

            ViewData["categoryid"] = categoryid;
            return View("AdvertCategoryList", list);
        }

        public ActionResult AdvertCategory(string categoryid, string pkid)
        {
            List<SelectListItem> lsSelItem = new List<SelectListItem>();
            SelectListItem sel = new SelectListItem();
            sel = new SelectListItem();
            sel.Value = "0";
            sel.Text = "请选择";
            lsSelItem.Add(sel);
            sel = new SelectListItem();
            sel.Value = "1";
            sel.Text = "跳转H5活动页";
            lsSelItem.Add(sel);
            sel = new SelectListItem();
            sel.Value = "2";
            sel.Text = "领取优惠券";
            lsSelItem.Add(sel);

            ViewData["categoryid"] = categoryid;
            if (string.IsNullOrWhiteSpace(categoryid))
            {
                ViewData["categoryid"] = "0";
            }
            List<SelectListItem> listItem = new List<SelectListItem>();
            AdvertCategoryModel model = new AdvertCategoryModel();
            if (pkid != null)
            {
                model = ConfigCategoryManager.GetAdvertCategoryModelByPKID(Int32.Parse(pkid));
                if (model.AdvertType != null)
                {

                    foreach (var ss in lsSelItem)
                    {
                        if (ss.Value == model.AdvertType.ToString())
                        {
                            ss.Selected = true;
                        }
                        listItem.Add(ss);
                    }
                }
                else
                {
                    listItem = lsSelItem;
                }
            }
            else
            {
                listItem = lsSelItem;
                model.IsShow = true;
                model.CategoryId = string.IsNullOrWhiteSpace(categoryid) ? 0 : Convert.ToInt32(categoryid);
            }
            SelectList ddlSelData = new SelectList(listItem.AsEnumerable(), "Value", "Text", "请选择");
            ViewData["ddlAdTypeData"] = ddlSelData;
            return View("AdvertCategory", model);
        }

        /// <summary>
        /// 图片上传至阿里云
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddAdvertImg()
        {
            string _Image = string.Empty;
            Exception ex = null;
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    var uploadResult = ImageUploadHelper.UpdateLoadImage(buffer);
                    if (uploadResult.Item1)
                    {
                        _Image  = ImageHelper.GetImageUrl(uploadResult.Item2);
                    }

                }
                catch (Exception error)
                {
                    ex = error;
                }
            }
            return Json(new
            {
                BImage = swc.WebConfigurationManager.AppSettings["DoMain_image"] + _Image,
                SImage = swc.WebConfigurationManager.AppSettings["DoMain_image"] + _Image,
                Msg = ex == null ? "上传成功" : ex.Message
            }, "text/html");
        }

        [HttpPost]
        public ActionResult InsertUpdateAdvertCategoryItem(AdvertCategoryModel model, HttpPostedFileBase file1)
        {
            HttpPostedFileBase file = Request.Files["files"];
            if (model != null && file != null && file.ContentLength > 0)
            {
                //string fileName = Path.GetFileName(file.FileName);
                //string mimeType = file.ContentType;
                using (Stream inputStream = file.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    // model.ImageURL = memoryStream.ToArray();
                }
            }
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem sel = new SelectListItem();
            sel = new SelectListItem();
            sel.Value = "0";
            sel.Text = "请选择";
            list.Add(sel);
            sel = new SelectListItem();
            sel.Value = "1";
            sel.Text = "跳转H5活动页";
            list.Add(sel);
            sel = new SelectListItem();
            sel.Value = "2";
            sel.Text = "领取优惠券";
            list.Add(sel);
            foreach (var ddl in list)
            {
                if (model.AdvertType == Convert.ToInt32(ddl.Value))
                {
                    model.AdvertTypeName = ddl.Text;
                }
            }
            if (model.PKID == null)
            {
                bool result = ConfigCategoryManager.InsertAdvertCategoryModel(model);
                return Json(result);
            }
            else
            {
                bool result = ConfigCategoryManager.UpdateAdvertCategoryModel(model);
                return Json(result);
            }
        }


        public ActionResult InsertAdvertCategoryItem(string categoryId, string adType, string typeName, string adName, string imageURL, string sort, string appLink, string mobileLink, string promotionCodeID, bool isShow, string extend1, string extend2)
        {
            AdvertCategoryModel model = new AdvertCategoryModel();
            model.CategoryId = Convert.ToInt32(categoryId);
            model.AdvertType = Convert.ToInt32(adType);
            model.AdvertTypeName = typeName;
            model.AdvertName = adName;
            model.ImageURL = imageURL;
            model.Sort = Convert.ToInt32(sort);
            model.AppLink = appLink;
            model.H5Link = mobileLink;
            model.PromotionRuleID = promotionCodeID;
            model.IsShow = isShow;
            model.PromotionName = extend1;
            model.PromotionDescription = extend2;

            bool result = ConfigCategoryManager.InsertAdvertCategoryModel(model);
            LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "添加广告:" + model.AdvertName, ObjectType = "GuangGao", ObjectID = "0" });
            return Json(result);
        }
        public ActionResult UpdateAdvertCategoryItem(int pkid, string categoryId, string adType, string typeName, string adName, string imageURL, string sort, string appLink, string mobileLink, string promotionCodeID, bool isShow, string extend1, string extend2)
        {
            AdvertCategoryModel model = new AdvertCategoryModel();
            model.PKID = pkid;
            model.CategoryId = Convert.ToInt32(categoryId);
            model.AdvertType = Convert.ToInt32(adType);
            model.AdvertTypeName = typeName;
            model.AdvertName = adName;
            model.ImageURL = imageURL;
            model.Sort = Convert.ToInt32(sort);
            model.AppLink = appLink;
            model.H5Link = mobileLink;
            model.PromotionRuleID = promotionCodeID;
            model.IsShow = isShow;
            model.PromotionName = extend1;
            model.PromotionDescription = extend2;
            model.IsActive = true;
            bool result = ConfigCategoryManager.UpdateAdvertCategoryModel(model);
            LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "修改广告:" + model.AdvertName, ObjectType = "GuangGao", ObjectID = model.PKID.ToString() });
            return Json(result);
        }

        public ActionResult DeleteAdvert(int pkid)
        {
            bool result = ConfigCategoryManager.DeleteAdvertCategoryModelByPkid(pkid);
            LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "删除广告:" + pkid, ObjectType = "GuangGao", ObjectID = pkid.ToString() });
            return Json(result);
        }
        public ActionResult ArticleCategory(string pkid, string categoryid)
        {
            GaiZhuangRelateArticleModel model = new GaiZhuangRelateArticleModel();
            if (!string.IsNullOrWhiteSpace(categoryid))
            {
                model.CategoryId = Convert.ToInt32(categoryid);
            }
            if (pkid != null)
            {
                model = ConfigCategoryManager.GetRelateArticleByPKID(Int32.Parse(pkid));
            }
            ViewData["articlecategoryid"] = categoryid;
            return View("ArticleCategory", model);
        }

        public ActionResult ArticleCategoryList(string categoryid)
        {
            IEnumerable<GaiZhuangRelateArticleModel> list = ConfigCategoryManager.SelectArticleCategoryList(categoryid);
            ViewData["articlecategoryid"] = categoryid;
            return View("ArticleCategoryList", list);
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteArticleCategory(int pkid)
        {
            bool result = ConfigCategoryManager.DeleteArticleCategoryModelByPkid(pkid);
            return Json(result);
        }

        [HttpPost]
        public ActionResult InsertUpdateArticleCategoryItem(GaiZhuangRelateArticleModel model)
        {
            if (model.PKID == null)//新增
            {
                bool result = ConfigCategoryManager.InsertArticleCategoryModel(model);
                return Json(result);
            }
            else//更新
            {
                bool result = ConfigCategoryManager.UpdateArticleCategoryModel(model);
                return Json(result);
            }
        }
        //添加文章
        public ActionResult InsertArticleItem(string param, string selectItems, int categoryId, string articleName, string articleLink)
        {
            GaiZhuangRelateArticleModel model = new GaiZhuangRelateArticleModel();
            model.CategoryId = categoryId;
            model.VehicleID = null;
            model.ArticleName = articleName;
            model.ArticleLink = articleLink;
            model.IsActive = true;
            int pkid = ConfigCategoryManager.InsertArticleCategory(model);
            if (pkid < 1)
            {
                return Json(new { msg = "fail" });
            }
            var conditionList = JsonConvert.DeserializeObject<List<Condition>>(param);
            var queryParams = JsonConvert.DeserializeObject<QueryCondition>(selectItems);//前端选择的筛选条件

            var brandList = conditionList.Select(item => item.Brand).ToList();
            var brands = brandList.Distinct().ToList();
            var sb = new StringBuilder();
            foreach (var item in brands)
            {
                sb.AppendFormat("{0},", item);
            }
            //先查出所有选择的品牌的车型信息，然后过滤出待插入数据
            var vehicleInfoList = ipvimgr.GetVehicleTypeInfoByBrandName(sb.ToString().TrimEnd(','));
            if (vehicleInfoList.Count == 0 && !string.IsNullOrEmpty(queryParams.Condition))
            {
                //防止没有数据的情况
                vehicleInfoList = ipvimgr.GetVehicleTypeInfoByCharacter(queryParams.Condition);
            }
            var vehicleInfoListAll = vehicleInfoList;

            if (string.IsNullOrEmpty(queryParams.VehicleSeries) && string.IsNullOrEmpty(queryParams.Price) && string.IsNullOrEmpty(queryParams.VehicleType))
            {
                vehicleInfoListAll.AddRange(vehicleInfoList);
            }

            var distinctList = vehicleInfoListAll.Distinct(new List_User_DistinctBy_TID()).ToList();

            List<ArticleAdaptVehicleModel> velist = new List<ArticleAdaptVehicleModel>();
            //获取所有待更新数据
            var dt = GetDtForUpdate(distinctList, conditionList, pkid, velist);
            bool result = ConfigCategoryManager.BulkSaveAriticleVehicle(dt);
            if (!result)
            {
                return Json(new { msg = "车型添加失败" });
            }
            //"二级车型"://需要vehicleid加pid
            //"四级车型"://需要vehicleid加pid加pailiang加nian
            //"五级车型"://需要tid加pid

            return Json(new { msg = "success" });
        }

        public DataTable GetDtForUpdate(List<VehicleTypeInfoDb> sourceList, List<Condition> items, int articleId, List<ArticleAdaptVehicleModel> existConfigList)
        {
            var dt = new DataTable();
            dt.Columns.Add("PKID", typeof(int));
            dt.Columns.Add("ArticleId", typeof(int));
            dt.Columns.Add("Brand", typeof(string));
            dt.Columns.Add("VehicleID", typeof(string));
            dt.Columns.Add("Nian", typeof(string));
            dt.Columns.Add("PaiLiang", typeof(string));
            dt.Columns.Add("CreatedTime", typeof(DateTime));
            dt.Columns.Add("UpdatedTime", typeof(DateTime));

            var destList = new List<VehicleTypeInfoDb>();
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.Brand) && !string.IsNullOrEmpty(item.Vehicle) && !string.IsNullOrEmpty(item.PaiLiang) && !string.IsNullOrEmpty(item.Nian))
                {
                    destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand && i.Vehicle == item.Vehicle && i.PaiLiang == item.PaiLiang && i.Nian == item.Nian));
                }
                else if (!string.IsNullOrEmpty(item.Brand) && !string.IsNullOrEmpty(item.Vehicle) && !string.IsNullOrEmpty(item.PaiLiang))
                {
                    destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand && i.Vehicle == item.Vehicle && i.PaiLiang == item.PaiLiang));
                }
                else if (!string.IsNullOrEmpty(item.Brand) && !string.IsNullOrEmpty(item.Vehicle))
                {
                    destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand && i.Vehicle == item.Vehicle));
                }
                else
                {
                    destList.AddRange(sourceList.FindAll(i => i.Brand == item.Brand));
                }
            }

            var distinctListN = destList.Distinct(new ListDistinctForSelectLevel4()).ToList();//待更新数据按TID去重防止TID相同生产年份不同的重复数据
                                                                                              //var deleteList = new List<ArticleAdaptVehicleModel>();//待删除配置列表
                                                                                              //List<VehicleTypeInfoDb> existList;
                                                                                              //所有已经在配置表中但不在待插入数据表中的数据都进待删除列表


            //foreach (var entity in existConfigList)
            //{
            //    existList =
            //        distinctListN.FindAll(
            //            i =>
            //                i.VehicleID == entity.VehicleID && i.PaiLiang == entity.PaiLiang &&
            //                i.Nian == entity.Nian);
            //    if (existList.Count > 0)
            //    {
            //        deleteList.Add(entity);
            //    }
            //}

            //执行删除配置操作
            if (existConfigList.Count > 0)
                ConfigCategoryManager.DeleteArticleVehicleCategoryModel(existConfigList);

            foreach (var itemp in distinctListN)
            {
                var dr = dt.NewRow();
                dr["PKID"] = 0;
                dr["ArticleId"] = articleId;
                dr["Brand"] = itemp.Brand;
                dr["VehicleID"] = itemp.VehicleID;
                dr["Nian"] = itemp.Nian;
                dr["PaiLiang"] = itemp.PaiLiang;
                dr["CreatedTime"] = DateTime.Now;
                dr["UpdatedTime"] = DateTime.Now;
                dt.Rows.Add(dr);
            }

            return dt;
        }
        public ActionResult UpdateArticleItem(string param, string selectItems, int pkid, int categoryId, string articleName, string articleLink)
        {
            GaiZhuangRelateArticleModel model = new GaiZhuangRelateArticleModel();
            model.PKID = pkid;
            model.CategoryId = categoryId;
            model.VehicleID = null;
            model.ArticleName = articleName;
            model.ArticleLink = articleLink;
            bool re = ConfigCategoryManager.UpdateArticleVehicleCategoryModel(model);
            if (!re)
            {
                return Json(new { msg = "update fail" });
            }
            var conditionList = JsonConvert.DeserializeObject<List<Condition>>(param);
            var queryParams = JsonConvert.DeserializeObject<QueryCondition>(selectItems);//前端选择的筛选条件

            var brandList = conditionList.Select(item => item.Brand).ToList();
            var brands = brandList.Distinct().ToList();
            var sb = new StringBuilder();
            foreach (var item in brands)
            {
                sb.AppendFormat("{0},", item);
            }
            //先查出所有选择的品牌的车型信息，然后过滤出待插入数据
            var vehicleInfoList = ipvimgr.GetVehicleTypeInfoByBrandName(sb.ToString().TrimEnd(','));
            if (vehicleInfoList.Count == 0 && !string.IsNullOrEmpty(queryParams.Condition))
            {
                //防止没有数据的情况
                vehicleInfoList = ipvimgr.GetVehicleTypeInfoByCharacter(queryParams.Condition);
            }
            var vehicleInfoListAll = vehicleInfoList;

            if (string.IsNullOrEmpty(queryParams.VehicleSeries) && string.IsNullOrEmpty(queryParams.Price) && string.IsNullOrEmpty(queryParams.VehicleType))
            {
                vehicleInfoListAll.AddRange(vehicleInfoList);
            }

            var distinctList = vehicleInfoListAll.Distinct(new List_User_DistinctBy_TID()).ToList();

            List<ArticleAdaptVehicleModel> velist = new List<ArticleAdaptVehicleModel>();
            velist = ConfigCategoryManager.SelectArticleVehicleCategoryList(pkid).ToList();
            var alphabet = "";
            if (distinctList.Count > 0)
            {
                alphabet = distinctList.First().Brand.Replace('—', '-').Split('-')[0].Trim();
            }
            var listByAlpha = ipvimgr.GetVehicleTypeInfoByCharacter(alphabet);

            for (var t = velist.Count - 1; t >= 0; t--)
            {
                if (!listByAlpha.Exists(i => i.VehicleID == velist[t].VehicleID))
                {
                    velist.RemoveAt(t);
                }
            }
            //获取所有待更新数据
            var dt = GetDtForUpdate(distinctList, conditionList, pkid, velist);
            bool result = ConfigCategoryManager.BulkSaveAriticleVehicle(dt);
            if (!result)
            {
                return Json(new { msg = "车型添加失败" });
            }
            //"二级车型"://需要vehicleid加pid
            //"四级车型"://需要vehicleid加pid加pailiang加nian
            //"五级车型"://需要tid加pid

            return Json(new { msg = "success" });
        }

        private readonly IProductVehicleInfoMgr ipvimgr = new ProductVehicleInfoMgr();
        /// <summary>
        /// 根据查询条件获取所有四级车型信息，编辑页使用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllFourVehicleTypeInfoByParams(QueryCondition param)//根据首字母筛选出对应的数据给到前端
        {
            if (string.IsNullOrEmpty(param.Condition))
            {
                param.Condition = "A";//如果用户未选择首字母则默认拉出A品牌的车型数据
            }
            var vehicleInfoList = ipvimgr.GetVehicleTypeInfoByCharacter(param.Condition);

            var sourceListAll = vehicleInfoList;//初始化

            //只有TID跟生产年份都一致的才去重
            var distinctList = sourceListAll.Distinct(new List_User_DistinctBy_TID()).ToList();
            //已配置的车型
            var checkedItemList = string.IsNullOrWhiteSpace(param.ArticleID) ? new List<ArticleAdaptVehicleModel>() : ConfigCategoryManager.SelectArticleVehicleCategoryList(Convert.ToInt32(param.ArticleID)).ToList();
            var vehicleTypeInfoList = new List<VehicleTypeInfoVm>();

            foreach (var item in distinctList)
            {
                if (string.IsNullOrEmpty(item.Vehicle) || string.IsNullOrEmpty(item.PaiLiang) ||
                    string.IsNullOrEmpty(item.Nian))
                {
                    continue;
                }
                var temp = new VehicleTypeInfoVm();
                if (checkedItemList.Exists(i => i.VehicleID == item.VehicleID && i.Nian == item.Nian && i.PaiLiang == item.PaiLiang))
                {
                    temp.IsChecked = "checked";
                }
                temp.VehicleID = item.VehicleID;
                temp.AvgPrice = item.AvgPrice;
                temp.Brand = string.IsNullOrEmpty(item.Brand) ? "" : item.Brand;
                temp.BrandCategory = item.BrandCategory;
                temp.JointVenture = item.JointVenture;
                temp.ListedYear = item.ListedYear;
                temp.Nian = string.IsNullOrEmpty(item.Nian) ? "" : item.Nian;
                temp.PaiLiang = string.IsNullOrEmpty(item.PaiLiang) ? "" : item.PaiLiang;
                temp.SalesName = item.SalesName;
                temp.StopProductionYear = item.StopProductionYear;
                temp.TID = item.TID;
                temp.Vehicle = string.IsNullOrEmpty(item.Vehicle) ? "" : item.Vehicle;
                temp.VehicleSeries = item.VehicleSeries;
                temp.VehicleType = item.VehicleType;

                vehicleTypeInfoList.Add(temp);
            }

            var dataList = new List<TreeItem>();
            var treeItems = new List<TreeItem>();
            //车型数据分组
            //TODO：查询首字母为X的四级车型信息

            var dataLevel4 = vehicleTypeInfoList.GroupBy(item => item.Brand)
                .ToDictionary(i => i.Key,
                    i => i.ToList().GroupBy(t => t.Vehicle).ToDictionary(j => j.Key, j => j.ToList().GroupBy(t => t.PaiLiang).ToDictionary(a => a.Key, a => a.ToList().GroupBy(c => c.Nian).ToDictionary(d => d.Key, d => d.ToList()))));

            foreach (var item in dataLevel4)
            {
                var currentKey = item.Key;//品牌
                var treeItemBrand = new TreeItem
                {
                    id = item.Key,
                    check = "true",
                    name = currentKey,
                    children = new List<TreeItem>()
                };

                foreach (var child in item.Value)
                {
                    var currentChildKey = child.Key;//车系
                    var treeItemVehicleId = new TreeItem()
                    {
                        id = item.Key + "$" + child.Key,
                        name = currentChildKey,
                        check = "true",
                        children = new List<TreeItem>()
                    };

                    foreach (var child2 in child.Value)
                    {
                        var currentChild2Key = child2.Key;//排量
                        var treeItemPailiang = new TreeItem()
                        {
                            id = item.Key + "$" + child.Key + "$" + child2.Key,
                            name = currentChild2Key,
                            check = "true",
                            children = new List<TreeItem>()
                        };

                        foreach (var child3 in child2.Value)
                        {
                            var currentChild3Key = child3.Key;//年款
                            var treeItemNian = new TreeItem()
                            {
                                id = item.Key + "$" + child.Key + "$" + child2.Key + "$" + child3.Key,
                                name = currentChild3Key,
                                check = "true"
                            };

                            if (child3.Value.Exists(i => i.IsChecked == null))
                            {
                                treeItemNian.check = "false";
                            }

                            treeItemPailiang.children.Add(treeItemNian);
                        }

                        if (treeItemPailiang.children.Exists(i => i.check == "false"))
                        {
                            treeItemPailiang.check = "false";
                        }
                        treeItemVehicleId.children.Add(treeItemPailiang);
                    }
                    if (treeItemVehicleId.children.Exists(i => i.check == "false"))
                    {
                        treeItemVehicleId.check = "false";
                    }

                    treeItemBrand.children.Add(treeItemVehicleId);
                }

                if (treeItemBrand.children.Exists(i => i.check == "false"))
                {
                    treeItemBrand.check = "false";
                }

                treeItems.Add(treeItemBrand);
            }


            return new ContentResult
            {
                Content = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(new { items = treeItems, count = treeItems.Count }),
                ContentType = "application/json"
            };


        }

        public ActionResult RelateProductList(int productType = 0, string productPID = "", string productNameKey = "", int pageIndex = 1, int pageSize = 10, int categoryId = 0, string productOids = null)
        {
            ViewData["productcategoryid"] = categoryId;
            #region dropdownlist
            List<SelectListItem> lsSelItem = new List<SelectListItem>();
            SelectListItem sel = new SelectListItem();
            sel = new SelectListItem();
            sel.Value = "0";
            sel.Text = "请选择";
            lsSelItem.Add(sel);
            sel = new SelectListItem();
            sel.Value = "1";
            sel.Text = "根据商品PID";
            lsSelItem.Add(sel);
            sel = new SelectListItem();
            sel.Value = "2";
            sel.Text = "根据商品名称关键词";
            lsSelItem.Add(sel);
            sel = new SelectListItem();
            sel.Value = "3";
            sel.Text = "根据商品类目";
            lsSelItem.Add(sel);

            SelectList ddlSelData = new SelectList(lsSelItem.AsEnumerable(), "Value", "Text", "请选择");
            ViewData["ddlProductTypeData"] = ddlSelData;
            #endregion

            int count = 0;
            string strSql = string.Empty;
            var lists = new List<RelateProductModel>();
            #region
            if (productType == 0)//进入页面
            {
                //lists = ConfigCategoryManager.GetRelateProductList(strSql, pageSize, pageIndex, out count);
            }
            if (productType == 1) //按商品PID查找
            {
                lists = ConfigCategoryManager.GetRelateProductListByProductPID(productPID, pageSize, pageIndex, out count);
            }
            else if (productType == 2)//按商品名称关键词查找
            {
                lists = ConfigCategoryManager.GetRelateProductListByProductKeyName(productNameKey, pageSize, pageIndex, out count);
            }
            else if (productType == 3)//按选中的商品类目查找
            {
                lists = ConfigCategoryManager.GetRelateProductListByProductItems(productOids, pageSize, pageIndex, out count);
            }
            List<string> pids = new List<string>();
            foreach (var pro in lists)
            {
                pids.Add(pro.PID);
            }

            IEnumerable<SkuProductDetailModel> pros = null;
            //try
            //{
            if (pids.Any())
            {
                using (var client = new Tuhu.Service.Product.ProductClient())
                {
                    var tire = client.SelectSkuProductListByPids(pids);
                    tire.ThrowIfException(true);
                    pros = tire.Result;
                }
            }
            for (int i = 0; i < lists.Count(); i++)
            {
                lists[i].ProductType = pros.Where(x => x.Pid == lists[i].PID).FirstOrDefault().RootCategoryName;
            }
            #endregion
            var list = new OutData<List<RelateProductModel>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            ViewBag.Total = count;
            ViewBag.pageSize = pageSize;
            ViewBag.pageNumber = pageIndex;
            //return this.View(lists);
            return this.View(new ListModel<RelateProductModel>(list.ReturnValue, pager).ToList());
        }

        public PartialViewResult List(int productType, string productPID, string productNameKey, int pageSize, int categoryId = 0, string productOids = null)
        {
            ViewData["productcategoryid"] = categoryId;

            int count = 0;

            var lists = new List<RelateProductModel>();

            if (productType == 1) //按商品PID查找
            {
                lists = ConfigCategoryManager.GetRelateProductListByProductPIDNoPage(productPID);
            }
            else if (productType == 2)//按商品名称关键词查找
            {
                lists = ConfigCategoryManager.GetRelateProductListByProductKeyNameNoPage(productNameKey);
            }
            else if (productType == 3)//按选中的商品类目查找
            {
                if (!string.IsNullOrWhiteSpace(productOids))
                {
                    productOids = productOids.Substring(0, productOids.Length - 1);
                    lists = ConfigCategoryManager.GetRelateProductListByProductItemsNoPage(productOids);
                }
            }
            for (int i = 0; i < lists.Count(); i++)
            {
                if (lists[i] != null)
                {
                    lists[i].OnSale = lists[i].OnSale == "True" ? "上架" : "下架";
                }
            }

            ViewBag.Total = count;
            ViewBag.pageSize = pageSize;
            return PartialView(lists);
        }

        public ActionResult SelectedRelateProductList(int pageIndex = 1, int pageSize = 100, int categoryId = 0)
        {
            ViewData["selectedproductcategoryid"] = categoryId;


            int count = 0;
            string strSql = string.Empty;
            var lists = new List<SelectedRelateProductModel>();
            var list = new OutData<List<SelectedRelateProductModel>, int>(lists, count);
            var relateProducts = ConfigCategoryManager.GetSelectedRelateProductListByCategoryId(categoryId,
                new List<string>().ToArray()).ToList();
            var brands= relateProducts.Select(r=>r.Brand).Distinct().ToList();
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            ViewBag.Total = count;
            ViewBag.pageSize = pageSize;
            ViewBag.pageNumber = pageIndex;
            ViewBag.BrandsCheckBoxList = brands;
            //ViewBag.RelateProducts = relateProducts;
            //return this.View(lists);
            return this.View(new ListModel<SelectedRelateProductModel>(list.ReturnValue, pager).ToList());
        }
        public PartialViewResult SelectedList(string[] brands,int pageSize = 100, int categoryId = 0)
        {
            ViewData["selectedproductcategoryid"] = categoryId;

            int count = 0;
            var lists = new List<SelectedRelateProductModel>();
            lists = ConfigCategoryManager.GetSelectedRelateProductListByCategoryId(categoryId,brands);

            for (int i = 0; i < lists.Count(); i++)
            {
                //lists[i].ProductType = pros.Where(x => x.Pid == lists[i].PID).FirstOrDefault()==null?"": pros.Where(x => x.Pid == lists[i].PID).FirstOrDefault().RootCategoryName;
                lists[i].OnSale = lists[i].OnSale == "True" ? "上架" : "下架";
            }
            var list = new OutData<List<SelectedRelateProductModel>, int>(lists, count);
            ViewBag.Total = count;
            ViewBag.pageSize = pageSize;
            ViewBag.Display = "全部";
            //return this.View(lists);
            return PartialView(list.ReturnValue);
            //this.View(new Tuhu.Component.Common.Models.ListModel<SelectedRelateProductModel>(list.ReturnValue, pager).ToList());
        }

        public ActionResult GetSelectedRelateProductListByCondition(int productType, string productPID, string productNameKey, int pageIndex = 1, int pageSize = 10, int categoryid = 0, string productOids = null)
        {
            ViewData["selectedproductcategoryid"] = categoryid;
            #region list
            List<SelectListItem> lsSelItem = new List<SelectListItem>();
            SelectListItem sel = new SelectListItem();
            sel = new SelectListItem();
            sel.Value = "0";
            sel.Text = "请选择";
            lsSelItem.Add(sel);
            sel = new SelectListItem();
            sel.Value = "1";
            sel.Text = "根据商品PID";
            lsSelItem.Add(sel);
            sel = new SelectListItem();
            sel.Value = "2";
            sel.Text = "根据商品名称关键词";
            lsSelItem.Add(sel);
            sel = new SelectListItem();
            sel.Value = "3";
            sel.Text = "根据商品类目";
            lsSelItem.Add(sel);

            SelectList ddlSelData = new SelectList(lsSelItem.AsEnumerable(), "Value", "Text", "请选择");
            ViewData["ddlProductTypeData"] = ddlSelData;

            #endregion
            int count = 0;
            string strSql = string.Empty;
            var lists = new List<SelectedRelateProductModel>();
            if (productType == 0) //进入页面
            {
                //lists = ConfigCategoryManager.GetSelectedRelateProductList(strSql, pageSize, pageIndex, out count);
            }
            if (productType == 1) //按商品PID查找 
            {
                lists = ConfigCategoryManager.GetSelectedRelateProductListByProductPID(productPID, pageSize, pageIndex, out count);
            }
            else if (productType == 2)//按商品名称关键词查找
            {
                lists = ConfigCategoryManager.GetSelectedRelateProductListByProductKeyName(productNameKey, pageSize, pageIndex, out count);
            }
            else if (productType == 3)//按商品类目查找
            {
                if (!string.IsNullOrWhiteSpace(productOids))
                {
                    productOids = productOids.Substring(0, productOids.Length - 1);
                    lists = ConfigCategoryManager.GetSelectedRelateProductListByProductItems(productOids, pageSize, pageIndex, out count);
                }
            }
            List<string> pids = new List<string>();
            foreach (var pro in lists)
            {
                pids.Add(pro.PID);
            }

            IEnumerable<SkuProductDetailModel> pros = null;
            //try
            //{
            using (var client = new Tuhu.Service.Product.ProductClient())
            {
                var tire = client.SelectSkuProductListByPids(pids);
                tire.ThrowIfException(true);
                pros = tire.Result;
            }
            for (int i = 0; i < lists.Count(); i++)
            {
                lists[i].ProductType = pros.Where(x => x.Pid == lists[i].PID).FirstOrDefault().RootCategoryName;
            }
            var list = new OutData<List<SelectedRelateProductModel>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            ViewBag.Total = count;
            ViewBag.pageSize = pageSize;
            ViewBag.pageNumber = pageIndex;
            return View("RelateProductList", new Tuhu.Component.Common.Models.ListModel<Tuhu.Provisioning.DataAccess.Entity.SelectedRelateProductModel>(list.ReturnValue, pager).ToList());
        }

        //批量添加子类目关联商品
        public ActionResult AddRelateProductList(int categoryId, string pids)
        {
            if (string.IsNullOrWhiteSpace(pids))
            {
                OprLogManager oprLog2 = new OprLogManager();
                DataAccess.Entity.OprLog modelLog2 = new DataAccess.Entity.OprLog();
                modelLog2.Author = User.Identity.Name;
                modelLog2.ObjectID = categoryId;
                modelLog2.ObjectType = "SelectedRelateProduct";
                modelLog2.Operation = "选择的产品为空:" + pids;
                try
                {
                    oprLog2.AddOprLog(modelLog2);
                }
                catch
                {
                }
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "选择的产品为空:" + pids, ObjectType = "SelectedRelateProduct", ObjectID = categoryId.ToString() });
                return Json(new { msg = "选择产品为空" });
            }
            List<GaiZhuangRelateProductModel> selectedList = ConfigCategoryManager.GetSelectedRelateProductListByCategoryID(categoryId);
            if (pids.EndsWith(";"))
            {
                pids = pids.Substring(0, pids.Length - 1);
            }
            string[] pidArr = pids.Split(';');
            List<GaiZhuangRelateProductModel> needInsertList = new List<GaiZhuangRelateProductModel>();

            foreach (var arr in pidArr)
            {
                if (selectedList.Where(T => T.PID == arr).FirstOrDefault() == null)
                {
                    GaiZhuangRelateProductModel model = new GaiZhuangRelateProductModel();
                    model.PID = arr;
                    model.CategoryId = categoryId;
                    model.CreatedTime = DateTime.Now;
                    model.UpdatedTime = DateTime.Now;
                    needInsertList.Add(model);
                }
            }

            if (needInsertList.Count < 1)
            {
                OprLogManager oprLog1 = new OprLogManager();
                DataAccess.Entity.OprLog modelLog1 = new DataAccess.Entity.OprLog();
                modelLog1.Author = User.Identity.Name;
                modelLog1.ObjectID = categoryId;
                modelLog1.ObjectType = "SelectedRelateProduct";
                modelLog1.Operation = "没有需要新加的关联商品:" + pids;
                try
                {
                    oprLog1.AddOprLog(modelLog1);
                }
                catch
                {
                }
                //LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "没有需要新加的关联商品:" + pids, ObjectType = "SelectedRelateProduct", ObjectID = categoryId.ToString() });
                return Json(new { msg = "没有需要新加的关联商品" });
            }
            var dt = new DataTable();
            dt.Columns.Add("PKID", typeof(int));
            dt.Columns.Add("CategoryId", typeof(int));
            dt.Columns.Add("PID", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            dt.Columns.Add("CreatedTime", typeof(DateTime));
            dt.Columns.Add("UpdatedTime", typeof(DateTime));

            foreach (var itemp in needInsertList)
            {
                var dr = dt.NewRow();
                dr["PKID"] = 0;
                dr["CategoryId"] = categoryId;
                dr["PID"] = itemp.PID;
                dr["IsActive"] = true;
                dr["CreatedTime"] = DateTime.Now;
                dr["UpdatedTime"] = DateTime.Now;
                dt.Rows.Add(dr);
            }

            bool result = ConfigCategoryManager.BulkSaveRelateProduct(dt);
            if (!result)
            {
                return Json(new { msg = "商品添加失败" });
            }
            //LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "批量添加关联商品:" + pids, ObjectType = "SelectedRelateProduct", ObjectID = categoryId.ToString() });
            //LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "批量添加关联商品成功数量:" + needInsertList.Count, ObjectType = "SelectedRelateProduct", ObjectID = categoryId.ToString() });
            OprLogManager oprLog = new OprLogManager();
            DataAccess.Entity.OprLog modelLog = new DataAccess.Entity.OprLog();
            modelLog.Author = User.Identity.Name;
            modelLog.ObjectID = categoryId;
            modelLog.ObjectType = "SelectedRelateProduct";
            modelLog.Operation = "批量添加关联商品:" + pids + ";批量添加关联商品成功数量" + needInsertList.Count;
            try
            {
                oprLog.AddOprLog(modelLog);
            }
            catch
            {
            }
            return Json(new { msg = "商品添加成功，数量" + needInsertList.Count });
        }

        /// <summary>
        /// 查询所有产品类目
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllProductCategorys()//查询所有产品类目
        {

            var categoryList = ConfigCategoryManager.GetProductCategoryList();

            var treeItems = new List<TreeItem>();
            //产品类目分组
            List<CategoryModel> firstList = new List<CategoryModel>();
            List<CategoryModel> secondList = new List<CategoryModel>();
            List<CategoryModel> thirdList = new List<CategoryModel>();
            foreach (var category in categoryList)
            {
                if (category.Oid.ToString() == category.NodeNo)
                {
                    category.FirstParentOid = category.Oid;
                    firstList.Add(category);
                }
            }
            foreach (var first in firstList)
            {
                foreach (var category in categoryList)
                {
                    if (category.ParentOid == first.Oid)
                    {
                        category.FirstParentOid = first.Oid;
                        secondList.Add(category);
                    }
                }
            }
            foreach (var second in secondList)
            {
                foreach (var category in categoryList)
                {
                    if (category.ParentOid == second.Oid)
                    {
                        category.FirstParentOid = second.ParentOid;
                        thirdList.Add(category);
                    }
                }
            }

            foreach (var item in firstList)
            {
                //第一级目录
                var treeItemFirst = new TreeItem
                {
                    id = item.Oid.ToString(),
                    name = item.DisplayName,
                    children = new List<TreeItem>()
                };

                foreach (var child in secondList.Where(x => x.ParentOid == item.Oid))
                {
                    //第二级目录
                    var treeItemSecond = new TreeItem()
                    {
                        id = child.Oid.ToString(),
                        name = child.DisplayName,
                        children = new List<TreeItem>()
                    };

                    foreach (var child2 in thirdList.Where(x => x.ParentOid == child.Oid))
                    {
                        //第三级目录
                        var treeItemThird = new TreeItem()
                        {
                            id = child2.Oid.ToString(),
                            name = child2.DisplayName,
                            children = new List<TreeItem>()
                        };

                        if (treeItemThird.children.Exists(i => i.check == "false"))
                        {
                            treeItemThird.check = "false";
                        }
                        treeItemSecond.children.Add(treeItemThird);
                    }
                    if (treeItemSecond.children.Exists(i => i.check == "false"))
                    {
                        treeItemSecond.check = "false";
                    }

                    treeItemFirst.children.Add(treeItemSecond);
                }

                if (treeItemFirst.children.Exists(i => i.check == "false"))
                {
                    treeItemFirst.check = "false";
                }

                treeItems.Add(treeItemFirst);
            }


            return new ContentResult
            {
                Content = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue }.Serialize(new { items = treeItems, count = treeItems.Count }),
                ContentType = "application/json"
            };


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
        public ActionResult GetCategory()
        {
            var source = PromotionManager.SelectProductCategory().Where(s => s.ParentCategory == null || !s.ParentCategory.Any()).ToList();

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
                id = r.oid,
                children = r.ChildrenCategory.Select(c => new { name = c.DisplayName, title = c.CategoryName, id = c.oid })
            }), JsonRequestBehavior.AllowGet);

        }

        public ActionResult ShowVehicles(int articleId)
        {
            if (articleId == 0)
            {
                return Json(new { msg = "文章为空" });
            }
            GaiZhuangRelateArticleModel article = ConfigCategoryManager.SelectArticleCategoryByArticleId(articleId).FirstOrDefault();

            return View(article);
        }

        public ActionResult Region(string ids)
        {
            ViewBag.ID = ids;
            return View();
        }

        public JsonResult UpdateRegiod(string pkids,string[] regionIds,int type)
        {
            var intregionIds = new List<int>();
            if (regionIds!=null)
             intregionIds = regionIds.Select(r => Convert.ToInt32(r)).ToList(); 
               var result = ConfigCategoryManager.UpdateRegion(pkids, intregionIds,type);
            if (result > 0)
                return Json(new {status = 1, msg = "成功"});
            else
            {
                return Json(new { status = 0, msg = "失败" });
            }
        }

        [HttpPost]
        public JsonResult UploadImage()
        {
            var _BImage = "";
            var _SImage = "";
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    using (var client = new Service.Utility.FileUploadClient())
                    {
                        var resBig = client.UploadImage(new ImageUploadRequest()
                        {
                            Contents = buffer,
                            DirectoryName = "GaiZhuangImg",
                            MaxHeight = 1920,
                            MaxWidth = 1920
                        });
                        if (resBig.Success && !string.IsNullOrWhiteSpace(resBig.Result))
                        {
                            _BImage = WebConfigurationManager.AppSettings["DoMain_image"] + resBig.Result;
                        }
                        var resSmall = client.UploadImage(new ImageUploadRequest()
                        {
                            Contents = buffer,
                            DirectoryName = "GaiZhuangImg",
                            MaxHeight = 100,
                            MaxWidth = 100
                        });
                        if (resSmall.Success && !string.IsNullOrWhiteSpace(resSmall.Result))
                        {
                            _SImage = WebConfigurationManager.AppSettings["DoMain_image"] + resSmall.Result;
                        }
                    }
                }
                catch (Exception exp)
                {
                    WebLog.LogException(exp);
                }
            }
            return Json(new { BImage = _BImage, SImage = _SImage }, "text/html");
        }


        /// <summary>
        /// 编辑关联商品页面
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public ActionResult EditRelateProduct(string pkid, string sort)
        {
            SelectedRelateProductModel model = new SelectedRelateProductModel();

            if (pkid != null)
            {
                model = ConfigCategoryManager.EditRelateProduct(Int32.Parse(pkid));

            }
            return View("EditRelateProduct", model);
        }

        /// <summary>
        /// 编辑关联商品排序字段
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="sort"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult UpdateRelateProduct(int pkid,  string sort)
        {
            bool result = ConfigCategoryManager.UpdateRelateProduct(pkid,sort);

            return Json(result);
        }
    }

}