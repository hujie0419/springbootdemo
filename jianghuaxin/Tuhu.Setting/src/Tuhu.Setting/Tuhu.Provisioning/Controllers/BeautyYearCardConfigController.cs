using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.BeautyYearCardConfig;
using Tuhu.Provisioning.DataAccess.Entity.BeautyYearCard;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Request;
using Tuhu.Service.Utility;

namespace Tuhu.Provisioning.Controllers
{
    public class BeautyYearCardConfigController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger<BeautyYearCardConfigController>();
        // GET: BeautyYearCardConfig
        private int _pageIndex;
        private int PageIndex
        {
            get
            {
                if (_pageIndex <= 0)
                {
                    if (!int.TryParse(Request["PageIndex"], out _pageIndex))
                        _pageIndex = 1;
                }
                return _pageIndex;
            }
        }
        private int _pageSize;
        private int PageSize
        {
            get
            {
                if (_pageSize <= 0)
                {
                    if (!int.TryParse(Request["PageSize"], out _pageSize) || _pageSize >= 200)
                        _pageSize = 20;
                }
                return _pageSize;
            }
        }

        private string _keyWord;
        private string KeyWord
        {
            get
            {
                if (string.IsNullOrEmpty(_keyWord))
                {
                    _keyWord = Request["KeyWord"];
                }
                return _keyWord;
            }
        }
        private int _searchNameType;
        private int SearchNameType
        {
            get
            {
                if (_searchNameType <= 0)
                {
                    int.TryParse(Request["SearchNameType"], out _searchNameType);
                }
                return _searchNameType;
            }
        }
        private int _searchStatusType;
        private int SearchStatusType
        {
            get
            {
                if (_searchStatusType <= 0)
                {
                    int.TryParse(Request["SearchStatusType"], out _searchStatusType);
                }
                return _searchStatusType;
            }
        }
        public ActionResult Index()
        {
            return View();
        }
        private string shopArea;
        public string ShopArea
        {
            get
            {
                if (string.IsNullOrEmpty(shopArea))
                    shopArea = Request["ShopArea"];
                return shopArea;
            }
        }
        public JsonResult GetBeautyYearCardConfigs()
        {
            var result = BeautyYearCardConfigManger.Instanse.GetBeautyYearCardConfigList(PageIndex, PageSize, KeyWord, SearchNameType, SearchStatusType);
            if (result?.Item2?.Any() ?? false)
            {
                var provincename = string.IsNullOrEmpty(ShopArea) ? "上海市" : ShopArea;
                var temp = result.Item2;
                temp.ForEach(f =>
                {
                    f.ShopCount = GetSupproseShopCount(provincename, f.BeautyYearCardProducts?.Select(s => s.ProductId).ToList());
                });
                return Json(Tuple.Create(result.Item1, temp));
            }
            return Json(result);
        }

        public ActionResult EditBeautyYearCardConfig()
        {
            return View(CardId);
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
        public async Task<JsonResult> UploadBeautyYearCardImage()
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
                        DirectoryName = "BeautyYearCard"
                    });
                    result.ThrowIfException(true);
                    if (result.Success)
                    {
                        return Json(Tuple.Create(true, result.Result.GetImageUrl()));
                    }
                    return Json(Tuple.Create(false, $"上传失败:{result.ErrorMessage}"));
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(Tuple.Create(false, $"服务端异常：{ex.Message}"));
            }

        }

        public async Task<JsonResult> GetAllProvinces()
        {

            try
            {
                using (var client = new RegionClient())
                {
                    var regions = await client.GetAllProvinceAsync();
                    regions.ThrowIfException(true);
                    var result = regions.Result.Select(s => new { id = s.ProvinceId, name = s.ProvinceName }).ToArray();
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"GetAllProvinces:{ex.Message}", ex);
            }
            return Json(null);
        }
        private int provinceId;
        public int ProvinceId
        {
            get
            {
                if (provinceId <= 0)
                    int.TryParse(Request["ProvinceId"], out provinceId);
                return provinceId;
            }
        }
        public async Task<JsonResult> GetAllCitys()
        {
            try
            {
                using (var client = new RegionClient())
                {
                    var regions = await client.GetRegionByRegionIdAsync(ProvinceId);
                    regions.ThrowIfException(true);
                    if (regions.Result.ChildRegions.FirstOrDefault().IsBelongMunicipality)
                        return Json(regions.Result.ChildRegions.Select(s => new { id = s.DistrictId, name = s.DistrictName }).ToArray());
                    else
                    {
                        return Json(regions.Result.ChildRegions.Select(s => new { id = s.CityId, name = s.CityName }).ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"GetAllCitys:{ex.Message}", ex);
            }
            return Json(null);
        }
        private int cardId;
        public int CardId
        {
            get
            {
                if (cardId <= 0)
                    int.TryParse(Request["CardId"], out cardId);
                return cardId;

            }
        }
        public JsonResult GetBeautyYearCardConfigDetailById()
        {
            var result = BeautyYearCardConfigManger.Instanse.GetBeautyYearCardConfigDetail(CardId);
            return Json(result);
        }

        public JsonResult SaveBeautyYearCardConfig(BeautyYearCardModel request)
        {
            if (request == null)
                return Json(Tuple.Create(false, $"请求参数不能为空：{JsonConvert.SerializeObject(request)}"));
            var result = BeautyYearCardConfigManger.Instanse.SaveBeautyYearCardConfig(request, User.Identity.Name);
            if (result)
                return Json(Tuple.Create(result, ""));
            return Json(Tuple.Create(result, "服务端执行异常！"));
        }

        public JsonResult DeleteBeautyYearCardConfig(int pkid)
        {
            if (pkid <= 0)
                return Json(Tuple.Create(false, $"请求参数不能为空：{JsonConvert.SerializeObject(pkid)}"));
            var result = BeautyYearCardConfigManger.Instanse.DeleteBeautyYearCardCofing(pkid, User.Identity.Name);
            if (result)
                return Json(Tuple.Create(result, ""));
            return Json(Tuple.Create(result, "服务端异常"));
        }
        public JsonResult GetBeautyYearCardConfigLog(int cardId)
        {
            if (cardId <= 0)
                return Json(null);
            var result = BeautyYearCardConfigManger.Instanse.GetBeautyYearCardConfigLog(cardId);
            return Json(result);
        }
        public ActionResult BeautyYearCardConfigLog()
        {
            return View(CardId);
        }
        public JsonResult GetSimpleProductInfo(string pid)
        {
            if (string.IsNullOrEmpty(pid))
                return Json(null);
            var result = BeautyYearCardConfigManger.Instanse.GetProductSimpleInfo(pid);
            return Json(result);
        }

        private int GetSupproseShopCount(string provinceName, List<string> pids)
        {
            ShopSearchRequest request = new ShopSearchRequest()
            {
                ProvinceName = provinceName,
                Filters = new List<ShopQueryFilterModel>()
                {
                    new ShopQueryFilterModel()
                    {
                        FilterValueType=ShopQueryFilterValueType.MrPid.ToString(),
                        Values=pids,//传服务ID
                        JoinType=JoinType.And.ToString()
                    }
                },
                PageSize = 10,//页面大小
                PageIndex = 1//第几页
            };
            int result = 0;
            try
            {
                using (var clinet = new ShopClient())
                {
                    var serviceResult = clinet.SearchShopIds(request);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result.Pager.Total;
                }
            }
            catch (Exception ex)
            {
                logger.Error($"SearchShopIds:{ex.Message}", ex);
            }
            return result;
        }
    }
}