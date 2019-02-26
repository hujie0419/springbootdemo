using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ActivityCalendar
{
    public class ActivityCalendarManager : IActivityCalendarManager
    {
        #region Private Fields

        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(ConnectionManager);
        private static readonly ILog Logger = LoggerFactory.GetLogger("ActivityCalendar");


        private readonly ActivityCalendarHandler _handler = null;

        #endregion

        public ActivityCalendarManager()
        {
            _handler = new ActivityCalendarHandler(DbScopeManager);
        }

        public void AddActivityCalendar(DataAccess.Entity.ActivityCalendar obj)
        {
            _handler.AddActivityCalendar(obj);
        }
        //获取套餐配置的数据

        public void GetDataForActivityCalendar(PromotionConfigures obj)
        {

        }

        public void HandlerGetDataForActivityCalendar(object obj)
        {

        }

        public List<DataAccess.Entity.ActivityCalendar> SelectActivityByCondition(string sqlWhere)
        {

            return _handler.SelectActivityByCondition(sqlWhere);

        }

        public void UpdateIsActivity(int datafromId, string dataFrom, bool status)
        {
            try
            {

                _handler.UpdateIsActivity(datafromId, dataFrom, status);
            }
            catch (TuhuBizException)
            {
                throw;
            }

        }

        public void UpdateIsActivity(int datafromId, string dataFrom, bool status,
            DateTime lastUpdatetime)
        {
            try
            {

                _handler.UpdateIsActivity(datafromId, dataFrom, status, lastUpdatetime);
            }
            catch (TuhuBizException)
            {
                throw;
            }

        }
        public List<ActivitySchedule> SelectActivitySchedules()
        {
            try
            {
                return _handler.SelectActivitySchedules();
            }
            catch (TuhuBizException)
            {
                throw;
            }

        }

        public List<DataAccess.Entity.ActivityCalendar> SelectActivityDetailData(string title, string date)
        {

            return _handler.SelectActivityDetailData(title, date);


        }


        public List<DataAccess.Entity.ActivityCalendar> SelectCurrentDayActivity()
        {

            return _handler.SelectCurrentDayActivity();

        }

    }
}