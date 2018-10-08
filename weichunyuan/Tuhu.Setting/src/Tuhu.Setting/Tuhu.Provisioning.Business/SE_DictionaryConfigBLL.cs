using System;
using System.Collections.Generic;
using Common.Logging;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business
{
    /// <summary>
    /// 逻辑处理-SE_DictionaryConfigBLL 
    /// </summary>
    public class SE_DictionaryConfigBLL
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SE_DictionaryConfigBLL));
        public static IEnumerable<SE_DictionaryConfigModel> SelectPages(int pageIndex = 1, int pageSize = 20, string strWhere = "")
        {
            try
            {
                return SE_DictionaryConfigDAL.SelectPages(ProcessConnection.OpenConfigurationReadOnly, pageIndex, pageSize, strWhere);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SE_DictionaryConfigModel GetEntity(int ParentId)
        {
            try
            {
                return SE_DictionaryConfigDAL.GetEntity(ProcessConnection.OpenConfigurationReadOnly, ParentId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Insert(SE_DictionaryConfigModel model)
        {
            try
            {
                return SE_DictionaryConfigDAL.Insert(ProcessConnection.OpenConfiguration, model);
            }
            catch (Exception ex)
            {
                Logger.Info($"赠品插入失败日志=>{ex}-{ex.InnerException}-{ex.StackTrace}");
                throw ex;
            }
        }

        public static bool Update(SE_DictionaryConfigModel model)
        {
            try
            {
                return SE_DictionaryConfigDAL.Update(ProcessConnection.OpenConfiguration, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Delete(int Id)
        {
            try
            {
                return SE_DictionaryConfigDAL.Delete(ProcessConnection.OpenConfiguration, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
