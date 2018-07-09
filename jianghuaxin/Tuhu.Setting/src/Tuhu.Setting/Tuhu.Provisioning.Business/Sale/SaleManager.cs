using System;
using System.Collections.Specialized;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.Business.Sale
{
    public class SaleManager
    {
        #region Private Fields

        private static readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(ConnectionManager);

        private readonly SaleHandler handler;

        private static readonly ILog Logger = LoggerFactory.GetLogger("Sale");

        #endregion

        public SaleManager()
        {
            handler = new SaleHandler(DbScopeManager, ConnectionManager);
        }

        public int Delete_act_saleproduct(int id, SqlSchema smSaleProd)
        {

            try
            {
                return handler.Delete_act_saleproduct(id, smSaleProd);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new SaleException(1, "Delete_act_saleproduct", ex);
                Logger.Log(Level.Error, "Delete_act_saleproduct", exception);
                throw ex;
            }
        }

        public bool Get_act_saleproduct_repeat(SqlSchema smSaleProd, NameValueCollection form)
        {
            try
            {
                return handler.Get_act_saleproduct_repeat(smSaleProd, form);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new SaleException(1, "Get_act_saleproduct_repeat", ex);
                Logger.Log(Level.Error, "Get_act_saleproduct_repeat", exception);
                throw ex;
            }
        }

        public int Insert_act_saleproduct(SqlSchema smSaleProd, NameValueCollection form)
        {
            try
            {
                return handler.Insert_act_saleproduct(smSaleProd, form);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new SaleException(1, "Insert_act_saleproduct", ex);
                Logger.Log(Level.Error, "Insert_act_saleproduct", exception);
                throw ex;
            }
        }
        public bool Get_act_saleproduct_mid(SqlSchema smSaleProd, NameValueCollection form)
        {
            try
            {
                return handler.Get_act_saleproduct_mid(smSaleProd, form);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new SaleException(1, "Get_act_saleproduct_mid", ex);
                Logger.Log(Level.Error, "Get_act_saleproduct_mid", exception);
                throw ex;
            }

        }

        public int Update_act_saleproduct(SqlSchema smSaleProd, NameValueCollection form)
        {
            try
            {
                return handler.Update_act_saleproduct(smSaleProd, form);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new SaleException(1, "Update_act_saleproduct", ex);
                Logger.Log(Level.Error, "Update_act_saleproduct", exception);
                throw ex;
            }
        }

        public DyModel Get_act_salemodule(SqlSchema smSaleModule, int parentid)
        {
            try
            {
                return handler.Get_act_salemodule(smSaleModule, parentid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new SaleException(1, "Get_act_salemodule(SqlSchema smSaleModule, int parentid) ", ex);
                Logger.Log(Level.Error, "Get_act_salemodule(SqlSchema smSaleModule, int parentid)", exception);
                throw ex;
            }
        }
        public DyModel Get_act_salemodule()
        {
            try
            {
                return handler.Get_act_salemodule();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new SaleException(1, "Get_act_salemodule", ex);
                Logger.Log(Level.Error, "Get_act_salemodule", exception);
                throw ex;
            }
        }
        public DyModel Get_act_saleproduct(string od, int mid)
        {
            try
            {
                return handler.Get_act_saleproduct(od, mid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new SaleException(1, "Get_act_saleproduct(string od, int mid)", ex);
                Logger.Log(Level.Error, "Get_act_saleproduct(string od, int mid)", exception);
                throw ex;
            }
        }
    }
}
