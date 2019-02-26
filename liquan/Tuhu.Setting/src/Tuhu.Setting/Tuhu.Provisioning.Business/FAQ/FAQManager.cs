using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.FAQManagement;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class FAQManager : IFAQManager
    {
        #region Private Fields

        private static readonly IConnectionManager connectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("FAQ");

        private FAQHandler handler = null;

        #endregion

        public FAQManager()
        {
            handler = new FAQHandler(DbScopeManager);
        }
        public List<FAQ> SelectBy(string Orderchannel, string CateOne, string CateTwo, string CateThree, string Question)
        {
            return handler.SelectBy(Orderchannel, CateOne, CateTwo, CateThree, Question);
        }
        public List<FAQ> SelectAll()
        {
            return handler.SelectAll();
        }
        public void Delete(int PKID)
        {
            handler.Delete(PKID);
        }
        public void Add(FAQ fAQ)
        {
            handler.Add(fAQ);
        }
        public void Update(FAQ fAQ)
        {
            handler.Update(fAQ);
        }
        public FAQ GetByPKID(int PKID)
        {
            return handler.GetByPKID(PKID);
        }


        public List<FAQ> TousuFaqSelectBy(string Orderchannel, string CateOne, string CateTwo, string CateThree, string Question)
        {
            return handler.TousuSelectBy(Orderchannel, CateOne, CateTwo, CateThree, Question);
        }

        public List<FAQ> TousuFaqSelectAll()
        {
            return handler.TousuSelectAll();
        }

        public void TousuFaqDelete(int PKID)
        {
            handler.TousuFaqDelete(PKID);
        }

        public void TousuFaqAdd(FAQ fAQ)
        {
            handler.TousuFAQAdd(fAQ);
        }

        public void TousuFaqUpdate(FAQ fAQ)
        {
            handler.TousuFaqUpdate(fAQ);
        }

        public FAQ TousuFaqGetByPKID(int PKID)
        {
            return handler.TousuGetByPKID(PKID);
        }

        /// <summary>
        /// 获取活动信息列表
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public List<ActivityIntroductionModel> GetAllActivityIntroductionList(string activityName, int pageIndex, int pageSize)
        {
            try
            {
                return DalActivityIntroduction.GetAllActivityIntroductionList(activityName, pageIndex, pageSize);
            }
            catch (Exception innerEx)
            {
                throw innerEx;
            }
        }

        /// <summary>
        /// 新增活动信息
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="Type"></param>
        public int AddOrUpActivityIntroduction(ActivityIntroductionModel activity, string Type)
        {
            try
            {
                return DalActivityIntroduction.AddOrUpActivityIntroduction(activity, Type);
            }
            catch (Exception innerEx)
            {
                throw innerEx;
            }
        }

        /// <summary>
        /// 删除活动信息
        /// </summary>
        /// <param name="ID"></param>
        public int DeleteActivityIntroductionById(int ID)
        {
            try
            {
                return DalActivityIntroduction.DeleteActivityIntroductionById(ID);
            }
            catch (Exception innerEx)
            {
                throw innerEx;
            }
        }
        public ActivityIntroductionModel GetActivityIntroductionById(int ID)
        {
            return handler.GetActivityIntroductionById(ID);
        }
    }
}
