using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.GroupBuying;
using Tuhu.Service.Utility;

namespace Tuhu.Provisioning.Controllers
{
    public class BeautyHomePageConfigController : Controller
    {
        private static readonly Lazy<BeautyHomePageConfigManager> lazy = new Lazy<BeautyHomePageConfigManager>();

        private BeautyHomePageConfigManager BeautyHomePageConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly Lazy<MeiRongAcitivityConfigManager> lazy1 = new Lazy<MeiRongAcitivityConfigManager>();

        private MeiRongAcitivityConfigManager MeiRongAcitivityConfigManager
        {
            get
            {
                return lazy1.Value;
            }
        }
        public ActionResult Index()
        {
            //ViewBag.ProvinceList = MeiRongAcitivityConfigManager.GetRegion(0);
            return View();
        }

        public ActionResult List(BeautyHomePageConfig model, int pageIndex = 1, int pageSize = 15)
        {
            int count = 0;
            string strSql = string.Empty;

            var lists = BeautyHomePageConfigManager.GetBeautyHomePageConfigList(model, pageSize, pageIndex, out count);

            var list = new OutData<List<BeautyHomePageConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<BeautyHomePageConfig>(list.ReturnValue, pager));
        }



        public ActionResult Edit(short type, int id = 0)
        {
            ViewBag.ProvinceList = MeiRongAcitivityConfigManager.GetRegion(0);

            if (id == 0)
            {
                BeautyHomePageConfig model = new BeautyHomePageConfig();
                model.StartTime = DateTime.Now;
                model.EndTime = DateTime.Now.AddDays(30);
                model.Status = true;
                model.Channel = "ios";
                model.Type = type;
                return View(model);
            }
            else
            {
                BeautyHomePageConfig model = BeautyHomePageConfigManager.GetBeautyHomePageConfigById(id);
                model.RegionList = MeiRongAcitivityConfigManager.GetRegionRelation(model.Id, 2);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(BeautyHomePageConfig model)
        {
            try
            {
                if (model.Id != 0)
                {
                    if (BeautyHomePageConfigManager.UpdateBeautyHomePageConfig(model))
                    {
                        CleanCahce(model.Channel, model.Type);
                    }

                }
                else
                {
                    int outId = 0;
                    if (BeautyHomePageConfigManager.InsertBeautyHomePageConfig(model, ref outId))
                    {
                        CleanCahce(model.Channel, model.Type);
                    }
                }

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(BeautyHomePageConfigManager.DeleteBeautyHomePageConfig(id));
        }

        public bool CleanCahce(string channel, int type)
        {
            using (var client = new GroupBuyingClient())
            {
                if (type == 1)
                {
                    var result = client.RefreshBeautyConfigCache(channel).Result;
                    return result;
                }
                else
                {
                    var result = client.RefreshActivityConfigCache(channel).Result;
                    return result;
                }
            }
        }

        public ActionResult RefreshShopMapConfigsCache(string channel)
        {
            using (var client = new GroupBuyingClient())
            {
                var serviceResult = client.RefreshShopMapConfigsCache(channel);
                return Json(new
                {
                    Status = serviceResult.Success,
                    Msg = serviceResult.ErrorMessage
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ShopMapConfig()
        {
            return View();
        }

        public ActionResult GetShopConfigs()
        {
            var result = BeautyHomePageConfigManager.GetShopMapConfigs();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditShopConfig(BeautyHomePageConfig model)
        {
            var result = false;
            model.StartTime = DateTime.Now;
            model.EndTime = new DateTime(2999, 12, 31);
            model.Type = (short)(model.Type + 1000);
            if (model.Id != 0)
            {
                result = BeautyHomePageConfigManager.UpdateBeautyHomePageConfig(model);
            }
            else
            {
                int outId = 0;
                result = BeautyHomePageConfigManager.InsertBeautyHomePageConfig(model, ref outId);
            }
            return Json(new { Status = result });
        }

        #region BannerConfig
        public ActionResult BannerConfig()
        {
            return View();
        }
        private int _currentPage;
        public int CurrentPage
        {
            get
            {
                if (_currentPage <= 0)
                    int.TryParse(Request["currentPage"], out _currentPage);
                return _currentPage;
            }
        }
        public int PageSize = 15;
        private string _keyWords;
        public string KeyWords
        {
            get
            {
                if (string.IsNullOrEmpty(_keyWords))
                    _keyWords = Request["keyWords"];
                return _keyWords;
            }
        }
        private int _status;
        public int Status
        {
            get
            {
                if (!int.TryParse(Request["status"], out _status))
                    _status = -1;
                return _status;
            }
        }
        public JsonResult SelectBeautyBannerConfigs()
        {
            int totalCount = 0;
            var bannerConfigs = BeautyHomePageConfigManager.SelectBeautyBannerConfig(Status, KeyWords, CurrentPage, PageSize, out totalCount);
            return Json(new { result = bannerConfigs, total = totalCount, }, JsonRequestBehavior.AllowGet);
        }
        private int _bannerId;
        public int BannerId
        {
            get
            {
                if (!int.TryParse(Request["bannerId"], out _bannerId))
                    _bannerId = 0;
                return _bannerId;
            }
        }
        public ActionResult EditBeautyBannerConfig()
        {
            return View(BannerId);
        }
        public JsonResult GetBeautyBannerConfig()
        {
            var model = BeautyHomePageConfigManager.GetBeautyHomePageConfigById(BannerId) ?? new BeautyHomePageConfig();
            model.RegionList = MeiRongAcitivityConfigManager.GetRegion(model.Id, 3);
            var regionresult = model.RegionList?.GroupBy(g => g.ProvinceName).Select(f =>
              new
              {
                  ProvinceName = f.Key,
                  checkallgroup = f.ToArray().Select(s => s.CityName),
              });
            var activitylist = MeiRongAcitivityConfigManager.GetMeiRongAcitivityConfigList();
            return Json(new
            {
                result = model,
                activity = activitylist,
                regionList = regionresult
            });
        }
        [HttpPost]
        public JsonResult SaveBeautyBannerConfig(BeautyHomePageConfig model)
        {
            var result = false;
            string msg = string.Empty;
            try
            {
                var log = new BeautyOprLog
                {
                    LogType = "SaveBeautyBannerConfig",
                    IdentityID = $"{model.Id}",
                    OldValue = null,
                    NewValue = JsonConvert.SerializeObject(model),
                    OperateUser = User.Identity.Name,
                    Remarks= $"新增美容首页Banner配置",
                };
                if (model.Id > 0)
                {
                    var oldModel = BeautyHomePageConfigManager.GetBeautyHomePageConfigById(model.Id) ?? new BeautyHomePageConfig();
                    oldModel.Region = JsonConvert.SerializeObject(MeiRongAcitivityConfigManager.GetRegion(model.Id, 3));
                    log.OldValue = JsonConvert.SerializeObject(oldModel);
                    log.Remarks = $"更新美容首页Banner配置";

                    result = BeautyHomePageConfigManager.UpdateBeautyHomePageBannerConfig(model);
                }
                else
                {
                    int outId = 0;
                    result = BeautyHomePageConfigManager.InsertBeautyHomePageBannerConfig(model, ref outId);

                    log.IdentityID = outId.ToString();
                }
                if (result)
                {
                    LoggerManager.InsertLog("BeautyOprLog", log);
                    UpdateBeautyBannerCache(model.Channel);
                }

            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { Result = result, Msg = msg });
        }
        private string _provinceName;
        public string ProvinceName
        {
            get
            {
                if (string.IsNullOrEmpty(_provinceName))
                    _provinceName = Request["provinceName"];
                return _provinceName;
            }
        }
        public JsonResult GetAllCitys()
        {
            using (var client = new Service.Shop.RegionClient())
            {
                var regions = client.GetRegionByRegionName(ProvinceName);
                regions.ThrowIfException(true);
                if (regions.Result.IsBelongMunicipality)
                    return Json(regions.Result.ChildRegions.Select(s => new { id = s.DistrictId, name = s.DistrictName }).ToArray());
                else
                {
                    return Json(regions.Result.ChildRegions.Select(s => new { id = s.CityId, name = s.CityName }).ToArray());
                }
            }
        }

        private HttpPostedFileBase _file;
        public HttpPostedFileBase SingleFile
        {
            get
            {
                if (_file == null && Request.Files.Count > 0)
                    _file = Request.Files[0];
                return _file;
            }
        }
        public async Task<JsonResult> UploadBeautyBannerImage(string type)
        {
            if (SingleFile == null)
                return Json(Tuple.Create(false, "请选择上传的图片！"));
            try
            {
                var bytes = new byte[SingleFile.InputStream.Length];
                SingleFile.InputStream.Read(bytes, 0, bytes.Length);
                using (var client = new FileUploadClient())
                {
                    var result = await client.UploadImageAsync(new Service.Utility.Request.ImageUploadRequest
                    {
                        Contents = bytes,
                        DirectoryName = "BeautyHomePageBanner"
                    });
                    result.ThrowIfException(true);
                    if (result.Success)
                    {
                        using (var image = System.Drawing.Image.FromStream(SingleFile.InputStream))
                        {
                            if (image.Width * 1.0 / image.Height != 2.0 / 1)
                                return Json(Tuple.Create(false, "图片宽高尺寸比例应为(2:1)"));
                        }
                        return Json(Tuple.Create(true, result.Result.GetImageUrl(), type));
                    }
                    return Json(Tuple.Create(false, $"上传失败:{result.ErrorMessage}", type));
                }

            }
            catch (Exception ex)
            {
                return Json(Tuple.Create(false, $"服务端异常：{ex.Message}", type));
            }

        }

        private bool UpdateBeautyBannerCache(string channel)
        {
            var result = false;
            using (var client = new GroupBuyingClient())
            {
                var temp = client.RefreshBeautyBannerCache(channel);
                temp.ThrowIfException(true);
                result = temp.Result;
            }
            return result;
        }

        #endregion

        #region 美容弹窗配置
        public ActionResult PopUpWindowsConfig()
        {
            return View();
        }

        public JsonResult GetPopUpWindowsConfigList()
        {
            int count = 0;
            var list = BeautyHomePageConfigManager.SelectBeautyPopUpConfig(Status, KeyWords, CurrentPage, PageSize, out count)?.ToList();
            Dictionary<string, List<BeautyCategorySimple>> dic = new Dictionary<string, List<BeautyCategorySimple>>();
            list.Select(s => s.Channel).Distinct().ForEach(f =>
            {
                if (!dic.ContainsKey(f))
                {
                    var categorys = BeautyHomePageConfigManager.GetBeautyCategoryByChannel(f)?.ToList();
                    if (categorys != null && categorys.Any())
                        dic.Add(f, categorys);
                }
            });
            list.ForEach(f =>
            {
                if (!string.IsNullOrEmpty(f.Channel) && f.CategoryId > 0 && dic.ContainsKey(f.Channel))
                    f.CategoryName = dic[f.Channel].FirstOrDefault(e => e.Id == f.CategoryId).Name;
            });



            return Json(new { result = list, total = count }, JsonRequestBehavior.AllowGet);
        }

        private int _pkid;
        public int PKID
        {
            get
            {
                if (!int.TryParse(Request["pkid"], out _pkid))
                    _pkid = 0;
                return _pkid;
            }
        }
        public ActionResult EditPopUpWindowsConfig()
        {
            return View(PKID);
        }
        public JsonResult GetPopUpWindowsConfigById()
        {
            var model = BeautyHomePageConfigManager.GetBeautyPopUpWindowsConfigById(PKID) ?? new BeautyPopUpWindowsConfig() { Channel = "ios" };
            var categorys = BeautyHomePageConfigManager.GetBeautyCategoryByChannel(model.Channel);
            List<PromotionInfoModel> promotions = new List<PromotionInfoModel>();
            if (!string.IsNullOrEmpty(model.PromotionInfo))
                promotions = JsonConvert.DeserializeObject<List<PromotionInfoModel>>(model.PromotionInfo);
            List<BeautyPopUpWindowsRegionModel> regions = new List<BeautyPopUpWindowsRegionModel>();
            if (model.IsRegion && model.PKID > 0)
                regions = BeautyHomePageConfigManager.GetBeautyPopUpWindowsRegionConfigs(model.PKID)?.ToList();


            return Json(new
            {
                result = model,
                CategoryList = categorys,
                PromotionList = promotions,
                RegionList = regions
            });
        }
        [HttpPost]
        public JsonResult GetBeautyCategorysByChannel(string channel)
        {
            IEnumerable<BeautyCategorySimple> categorys = null;
            bool success = true;
            string msg = "";
            try
            {
                categorys = BeautyHomePageConfigManager.GetBeautyCategoryByChannel(channel);
            }
            catch (Exception ex)
            {
                success = false;
                msg = ex.Message;
            }
            return Json(new
            {
                Success = success,
                CategoryList = categorys,
                Msg = msg
            });
        }
        [HttpPost]
        public JsonResult DeletePopUpWindowsConfigById()
        {
            string msg = string.Empty;
            bool result = false;
            try
            {
                result = BeautyHomePageConfigManager.DeleteBeautyPopUpWindowsConfig(PKID);
                if (result)
                    RefreshBeautyPopUpCache();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return Json(new
            {
                Result = result,
                Msg = msg
            });
        }
        [HttpPost]
        public JsonResult SavePopUpWindowsConfig(BeautyPopUpWindowsConfig model)
        {
            string msg = string.Empty;
            bool result = false;
            try
            {
                result = BeautyHomePageConfigManager.SaveBeautyPopUpWindowsConfig(model,User.Identity.Name);
                if (result)
                    RefreshBeautyPopUpCache();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { Result = result, Msg = msg });
        }

        public async Task<JsonResult> UploadBeautyPopUpImage(string type, int? id)
        {
            if (SingleFile == null)
                return Json(Tuple.Create(false, "请选择上传的图片！"));
            try
            {
                var bytes = new byte[SingleFile.InputStream.Length];
                SingleFile.InputStream.Read(bytes, 0, bytes.Length);
                using (var client = new FileUploadClient())
                {
                    var result = await client.UploadImageAsync(new Service.Utility.Request.ImageUploadRequest
                    {
                        Contents = bytes,
                        DirectoryName = "BeautyPopUpWindows"
                    });
                    result.ThrowIfException(true);
                    if (result.Success)
                    {
                        //using (var image = System.Drawing.Image.FromStream(SingleFile.InputStream))
                        //{
                        //    if (image.Width * 1.0 / image.Height != 2.0 / 1)
                        //        return Json(Tuple.Create(false, "图片宽高尺寸比例应为(2:1)"));
                        //}
                        return Json(Tuple.Create(true, result.Result.GetImageUrl(), type, id));
                    }
                    return Json(Tuple.Create(false, $"上传失败:{result.ErrorMessage}", type, id));
                }

            }
            catch (Exception ex)
            {
                return Json(Tuple.Create(false, $"服务端异常：{ex.Message}", type));
            }

        }

        private bool RefreshBeautyPopUpCache()
        {
            using (var client = new GroupBuyingClient())
            {
                var result = client.RefreshBeautyPopUpWindowsConfigs();
                result.ThrowIfException(true);
                return result.Success;
            }
        }
        #endregion
    }
}
