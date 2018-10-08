
using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ActivityCalendar
{
    internal class ActivityCalendarHandler
    {
        private readonly IDBScopeManager _dbManager;
        private static readonly ILog Logger = LoggerFactory.GetLogger("Gungnir.ActivityCalendar");
        internal ActivityCalendarHandler(IDBScopeManager dbManager)
        {
            this._dbManager = dbManager;
        }

        public List<DataAccess.Entity.ActivityCalendar> SelectActivityByCondition(string sqlWhere)
        {
            return _dbManager.Execute(conn => DalActivityCalendar.SelectActivityByCondition(conn, sqlWhere));
        }

        public void AddActivityCalendar(DataAccess.Entity.ActivityCalendar obj)
        {
            _dbManager.Execute(conn => DalActivityCalendar.Add(conn, obj));
        }

        public void UpdateIsActivity(int datafromId, string dataFrom, bool status)
        {
            _dbManager.Execute(conn => DalActivityCalendar.UpdateIsActivity(conn, datafromId, dataFrom, status));
        }

        public void UpdateIsActivity(int datafromId, string dataFrom, bool status,
            DateTime lastUpdatetime)
        {
            _dbManager.Execute(conn => DalActivityCalendar.UpdateIsActivity(conn, datafromId, dataFrom, status, lastUpdatetime));
        }

        public List<ActivitySchedule> SelectActivitySchedules()
        {
            return _dbManager.Execute(connection => DalActivitySchedule.SelectActivitySchedule(connection));
        }

        public List<DataAccess.Entity.ActivityCalendar> SelectActivityDetailData(string title, string date)
        {
            return _dbManager.Execute(connection => DalActivitySchedule.SelectActivityDetailData(connection, title, date));
        }

        public List<DataAccess.Entity.ActivityCalendar> SelectCurrentDayActivity()
        {
            return _dbManager.Execute(connection => DalActivitySchedule.SelectCurrentDayActivity(connection));
        }
    }
}
