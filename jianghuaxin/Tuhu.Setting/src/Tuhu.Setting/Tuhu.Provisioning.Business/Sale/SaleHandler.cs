using System.Collections.Specialized;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.Sale
{
    public class SaleHandler
    {
        #region Private Fields

        private readonly IConnectionManager connectionManager;
        private readonly IDBScopeManager dbManager;
        #endregion

        #region Ctor
        public SaleHandler(IDBScopeManager dbManager, IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            this.dbManager = dbManager;
        }
        #endregion


        public int Delete_act_saleproduct(int id, SqlSchema smSaleProd)
        {
            return DALSale.Delete_act_saleproduct(id, smSaleProd);
        }

        public bool Get_act_saleproduct_repeat(SqlSchema smSaleProd, NameValueCollection form)
        {
            return DALSale.Get_act_saleproduct_repeat(smSaleProd, form);
        }

        public int Insert_act_saleproduct(SqlSchema smSaleProd, NameValueCollection form)
        {
            return DALSale.Insert_act_saleproduct(smSaleProd, form);
        }

        public bool Get_act_saleproduct_mid(SqlSchema smSaleProd, NameValueCollection form)
        {
            return DALSale.Get_act_saleproduct_mid(smSaleProd, form);
        }

        public int Update_act_saleproduct(SqlSchema smSaleProd, NameValueCollection form)
        {
            return DALSale.Update_act_saleproduct(smSaleProd, form);
        }
        public DyModel Get_act_salemodule(SqlSchema smSaleModule, int parentid)
        {
            return DALSale.Get_act_salemodule(smSaleModule, parentid);
        }

        public DyModel Get_act_salemodule()
        {
            return DALSale.Get_act_salemodule();
        }

        public DyModel Get_act_saleproduct(string od, int mid)
        {
            return DALSale.Get_act_saleproduct(od, mid);
        }
    }
}
