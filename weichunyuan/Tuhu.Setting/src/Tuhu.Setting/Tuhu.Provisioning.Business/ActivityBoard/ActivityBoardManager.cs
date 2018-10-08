using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.ActivityBoard;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.ActivityBoard;

namespace Tuhu.Provisioning.Business.ActivityBoard
{
    public class ActivityBoardManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager ConnectionManagerBI =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_BI"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;
        private readonly IDBScopeManager dbScopeManagerBI = null;

        private static readonly ILog logger = LoggerFactory.GetLogger("ActivityBoardManager");

        public ActivityBoardManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
            this.dbScopeManagerBI = new DBScopeManager(ConnectionManagerBI);
        }
        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="method"></param>
        /// <param name="objectId"></param>
        /// <param name="remarks"></param>
        /// <param name="msg"></param>
        /// <param name="opera"></param>
        /// <param name="type"></param>
        public static void InsertLog(string method, string objectId, string remarks, string msg, string opera, string type)
        {
            try
            {
                ActivityBoardLog info = new ActivityBoardLog
                {
                    ObjectId = objectId,
                    Method = method,
                    Message = msg,
                    Remarks = remarks,
                    Operator = opera,
                    Type = type.Trim(),
                    CreatedTime = DateTime.Now
                };
                LoggerManager.InsertLog("ActivityBoardLog", info);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "ConfigLog");
            }
        }

        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<ActivityBoardLog> SelectOperationLog(string objectId, ActivityBoardModuleType type)
        {
            List<ActivityBoardLog> result = new List<ActivityBoardLog>();
            try
            {
                result = DALActivityBoard.SelectOperationLog(objectId, type);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "ConfigLog");
            }
            return result;
        }


        public List<BIActivityPageModel> GetActivityForBI(DateTime start, DateTime end, string activityId)
        {
            List<BIActivityPageModel> result = new List<BIActivityPageModel>();

            try
            {
                if (start != null && end != null)
                {
                    result = dbScopeManagerBI.Execute(conn => DALActivityBoard.SelectActvityForBI(conn, start, end, activityId));
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetActivityForBI");
            }

            return result;
        }

        public ActivityBuild GetActivityForPage(int pkid)
        {
            ActivityBuild result = null;

            try
            {
                result = dbScopeReadManager.Execute(conn => DALActivityBoard.GetActivityForPage(conn, pkid));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetActivityForPage");
            }

            return result;
        }
    }
}
