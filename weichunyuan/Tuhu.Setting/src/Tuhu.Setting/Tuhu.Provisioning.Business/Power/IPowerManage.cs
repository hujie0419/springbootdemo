using System.Collections.Generic;
using System.Data;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Power
{
    public interface IPowerManage
    {
        List<CAPower> GetPowerList(string userNo, byte issupper);

        DataTable GetDeptInfo(string userNo);

        int InsertRole(string roleName, string remark, int deptId, string pid = "", string cid = "");

        DataTable GetRole(int id, int flag, int deptid);

        DataTable GetEmployee(int deptId, int roleId, string emailAddress);

        int InsertOrDeleteEr(int employeeId, int roleId, string insOrDel);

        int DelRole(int roleId);

        DataSet GetFirstDepartment(string userNo, byte isSupper);

        List<ActionPower> GetDeptOrRoleAction(int id, byte flag, int deptid);

        int SaveRolePower(int roleid, int deptid, string actions);

        int SavePersonPower(int employeeid, int deptid, string actions);

        int AddModuleInf(string xmlData);

        int UpdateModuleInf(int id, string xmlData);

        DataTable GetModuleGroupID(int moduleid);

        DataTable GetWorkFlowInfo();

        void SaveWork(string name, string pid, string remark);

        void DelWork(int pkid);

        void UptWork(string name, string pid, string remark, int id);

        DataTable GetAllUsers();

        string GetWorkPowerUsers(int id);

        void SavePower(string pus, string keyid, int keytype);

        void SaveStep(string name, string pname, int isend, int workid);

        void DelStep(int id);

        void UptStep(string name, string pname, int isend, int id);

        string GetStepPowerUsers(int id);

        string GetStepPowerForms(int id);

        DataTable GetAllForms();

        void saveFormPower(string forms, string keyid, int keytype);

        void SaveForm(string fname, string furl, int gno, int stepid);

        void DelStepForm(string sfid);

        void UptForm(string fname, string furl, int gno, int formid);

        void SaveBtn(string BtnKey, int BtnType, string sfid);

        void DelBtn(int id);

        string GetBtnPowerUsers(int id);

        DataTable GetNewWorkInfo(int id);

        List<ActionPower> GetBusPower(string userNo, byte issupper);

        bool IsOprPower(List<ActionPower> list, string userno, string controller, string action, string btnKey);

        void ModifyRoleNameByID(string rolename, int id);

        void InsertOrDelMenuRelation(int cid, int aid, int flag);

        void AddMenu(string cname, string iname, string createdBy = "admin");

        void AddDefaultEr(string userEmail, string roles);
    }
}
