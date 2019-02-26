using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json.Linq;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;

namespace Tuhu.Provisioning.Business.Activity
{
    public class StarRatingStoreManage
    {
        private static readonly Common.Logging.ILog Logger = Common.Logging.LogManager.GetLogger(typeof(ActivityManager));

        #region 工厂店投放

        /// <summary>
        /// 工厂店投放-获取某个时间段的工厂店列表
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<StarRatingStoreModel> GetStarRatingStoreList(out int recordCount, string startTime, string endTime, int pageSize = 10, int pageIndex = 1)
        {
            try
            {
                return DataAccess.DAO.DALStarRatingStore.GetStarRatingStoreList(out recordCount, startTime, endTime, pageSize, pageIndex);
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "GetStarRatingStoreList", ex);
                Logger.Error("GetStarRatingStoreList", exception);
                throw ex;
            }
        }

        /// <summary>
        /// 工厂店投放-查看详情
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public StarRatingStoreModel GetStarRatingStoreModel(int PKID)
        {
            try
            {
                return DataAccess.DAO.DALStarRatingStore.GetStarRatingStoreModel(PKID);
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "GetStarRatingStoreModel", ex);
                Logger.Error("GetStarRatingStoreModel", exception);
                throw ex;
            }
        }

        /// <summary>
        /// 工厂店投放-excel导出
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<StarRatingStoreModel> GetStarList(string startTime, string endTime)
        {
            try
            {
                return DataAccess.DAO.DALStarRatingStore.GetStarList(startTime, endTime);
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "GetStarList", ex);
                Logger.Error("GetStarList", exception);
                throw ex;
            }
        }
        #endregion
    }
}
