using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalPower
    {
        /// <summary>
        /// 获取权限信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="userNo">用户账号</param>
        /// <returns>实体类</returns>
        public List<CAPower> GetPowerList(SqlConnection sqlconnection, string userNo, byte issupper)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@userNo", userNo));
            parameters.Add(new SqlParameter("@isSupper", issupper));
            DataTable dt = SqlHelper.ExecuteDataset(sqlconnection, CommandType.StoredProcedure, "Home_Power_GetActionList", parameters.ToArray()).Tables[0];
            return dt.ConvertTo<CAPower>().ToList();
        }

        public static List<CAPower> SelectPowerLists(SqlConnection connection, string userNo, bool isSupper, string module)
        {
            var caPowers = new List<CAPower>();

            var parameters = new[]
            {
                new SqlParameter("@UserNo", userNo),
                new SqlParameter("@IsSupper", isSupper),
                new SqlParameter("@Module", module)
            };

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "Home_Power_SelectActionListByUserNameAndModule", parameters))
            {
                while (reader.Read())
                {
                    var caPower = new CAPower();
                    caPower.LinkName = reader.GetTuhuString(0);
                    caPower.Controller = reader.GetTuhuString(1);
                    caPower.Action = reader.GetTuhuString(2);
                    caPower.ParametersName = reader.GetTuhuString(3);
                    caPower.ParametersValue = reader.GetTuhuString(4);
                    caPower.Module = reader.GetTuhuString(5);
                    caPower.Type = reader.GetTuhuString(6);
                    caPower.BtnKey = reader.GetTuhuString(7);

                    caPowers.Add(caPower);
                }
            }

            return caPowers;
        }

        /// <summary>
        /// 根据账号获取部门信息 超级管理员获取所有
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public DataTable GetDeptInfo(SqlConnection sqlconnection, string userNo)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@userNo", userNo));
            DataTable dt = SqlHelper.ExecuteDataset(sqlconnection, CommandType.StoredProcedure, "Home_RoleManage_GetDeptInfo", parameters.ToArray()).Tables[0];
            return dt;
        }

        /// <summary>
        /// 插入角色信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="roleName"></param>
        /// <param name="remark"></param>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public int InsertRole(SqlConnection sqlconnection, string roleName, string remark, int deptId, string pid, string cid)
        {
            string sql = "Insert into HrRoles(Name,Remark,GroupID,ParentID,ChildID)values(@Name,@Remark,@GroupID,@ParentID,@ChildID)";
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@Name", roleName));
            list.Add(new SqlParameter("@Remark", remark));
            list.Add(new SqlParameter("@GroupID", deptId.ToString()));
            list.Add(new SqlParameter("@ParentID", pid));
            list.Add(new SqlParameter("@ChildID", cid));
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, list.ToArray());
        }

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public DataTable GetRole(SqlConnection sqlconnection, int id, int flag, int deptid)
        {
            string sql = "select r.PKID,r.Name as RoleName,g.Name as GroupName,r.Remark,r.ParentID,r.ChildID from HrRoles r,HrGroup g where r.GroupID=cast(g.PKID as nvarchar(50)) and g.PKID=@PKID";
            if (flag == 2)
                sql = "select r.PKID,r.Name as RoleName,e.DepartmentName as GroupName from HrRoles r,HrEmployeeRole er,HrEmployee e " +
                      "where r.PKID=er.RoleID and er.EmployeeID=e.PKID AND e.PKID=@PKID and e.IsActive = 1 and r.GroupID=@GroupID";
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@PKID", id));
            if (flag == 2)
                list.Add(new SqlParameter("@GroupID", deptid.ToString()));
            return SqlHelper.ExecuteDataset(sqlconnection, CommandType.Text, sql, list.ToArray()).Tables[0];
        }

        /// <summary>
        /// 获取职员信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public DataTable GetEmployee(SqlConnection sqlconnection, int deptId, int roleId, string emailAddress)
        {
            var where = "";
            var sql = @"SELECT  e.PKID ,
                                e.EmailAddress ,
                                e.EmployeeName ,
                                ( SELECT    r.PKID
                                  FROM      HrRoles r WITH ( NOLOCK ) ,
                                            HrEmployeeRole er WITH ( NOLOCK )
                                  WHERE     er.RoleID = r.PKID
                                            AND r.PKID = @PKID
                                            AND er.EmployeeID = e.PKID
                                ) AS RoleID ,
                                e.HeadImg ,
                                e.Department ,
                                e.DepartmentName
                        FROM    HrEmployee e WITH ( NOLOCK )
                        WHERE   e.IsActive = 1
                                AND ( CHARINDEX(',' + @Department + ',', ',' + e.Department + ',') > 0
                                      OR EXISTS ( SELECT    1
                                                  FROM      HrGroup g WITH ( NOLOCK )
                                                  WHERE     g.Parent = @PID
                                                            AND CHARINDEX(','
                                                                          + CAST(g.PKID AS VARCHAR(20))
                                                                          + ',',
                                                                          ',' + e.Department + ',') > 0 ))  {0}";
            var list = new List<SqlParameter>();
            list.Add(new SqlParameter("@PKID", roleId));
            list.Add(new SqlParameter("@Department", deptId.ToString()));
            list.Add(new SqlParameter("@PID", deptId));
            if (!string.IsNullOrEmpty(emailAddress))
            {
                where = " and e.EmailAddress like '%'+@EmailAddress+'%'";
                list.Add(new SqlParameter("@EmailAddress", emailAddress));
            }
            sql = string.Format(sql, where);
            return SqlHelper.ExecuteDataset(sqlconnection, CommandType.Text, sql, list.ToArray()).Tables[0];
        }


        /// <summary>
        /// 采购查货组人员的邮箱地址
        /// </summary>
        /// <param name="sqlconnection"></param>     
        /// <returns></returns>
        public static List<PurchaseEmplpyee> GetPurchaseEmplpyeeEmail(SqlConnection sqlconnection)
        {
            string sql = @"SELECT 
                            e.EmailAddress
                            FROM  
                            HrRoles r with(nolock),HrEmployeeRole er with(nolock),HrEmployee e with(nolock)
                            WHERE er.RoleID = r.PKID and (r.Name like  N'%查货组采购人员%' OR r.Name like  N'%查货组管理人员%')
                            and e.pkid = er.employeeid
                            and e.IsActive=1";

            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql).ConvertTo<PurchaseEmplpyee>().ToList();
        }

        /// <summary>
        /// 插入或删除用户角色
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="employeeId"></param>
        /// <param name="roleId"></param>
        /// <param name="insOrDel"></param>
        /// <returns></returns>
        public int InsertOrDeleteEr(SqlConnection sqlconnection, int employeeId, int roleId, string insOrDel)
        {
            string sql = "";
            if (insOrDel.ToLower() == "ins")
                sql = "insert into HrEmployeeRole(EmployeeID,RoleID)values(@EmployeeID,@RoleID)";
            else
                sql = "delete from HrEmployeeRole where EmployeeID=@EmployeeID and RoleID=@RoleID";
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@EmployeeID", employeeId));
            list.Add(new SqlParameter("@RoleID", roleId));
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, list.ToArray());
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int DelRole(SqlConnection sqlconnection, int roleId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@roleId", roleId));
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.StoredProcedure, "Home_DelRole_Role", parameters.ToArray());
        }

        /// <summary>
        /// 根据编码获取一级部门
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public DataSet GetFirstDepartment(SqlConnection sqlconnection, string userNo, byte isSupper)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@userNo", userNo));
            list.Add(new SqlParameter("@isSupper", isSupper));
            return SqlHelper.ExecuteDataset(sqlconnection, CommandType.StoredProcedure, "Home_RolePower_GetDeptRole", list.ToArray());
        }

        /// <summary>
        /// 获取部门或角色ACTION
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="id"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public List<ActionPower> GetDeptOrRoleAction(SqlConnection sqlconnection, int id, byte flag, int deptid)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@ID", id));
            list.Add(new SqlParameter("@Flag", flag));
            list.Add(new SqlParameter("@DeptID", deptid));
            DataSet ds = SqlHelper.ExecuteDataset(sqlconnection, CommandType.StoredProcedure, "Home_RolePower_GetDeptOrRoleActions", list.ToArray());
            return ds.Tables[0].ConvertTo<ActionPower>().ToList();
        }

        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="roleid"></param>
        /// <param name="deptid"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public int SaveRolePower(SqlConnection sqlconnection, int roleid, int deptid, string actions)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@RoleID", roleid));
            list.Add(new SqlParameter("@DeptID", deptid));
            list.Add(new SqlParameter("@Actions", actions));
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.StoredProcedure, "Home_SaveRolePower_Save", list.ToArray());
        }

        /// <summary>
        /// 保存用户权限
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="roleid"></param>
        /// <param name="deptid"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public int SavePersonPower(SqlConnection sqlconnection, int employeeid, int deptid, string actions)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@EmployeeID", employeeid));
            list.Add(new SqlParameter("@DeptID", deptid));
            list.Add(new SqlParameter("@Actions", actions));
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.StoredProcedure, "Home_SavePersonPower_Save", list.ToArray());
        }

        public int AddModuleInf(SqlConnection sqlconnection, string xmlData)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@XmlData", xmlData));
            //parameters.Add(new SqlParameter("@ParentID", ParentID));
            //parameters.Add(new SqlParameter("@LinkName", LinkName));
            //parameters.Add(new SqlParameter("@controller", controller));
            //parameters.Add(new SqlParameter("@action", action));
            //parameters.Add(new SqlParameter("@ParametersName", ParametersName));
            //parameters.Add(new SqlParameter("@ParametersValue", ParametersValue));
            //parameters.Add(new SqlParameter("@OrderBy", OrderBy));
            //parameters.Add(new SqlParameter("@type", type));
            //parameters.Add(new SqlParameter("@IsActive", IsActive));
            //parameters.Add(new SqlParameter("@BtnKey", BtnKey));
            //parameters.Add(new SqlParameter("@BtnType", BtnType));
            //parameters.Add(new SqlParameter("@Remark", Remark));
            //parameters.Add(new SqlParameter("@GroupID", GroupID));
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.StoredProcedure, "Home_InsertModuleInf_V2", parameters.ToArray());
        }
        public List<ActionPower> SelectHrAccessList_HrGroupModule(SqlConnection connection)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@LinkName", "-1"));
            parameters.Add(new SqlParameter("@PageNumber", -1));
            DataTable dt = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "Home_SelectHrAccessList_HrGroupModule", parameters.ToArray()).Tables[0];
            return dt.ConvertTo<ActionPower>().ToList();
        }
        public List<ActionPower> SelectHrAccessList_HrGroupModule(SqlConnection connection, string linkName, int PageNumber)
        {
            SqlParameter[] parameters =
            {
                 new SqlParameter("@linkName", string.IsNullOrEmpty(linkName) ? "-1" : linkName),
                 new SqlParameter("@PageNumber", PageNumber)
            };
            DataTable dt = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "Home_SelectHrAccessList_HrGroupModule", parameters).Tables[0];
            return dt.ConvertTo<ActionPower>().ToList();
        }
        public int GetTotalModuleManagerNumber(SqlConnection connection, string linkName)
        {
            string temp = "";
            if (!string.IsNullOrEmpty(linkName))
                temp = " and LinkName like N'%" + linkName + "%'";
            string sql = "select count(*) as A  from dbo.HrAccessList  where IsActive=1" + temp + "";
            return Convert.ToInt32(SqlHelper.ExecuteScalar(connection, CommandType.Text, sql));
        }
        /// <summary>
        /// 删除Module相关信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteModule(SqlConnection sqlconnection, int id)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", id));
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.StoredProcedure, "Home_DeleteModuleInf", parameters.ToArray());
        }
        /// <summary>
        /// 更新Module相关信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="ParentID"></param>
        /// <param name="LinkName"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="ParametersName"></param>
        /// <param name="ParametersValue"></param>
        /// <param name="OrderBy"></param>
        /// <param name="type"></param>
        /// <param name="IsActive"></param>
        /// <param name="BtnKey"></param>
        /// <param name="BtnType"></param>
        /// <param name="Remark"></param>
        /// <returns></returns>
        public int UpdateModuleInf(SqlConnection sqlconnection, int id, string xmlData)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@PKID", id));
            parameters.Add(new SqlParameter("@XmlData", xmlData));
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.StoredProcedure, "Home_UpadateHrAccessList", parameters.ToArray());
        }
        public int ChangeDeptModule(SqlConnection sqlconnection, int id, string GroupIds)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ID", id));
            parameters.Add(new SqlParameter("@DeptIds", GroupIds));
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.StoredProcedure, "Home_UpdateModule_ChangeDeptModule", parameters.ToArray());
        }

        public DataTable GetModuleGroupID(SqlConnection sqlconnection, int moduleid)
        {
            string sql = "select GroupID from HrGroupModule where ModuleID=@ModuleID";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ModuleID", moduleid));
            return SqlHelper.ExecuteDataset(sqlconnection, CommandType.Text, sql, parameters.ToArray()).Tables[0];
        }

        public static DataTable GetWorkFlowInfo(SqlConnection sqlconnection)
        {
            string sql = "select * from fa_View_Config";
            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql);
        }

        /// <summary>
        /// 保存工作
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="name">工作名称</param>
        /// <param name="pid">父ID</param>
        /// <param name="remark">备注</param>
        public static void SaveWork(SqlConnection sqlconnection, string name, string pid, string remark)
        {
            string sql = "insert into fa_Work(WorkName,ParentID,Remark,IsActive)values(@WorkName,@ParentID,@Remark,1)";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@WorkName", name));
            parameters.Add(new SqlParameter("@ParentID", pid));
            parameters.Add(new SqlParameter("@Remark", remark));
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 删除工作
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="pkid">主键值</param>
        public static void DelWork(SqlConnection sqlconnection, int pkid)
        {
            string sql = "update fa_Work with(rowlock) set IsActive = 0 where PKID=@ID;";
            sql += "update fa_Work with(rowlock) set IsActive = 0 where ParentID=@ID;";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ID", pkid));
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 更新任务信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="name">任务名称</param>
        /// <param name="pid">父ID</param>
        /// <param name="remark">备注</param>
        /// <param name="id">pkid</param>
        public static void UptWork(SqlConnection sqlconnection, string name, string pid, string remark, int id)
        {
            string[] strs = name.Split('|');
            name = strs[0];
            string sql = "update fa_work with(rowlock) set WorkName=@WorkName,ParentID=@ParentID,Remark=@Remark {0} where PKID=@ID";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@WorkName", name));
            parameters.Add(new SqlParameter("@ParentID", pid));
            parameters.Add(new SqlParameter("@Remark", remark));
            parameters.Add(new SqlParameter("@ID", id));
            string condition = "";
            if (!string.IsNullOrEmpty(strs[1]))
            {
                condition += "," + "SearchItems=@SearchItems";
                parameters.Add(new SqlParameter("@SearchItems", strs[1]));
            }
            if (!string.IsNullOrEmpty(strs[2]))
            {
                condition += "," + "SearchSql=@SearchSql";
                parameters.Add(new SqlParameter("@SearchSql", strs[2]));
            }
            if (!string.IsNullOrEmpty(strs[3]))
            {
                condition += "," + "AppProcedure=@AppProcedure";
                parameters.Add(new SqlParameter("@AppProcedure", strs[3]));
            }
            if (!string.IsNullOrEmpty(strs[4]))
            {
                condition += "," + "IsOpenSend=@IsOpenSend";
                parameters.Add(new SqlParameter("@IsOpenSend", Convert.ToByte(strs[4])));
            }
            if (!string.IsNullOrEmpty(strs[5]))
            {
                condition += "," + "RolePrefix=@RolePrefix";
                parameters.Add(new SqlParameter("@RolePrefix", strs[5]));
            }
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, string.Format(sql, condition), parameters.ToArray());
        }

        /// <summary>
        /// 获取所有用户账号
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <returns>数据集</returns>
        public static DataTable GetAllUsers(SqlConnection sqlconnection)
        {
            string sql = "select distinct(EmailAddress) from Gungnir.dbo.HrEmployee with(nolock) where IsActive=1 order by EmailAddress";
            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql);
        }

        /// <summary>
        /// 获取任务权限
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="id">任务ID</param>
        /// <returns>字符串</returns>
        public static string GetWorkPowerUsers(SqlConnection sqlconnection, int id)
        {
            string sql = "select * from fa_WorkUser with(nolock) where WorkID=@WorkID";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@WorkID", id));
            DataTable dt = SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, parameters.ToArray());
            string result = "";
            foreach (DataRow dr in dt.Rows)
                result += dr["UserNo"].ToString() + ",";
            return result;
        }

        /// <summary>
        /// 存储权限
        /// </summary>
        /// <param name="sqlconnetion"></param>
        /// <param name="pus">权限用户</param>
        /// <param name="keyid">id</param>
        /// <param name="keytype">类别 1任务 2环节 3按钮</param>
        public static void SavePower(SqlConnection sqlconnetion, string pus, string keyid, int keytype)
        {
            pus = pus.TrimEnd(',');
            string sql = "";
            if (keytype == 1)
            {
                sql = "delete from fa_WorkUser where WorkID=" + keyid + "\r";
                foreach (string userno in pus.Split(','))
                    sql += "insert into fa_WorkUser(WorkID,UserNo)values(" + keyid + ",'" + userno + "')\r";
            }
            else if (keytype == 2)
            {
                sql = "delete from fa_StepUser where StepID=" + keyid + "\r";
                foreach (string userno in pus.Split(','))
                    sql += "insert into fa_StepUser(StepID,UserNo)values(" + keyid + ",'" + userno + "')\r";
            }
            else if (keytype == 3)
            {
                sql = "delete from fa_BtnUser where SFBtnPowerID=" + keyid + "\r";
                foreach (string userno in pus.Split(','))
                    sql += "insert into fa_BtnUser(SFBtnPowerID,UserNo)values(" + keyid + ",'" + userno + "')\r";
            }
            SqlHelper.ExecuteNonQuery(sqlconnetion, CommandType.Text, sql);
        }

        /// <summary>
        /// 保存环节
        /// </summary>
        /// <param name="sqlconnetion"></param>
        /// <param name="name">环节名称</param>
        /// <param name="pname">执行存储过程名称</param>
        /// <param name="isend">是否末环节</param>
        /// <param name="workid">任务ID</param>
        public static void SaveStep(SqlConnection sqlconnection, string name, string pname, int isend, int workid)
        {
            string sql = "insert into fa_step(WorkID,StepName,IsActive,StepOrder,ProcedureName,IsFinish,AccessPolicy) " +
                         "select @WorkID,@StepName,1,ISNULL(MAX(StepOrder),0)+1,@ProcedureName,@IsFinish,@AccessPolicy from fa_Step where WorkID=@WorkID and IsActive=1";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@WorkID", workid));
            parameters.Add(new SqlParameter("@StepName", name));
            parameters.Add(new SqlParameter("@ProcedureName", pname.Split('|')[0]));
            parameters.Add(new SqlParameter("@IsFinish", isend));
            parameters.Add(new SqlParameter("@AccessPolicy", pname.Split('|')[1]));
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 删除环节
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="pkid">主键值</param>
        public static void DelStep(SqlConnection sqlconnection, int pkid)
        {
            string sql = "update fa_Step with(rowlock) set IsActive = 0 where PKID=@ID";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ID", pkid));
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 更新环节
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="name">环节名称</param>
        /// <param name="pname">存储过程名称</param>
        /// <param name="isend">是否末节点</param>
        /// <param name="id">PKID</param>
        public static void UptStep(SqlConnection sqlconnection, string name, string pname, int isend, int id)
        {
            string sql = "update fa_step with(rowlock) set StepName=@StepName,ProcedureName=@ProcedureName,IsFinish=@IsFinish,AccessPolicy=@AccessPolicy where PKID=@ID";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ID", id));
            parameters.Add(new SqlParameter("@StepName", name));
            parameters.Add(new SqlParameter("@ProcedureName", pname.Split('|')[0]));
            parameters.Add(new SqlParameter("@IsFinish", isend));
            parameters.Add(new SqlParameter("@AccessPolicy", pname.Split('|')[1]));
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 获取环节权限
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="id">环节ID</param>
        /// <returns>字符串</returns>
        public static string GetStepPowerUsers(SqlConnection sqlconnection, int id)
        {
            string sql = "select * from fa_StepUser with(nolock) where StepID=@StepID";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@StepID", id));
            DataTable dt = SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, parameters.ToArray());
            string result = "";
            foreach (DataRow dr in dt.Rows)
                result += dr["UserNo"].ToString() + ",";
            return result;
        }

        /// <summary>
        /// 获取环节表单权限
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="id">环节ID</param>
        /// <returns>字符串</returns>
        public static string GetStepPowerForms(SqlConnection sqlconnection, int id)
        {
            string sql = "select * from fa_StepForm with(nolock) where StepID=@StepID";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@StepID", id));
            DataTable dt = SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, parameters.ToArray());
            string result = "";
            foreach (DataRow dr in dt.Rows)
                result += dr["FormID"].ToString() + ",";
            return result;
        }

        /// <summary>
        /// 获取所有表单
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <returns>数据集</returns>
        public static DataTable GetAllForms(SqlConnection sqlconnection)
        {
            string sql = "select cast(SystemCode as nvarchar(10))+'.'+FormName+'('+FormUrl+')' as FormInfo,PKID from fa_Form with(nolock) order by SystemCode,PKID";
            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql);
        }

        /// <summary>
        /// 存储表单权限
        /// </summary>
        /// <param name="sqlconnetion"></param>
        /// <param name="pus">权限表单</param>
        /// <param name="keyid">id</param>
        /// <param name="keytype">类别 0环节</param>
        public static void saveFormPower(SqlConnection sqlconnetion, string forms, string keyid, int keytype)
        {
            forms = forms.TrimEnd(',');
            string sql = "";
            if (keytype == 0)
            {
                sql = "delete from fa_StepForm where StepID=" + keyid + ";";
                foreach (string form in forms.Split(','))
                    sql += "insert into fa_StepForm(StepID,FormID)values(" + keyid + "," + form + ");";
            }
            SqlHelper.ExecuteNonQuery(sqlconnetion, CommandType.Text, sql);
        }

        /// <summary>
        /// 保存环节
        /// </summary>
        /// <param name="sqlconnetion"></param>
        /// <param name="fname">表单名称</param>
        /// <param name="furl">表单路径</param>
        /// <param name="gno">组号</param>
        /// <param name="stepid">环节ID</param>
        public static void SaveForm(SqlConnection sqlconnection, string fname, string furl, int gno, int stepid)
        {
            string sql = "insert into fa_Form(FormName,FormUrl,SystemCode)values(@FormName,@FormUrl,@SystemCode);";
            sql += "delete from fa_StepForm where StepID=@StepID and FormID=SCOPE_IDENTITY();";
            sql += "insert into fa_StepForm(StepID,FormID) select @StepID,SCOPE_IDENTITY();";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@FormName", fname));
            parameters.Add(new SqlParameter("@FormUrl", furl));
            parameters.Add(new SqlParameter("@SystemCode", gno));
            parameters.Add(new SqlParameter("@StepID", stepid));
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 删除步骤表单权限
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="sfid">步骤表单ID串</param>
        public static void DelStepForm(SqlConnection sqlconnection, string sfid)
        {
            int stepid = Convert.ToInt32(sfid.Split('-')[0]);
            int formid = Convert.ToInt32(sfid.Split('-')[1]);
            string sql = "delete from fa_StepForm where StepID=@StepID and FormID=@FormID";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@StepID", stepid));
            parameters.Add(new SqlParameter("@FormID", formid));
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        public static void UptForm(SqlConnection sqlconnection, string fname, string furl, int gno, int formid)
        {
            string sql = "update fa_Form with(rowlock) set FormName=@FormName,FormUrl=@FormUrl,SystemCode=@SystemCode where PKID=@PKID";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@FormName", fname));
            parameters.Add(new SqlParameter("@FormUrl", furl));
            parameters.Add(new SqlParameter("@SystemCode", gno));
            parameters.Add(new SqlParameter("@PKID", formid));
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 存储按钮
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="BtnKey">按钮key</param>
        /// <param name="BtnType">按钮类别</param>
        /// <param name="SFID">步骤、表单ID串</param>
        public static void SaveBtn(SqlConnection sqlconnection, string BtnKey, int BtnType, string sfid)
        {
            int stepid = Convert.ToInt32(sfid.Split('-')[0]);
            int formid = Convert.ToInt32(sfid.Split('-')[1]);
            string sql = "insert into fa_SFBtnPower(StepID,FormID,BtnKey,BtnType)values(@StepID,@FormID,@BtnKey,@BtnType)";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@StepID", stepid));
            parameters.Add(new SqlParameter("@FormID", formid));
            parameters.Add(new SqlParameter("@BtnKey", BtnKey));
            parameters.Add(new SqlParameter("@BtnType", BtnType));
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 删除步骤表单按钮
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="id">按钮ID</param>
        public static void DelBtn(SqlConnection sqlconnection, int id)
        {
            string sql = "delete from fa_BtnUser where SFBtnPowerID=@ID;" +
                         "delete from fa_SFBtnPower where PKID=@ID;";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ID", id));
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 获取环节表单按钮权限
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="id">按钮ID</param>
        /// <returns>字符串</returns>
        public static string GetBtnPowerUsers(SqlConnection sqlconnection, int id)
        {
            string sql = "select * from fa_BtnUser with(nolock) where SFBtnPowerID=@SFBtnPowerID";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@SFBtnPowerID", id));
            DataTable dt = SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, parameters.ToArray());
            string result = "";
            foreach (DataRow dr in dt.Rows)
                result += dr["UserNo"].ToString() + ",";
            return result;
        }

        public static DataTable GetNewWorkInfo(SqlConnection sqlconnection, int id)
        {
            string sql = "select * from fa_Work with(nolock) where PKID=@ID";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ID", id));
            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 权限
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="userNo"></param>
        /// <param name="issupper"></param>
        /// <returns></returns>
        public List<ActionPower> GetBusPower(SqlConnection sqlconnection, string userNo, byte issupper)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@userNo", userNo));
            parameters.Add(new SqlParameter("@isSupper", issupper));
            DataTable dt = SqlHelper.ExecuteDataset(sqlconnection, CommandType.StoredProcedure, "Home_Power_GetBusActionList", parameters.ToArray()).Tables[0];
            return dt.ConvertTo<ActionPower>().ToList();
        }

        public static bool IsAccessPower(SqlConnection sqlconnection, string userno, int accessid)
        {
            string sql = "select count(1) as num from(select distinct EmailAddress from HrEmployee a with(nolock),HrEmployeeAccess b with(nolock),HrAccessList c with(nolock) " +
           "where a.PKID=b.EmployeeID and b.AccessID=c.PKID " +
           "and a.IsActive=1 and c.IsActive=1 and c.PKID=@AccessId " +
           "UNION ALL " +
           "select distinct EmailAddress from HrEmployee a with(nolock),HrEmployeeRole b with(nolock),HrRoles c with(nolock),HrRoleAccess d with(nolock),HrAccessList e with(nolock) " +
           "where a.PKID=b.EmployeeID and b.RoleID=c.PKID AND c.PKID = d.RoleID and d.AccessID=e.PKID " +
           "and a.IsActive=1 and e.IsActive=1 AND e.PKID=@AccessId) a where EmailAddress=@UserNo";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@AccessId", accessid));
            parameters.Add(new SqlParameter("@UserNo", userno));
            int i = Convert.ToInt32(SqlHelper.ExecuteScalar(sqlconnection, CommandType.Text, sql, parameters.ToArray()));
            return (i >= 1);
        }

        /// <summary>
        /// 修改角色名称
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="rolename"></param>
        /// <param name="id"></param>
        public static void ModifyRoleNameByID(SqlConnection sqlconnection, string rolename, int id)
        {
            string sql = "update HrRoles set Name=@Name where PKID=@PKID";
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@Name", rolename);
            parameters[1] = new SqlParameter("@PKID", id);
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters);
        }
        /// <summary>
        /// 获取用户的按钮权限
        /// </summary>
        /// <returns></returns>
        public static List<ActionPower> GetUserButtonPermission(SqlConnection conn, string controller, string action, string emailAddress)
        {
            const string commandText = @"SELECT  HA.BtnKey
                                                                FROM    dbo.HrAccessList AS HA WITH ( NOLOCK )
                                                                INNER JOIN dbo.HrEmployeeAccess AS HEA WITH ( NOLOCK )
                                                                ON      HEA.AccessID = HA.PKID
                                                                INNER JOIN dbo.HrEmployee AS HE WITH ( NOLOCK )
                                                                ON      HE.PKID = HEA.EmployeeID AND HE.EmailAddress=@EmailAddress
                                                                WHERE   HA.[Type] = 'Function'
                                                                        AND HA.Controller = @Controller
                                                                        AND HA.[Action] = @Action
                                                                        AND HA.IsActive = 1";
            var buttonPermission = new List<ActionPower>();
            var parameters = new[]
            {
                new SqlParameter("@Controller", controller),
                new SqlParameter("@Action", action),
                new SqlParameter("@EmailAddress", emailAddress),
            };
            using (var reader = SqlHelper.ExecuteReaderV2(conn, CommandType.Text, commandText, parameters))
            {
                while (reader.Read())
                {
                    var permission = new ActionPower()
                    {
                        BtnKey = reader.GetTuhuString(0)
                    };
                    buttonPermission.Add(permission);
                }
            }
            return buttonPermission;
        }

        /// <summary>
        /// 获取菜单信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <returns></returns>
        public static DataSet GetMenuInfo(SqlConnection sqlconnection)
        {
            string sql = "select LinkName,PKID from HrAccessList hal with(nolock) where IsActive=1 and ParentID=0 and NOT EXISTS (select 1 from HrCategoryAccesslist hcal with(nolock) where AccessID=hal.PKID);";
            sql += "select CategoryName,PKID from HrCategory hc with(nolock) order by pkid;";
            sql += "select hal.LinkName,hal.PKID,hcal.CategoryID from HrAccessList hal with(nolock) INNER JOIN HrCategoryAccesslist hcal with(nolock) ON hal.PKID =  hcal.AccessID where hal.IsActive=1 and hal.ParentID=0";
            return SqlHelper.ExecuteDataset(sqlconnection, CommandType.Text, sql);
        }

        /// <summary>
        /// 关联或解除关联
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="cid"></param>
        /// <param name="aid"></param>
        /// <param name="flag"></param>
        public static void InsertOrDelMenuRelation(SqlConnection sqlconnection, int cid, int aid, int flag)
        {
            string sql = "";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@CategoryID", cid));
            parameters.Add(new SqlParameter("@AccessID", aid));
            if (flag == 1)
            {
                sql = "insert into HrCategoryAccesslist(CategoryID,AccessID) select @CategoryID,@AccessID where not exists (select 1 from HrCategoryAccesslist with(nolock) where CategoryID = @CategoryID and AccessID=@AccessID)";
            }
            else
                sql = "delete from HrCategoryAccesslist where CategoryID = @CategoryID and AccessID=@AccessID";
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 添加一级菜单
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="cname"></param>
        /// <param name="iname"></param>
        public static void AddMenu(SqlConnection sqlconnection, string cname, string iname, string createdBy = "admin")
        {
            string sql = "insert into HrCategory(CategoryName,Icon,CreatedBy)values(@CategoryName,@Icon,@CreatedBy)";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@CategoryName", cname));
            parameters.Add(new SqlParameter("@Icon", iname));
            parameters.Add(new SqlParameter("@CreatedBy", createdBy));
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, parameters.ToArray());
        }

        /// <summary>
        /// 增加用户入角色
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="userEmail">账号</param>
        /// <param name="roles">角色</param>
        public static void AddDefaultEr(SqlConnection sqlconnection, string userEmail, string roles)
        {
            roles = roles.TrimEnd(',');
            SqlParameter[] paramters = new SqlParameter[2];
            paramters[0] = new SqlParameter("@UserEmail", userEmail);
            paramters[1] = new SqlParameter("@Roles", roles);
            SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.StoredProcedure, "Accoungting_AddDefaultEr", paramters);
        }
    }

    public class PurchaseEmplpyee
    {
        public string EmailAddress { get; set; }
    }

}
