using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.CustomersActivity;
using Tuhu.Provisioning.DataAccess.Entity.CustomersActivity;

namespace Tuhu.Provisioning.Business.CustomersActivity
{
    public class CustomersActivityManager
    {
        #region 大客户活动专享配置

        /// <summary>
        /// 查询大客户专享活动配置列表
        /// </summary>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        public static List<CustomerExclusiveSettingModel> SelectCustomerExclusives(int pageIndex, int pageSize)
        {
            using (var conn = ProcessConnection.OpenGungnir)
            {
                return DalCustomerExclusiveSetting.
                    SelectCustomerExclusives(conn, pageIndex, pageSize).ToList();
            }
        }


        /// <summary>
        /// 查询大客户专享活动配置列表总数
        /// </summary>
        /// <returns></returns>
        public static int SelectCustomerExclusiveCount()
        {
            using (var conn = ProcessConnection.OpenGungnir)
            {
                return DalCustomerExclusiveSetting.
                    SelectCustomerExclusiveCount(conn);
            }
        }

        /// <summary>
        /// 调用服务端接口查询公司信息
        /// </summary>
        /// <returns></returns>
        public static List<CompanyInfoDictModel> GetCompanyInfoDict()
        {
            var result = UserAccountService.SelectCompanyInfoById(0);

            var listCompanyInfo = result.Select(a => new CompanyInfoDictModel
            {
                text = a.Name,
                value = "" + a.Id
            }).ToList();

            return listCompanyInfo;
        }

        /// <summary>
        /// 客户专享活动配置修改
        /// </summary>
        /// <param name="customerExclusiveSettingModel"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int UpdateCustomerExclusiveSetting(CustomerExclusiveSettingModel customerExclusiveSettingModel, string user)
        {
            int result = 0;

            if (customerExclusiveSettingModel != null)
            {
                customerExclusiveSettingModel.UpdateBy = user;
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    var current = DalCustomerExclusiveSetting.SelectCustomerExclusive(conn, customerExclusiveSettingModel.ActivityId, customerExclusiveSettingModel.PKID).ToList();

                    if (current != null && current.Count() == 1)
                    {
                        result = -9;
                    }
                    else
                    {
                        var customerModel = DalCustomerExclusiveSetting.SelectCustomerExclusive(conn, customerExclusiveSettingModel.PKID).ToList();

                        if (customerModel != null && customerModel.Count() == 1)
                        {
                            var rows = DalCustomerExclusiveSetting.UpdateCustomerExclusiveSetting(conn, customerExclusiveSettingModel);
                            result = rows == 1 ? 1 : 0;
                            if (result == 1)
                            {
                                var log = new
                                {
                                    ObjectId = customerExclusiveSettingModel.PKID,
                                    Source = "CustomerExclusiveSetting",
                                    ObjectType = "UpdateCustomerExclusiveSetting",
                                    BeforeValue = JsonConvert.SerializeObject(customerModel),
                                    AfterValue =
                                    JsonConvert.SerializeObject(customerExclusiveSettingModel),
                                    Remark = "客户专享活动配置修改",
                                    IsDeleted = 0,
                                    CreateDateTime = DateTime.Now,
                                    Creator = user
                                };
                                LoggerManager.InsertLog("CustomerSetting", log);
                                ActivityService.RefreshRedisCacheCustomerSetting(customerExclusiveSettingModel.ActivityExclusiveId);
                            }
                        }

                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 客户专享活动配置新增
        /// </summary>
        /// <param name="customerExclusiveSettingModel"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int InsertCustomerExclusiveSetting(CustomerExclusiveSettingModel customerExclusiveSettingModel, string user)
        {
            int result = 0;

            if (customerExclusiveSettingModel != null)
            {
                string guid = "" + System.Guid.NewGuid();
                customerExclusiveSettingModel.ActivityExclusiveId = SecurityHelper.Hash(guid);
                customerExclusiveSettingModel.CreateBy = user;
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    var current = DalCustomerExclusiveSetting.SelectCustomerExclusive(conn, customerExclusiveSettingModel.ActivityId).ToList();

                    if (current != null && current.Count() == 1)
                    {
                        result = -9;
                    }
                    else
                    {
                        var rows = DalCustomerExclusiveSetting.InsertCustomerExclusiveSetting(conn, customerExclusiveSettingModel);
                        result = rows == 1 ? 1 : 0;
                        if (result == 1)
                        {
                            var log = new
                            {
                                ObjectId =
                                DalCustomerExclusiveSetting.GetCustomerExclusiveSettingMaxPkId(conn),
                                Source = "CustomerExclusiveSetting",
                                ObjectType = "InsertCustomerExclusiveSetting",
                                BeforeValue = "",
                                AfterValue = JsonConvert.SerializeObject(customerExclusiveSettingModel),
                                Remark = "客户专享活动配置新增",
                                CreateDateTime = DateTime.Now,
                                IsDeleted = 0,
                                Creator = user
                            };
                            LoggerManager.InsertLog("CustomerSetting", log);

                            ActivityService.RefreshRedisCacheCustomerSetting(customerExclusiveSettingModel.ActivityExclusiveId);
                        }
                    }
                }
            }

            return result;
        }

        #endregion



        #region 大客户活动专享券码

        /// <summary>
        /// 查询大客户专享活动券码列表
        /// </summary>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="queryString">查询条件</param>
        /// <param name="customersSettingId">活动专享配置表PKID</param>
        /// <param name="activityExclusiveId">活动专享Id</param>
        /// <returns></returns>
        public static List<CustomerExclusiveCouponModel> SelectCustomerCoupons(string queryString, string customersSettingId, string activityExclusiveId, int pageIndex, int pageSize)
        {
            using (var conn = ProcessConnection.OpenGungnir)
            {
                return DalCustomerExclusiveSetting.
                    SelectCustomerCoupons(conn, queryString, customersSettingId, activityExclusiveId, pageIndex, pageSize).ToList();
            }
        }


        /// <summary>
        /// 查询大客户专享活动券码总数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="queryString">查询条件</param>
        /// <param name="customersSettingId">活动专享配置表ID</param>
        /// <param name="activityExclusiveId">活动专享Id</param>
        /// <returns></returns>
        public static int SelectCustomerCouponCount(string queryString, string customersSettingId, string activityExclusiveId)
        {
            using (var conn = ProcessConnection.OpenGungnir)
            {
                return DalCustomerExclusiveSetting.
                    SelectCustomerCouponCount(conn, queryString, customersSettingId, activityExclusiveId);
            }
        }


        /// <summary>
        /// 客户专享活动券码新增
        /// </summary>
        /// <param name="customerExclusiveCouponModel"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int InsertCustomerCoupon(CustomerExclusiveCouponModel customerExclusiveCouponModel, string user)
        {
            int result = 0;

            if (customerExclusiveCouponModel != null)
            {
                customerExclusiveCouponModel.CreateBy = user;
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    var current = DalCustomerExclusiveSetting.SelectCustomerCouponInfo(conn, customerExclusiveCouponModel.CouponCode, customerExclusiveCouponModel.ActivityExclusiveId).ToList();

                    if (current != null && current.Count() == 1)
                    {
                        result = -9;
                    }
                    else
                    {
                        var rows = DalCustomerExclusiveSetting.InsertCustomerCoupon(conn, customerExclusiveCouponModel);
                        result = rows == 1 ? 1 : 0;
                        if (result == 1)
                        {
                            var log = new
                            {
                                ObjectId =
                                DalCustomerExclusiveSetting.GetCustomerCouponMaxPkId(conn),
                                Source = "CustomerExclusiveCoupon",
                                ObjectType = "InsertCustomerCoupon",
                                BeforeValue = "",
                                AfterValue = JsonConvert.SerializeObject(customerExclusiveCouponModel),
                                Remark = "客户专享活动券码新增",
                                CreateDateTime = DateTime.Now,
                                IsDeleted = 0,
                                Creator = user
                            };
                            LoggerManager.InsertLog("CustomerSetting", log);
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 客户专享活动券码状态修改
        /// </summary>
        /// <param name="customerExclusiveSettingModel"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int UpdateCustomerCouponStatus(CustomerExclusiveCouponModel customerExclusiveCouponModel, string user)
        {
            int result = 0;

            if (customerExclusiveCouponModel != null)
            {
                customerExclusiveCouponModel.UpdateBy = user;
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    var customerModel = DalCustomerExclusiveSetting.SelectCustomerCoupon(conn, customerExclusiveCouponModel.PKID).ToList();

                    if (customerModel != null && customerModel.Count() == 1)
                    {
                        var rows = DalCustomerExclusiveSetting.UpdateCustomerCouponStatus(conn, customerExclusiveCouponModel);
                        result = rows == 1 ? 1 : 0;
                        if (result == 1)
                        {
                            var log = new
                            {
                                ObjectId = customerExclusiveCouponModel.PKID,
                                Source = "CustomerExclusiveCoupon",
                                ObjectType = "UpdateCustomerCoupon",
                                BeforeValue = JsonConvert.SerializeObject(customerModel),
                                AfterValue =
                                JsonConvert.SerializeObject(customerExclusiveCouponModel),
                                Remark = "客户专享活动券码状态修改",
                                IsDeleted = 0,
                                CreateDateTime = DateTime.Now,
                                Creator = user
                            };
                            LoggerManager.InsertLog("CustomerSetting", log);
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 客户专享活动券码批量导入
        /// </summary>
        /// <param name="customerExclusiveCouponModel"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int InsertCustomerCoupons(List<CustomerExclusiveCouponModel> listCustomerExclusiveCouponModel, string user, out string resultCoupons)
        {
            var resultTotalCount = 0;
            resultCoupons = string.Empty;

            if (listCustomerExclusiveCouponModel != null)
            {
                using (var conn = ProcessConnection.OpenGungnir)
                {
                    foreach (var item in listCustomerExclusiveCouponModel)
                    {
                        int result = 0;

                        item.CreateBy = user;
                        var current = DalCustomerExclusiveSetting.SelectCustomerCouponInfo(conn, item.CouponCode,item.ActivityExclusiveId).ToList();

                        if (current != null && current.Count() == 1)
                        {
                            result = -9;
                            if (string.IsNullOrWhiteSpace(resultCoupons))
                                resultCoupons = item.CouponCode;
                            else
                                resultCoupons += "," + item.CouponCode;
                        }
                        else
                        {
                            var rows = DalCustomerExclusiveSetting.InsertCustomerCoupon(conn, item);
                            result = rows == 1 ? 1 : 0;
                            if (result == 1)
                            {
                                var log = new
                                {
                                    ObjectId =
                                    DalCustomerExclusiveSetting.GetCustomerCouponMaxPkId(conn),
                                    Source = "CustomerExclusiveCoupon",
                                    ObjectType = "InsertCustomerCoupon",
                                    BeforeValue = "",
                                    AfterValue = JsonConvert.SerializeObject(item),
                                    Remark = "客户专享活动券码导入",
                                    CreateDateTime = DateTime.Now,
                                    IsDeleted = 0,
                                    Creator = user
                                };
                                LoggerManager.InsertLog("CustomerSetting", log);
                                resultTotalCount++;
                            }

                        }
                    }
                }
            }

            return resultTotalCount;
        }


        #endregion


        /// <summary>
        /// 查询客户专项配置单条日志信息
        /// </summary>
        /// <param name="ojbetId">业务ID</param>
        /// <returns></returns>
        public List<CustomerExclusiveSettingLogModel> GetCustomerExclusiveSettingLog(string ojbetId, string source)
        {
            return DalCustomerExclusiveSetting.GetCustomerExclusiveSettingLog(ojbetId, source);
        }
    }
}
