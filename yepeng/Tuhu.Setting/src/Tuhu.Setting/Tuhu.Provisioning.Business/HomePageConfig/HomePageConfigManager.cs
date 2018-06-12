using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.HomePageConfig
{
    public class HomePageConfigManager
    {
        #region Private Fields

        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);

        #region ReadOnly
        static string strConnReadOnly = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static readonly IConnectionManager connectionManagerReadOnly = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConnReadOnly) ? SecurityHelp.DecryptAES(strConnReadOnly) : strConnReadOnly);
        private static readonly IDBScopeManager DbScopeManagerReadOnly = new DBScopeManager(connectionManagerReadOnly);
        #endregion

        private static readonly ILog logger = LoggerFactory.GetLogger("HomePageConfigManager");
        private HomePageConfigHandler handler = null;
        private HomePageConfigHandler handlerReadOnly = null;

        #endregion 

        public HomePageConfigManager()
        {
            handler = new HomePageConfigHandler(DbScopeManager);
            handlerReadOnly = new HomePageConfigHandler(DbScopeManagerReadOnly);
        }

        /// <summary>
        /// 根据ID获取配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<tal_newappsetdata_v2> GetTal_newappsetdata_v2ById(int id)
        {
            return handlerReadOnly.GetTal_newappsetdata_v2ById(id);
        }

        /// <summary>
        /// 根据parentid获取配置信息
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public List<tal_newappsetdata_v2> GetTal_newappsetdata_v2ByParentId(int parentid)
        {
            return handlerReadOnly.GetTal_newappsetdata_v2ByParentId(parentid);
        }

        /// <summary>
        /// 获取全部配置信息
        /// </summary>
        /// <returns></returns>
        public List<tal_newappsetdata_v2> GetALLTal_newappsetdata_v2()
        {
            return handlerReadOnly.GetALLTal_newappsetdata_v2();
        }

        /// <summary>
        /// 修改大渠道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateTal_newappsetdata_v2(tal_newappsetdata_v2 model)
        {
            return handler.UpdateTal_newappsetdata_v2(model);
        }

        /// <summary>
        /// 添加大渠道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddTal_newappsetdata_v2(tal_newappsetdata_v2 model)
        {
            return handler.AddTal_newappsetdata_v2(model);
        }

        /// <summary>
        /// 添加区域
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddChidAreaData(tal_newappsetdata_v2 model)
        {
            return handler.AddChidAreaData(model);
        }

        /// <summary>
        /// 修改区域
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateChidAreaData(tal_newappsetdata_v2 model)
        {
            return handler.UpdateChidAreaData(model);
        }

        public int CopyNewAppSetData(int id, int newid)
        {
            return handler.CopyNewAppSetData(id, newid);
        }
        public int AnimationSave(HomePagePopupAnimation model)
        {
            return handler.AnimationSave(model);
        }
        public bool AnimationUpdate(HomePagePopupAnimation model)
        {
            return handler.AnimationUpdate(model);
        }
        public int AnimationDelete(int PKID)
        {
            return handler.AnimationDelete(PKID);
        }

        public List<CouponsInPopup> SelectCouponsOnAnimaId(int animaId)
        {
            return handler.SelectCouponsOnAnimaId(animaId);
        }

        public int InsertCouponInPopup(CouponsInPopup model)
        {
            return handler.InsertCouponInPopup(model);
        }

        public int UpdateCouponInPopup(CouponsInPopup model)
        {
            return handler.UpdateCouponInPopup(model);
        }

        public int DeleteCouponInPopup(int PKId)
        {
            return handler.DeleteCouponInPopup(PKId);
        }
    }
}
