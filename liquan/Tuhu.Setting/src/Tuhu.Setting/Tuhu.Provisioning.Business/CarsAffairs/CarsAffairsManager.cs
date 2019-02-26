using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO.CarsAffairs;
using Tuhu.Provisioning.DataAccess.Entity.CarsAffairs;

namespace Tuhu.Provisioning.Business.CarsAffairs
{
    public class CarsAffairsManager
    {
        private static readonly IConnectionManager tuhuLogConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString);
        private readonly IDBScopeManager dbScopeManagerTuhuLog = null;

        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(CarsAffairsManager));

        public CarsAffairsManager()
        {
            dbScopeManagerTuhuLog = new DBScopeManager(tuhuLogConnectionManager);
        }

        /// <summary>
        /// 获取车务日志
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="orderType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<CarsAffairsLog> SelectCarsAffairs(DateTime startTime, DateTime endTime, string orderType, int pageIndex, int pageSize)
        {
            List<CarsAffairsLog> result = new List<CarsAffairsLog>();
            try
            {
                result = dbScopeManagerTuhuLog.Execute(conn => DalCarsAffairs.GetCarsAffairs(conn, startTime, endTime, orderType, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
    }
}
