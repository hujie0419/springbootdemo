using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.RunSwitch
{
    public class RuntimeSwitchManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("RuntimeSwitch");

        public RuntimeSwitch GetRuntimeSwitch(int id)
        {
            try
            {
                return DALRuntimeSwitch.GetRuntimeSwitch(id);
            }
            catch (Exception ex)
            {
                var exception = new RuntimeSwitchException(1, "GetRuntimeSwitch", ex);
                Logger.Log(Level.Error, exception, "GetRuntimeSwitch");
                throw ex;
            }
        }

        public List<RuntimeSwitch> GetRuntimeSwitch()
        {
            try
            {
                return DALRuntimeSwitch.GetRuntimeSwitch();
            }
            catch (Exception ex)
            {
                var exception = new RuntimeSwitchException(1, "GetRuntimeSwitch", ex);
                Logger.Log(Level.Error, exception, "GetRuntimeSwitch");
                throw ex;
            }
        }

        public bool DeleteRuntimeSwitch(int id)
        {
            try
            {
                return DALRuntimeSwitch.DeleteRuntimeSwitch(id);
            }
            catch (Exception ex)
            {
                var exception = new RuntimeSwitchException(1, "DeleteRuntimeSwitch", ex);
                Logger.Log(Level.Error, exception, "DeleteRuntimeSwitch");
                throw ex;
            }
        }

        public bool UpdateRuntimeSwitch(RuntimeSwitch model)
        {
            try
            {
                return DALRuntimeSwitch.UpdateRuntimeSwitch(model);
            }
            catch (Exception ex)
            {
                var exception = new RuntimeSwitchException(1, "UpdateRuntimeSwitch", ex);
                Logger.Log(Level.Error, exception, "UpdateRuntimeSwitch");
                throw ex;
            }
        }

        public int InsertRuntimeSwitch(RuntimeSwitch model)
        {
            try
            {
                return DALRuntimeSwitch.InsertRuntimeSwitch(model);
            }
            catch (Exception ex)
            {
                var exception = new RuntimeSwitchException(1, "InsertRuntimeSwitch", ex);
                Logger.Log(Level.Error, exception, "InsertRuntimeSwitch");
                throw ex;
            }
        }
    }
}
