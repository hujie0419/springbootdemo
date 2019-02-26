using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class PersonalCenterConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("PersonalCenterConfig");

        public List<PersonalCenterConfig> GetPersonalCenterConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALPersonalCenterConfig.GetPersonalCenterConfigList(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PersonalCenterConfigException(1, "GetPersonalCenterConfigList", ex);
                Logger.Log(Level.Error, exception, "GetPersonalCenterConfigList");
                throw ex;
            }
        }

        public PersonalCenterConfig GetPersonalCenterConfig(int id)
        {
            try
            {
                return DALPersonalCenterConfig.GetPersonalCenterConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PersonalCenterConfigException(1, "GetPersonalCenterConfig", ex);
                Logger.Log(Level.Error, exception, "GetPersonalCenterConfig");
                throw ex;
            }
        }

        public bool UpdatePersonalCenterConfig(PersonalCenterConfig model)
        {
            try
            {
                return DALPersonalCenterConfig.UpdatePersonalCenterConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PersonalCenterConfigException(1, "UpdatePersonalCenterConfig", ex);
                Logger.Log(Level.Error, exception, "UpdatePersonalCenterConfig");
                throw ex;
            }

        }

        public bool InsertPersonalCenterConfig(PersonalCenterConfig model)
        {
            try
            {
                return DALPersonalCenterConfig.InsertPersonalCenterConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PersonalCenterConfigException(1, "InsertPersonalCenterConfig", ex);
                Logger.Log(Level.Error, exception, "InsertPersonalCenterConfig");
                throw ex;
            }
        }
        public bool DeletePersonalCenterConfig(int id)
        {
            try
            {
                return DALPersonalCenterConfig.DeletePersonalCenterConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PersonalCenterConfigException(1, "DeletePersonalCenterConfig", ex);
                Logger.Log(Level.Error, exception, "DeletePersonalCenterConfig");
                throw ex;
            }
        }
    }
}
