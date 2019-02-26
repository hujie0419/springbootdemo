using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO.BeautyServicePackageDal;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;

namespace Tuhu.Provisioning.Business.BeautyService
{
    public class LimitConfigManager
    {
        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(typeof(LimitConfigManager));

        public BeautyServicePackageLimitConfig config;
        public LimitConfigManager (BeautyServicePackageLimitConfig config)
        {
            this.config = config;
        }
        /// <summary>
        /// 获取限购配置
        /// </summary>
        /// <param name="packageDetailId"></param>
        /// <returns></returns>
        public BeautyServicePackageLimitConfig GetBeautyServicePackageLimitConfig()
        {
            BeautyServicePackageLimitConfig result = null;

            try
            {
                result = BeautyServicePackageDal.SelectBeautyServicePackageLimitConfigByPackageDetailId(config.PackageDetailId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return result;
        }
        /// <summary>
        /// 新增限购配置
        /// </summary>
        /// <returns></returns>
        public bool InsertBeautyServicePackageLimitConfig()
        {
            var result = false;

            try
            {
                result = BeautyServicePackageDal.InsertBeautyServicePackageLimitConfig(config);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return result;
        }
        /// <summary>
        /// 更新限购配置
        /// </summary>
        /// <returns></returns>
        public bool UpdateBeautyServicePackageLimitConfig()
        {
            var result = false;

            try
            {
                result = BeautyServicePackageDal.UpdateBeautyServicePackageLimitConfig(config);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
