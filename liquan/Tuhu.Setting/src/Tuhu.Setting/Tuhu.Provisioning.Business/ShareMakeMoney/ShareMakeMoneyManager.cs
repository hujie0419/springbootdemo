using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;
using System.Data;
using System.Data.SqlClient;

namespace Tuhu.Provisioning.Business.ShareMakeMoney
{

    public class ShareMakeMoneyManager
    {
        

        public bool Save(SE_ShareMakeMoneyConfig model,string userName)
        {
            DALShareMakeMoneyConfig dal = new DALShareMakeMoneyConfig();
            if (model.ID == 0)
            {
                model.CreateUserName = userName;
                model.UpdateUserName = userName;
                int id = 0;
                return dal.Add(model,out id);
            }
            else
            {
                model.UpdateUserName = userName;
                return dal.Update(model);
            }
        }

        public int AddProducts(IEnumerable<SE_ShareMakeImportProducts> models,int id)
        {
            DALShareMakeMoneyConfig dal = new DALShareMakeMoneyConfig();
            return dal.AddProducts(models,id);
        }

        public bool DeleteProduct(int id)
        {
            DALShareMakeMoneyConfig dal = new DALShareMakeMoneyConfig();
            return dal.DeleteProduct(id);
        }


        public SE_ShareMakeMoneyConfig GetEntity(int id)
        {
            DALShareMakeMoneyConfig dal = new DALShareMakeMoneyConfig();
            return dal.GetEntity(id);
        }

        public IEnumerable<SE_ShareMakeImportProducts> GetProductEntities(int id ,int pageIndex,int pageSize,string datetime,string pid,string displayName,string times,string status,out int total)
        {
            DALShareMakeMoneyConfig dal = new DALShareMakeMoneyConfig();
            List<SqlParameter> parameters = new List<SqlParameter>();
            StringBuilder whereString = new StringBuilder();
            whereString.Append(" and  FKID=@FKID ");
            parameters.Add(new SqlParameter()
            {
                ParameterName = "@FKID",
                Value = id
            });
            if (!string.IsNullOrWhiteSpace(datetime))
            {
                DateTime date = Convert.ToDateTime(datetime);
                if (date != null)
                {
                    whereString.Append(" and CreateDate >= @CreateDateStart and CreateDate <=@CreateDateEnd ");
                    parameters.Add(new SqlParameter("@CreateDateStart", date.ToString("yyyy-MM-dd 00:00:00")));
                    parameters.Add(new SqlParameter("@CreateDateEnd", date.ToString("yyyy-MM-dd 23:59:59")));
                }
            }

            if (!string.IsNullOrWhiteSpace(pid))
            {
                whereString.Append(" and PID=@PID ");
                parameters.Add(new SqlParameter("@PID", pid));
            }
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                whereString.Append(string.Format("  and   DisplayName LIKE N'%{0}%' ", displayName));
            }

            if (!string.IsNullOrWhiteSpace(times))
            {
                whereString.Append(" and Times=@Times ");
                parameters.Add(new SqlParameter("@Times", times));
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                whereString.Append(" and IsMakeMoney=@IsMakeMoney");
                parameters.Add(new SqlParameter("@IsMakeMoney", Convert.ToInt32(status)));
            }
            return dal.GetProductEntities( pageIndex, pageSize, whereString.ToString(),parameters.ToArray(),  out total);
        }

        public IEnumerable<SE_ShareMakeImportProducts> GetProductEntities(int id)
        {
            DALShareMakeMoneyConfig dal = new DALShareMakeMoneyConfig();
            return dal.GetProductEntities(id);
        }

        public IEnumerable<SE_ShareMakeImportProducts> GetProductEntities(string datetime, string pid, string displayName, string times, string status,int id)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            StringBuilder whereString = new StringBuilder();
            whereString.Append(" and  FKID=@FKID ");
            parameters.Add(new SqlParameter()
            {
                 ParameterName="@FKID",
                 Value=id
            });
            if (!string.IsNullOrWhiteSpace(datetime))
            {
                DateTime date = Convert.ToDateTime(datetime);
                if (date != null)
                {
                    whereString.Append(" and CreateDate >= @CreateDateStart and CreateDate <=@CreateDateEnd ");
                    parameters.Add(new SqlParameter("@CreateDateStart", date.ToString("yyyy-MM-dd 00:00:00")));
                    parameters.Add(new SqlParameter("@CreateDateEnd", date.ToString("yyyy-MM-dd 23:59:59")));
                }
            }

            if (!string.IsNullOrWhiteSpace(pid))
            {
                whereString.Append(" and PID=@PID ");
                parameters.Add(new SqlParameter("@PID",pid));
            }
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                whereString.Append(string.Format("  and   DisplayName LIKE N'%{0}%' ",displayName));
            }

            if (!string.IsNullOrWhiteSpace(times))
            {
                whereString.Append(" and Times=@Times ");
                parameters.Add(new SqlParameter("@Times", times));
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                whereString.Append(" and IsMakeMoney=@IsMakeMoney");
                parameters.Add(new SqlParameter("@IsMakeMoney", Convert.ToInt32(status)));
            }
            DALShareMakeMoneyConfig dal = new DALShareMakeMoneyConfig();
            return dal.GetProductEntities(whereString.ToString(), parameters.ToArray());
        }

        public bool Delete(int id) => new DALShareMakeMoneyConfig().Delete(id);

        public bool UpdateProductIsShare(int id, string isShare)
        {
            return new DALShareMakeMoneyConfig().UpdateProductIsShare(id, isShare);
        }


        public IEnumerable<SE_ShareMakeMoneyConfig> GetList() => new DALShareMakeMoneyConfig().GetList();

        public DataTable GetFenxiangzhuanqianProduct()
        {
            DALShareMakeMoneyConfig dal = new DALShareMakeMoneyConfig();
            return dal.GetFenxiangzhuanqianProduct();
        }

    }
}
