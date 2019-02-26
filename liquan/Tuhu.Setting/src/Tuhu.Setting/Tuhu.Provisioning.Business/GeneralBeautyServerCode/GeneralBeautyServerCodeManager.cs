using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.Business.BeautyService;
using Tuhu.Provisioning.DataAccess.DAO.GeneralBeautyServerCode;
using Tuhu.Provisioning.DataAccess.Entity.GeneralBeautyServerCode;

namespace Tuhu.Provisioning.Business.GeneralBeautyServerCode
{
    public class GeneralBeautyServerCodeManager
    {
        private DalGeneralBeautyServerCode _DalGeneralBeautyServerCode;

        private static ILog _logger = LogManager.GetLogger("GeneralBeautyServerCodeManager");
        private GeneralBeautyServerCodeManager()
        {
            _DalGeneralBeautyServerCode = new DalGeneralBeautyServerCode();
        }
        private static object _lockObj = new object();
        private static GeneralBeautyServerCodeManager _Instanse;

        public static GeneralBeautyServerCodeManager Instanse
        {
            get
            {
                if (_Instanse == null)
                {
                    lock (_lockObj)
                    {
                        if (_Instanse == null)
                        {
                            _Instanse = new GeneralBeautyServerCodeManager();
                        }
                    }
                }
                return _Instanse;
            }
        }

        /// <summary>
        /// 获取美容通用服务码模板列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="appId"></param>
        /// <param name="settlementMethod"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<ThirdPartyBeautyPackageConfigModel> GetGeneralBeautyServerCodeTmpList(int pageIndex, int pageSize, int cooperateId, string settlementMethod, out int count)
        {
            IEnumerable<ThirdPartyBeautyPackageConfigModel> result = null;
            int dalCount = 0;
            try
            {
                result = _DalGeneralBeautyServerCode.GetGeneralBeautyServerCodeTmpList(pageIndex, pageSize, cooperateId, settlementMethod, out dalCount);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            count = dalCount;
            return result;
        }
        /// <summary>
        /// 获取美容通用服务码模板详情
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="appId"></param>
        /// <param name="settlementMethod"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ThirdPartyBeautyPackageConfigModel GetGeneralBeautyServerCodeTmpDetail(Guid packageId)
        {
            ThirdPartyBeautyPackageConfigModel result = null;
            try
            {
                result = _DalGeneralBeautyServerCode.GetGeneralBeautyServerCodeTmpDetail(packageId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 保存美容通用服务码模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> SaveGeneralBeautyServerCodeTmpAsync(ThirdPartyBeautyPackageConfigModel model)
        {
            var result = false;
            try
            {
                if (model.PKID > 0)
                    result = await _DalGeneralBeautyServerCode.UpdateGeneralBeautyServerCodeTmpAsync(model);
                else
                {
                    model.PackageId = Guid.NewGuid();
                    result = await _DalGeneralBeautyServerCode.SaveGeneralBeautyServerCodeTmpAsync(model);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 删除美容通用服务码模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> DeleteGeneralBeautyServerCodeTmpAsync(Guid packageId)
        {
            var result = false;
            try
            {
                if (packageId != Guid.Empty)
                    result = await _DalGeneralBeautyServerCode.DeleteGeneralBeautyServerCodeTmpAsync(packageId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 获取美容服务包下所有产品
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ThirdPartyBeautyPackageProductConfigModel>> GetGeneralBeautyServerCodeProductsListAsync(Guid packageId)
        {
            IEnumerable<ThirdPartyBeautyPackageProductConfigModel> result = null;
            try
            {
                result = await _DalGeneralBeautyServerCode.GetGeneralBeautyServerCodeProductsListAsync(packageId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 获取美容服务包下所有产品
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public async Task<ThirdPartyBeautyPackageProductConfigModel> GetGeneralBeautyServerCodeProductsDetailAsync(int pkid)
        {
            ThirdPartyBeautyPackageProductConfigModel result = null;
            try
            {
                result = await _DalGeneralBeautyServerCode.GetGeneralBeautyServerCodeProductsDetailAsync(pkid);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 保存美容服务包下产品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> SaveGeneralBeautyServerCodeProductsAsync(ThirdPartyBeautyPackageProductConfigModel model)
        {
            var result = false;
            try
            {
                if (model.PKID > 0)
                    result = await _DalGeneralBeautyServerCode.UpdateGeneralBeautyServerCodeProductsAsync(model);
                else
                {
                    result = await _DalGeneralBeautyServerCode.SaveGeneralBeautyServerCodeProductsAsync(model);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 删除美容服务包下产品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> DeleteGeneralBeautyServerCodeProductsAsync(int pkid)
        {
            var result = false;
            try
            {
                if (pkid > 0)
                    result = await _DalGeneralBeautyServerCode.DeleteGeneralBeautyServerCodeProductsAsync(pkid);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 获取通用美容服务码发放记录列表
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<ThirdPartyBeautyPackageRecordModel> GetGeneralBeautyServerCodeSendRecords(Guid? packageId, int pageIndex, int pageSize, int cooperateId, string settlementMethod, out int count)
        {
            IEnumerable<ThirdPartyBeautyPackageRecordModel> result = null;
            int dalCount = 0;
            try
            {
                result = _DalGeneralBeautyServerCode.GetGeneralBeautyServerCodeSendRecords(packageId ?? Guid.Empty, pageIndex, pageSize, cooperateId, settlementMethod, out dalCount);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            count = dalCount;
            return result;
        }
        /// <summary>
        /// 获取通用美容服务码发放记录详情
        /// </summary>
        /// <param name="packageRecordId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ThirdPartyBeautyPackageRecordDetailModel>> GetGeneralBeautyServerCodeSendRecordDetail(int packageRecordId)
        {
            IEnumerable<ThirdPartyBeautyPackageRecordDetailModel> result = null;
            try
            {
                result = await _DalGeneralBeautyServerCode.GetGeneralBeautyServerCodeSendRecordDetail(packageRecordId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 获取通用美容服务码发放记录
        /// </summary>
        /// <param name="packageRecordId"></param>
        /// <returns></returns>
        public async Task<ThirdPartyBeautyPackageRecordModel> GetGeneralBeautyServerCodeSendRecordById(int pkid)
        {
            ThirdPartyBeautyPackageRecordModel result = null;
            try
            {
                result = await _DalGeneralBeautyServerCode.GetGeneralBeautyServerCodeSendRecordById(pkid);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }

        public async Task<Tuple<bool, object, string>> GetGeneralBeautyServerCodeSendRecordsDetail(int pkid)
        {
            var sendRecordDetail = (await GetGeneralBeautyServerCodeSendRecordDetail(pkid))?.ToArray();
            if (sendRecordDetail == null && !sendRecordDetail.Any())
                return Tuple.Create<bool, object, string>(false, null, "服务码详细记录信息为空");//Json(new { Result = new { }, IsSuccess = false, Msg = "" });
            var sendRecord = await GetGeneralBeautyServerCodeSendRecordById(pkid);
            if (sendRecord == null)
                return Tuple.Create<bool, object, string>(false, null, "服务码记录信息为空");
            var productList = await GetGeneralBeautyServerCodeProductsListAsync(sendRecord.PackageId);
            if (sendRecord == null)
                return Tuple.Create<bool, object, string>(false, null, "服务码记录对应产品信息为空");
            var codeTypeConfigs = BeautyServicePackageManager.SelectAllBeautyServiceCodeTypeConfig(0)?.ToList();
            var codes = (await new KuaiXiuService.KuaiXiuService().GetServiceCodeDetailsByCodes(sendRecordDetail.Select(s => s.ServiceCode)))?.ToArray();
            var shopDetails = (await ShopServiceProxy.ShopServiceProxy.Instanse.SelectShopDetailsAsync(codes.Where(w => (w.InstallShopId ?? 0) > 0).Select(s => s.InstallShopId ?? 0).Distinct()))?.ToArray();
            var result = from a in sendRecordDetail
                         join b in productList
                         on new { a.PackageId, a.PackageProductId } equals new { b.PackageId, PackageProductId = b.PKID }
                         where a.PackageId == sendRecord.PackageId
                         select new
                         {
                             ServiceCode = a.ServiceCode,
                             Name = b.Name,
                             CodeTypeConfigId = b.CodeTypeConfigId,
                             Phone = sendRecord.Phone,
                             PID = codeTypeConfigs?.FirstOrDefault(f => f.PKID == b.CodeTypeConfigId)?.PID,
                             ShopName = shopDetails?.FirstOrDefault(f => f.ShopId == codes.FirstOrDefault(e => e.Code == a.ServiceCode)?.InstallShopId)?.SimpleName,
                             VerifyTime = codes.FirstOrDefault(f => f.Code == a.ServiceCode)?.VerifyTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                             VerifyArea = shopDetails?.FirstOrDefault(f => f.ShopId == codes.FirstOrDefault(e => e.Code == a.ServiceCode)?.InstallShopId)?.Province + shopDetails?.FirstOrDefault(f => f.ShopId == codes.FirstOrDefault(e => e.Code == a.ServiceCode)?.InstallShopId)?.City,
                         };
            return Tuple.Create<bool, object, string>(true, result, "");


        }

        public async Task<IEnumerable<ThirdPartyBeautyPackageProductRegionConfigModel>> GetGeneralBeautyServerCodeProductRegions(int pkid)
        {

            IEnumerable<ThirdPartyBeautyPackageProductRegionConfigModel> result = null;
            try
            {
                result = await _DalGeneralBeautyServerCode.GetGeneralBeautyServerCodeProductRegions(pkid);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }
        public async Task<bool> SaveGeneralBeautyServerCodeProductRegions(IEnumerable<ThirdPartyBeautyPackageProductRegionConfigModel> models)
        {

            var result = false;
            try
            {
                result = await _DalGeneralBeautyServerCode.SaveGeneralBeautyServerCodeProductRegions(models);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 获取通用服务码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="serviceCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<List<GeneralBeautyServerCodes>> GetGeneralBeautyServerCodes(string mobile, string serviceCode, int pageIndex, int pageSize)
        {
            var result = await DalGeneralBeautyServerCode.GetGeneralBeautyServerCodes(mobile, serviceCode, pageIndex, pageSize);
            if (result != null && result.Any())
            {
                var codes = CommonServices.KuaiXiuService.GetServiceCodeDetailsByCodes(result.Select(x => x.ServiceCode).ToList());
                if (codes != null && codes.Any())
                {
                    foreach (var item in result)
                    {
                        var code = codes.Where(y => String.Equals(item.ServiceCode, y.Code))?.FirstOrDefault() ?? new Service.KuaiXiu.Models.ServiceCode(); ;
                        item.Status = code.Status.ToString();
                        item.StartTime = code.CreateTime;
                        item.EndTime = code.OverdueTime;
                        item.TuhuOrderId = code.TuhuOrderId;
                        item.VerifyTime = code.VerifyTime;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 获取所有的美容包配置
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ThirdPartyBeautyPackageConfigModel> GetAllThirdPartyBeautyPackageConfig()
        {
            IEnumerable<ThirdPartyBeautyPackageConfigModel> result = null;
            try
            {
                result = _DalGeneralBeautyServerCode.GetAllThirdPartyBeautyPackageConfig();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }
    }
}
