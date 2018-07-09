using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class RegionActivityPageConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("RegionActivityPageConfig");

        public List<ActivityPageConfig> GetActivityPageConfig(ActivityPageConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALRegionActivityPageConfig.GetActivityPageConfig(model, pageSize, pageIndex, out recordCount);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetActivityPageConfig");
                throw ex;
            }
        }

        public ActivityPageConfig GetActivityPageConfigById(int id)
        {
            try
            {
                return DALRegionActivityPageConfig.GetActivityPageConfigById(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetActivityPageConfigById");
                throw ex;
            }
        }

        public List<ActivityPageUrlConfig> GetActivityPageUrlConfigById(int id)
        {
            try
            {
                return DALRegionActivityPageConfig.GetActivityPageUrlConfigById(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetActivityPageConfigById");
                throw ex;
            }
        }

        public bool UpdateActivityPageConfig(ActivityPageConfig model)
        {
            try
            {
                return DALRegionActivityPageConfig.UpdateActivityPageConfig(model);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdateActivityPageConfig");
                throw ex;
            }

        }

        public bool UpdateActivityPageUrlConfig(ActivityPageUrlConfig model)
        {
            try
            {
                return DALRegionActivityPageConfig.UpdateActivityPageUrlConfig(model);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdateActivityPageUrlConfig");
                throw ex;
            }

        }

        public bool InsertActivityPageConfig(ActivityPageConfig model, ref int id)
        {
            try
            {
                return DALRegionActivityPageConfig.InsertActivityPageConfig(model, ref id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertActivityPageConfig");
                throw ex;
            }
        }

        public bool InsertActivityPageUrlConfig(ActivityPageUrlConfig model, ref int id)
        {
            try
            {
                return DALRegionActivityPageConfig.InsertActivityPageUrlConfig(model, ref id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertActivityPageConfig");
                throw ex;
            }
        }
        public bool DeleteActivityPageUrlConfig(int id)
        {
            try
            {
                return DALRegionActivityPageConfig.DeleteActivityPageUrlConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "DeleteActivityPageConfig");
                throw ex;
            }
        }

        public bool DeleteRegionActivityPageConfig(int id)
        {
            try
            {
                return DALRegionActivityPageConfig.DeleteActivityPageConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "DeleteActivityPageConfig");
                throw ex;
            }
        }

        public bool DeleteActivityPageRegionConfig(int id)
        {
            try
            {
                return DALRegionActivityPageConfig.DeleteActivityPageRegionConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "DeleteActivityPageRegionConfig");
                throw ex;
            }
        }

        public bool InsertActivityPageRegionConfig(ActivityPageRegionConfig model)
        {
            try
            {
                int outid = 0;
                return DALRegionActivityPageConfig.InsertActivityPageRegionConfig(model, ref outid);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertActivityPageRegionConfig");
                throw ex;
            }
        }


        public List<ActivityPageRegionConfig> GetRegionRelationGroup(int id)
        {
            try
            {
                return DALRegionActivityPageConfig.GetRegionRelationGroup(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetRegionRelationGroup");
                throw ex;
            }

        }
        public List<ActivityPageRegionConfig> GetRegionRelation(int id)
        {
            try
            {
                return DALRegionActivityPageConfig.GetRegionRelation(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetRegionRelation");
                throw ex;
            }

        }
        public List<ActivityPageRegionConfig> GetRegionRelationByName(int id, string name)
        {
            try
            {
                return DALRegionActivityPageConfig.GetRegionRelation(id, name);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetRegionRelation");
                throw ex;
            }

        }
    }
}
