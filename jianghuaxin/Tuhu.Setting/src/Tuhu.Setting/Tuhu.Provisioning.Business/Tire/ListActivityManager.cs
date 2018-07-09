using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity.Tire;
using Tuhu.Provisioning.DataAccess.DAO.Tire;
using Tuhu.Provisioning.Business.TiresActivity;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.Tire
{
    public class ListActivityManager
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(TiresActivityManager));
        private static string LogType = "TiresActivity";

        public static IEnumerable<ActivityItem> SelectList(ListActCondition model, PagerModel pager) => DALListActivity.SelectList(model, pager);

        public static int CheckGetRuleGUID(Guid? guid) => DALListActivity.CheckGetRuleGUID(guid);

        public static int SaveBitchAdd(ActivityItem model, out List<Guid> activityIDS) => DALListActivity.SaveBitchAdd(model,out activityIDS);

        public static IEnumerable<ActivityProducts> SelectRelationPIDs(Guid activityID) => DALListActivity.SelectRelationPIDs(activityID);

        public static int DeleteListActivity(Guid activityID) => DALListActivity.DeleteListActivity(activityID);

        public static int BitchOff(string activityIDs) => DALListActivity.BitchOff(activityIDs);

        public static int BitchOn(string activityIDs) => DALListActivity.BitchOn(activityIDs);

        public static ActivityItem GetListActivityByID(Guid activityID)
        {
            var list = DALListActivity.GetListActivityByID(activityID);
            var model = list.FirstOrDefault();
            model.Products = list.Any(C => string.IsNullOrWhiteSpace(C.PID)) ? null : list.OrderBy(C=>C.Postion).Select(C =>
            {
                return new ActivityProducts()
                {
                    PID = C.PID,
                    Postion = C.Postion,
                    DisplayName=C.DisplayName
                };
            });
            model.EndTimeStr = model.EndTime.Value.ToString("yyyy-MM-dd HH:mm");
            model.StartTimeStr = model.StartTime.Value.ToString("yyyy-MM-dd HH:mm");
            return model;
        }

        public static int EditActivity(ActivityItem model) => DALListActivity.EditActivity(model);

        public static int ReplaceListActivityItem(string activityName, string image, string icon,string image2, string buttonText, string activityIDs)
        => DALListActivity.ReplaceListActivityItem(activityName, image, icon,image2, buttonText, activityIDs);

        public static IEnumerable<ActivityItem> SelectActPageOnTireChange(ListActCondition model, PagerModel pager) => DALListActivity.SelectActPageOnTireChange(model, pager); 
        public static ActivityItem FetchTireActivityById(int PKId) => DALListActivity.FetchTireActivityById(PKId);

        public static bool UpdateTireChangedAct(ActivityItem model) => DALListActivity.UpdateTireChangedAct(model);
        public static bool AddTireChangedAct(ActivityItem model) => DALListActivity.AddTireChangedAct(model);
        public static bool DeleteTireChangedAct(int PKId) => DALListActivity.DeleteTireChangedAct(PKId); 
        public static IEnumerable<TireChangedActivityLog> SelectTireChangedActivityLog(string vehicleId, string tireSize) => DALListActivity.SelectTireChangedActivityLog(vehicleId, tireSize);

        public bool AddAndUpdareTireChangeActInBatch(ActivityItem updateModel, List<string> pkids, List<ActivityItem> activityListToAdd)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                try
                {
                    dbHelper.BeginTransaction();
                    if (pkids != null && pkids.Any())
                    {
                        if (!DALListActivity.UpdateTireChangedActInBatch(dbHelper, updateModel, pkids))
                        {
                            dbHelper.Rollback();
                            return false;
                        }
                    }
                    if (activityListToAdd != null && activityListToAdd.Any())
                    {
                        foreach (var actToAdd in activityListToAdd)
                        {
                            if (!DALListActivity.AddTireChangedAct(actToAdd))
                            {
                                dbHelper.Rollback();
                                return false;
                            }
                        }
                    }
                    dbHelper.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    logger.Error("批量生成服务码错误", ex);
                    return false;
                }
            }
        }
    }
}
