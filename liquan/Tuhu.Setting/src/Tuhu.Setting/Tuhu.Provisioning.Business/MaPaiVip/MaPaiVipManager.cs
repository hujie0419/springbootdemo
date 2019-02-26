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

namespace Tuhu.Provisioning.Business.ContinentalConfig
{
    public class MaPaiVipManager
    {
        private static readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;
        private static readonly ILog logger = LoggerFactory.GetLogger("ContinentalConfigManager");

        public MaPaiVipManager()
        {
            dbScopeManager = new DBScopeManager(ConnectionManager);
            dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }

        /// <summary>
        /// 获取券码
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<MaPaiVipModel> GetContinentalActivityList(string keyword, int pageIndex, int pageSize)
        {
            var result = new List<MaPaiVipModel>();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALMaPaiVip.SelectContinentalActivityList(conn, keyword, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetContinentalActivityList");
            }

            return result;
        }

        /// <summary>
        /// 根据券码查询信息
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public MaPaiVipModel SelectContinentalConfigInfoByCouponCode(string couponCode)
        {
            MaPaiVipModel result = null;

            try
            {
                if (!string.IsNullOrEmpty(couponCode))
                {
                    result = dbScopeReadManager.Execute(conn => DALMaPaiVip.SelectContinentalConfigInfoByCouponCode(conn, couponCode));
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SelectContinentalConfigInfoByCouponCode");
            }

            return result;
        }

        /// <summary>
        /// 验证上传券码的信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Tuple<List<MaPaiVipModel>, List<MaPaiVipModel>, string> VerifyData(List<MaPaiVipModel> data)
        {
            List<MaPaiVipModel> successData = new List<MaPaiVipModel>();
            List<MaPaiVipModel> errorData = new List<MaPaiVipModel>();
            var msg = string.Empty;
            try
            {
                if (data != null && data.Any())
                {
                    data = data.GroupBy(_ => _.UniquePrivilegeCode).Select(x => x.FirstOrDefault()).ToList();
                    if (data.GroupBy(_ => _.IsDeleted).Count() > 1)
                    {
                        msg = "上传的数据仅支持单向操作，即全增或全删";
                    }
                    else
                    {
                        dbScopeReadManager.Execute(conn =>
                        {
                            if (data.FirstOrDefault().IsDeleted)
                            {
                                foreach (var item in data)
                                {
                                    var info = DALMaPaiVip.SelectContinentalConfigInfoByCouponCode(conn, item.UniquePrivilegeCode);
                                    if (info != null && !info.IsDeleted)
                                    {
                                        successData.Add(item);
                                    }
                                    else
                                    {
                                        msg = "该批次的券码在现有表中正常状态的券码中没有查找到数据，删除该券码后重新操作";
                                        errorData.Add(item);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var item in data)
                                {
                                    var info = DALMaPaiVip.SelectContinentalConfigInfoByCouponCode(conn, item.UniquePrivilegeCode);
                                    if (info != null)
                                    {
                                        msg = "该批次的券码与表中现有券码重复，请删除该券码后重新操作";
                                        errorData.Add(item);
                                    }
                                    else
                                    {
                                        successData.Add(item);
                                    }
                                }

                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "VerifyData");
            }

            return Tuple.Create(successData, errorData, msg);
        }

        /// <summary>
        /// 增加券码
        /// </summary>
        /// <param name="couponCode"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool InsertCouponCodeConfig(string couponCode, string user)
        {
            var result = false;

            try
            {
                var pkid = dbScopeManager.Execute(conn => DALMaPaiVip.InsertContinentalConfig(conn, couponCode, false));
                result = pkid > 0;
                if (result) { InsertLog("InsertCouponCodeConfig", pkid.ToString(), $"CouponCode:{couponCode}", result ? "添加成功" : "添加失败", user); }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "InsertCouponCodeConfig");
            }

            return result;
        }

        /// <summary>
        /// 批量操作券码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool BatchOperatorCouponCodeConfig(List<MaPaiVipModel> data)
        {
            var result = false;

            try
            {
                dbScopeManager.Execute(conn =>
                {
                    foreach (var item in data)
                    {
                        var info = DALMaPaiVip.SelectContinentalConfigInfoByCouponCode(conn, item.UniquePrivilegeCode);
                        if (info != null)
                        {
                            result = DALMaPaiVip.UpdateConfigByCouponCode(conn, item.UniquePrivilegeCode, item.IsDeleted) > 0;
                        }
                        else
                        {
                            result = DALMaPaiVip.InsertContinentalConfig(conn, item.UniquePrivilegeCode, item.IsDeleted) > 0;
                        }
                    }
                });
                result = true;
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "BatchInsertCouponCodeConfig");
            }

            return result;
        }

        /// <summary>
        /// 批量删除券码
        /// </summary>
        /// <param name="pkidStr"></param>
        /// <returns></returns>
        public bool BatchDeleteConfig(string pkidStr)
        {
            var result = false;

            try
            {
                result = dbScopeManager.Execute(conn => DALMaPaiVip.DeletedContinentalConfig(conn, pkidStr)) > 0;
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "BatchDeleteConfig");
            }

            return result;
        }

        /// <summary>
        /// 更新券码状态
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="status"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateContinentalConfigStatus(long pkid, int status, string user)
        {
            var result = false;

            try
            {
                result = dbScopeManager.Execute(conn => DALMaPaiVip.UpdateContinentalConfigStatus(conn, pkid, status)) > 0;
                if (result) { InsertLog("UpdateContinentalConfigStatus", pkid.ToString(), $"IsDeleted:{status}", result ? "更新成功" : "更新成功", user); }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "UpdateContinentalConfigStatus");
            }

            return result;
        }

        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="method"></param>
        /// <param name="objectId"></param>
        /// <param name="remarks"></param>
        /// <param name="msg"></param>
        /// <param name="opera"></param>
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
                    Type= "MaPaiVip",
                    CreatedTime = DateTime.Now
                };
                LoggerManager.InsertLog("CompanyClientLog", info);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "ConfigLog");
            }
        }
    }
}
