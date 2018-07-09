using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business
{
    public class ShopSaleItemForGrouponBLL
    {
        public static IEnumerable<ShopSaleItemForGrouponModel> SelectPages(int pageIndex = 1, int pageSize = 20, string strWhere = "")
        {
            try
            {
                return ShopSaleItemForGrouponDAL.SelectPages(ProcessConnection.OpenTuhu_GrouponReadOnly, pageIndex, pageSize, strWhere);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static IEnumerable<ShopSaleItemForGrouponModel> SelectPagesNew(
            int? pageIndex,
            int? pageSize,
            string shopName,
            string province,
            string city,
            string area,
            int shopType,
            string category,
            string proName,
            int sales,
            int isActive,
            out int count
            )
        {
            try
            {
                count = 0;
                return ShopSaleItemForGrouponDAL.SelectPagesNew(
                    shopName,
                    province,
                   city,
                    area,
                   shopType,
                    category,
                    proName,
                    sales,
                    isActive,
                  out count,
                   pageIndex ?? 1,
                    pageSize ?? 10);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static ShopSaleItemForGrouponModel GetEntity(int PKID)
        {
            try
            {
                return ShopSaleItemForGrouponDAL.GetEntity(ProcessConnection.OpenTuhu_GrouponReadOnly, PKID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Insert(ShopSaleItemForGrouponModel model)
        {
            try
            {
                return ShopSaleItemForGrouponDAL.Insert(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(ShopSaleItemForGrouponModel model)
        {
            try
            {
                return ShopSaleItemForGrouponDAL.Update(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Delete(int PKID)
        {
            try
            {
                return ShopSaleItemForGrouponDAL.Delete(ProcessConnection.OpenTuhu_Groupon, PKID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
