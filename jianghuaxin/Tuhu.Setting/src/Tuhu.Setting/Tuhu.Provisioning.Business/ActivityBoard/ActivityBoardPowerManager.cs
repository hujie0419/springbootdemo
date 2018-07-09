using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.ActivityBoard;
using Tuhu.Provisioning.DataAccess.Entity.ActivityBoard;

namespace Tuhu.Provisioning.Business.ActivityBoard
{
    public class ActivityBoardPowerManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;

        private static readonly ILog logger = LoggerFactory.GetLogger("ActivityBoardPowerManager");

        public ActivityBoardPowerManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }

        /// <summary>
        /// 获取权限配置信息
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public List<ActivityBoardPermissionConfig> GetActivityBoardPowerConfig(string userEmail, int pageIndex, int pageSize, PagerModel pager)
        {
            List<ActivityBoardPermissionConfig> result = new List<ActivityBoardPermissionConfig>();

            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    pager.TotalItem = DALActivityBoardPower.SelectPowerConfigUserCount(conn);
                    var userEmailList = DALActivityBoardPower.SelectActivityBoardPowerConfigUser(conn, userEmail, pageIndex, pageSize);
                    if (userEmailList != null && userEmailList.Any())
                    {
                        var userEmailStr = string.Join(",", userEmailList);
                        result = DALActivityBoardPower.SelectActivityBoardPowerConfig(conn, userEmailStr);
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetActivityBoardPowerConfig");
            }

            return result;
        }
        
        /// <summary>
        /// 根据账户查询用户权限
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public List<ActivityBoardPermissionConfig> SelectPowerConfigByUserEmail(string userEmail)
        {
            List<ActivityBoardPermissionConfig> result = new List<ActivityBoardPermissionConfig>();

            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DALActivityBoardPower.SelectPowerConfigByUserEmail(conn, userEmail);
                });
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SelectPowerConfigByUserEmail");
            }

            return result;
        }

        /// <summary>
        /// 根据用户账号和模块名称获取权限
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActivityBoardPermissionConfig GetPowerConfigByUserEmail(string userEmail, ActivityBoardModuleType type)
        {
            ActivityBoardPermissionConfig result = null;

            try
            {
                result = dbScopeReadManager.Execute(conn => DALActivityBoardPower.SelectPowerConfigByUserEmail(conn, userEmail, type));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetPowerConfigByUserEmail");
            }

            return result;
        }

        /// <summary>
        /// 添加模块权限
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Tuple<bool, string> InsertActivityBoardPowerConfig(ActivityBoardPermissionConfig data)
        {
            var result = false;
            var msg = "数据为空";
            try
            {
                if (data != null)
                {
                    dbScopeManager.Execute(conn =>
                    {
                        var info = DALActivityBoardPower.SelectPowerConfigByUserEmail(conn, data.UserEmail.Trim(), data.ModuleType);
                        if (info == null)
                        {
                            result = DALActivityBoardPower.InsertActivityBoardPowerConfig(conn, data) > 0;
                        }
                        else
                        {
                            msg = "此用户的该模块权限已配置";
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                msg = "操作异常";
                logger.Log(Level.Error, ex, "InsertActivityBoardPowerConfig");
            }

            return Tuple.Create(result, msg);
        }

        /// <summary>
        /// 编辑模块权限
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Tuple<bool, string> UpdateActivityBoardPowerConfig(ActivityBoardPermissionConfig data)
        {
            var result = false;
            var msg = "参数为空";

            try
            {
                if (data != null)
                {
                    dbScopeManager.Execute(conn =>
                    {
                        if (DALActivityBoardPower.SelectPowerConfigByUserEmail(conn, data.UserEmail, data.ModuleType) == null)
                        {
                            result = DALActivityBoardPower.InsertActivityBoardPowerConfig(conn, data) > 0;
                        }
                        else
                        {
                            result = DALActivityBoardPower.UpdateActivityBoardPowerConfig(conn, data) > 0;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                msg = "操作异常";
                logger.Log(Level.Error, ex, "UpdateActivityBoardPowerConfig");
            }

            return Tuple.Create(result, msg);
        }

        /// <summary>
        /// 删除模块权限
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public bool DeleteActivityBoardPowerConfig(string userEmail)
        {
            var result = false;

            try
            {
                result = dbScopeManager.Execute(conn => DALActivityBoardPower.DeleteActivityBoardPowerConfig(conn, userEmail)) > 0;
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DeleteActivityBoardPowerConfig");
            }

            return result;
        }

        /// <summary>
        /// 验证上传的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Tuple<List<ActivityBoardPermissionConfig>, List<ActivityBoardPermissionConfig>> VerifyData(List<ActivityBoardPermissionConfig> data)
        {
            var success = new List<ActivityBoardPermissionConfig>();
            var error = new List<ActivityBoardPermissionConfig>();

            try
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (Regex.IsMatch(item.UserEmail.Trim(), @"\w*@tuhu.cn$"))
                        {
                            success.Add(item);
                        }
                        else
                        {
                            error.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "VerifyData");
            }

            return Tuple.Create(success, error);
        }

        /// <summary>
        /// 批量操作数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Tuple<bool, string> BatchOperationData(List<ActivityBoardPermissionConfig> data)
        {
            var result = false;
            string msg = "失败用户：";
            try
            {
                if (data != null && data.Any())
                {
                    dbScopeManager.Execute(conn =>
                    {
                        foreach (var item in data)
                        {
                            if (DALActivityBoardPower.SelectPowerConfigByUserEmail(conn, item.UserEmail, item.ModuleType) == null)
                            {
                                result = DALActivityBoardPower.InsertActivityBoardPowerConfig(conn, item) > 0;
                                if (!result)
                                {
                                    msg += $"{ item.UserEmail } ";
                                }
                            }
                            else
                            {
                                result = DALActivityBoardPower.UpdateActivityBoardPowerConfig(conn, item) > 0;
                                if (!result)
                                {
                                    msg += $"{ item.UserEmail } ";
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                msg = "操作异常";
                logger.Log(Level.Error, ex, "BatchOperationData");
            }

            return Tuple.Create(result, msg);
        }

        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="moduleType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool VerifyPermissions(string userEmail, ActivityBoardModuleType moduleType, OperationType type)
        {
            var result = false;

            try
            {
                var data = dbScopeReadManager.Execute(conn => DALActivityBoardPower.SelectPowerConfigByUserEmail(conn, userEmail, moduleType));
                if (data != null)
                {
                    switch (type)
                    {
                        case OperationType.Insert:
                            result = data.InsertActivity;
                            break;
                        case OperationType.Delete:
                            result = data.DeleteActivity;
                            break;
                        case OperationType.Update:
                            result = data.UpdateActivity;
                            break;
                        case OperationType.View:
                            result = data.ViewActivity;
                            break;
                        case OperationType.IsVisible:
                            result = data.IsVisible;
                            break;
                        case OperationType.Effect:
                            result = data.ActivityEffect;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "VerifyPermissions");
            }

            return result;
        }
    }
}
