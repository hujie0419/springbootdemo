using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.CompanyClient
{
    public class CompanyClientManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;
        private static readonly ILog logger = LoggerFactory.GetLogger("CompanyClientManager");


        public CompanyClientManager()
        {
            dbScopeManager = new DBScopeManager(ConnectionManager);
            dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }

        public List<CompanyClientConfig> GetAllCompanyClientConfig(int pageIndex, int pageSize)
        {
            List<CompanyClientConfig> result = new List<CompanyClientConfig>();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALCompanyClient.SelectAllCompanyClientConfig(conn, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetAllCompanyClientConfig");
            }

            return result;
        }

        public CompanyClientConfig GetCompanyClientConfigByPkid(int pkid)
        {
            CompanyClientConfig result = new CompanyClientConfig();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALCompanyClient.SelectCompanyClientConfigByPkid(conn, pkid));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetAllCompanyClientConfig");
            }

            return result;
        }

        public List<ClientCouponCode> SelectCouponCodeByParentId(int parentId,int isBind)
        {
            List<ClientCouponCode> result = new List<ClientCouponCode>();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALCompanyClient.SelectCouponCodeByParentId(conn, parentId, isBind));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SelectCouponCodeByParentId");
            }

            return result;
        }

        public bool InsertCompanyClientConfig(string channel, string url, string user)
        {
            var result = false;

            try
            {
                var pkid = dbScopeManager.Execute(conn => DALCompanyClient.InsertCompanyClientConfig(conn, channel, url));
                result = pkid > 0;
                InsertLog("InsertCompanyClientConfig", pkid.ToString(), $"Channel:{channel},Url:{url}", result ? "添加成功" : "添加失败", user);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "InsertCompanyClientConfig");
            }

            return result;
        }

        public bool UpdateCompanyClientConfig(string channel, string url, int pkid, string user)
        {
            var result = false;

            try
            {
                result = dbScopeManager.Execute(conn => DALCompanyClient.UpdateCompanyClientConfig(conn, channel, url, pkid)) > 0;
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "UpdateCompanyClientConfig");
            }

            InsertLog("UpdateCompanyClientConfig", pkid.ToString(), $"Channel:{channel},Url:{url}", result ? "修改成功" : "修改失败", user);

            return result;
        }

        public bool DeletedCompanyClientConfig(int pkid, string user)
        {
            var result = false;
            CompanyClientConfig data = null;
            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    data = DALCompanyClient.SelectCompanyClientConfigByPkid(conn, pkid);
                    if (data != null)
                    {
                        DALCompanyClient.DeletedCompanyClientConfig(conn, pkid);
                        DALCompanyClient.DeletedCouponCodeByPkid(conn, pkid);
                        result = true;
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DeletedCompanyClientConfig");
            }

            InsertLog("DeletedCompanyClientConfig", pkid.ToString(), JsonConvert.SerializeObject(data), result ? "删除成功" : "删除失败", user);

            return result;
        }

        public bool DeletedCouponCodeByPkid(int pkid)
        {
            var result = false;

            try
            {
                dbScopeManager.Execute(conn =>
                {
                    result = DALCompanyClient.DeletedCouponCodeByPkid(conn, pkid) > 0;
                });
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DeletedCompanyClientConfig");
            }

            return result;
        }

        public bool GenerateCouponCode(string channel, int count, int parentId, string user)
        {
            var result = false;

            try
            {
                result = dbScopeManager.Execute(conn => DALCompanyClient.GenerateCouponCode(conn, count, parentId)) > 0;
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GenerateCouponCode");
            }

            InsertLog("GenerateCouponCode", parentId.ToString(), $"Channel:{channel},Count:{count},ParentId:{parentId}", result ? "生成成功" : "生成失败", user);

            return result;
        }

        public void InsertLog(string method, string objectId, string remarks, string msg, string opera)
        {
            try
            {
                CompanyClientConfigLog info = new CompanyClientConfigLog
                {
                    ObjectId = objectId,
                    Method = method,
                    Message = msg,
                    Remarks = remarks,
                    Operator = opera,
                    CreatedTime = DateTime.Now
                };
                LoggerManager.InsertLog("CompanyClientLog", info);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "ConfigLog");
            }
        }

        public List<CompanyClientConfigLog> SelectOperationLog(string objectId,string type)
        {
            List<CompanyClientConfigLog> result = new List<CompanyClientConfigLog>();
            try
            {
                result= DALCompanyClient.SelectOperationLog(objectId,type);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "ConfigLog");
            }
            return result;
        }
    }
}
