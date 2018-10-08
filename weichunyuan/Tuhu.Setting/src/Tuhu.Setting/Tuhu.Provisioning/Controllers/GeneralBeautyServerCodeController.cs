using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.BeautyService;
using Tuhu.Provisioning.Business.GeneralBeautyServerCode;
using Tuhu.Provisioning.Business.KuaiXiuService;
using Tuhu.Provisioning.Business.UnivRedemptionCode;
using Tuhu.Provisioning.DataAccess.Entity.GeneralBeautyServerCode;
using Tuhu.Service.Enterprise;
namespace Tuhu.Provisioning.Controllers
{
    public class GeneralBeautyServerCodeController : Controller
    {
        // GET: GeneralBeautyServerCode
        private readonly static ILog _logger = LogManager.GetLogger(nameof(GeneralBeautyServerCodeController));
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取通用服务码配置模板列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetGeneralBeautyServerCodeTmpList(int pageIndex, int pageSize, int? cooperateId, string settlementMethod)
        {
            int total = 0;
            var result = GeneralBeautyServerCodeManager.Instanse.GetGeneralBeautyServerCodeTmpList(pageIndex, pageSize, cooperateId ?? 0, settlementMethod, out total)?.ToArray();
            var cooperUsers = new BankMRManager().GetAllMrCooperateUserConfigs()?.ToList();
            result.ForEach(f =>
            {
                f.CooperateName = cooperUsers?.FirstOrDefault(e => e.PKID == f.CooperateId)?.CooperateName;
            });
            return Json(new
            {
                Result = result,
                Total = total
            });
        }
        /// <summary>
        /// 保存通用服务码配置模板
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveGeneralBeautyServerCodeTmp(ThirdPartyBeautyPackageConfigModel model)
        {
            bool result = false;
            if (model == null)
                return Json(new { Result = result });
            model.CreatedUser = User.Identity.Name;
            result = await GeneralBeautyServerCodeManager.Instanse.SaveGeneralBeautyServerCodeTmpAsync(model);
            return Json(new { Result = result });
        }
        /// <summary>
        /// 删除通用服务码配置模板
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> DeleteGeneralBeautyServerCodeTmp(Guid? packageId)
        {
            var result = await GeneralBeautyServerCodeManager.Instanse.DeleteGeneralBeautyServerCodeTmpAsync(packageId ?? Guid.NewGuid());
            return Json(new { Result = result, Msg = "" });
        }
        /// <summary>
        /// 通用服务码配置模板详情
        /// </summary>
        /// <returns></returns>
        public ActionResult GeneralBeautyServerCodeTmpDetail(Guid? packageId)
        {
            ViewBag.PackageId = packageId;
            return View();
        }
        /// <summary>
        /// 获取通用服务码配置模板详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetGeneralBeautyServerCodeTmpDetail(Guid? packageId)
        {
            if (packageId == null || packageId == Guid.Empty)
                return Json(new { Result = new ThirdPartyBeautyPackageConfigModel() });
            var result = GeneralBeautyServerCodeManager.Instanse.GetGeneralBeautyServerCodeTmpDetail(packageId ?? Guid.Empty);
            return Json(new { Result = result });
        }
        /// <summary>
        /// 通用美容服务码产品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GeneralBeautyServerCodeProductsList(Guid packageId)
        {
            ViewBag.PackageId = packageId;
            return View();
        }
        /// <summary>
        ///获取通用美容服务码产品列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetGeneralBeautyServerCodeProductsList(Guid packageId)
        {
            var result = (await GeneralBeautyServerCodeManager.Instanse.GetGeneralBeautyServerCodeProductsListAsync(packageId))?.ToArray();
            var codeTypeConfigs = BeautyServicePackageManager.SelectAllBeautyServiceCodeTypeConfig(0)?.ToList();
            result?.ForEach(f =>
            {
                f.PID = codeTypeConfigs?.FirstOrDefault(e => e.PKID == f.CodeTypeConfigId)?.PID;
            });
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 通用美容服务码产品详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GeneralBeautyServerCodeProductsDetail(int? pkid, Guid? packageId)
        {
            ViewBag.PKID = pkid;
            ViewBag.PackageId = packageId;
            return View();
        }
        /// <summary>
        /// 保存通用美容服务码产品服务详情
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveGeneralBeautyServerCodeProductDetail(ThirdPartyBeautyPackageProductConfigModel model)
        {
            var tmp = GeneralBeautyServerCodeManager.Instanse.GetGeneralBeautyServerCodeTmpDetail(model.PackageId);
            model.SettlementMethod = tmp.SettlementMethod;
            model.CreatedUser = User.Identity.Name;
            var result = await GeneralBeautyServerCodeManager.Instanse.SaveGeneralBeautyServerCodeProductsAsync(model);
            return Json(new { Result = result });
        }
        /// <summary>
        /// 删除通用美容服务码产品服务
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteGeneralBeautyServerCodeProduct(int pkid)
        {
            var reuslt = await GeneralBeautyServerCodeManager.Instanse.DeleteGeneralBeautyServerCodeProductsAsync(pkid);
            return Json(new { Result = reuslt, Msg = "" });
        }
        /// <summary>
        ///获取通用美容服务码产品服务详情
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetGeneralBeautyServerCodeProductDetail(int? pkid)
        {
            if (pkid == null || pkid <= 0)
                return Json(new { Result = new ThirdPartyBeautyPackageProductConfigModel() });
            var result = await GeneralBeautyServerCodeManager.Instanse.GetGeneralBeautyServerCodeProductsDetailAsync(pkid ?? 0);
            return Json(new { Result = result });
        }
        /// <summary>
        /// 通用美容服务码发放记录
        /// </summary>
        /// <returns></returns>
        public ActionResult GeneralBeautyServerCodeSendRecords()
        {
            return View();
        }
        /// <summary>
        /// 获取通用美容服务码发放记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetGeneralBeautyServerCodeSendRecords(int pageIndex, int pageSize, int? cooperateId, string settlementMethod)
        {
            int total = 0;
            var result = GeneralBeautyServerCodeManager.Instanse.GetGeneralBeautyServerCodeSendRecords(null, pageIndex, pageSize, cooperateId ?? 0, settlementMethod, out total)?.ToArray();
            var cooperUsers = new BankMRManager().GetAllMrCooperateUserConfigs()?.ToList();
            result?.ForEach(f =>
            {
                var packageConfig = GeneralBeautyServerCodeManager.Instanse.GetGeneralBeautyServerCodeTmpDetail(f.PackageId);
                f.PackageName = packageConfig.PackageName;
                f.SettlementMethod = packageConfig.SettlementMethod;
                f.CooperateName = cooperUsers?.FirstOrDefault(e => e.PKID == packageConfig.CooperateId)?.CooperateName;
            });

            return Json(new { Result = result, Total = total });
        }
        /// <summary>
        /// 服务包作废
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> InvaildBeautyServerCodePackage(string serialNumber, Guid packageId)
        {
            var result = false;
            var msg = string.Empty;
            try
            {
                using (var client = new BeautyCodeClient())
                {
                    var temp = await client.InvalidBeautyServiceCodeAsync(new Service.Enterprise.Models.InvalidBeautyServiceCodeReqeust
                    {
                        AppId = "test",
                        PackageId = packageId,
                        SerialNumber = serialNumber,
                    });
                    temp.ThrowIfException(true);
                    result = temp.Result.Data;
                    msg = temp.Result.Message;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                _logger.Error(ex.Message, ex);
            }
            return Json(new { Result = result, Msg = msg });
        }
        /// <summary>
        /// 查询服务码兑换记录详细信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetGeneralBeautyServerCodeSendRecordsDetail(int pkid)
        {
            var result = await GeneralBeautyServerCodeManager.Instanse.GetGeneralBeautyServerCodeSendRecordsDetail(pkid);
            return Json(new { Result = result.Item2, IsSuccess = result.Item1, Msg = result.Item2 });
        }
        /// <summary>
        /// 查询包产品限购地区信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetGeneralBeautyServerCodeProductRegions(int pkid)
        {
            IEnumerable<ThirdPartyBeautyPackageProductRegionConfigModel> result = null;
            if (pkid <= 0)
                return Json(new { Result = result, IsSuccess = false, Msg = "请求参数不符合要求" });
            result = await GeneralBeautyServerCodeManager.Instanse.GetGeneralBeautyServerCodeProductRegions(pkid);
            return Json(new { Result = result, IsSuccess = true, Msg = "" });
        }
        /// <summary>
        /// 保存包产品限购地区信息
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SaveGeneralBeautyServerCodeProductRegions(IEnumerable<ThirdPartyBeautyPackageProductRegionConfigModel> models)
        {
            bool result = false;
            if (models == null || !models.Any())
                return Json(new { Result = result, Msg = "请求参数不符合要求" });
            result = await GeneralBeautyServerCodeManager.Instanse.SaveGeneralBeautyServerCodeProductRegions(models);
            return Json(new { Result = result, Msg = "" });
        }
        /// <summary>
        /// 获取所有大客户得openApp配置
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllBigCustomerOpenAppIds()
        {
            UnivRedemptionCodeManager manager = new UnivRedemptionCodeManager();
            var result = manager.GetAllBigCustomerOpenAppIds();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}