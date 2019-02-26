using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Service.Utility.Request;
using CarInsuranceManager = Tuhu.Provisioning.Business.CarInsurance.CarInsuranceManage;
using swc = System.Web.Configuration;

namespace Tuhu.Provisioning.Controllers
{
    public class CarInsuranceController : Controller
    {
        // GET: CarInsure
        public ActionResult Index()
        {
            return View();
        }

        #region banner

        [HttpPost]
        public ActionResult UpdateBannerIndex(string bannerIds)
        {
            if (string.IsNullOrEmpty(bannerIds))
                return Json(1);
            var status = -1;
            if (CarInsuranceManager.UpdateBannerIndex(bannerIds))
                status = 1;
            return Json(new { status = status, msg = status > 0 ? "排序成功" : "排序失败" });
        }

        [HttpPost]
        public ActionResult UpdateBanner(int bannerId, string name, string img, string linkUrl, string displayPage)
        {
            name = name?.Trim();
            img = img?.Trim();
            linkUrl = linkUrl?.Trim();
            displayPage?.Trim();
            var status = -1;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(img))
            {
                return Json(new { status = status, msg = "更新失败，输入不允许为空" });
            }
            if (CarInsuranceManager.UpdateBanner(bannerId, name, img, linkUrl, displayPage))
                status = 1;
            return Json(new { status = status, msg = status > 0 ? "更新成功" : "更新失败" });
        }

        [HttpPost]
        public ActionResult UpdateBannerIsEnable(int bannerId, int IsEnable)
        {
            var status = -1;
            if (CarInsuranceManager.UpdateBannerIsEnable(bannerId, IsEnable))
                status = 1;
            return Json(new { status = status, msg = status > 0 ? "更新成功" : "更新失败" });
        }

        [HttpPost]
        public ActionResult CreateBanner(string name, string img, string linkUrl, string displayPage)
        {
            name = name?.Trim();
            img = img?.Trim();
            linkUrl = linkUrl?.Trim();
            displayPage?.Trim();
            var status = -1;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(img))
            {
                return Json(new { status = status, msg = "添加失败，输入不允许为空" });
            }
            if (CarInsuranceManager.CreateBanner(name, img, linkUrl, displayPage, 0)) status = 1;
            return Json(new { status = status, msg = status > 0 ? "添加成功" : "添加失败" });
        }

        [HttpPost]
        public ActionResult DeleteBanner(int bannerId)
        {
            var status = -1;
            if (CarInsuranceManager.DeleteBanner(bannerId))
                status = 1;
            return Json(new { status = status, msg = status > 0 ? "删除成功" : "删除失败" });
        }

        #endregion


        #region insurance

        [HttpPost]
        public ActionResult UpdateInsuranceIndex(string insurancePartnerIds)
        {
            var status = -1;
            var insuranceIds = insurancePartnerIds
                ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                ?.Select(x => int.Parse(x))
                ?.ToList() ?? new List<int>();
            if (CarInsuranceManager.UpdateInsuranceIndex(insuranceIds))
                status = 1;
            return Json(new { status = status, msg = status > 0 ? "排序成功" : "排序失败" });
        }

        [HttpPost]
        public ActionResult UpdateInsurance(int insurancePartnerId, string name, string img, string linkUrl,
            string insuranceId, string providerCode, string regionCode, string remarks, string title, string subTitle,
            string tagText, string tagColor, int displayIndex, int isEnable, string regions)
        {
            var status = -1;
            name = name?.Trim();
            img = img?.Trim();
            insuranceId = insuranceId?.Trim();
            linkUrl = linkUrl?.Trim();
            providerCode = providerCode?.Trim();
            regionCode = regionCode?.Trim();
            remarks = remarks?.Trim();
            title = title?.Trim();
            subTitle = subTitle?.Trim();
            tagText = tagText?.Trim();
            tagColor = tagColor?.Trim();
            regions = regions?.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(title) ||
                (string.IsNullOrEmpty(providerCode) && string.IsNullOrEmpty(linkUrl)) || string.IsNullOrEmpty(img))
            {
                return Json(new { status = status, msg = status > 0 ? "更新成功" : "更新失败，输入不允许为空" });
            }

            var oldRegion = CarInsuranceManager.GetRegionByInsurancePartner(insurancePartnerId);
            if (CarInsuranceManager.UpdateInsurance(insurancePartnerId, name, img, linkUrl,
                insuranceId, remarks, title, subTitle, tagText, tagColor, displayIndex, isEnable,
                regionCode, providerCode) && CarInsuranceManager.UpdateInsuranceRegions(insurancePartnerId, regions))
                status = 1;
            var regionIds = oldRegion?.Select(x => x.CityId)?.ToList() ?? new List<int>();
            if (!regionIds.Any())
            {
                regionIds.Add(0);
            }
            regionIds.AddRange(regions
                ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                ?.Select(x => int.Parse(x))
                ?.ToList() ?? new List<int>());
            CarInsuranceManager.UpdatePartnerCache(regionIds);
            return Json(new
            {
                status = status,
                msg = status > 0 ? "更新成功" : "更新失败"
            });
        }

        [HttpPost]
        public ActionResult UpdateInsuranceRegions(int insurancePartnerId, string regions)
        {
            var status = -1;
            var oldRegion = CarInsuranceManager.GetRegionByInsurancePartner(insurancePartnerId);
            if (CarInsuranceManager.UpdateInsuranceRegions(insurancePartnerId, regions))
            {
                status = 1;
            }
            var regionIds = oldRegion?.Select(x => x.CityId)?.ToList() ?? new List<int>();
            if (!regionIds.Any())
            {
                regionIds.Add(0);
            }
            regionIds.AddRange(regions
                ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                ?.Select(x => int.Parse(x))
                ?.ToList() ?? new List<int>());
            CarInsuranceManager.UpdatePartnerCache(regionIds);
            return Json(new
            {
                status = status,
                msg = status > 0 ? "更新成功" : "更新失败"
            });
        }

        [HttpPost]
        public ActionResult UpdateInsuranceIsEnable(int insurancePartnerId, int isEnable)
        {
            var status = -1;
            var oldRegion = CarInsuranceManager.GetRegionByInsurancePartner(insurancePartnerId);
            if (CarInsuranceManager.UpdateInsuranceIsEnable(insurancePartnerId, isEnable))
                status = 1;

            return Json(new
            {
                status = status,
                msg = status > 0 ? "更新成功" : "更新失败"
            });
        }

        [HttpPost]
        public ActionResult CreateInsurance(string name, string img, string linkUrl, string insuranceId,
            string remarks, string title, string subTitle, string tagText, string tagColor,
            int displayIndex, int isEnable, string regions, string regionCode, string providerCode)
        {
            var status = -1;

            name = name?.Trim();
            img = img?.Trim();
            insuranceId = insuranceId?.Trim();
            linkUrl = linkUrl?.Trim();
            providerCode = providerCode?.Trim();
            regionCode = regionCode?.Trim();
            remarks = remarks?.Trim();
            title = title?.Trim();
            subTitle = subTitle?.Trim();
            tagText = tagText?.Trim();
            tagColor = tagColor?.Trim();
            regions = regions?.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(img))
                return Json(new
                {
                    status = status,
                    msg = status > 0 ? "添加成功" : "添加失败，输入不允许为空"
                });
            if (CarInsuranceManager.CreateInsurance(name, img, linkUrl, insuranceId,
                remarks, title, subTitle, tagText, tagColor,
                displayIndex, isEnable, regions, regionCode, providerCode))
                status = 1;

            return Json(new
            {
                status = status,
                msg = status > 0 ? "添加成功" : "添加失败"
            });
        }

        [HttpPost]
        public ActionResult DeleteInsurance(int insurancePartnerId)
        {
            var status = -1;
            var oldRegion = CarInsuranceManager.GetRegionByInsurancePartner(insurancePartnerId);
            if (CarInsuranceManager.DeleteInsurance(insurancePartnerId))
                status = 1;
            var regionIds = oldRegion?.Select(x => x.CityId)?.ToList() ?? new List<int>();
            if (!regionIds.Any())
            {
                regionIds.Add(0);
            }
            CarInsuranceManager.UpdatePartnerCache(regionIds);
            return Json(new
            {
                status = status,
                msg = status > 0 ? "删除成功" : "删除失败"
            });
        }

        #endregion

        #region select 

        [HttpPost]
        public ActionResult SelectBannerById(int bannerId)
        {
            var result = CarInsuranceManager.SelectBannerById(bannerId);
            return Json(result);
        }

        [HttpPost]
        public ActionResult SelectInsurancePartnerById(int insurancePartnerId)
        {
            var result = CarInsuranceManager.SelectInsurancePartnerById(insurancePartnerId);
            return Json(new
            {
                status = result == null ? -1 : 1,
                msg = result == null ? "获取信息失败" : "",
                result = result == null ? null : result
            });
        }

        [HttpPost]
        public ActionResult SelectRegionByInsuranceId(int insurancePartnerId)
        {
            var result = CarInsuranceManager.GetInsurancePartnerRegionIds(insurancePartnerId);
            return Json(new
            {
                status = result == null ? -1 : 1,
                msg = result == null ? "获取信息失败" : "",
                result = result == null ? null : result
            });
        }

        #endregion

        #region get

        public ActionResult GetAllProvince()
        {
            using (var client = new Service.Shop.RegionClient())
            {
                var result = client.GetAllProvince();
                if (!result.Success)
                {
                    result.ThrowIfException(true);
                }
                var provinces = result.Result;
                var test = provinces.ToList();
                return Json(provinces.Select(x => new { x.ProvinceName, x.ProvinceId }), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetRegionByName(int provinceId)
        {
            using (var client = new Service.Shop.RegionClient())
            {
                var result = client.GetRegionByRegionId(provinceId);
                if (!result.Success || result.Result == null)
                {
                    result.ThrowIfException(true);
                }
                var region = result.Result;
                return Json(region.ChildRegions.Select(x => new { x.CityId, x.CityName }), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region  FAQ

        [HttpPost]
        public ActionResult SelectFAQ()
        {
            var result = CarInsuranceManager.SelectFAQ();
            return Json(result);
        }

        [ValidateInput(false), HttpPost]
        public ActionResult UpdateFAQ(string FAQ)
        {
            var status = -1;
            if (string.IsNullOrEmpty(FAQ))
                FAQ = " ";
            if (CarInsuranceManager.UpdateFAQ(FAQ))
                status = 1;
            return Json(new
            {
                status = status,
                msg = status > 0 ? "更新成功" : "更新失败"
            });
        }

        #endregion

        #region updateCache

        public JsonResult UpdateBannerCache()
        {
            var result = CarInsuranceManager.UpdateBannerCache();
            return Json(new { Status = result });
        }

        public JsonResult UpdateFooterCache()
        {
            var result = CarInsuranceManager.UpdateFooterCache();
            return Json(new { Status = result });
        }

        public JsonResult UpdatePartnerCache()
        {
            var regionids = CarInsuranceManager.GetRegionIds();
            var result = CarInsuranceManager.UpdatePartnerCache(regionids);
            return Json(new { Status = result });
        }

        #endregion

        #region 图片上传

        public ActionResult ImageUploadToAli(double hwProporLimit)
        {
            if (Request.Files.Count <= 0)
            {
                return Json(new
                {
                    BImage = string.Empty,
                    SImage = string.Empty,
                    Msg = "请先选择要上传的图片!",
                }, "text/html");
            }

            var file = Request.Files[0];

            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
            {
                return Json(new
                {
                    BImage = string.Empty,
                    SImage = string.Empty,
                    Msg = "请上传jpg或者png!",
                }, "text/html");
            }

            System.Drawing.Image imgFile = System.Drawing.Image.FromStream(file.InputStream);
            var proportion = imgFile.Height * 1.0 / imgFile.Width;
            if (hwProporLimit != 0 && proportion != hwProporLimit)
            {
                string msg = "";
                if (hwProporLimit == 0.375)
                {
                    msg = "图片宽高比不是8:3";
                }

                if (hwProporLimit == 1)
                {
                    msg = "图片宽高比不是1:1";
                }

                return Json(new
                {
                    BImage = string.Empty,
                    SImage = string.Empty,
                    Msg = msg,
                }, "text/html");
            }

            if (file.ContentLength > 480 * 1024)
            {
                return Json(new
                {
                    BImage = string.Empty,
                    SImage = string.Empty,
                    Msg = "图片大小超过480KB限制!",
                }, "text/html");
            }

            string _BImage = string.Empty;
            var message = string.Empty;

            try
            {
                var buffer = new byte[file.ContentLength];
                file.InputStream.Position = 0;
                file.InputStream.Read(buffer, 0, buffer.Length);
                using (var client = new Tuhu.Service.Utility.FileUploadClient())
                {
                    string dirName = swc.WebConfigurationManager.AppSettings["UploadDoMain_image"];

                    var result = client.UploadImage(new ImageUploadRequest(dirName, buffer));
                    result.ThrowIfException(true);
                    if (result.Success)
                    {
                        _BImage = result.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            string imgUrl = string.IsNullOrEmpty(_BImage) ? string.Empty : $"{swc.WebConfigurationManager.AppSettings["DoMain_image"]}{_BImage}";
            return Json(new
            {
                BImage = imgUrl,
                SImage = imgUrl,
                Msg = !string.IsNullOrEmpty(message) ? "上传成功" : message
            }, "text/html");
        }

        #endregion

    }
}