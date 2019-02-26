using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.TireActivity;

namespace Tuhu.Provisioning.Business.TireActivity
{
    public class TireActivityManage
    {
        private static readonly Common.Logging.ILog Logger;

        #region 小保养套餐优惠价格

        /// <summary>
        /// 获取小保养套餐优惠价格列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<MaintenancePackageOnSaleModel> GetMaintenancePackageOnSaleList( out int recordCount,int pageSize = 20, int pageIndex = 1)
        {
            try
            {
                var result=DataAccess.DAO.TireActivity.DalTireActivity.GetMaintenancePackageOnSaleList(out recordCount,pageSize, pageIndex);
                return result;
            }
            catch(Exception ex)
            {
                Logger.Error("GetMaintenancePackageOnSaleList", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取有效的小保养套餐PID集合
        /// </summary>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public List<MaintenancePackageOnSaleModel> GetMaintenancePackagePidList(List<string> pidList)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.GetMaintenancePackagePidList(pidList);
            }
            catch (Exception ex)
            {
                Logger.Error("GetMaintenancePackagePidList", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 下载每一次上传的excel
        /// </summary>
        /// <param name="updateID"></param>
        /// <returns></returns>
        public List<MaintenancePackageOnSaleModel> GetEachMaintenancePackageList(int updateID)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.GetEachMaintenancePackageList(updateID);
            }
            catch (Exception ex)
            {
                Logger.Error("GetEachMaintenancePackageList", ex);
                throw ex;
            }
        }

        #region 导入excel-小保养套餐优惠价格数据

        /// <summary>
        /// 获取小保养套餐优惠价格表中最大的更新ID
        /// </summary>
        /// <returns></returns>
        public static int GetMaxUpdateID()
        {
            try
            {
				int maxUpdateID = 0;
                var maxModel = DataAccess.DAO.TireActivity.DalTireActivity.GetMaxUpdateID().FirstOrDefault();
                if (maxModel != null) maxUpdateID = maxModel.UpdateID;
                return maxUpdateID;
            }
            catch (Exception ex)
            {
                Logger.Error("GetMaxUpdateID", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 将有效的小保养套餐设置为无效的
        /// </summary>
        /// <returns></returns>
        public static int UpdateMaintenancePackageState()
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.UpdateMaintenancePackageState();
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateMaintenancePackageState", ex);
                throw ex;
            }
        }
        /// <summary>
        /// 根据PID获取小保养套餐优惠价格数据
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<MaintenancePackageOnSaleModel> GetMaintenancePackageOnSaleModelByPid(string pid)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.GetMaintenancePackageOnSaleModelByPid(pid);
            }
            catch (Exception ex)
            {
                Logger.Error("GetMaintenancePackageOnSaleModelByPid", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 逻辑删除小保养套餐优惠价格数据
        /// </summary>
        /// <param name="pkidList"></param>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public static int UpdateMaintenancePackageOnSale(List<int> pkidList, string lastUpdateBy)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.UpdateMaintenancePackageOnSale(pkidList, lastUpdateBy);
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateMaintenancePackageOnSale", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 添加小保养套餐优惠价格数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="maxUpdateID"></param>
        /// <returns></returns>
        public static int AddMaintenancePackageOnSale(MaintenancePackageOnSaleModel model, int maxUpdateID)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.AddMaintenancePackageOnSale(model,maxUpdateID);
            }
            catch (Exception ex)
            {
                Logger.Error("AddMaintenancePackageOnSale", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 添加小保养套餐优惠价格数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddMaintenancePackageOnSaleList(out int maxUpdateID,List<MaintenancePackageOnSaleModel> list)
        {
            bool success = false;
            maxUpdateID = GetMaxUpdateID()+1;
            int count=UpdateMaintenancePackageState();
            if (count >= 0)
            {
                foreach (var item in list)
                {
                    success = AddMaintenancePackageOnSale(item, maxUpdateID) > 0;
                }
            }
            return success;
        }

        #endregion

       
        #endregion

        #region 轮保定价

        /// <summary>
        /// 获取轮胎活动列表
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<TireActivityModel> GetTireActivityList(out int recordCount, int pageSize = 20, int pageIndex = 1)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.GetTireActivityList(out recordCount, pageSize , pageIndex);
            }
            catch (Exception ex)
            {
                Logger.Error("GetTireActivityList", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 根据pkid获得轮胎活动计划数据
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public TireActivityModel GetTireActivityModel(int pkid)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.GetTireActivityModel(pkid);
            }
            catch (Exception ex)
            {
                Logger.Error("GetTireActivityModel", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 停止轮胎活动计划
        /// </summary>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public int UpdateTireActivityStatus(int pkid, string lastUpdateBy)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.UpdateTireActivityStatus(pkid,lastUpdateBy);
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateTireActivityStatus", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 轮胎活动列表-下载excel
        /// </summary>
        /// <param name="tireActivityID"></param>
        /// <returns></returns>
        public List<TireActivityPIDModel> GetTireActivityPIDList(int tireActivityID)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.GetTireActivityPIDList(tireActivityID);
            }
            catch (Exception ex)
            {
                Logger.Error("GetTireActivityPIDList", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取有效的轮胎产品PID集合
        /// </summary>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public List<TireActivityPIDModel> GetValidTirePid(List<string> pidList)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.GetValidTirePid(pidList);
            }
            catch (Exception ex)
            {
                Logger.Error("GetValidTirePid", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取轮胎活动计划最大更新ID
        /// </summary>
        /// <returns></returns>
        public int GetMaxTireActivityUpdateID()
        {
            try
            {
 				int maxUpdateID = 0;
                var maxModel = DataAccess.DAO.TireActivity.DalTireActivity.GetMaxTireActivityUpdateID().FirstOrDefault();
                if (maxModel != null) maxUpdateID = maxModel.UpdateID;
                return maxUpdateID;
            }
            catch (Exception ex)
            {
                Logger.Error("GetMaxTireActivityUpdateID", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取重复轮胎PID个数与计划
        /// </summary>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public List<TireActivityModel> GetRepeatTirePids(List<string> pidList)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.GetRepeatTirePids(pidList);
            }
            catch (Exception ex)
            {
                Logger.Error("GetRepeatTirePids", ex);
                throw ex;
            }
        }

        #region 导入Excel-轮保定价

        /// <summary>
        ///  获取轮胎活动数据
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<TireActivityPIDModel> GetTireActivityPidByPid(string pid)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.GetTireActivityPidByPid(pid);
            }
            catch (Exception ex)
            {
                Logger.Error("GetTireActivityPidByPid", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 逻辑删除轮胎
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="LastUpdateBy"></param>
        /// <returns></returns>
        public static int UpdateTireActivityPid(int pkid, string LastUpdateBy)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.UpdateTireActivityPid(pkid, LastUpdateBy);
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateTireActivityPid", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 轮胎活动计划的PID数量减1
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="LastUpdateBy"></param>
        /// <returns></returns>
        public static int UpdateTireActivityPidNum(int pkid, string LastUpdateBy)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.UpdateTireActivityPidNum(pkid, LastUpdateBy);
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateTireActivityPidNum", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 添加轮胎活动计划
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddTireActivity(TireActivityModel model)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.AddTireActivity(model);
            }
            catch (Exception ex)
            {
                Logger.Error("AddTireActivity", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 添加轮胎数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddTireActivityPid(TireActivityPIDModel model)
        {
            try
            {
                return DataAccess.DAO.TireActivity.DalTireActivity.AddTireActivityPid(model);
            }
            catch (Exception ex)
            {
                Logger.Error("AddTireActivityPid", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 导入Excel-轮胎PID
        /// </summary>
        /// <param name="model"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool ImportTireActivityPid(TireActivityModel model, List<TireActivityPIDModel> list)
        {
            bool sucess = false;
            int tireActivityID=AddTireActivity(model);
            foreach (var item in list)
            {
                //检测Excel中的PID是否与己有计划的PID重复
                var repeatList =GetTireActivityPidByPid(item.PID);
                if (repeatList!=null&&repeatList.Count > 0)
                {
                    foreach (var piditem in repeatList)
                    {
                        //重复的PID在被覆盖计划中删除
                        UpdateTireActivityPid(piditem.PKID, item.CreateBy);
                        //重复的PID所在计划的PID数量减去1
                        UpdateTireActivityPidNum(piditem.TireActivityID, item.CreateBy);

                        //如果重复的PID所在计划的PIDNum为0，则暂停该计划
                        var tireActivityModel=GetTireActivityModel(piditem.TireActivityID);
                        if (tireActivityModel != null&& tireActivityModel.PIDNum==0)
                        {
                            UpdateTireActivityStatus(piditem.TireActivityID, item.CreateBy);
                        }
                    }
                }
                item.TireActivityID = tireActivityID;
                sucess=AddTireActivityPid(item)>0;
            }
            return sucess;
        }

        #endregion

        #endregion
    }
}
