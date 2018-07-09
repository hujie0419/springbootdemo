using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.LogisticManagement;
using Tuhu.Nosql;

namespace Tuhu.Provisioning.Business.Power
{
    public class PowerManage : IPowerManage
    {
        #region private connectionStr
        private SqlConnection con = null;
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager =
            new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly ILog logger = LoggerFactory.GetLogger("Power");

        private static readonly IDBScopeManager dbScopeManager = new DBScopeManager(connectionManager);

        static string str2 = ConfigurationManager.ConnectionStrings["GungnirWorkFlow"].ConnectionString;
        private static readonly IConnectionManager powerConnectionManager =
            new ConnectionManager(SecurityHelp.IsBase64Formatted(str2) ? SecurityHelp.DecryptAES(str2) : str2);

        private static readonly IDBScopeManager powerBbScopeManager = new DBScopeManager(powerConnectionManager);
        private readonly PowerHandle handler;
        private readonly PowerHandle powerHandler;
        #endregion

        #region Ctor
        public PowerManage(string connectionStr)
        {
            con = new SqlConnection(connectionStr);
        }

        public PowerManage()
        {
            handler = new PowerHandle(dbScopeManager);
            powerHandler = new PowerHandle(powerBbScopeManager);
        }
        #endregion

        public List<CAPower> SelectPowerLists(string userNo, bool isSupper, string module)
        {
            try
            {
                return handler.SelectPowerLists(userNo, isSupper, module);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取权限信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in Get information of power.");
                throw exception;
            }
        }

        public List<CAPower> GetPowerList(string userNo, byte issupper)
        {
            try
            {
                return handler.GetPowerList(userNo, issupper);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取权限信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in Get information of power.");
                throw exception;
            }
        }

        /// <summary>
        /// 获取部门信息 超级管理员获取所有
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public DataTable GetDeptInfo(string userNo)
        {
            try
            {
                return handler.GetDeptInfo(userNo);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取部门信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in Get information of deparment.");
                throw exception;
            }
        }

        public int InsertRole(string roleName, string remark, int deptId, string pid = "", string cid = "")
        {
            try
            {
                return handler.InsertRole(roleName, remark, deptId, pid, cid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "插入角色信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in inserting into table of 'HrRoles'.");
                throw exception;
            }
        }

        public DataTable GetRole(int id, int flag, int deptid)
        {
            try
            {
                return handler.GetRole(id, flag, deptid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取角色信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in geting information of role.");
                throw exception;
            }
        }

        public DataTable GetEmployee(int deptId, int roleId, string emailAddress)
        {
            try
            {
                return handler.GetEmployee(deptId, roleId, emailAddress);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取职员信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in geting information of employee.");
                throw exception;
            }
        }

        public int InsertOrDeleteEr(int employeeId, int roleId, string insOrDel)
        {
            try
            {
                return handler.InsertOrDeleteEr(employeeId, roleId, insOrDel);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "插入或删除角色用户出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in geting or deleting information.");
                throw exception;
            }
        }

        public int DelRole(int roleId)
        {
            try
            {
                return handler.DelRole(roleId);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "删除角色出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in deleting role.");
                throw exception;
            }
        }

        /// <summary>
        /// 获取部门角色
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="isSupper"></param>
        /// <returns></returns>
        public DataSet GetFirstDepartment(string userNo, byte isSupper)
        {
            try
            {
                return handler.GetFirstDepartment(userNo, isSupper);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取部门角色出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in getting role of department.");
                throw exception;
            }
        }

        public List<ActionPower> GetDeptOrRoleAction(int id, byte flag, int deptid)
        {
            try
            {
                return handler.GetDeptOrRoleAction(id, flag, deptid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取部门或角色ACTION出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred.");
                throw exception;
            }
        }

        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="deptid"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public int SaveRolePower(int roleid, int deptid, string actions)
        {
            try
            {
                return handler.SaveRolePower(roleid, deptid, actions);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "保存角色权限出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred.");
                throw exception;
            }
        }

        /// <summary>
        /// 保存用户权限
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="deptid"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public int SavePersonPower(int employeeid, int deptid, string actions)
        {
            try
            {
                return handler.SavePersonPower(employeeid, deptid, actions);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "保存用户权限出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred.");
                throw exception;
            }
        }

        public int AddModuleInf(string xmlData)
        {
            try
            {
                return handler.AddModuleInf(xmlData);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "插入菜单信息", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in insert ModuleInf.");
                throw exception;
            }
        }
        public List<ActionPower> SelectHrAccessList_HrGroupModule()
        {
            try
            {
                return handler.SelectHrAccessList_HrGroupModule();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取Module信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in Get information of power.");
                throw exception;
            }
        }
        public List<ActionPower> SelectHrAccessList_HrGroupModule(string linkName, int PageNumber)
        {
            try
            {
                return handler.SelectHrAccessList_HrGroupModule(linkName, PageNumber);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取Module信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in Get information of power.");
                throw exception;
            }
        }
        public int GetTotalModuleManagerNumber(string linkName)
        {
            return handler.GetTotalModuleManagerNumber(linkName);
        }
        public int DeleteModule(int id)
        {
            try
            {
                return handler.DeleteModule(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "删除Module相关信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in deleting Module.");
                throw exception;
            }
        }
        public int UpdateModuleInf(int id, string xmlData)
        {
            try
            {
                return handler.UpdateModuleInf(id, xmlData);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "更新菜单信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in update ModuleInf.");
                throw exception;
            }
        }
        public int ChangeDeptModule(int id, string GroupIds)
        {
            try
            {
                return handler.ChangeDeptModule(id, GroupIds);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "更新HrGroupModule信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in update HrGroupModule.");
                throw exception;
            }
        }

        public DataTable GetModuleGroupID(int moduleid)
        {
            try
            {
                return handler.GetModuleGroupID(moduleid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "更新GetModuleGroupID信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in getting.");
                throw exception;
            }
        }

        public DataTable GetWorkFlowInfo()
        {
            try
            {
                return powerHandler.GetWorkFlowInfo();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取工作流信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in getting.");
                throw exception;
            }
        }

        public void SaveWork(string name, string pid, string remark)
        {
            try
            {
                powerHandler.SaveWork(name, pid, remark);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "保存工作流信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in saving worked information.");
                throw exception;
            }
        }

        public void DelWork(int pkid)
        {
            try
            {
                powerHandler.DelWork(pkid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "删除工作流信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in deleting worked information.");
                throw exception;
            }
        }

        public void UptWork(string name, string pid, string remark, int id)
        {
            try
            {
                powerHandler.UptWork(name, pid, remark, id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "更新工作流信息出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in updating worked information.");
                throw exception;
            }
        }

        public DataTable GetAllUsers()
        {
            try
            {
                return powerHandler.GetAllUsers();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取所有用户账号出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in getting used information.");
                throw exception;
            }
        }

        public string GetWorkPowerUsers(int id)
        {
            try
            {
                return powerHandler.GetWorkPowerUsers(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取任务权限出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in getting worked users.");
                throw exception;
            }
        }

        public void SavePower(string pus, string keyid, int keytype)
        {
            try
            {
                powerHandler.SavePower(pus, keyid, keytype);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "保存工作流权限出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in saving power.");
                throw exception;
            }
        }

        public void SaveStep(string name, string pname, int isend, int workid)
        {
            try
            {
                powerHandler.SaveStep(name, pname, isend, workid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "保存任务环节出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in saving step.");
                throw exception;
            }
        }

        public void DelStep(int pkid)
        {
            try
            {
                powerHandler.DelStep(pkid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "删除任务环节出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in deleting worked step.");
                throw exception;
            }
        }

        public void UptStep(string name, string pname, int isend, int id)
        {
            try
            {
                powerHandler.UptStep(name, pname, isend, id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "更新任务环节出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in updating worked step.");
                throw exception;
            }
        }

        public string GetStepPowerUsers(int id)
        {
            try
            {
                return powerHandler.GetStepPowerUsers(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取任务环节权限出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in getting steped users.");
                throw exception;
            }
        }

        public string GetStepPowerForms(int id)
        {
            try
            {
                return powerHandler.GetStepPowerForms(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取任务环节表单权限出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in getting steped forms.");
                throw exception;
            }
        }

        public DataTable GetAllForms()
        {
            try
            {
                return powerHandler.GetAllForms();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取表单出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in getting forms.");
                throw exception;
            }
        }

        public void saveFormPower(string forms, string keyid, int keytype)
        {
            try
            {
                powerHandler.saveFormPower(forms, keyid, keytype);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "保存表单权限出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in saving form power.");
                throw exception;
            }
        }

        public void SaveForm(string fname, string furl, int gno, int stepid)
        {
            try
            {
                powerHandler.SaveForm(fname, furl, gno, stepid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "保存表单出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in saving form.");
                throw exception;
            }
        }

        public void DelStepForm(string sfid)
        {
            try
            {
                powerHandler.DelStepForm(sfid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "删除步骤表单权限出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in deleting steped form.");
                throw exception;
            }
        }

        public void UptForm(string fname, string furl, int gno, int formid)
        {
            try
            {
                powerHandler.UptForm(fname, furl, gno, formid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "更新表单出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in updating form.");
                throw exception;
            }
        }

        public void SaveBtn(string BtnKey, int BtnType, string sfid)
        {
            try
            {
                powerHandler.SaveBtn(BtnKey, BtnType, sfid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "新增按钮出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in saving button information.");
                throw exception;
            }
        }

        public void DelBtn(int id)
        {
            try
            {
                powerHandler.DelBtn(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "删除步骤表单按钮出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in deleting button information.");
                throw exception;
            }
        }

        public string GetBtnPowerUsers(int id)
        {
            try
            {
                return powerHandler.GetBtnPowerUsers(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取步骤表单按钮权限出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in getting button_power information.");
                throw exception;
            }
        }

        public DataTable GetNewWorkInfo(int id)
        {
            try
            {
                return powerHandler.GetNewWorkInfo(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "获取任务出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in getting work information.");
                throw exception;
            }
        }

        public List<ActionPower> GetBusPower(string userNo, byte issupper)
        {
            string conn = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
            IConnectionManager connManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(conn) ? SecurityHelp.DecryptAES(conn) : conn);
            IDBScopeManager dbscope = new DBScopeManager(connManager);
            PowerHandle pHandle = new PowerHandle(dbscope);
            List<ActionPower> actionPowerList = new List<ActionPower>();
            try
            {
                //var cache = CacheFactory.Create(CacheType.CouchBase);
                //var key = EnOrDeHelper.GetMd5(userNo + "power!@#$", Encoding.UTF8);
                //cache.TryGet(key, out actionPowerList);

                //if (actionPowerList != null && actionPowerList.Count != 0)
                //    return actionPowerList;
                var key = EnOrDeHelper.GetMd5(userNo + "power!@#$", Encoding.UTF8);
                using (var client = CacheHelper.CreateCacheClient("Setting"))
                {
                    return (client.GetOrSet(key, () => pHandle.GetBusPower(userNo, issupper),
                        new TimeSpan(1, 0, 0)))?.Value ?? new List<ActionPower>();
                }                  
            }
            catch
            {
                actionPowerList = pHandle.GetBusPower(userNo, issupper);
            }
            return actionPowerList;
        }

        public bool IsOprPower(List<ActionPower> list, string userno, string controller, string action, string btnKey)
        {
            try
            {
                return handler.IsOprPower(list, userno, controller, action, btnKey);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "判断页面按钮出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred.");
                throw exception;
            }
        }

        public void ModifyRoleNameByID(string rolename, int id)
        {
            try
            {
                handler.ModifyRoleNameByID(rolename, id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "更新角色名称出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred.");
                throw exception;
            }
        }
        public List<PurchaseEmplpyee> GetPurchaseEmplpyeeEmail()
        {
            try
            {
                return handler.GetPurchaseEmplpyeeEmail();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "GetPurchaseEmplpyeeEmail", innerEx);
                logger.Log(Level.Error, exception, "Error GetPurchaseEmplpyeeEmail.");
                throw exception;
            }
        }

        /// <summary>
        /// 获取用户的按钮权限
        /// </summary>
        /// <returns></returns>
        public List<ActionPower> GetUserButtonPermission(string controller, string action, string emailAddress)
        {
            try
            {
                return handler.GetUserButtonPermission(controller, action, emailAddress);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "GetUserButtonPermission", innerEx);
                logger.Log(Level.Error, exception, "Error GetUserButtonPermission.");
                throw exception;
            }
        }

        public void InsertOrDelMenuRelation(int cid, int aid, int flag)
        {
            try
            {
                handler.InsertOrDelMenuRelation(cid, aid, flag);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "关联或解除关联错误", innerEx);
                logger.Log(Level.Error, exception, "Error in InsertOrDelMenuRelation.");
                throw exception;
            }
        }

        public void AddMenu(string cname, string iname, string createdBy = "admin")
        {
            try
            {
                handler.AddMenu(cname, iname, createdBy);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "添加菜单错误", innerEx);
                logger.Log(Level.Error, exception, "Error in AddMenu.");
                throw exception;
            }
        }

        public void AddDefaultEr(string userEmail, string roles)
        {
            try
            {
                handler.AddDefaultEr(userEmail, roles);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new LogisticException(BizErrorCode.SystemError, "添加用户角色", innerEx);
                logger.Log(Level.Error, exception, "Error in AddDefaultEr.");
                throw exception;
            }
        }
    }
}
