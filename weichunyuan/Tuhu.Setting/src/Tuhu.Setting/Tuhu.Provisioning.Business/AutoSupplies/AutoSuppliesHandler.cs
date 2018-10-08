using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.AutoSuppliesManagement
{
    public class AutoSuppliesHandler
    {
        #region Private Fields
        private readonly IDBScopeManager dbManager;
        #endregion

        #region Ctor
        public AutoSuppliesHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion

        #region Advertise
        public List<Advertise> GetSuppliesModule()
        {
            Func<SqlConnection, List<Advertise>> action = (connection) => DALAdvertise.GetAllAdvertise(connection);
            return dbManager.Execute(action);
        }
        public void DeleteAdvertise(int id)
        {
            Action<SqlConnection> action = (connection) => DALAdvertise.DeleteAdvertise(connection, id);
            dbManager.Execute(action);
        }
        public string GetStatusName(DateTime? BeginDateTime, DateTime? EndDateTime)
        {
            string kstr = string.Empty;
            if (BeginDateTime.HasValue && EndDateTime.HasValue)
            {
                if (BeginDateTime > DateTime.Now)
                { kstr = "未开始"; }
                else
                {
                    if (EndDateTime < DateTime.Now)
                    { kstr = "时间已截止"; }
                    else
                    { kstr = "截止时间:" + EndDateTime.Value.ToString("yyyy-MM-dd"); }
                }
            }
            else
            { kstr = "时间已截止"; }
            return kstr;
        }
        public void AddAdvertise(Advertise advertise)
        {
            Action<SqlConnection> action = (connection) => DALAdvertise.AddAdvertise(connection, advertise);
            dbManager.Execute(action);
        }
        public void UpdateAdvertise(Advertise advertise)
        {
            Action<SqlConnection> action = (connection) => DALAdvertise.UpdateAdvertise(connection, advertise);
            dbManager.Execute(action);
        }
        public Advertise GetAdvertiseByID(int id)
        {
            Func<SqlConnection, Advertise> action = (connection) => DALAdvertise.GetAdvertiseByID(connection, id);
            return dbManager.Execute(action);
        }
        public bool IsExistsxAdColumnID(string AdColumnID)
        {
            Func<SqlConnection, bool> action = (connection) => DALAdvertise.IsExistsxAdColumnID(connection, AdColumnID);
            return dbManager.Execute(action);
        }
        #endregion

        #region AdProList
        public List<AdProduct> GetAdProListByAdID(int AdvertiseID)
        {
            Func<SqlConnection, List<AdProduct>> action = (connection) => DALAdvertise.GetAdProListByAdID(connection, AdvertiseID);
            return dbManager.Execute(action);
        }

        public string GetCountByAdID(int AdvertiseID)
        {
            Func<SqlConnection, string> action = (connection) => DALAdvertise.GetCountByAdID(connection, AdvertiseID);
            return dbManager.Execute(action);
        }
        public void DeleteAdProduct(int AdvertiseID, string PID)
        {
            Action<SqlConnection> action = (connection) => DALAdvertise.DeleteAdProduct(connection, AdvertiseID, PID);
            dbManager.Execute(action);
        }
        public void ChangeState(int AdvertiseID, string PID, byte State)
        {
            Action<SqlConnection> action = (connection) => DALAdvertise.ChangeState(connection, AdvertiseID, PID, State);
            dbManager.Execute(action);
        }

        public string GetProductNameByPID(string PID)
        {
            Func<SqlConnection, string> action = (connection) => DALAdvertise.GetProductNameByPID(connection, PID);
            return dbManager.Execute(action);
        }

        public Dictionary<string, string> GetProductNamesByPids(IEnumerable<string> pids)
        {
            Func<SqlConnection, Dictionary<string, string>> action = (connection) => DALAdvertise.GetProductNamesByPids(connection, pids);
            return dbManager.Execute(action);
        }

        public string GetCateNameByCateID(string CateID)
        {
            Func<SqlConnection, string> action = (connection) => DALAdvertise.GetCateNameByCateID(connection, CateID);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 根据PID获取产品相关信息
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public DataTable GetProductInfoByPID(string PID)
        {
            Func<SqlConnection, DataTable> action = (connection) => DALAdvertise.GetProductInfoByPID(connection, PID);
            return dbManager.Execute(action);
        }

        public void UpdateAdProduct(int AdvertiseID, string PID, string NewPID, byte Position, decimal PromotionPrice, int PromotionNum)
        {
            Action<SqlConnection> action = (connection) => DALAdvertise.UpdateAdProduct(connection, AdvertiseID, PID, NewPID, Position, PromotionPrice, PromotionNum);
            dbManager.Execute(action);
        }
        public void AddAdProduct(int AdvertiseID, string PID, byte Position, byte State, decimal PromotionPrice, int PromotionNum)
        {
            Action<SqlConnection> action = (connection) => DALAdvertise.AddAdProduct(connection, AdvertiseID, PID, Position, State, PromotionPrice, PromotionNum);
            dbManager.Execute(action);
        }
        #endregion

        #region ActionsetTab
        public List<BizActionsetTab> GetAllActionsetTab()
        {
            Func<SqlConnection, List<BizActionsetTab>> action = (connection) => DalActionsetTab.GetAllActionsetTab(connection);
            return dbManager.Execute(action);
        }
        public void DeleteActionsetTab(int id)
        {
            Action<SqlConnection> action = (connection) => DalActionsetTab.DeleteActionsetTab(connection, id);
            dbManager.Execute(action);
        }
        public void AddActionsetTab(BizActionsetTab actionsetTab)
        {
            Action<SqlConnection> action = (connection) => DalActionsetTab.AddActionsetTab(connection, actionsetTab);
            dbManager.Execute(action);
        }
        public void UpdateActionsetTab(BizActionsetTab actionsetTab)
        {
            Action<SqlConnection> action = (connection) => DalActionsetTab.UpdateActionsetTab(connection, actionsetTab);
            dbManager.Execute(action);
        }
        public BizActionsetTab GetActionsetTabByID(int id)
        {
            Func<SqlConnection, BizActionsetTab> action = (connection) => DalActionsetTab.GetActionsetTabByID(connection, id);
            return dbManager.Execute(action);
        }
        #endregion

        public List<NewAppSet> SelectNewAppSet()
        {
            return dbManager.Execute(conn => DalNewAppSet.SelectNewAppSet(conn));
        }

        /// <summary>
        /// 新版本 通过产品id获取产品信息
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public DataTable GetProductInfoByPIdNewVersion(string PID)
        {
            Func<SqlConnection, DataTable> action = (connection) => DALAdvertise.GetProductInfoByPIdNewVersion(connection, PID);
            return dbManager.Execute(action);
        }
    }
}
