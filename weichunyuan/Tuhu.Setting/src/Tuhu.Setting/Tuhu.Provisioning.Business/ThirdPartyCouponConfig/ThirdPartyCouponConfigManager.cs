using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ThirdPartyCouponConfig
{
    public class ThirdPartyCouponConfigManager
    {
        #region Private Fields
        static readonly string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);

        private static readonly ILog Logger = LoggerFactory.GetLogger("ThirdPartyCouponConfig");

        private readonly ThirdPartyCouponConfigHandler handler = null;

        public ThirdPartyCouponConfigManager()
        {
            handler = new ThirdPartyCouponConfigHandler(DbScopeManager);
        }
        #endregion
        public static IEnumerable<ThirdPartyCouponConfigModel> SelecThirdPartyCouponConfigModels(int pageNum, int pageSize)
        {
            return DalThirdPartyCouponConfig.SelectThirdPartyCouponConfigModels(pageNum, pageSize);
        }

        public static int SelectThirdPartyCouponConfigsByChannelAndPatch(string thirdPartyChannel, string thirdPartyCouponPatch)
        {
            return DalThirdPartyCouponConfig.SelectThirdPartyCouponConfigsByChannelAndPatch(thirdPartyChannel, thirdPartyCouponPatch);
        }
        public static ThirdPartyCouponConfigModel SelecThirdPartyCouponConfigModelById(int id)
        {
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return DalThirdPartyCouponConfig.SelectThirdPartyCouponConfigModelById(c,id);
            }
        }

        public static bool DeleteThirdPartyCouponConfig(int id)
        {
            using (var c = ProcessConnection.OpenGungnir)
            {
                return DalThirdPartyCouponConfig.DeleteThirdPartyCouponConfig(c, id);
            }
        }

        public  int UpdateThirdPartyCouponConfig(ThirdPartyCouponConfigModel model)
        {
            try
            {
                return handler.UpdateThirdPartyCouponConfig(model);
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyCouponConfigException(0, "修改第三方优惠券配置失败！", ex);
                Logger.Log(Level.Error, exception, "Error in UpdateThirdPartyCouponConfig.");
                throw exception;
            }
        }

        public static GetPCodeModel SelectGetCouponRulesByGetRuleId(Guid getruleId)
        {
            using (var c = ProcessConnection.OpenGungnir)
            {
                return DalThirdPartyCouponConfig.SelectGetCouponRulesByGetRuleId(c, getruleId);
            }
        }

        public  int InsertThirdPartyCouponConfig(ThirdPartyCouponConfigModel model)
        {
            try
            {
                return handler.InsertThirdPartyCouponConfig(model);

            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyCouponConfigException(0, "新增第三方优惠券配置失败！", ex);
                Logger.Log(Level.Error, exception, "Error in InsertThirdPartyCouponConfig.");
                throw exception;
            }

        }
    }
}
