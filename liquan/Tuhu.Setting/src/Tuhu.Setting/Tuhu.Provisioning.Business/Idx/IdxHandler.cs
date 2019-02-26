using System.Collections.Specialized;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.Idx
{
    public class IdxHandler
    {
        private readonly IConnectionManager connectionManager;
        private readonly IDBScopeManager dbManager;

        public IdxHandler(IDBScopeManager dbManager, IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            this.dbManager = dbManager;
        }

        public bool Insert_tal_newappsetdata(SqlSchema smAppModel, NameValueCollection form)
        {
            return DALIdx.Insert_tal_newappsetdata(smAppModel, form);
        }

        public bool Update_tal_newappsetdata(SqlSchema smAppModel, NameValueCollection form)
        {
            return DALIdx.Update_tal_newappsetdata(smAppModel, form);
        }

        public DyModel Get_tal_newappsetdata(SqlSchema smAppModel)
        {
            return DALIdx.Get_tal_newappsetdata(smAppModel);
        }

        public DyModel Get_tbl_AdProduct(SqlSchema smp, string mid)
        {
            return DALIdx.Get_tbl_AdProduct(smp, mid);
        }
        public bool Delete_tal_newappsetdata(SqlSchema smAppModel, string id)
        {
            return DALIdx.Delete_tal_newappsetdata(smAppModel, id);
        }

        public bool Delete_tbl_AdProduct(SqlSchema smp, string mid, string id)
        {
            return DALIdx.Delete_tbl_AdProduct(smp, mid, id);
        }

        public  bool Insert_tbl_AdProduct(SqlSchema smp, NameValueCollection form)
        {
            return DALIdx.Insert_tbl_AdProduct(smp, form);
        }

        public  bool Update_tbl_AdProduct(SqlSchema smp, NameValueCollection form, string id)
        {
            return DALIdx.Update_tbl_AdProduct(smp, form, id);
        }
    }
}
