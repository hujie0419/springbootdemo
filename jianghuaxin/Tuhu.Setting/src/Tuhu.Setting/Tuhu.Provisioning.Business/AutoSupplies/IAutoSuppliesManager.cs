using System;
using System.Collections.Generic;
using System.Data;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public interface IAutoSuppliesManager
    {
        #region Advertise
        /// <summary>
        /// 获取所有Advertise
        /// </summary>
        List<Advertise> GetSuppliesModule();
        /// <summary>
        /// 删除Advertise
        /// </summary>
        /// <param name="id">PKID</param>
        void DeleteAdvertise(int id);
        /// <summary>
        /// 获取是否在有效期
        /// </summary>
        /// <param name="BeginDateTime">开始时间</param>
        /// <param name="EndDateTime">截止时间</param>
        /// <returns>未开始 未结束 时间已截止</returns>
        string GetStatusName(DateTime? BeginDateTime, DateTime? EndDateTime);
        /// <summary>
        /// 添加Advertise
        /// </summary>
        /// <param name="advertise">Advertise对象</param>
        void AddAdvertise(Advertise advertise);
        /// <summary>
        /// 修改Advertise
        /// </summary>
        /// <param name="advertise">Advertise对象</param>
        void UpdateAdvertise(Advertise advertise);
        /// <summary>
        /// 根据PKID获取单个Advertise
        /// </summary>
        /// <param name="id">PKID</param>
        Advertise GetAdvertiseByID(int id);
        /// <summary>
        /// Advertise中是否存在AdColumnID的对象
        /// </summary>
        /// <param name="AdColumnID"></param>
        /// <returns></returns>
        bool IsExistsxAdColumnID(string AdColumnID);
        #endregion

        #region AdProList
        /// <summary>
        /// 通过模块ID获取产品列表
        /// </summary>
        /// <param name="AdvertiseID">模块ID</param>
        List<AdProduct> GetAdProListByAdID(int AdvertiseID);
        string GetCountByAdID(int AdvertiseID);
        /// <summary>
        /// 删除AdProduct
        /// </summary>
        void DeleteAdProduct(int AdvertiseID, string PID);
        /// <summary>
        /// 修改AdProduct显示状态
        /// </summary>
        /// <param name="State">原状态，如果为0则修改为1</param>
        void ChangeState(int AdvertiseID, string PID, byte State);
        string GetProductNameByPID(string PID);
        string GetCateNameByCateID(string CateID);
        /// <summary>
        /// 根据PID获取产品相关信息
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        DataTable GetProductInfoByPID(string PID);

        Dictionary<string, string> GetProductNamesByPids(IEnumerable<string> pids);
        /// <summary>
        /// 新版本 根据PId获取产品相关信息
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        DataTable GetProductInfoByPIdNewVersion(string PID);

        void UpdateAdProduct(int AdvertiseID, string PID, string NewPID, byte Position, decimal PromotionPrice, int PromotionNum);
        void AddAdProduct(int AdvertiseID, string PID, byte Position, byte State, decimal PromotionPrice, int PromotionNum);

        #endregion

        #region Advertise
        List<BizActionsetTab> GetAllActionsetTab();
        void DeleteActionsetTab(int id);
        void AddActionsetTab(BizActionsetTab actionsetTab);
        void UpdateActionsetTab(BizActionsetTab actionsetTab);
        BizActionsetTab GetActionsetTabByID(int id);
        #endregion
    }
}