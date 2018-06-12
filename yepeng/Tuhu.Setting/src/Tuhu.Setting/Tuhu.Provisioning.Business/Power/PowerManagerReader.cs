using System.Configuration;
using System.Data;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Power
{
    public class PowerManagerReader
    {
        #region private connectionStr
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly ILog logger = LoggerFactory.GetLogger("Power");

        private static readonly IDBScopeManager dbScopeManager = new DBScopeManager(connectionManager);
        private readonly PowerHandle handler;
        #endregion

        public PowerManagerReader()
        {
            handler = new PowerHandle(dbScopeManager);
        }

        public DataSet GetMenuInfo()
        {
            try
            {
                return handler.GetMenuInfo();
            }
            catch (TuhuBizException)
            {
                throw;
            }

        }
    }
}
