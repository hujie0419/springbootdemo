using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business
{
    public class SE_MDBeautyCategoryConfigBLL
    {
        public static bool Insert(SE_MDBeautyCategoryConfigModel model)
        {
            try
            {
                return SE_MDBeautyCategoryConfigDAL.Insert(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(SE_MDBeautyCategoryConfigModel model)
        {
            try
            {
                return SE_MDBeautyCategoryConfigDAL.Update(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SE_MDBeautyCategoryConfigModel Select(int id)
        {
            try
            {
                return SE_MDBeautyCategoryConfigDAL.Select(ProcessConnection.OpenTuhu_GrouponReadOnlyForDelay, id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<SE_MDBeautyCategoryConfigModel> SelectList()
        {
            try
            {
                return SE_MDBeautyCategoryConfigDAL.SelectList(ProcessConnection.OpenTuhu_GrouponReadOnlyForDelay);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static IEnumerable<SE_MDBeautyCategoryConfigForPartModel> SelectListForPart()
        {
            try
            {
                return SE_MDBeautyCategoryConfigDAL.SelectListForPart(ProcessConnection.OpenTuhu_GrouponReadOnlyForDelay);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static IEnumerable<string> GetPidsFromMDBeautyCategoryProductConfigByCategoryIds(IEnumerable<string> categoryIds)
        {
            try
            {
                return SE_MDBeautyCategoryConfigDAL.GetPidsFromMDBeautyCategoryProductConfigByCategoryIds(ProcessConnection.OpenTuhu_GrouponReadOnlyForDelay, categoryIds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}