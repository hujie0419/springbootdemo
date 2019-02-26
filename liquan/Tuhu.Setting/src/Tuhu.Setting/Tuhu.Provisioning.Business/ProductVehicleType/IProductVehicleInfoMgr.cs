using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ProductVehicleType
{
    public interface IProductVehicleInfoMgr
    {
        List<ProductInfo> SearchProductInfoByParam(string input, int pageIndex, int pageSize, int type);

        ProductInfo GetProductInfoByPid(string pid);

        List<ProductInfo> GetProductInfoByPids(string pids);

        bool SaveProductInfoByPid(string pid, string cpremark, bool isAuto,string vehicleLevel);

        List<VehicleTypeInfoDb> GetVehicleTypeInfoByCharacter(string alphabet);

        bool ImportVehicleInfoByPid(string pid, DataTable table, string fileName, string cpremark);

        bool InsertProductVehicleTypeConfigInfoByPid(string pid, string destPids, string remark,string vehicleLevel);

        List<VehicleTypeInfoDb> GetVehicleTypeInfoByBrandName(string brandNames);

        bool UpdateVehicleInfoByPid(string pid, DataTable table, string level, string cpremark);

        void WriteOperatorLog(ProductVehicleTypeConfigOpLog log);

        List<ProductVehicleTypeConfigOpLog> GetAllLogByTime(string timeS, string timeE);

        List<ProductVehicleTypeConfigOpLog> GetAllLogByPid(string pid);

        List<ProductVehicleTypeConfigDb> GetProductVehicleTypeConfigInfoListByPid(string pid);

        VehicleInfoExDb GetVehicleInfoExByTid(string tid);

        List<VehicleInfoExDb> GetVehicleInfoExByVehicleId(string vehicleId);

        List<VehicleInfoExDb> GetVehicleInfoExByPid(string pid, string level);

        bool SaveProductVehicleExcelInfo(ProductVehicleTypeFileInfoDb entity);

        List<ProductVehicleTypeFileInfoDb> GetExcelInfoByPid(string pid);

        List<ProductVehicleTypeConfigOpLog> GetAllLog();

        bool DeleteProductVehicleTypeConfigByParams(List<ProductVehicleTypeConfigDb> deleteList);

        List<VehicleInfoExDb> GetVehicleInfoEx();

        void BulkSaveOperateLogInfo(List<ProductVehicleTypeConfigOpLog> tb);

        VehicleBrandCategoryAndType GetExistVehicleBrandCategoryAndVehicleType();

        List<ProductInfo> GetAllNoImportProduct(int pageIndex, int pageSize);

    }
}
