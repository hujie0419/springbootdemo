using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;

namespace Tuhu.Provisioning.Business.ProductDescModule
{
    public class ProductModuleConfigManager
    {
        public static IEnumerable<ProductDescriptionModel> SelectAllProductConfig(int pageSize, int pageIndex, string pid, int isActive, int isAdvert, string moduleName, out int totalCount)
        {
            var dt = DALProductModuleConfig.SelectAllProductConfig(pageSize, pageIndex, pid, isActive, isAdvert, moduleName, out totalCount);

            if (dt == null || dt.Rows.Count <= 0)
                return new ProductDescriptionModel[0];
            return dt.Rows.Cast<DataRow>().Select(row => new ProductDescriptionModel(row));
        }

        public static IEnumerable<ProductDescriptionModel> GetProductDescModuleDetail(int id)
        {
            var dt = DALProductModuleConfig.GetProductDescModuleDetail(id);
            if (dt == null || dt.Rows.Count <= 0)
                return new ProductDescriptionModel[0];
            return dt.Rows.Cast<DataRow>().Select(row => new ProductDescriptionModel(row));
        }

        public static int InsertProductConfig(ProductDescriptionModel productModuleModel, List<ProductDescriptionModel> platformList,
            List<ProductDescriptionModel> categoryList, List<ProductDescriptionModel> pidList, List<ProductDescriptionModel> brandList,
            string userName, out int moduleID)
        {
            return DALProductModuleConfig.InsertProductConfig(productModuleModel, platformList, categoryList, pidList, brandList, userName, out moduleID);
        }

        public static int UpdateProductConfig(ProductDescriptionModel productModuleModel, List<ProductDescriptionModel> platformList,
            List<ProductDescriptionModel> categoryList, List<ProductDescriptionModel> pidList, List<ProductDescriptionModel> brandList,
            int ParentID, string userName)
        {
            return DALProductModuleConfig.UpdateProductConfig(productModuleModel, platformList, categoryList, pidList, brandList, ParentID, userName);
        }

        public static int DeleteProductAllInfo(int pkid, string userName)
        {
            return DALProductModuleConfig.DeleteProductAllInfo(pkid, userName);
        }

        public static string ImageUploadFile(string pathFormat, byte[] uploadFileBytes, string uploadFileName, short maxWidthHeight)
        {
            var data = string.Empty;
            try
            {
                using (var client = new FileUploadClient())
                {
                    var result = client.UploadImage(new ImageUploadRequest(pathFormat, uploadFileBytes));

                    if (result.Success)
                    {
                        data = result.Result;
                    }
                    result.ThrowIfException(true);
                }
            }
            catch (Exception e)
            {
                //Result.State = UploadState.FileAccessError;
                //Result.ErrorMessage = e.Message;
            }
            finally
            {
                //WriteResult();
            }

            return data;
        }
    }
}
