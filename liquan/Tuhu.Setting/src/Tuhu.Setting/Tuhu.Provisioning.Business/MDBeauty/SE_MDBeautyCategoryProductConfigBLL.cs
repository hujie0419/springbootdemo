using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business
{
    public class SE_MDBeautyCategoryProductConfigBLL
    {
        public static IEnumerable<SE_MDBeautyCategoryProductConfigModel> Select(int pageIndex = 1, int pageSize = 20, string categoryIds = "")
        {
            try
            {
                return SE_MDBeautyCategoryProductConfigDAL.Select(ProcessConnection.OpenTuhu_GrouponReadOnlyForDelay, pageIndex, pageSize, categoryIds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SE_MDBeautyCategoryProductConfigModel Select(int id)
        {
            try
            {
                return SE_MDBeautyCategoryProductConfigDAL.Select(ProcessConnection.OpenTuhu_GrouponReadOnlyForDelay, id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Insert(SE_MDBeautyCategoryProductConfigModel model)
        {
            try
            {
                return SE_MDBeautyCategoryProductConfigDAL.Insert(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(SE_MDBeautyCategoryProductConfigModel model)
        {
            try
            {
                return SE_MDBeautyCategoryProductConfigDAL.Update(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量生成&批量修改产品
        /// </summary>
        public static bool BatchInsertOrUpdate(SE_MDBeautyCategoryProductConfigModel model)
        {
            try
            {
                return SE_MDBeautyCategoryProductConfigDAL.BatchInsertOrUpdate(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量插入或修改产品，并同步产品库
        /// </summary>
        public static bool BatchInsertOrUpdateSyncProdcutLibrary(Dictionary<string, string> dicSQL, SE_MDBeautyCategoryProductConfigModel model)
        {
            try
            {
                return SE_MDBeautyCategoryProductConfigDAL.BatchInsertOrUpdateSyncProdcutLibrary(ProcessConnection.OpenTuhu_Groupon, dicSQL, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 自定义SQL查询
        /// </summary>
        public static IEnumerable<T> CustomQuery<T>(string sql)
        {
            try
            {
                return SE_MDBeautyCategoryProductConfigDAL.CustomQuery<T>(ProcessConnection.OpenTuhu_Groupon, sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}