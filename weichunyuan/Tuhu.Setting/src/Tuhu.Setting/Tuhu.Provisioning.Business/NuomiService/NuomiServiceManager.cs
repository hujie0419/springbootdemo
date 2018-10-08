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
using Tuhu.Service.Shop;

namespace Tuhu.Provisioning.Business.NuomiService
{
    public class NuomiServiceManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;
        private static readonly ILog logger = LoggerFactory.GetLogger("NuomiServiceManager");

        private static string Email = ConfigurationManager.AppSettings["NuomiUserEmail"].ToString();

        public NuomiServiceManager()
        {
            dbScopeManager = new DBScopeManager(ConnectionManager);
            dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }

        public List<NuomiServicesConfig> GetNuomiServicesConfig(string nuomiTitle, long nuomiId, string serviceId,int isValid, int pageIndex, int pageSize)
        {
            List<NuomiServicesConfig> result = new List<NuomiServicesConfig>();

            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DALNuomiService.SelectNuomiServicesConfig(conn, nuomiTitle, nuomiId, serviceId, isValid, pageIndex, pageSize);
                });
                if (result != null && result.Any())
                {
                    var allShopServices = GetAllShopServices();
                    if (allShopServices != null && allShopServices.Any())
                    {
                        result.ForEach(x =>
                        {
                            x.ServiceName = allShopServices
                            .Where(o => String.Equals(o.Key, x.ServiceId))
                            .Select(i => i.Value).FirstOrDefault();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetNuomiServicesConfig");
            }

            return result;
        }

        public string GetServiceNameByServiceId(string serviceId)
        {
            var result = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(serviceId))
                {
                    var allServices = GetAllShopServices();
                    if (allServices != null && allServices.Any())
                    {
                        result = allServices.
                            Where(x => String.Equals(x.Key, serviceId))
                            .Select(o => o.Value).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetServiceNameByServiceId");
            }

            return result == null ? string.Empty : result;
        }

        public int AddNuomiServiceConfig(string nuomiTitle, long nuomiId, string serviceId, string userEmail, string remarks, string user)
        {
            var result = -1;

            try
            {
                dbScopeManager.Execute(conn =>
                {
                    if (DALNuomiService.SelectConfigByNuomiIdAndEmail(conn, nuomiId, userEmail) == null)
                    {
                        if (DALNuomiService.InsertNuomiServiceConfig(conn, nuomiTitle, nuomiId, serviceId, userEmail, remarks) > 0)
                        {
                            result = 1;
                        }
                    }
                    else
                    {
                        result = 2;
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "AddNuomiServiceConfig");
            }
            var remark = $"NuomiTitle:{nuomiTitle},NuomiId:{nuomiId},ServiceId：{serviceId},UserEmail:{userEmail},Remarks:{remarks},Result:{result}";
            InsertLog("AddNuomiServiceConfig", nuomiId.ToString(), remark, result==1 ? "添加成功" : "添加失败", user, "NuomiServicesConfig", "", "");
            return result;
        }

        public bool EditNuomiServiceConfig(int pkid, string nuomiTitle, long nuomiId, string serviceId, string userEmail, string remarks, string user)
        {
            var result = false;

            try
            {
                dbScopeManager.Execute(conn =>
                {
                    result = DALNuomiService.UpdateNuomiServiceConfig(conn, pkid, nuomiTitle, serviceId, remarks) > 0;
                });
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "EditNuomiServiceConfig");
            }
            var remark = $"PKID:{pkid}NuomiTitle:{nuomiTitle},NuomiId:{nuomiId},ServiceId：{serviceId},UserEmail:{userEmail},Remarks:{remarks}";
            InsertLog("EditNuomiServiceConfig", nuomiId.ToString(), remark, result ? "修改成功" : "修改失败", user, "NuomiServicesConfig", "", "");
            return result;
        }

        public bool DelNuomiServiceConfig(int pkid, string user)
        {
            var result = false;
            NuomiServicesConfig data = null;
            try
            {
                dbScopeManager.Execute(conn =>
                {
                    data = DALNuomiService.SelectConfigByPKID(conn, pkid);
                    if (data != null)
                    {
                        result = DALNuomiService.DelNuomiServiceConfig(conn, pkid) > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "DelNuomiServiceConfig");
            }
            InsertLog("DelNuomiServiceConfig", data != null ? data.NuomiId.ToString() : pkid.ToString(), JsonConvert.SerializeObject(data),
            result ? "删除成功" : "删除失败", user, "NuomiServicesConfig", "", "");
            return result;
        }

        public static IDictionary<string, string> GetAllShopServices()
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                using (var client = new ShopClient())
                {
                    var data = client.GetAllBeautyProductPidAndName();
                    if (data != null && data.Success)
                    {
                        result = data.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetAllShopServices");
            }

            return result;
        }

        public void InsertLog(string type, string nuomiId, string remarks, string msg, string opera, string operateType, string ipAddress, string hostName)
        {
            try
            {
                var info = new
                {
                    Type = type.ToString(),
                    NuomiId = nuomiId,
                    Remarks = remarks,
                    Message = msg,
                    Operator = opera,
                    OperateType = operateType.ToString(),
                    IPAddress = ipAddress,
                    HostName = hostName
                };
                LoggerManager.InsertLog("NuomiMeiRongConfig", info);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "ConfigLog");
            }
        }

        public bool BatchInsertNuomiConfig(List<NuomiServicesConfig> info, string user)
        {
            var result = false;
            try
            {
                dbScopeManager.Execute(conn => DALNuomiService.BatchInsertNuomiConfig(conn, info, Email));
                result = true;
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "BatchInsertNuomiConfig");
            }
            if (result)
            {
                InsertLog("BatchInsertNuomiConfig", "-1", JsonConvert.SerializeObject(info), "批量导入糯米团单配置", user, "NuomiServicesConfig", "", "");
            }
            return result;
        }

        public Tuple<List<NuomiServicesConfig>, List<NuomiServicesConfig>> VerifyData(List<NuomiServicesConfig> data, IDictionary<string, string> allServices)
        {
            var success = new List<NuomiServicesConfig>();
            var error = new List<NuomiServicesConfig>();
            var flag = false;
            var msg = string.Empty;
            var serviceName = string.Empty;
            try
            {
                if (data != null && data.Any() && allServices != null && allServices.Any())
                {
                    data = data.GroupBy(_ => _.NuomiId).Select(x => x.FirstOrDefault()).ToList();
                    dbScopeManager.Execute(conn =>
                    {
                        foreach (var item in data)
                        {
                            flag = false;
                            msg = string.Empty;
                            serviceName = string.Empty;
                            flag = DALNuomiService.SelectConfigByNuomiId(conn, item.NuomiId) == null ? true : false;
                            if (flag)
                            {
                                serviceName = allServices.
                                     Where(x => String.Equals(x.Key, item.ServiceId))
                                     .Select(o => o.Value).FirstOrDefault();
                                if (!string.IsNullOrEmpty(serviceName))
                                {
                                    success.Add(new NuomiServicesConfig()
                                    {
                                        NuomiId = item.NuomiId,
                                        NuomiTitle = item.NuomiTitle,
                                        ServiceId = item.ServiceId,
                                        Email = item.Email,
                                        Remarks = item.Remarks
                                    });
                                }
                                else
                                {
                                    flag = false;
                                    msg = "服务Id无效";
                                }
                            }
                            else
                            {
                                flag = false;
                                msg = "糯米Id已存在";
                            }
                            if (!flag)
                            {
                                error.Add(new NuomiServicesConfig()
                                {
                                    NuomiId = item.NuomiId,
                                    ServiceId = item.ServiceId,
                                    Remarks = msg
                                });
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "VerifyData");
            }

            return Tuple.Create(success, error);
        }

        public bool BatchSoleteConfig(string pkidStr,string user)
        {
            var result = false;

            try
            {
                result = dbScopeManager.Execute(conn => DALNuomiService.BatchSoleteConfig(conn, pkidStr)) > 0;
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "BatchSoleteConfig");
            }
            if (result)
            {
                InsertLog("BatchSoleteConfig", "-1", $"PkidStr:{pkidStr}", "批量作废糯米团单配置", user, "NuomiServicesConfig", "", "");
            }
            return result;
        }
    }
}
