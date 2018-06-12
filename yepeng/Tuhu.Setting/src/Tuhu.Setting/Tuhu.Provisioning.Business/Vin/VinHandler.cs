using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.Vin
{
    public class VinHandler
    {
        #region Private Fields

        private readonly IConnectionManager connectionManager;
        private readonly IDBScopeManager dbManager;
        #endregion

        #region Ctor
        public VinHandler(IDBScopeManager dbManager, IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            this.dbManager = dbManager;
        }
        #endregion

        public int DeleteVin_recordById(string id)
        {
            return DALVin.DeleteVin_recordById(id);
        }

        public int UpdateVin_record(string phone, string vin, string id)
        {
            return DALVin.UpdateVin_record(phone, vin, id);
        }

        public bool IsRepeatVin_record(string phone, string vin)
        {
            return DALVin.IsRepeatVin_record(phone, vin);
        }

        public bool InsertVin_record(string id, string phone, string vin, string u)
        {
            return DALVin.InsertVin_record(id, phone, vin, u);
        }
        public DyModel GetVin_record(int ps, int pe)
        {
            return DALVin.GetVin_record(ps, pe);
        }

        public bool GetVIN_REGION()
        {
            return DALVin.GetVIN_REGION();
        }

        public DyModel GetVIN_REGIONDyModel()
        {
            return DALVin.GetVIN_REGIONDyModel();
        }

        public void Operate(int isEnable, int id, string region)
        {
            DALVin.Operate(isEnable, id, region);
        }

        public void InsertVIN_REGION(string region)
        {
            DALVin.InsertVIN_REGION(region);
        }
    }
}
