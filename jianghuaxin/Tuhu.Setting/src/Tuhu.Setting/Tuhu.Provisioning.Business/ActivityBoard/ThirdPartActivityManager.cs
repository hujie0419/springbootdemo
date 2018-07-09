using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.DataAccess.DAO.ActivityBoard;
using Tuhu.Provisioning.DataAccess.Entity.ActivityBoard;
using Tuhu.Provisioning.DataAccess.Entity.CommonServices;

namespace Tuhu.Provisioning.Business.ActivityBoard
{
    public class ThirdPartActivityManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;

        private static readonly ILog logger = LoggerFactory.GetLogger("ThirdPartActivityManager");
        private static string LogType = ActivityBoardModuleType.ThirdPartyActivity.ToString();

        public ThirdPartActivityManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }

        /// <summary>
        /// 获取第三方活动
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="activityType"></param>
        /// <returns></returns>
        public List<ThirdPartActivity> GetThirdPartActivity(int pageIndex, int pageSize, DateTime? startTime, DateTime? endTime, int activityType, string channel)
        {
            List<ThirdPartActivity> result = new List<ThirdPartActivity>();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALThirdPartActivity.SelectThirdPartActivity(conn, pageIndex, pageSize, startTime, endTime, activityType, channel));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetThirdPartActivity");
            }

            return result;
        }

        /// <summary>
        /// 根据PKID获取活动信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ThirdPartActivity GetThirdPartActivityByPKID(int pkid)
        {
            ThirdPartActivity result = null;

            try
            {
                result = dbScopeReadManager.Execute(conn => DALThirdPartActivity.SelectThirdPartActivityByPKID(conn, pkid));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetThirdPartActivityByPKID");
            }

            return result;
        }

        /// <summary>
        /// 添加活动信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool InsertThirdPartActivity(ThirdPartActivity data, string user)
        {
            var result = 0;

            try
            {
                if (data != null)
                {
                    dbScopeManager.Execute(conn =>
                    {
                        result = DALThirdPartActivity.InsertThirdPartActivity(conn, data);
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "InsertThirdPartActivity");
            }

            if (result > 0)
            {
                ActivityBoardManager.InsertLog("InsertThirdPartActivity", result.ToString(),
                    $"Data:{JsonConvert.SerializeObject(data)}", result > 0 ? "添加成功" : "添加失败", user, LogType);
                CallCRMService.NewAddActivity(data.ActivityName, data.StartTime, data.EndTime, data.WebUrl, data.ActivityRules, result.ToString(), CRMSourceType.ActivityBoaardThirdParty, user);
            }

            return result > 0;
        }

        /// <summary>
        /// 更新活动信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateThirdPartActivity(ThirdPartActivity data, string user)
        {
            var result = false;

            try
            {
                if (data != null)
                {
                    result = dbScopeManager.Execute(conn => DALThirdPartActivity.UpdateThirdPartActivity(conn, data)) > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "UpdateThirdPartActivity");
            }

            if (result)
            {
                ActivityBoardManager.InsertLog("UpdateThirdPartActivity", data.PKID.ToString(),
                    $"Data:{JsonConvert.SerializeObject(data)}", result ? "修改成功" : "修改失败", user, LogType);
                CallCRMService.NewUpdateActivity(data.ActivityName, data.StartTime, data.EndTime, data.WebUrl, data.ActivityRules, data.PKID.ToString(), CRMSourceType.ActivityBoaardThirdParty, user);
            }

            return result;
        }

        /// <summary>
        /// 删除活动信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteActivityByPKID(int pkid, string user)
        {
            var result = false;

            try
            {
                result = dbScopeManager.Execute(conn => DALThirdPartActivity.DeleteActivityByPKID(conn, pkid)) > 0;
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DeleteActivityByPKID");
            }
            if (result)
            {
                ActivityBoardManager.InsertLog("DeleteActivityByPKID", pkid.ToString(), $"删除第三方活动", result ? "删除成功" : "删除失败", user, LogType);
                CallCRMService.NewDeleteActivityBySourceId(pkid.ToString(), CRMSourceType.ActivityBoaardThirdParty, user);
            }
            return result;
        }

        /// <summary>
        /// 根据岂止时间获取活动信息
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<ThirdPartActivity> GetActivityForActivityBoard(DateTime startTime, DateTime endTime, string title, string owner, string channel)
        {
            List<ThirdPartActivity> result = new List<ThirdPartActivity>();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALThirdPartActivity.SelectActivityForActivityBoard(conn, startTime, endTime, title, owner, channel));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetActivityForActivityBoard");
            }

            return result;
        }
    }
}
