using System;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.Business.Vin
{
    public class VinManager
    {
        #region Private Fields

        private static readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(ConnectionManager);

        private readonly VinHandler handler;

        private static readonly ILog Logger = LoggerFactory.GetLogger("Vin");

        #endregion

        public VinManager()
        {
            handler = new VinHandler(DbScopeManager, ConnectionManager);
        }


        public int DeleteVin_recordById(string id)
        {
            try
            {
                return handler.DeleteVin_recordById(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VinException(0, "DeleteVin_recordById", ex);
                Logger.Log(Level.Error, exception, "DeleteVin_recordById");
                throw exception;
            }
        }

        public int UpdateVin_record(string phone, string vin, string id)
        {
            try
            {
                return handler.UpdateVin_record(phone, vin, id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VinException(0, "UpdateVin_record", ex);
                Logger.Log(Level.Error, exception, "UpdateVin_record");
                throw exception;
            }
        }

        public bool IsRepeatVin_record(string phone, string vin)
        {
            try
            {
                return handler.IsRepeatVin_record(phone, vin);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VinException(0, "IsRepeatVin_record", ex);
                Logger.Log(Level.Error, exception, "IsRepeatVin_record");
                throw exception;
            }

        }

        public bool InsertVin_record(string id, string phone, string vin, string u)
        {
            try
            {
                return handler.InsertVin_record(id, phone, vin, u);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VinException(0, "InsertVin_record", ex);
                Logger.Log(Level.Error, exception, "InsertVin_record");
                throw exception;
            }
        }

        public DyModel GetVin_record(int ps, int pe)
        {
            try
            {
                return handler.GetVin_record(ps, pe);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VinException(0, "GetVin_record", ex);
                Logger.Log(Level.Error, exception, "GetVin_record");
                throw exception;
            }

        }

        public bool GetVIN_REGION()
        {
            try
            {
                return handler.GetVIN_REGION();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VinException(0, "GetVIN_REGION", ex);
                Logger.Log(Level.Error, exception, "GetVIN_REGION");
                throw exception;
            }
        }
        public DyModel GetVIN_REGIONDyModel()
        {
            try
            {
                return handler.GetVIN_REGIONDyModel();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VinException(0, "GetVIN_REGIONDyModel", ex);
                Logger.Log(Level.Error, exception, "GetVIN_REGIONDyModel");
                throw exception;
            }

        }

        public void Operate(int isEnable, int id, string region)
        {
            try
            {
                handler.Operate(isEnable, id, region);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VinException(0, "Operate", ex);
                Logger.Log(Level.Error, exception, "Operate");
                throw exception;
            }
        }

        public void InsertVIN_REGION(string region)
        {
            try
            {
                handler.InsertVIN_REGION(region);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VinException(0, "InsertVIN_REGION", ex);
                Logger.Log(Level.Error, exception, "InsertVIN_REGION");
                throw exception;
            }
        }
    }
}
