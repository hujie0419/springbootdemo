using System;
using System.Collections.Specialized;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.Business.Idx
{
    public class IdxManager
    {
        #region Private Fields

        private static readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(ConnectionManager);

        private readonly IdxHandler handler;

        private static readonly ILog Logger = LoggerFactory.GetLogger("Sale");

        #endregion

        public IdxManager()
        {
            handler = new IdxHandler(DbScopeManager, ConnectionManager);
        }

        public bool Insert_tal_newappsetdata(SqlSchema smAppModel, NameValueCollection form)
        {
            try
            {
                return handler.Insert_tal_newappsetdata(smAppModel, form);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new IdxException(1, "Insert_tal_newappsetdata(SqlSchema smAppModel, NameValueCollection form)", ex);
                Logger.Log(Level.Error, "Insert_tal_newappsetdata(SqlSchema smAppModel, NameValueCollection form)", exception);
                throw ex;
            }
        }

        public bool Update_tal_newappsetdata(SqlSchema smAppModel, NameValueCollection form)
        {
            try
            {
                return handler.Update_tal_newappsetdata(smAppModel, form);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new IdxException(1, "Update_tal_newappsetdata(SqlSchema smAppModel, NameValueCollection form)", ex);
                Logger.Log(Level.Error, "Update_tal_newappsetdata(SqlSchema smAppModel, NameValueCollection form)", exception);
                throw ex;
            }
        }

        public DyModel Get_tal_newappsetdata(SqlSchema smAppModel)
        {
            try
            {
                return handler.Get_tal_newappsetdata(smAppModel);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new IdxException(1, "Get_tal_newappsetdata(SqlSchema smAppModel)", ex);
                Logger.Log(Level.Error, "Get_tal_newappsetdata(SqlSchema smAppModel)", exception);
                throw ex;
            }
        }
        public DyModel Get_tbl_AdProduct(SqlSchema smp, string mid)
        {
            try
            {
                return handler.Get_tbl_AdProduct(smp, mid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new IdxException(1, " Get_tbl_AdProduct(SqlSchema smp, string mid)", ex);
                Logger.Log(Level.Error, " Get_tbl_AdProduct(SqlSchema smp, string mid)", exception);
                throw ex;
            }
        }
        public bool Delete_tal_newappsetdata(SqlSchema smAppModel, string id)
        {
            try
            {
                return handler.Delete_tal_newappsetdata(smAppModel, id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new IdxException(1, " Delete_tal_newappsetdata(SqlSchema smAppModel, string id)", ex);
                Logger.Log(Level.Error, " Delete_tal_newappsetdata(SqlSchema smAppModel, string id)", exception);
                throw ex;
            }

        }

        public bool Delete_tbl_AdProduct(SqlSchema smp, string mid, string id)
        {
            try
            {
                return handler.Delete_tbl_AdProduct(smp, mid, id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new IdxException(1, "Delete_tbl_AdProduct(SqlSchema smp, string mid, string id)", ex);
                Logger.Log(Level.Error, "Delete_tbl_AdProduct(SqlSchema smp, string mid, string id)", exception);
                throw ex;
            }
        }

        public bool Insert_tbl_AdProduct(SqlSchema smp, NameValueCollection form)
        {
            try
            {
                return handler.Insert_tbl_AdProduct(smp, form);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new IdxException(1, "Insert_tbl_AdProduct(SqlSchema smp, NameValueCollection form)", ex);
                Logger.Log(Level.Error, "Insert_tbl_AdProduct(SqlSchema smp, NameValueCollection form)", exception);
                throw ex;
            }
        }
        public bool Update_tbl_AdProduct(SqlSchema smp, NameValueCollection form, string id)
        {
            try
            {
                return handler.Update_tbl_AdProduct(smp, form, id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new IdxException(1, "Update_tbl_AdProduct(SqlSchema smp, NameValueCollection form, string id)", ex);
                Logger.Log(Level.Error, "Update_tbl_AdProduct(SqlSchema smp, NameValueCollection form, string id)", exception);
                throw ex;
            }
        }
    }
}
