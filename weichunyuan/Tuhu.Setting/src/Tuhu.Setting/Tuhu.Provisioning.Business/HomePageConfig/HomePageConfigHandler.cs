using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.Business.HomePageConfig
{
    public class HomePageConfigHandler
    {
        private readonly IDBScopeManager dbManager;

        public HomePageConfigHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        /// <summary>
        /// 根据ID获取配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<tal_newappsetdata_v2> GetTal_newappsetdata_v2ById(int id)
        {
            Func<SqlConnection, List<tal_newappsetdata_v2>> action = (connection) => DalHomePageConfig.GetTal_newappsetdata_v2ById(connection, id);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 根据parentid获取配置信息
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public List<tal_newappsetdata_v2> GetTal_newappsetdata_v2ByParentId(int parentid)
        {
            Func<SqlConnection, List<tal_newappsetdata_v2>> action = (connection) => DalHomePageConfig.GetTal_newappsetdata_v2ByParentId(connection, parentid);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 获取全部配置信息
        /// </summary>
        /// <returns></returns>
        public List<tal_newappsetdata_v2> GetALLTal_newappsetdata_v2()
        {
            Func<SqlConnection, List<tal_newappsetdata_v2>> action = (connection) => DalHomePageConfig.GetALLTal_newappsetdata_v2(connection);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 修改大渠道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateTal_newappsetdata_v2(tal_newappsetdata_v2 model)
        {
            Func<SqlConnection, bool> action = (connection) => DalHomePageConfig.UpdateTal_newappsetdata_v2(connection, model);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 添加大渠道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddTal_newappsetdata_v2(tal_newappsetdata_v2 model)
        {
            Func<SqlConnection, bool> action = (connection) => DalHomePageConfig.AddTal_newappsetdata_v2(connection, model);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 添加区域
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddChidAreaData(tal_newappsetdata_v2 model)
        {
            Func<SqlConnection, bool> action = (connection) => DalHomePageConfig.AddChidAreaData(connection, model);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 修改区域
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateChidAreaData(tal_newappsetdata_v2 model)
        {
            Func<SqlConnection, bool> action = (connection) => DalHomePageConfig.UpdateChidAreaData(connection, model);
            return dbManager.Execute(action);
        }

        public int CopyNewAppSetData(int id, int newid)
        {
            Func<SqlConnection, int> action = (connection) => DalHomePageConfig.CopyNewAppSetData(connection, id, newid);
            return dbManager.Execute(action);
        }
        /// <summary>
        /// 动画保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AnimationSave(HomePagePopupAnimation model) {
            Func<SqlConnection, int> action = (connection) => DalHomePageConfig.AnimationSave(connection, model);
            return dbManager.Execute(action);
        }
        /// <summary>
        /// 更换图片
        /// </summary>

        public  bool AnimationUpdate(HomePagePopupAnimation model)
        {
            Func<SqlConnection,bool> action = (connection) => DalHomePageConfig.AnimationUpdate(connection, model);
            return dbManager.Execute(action);
        }
        /// <summary>
        /// 动画删除
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public int AnimationDelete(int PKID)
        {
            Func<SqlConnection, int> action = (connection) => DalHomePageConfig.AnimationDelete(connection, PKID);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 获取可用优惠券
        /// </summary>
        /// <param name="aminaId"></param>
        /// <returns></returns>
        public List<CouponsInPopup> SelectCouponsOnAnimaId(int animaId)
        {
            Func<SqlConnection, List<CouponsInPopup>> action = (connection) => DalHomePageConfig.SelectCouponsOnAnimaId(connection, animaId);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 插入优惠券
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertCouponInPopup(CouponsInPopup model)
        {
            Func<SqlConnection, int> action = (connection) => DalHomePageConfig.InsertCouponInPopup(connection, model);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 更新优惠券
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateCouponInPopup(CouponsInPopup model)
        {
            Func<SqlConnection, int> action = (connection) => DalHomePageConfig.UpdateCouponInPopup(connection, model);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 删除优惠券
        /// </summary>
        /// <param name="PKId"></param>
        /// <returns></returns>
        public int DeleteCouponInPopup(int PKId)
        {
            Func<SqlConnection, int> action = (connection) => DalHomePageConfig.DeleteCouponInPopup(connection, PKId);
            return dbManager.Execute(action);
        }
    }
}
