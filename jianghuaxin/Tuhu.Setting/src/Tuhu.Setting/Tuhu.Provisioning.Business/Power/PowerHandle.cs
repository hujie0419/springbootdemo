using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using EnOrDe = Tuhu.Component.Framework.EnOrDeHelper;

namespace Tuhu.Provisioning.Business.Power
{
    public class PowerHandle
    {
        #region Private Fields

        private readonly IDBScopeManager dbManager;

        #endregion

        #region Ctor

        public PowerHandle(IDBScopeManager _dbManager)
        {
            this.dbManager = _dbManager;
        }

        #endregion

        /// <summary>
        /// 权限验证—服务类
        /// </summary>
        /// <param name="listPower">权限实体</param>
        /// <param name="userNo">账号</param>
        /// <param name="controllerName">控制器名</param>
        /// <param name="actionName"></param>
        /// <param name="query">页面INFO</param>
        /// <param name="msg">输出错误</param>
        /// <returns>布尔值</returns>
        public static bool PowerValidServer(List<ActionPower> listPower, string userNo, string controllerName, string actionName, string query, string actionKey, bool isview, out string msg)
        {
            msg = "";
            string supperUsers = System.Configuration.ConfigurationManager.AppSettings["SupperUsers"];
            if (supperUsers.Contains(userNo + "|"))
            {
                msg = "|";
                return true;
            }
            if (controllerName.ToLower() == "home" && (actionName.ToLower() == "errorpage" || actionName.ToLower() == "errorfunc" || actionName.ToLower() == "index" || actionName.ToLower() == "getbtnpower" || actionName.ToLower() == "issupperuser"))
            {
                msg = "|";
                return true;
            }
            string type = "module";
            if (!isview)
                type = "function";
            else
            {
                if (!query.ToLower().Contains("key"))
                    type = "function";
            }
            if (listPower == null || listPower.Count == 0)
            {
                msg = "人员权限未配置或资源丢失，请重新登录或联系系统管理员|" + type;
                return false;
            }
            #region 特征属性KEY验证
            if (!string.IsNullOrEmpty(actionKey))
            {
                listPower = listPower.Where(ap => (string.IsNullOrEmpty(ap.ActionKey) ? "" : ap.ActionKey).Equals(actionKey)).ToList();
                if (listPower.Count == 0)
                {
                    msg = "没有该人员权限，请联系部门管理员配置权限|" + type;
                    return false;
                }
                else
                {
                    msg = "|" + type;
                    return true;
                }
            }
            #endregion
            #region 路径KEY值验证
            query = query.TrimStart('?');
            if (query.Contains("KEY="))
            {
                string sign = "";
                foreach (string str in query.Split('&'))
                {
                    var strs = str.Split('=');
                    if (strs.Length < 2)
                        continue;
                    if (strs[0].ToLower() == "key")
                    {
                        sign = strs[1];
                        break;
                    }
                    else
                        continue;
                }
                string keyStr = "search12031136";
                listPower = listPower.Where(ap => EnOrDe.GetMd5("Info=" + ap.PKID.ToString() + EnOrDe.GetMd5(keyStr, Encoding.UTF8), Encoding.UTF8).Equals(sign) && ap.Controller.ToLower() == controllerName.ToLower() && ap.Action.ToLower() == actionName.ToLower()).ToList();
                if (listPower.Count == 0)
                {
                    msg = "没有该人员权限，请联系部门管理员配置权限|" + type;
                    return false;
                }
                else
                {
                    msg = "|" + type;
                    return true;
                }
            }
            #endregion
            #region 普通验证
            listPower = listPower.Where(p => p.Controller.ToLower() == controllerName.ToLower() && p.Action.ToLower() == actionName.ToLower()).Select(p => p).ToList();
            if (listPower.Count == 0)
            {
                msg = "没有该人员权限，请联系部门管理员配置权限|" + type;
                return false;
            }
            #endregion
            if (msg == "")
                msg = "|" + type;
            return true;
        }

        public List<CAPower> SelectPowerLists(string userNo, bool isSupper, string module)
        {
            return dbManager.Execute(connection => DalPower.SelectPowerLists(connection, userNo, isSupper, module));
        }

        /// <summary>
        /// 获取权限信息
        /// </summary>
        /// <param name="userNo">用户账号</param>
        /// <returns>实体类</returns>
        public List<CAPower> GetPowerList(string userNo, byte issupper)
        {
            return dbManager.Execute(conn => new DalPower().GetPowerList(conn, userNo, issupper));
        }

        /// <summary>
        /// 获取部门信息 超级管理员获取所有
        /// </summary>
        /// <param name="userNo">账号</param>
        /// <returns>数据集</returns>
        public DataTable GetDeptInfo(string userNo)
        {
            return dbManager.Execute(conn => new DalPower().GetDeptInfo(conn, userNo));
        }

        /// <summary>
        /// 插入角色信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="roleName"></param>
        /// <param name="remark"></param>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public int InsertRole(string roleName, string remark, int deptId, string pid, string cid)
        {
            return dbManager.Execute(conn => new DalPower().InsertRole(conn, roleName, remark, deptId, pid, cid));
        }

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public DataTable GetRole(int id, int flag, int deptid)
        {
            return dbManager.Execute(conn => new DalPower().GetRole(conn, id, flag, deptid));
        }

        /// <summary>
        /// 获取职员信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public DataTable GetEmployee(int deptId, int roleId, string emailAddress)
        {
            return dbManager.Execute(conn => new DalPower().GetEmployee(conn, deptId, roleId, emailAddress));
        }

        /// <summary>
        /// 插入或删除用户角色
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="roleId"></param>
        /// <param name="insOrDel"></param>
        /// <returns></returns>
        public int InsertOrDeleteEr(int employeeId, int roleId, string insOrDel)
        {
            return dbManager.Execute(conn => new DalPower().InsertOrDeleteEr(conn, employeeId, roleId, insOrDel));
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int DelRole(int roleId)
        {
            var delResult = dbManager.Execute(conn => new DalPower().DelRole(conn, roleId));

            var oprLog = new OprLog();
            oprLog.Author = ThreadIdentity.Operator.Name;
            oprLog.ChangeDatetime = DateTime.Now;
            oprLog.ObjectID = roleId;
            oprLog.ObjectType = "ROLE";
            oprLog.Operation = "DelRole";
            new OprLogManager().AddOprLog(oprLog);

            return delResult;
        }

        /// <summary>
        /// 获取部门角色信息
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="isSupper"></param>
        /// <returns></returns>
        public DataSet GetFirstDepartment(string userNo, byte isSupper)
        {
            return dbManager.Execute(conn => new DalPower().GetFirstDepartment(conn, userNo, isSupper));
        }

        public List<ActionPower> GetDeptOrRoleAction(int id, byte flag, int deptid)
        {
            return dbManager.Execute(conn => new DalPower().GetDeptOrRoleAction(conn, id, flag, deptid));
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
            return dbManager.Execute(conn => new DalPower().SaveRolePower(conn, roleid, deptid, actions));
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
            return dbManager.Execute(conn => new DalPower().SavePersonPower(conn, employeeid, deptid, actions));
        }

        public int AddModuleInf(string xmlData)
        {
            return dbManager.Execute(conn => new DalPower().AddModuleInf(conn, xmlData));
        }
        public List<ActionPower> SelectHrAccessList_HrGroupModule()
        {
            return dbManager.Execute(conn => new DalPower().SelectHrAccessList_HrGroupModule(conn));
        }
        public List<ActionPower> SelectHrAccessList_HrGroupModule(string linkName, int PageNumber)
        {
            return dbManager.Execute(conn => new DalPower().SelectHrAccessList_HrGroupModule(conn, linkName, PageNumber));
        }
        public int GetTotalModuleManagerNumber(string linkName)
        {

            return dbManager.Execute(conn => new DalPower().GetTotalModuleManagerNumber(conn, linkName));
        }

        public int DeleteModule(int id)
        {
            return dbManager.Execute(conn => new DalPower().DeleteModule(conn, id));
        }

        public int UpdateModuleInf(int id, string xmlData)
        {
            return dbManager.Execute(conn => new DalPower().UpdateModuleInf(conn, id, xmlData));
        }

        public int ChangeDeptModule(int id, string GroupIds)
        {
            return dbManager.Execute(conn => new DalPower().ChangeDeptModule(conn, id, GroupIds));
        }

        public DataTable GetModuleGroupID(int moduleid)
        {
            return dbManager.Execute(conn => new DalPower().GetModuleGroupID(conn, moduleid));
        }

        /// <summary>
        /// 是否超级用户
        /// </summary>
        /// <param name="userno"></param>
        /// <returns></returns>
        public static bool IsSupperUser(string userno)
        {
            return System.Configuration.ConfigurationManager.AppSettings["SupperUsers"].Contains(userno + "|");
        }

        /// <summary>
        /// 是否有操作权限
        /// </summary>
        /// <param name="userno"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="btnKey"></param>
        /// <returns></returns>
        public bool IsOprPower(List<ActionPower> list, string userno, string controller, string action, string btnKey)
        {
            if (IsSupperUser(userno))
                return true;
            list = list.Where(ap => ap.Controller.ToLower() == controller.ToLower() && ap.Action.ToLower() == action.ToLower() && ap.BtnKey == btnKey).ToList();
            if (list == null || list.Count == 0)
                return false;
            return dbManager.Execute(conn => DalPower.IsAccessPower(conn, userno, list[0].PKID));
        }

        /// <summary>
        /// 工作流配置
        /// </summary>
        /// <returns></returns>
        public DataTable GetWorkFlowInfo()
        {
            return this.dbManager.Execute(conn => DalPower.GetWorkFlowInfo(conn));
        }

        public void SaveWork(string name, string pid, string remark)
        {
            this.dbManager.Execute(conn => DalPower.SaveWork(conn, name, pid, remark));
        }

        public void DelWork(int pkid)
        {
            this.dbManager.Execute(conn => DalPower.DelWork(conn, pkid));
        }

        public void UptWork(string name, string pid, string remark, int id)
        {
            this.dbManager.Execute(conn => DalPower.UptWork(conn, name, pid, remark, id));
        }

        public DataTable GetAllUsers()
        {
            return this.dbManager.Execute(conn => DalPower.GetAllUsers(conn));
        }

        public string GetWorkPowerUsers(int id)
        {
            return this.dbManager.Execute(conn => DalPower.GetWorkPowerUsers(conn, id));
        }

        public void SavePower(string pus, string keyid, int keytype)
        {
            this.dbManager.Execute(conn => DalPower.SavePower(conn, pus, keyid, keytype));
        }

        public void SaveStep(string name, string pname, int isend, int workid)
        {
            this.dbManager.Execute(conn => DalPower.SaveStep(conn, name, pname, isend, workid));
        }

        public void DelStep(int id)
        {
            this.dbManager.Execute(conn => DalPower.DelStep(conn, id));
        }

        public void UptStep(string name, string pname, int isend, int id)
        {
            this.dbManager.Execute(conn => DalPower.UptStep(conn, name, pname, isend, id));
        }

        public string GetStepPowerUsers(int id)
        {
            return this.dbManager.Execute(conn => DalPower.GetStepPowerUsers(conn, id));
        }

        public string GetStepPowerForms(int id)
        {
            return this.dbManager.Execute(conn => DalPower.GetStepPowerForms(conn, id));
        }

        public DataTable GetAllForms()
        {
            return this.dbManager.Execute(conn => DalPower.GetAllForms(conn));
        }

        public void saveFormPower(string forms, string keyid, int keytype)
        {
            this.dbManager.Execute(conn => DalPower.saveFormPower(conn, forms, keyid, keytype));
        }

        public void SaveForm(string fname, string furl, int gno, int stepid)
        {
            this.dbManager.Execute(conn => DalPower.SaveForm(conn, fname, furl, gno, stepid));
        }

        public void DelStepForm(string sfid)
        {
            this.dbManager.Execute(conn => DalPower.DelStepForm(conn, sfid));
        }

        public void UptForm(string fname, string furl, int gno, int formid)
        {
            this.dbManager.Execute(conn => DalPower.UptForm(conn, fname, furl, gno, formid));
        }

        public void SaveBtn(string BtnKey, int BtnType, string sfid)
        {
            this.dbManager.Execute(conn => DalPower.SaveBtn(conn, BtnKey, BtnType, sfid));
        }

        public void DelBtn(int id)
        {
            this.dbManager.Execute(conn => DalPower.DelBtn(conn, id));
        }

        public string GetBtnPowerUsers(int id)
        {
            return this.dbManager.Execute(conn => DalPower.GetBtnPowerUsers(conn, id));
        }

        public DataTable GetNewWorkInfo(int id)
        {
            return this.dbManager.Execute(conn => DalPower.GetNewWorkInfo(conn, id));
        }

        public List<ActionPower> GetBusPower(string userNo, byte issupper)
        {
            return dbManager.Execute(conn => new DalPower().GetBusPower(conn, userNo, issupper));
        }

        public void ModifyRoleNameByID(string rolename, int id)
        {
            dbManager.Execute(conn => DalPower.ModifyRoleNameByID(conn, rolename, id));
        }

        public List<PurchaseEmplpyee> GetPurchaseEmplpyeeEmail()
        {
            return dbManager.Execute(conn => DalPower.GetPurchaseEmplpyeeEmail(conn));
        }

        /// <summary>
        /// 获取用户的按钮权限
        /// </summary>
        /// <returns></returns>
        public List<ActionPower> GetUserButtonPermission(string controller, string action, string emailAddress)
        {
            return dbManager.Execute(conn => DalPower.GetUserButtonPermission(conn, controller, action, emailAddress));
        }

        /// <summary>
        /// 获取菜单信息
        /// </summary>
        /// <returns></returns>
        public DataSet GetMenuInfo()
        {
            return dbManager.Execute(conn => DalPower.GetMenuInfo(conn));
        }

        public void InsertOrDelMenuRelation(int cid, int aid, int flag)
        {
            dbManager.Execute(conn => DalPower.InsertOrDelMenuRelation(conn, cid, aid, flag));
        }

        public void AddMenu(string cname, string iname, string createdBy = "admin")
        {
            dbManager.Execute(conn => DalPower.AddMenu(conn, cname, iname, createdBy));
        }

        public void AddDefaultEr(string userEmail, string roles)
        {
            dbManager.Execute(conn => DalPower.AddDefaultEr(conn, userEmail, roles));
        }
    }
}
