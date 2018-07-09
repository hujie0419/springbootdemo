using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business
{
    public class SE_MDBeautyBrandConfigBLL
    {
        public static bool Insert(SE_MDBeautyBrandConfigModel model)
        {
            try
            {
                return SE_MDBeautyBrandConfigDAL.Insert(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool BatchInsert(SE_MDBeautyBrandConfigModel model)
        {
            try
            {
                return SE_MDBeautyBrandConfigDAL.BatchInsert(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(SE_MDBeautyBrandConfigModel model)
        {
            try
            {
                return SE_MDBeautyBrandConfigDAL.Update(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static SE_MDBeautyBrandConfigModel Select(int id)
        {
            try
            {
                return SE_MDBeautyBrandConfigDAL.Select(ProcessConnection.OpenTuhu_GrouponReadOnlyForDelay, id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static IEnumerable<SE_MDBeautyBrandConfigModel> SelectList()
        {
            try
            {
                return SE_MDBeautyBrandConfigDAL.SelectList(ProcessConnection.OpenTuhu_GrouponReadOnlyForDelay);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<SE_MDBeautyBrandConfigModel> SelectListByBrandName(IEnumerable<string> brandName, int id)
        {
            try
            {
                List<string> listParms = new List<string>();
                foreach (var item in brandName)
                {
                    listParms.Add(string.Format("N'{0}'", item));
                }
                return SE_MDBeautyBrandConfigDAL.SelectListByBrandName(ProcessConnection.OpenTuhu_GrouponReadOnlyForDelay, listParms, id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<SE_MDBeautyBrandConfigModel> SelectListForCategoryId(string categoryId)
        {
            try
            {
                return SE_MDBeautyBrandConfigDAL.SelectListForCategoryId(ProcessConnection.OpenTuhu_GrouponReadOnlyForDelay, categoryId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}