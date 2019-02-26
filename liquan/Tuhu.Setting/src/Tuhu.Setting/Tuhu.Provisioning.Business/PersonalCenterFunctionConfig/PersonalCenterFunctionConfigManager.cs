using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class PersonalCenterFunctionConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("PersonalCenterFunctionConfig");

        public List<PersonalCenterFunctionConfig> GetPersonalCenterFunctionConfigList(PersonalCenterFunctionConfig sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALPersonalCenterFunctionConfig.GetPersonalCenterFunctionConfigList(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PersonalCenterFunctionConfigException(1, "GetPersonalCenterFunctionConfigList", ex);
                Logger.Log(Level.Error, exception, "GetPersonalCenterFunctionConfigList");
                throw ex;
            }
        }

        public PersonalCenterFunctionConfig GetPersonalCenterFunctionConfig(int id)
        {
            try
            {
                return DALPersonalCenterFunctionConfig.GetPersonalCenterFunctionConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PersonalCenterFunctionConfigException(1, "GetPersonalCenterFunctionConfig", ex);
                Logger.Log(Level.Error, exception, "GetPersonalCenterFunctionConfig");
                throw ex;
            }
        }

        public bool UpdatePersonalCenterFunctionConfig(PersonalCenterFunctionConfig model)
        {
            try
            {
                return DALPersonalCenterFunctionConfig.UpdatePersonalCenterFunctionConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PersonalCenterFunctionConfigException(1, "UpdatePersonalCenterFunctionConfig", ex);
                Logger.Log(Level.Error, exception, "UpdatePersonalCenterFunctionConfig");
                throw ex;
            }

        }

        public bool InsertPersonalCenterFunctionConfig(PersonalCenterFunctionConfig model)
        {
            try
            {
                return DALPersonalCenterFunctionConfig.InsertPersonalCenterFunctionConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PersonalCenterFunctionConfigException(1, "InsertPersonalCenterFunctionConfig", ex);
                Logger.Log(Level.Error, exception, "InsertPersonalCenterFunctionConfig");
                throw ex;
            }
        }
        public bool DeletePersonalCenterFunctionConfig(int id)
        {
            try
            {
                return DALPersonalCenterFunctionConfig.DeletePersonalCenterFunctionConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PersonalCenterFunctionConfigException(1, "DeletePersonalCenterFunctionConfig", ex);
                Logger.Log(Level.Error, exception, "DeletePersonalCenterFunctionConfig");
                throw ex;
            }
        }
    }
}
