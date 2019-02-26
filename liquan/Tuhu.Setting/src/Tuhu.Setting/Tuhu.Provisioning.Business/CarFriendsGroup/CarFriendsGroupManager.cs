using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.CarFriendsGroup;
using Tuhu.Provisioning.DataAccess.Entity.CarFriendsGroup;

namespace Tuhu.Provisioning.Business.CarFriendsGroup
{
    public class CarFriendsGroupManager
    {
        private DalCarFriendsGroup dal = null;

        public CarFriendsGroupManager()
        {
            dal = new DalCarFriendsGroup();
        }

        private static readonly Common.Logging.ILog Logger = Common.Logging.LogManager.GetLogger(typeof(CarFriendsGroupManager));

        #region 车友群

        /// <summary>
        /// 获取车友群列表
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<CarFriendsGroupModel> GetCarFriendsGroupList(out int recordCount, int pageSize, int pageIndex)
        {
            var result = new List<CarFriendsGroupModel>();
            try
            {
                using (var conn = ProcessConnection.OpenGungnirReadOnly)
                {
                    result = dal.GetCarFriendsGroupList(conn, out recordCount, pageSize, pageIndex);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetCarFriendsGroupList-> {pageSize} -> {pageIndex}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 新建车友群
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddCarFriendsGroup(CarFriendsGroupModel model)
        {
            var result = false;
            try
            {
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    result = dal.AddCarFriendsGroup(conn, model);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"AddCarFriendsGroup-> {JsonConvert.SerializeObject(model)}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 编辑车友群
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateCarFriendsGroup(CarFriendsGroupModel model)
        {
            bool result = false;
            try
            {
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    result = dal.UpdateCarFriendsGroup(conn, model);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"UpdateCarFriendsGroup -> {JsonConvert.SerializeObject(model)}",e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 逻辑删除车友群
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public bool DeleteCarFriendsGroup(int pkid, string lastUpdateBy)
        {
            bool result = false;
            try
            {
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    result = dal.DeleteCarFriendsGroup(conn,pkid, lastUpdateBy);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"DeleteCarFriendsGroup -> {pkid} -> {lastUpdateBy}", e);
                throw;
            }
            return result;
        }

        #endregion

        #region 途虎管理员

        /// <summary>
        /// 获取途虎管理员列表
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<CarFriendsAdministratorsModel> GetCarFriendsAdministratorList(out int recordCount, int pageSize, int pageIndex)
        {
            var result = new List<CarFriendsAdministratorsModel>();
            try
            {
                using (var conn = ProcessConnection.OpenGungnirReadOnly)
                {
                    result = dal.GetCarFriendsAdministratorList(conn,out recordCount,pageSize,pageIndex);
                }
            }
            catch(Exception e)
            {
                Logger.Error($"GetCarFriendsAdministratorList -> {pageSize} -> {pageIndex}",e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 新增途虎管理员
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddCarFriendsAdministrator(CarFriendsAdministratorsModel model)
        {
            bool isSuccess = false;
            try
            {
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    isSuccess = dal.AddCarFriendsAdministrator(conn, model);
                }
            }
            catch(Exception e)
            {
                Logger.Error($"AddCarFriendsAdministrator -> {JsonConvert.SerializeObject(model)}",e);
                throw;
            }
            return isSuccess;
        }

        /// <summary>
        /// 编辑途虎管理员信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateCarFriendsAdministrator(CarFriendsAdministratorsModel model)
        {
            bool isSuccess = false;
            try
            {
                using(var conn= ProcessConnection.OpenGungnir)
                {
                    isSuccess = dal.UpdateCarFriendsAdministrator(conn, model);
                }
            }
            catch(Exception e)
            {
                Logger.Error($"UpdateCarFriendsAdministrator -> {JsonConvert.SerializeObject(model)}", e);
                throw;
            }
            return isSuccess;
        }

        /// <summary>
        /// 逻辑删除途虎管理员信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="lastUpdateBy"></param>
        /// <returns></returns>
        public bool DeleteCarFriendsAdministrator(int pkid, string lastUpdateBy)
        {
            bool isSuccess = false;
            try
            {
                using(var conn = ProcessConnection.OpenGungnir)
                {
                    isSuccess = dal.DeleteCarFriendsAdministrator(conn,pkid, lastUpdateBy);
                }
            }
            catch(Exception e)
            {
                Logger.Error($"DeleteCarFriendsAdministrator -> {pkid} -> {lastUpdateBy}", e);
                throw;
            }
            return isSuccess;
        }
        #endregion
    }
}
