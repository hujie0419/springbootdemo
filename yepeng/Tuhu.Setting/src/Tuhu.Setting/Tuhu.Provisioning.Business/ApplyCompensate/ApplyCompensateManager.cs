using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class ApplyCompensateManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("ApplyCompensate");

        public List<ApplyCompensate> GetApplyCompensateList(ApplyCompensate model, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALApplyCompensate.GetApplyCompensateList(model, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ApplyCompensateException(1, "GetApplyCompensateList", ex);
                Logger.Log(Level.Error, exception, "GetApplyCompensateList");
                throw ex;
            }
        }

        public List<ApplyCompensate> GetApplyCompensate(ApplyCompensate model)
        {
            try
            {
                return DALApplyCompensate.GetApplyCompensate(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ApplyCompensateException(1, "GetApplyCompensate", ex);
                Logger.Log(Level.Error, exception, "GetApplyCompensate");
                throw ex;
            }
        }
        public ApplyCompensate GetApplyCompensate(int id)
        {
            try
            {
                return DALApplyCompensate.GetApplyCompensate(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ApplyCompensateException(1, "GetApplyCompensateById", ex);
                Logger.Log(Level.Error, exception, "GetApplyCompensateById");
                throw ex;
            }
        }
        public bool UpdateApplyCompensate(ApplyCompensate model)
        {
            try
            {
                return DALApplyCompensate.UpdateApplyCompensate(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ApplyCompensateException(1, "UpdateApplyCompensate", ex);
                Logger.Log(Level.Error, exception, "UpdateApplyCompensate");
                throw ex;
            }
        }
    }
}
