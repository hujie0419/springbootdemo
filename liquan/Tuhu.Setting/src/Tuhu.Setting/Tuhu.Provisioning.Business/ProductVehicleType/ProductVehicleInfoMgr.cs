using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ProductVehicleType
{
    public class ProductVehicleInfoMgr : IProductVehicleInfoMgr
    {
        static string strReadConn = ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString;
        static string strConn = ConfigurationManager.ConnectionStrings["StarterSite_productcatalogConnectionString"].ConnectionString;
        private static readonly IConnectionManager tuhuProductConnectionManager =
            new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);

        private static readonly IConnectionManager tuhuProductConnectionManagerReadOnly =
            new ConnectionManager(SecurityHelp.IsBase64Formatted(strReadConn) ? SecurityHelp.DecryptAES(strReadConn) : strReadConn);

        private static readonly IDBScopeManager TuHuProductDbScopeManager = new DBScopeManager(tuhuProductConnectionManager);

        private static readonly IDBScopeManager TuHuProductDbScopeManagerReadOnly = new DBScopeManager(tuhuProductConnectionManagerReadOnly);

        static string strConn1 = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static readonly IConnectionManager tuhuVehicleConnectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn1) ? SecurityHelp.DecryptAES(strConn1) : strConn1);
        private static readonly IDBScopeManager tuhuVehicleDbScopeManager = new DBScopeManager(tuhuVehicleConnectionManager);

        private ProductVehicleInfoHandler productHandler = null;
        private VehicleTypeHandler vehicleHandler = null;
        private ProductVehicleInfoHandler productHandlerReadOnly = null;

        public ProductVehicleInfoMgr()
        {
            productHandler = new ProductVehicleInfoHandler(TuHuProductDbScopeManager, TuHuProductDbScopeManagerReadOnly);
            vehicleHandler = new VehicleTypeHandler(tuhuVehicleDbScopeManager);
            productHandlerReadOnly = new ProductVehicleInfoHandler(null, tuhuVehicleDbScopeManager);
        }

        #region 产品相关

        public List<ProductInfo> SearchProductInfoByParam(string input,int pageIndex, int pageSize,int type)
        {
            return productHandler.SearchProductInfoByParam(input,pageIndex,pageSize,type);
        }

        public ProductInfo GetProductInfoByPid(string pid)
        {
            return productHandler.GetProductInfoByPid(pid);
        }

        public List<ProductVehicleTypeFileInfoDb> GetExcelInfoByPid(string pid)
        {
            return productHandler.GetExcelInfoByPid(pid);
        }

        public bool SaveProductVehicleExcelInfo(ProductVehicleTypeFileInfoDb entity)
        {
            return productHandler.SaveProductVehicleExcelInfo(entity);
        }

        public List<ProductInfo> GetProductInfoByPids(string pids)
        {
            return productHandler.GetProductInfoByPids(pids);
        }

        public bool SaveProductInfoByPid(string pid, string cpremark, bool isAuto,string vehicleLevel)
        {
            return productHandler.SaveProductInfoByPid(pid, cpremark, isAuto,vehicleLevel);
        }

        public bool ImportVehicleInfoByPid(string pid, DataTable table, string fileName, string cpremark)
        {
            return productHandler.InsertOrUpdateVehicleInfoByPID(pid, table, fileName, cpremark);
        }

        public bool UpdateVehicleInfoByPid(string pid, DataTable table, string level, string vehicleLevel)
        {
            return productHandler.UpdateVehicleInfoByPid(pid, table, level, vehicleLevel);
        }

        public List<ProductInfo> GetAllNoImportProduct(int pageIndex, int pageSize)
        {
            return productHandler.GetAllNoImportProduct(pageIndex, pageSize);
        }

        #endregion

        #region 车型相关
        /// <summary>
        /// 通过品牌首字母过滤查询对应所有车型信息
        /// </summary>
        /// <param name="alphabet"></param>
        /// <returns></returns>
        public List<VehicleTypeInfoDb> GetVehicleTypeInfoByCharacter(string alphabet)
        {
            return vehicleHandler.GetVehicleTypeInfoByCharacter(alphabet);
        }

        public List<VehicleTypeInfoDb> GetVehicleTypeInfoByBrandName(string brandNames)
        {
            return vehicleHandler.GetVehicleTypeInfoByBrandName(brandNames);
        }
        #endregion

        #region 产品车型配置相关
        public bool InsertProductVehicleTypeConfigInfoByPid(string pid, string destPids, string remark, string vehicleLevel)
        {
            return productHandler.InsertProductVehicleTypeConfigInfoByPid(pid, destPids, remark, vehicleLevel);
        }

        public List<ProductVehicleTypeConfigDb> GetProductVehicleTypeConfigInfoListByPid(string pid)
        {
            return productHandler.GetProductVehicleTypeConfigInfoListByPid(pid);
        }

        public bool DeleteProductVehicleTypeConfigByParams(List<ProductVehicleTypeConfigDb> deleteList)
        {
            return productHandler.DeleteProductVehicleTypeConfigByParams(deleteList);
        }

        public VehicleInfoExDb GetVehicleInfoExByTid(string tid)
        {
            return vehicleHandler.GetVehicleInfoExByTid(tid);
        }

        public List<VehicleInfoExDb> GetVehicleInfoExByVehicleId(string vehicleId)
        {
            return vehicleHandler.GetVehicleInfoExByVehicleId(vehicleId);
        }

        public List<VehicleInfoExDb> GetVehicleInfoExByPid(string pid, string level)
        {
            return vehicleHandler.GetVehicleInfoExByPid(pid, level);
        }

        public List<VehicleInfoExDb> GetVehicleInfoEx()
        {
            return vehicleHandler.GetVehicleInfoEx();
        }

        public VehicleBrandCategoryAndType GetExistVehicleBrandCategoryAndVehicleType()
        {
            var result = new VehicleBrandCategoryAndType()
            {
                BrandCategoryList = new List<string>(),
                VehicleTypeList = new List<string>()
            };
            result.BrandCategoryList = vehicleHandler.GetVehicleBrandCategory();
            result.VehicleTypeList = vehicleHandler.GetVehicleBodyType();
            return result;
        }


        #endregion

        public void WriteOperatorLog(ProductVehicleTypeConfigOpLog log)
        {
            //User.Identity.Name, "", id, operation
            LoggerManager.InsertOpLogForProductVehicleTypeConfig(log);
        }

        public void BulkSaveOperateLogInfo(List<ProductVehicleTypeConfigOpLog> tbList)
        {
            LoggerManager.BulkSaveOperateLogInfo(tbList);
        }

        public List<ProductVehicleTypeConfigOpLog> GetAllLogByTime(string timeS, string timeE)
        {
            return LoggerManager.GetAllLogByTime(timeS, timeE);
        }

        public List<ProductVehicleTypeConfigOpLog> GetAllLogByPid(string pid)
        {
            return LoggerManager.GetAllLogByPid(pid);
        }

        public List<ProductVehicleTypeConfigOpLog> GetAllLog()
        {
            return LoggerManager.GetAllLog();
        }
    }
}
