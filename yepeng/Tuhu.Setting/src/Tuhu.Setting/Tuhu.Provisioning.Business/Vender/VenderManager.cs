using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO.Vender;
using Tuhu.Provisioning.DataAccess.Entity.Vender;

namespace Tuhu.Provisioning.Business.Vender
{
    public class VenderManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;
        private readonly IDBScopeManager dbTuhuLogScopeReadManager = null;

        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(VenderManager));
        //private static String VIOLATION_URL = $"{ConfigurationManager.AppSettings["CheXinYiHost"]}gateway.aspx";

        public VenderManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }

        public List<VenderModel> SelectVenderInfoByVendorType(string vendorType)
        {
            List<VenderModel> result = new List<VenderModel>();
            try
            {
                if (!string.IsNullOrEmpty(vendorType))
                    result = dbScopeReadManager.Execute(conn => DALVender.SelectVenderInfoByVendorType(conn, vendorType));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
    }
}
