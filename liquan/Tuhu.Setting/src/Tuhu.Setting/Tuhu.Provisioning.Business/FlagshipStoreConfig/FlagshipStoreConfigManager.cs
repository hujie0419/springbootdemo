using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;
using System.Data;

namespace Tuhu.Provisioning.Business
{
  public  class FlagshipStoreConfigManager
    {
        private static DALFlagshipStoreConfig dal = null;
        public FlagshipStoreConfigManager()
        {
            dal = new DALFlagshipStoreConfig();
        }

        public SE_FlagshipStoreConfig GetEntity(string PKID)
        {
            return dal.GetDataRow(PKID).ConvertTo<SE_FlagshipStoreConfig>().FirstOrDefault();
        }

        public IEnumerable<SE_FlagshipStoreConfig> GetList()
        {
            var dt = dal.GetDataTable();
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.ConvertTo<SE_FlagshipStoreConfig>();
        }

        public bool Add(SE_FlagshipStoreConfig model)
        {
            if (model == null)
                return false;
            return dal.Add(model);
        }

        public bool Update(SE_FlagshipStoreConfig model)
        {
            if (model == null || model.PKID == 0)
                return false;
            return dal.Update(model);
        }

        public DataTable GetBrand()
        {
            return dal.GetBrand();
        }


        public bool Delete(string PKID)
        {
            return dal.Delete(PKID);
        }


    }
}
