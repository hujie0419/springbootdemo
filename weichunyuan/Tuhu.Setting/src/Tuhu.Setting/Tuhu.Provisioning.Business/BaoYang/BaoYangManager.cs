using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Transactions;
using Newtonsoft.Json;
using Tuhu.Component.Framework;
using Tuhu.Framework;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.BaoYang;
using Tuhu.Service.ConfigLog;
using static Tuhu.Provisioning.DataAccess.Entity.OilBrandPriorityModel;
using System.Text.RegularExpressions;
using Common.Logging;
using Tuhu.Provisioning.DataAccess.Entity.BaoYang;
using Tuhu.Provisioning.Business.Logger;
using System.Xml;
using System.Threading.Tasks;
using Tuhu.Service.BaoYang.Models.Result;
using Tuhu.Service.BaoYang.Models.Package;
using Tuhu.Service.BaoYang.Config;
using Tuhu.Service.BaoYang.Models;

namespace Tuhu.Provisioning.Business.BaoYang
{
    public class BaoYangManager
    {
        private static readonly IConnectionManager connectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager grAlwaysOnReadConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager tuhuLogConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString);
        private static readonly IConnectionManager byAlwaysOnReadConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["BaoYang_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager tuhuBiReadConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_BI_ReadOnly"].ConnectionString);

        private readonly IDBScopeManager dbScopeManagerBY = null;
        private readonly IDBScopeManager GRAlwaysOnReadDbScopeManager = null;
        private readonly IDBScopeManager dbScopeManagerTuhuLog = null;
        private readonly IDBScopeManager BYAlwaysOnReadDbScopeManager = null;
        private readonly IDBScopeManager TuhuBiReadDbScopeManager = null;
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(BaoYangManager));

        public BaoYangManager()
        {
            dbScopeManagerBY = new DBScopeManager(connectionManager);
            GRAlwaysOnReadDbScopeManager = new DBScopeManager(grAlwaysOnReadConnectionManager);
            dbScopeManagerTuhuLog = new DBScopeManager(tuhuLogConnectionManager);
            BYAlwaysOnReadDbScopeManager = new DBScopeManager(byAlwaysOnReadConnectionManager);
            TuhuBiReadDbScopeManager = new DBScopeManager(tuhuBiReadConnectionManager);
            //using (var client = new CacheClient())
            //{
            //    client.UpdateBaoYangActivityAsync("activtiyId");
            //    client.UpdateTuhuRecommendConfigAsync();
            //}
        }

        /// <summary>
        /// 获取保养页面保养配置列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<BaoYangModel> SelectBaoYangActivityStyle()
        {
            DataTable resulTable = DALBaoyang.SelectBaoYangActivityStyle();
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new BaoYangModel(dr));
            }
            else
            {
                return new BaoYangModel[0];
            }
        }

        public static BaoYangModel GetBaoYangModelByPKID(int pkid)
        {
            var dr = DALBaoyang.GetBaoYangModelByPKID(pkid);
            return new BaoYangModel(dr);
        }

        public static int UpdateBaoYangModel(BaoYangModel model)
        {
            if (!string.IsNullOrEmpty(model.PageType))//index
            {
                return DALBaoyang.UpdateBaoYangIndexModel(model);
            }
            else//baoyang
            {
                return DALBaoyang.UpdateBaoYangModel(model);
            }
        }


        /// <summary>
        /// 获取首页保养配置列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<BaoYangModel> GetBaoyangIndexConfigItemList()
        {
            DataTable resulTable = DALBaoyang.GetBaoyangIndexConfigItemList();
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new BaoYangModel(dr));
            }
            else
            {
                return new BaoYangModel[0];
            }
        }


        /// <summary>
        /// 添加保养活动项目安排
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertBaoyangList(BaoYangItemModel model)
        {
            var result = DALBaoyang.InsertBaoyangList(model);
            return result;
        }


        /// <summary>
        /// 获取当前项目的活动集合
        /// </summary>
        /// <param name="BaoYangActivityStyleID"></param>
        /// <returns></returns>
        public static IEnumerable<BaoYangItemModel> GetBaoyangListByBaoYangActivityStyleID(string BaoYangActivityStyleID)
        {
            DataTable resulTable = DALBaoyang.GetBaoyangListByBaoYangActivityStyleID(BaoYangActivityStyleID);
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new BaoYangItemModel(dr));
            }
            else
            {
                return new BaoYangItemModel[0];
            }
        }

        /// <summary>
        /// 根据PKID删除保养项
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteActivityItemByPkid(int pkid)
        {
            return DALBaoyang.DeleteActivityItemByPkid(pkid);
        }

        //修改页面保存
        public static bool SaveRecommendProducts(List<PrioritySettingModel> projectList, string userName)
        {
            return DALBaoyang.SaveRecommendProducts(projectList, userName);
        }

        //public static DataTable GetBaoYangCP_Brand(string PrimaryParentCategory)
        //{
        //    DataTable dt = DALBaoyang.GetBaoYangCP_Brand(PrimaryParentCategory);
        //    return dt;
        //}
        /// <summary>
        /// 根据产品类目获取产品的品牌集合
        /// </summary>
        /// <param name="PrimaryParentCategory">产品类目</param>
        /// <returns></returns>
        public static List<string> GetBaoYangCP_Brand(string PrimaryParentCategory)
        {
            return DALBaoyang.GetBaoYangCP_Brand(PrimaryParentCategory).Rows.Cast<DataRow>().Select(x => x["CP_Brand"].ToString()).ToList();
        }

        #region 防冻液相关Method
        /// <summary>
        ///  获取省份集合
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ProvinceItem> GetProvinceList()
        {
            IEnumerable<ProvinceItem> result = new List<ProvinceItem>();
            try
            {
                result = DALBaoyang.GetProviceList().Rows.Cast<DataRow>().Select(x => new ProvinceItem(x));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 获取防冻液配置数据
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AntifreezeSettingModel> GetAntifreezeSetting()
        {
            IEnumerable<AntifreezeSettingModel> result = new List<AntifreezeSettingModel>();
            try
            {
                result = DALBaoyang.GetAntifreezeSetting().Rows.Cast<DataRow>().Select(x => new AntifreezeSettingModel(x));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 设置防冻液配置
        /// </summary>
        /// <param name="Code">0：添加 1：修改</param>
        /// <param name="AntifreezeSettingItem">防冻液配置实体类</param>
        /// <returns></returns>
        public static bool SaveAntifreezeSetting(int Code, AntifreezeSettingModel AntifreezeSettingItem, out string ErrorMessage)
        {
            var IsExist = false; var Result = false;
            var TmepProvinceName = string.Empty;
            ErrorMessage = string.Empty;
            try
            {
                var SelectAntifreezePovinceList = AntifreezeSettingItem.ProvinceNames.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();//此处把空数组排除，防止拿空值做对比
                if (SelectAntifreezePovinceList.Count() > 0)//如果将要配置的省份为空，直接不做接下来的比较
                {
                    var SettingList = GetAntifreezeSetting();// GetAntifreezeSetting();
                    if (SettingList != null && SettingList.Count() > 0)//如果没有配置数据则不做处理
                    {
                        if (Code == 1) SettingList = SettingList.Where(x => x.FreezingPoint != AntifreezeSettingItem.FreezingPoint);// 如果是修改操作，仅仅比较除了本身之外的配置数据
                        foreach (var item in SettingList)
                        {
                            var SettingPids = item.ProvinceNames.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                            if (SettingPids.Count() > 0)
                            {
                                foreach (var item1 in SelectAntifreezePovinceList)
                                {
                                    if (SettingPids.Contains(item1))
                                    {
                                        IsExist = true;
                                        TmepProvinceName = item1;
                                        break;
                                    }
                                }
                            }
                            if (IsExist) break;
                        }
                    }

                }
                if (IsExist) ErrorMessage = TmepProvinceName + "已经被设置过防冻液配置了，同一省份或者直辖市不能重复设置";
                else
                {
                    Result = DALBaoyang.SaveAntifreezeSetting(Code, AntifreezeSettingItem);
                    if (!Result) ErrorMessage = "操作失败！";
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return Result;
        }
        /// <summary>
        /// 停用或启用防冻液配置
        /// </summary>
        /// <param name="Status">0：未启用 1：启用</param>
        /// <returns></returns>
        public static bool SetAntifreezeStatus(byte Status)
        {
            return DALBaoyang.SetAntifreezeStatus(Status);
        }
        #endregion


        public static DataTable GetBatteryCP_Brand(string partName)
        {
            DataTable dt = DALBaoyang.GetBatteryCP_Brand(partName);
            return dt;
        }
        //停用或启用模块
        public static bool setIsEnable(string PartName, int onOrof)
        {
            return DALBaoyang.setIsEnable(PartName, onOrof);
        }

        public static IEnumerable<PrioritySettingModel> GetBaoYangPriority(string PriorityField)
        {
            DataTable resultTable = DALBaoyang.GetBaoYangPriority(PriorityField);
            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                return resultTable.Rows.Cast<DataRow>().Select(dr => new PrioritySettingModel(dr));
            }
            else
            {
                return new PrioritySettingModel[0];
            }
        }

        /// <summary>
        /// 获取保养默认推荐表的所有信息通过type(采用新表，目前只有机油配置使用)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<BaoYangProductRecommendModel> GetRecommandProductsPriority(string type)
        {
            DataTable resultTable = DALBaoyang.GetRecommandProductsPriority(type);
            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                return resultTable.Rows.Cast<DataRow>().Select(dr => new BaoYangProductRecommendModel(dr));
            }
            else
            {
                return new BaoYangProductRecommendModel[0];
            }
        }

        /// <summary>
        /// 关闭或启用模块通过partName
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="onOrof"></param>
        /// <returns></returns>
        public static bool setProductsRecommendIsEnable(string partName, int onOrof)
        {
            return DALBaoyang.setProductsRecommendIsEnable(partName, onOrof);
        }

        /// <summary>
        /// 保存保养推荐页面的数据，根据名称和属性查找，如果存在此条数据，则修改，否则做添加操作
        /// </summary>
        /// <param name="projectList"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool UpsertRecommendPrioritySetting(List<BaoYangProductRecommendModel> projectList, string userName)
        {
            return DALBaoyang.UpsertRecommendPrioritySetting(projectList, userName);
        }

        /// <summary>
        /// 获取保养默认推荐的信息
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<BaoYangProductRecommendModel> SelectRecommendPriorityByProjectName(string projectName, string type)
        {
            DataTable resulTable = DALBaoyang.SelectRecommendPriorityByProjectName(projectName, type);
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new BaoYangProductRecommendModel(dr));
            }
            else
            {
                return new BaoYangProductRecommendModel[0];
            }
        }
        public static IEnumerable<PrioritySettingModel> GetBaoYangPriorityByPartNameAndField(string projectName, string field)
        {
            DataTable resulTable = DALBaoyang.GetBaoYangPriorityByPartNameAndField(projectName, field);
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new PrioritySettingModel(dr));
            }
            else
            {
                return new PrioritySettingModel[0];
            }
        }

        public static string GetProductNameByPid(string pid, string category)
        {
            string productName = string.Empty;

            productName = DALBaoyang.SelectProductNameByPid(pid, category);

            return productName;
        }

        /// <summary>
        /// 根据产品类目获取车灯的属性
        /// </summary>
        /// <param name="PrimaryParentCategory">产品类目</param>
        /// <returns></returns>
        public static List<string> GetDynamoCP_ShuXing(string PrimaryParentCategory)
        {
            return DALBaoyang.GetDynamoCP_ShuXing(PrimaryParentCategory).Rows.Cast<DataRow>().Select(x => x["CP_ShuXing2"].ToString()).ToList();
        }

        /// <summary>
        /// 获取所有车型的品牌
        /// </summary>
        /// <returns></returns>
        public List<string> SelectAllVehicleBrands()
        {
            return GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.SelectAllVehicleBrands(conn));
        }

        /// <summary>
        /// 根据productCategory获取产品表对应的系列名称
        /// </summary>
        /// <param name="productCategory"></param>
        /// <returns></returns>
        public List<string> GetVehicleSeriesByProductCategory(string productCategory)
        {
            return GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.GetVehicleSeriesByProductCategory(conn, productCategory));
        }

        /// <summary>
        /// 根据系列名称获取产品表机油对应的品牌
        /// </summary>
        /// <param name="priorityType"></param>
        /// <param name="series"></param>
        /// <returns></returns>
        public List<string> GetJYBrandByPriorityTypeAndSeries(string priorityType, string series)
        {
            return GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.GetJYBrandByPriorityTypeAndSeries(conn, priorityType, series));
        }

        /// <summary>
        /// 根据选择的品牌该品牌的系列
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public IDictionary<string, string> SelectVehicleSeries(string brand)
        {
            return GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.SelectVehicleSeries(conn, brand));
        }

        public IEnumerable<string> SelectVehiclePaiLiang(string vid)
        {
            return GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.SelectVehiclePaiLiang(conn, vid));
        }


        /// <summary>
        /// 根据partName获取当前车型的配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="partName"></param>
        /// <param name="brand"></param>
        /// <param name="vehicleID"></param>
        /// <param name="staPrice"></param>
        /// <param name="endPrice"></param>
        /// <param name="isConfig"></param>
        /// <returns></returns>
        public Tuple<List<BaoYangPriorityVehicleSettingModel>, int> SelectBaoYangVehicleSetting(int pageIndex,
            int pageSize, string partName, string brand, string vehicleID, string staPrice, string endPrice,
            int isConfig, string priorityType1, string firstPriority, string priorityType2,
            string secondPriority, string viscosity)
        {
            Tuple<List<BaoYangPriorityVehicleSettingModel>, int> result = null;
            if (string.Equals(partName, "机油"))
            {
                if (string.IsNullOrWhiteSpace(viscosity))
                {
                    result = GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.SelectBaoYangVehicleSettingOil(conn,
                        pageIndex, pageSize, brand, vehicleID, staPrice, endPrice, isConfig, priorityType1, firstPriority,
                        priorityType2, secondPriority));

                }
                else
                {
                    result = GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.SelectBaoYangVehicleSettingOil(conn,
                        pageIndex, pageSize, brand, vehicleID, staPrice, endPrice, isConfig, priorityType1, firstPriority,
                        priorityType2, secondPriority, viscosity));
                }
            }
            else
            {
                result = GRAlwaysOnReadDbScopeManager.Execute(conn =>
                           DALBaoyang.SelectBaoYangVehicleSettingOther(conn, pageIndex, pageSize, partName, brand, vehicleID,
                               staPrice, endPrice, isConfig, firstPriority, secondPriority));
            }

            return result;
        }

        public Tuple<List<VehicleOilSetting>, int> SelectVehicleOilSetting(int pageIndex, int pageSize, string brand,
            string vehicleID, string paiLiang, int isConfig)
        {
            Tuple<List<VehicleOilSetting>, int> result = null;

            result =
                GRAlwaysOnReadDbScopeManager.Execute(
                    conn =>
                        DALBaoyang.SelectVehicleOilSetting(conn, pageIndex, pageSize, brand, vehicleID, paiLiang,
                            isConfig));

            return result;
        }

        public bool UpsertVehicleOilViscosity(string vehicleId, string paiLiang, string viscosity, string user)
        {
            return
                dbScopeManagerBY.Execute(
                    conn => DALBaoyang.UpsertVehicleOilViscosity(conn, vehicleId, paiLiang, viscosity, user));
        }

        public bool BatchUpsertVehicleOilViscosity(List<VehicleOilSetting> batchVehicleModels, string viscosity,
            string user)
        {
            return
                dbScopeManagerBY.Execute(
                    conn => DALBaoyang.BatchUpsertVehicleOilViscosity(conn, batchVehicleModels, viscosity, user));
        }

        public bool DeleteVehicleOilViscosity(string vehicleId, string paiLiang)
        {
            return dbScopeManagerBY.Execute(conn => DALBaoyang.DeleteVehicleOilViscosity(conn, vehicleId, paiLiang));
        }


        /// <summary>
        /// 根据partName、PriorityType、vehicleID获取特殊车型配置表养护类和品牌类的配置信息
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="priorityType"></param>
        /// <param name="vehicleID"></param>
        /// <returns></returns>
        public List<BaoYangPriorityVehicleSettingModel> GetPriorityVehicleRulesByPartNameAndType(string partName, string priorityType, string vehicleID)
        {

            var result = GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.GetPriorityVehicleRulesByPartNameAndType(conn, partName, priorityType, vehicleID));

            return result;
        }

        /// <summary>
        /// 根据VehicleID获取机油的特殊车型的配置信息
        /// </summary>
        /// <param name="vehicleID"></param>
        /// <returns></returns>
        public BaoYangPriorityVehicleSettingModel GetJYPriorityVehicleSetting(string vehicleID)
        {
            BaoYangPriorityVehicleSettingModel result = null;

            result = GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.GetJYPriorityVehicleSetting(conn, vehicleID));

            return result;
        }

        /// <summary>
        /// 保存保养的特殊车型的配置信息
        /// </summary>
        /// <param name="projectList"></param>
        /// <param name="vehicleIDList"></param>
        /// <param name="partName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool InsertOrUpdateVehicleSetting(List<BaoYangPriorityVehicleSettingModel> projectList, List<string> vehicleIDList, string partName, string userName)
        {
            bool result = false;

            var row = DALBaoyang.InsertOrUpdateVehicleSetting(projectList, vehicleIDList, userName);

            if (row > 0)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 保存机油的特殊车型的配置信息
        /// </summary>
        /// <param name="projectList"></param>
        /// <param name="vehicleIDList"></param>
        /// <param name="partName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool InsertBaoYangJYVehicleSeriesSetting(List<BaoYangPriorityVehicleSettingModel> projectList, List<string> vehicleIDList, string partName, string userName)
        {
            bool result = false;

            var row = DALBaoyang.InsertBaoYangJYVehicleSeriesSetting(projectList, vehicleIDList, partName, userName);

            if (row > 0)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 删除保养车型推荐的信息
        /// </summary>
        /// <param name="vehicleID"></param>
        /// <param name="partName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool DeletedBaoYangPriorityVehicleSettingByVehicleIDAndPartName(string vehicleID, string partName, string userName)
        {
            bool result = false;

            var row = dbScopeManagerBY.Execute(conn => DALBaoyang.DeletedBaoYangPriorityVehicleSettingByVehicleIDAndPartName(conn, vehicleID, partName, userName));

            if (row > 0)
            {
                result = true;
            }

            return result;
        }

        public bool BatchDeletedBaoYangVehicleConfig(string vehicleIdStr, string partName, string userName)
        {
            return dbScopeManagerBY.Execute(conn => DALBaoyang.BatchDeletedBaoYangVehicleConfig(conn, vehicleIdStr, partName, userName)) > 0;
        }

        public static void SendEmailToUser(List<PrioritySettingModel> oldData, List<PrioritySettingModel> newData, string userName)
        {
            var configManager = new BaoYangConfigManager();
            var result = configManager.GetEmailPersonsConfig();//获取邮件接收的人员配置信息
            var html = htmlConvert(oldData, newData, userName);//转换为html
            List<string> recevierStr = null;
            if (result != null)
            {
                recevierStr = string.Join(",", result.BaoYangRecommendConfigReceiver).Split(',').ToList();
            }
            if (recevierStr != null && recevierStr.Any())
            {
                foreach (var item in recevierStr)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        Send("保养产品推荐配置修改通知", html, item);
                    }
                }
            }
        }

        public static void SendEmailToUserForJY(List<BaoYangProductRecommendModel> oldData, List<BaoYangProductRecommendModel> newData, string userName)
        {
            var configManager = new BaoYangConfigManager();
            var result = configManager.GetEmailPersonsConfig();//获取邮件接收的人员配置信息
            var html = htmlConvert(oldData, newData, userName);//转换为html
            List<string> recevierStr = null;
            if (result != null)
            {
                recevierStr = string.Join(",", result.BaoYangRecommendConfigReceiver).Split(',').ToList();
            }
            if (recevierStr != null && recevierStr.Any())
            {
                foreach (var item in recevierStr)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        Send("保养产品推荐配置修改通知", html, item);
                    }
                }
            }
        }

        public static void SendEmailToUser(List<AntifreezeSettingModel> oldData, List<AntifreezeSettingModel> newData, string userName)
        {
            var configManager = new BaoYangConfigManager();
            var result = configManager.GetEmailPersonsConfig();//获取邮件接收的人员配置信息
            var html = htmlConvert(oldData, newData, userName);//转换为html
            List<string> recevierStr = null;
            if (result != null)
            {
                recevierStr = string.Join(",", result.BaoYangRecommendConfigReceiver).Split(',').ToList();
            }
            if (recevierStr != null && recevierStr.Any())
            {
                foreach (var item in recevierStr)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        Send("保养产品推荐配置修改通知", html, item);
                    }
                }
            }
        }

        private static void Send(string subject, string body, string to)
        {
            MailMessage email = new MailMessage();
            email.From = new MailAddress("servicesender@tuhu.cn", "保养产品推荐配置修改通知", Encoding.UTF8);
            email.To.Add(to);
            email.Subject = subject;
            email.Body = body;
            email.BodyEncoding = Encoding.UTF8;
            email.Priority = MailPriority.High;
            email.IsBodyHtml = true;

            try
            {
                using (var client = new SmtpClient
                {
                    Host = "smtp.exmail.qq.com",
                    Port = 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 3000,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("servicesender@tuhu.cn", "Lushanshenglong8")
                })
                {
                    client.Send(email);
                }
            }
            catch (Exception ex)
            {
                new Exception("邮件发送失败:" + ex.Message);
            }
        }

        private static string htmlConvert(List<PrioritySettingModel> oldData, List<PrioritySettingModel> newDate, string userName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"<style type='text/css'> .table-striped tr:nth-child(odd) {
                background-color: #EEEEEE;
            }

            .table-striped tr:hover:nth-child(odd) {
                background: #FFEC8B;
            }

            .table-striped tr:hover:nth-child(even) {
                background: #FFEC8B;
            }

            .table-striped tr th {
                text-align: center;
            }

            .table-striped tr th {
                background: #22A7F0;
            }</style>");

            builder.Append("<table style='text-align: center;width:100%; ' class='table-striped'>");
            builder.Append("<tr style='color:white'>");
            builder.Append("<th style='text-align:center'>Change</th>");
            builder.Append("<th style='text-align:center'>PKID</th>");
            builder.Append("<th style='text-align:center'>名称</th>");
            builder.Append("<th style='text-align:center'>Type</th>");
            builder.Append("<th style='text-align:center'>是否启用</th>");
            builder.Append("<th style='text-align:center'>第一优先级</th>");
            builder.Append("<th style='text-align:center'>第二优先级</th>");
            builder.Append("<th style='text-align:center'>第三优先级</th></tr>");

            if (oldData != null && oldData.Any())
            {
                builder.Append($"<tr><td rowspan={oldData.Count() + 1}>改变前</td></tr>");

                foreach (var item in oldData)
                {
                    builder.Append("<tr>");
                    builder.Append("<td>" + item.ID + "</td>");
                    builder.Append("<td>" + item.PartName + "</td>");
                    builder.Append("<td>" + item.PropertyType + "</td>");
                    builder.Append("<td>" + (item.Enabled == 0 ? "启用" : "禁用") + "</td>");
                    builder.Append("<td>" + item.FirstPriority + "</td>");
                    builder.Append("<td>" + item.SecondPriority + "</td>");
                    builder.Append("<td>" + item.ThirdPriority + "</td>");
                    builder.Append("</tr>");
                }
            }

            if (newDate != null && newDate.Any())
            {
                builder.Append($"<tr><td rowspan={oldData.Count() + 1}>改变后</td></tr>");
                foreach (var item in newDate)
                {
                    var first = false;
                    var second = false;
                    var third = false;
                    var enable = false;
                    var oldEnabled = oldData.Where(_ => _.ID.Equals(item.ID)).Select(x => x.Enabled).FirstOrDefault();
                    var oldFirstPriority = oldData.Where(_ => _.ID.Equals(item.ID)).Select(x => x.FirstPriority).FirstOrDefault();
                    var oldSecondPriority = oldData.Where(_ => _.ID.Equals(item.ID)).Select(x => x.SecondPriority).FirstOrDefault();
                    var oldThirdPriority = oldData.Where(_ => _.ID.Equals(item.ID)).Select(x => x.ThirdPriority).FirstOrDefault();

                    enable = String.Equals(item.Enabled, oldEnabled);
                    first = String.Equals(item.FirstPriority, oldFirstPriority);
                    second = String.Equals(item.SecondPriority, oldSecondPriority);
                    third = String.Equals(item.ThirdPriority, oldThirdPriority);

                    builder.Append("<tr>");
                    builder.Append("<td>" + item.ID + "</td>");
                    builder.Append("<td>" + item.PartName + "</td>");
                    builder.Append("<td>" + item.PropertyType + "</td>");
                    builder.Append("<td " + (enable ? "" : "style='background:red'") + ">" + (item.Enabled == 0 ? "启用" : "禁用") + "</td>");
                    builder.Append("<td " + (first ? "" : "style='background:#ec9a9a'") + ">" + item.FirstPriority + "</td>");
                    builder.Append("<td " + (second ? "" : "style='background:#ec9a9a'") + ">" + item.SecondPriority + "</td>");
                    builder.Append("<td " + (third ? "" : "style='background:#ec9a9a'") + ">" + item.ThirdPriority + "</td>");
                    builder.Append("</tr>");
                }
            }

            builder.Append("</table>");
            builder.Append("<h3><span style='color:red'>" + userName + "</span>修改了保养推荐配置,修改时间：" + DateTime.Now + "</h3>");
            return builder.ToString();
        }

        private static string htmlConvert(List<BaoYangProductRecommendModel> oldData, List<BaoYangProductRecommendModel> newDate, string userName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"<style type='text/css'> .table-striped tr:nth-child(odd) {
                background-color: #EEEEEE;
            }

            .table-striped tr:hover:nth-child(odd) {
                background: #FFEC8B;
            }

            .table-striped tr:hover:nth-child(even) {
                background: #FFEC8B;
            }

            .table-striped tr th {
                text-align: center;
            }

            .table-striped tr th {
                background: #22A7F0;
            }</style>");

            builder.Append("<table style='text-align: center;width:100%; ' class='table-striped'>");
            builder.Append("<tr style='color:white'>");
            builder.Append("<th style='text-align:center'>Change</th>");
            builder.Append("<th style='text-align:center'>PKID</th>");
            builder.Append("<th style='text-align:center'>名称</th>");
            builder.Append("<th style='text-align:center'>Type</th>");
            builder.Append("<th style='text-align:center'>是否启用</th>");
            builder.Append("<th style='text-align:center'>机油优先级</th></tr>");

            if (oldData != null && oldData.Any())
            {
                builder.Append($"<tr><td rowspan={oldData.Count() + 1}>改变前</td></tr>");

                foreach (var item in oldData)
                {
                    builder.Append("<tr>");
                    builder.Append("<td>" + item.PKID + "</td>");
                    builder.Append("<td>" + item.PartName + "</td>");
                    builder.Append("<td>" + item.Property + "</td>");
                    builder.Append("<td>" + (item.Enabled == 0 ? "启用" : "禁用") + "</td>");
                    builder.Append("<td>" + item.Priority + "</td>");
                    builder.Append("</tr>");
                }
            }

            if (newDate != null && newDate.Any())
            {
                builder.Append($"<tr><td rowspan={oldData.Count() + 1}>改变后</td></tr>");
                foreach (var item in newDate)
                {
                    var priority = false;
                    var enable = false;
                    var oldEnabled = oldData.Where(_ => _.PKID.Equals(item.PKID)).Select(x => x.Enabled).FirstOrDefault();
                    var oldPriority = oldData.Where(_ => _.PKID.Equals(item.PKID)).Select(x => x.Priority).FirstOrDefault();
                    enable = String.Equals(item.Enabled, oldEnabled);
                    priority = String.Equals(item.Priority, oldPriority);

                    builder.Append("<tr>");
                    builder.Append("<td>" + item.PKID + "</td>");
                    builder.Append("<td>" + item.PartName + "</td>");
                    builder.Append("<td>" + item.Property + "</td>");
                    builder.Append("<td " + (enable ? "" : "style='background:#ec9a9a'") + ">" + (item.Enabled == 0 ? "启用" : "禁用") + "</td>");
                    builder.Append("<td " + (priority ? "" : "style='background:#ec9a9a'") + ">" + item.Priority + "</td>");
                    builder.Append("</tr>");
                }
            }

            builder.Append("</table>");
            builder.Append("<h3><span style='color:red'>" + userName + "</span>修改了保养推荐配置,修改时间：" + DateTime.Now + "</h3>");

            return builder.ToString();
        }

        private static string htmlConvert(List<AntifreezeSettingModel> oldData, List<AntifreezeSettingModel> newDate, string userName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"<style type='text/css'> .table-striped tr:nth-child(odd) {
                background-color: #EEEEEE;
            }

            .table-striped tr:hover:nth-child(odd) {
                background: #FFEC8B;
            }

            .table-striped tr:hover:nth-child(even) {
                background: #FFEC8B;
            }

            .table-striped tr th {
                text-align: center;
            }

            .table-striped tr th {
                background: #22A7F0;
            }</style>");

            builder.Append("<table style='text-align: center;width:100%; ' class='table-striped'>");
            builder.Append("<tr style='color:white'>");
            builder.Append("<th style='text-align:center'>Change</th>");
            builder.Append("<th style='text-align:center'>PKID</th>");
            builder.Append("<th style='text-align:center'>名称</th>");
            builder.Append("<th style='text-align:center'>凝固点</th>");
            builder.Append("<th style='text-align:center'>是否启用</th>");
            builder.Append("<th style='text-align:center'>品牌</th>");
            builder.Append("<th style='text-align:center'>省市</th></tr>");

            if (oldData != null && oldData.Any())
            {
                builder.Append($"<tr><td rowspan={oldData.Count() + 1}>改变前</td></tr>");

                foreach (var item in oldData)
                {
                    builder.Append("<tr>");
                    builder.Append("<td>" + item.PKID + "</td>");
                    builder.Append("<td>防冻液</td>");
                    builder.Append("<td>" + item.FreezingPoint + "</td>");
                    builder.Append("<td>" + (item.Status == 1 ? "启用" : "禁用") + "</td>");
                    builder.Append("<td>" + item.Brand + "</td>");
                    builder.Append("<td>" + item.ProvinceNames + "</td>");
                    builder.Append("</tr>");
                }
            }

            if (newDate != null && newDate.Any())
            {
                builder.Append($"<tr><td rowspan={oldData.Count() + 1}>改变后</td></tr>");
                foreach (var item in newDate)
                {
                    var brand = false;
                    var status = false;
                    var provinceName = false;
                    var oldStatus = oldData.Where(_ => _.PKID.Equals(item.PKID)).Select(x => x.Status).FirstOrDefault();
                    var oldBrand = oldData.Where(_ => _.PKID.Equals(item.PKID)).Select(x => x.Brand).FirstOrDefault();
                    var oldProvinceName = oldData.Where(_ => _.PKID.Equals(item.PKID)).Select(x => x.ProvinceNames).FirstOrDefault();

                    status = String.Equals(item.Status, oldStatus);
                    brand = String.Equals(item.Brand, oldBrand);
                    provinceName = String.Equals(item.ProvinceNames, oldProvinceName);

                    builder.Append("<tr>");
                    builder.Append("<td>" + item.PKID + "</td>");
                    builder.Append("<td>防冻液</td>");
                    builder.Append("<td>" + item.FreezingPoint + "</td>");
                    builder.Append("<td " + (status ? "" : "style='background:#ec9a9a'") + ">" + (item.Status == 1 ? "启用" : "禁用") + "</td>");
                    builder.Append("<td " + (brand ? "" : "style='background:#ec9a9a'") + ">" + item.Brand + "</td>");
                    builder.Append("<td " + (provinceName ? "" : "style='background:#ec9a9a'") + ">" + item.ProvinceNames + "</td>");

                    builder.Append("</tr>");
                }
            }

            builder.Append("</table>");
            builder.Append("<h3><span style='color:red'>" + userName + "</span>修改了保养推荐配置,修改时间：" + DateTime.Now + "</h3>");

            return builder.ToString();
        }

        public static IEnumerable<PrioritySettingModel> GetBaoYangPriorityDetailsByPartName(string partName)
        {
            var resulTable = DALBaoyang.SelectBaoYangPriorityDetailsByPartName(partName);

            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new PrioritySettingModel(dr));
            }
            else
            {
                return new PrioritySettingModel[0];
            }
        }

        public static IEnumerable<BaoYangProductRecommendModel> GetJYPriorityDetailsByPartName(string partName)
        {
            var resulTable = DALBaoyang.SelectJYPriorityDetailsByPartName(partName);

            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new BaoYangProductRecommendModel(dr));
            }
            else
            {
                return new BaoYangProductRecommendModel[0];
            }
        }

        public Tuple<bool, List<BaoYangYearCard>> SelectAllBaoYangYearCard(int pageIndex, int pageSize, string pid, 
            string category, string fuelBrand, string productId)
        {
            List<BaoYangYearCard> result = new List<BaoYangYearCard>();
            var flag = true;
            try
            {
                DataTable data = new DataTable();
                if (!string.IsNullOrWhiteSpace(pid) || !string.IsNullOrWhiteSpace(category) ||
                    !string.IsNullOrWhiteSpace(fuelBrand) || !string.IsNullOrWhiteSpace(productId))
                {
                    data =
                        BYAlwaysOnReadDbScopeManager.Execute(
                            conn =>
                                DalBaoYangYearCard.SelectBaoYangYearCardWithCondition(conn, pageIndex, pageSize, pid, category,
                                    fuelBrand, productId));
                }
                else
                {
                    data = BYAlwaysOnReadDbScopeManager.Execute(
                            conn =>
                                DalBaoYangYearCard.SelectAllBaoYangYearCard(conn, pageIndex, pageSize));
                }
                foreach (DataRow dr in data.Rows)
                {
                    if (result.Count(p => p.Pkid.Equals(Convert.ToInt32(dr["PKID"]))) > 0)
                    {
                        foreach (var item in result.Where(p => p.Pkid.Equals(Convert.ToInt32(dr["PKID"]))))
                        {
                            if (Convert.ToInt32(dr["PromotionCount"]) > item.PromotionCount)
                            {
                                item.PromotionCount = Convert.ToInt32(dr["PromotionCount"]);
                            }
                            if (string.IsNullOrWhiteSpace(dr["ProductName"].ToString())) continue;
                            if (!string.IsNullOrWhiteSpace(item.FuelType))
                            {
                                if (item.FuelType.Split(';').Count(p => p.Equals(dr["ProductName"].ToString())) == 0)
                                {
                                    item.FuelType += dr["ProductName"] + ";";
                                }
                            }
                            else
                            {
                                item.FuelType += dr["ProductName"] + ";";
                            }
                        }
                    }
                    else
                    {
                        BaoYangYearCard item = new BaoYangYearCard
                        {
                            Pkid = Convert.ToInt32(dr["PKID"]),
                            Pid = dr["PID"].ToString(),
                            DisplayName = dr["DisplayName"].ToString(),
                            CategoryName = dr["Category"].ToString(),
                            ProductName = dr["ProductName"].ToString(),
                            PromotionCount = Convert.ToInt32(dr["PromotionCount"]),
                            FuelType = !string.IsNullOrWhiteSpace(dr["ProductName"].ToString()) ? dr["ProductName"] + ";" : string.Empty
                        };
                        result.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                flag = false;
            }
            return new Tuple<bool, List<BaoYangYearCard>>(flag, result);
        }
        /// <summary>
        /// 获取保养类别详细
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BaoYangPackageDescription>> GetBaoYangPackageDescriptionAsync()
        {
            IEnumerable<BaoYangPackageDescription> result = null;
            try
            {
                using (var client = new BaoYangClient())
                {
                    var serviceResult = await client.GetBaoYangPackageDescriptionAsync();
                    serviceResult.ThrowIfException(true);
                    return serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetBaoYangPackageDescriptionAsync", ex);
            }
            return result ?? new List<BaoYangPackageDescription>();
        }
        /// <summary>
        /// 获取所有的保养类别
        /// </summary>
        /// <returns></returns>
        public async Task<AppCategoryResultModel<BaoYangPackageDescriptionModel>> GetAllBaoYangPackageCategoriesAsync()
        {
            using (var client = new BaoYangClient())
            {
                var serviceResult = await client.GetAllBaoYangPackageCategoriesAsync(0, Guid.Empty, null, null);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

        public int SelectBaoYangYearCardCount(string pid, string category, string fuelBrand, string productId)
        {
            var result = 0;
            try
            {
                result = BYAlwaysOnReadDbScopeManager.Execute(conn => DalBaoYangYearCard.SelectBaoYangYearCardCount(conn, pid, 
                                                  category, fuelBrand, productId));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public Tuple<bool, List<string>> SelectAllFuelBrand()
        {
            var flag = true;
            List<string> result = new List<string>();
            try
            {
                result = BYAlwaysOnReadDbScopeManager.Execute(conn=>DalBaoYangYearCard.SelectAllFuelBrand(conn));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                flag = false;
            }
            return new Tuple<bool, List<string>>(flag, result);
        }

        public Tuple<bool, List<string>> SelectAllOilLevel()
        {
            var flag = true;
            List<string> result = new List<string>();
            try
            {
                result = BYAlwaysOnReadDbScopeManager.Execute(conn => DalBaoYangYearCard.SelectAllOilLevel(conn));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                flag = false;
            }
            return new Tuple<bool, List<string>>(flag, result);
        }

        public Tuple<bool, List<string>> SelectOilviscosity()
        {
            var flag = true;
            List<string> result = new List<string>();
            try
            {
                result = BYAlwaysOnReadDbScopeManager.Execute(conn => DalBaoYangYearCard.SelectOilviscosity(conn));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                flag = false;
            }
            return new Tuple<bool, List<string>>(flag, result);
        }

        public Tuple<bool, List<string>> SelectOilBundle(string brand, string level, string viscosity)
        {
            var flag = true;
            List<string> result = new List<string>();
            try
            {
                result = BYAlwaysOnReadDbScopeManager.Execute(conn => DalBaoYangYearCard.SelectOilBundle(conn, brand, level, viscosity));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                flag = false;
            }
            return new Tuple<bool, List<string>>(flag, result);
        }

        public Tuple<string,string> SelectOilDisplayNameByProperty(string brand, string level, string viscosity, string unit)
        {
            Tuple<string, string> result = null;
            try
            {
                result =
                    BYAlwaysOnReadDbScopeManager.Execute(
                        conn => DalBaoYangYearCard.SelectOilDisplayNameByProperty(conn, brand, level, viscosity, unit));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public Tuple<bool, string> IsBaoYangYearCardPidValidate(string pid)
        {
            bool result = true;
            string msg = string.Empty;
            try
            {
                var isExistPid = false;
                var isYearCardPid = false;
                BYAlwaysOnReadDbScopeManager.Execute(conn =>
                {
                    isExistPid = DalBaoYangYearCard.IsExistBaoYangYearCardPid(conn, pid);
                    isYearCardPid = DalBaoYangYearCard.IsInputPidBelongsToYearCardPid(conn, pid);
                });
                if (isExistPid)
                {
                    result = false;
                    msg = "输入的PID已经存在！";
                }
                if (!isYearCardPid)
                {
                    result = false;
                    msg = "输入的PID不是有效的年卡PID！";
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
            }
            return new Tuple<bool, string>(result,msg);
        }

        public bool DeleteBaoYangYearCard(int pkid, string user)
        {
            var result = false;
            try
            {
                var oldYearCard = SelectYearCardInfoByPkid(pkid).FirstOrDefault();
                var oldDetails = SelectBaoYangYearCardDetails(pkid).Item2;  
                var oldShops = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.SelectBaoYangYearCardShops(conn, pkid));
                result = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.DeleteBaoYangYearCard(conn, pkid));
                if (result)
                {
                    YearCardParameter yearCard = new YearCardParameter();
                    if (oldYearCard != null)
                    {
                        yearCard.Pid = oldYearCard.Pid;
                        yearCard.CategoryName = oldYearCard.CategoryName;
                        yearCard.DisplayName = oldYearCard.DisplayName;
                        yearCard.Pkid = oldYearCard.Pkid;
                        yearCard.ImageUrl = oldYearCard.ImageUrl;
                    }
                    dbScopeManagerTuhuLog.CreateTransaction(conn =>
                    {
                        DalBaoYangYearCard.AddLogToBaoYangOprLog(conn, "BaoYangYearCard", yearCard.Pkid.ToString(), JsonConvert.SerializeObject(yearCard), null, user, "Delete");
                        foreach (var detail in oldDetails)
                        {
                            var oldValue = JsonConvert.SerializeObject(detail);
                            DalBaoYangYearCard.AddLogToBaoYangOprLog(conn, "BaoYangYearCardDetail",detail.YearCardId.ToString(), oldValue, null, user, "Delete");
                        }
                        foreach (var shop in oldShops)
                        {
                            var oldValue = JsonConvert.SerializeObject(shop);
                            DalBaoYangYearCard.AddLogToBaoYangOprLog(conn, "BaoYangYearCardShop", shop.YearCardId.ToString(), oldValue, null, user, "Delete");
                        }
                    });                   
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public List<BaoYangYearCard> SelectYearCardInfoByPkid(int pkid)
        {
            List<BaoYangYearCard> result = new List<BaoYangYearCard>();
            try
            {
                result = BYAlwaysOnReadDbScopeManager.Execute(conn => DalBaoYangYearCard.SelectYearCardInfoByPkid(conn, pkid));
                
                foreach (var item in result)
                {
                    var maxCount = result.Max(p => p.PromotionIndex);
                    item.PromotionCount = maxCount;
                    item.PromotionPercentage = item.PromotionPercentage * 100;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public Tuple<bool,List<BaoYangYearCardDetail>> SelectBaoYangYearCardDetails(int pkid)
        {
            bool flag = false;
            List<BaoYangYearCardDetail> result = new List<BaoYangYearCardDetail>();
            try
            {
                result = BYAlwaysOnReadDbScopeManager.Execute(conn => DalBaoYangYearCard.SelectBaoYangYearCardDetails(conn, pkid));
                BaoYangConfigManager configManager = new BaoYangConfigManager();
                var config = configManager.GetBaoYangYearCardConfig();
                foreach (var item in result)
                {
                    foreach (var node in config.BaoYangYearCardConfig)
                    {
                        if (item.PackageType.Equals(node.Type))
                        {
                            item.PackageType = node.Name;
                        }
                        foreach (var content in node.Content)
                        {
                            if (item.BaoYangType.Equals(content.Type))
                            {
                                item.BaoYangType = content.Name;
                            }
                        }
                    }
                    item.PromotionPercentage = item.PromotionPercentage * 100;
                }
                flag = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return new Tuple<bool, List<BaoYangYearCardDetail>>(flag, result);
        }


        public string SelectProductNameByPid(string pid)
        {
            string result = string.Empty;
            try
            {
                result = BYAlwaysOnReadDbScopeManager.Execute(conn => DalBaoYangYearCard.SelectProductNameByPid(conn, pid));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public bool AddBaoYangYearCardShop(List<BaoYangYearCardShop> shops, string user)
        {
            bool result = true;
            try
            {
                var yearCardId = shops.FirstOrDefault().YearCardId;
                var oldShopInfo = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.SelectBaoYangYearCardShops(conn, yearCardId));
                if (oldShopInfo != null && oldShopInfo.Count > 0)
                {
                    var flag = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.DeleteBaoYangYearCardShop(conn, yearCardId));
                    if (flag)
                    {
                        dbScopeManagerTuhuLog.CreateTransaction(conn =>
                        {
                            foreach (var shop in oldShopInfo)
                            {
                                var log = new BaoYangOprLog
                                {
                                    LogType = "BaoYangYearCardShop",
                                    IdentityID = shop.YearCardId.ToString(),
                                    OldValue = JsonConvert.SerializeObject(shop),
                                    NewValue = null,
                                    Remarks = "Delete",
                                    OperateUser = user,
                                };
                                InsertLog("BYOprLog", log);
                            }
                        });
                    }
                    else
                    {
                        return false;
                    }
                }
                dbScopeManagerBY.CreateTransaction(conn =>
                {
                    foreach (var item in shops)
                    {
                        var data = DalBaoYangYearCard.AddBaoYangYearCardShop(conn, item.YearCardId, item.ShopType, item.ShopID);
                        if (!data)
                        {
                            result = false;
                        }
                    }
                });
                if (result)
                {
                    var newShopInfo = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.SelectBaoYangYearCardShops(conn, yearCardId));
                    dbScopeManagerTuhuLog.CreateTransaction(conn =>
                    {
                        foreach (var shop in newShopInfo)
                        {
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangYearCardShop",
                                IdentityID = shop.YearCardId.ToString(),
                                OldValue = null,
                                NewValue = JsonConvert.SerializeObject(shop),
                                Remarks = "Add",
                                OperateUser = user,
                            };
                            InsertLog("BYOprLog", log);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
            }
            return result;
        }

        public bool AddBaoYangYearCardDetail(List<BaoYangYearCardDetail> details, string user)
        {
            bool result = true;
            try
            {
                var yearCardId = details.FirstOrDefault().YearCardId;
                var oldDetailInfo = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.SelectBaoYangYearCardDetails(conn, yearCardId));
                BaoYangConfigManager configManager = new BaoYangConfigManager();
                var config = configManager.GetBaoYangYearCardConfig();
                if (oldDetailInfo != null && oldDetailInfo.Count > 0)
                {
                    var flag = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.DeleteYearCardProjectItem(conn, yearCardId));
                    if (flag)
                    {
                        dbScopeManagerTuhuLog.CreateTransaction(conn =>
                        {
                            foreach (var shop in oldDetailInfo)
                            {
                                var log = new BaoYangOprLog
                                {
                                    LogType = "BaoYangYearCardDetail",
                                    IdentityID = shop.YearCardId.ToString(),
                                    OldValue = JsonConvert.SerializeObject(shop),
                                    NewValue = null,
                                    Remarks = "Delete",
                                    OperateUser = user,
                                };
                                InsertLog("BYOprLog", log);
                            }
                        });
                    }
                    else
                    {
                        return false;
                    }
                }
                dbScopeManagerBY.CreateTransaction(conn =>
                {
                    foreach (var item in details)
                    {
                        foreach (var node in config.BaoYangYearCardConfig)
                        {
                            if (item.PackageType == node.Name)
                            {
                                item.PackageType = node.Type;
                                foreach (var property in node.Content)
                                {
                                    if (item.BaoYangType == property.Name)
                                    {
                                        item.BaoYangType = property.Type;
                                    }
                                }
                            }                            
                        }
                        var data = DalBaoYangYearCard.AddBaoYangYearCardDetail(conn, item);
                        if (!data)
                        {
                            result = false;
                        }
                    }
                });
                if (result)
                {
                    var newDetailInfo = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.SelectBaoYangYearCardDetails(conn, yearCardId));
                    dbScopeManagerTuhuLog.CreateTransaction(conn =>
                    {
                        foreach (var detail in newDetailInfo)
                        {
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangYearCardDetail",
                                IdentityID = detail.YearCardId.ToString(),
                                OldValue = null,
                                NewValue = JsonConvert.SerializeObject(detail),
                                Remarks = "Add",
                                OperateUser = user,
                            };
                            InsertLog("BYOprLog", log);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
            }
            return result;
        }

        public Tuple<bool,int> AddBaoYangYearCard(YearCardParameter card, string user)
        {
            bool result = false;
            var yearCardId = 0;
            try
            {
                var data = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.AddBaoYangYearCard(conn, card));
                if (data > 0)
                {
                    result = true;
                    yearCardId = data;
                    card.Pkid = data;
                    var log = new BaoYangOprLog
                    {
                        LogType = "BaoYangYearCard",
                        IdentityID = card.Pkid.ToString(),
                        OldValue = null,
                        NewValue = JsonConvert.SerializeObject(card),
                        Remarks = "Add",
                        OperateUser = user,
                    };
                    InsertLog("BYOprLog", log);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
            }
            return new Tuple<bool, int>(result, yearCardId);
        }

        public bool UpdateBaoYangYearCardInfo(YearCardParameter card, string user)
        {
            bool result = false;
            try
            {

                var oldCardData = SelectYearCardInfoByPkid(card.Pkid);
                YearCardParameter oldValue = new YearCardParameter();
                if (oldCardData != null && oldCardData.Count > 0)
                {
                    var baoYangYearCard = oldCardData.FirstOrDefault();
                    if (baoYangYearCard != null)
                    {
                        oldValue = new YearCardParameter
                        {
                            Pkid = baoYangYearCard.Pkid,
                            CategoryName = baoYangYearCard.CategoryName,
                            DisplayName = baoYangYearCard.DisplayName,
                            ImageUrl = baoYangYearCard.ImageUrl,
                            Pid = baoYangYearCard.Pid
                        };
                    }
                }
                result = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.UpdateBaoYangYearCardInfo(conn, card));
                if (result)
                {
                    var log = new BaoYangOprLog
                    {
                        LogType = "BaoYangYearCard",
                        IdentityID = card.Pkid.ToString(),
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = JsonConvert.SerializeObject(card),
                        Remarks = "Update",
                        OperateUser = user,
                    };
                    InsertLog("BYOprLog", log);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public List<YearCardConfig> SelectYearCardConfig()
        {
            List<YearCardConfig> result = new List<YearCardConfig>();
            try
            {
                result = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.SelectYearCardConfig(conn));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public bool AddOrUpdateYearCardConfig(List<YearCardConfig> config, string user)
        {
            bool result = true;
            try
            {
                foreach (var item in config)
                {
                    var oldValue =
                        BYAlwaysOnReadDbScopeManager.Execute(
                            conn => DalBaoYangYearCard.SelectYearCardConfigByYearType(conn, item.YearCardType));
                    if (oldValue != null && oldValue.Rows.Count > 0)
                    {
                        var isUpdateSuccess = dbScopeManagerBY.Execute(
                            conn => DalBaoYangYearCard.UpdateYearCardConfig(conn, item.YearCardType, item.Icon, item.PanelImage));
                        if (isUpdateSuccess)
                        {
                            var newValue = dbScopeManagerBY.Execute(
                                conn => DalBaoYangYearCard.SelectYearCardConfigByYearType(conn, item.YearCardType));
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangYearCardConfig",
                                IdentityID = item.YearCardType,
                                OldValue = JsonConvert.SerializeObject(oldValue),
                                NewValue = JsonConvert.SerializeObject(newValue),
                                Remarks = "Update",
                                OperateUser = user,
                            };
                            InsertLog("BYOprLog", log);
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        var isAddSuccess = dbScopeManagerBY.Execute(
                            conn => DalBaoYangYearCard.AddYearCardConfig(conn, item.YearCardType, item.Icon, item.PanelImage));
                        if (isAddSuccess)
                        {
                            var newValue = BYAlwaysOnReadDbScopeManager.Execute(
                                conn => DalBaoYangYearCard.SelectYearCardConfigByYearType(conn, item.YearCardType));
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangYearCardConfig",
                                IdentityID = item.YearCardType,
                                OldValue = null,
                                NewValue = JsonConvert.SerializeObject(newValue),
                                Remarks = "Add",
                                OperateUser = user,
                            };
                            InsertLog("BYOprLog", log);
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
            }
            return result;
        }

        public bool AddOrUpdateYearCardRecommendConfig(List<YearCardConfig> config, string user)
        {
            bool result = true;
            try
            {
                foreach (var item in config)
                {
                    var oldValue =
                        BYAlwaysOnReadDbScopeManager.Execute(
                            conn => DalBaoYangYearCard.SelectYearCardRecommendConfig(conn, item.YearCardType));
                    if (oldValue != null && oldValue.Rows.Count > 0)
                    {
                        var isUpdateSuccess = dbScopeManagerBY.Execute(
                            conn => DalBaoYangYearCard.UpdateYearCardRecommendConfig(conn, item.YearCardType, item.Brands, "机油"));
                        if (isUpdateSuccess)
                        {
                            var newValue = dbScopeManagerBY.Execute(
                                conn => DalBaoYangYearCard.SelectYearCardRecommendConfig(conn, item.YearCardType));
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangYearCardRecommendConfig",
                                IdentityID = item.YearCardType,
                                OldValue = JsonConvert.SerializeObject(oldValue),
                                NewValue = JsonConvert.SerializeObject(newValue),
                                Remarks = "Update",
                                OperateUser = user,
                            };
                            InsertLog("BYOprLog", log);
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        var isAddSuccess = dbScopeManagerBY.Execute(
                            conn => DalBaoYangYearCard.AddYearCardRecommendConfig(conn, item.YearCardType, item.Brands, "机油"));
                        if (isAddSuccess)
                        {
                            var newValue = BYAlwaysOnReadDbScopeManager.Execute(
                                conn => DalBaoYangYearCard.SelectYearCardRecommendConfig(conn, item.YearCardType));
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangYearCardRecommendConfig",
                                IdentityID = item.YearCardType,
                                OldValue = null,
                                NewValue = JsonConvert.SerializeObject(newValue),
                                Remarks = "Add",
                                OperateUser = user,
                            };
                            InsertLog("BYOprLog", log);
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
            }
            return result;
        }

        public bool UpdateBaoYangYearcardConfig()
        {
            bool result = false;

            using (var client = new CacheClient())
            {
                var updateConfig = client.UpdateBaoYangYearcardConfig();
                if (updateConfig.Result)
                {
                    result = true;
                    
                }
                else
                {
                    updateConfig.ThrowIfException(true);
                }
            }
            return result;
        }

        public bool AddYearCardPromotionPercentage(List<BaoYangYearCardPromotion> promotion, string user)
        {
            bool result = true;
            try
            {
                var yearCardId = promotion.FirstOrDefault().YearCardId;
                var oldPromotionInfo = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.SelectBaoYangYearCardPromotion(conn, yearCardId));
                if (oldPromotionInfo != null && oldPromotionInfo.Count > 0)
                {
                    var flag = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.DeleteYearCardPromotionPercentage(conn, yearCardId));
                    if (flag)
                    {
                        dbScopeManagerTuhuLog.CreateTransaction(conn =>
                        {
                            foreach (var promotionPercentage in oldPromotionInfo)
                            {
                                var log = new BaoYangOprLog
                                {
                                    LogType = "BaoYangYearCardPromotionPercentage",
                                    IdentityID = promotionPercentage.YearCardId.ToString(),
                                    OldValue = JsonConvert.SerializeObject(promotionPercentage),
                                    NewValue = null,
                                    Remarks = "Delete",
                                    OperateUser = user,
                                };
                                InsertLog("BYOprLog", log);
                            }                            
                        });
                    }
                    else
                    {
                        return false;
                    }
                }
                dbScopeManagerBY.CreateTransaction(conn =>
                {
                    foreach (var item in promotion)
                    {
                        var data = DalBaoYangYearCard.AddYearCardPromotionPercentage(conn, item);
                        if (!data)
                        {
                            result = false;
                        }
                    }
                });
     
                if (result)
                {
                    var newPromotionInfo = dbScopeManagerBY.Execute(conn => DalBaoYangYearCard.SelectBaoYangYearCardPromotion(conn, yearCardId));
                    dbScopeManagerTuhuLog.CreateTransaction(conn =>
                    {
                        foreach (var promotionPercentage in newPromotionInfo)
                        {
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangYearCardPromotionPercentage",
                                IdentityID = promotionPercentage.YearCardId.ToString(),
                                OldValue = null,
                                NewValue = JsonConvert.SerializeObject(promotionPercentage),
                                Remarks = "Add",
                                OperateUser = user,
                            };
                            InsertLog("BYOprLog", log);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
            }
            return result;
        }

        public void InsertLog(string type, object data)
        {
            var dict = new Dictionary<string, string>
            {
                ["type"] = type,
                ["data"] = JsonConvert.SerializeObject(data),
            };

            using (var client = new ConfigLogClient())
            {
                var status = client.InsertDefaultLogQueue(type, JsonConvert.SerializeObject(data));
                if (!status.Success)
                {
                    status.ThrowIfException(true);
                }
            }
        }

        public Tuple<bool, object> SelectBaoYangYearCardInfo(long orderId)
        {
            var flag = false;
            object obj = null;
            try
            {
                using (var client = new BaoYangClient())
                {
                    var result = client.GetYearCardByOrderId(orderId);
                    if (!result.Success)
                    {
                        result.ThrowIfException(true);
                    }
                    flag = true;
                    obj = result.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                flag = false;
            }
            return Tuple.Create(flag, obj);
        }

        public bool EditYearCardInfo(string promotionCode, string cardItemStr, string PID, string user)
        {
            Tuhu.Service.BaoYang.Models.YearCard.UserYearCardPromotionData promotionData = null;

            using (var client = new BaoYangClient())
            {
                var result = client.GetUserYearCardPromotionDataByPromotionCode(promotionCode);
                if (!result.Success)
                {
                    result.ThrowIfException(true);
                }
                promotionData = result.Result;
            }

            if (promotionData == null)
            {
                return false;
            }
            var oldData = promotionData.Data;
            promotionData.Data = null;

            //找到要修改的CardItem
            var cardItem = JsonConvert.DeserializeObject<Tuhu.Service.BaoYang.Models.YearCard.YearCardDetailConfig>(cardItemStr);
            var updateCardItem = promotionData.CardItems.First(x =>
                    string.Equals(x.BaoYangType, cardItem.BaoYangType) &&
                    string.Equals(x.PackageType, cardItem.PackageType) &&
                    string.Equals(x.PID, cardItem.PID) &&
                    x.ProductCount == cardItem.ProductCount &&
                    x.PromotionIndex == cardItem.PromotionIndex &&
                    x.YearCardId == cardItem.YearCardId);
            updateCardItem.PID = PID;

            var success = false;
            using (var client = new BaoYangClient())
            {
                var result = client.EditYearCardInfo(promotionData);
                if (!result.Success)
                {
                    result.ThrowIfException(true);
                }
                success = result.Result;
            }

            if (success)
            {
                Dictionary<string, object> promotionDataDict = new Dictionary<string, object>();
                promotionDataDict.Add("carditems", JsonConvert.SerializeObject(promotionData.CardItems));
                promotionDataDict.Add("shopconfig", JsonConvert.SerializeObject(promotionData.ShopConfig));
                string newData = JsonConvert.SerializeObject(promotionDataDict);
                var log = new BaoYangOprLog
                {
                    LogType = "BaoYangYearCardEditJiYouPID",
                    IdentityID = promotionData.PromotionCode,
                    OldValue = oldData,
                    NewValue = newData,
                    Remarks = "Update",
                    OperateUser = user,
                };
                InsertLog("BYOprLog", log);
            }
            return success;
        }

        public int CreateYearCardPromotionCode(long orderId)
        {
            Service.BaoYang.Models.YearCard.UserYearCard userYearCardData = null;
            using (var client = new BaoYangClient())
            {
                var result = client.GetYearCardByOrderId(orderId);
                if (!result.Success)
                {
                    result.ThrowIfException(true);
                }
                userYearCardData = result.Result;
            }
            if (userYearCardData != null)
            {
                return -1;//已经存在
            }

            var success = false;
            using (var client = new BaoYangClient())
            {
                var result = client.CreateYearCardPromotionCodes(orderId);
                result.ThrowIfException(true);
                success = result.Result;
            }
            return success ? 1 : 0;
        }



        #region 机油品牌推荐优先级--指定手机号

        public Tuple<List<OilBrandPhonePriorityModel>, int> SelectOilBrandPhonePriority(string phoneNumber,
           string brand, int pageIndex, int pageSize)
        {
            return DALBaoyang.SelectOilBrandPhonePriority(phoneNumber, brand, pageIndex, pageSize);
        }

        public bool? AddOilBrandPhonePriority(List<OilBrandPhonePriorityModel> models,string user)
        {
            bool? result = null;            
            try
            {
                
                var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var dbhelper = new Tuhu.Component.Common.SqlDbHelper(conn))
                {                   
                    List<int> pkids = new List<int>();   
                    dbhelper.BeginTransaction();
                    foreach (var model in models)
                    {
                        if (model.PKID > 0)
                        {
                            return false;
                        }  
                        int pkid = DALBaoyang.AddOilBrandPhonePriority(dbhelper,model);                       
                        if (pkid > 0)
                        {
                            pkids.Add(pkid);
                          
                        }
                    }
                    if(pkids.Count!= models.Count)
                    {
                        result = false;
                    }                   
                    dbhelper.Commit();
                    result = true;
                   for(int i=0;i< models.Count();i++)
                    {
                        OilBrandPhonePriorityModel newValue = new OilBrandPhonePriorityModel
                        {
                            PKID = pkids[i],
                            PhoneNumber = models[i].PhoneNumber,
                            Brand = models[i].Brand,
                            CreateDateTime = DateTime.Now,
                            LastUpdateDateTime = DateTime.Now
                        };
                        var log = new BaoYangOprLog
                        {
                            LogType = "OilBrandPhonePriority",
                            IdentityID = newValue.PhoneNumber,
                            OldValue = "",
                            NewValue = JsonConvert.SerializeObject(newValue),
                            Remarks = "Add",
                            OperateUser = user,
                        };
                        InsertLog("BYOprLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public bool? DeleteOilBrandPhonePriorityByPKID(int pkid, string user)
        {
            bool? result = null;
            try
            {
                var oldValue = DALBaoyang.GetOilBrandPhonePriorityByPKID(pkid);
                if(oldValue==null)
                {
                    return false;
                }
                result = DALBaoyang.DeleteOilBrandPriorityByPKID(pkid);
                if (result == true)
                {
                    var log = new BaoYangOprLog
                    {
                        LogType = "OilBrandPhonePriority",
                        IdentityID = oldValue.PhoneNumber,
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = "",
                        Remarks = "Delete",
                        OperateUser = user,
                    };
                    InsertLog("BYOprLog", log);
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public bool? EditOilBrandPhonePriority(OilBrandPhonePriorityModel model, string user)
        {
            bool? result = null;
            try
            {
                if(model.PKID<1)
                {
                    return false;
                }
                var oldValue = DALBaoyang.GetOilBrandPhonePriorityByPKID(model.PKID);
                result = DALBaoyang.EditOilBrandPhonePriority(model);
                if (result == true)
                {
                    var newValue = new OilBrandPhonePriorityModel
                    {
                        PKID = model.PKID,
                        PhoneNumber = model.PhoneNumber,
                        Brand = model.Brand,                       
                        CreateDateTime = oldValue.CreateDateTime,
                        LastUpdateDateTime = DateTime.Now                       
                    };
                    var log = new BaoYangOprLog
                    {
                        LogType = "OilBrandPhonePriority",
                        IdentityID = model.PhoneNumber,
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = JsonConvert.SerializeObject(newValue),
                        Remarks = "Update",
                        OperateUser = user,
                    };
                    InsertLog("BYOprLog", log);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        #endregion

        #region 机油品牌推荐优先级--指定城市

        public Tuple<List<OilBrandRegionPriorityModel>, int> SelectOilBrandRegionPriority(int provinceId,
          int regionId, string brand, int pageIndex, int pageSize)
        {
            return DALBaoyang.SelectOilBrandRegionPriority(provinceId, regionId, brand, pageIndex, pageSize);
        }

        public bool? AddOilBrandRegionPriority(OilBrandRegionPriorityModel model, string user)
        {
            bool? result = null;
            try
            {

                if (model.PKID > 0)
                {
                    return false;
                }
                int pkid = DALBaoyang.AddOilBrandRegionPriority(model);
                result = pkid > 0 ? true : false;
                if (pkid > 0)
                {
                    OilBrandRegionPriorityModel newValue = new OilBrandRegionPriorityModel
                    {
                        PKID = pkid,
                        ProvinceId = model.ProvinceId,
                        ProvinceName=model.ProvinceName,
                        RegionId=model.RegionId,
                        CityName=model.CityName,
                        Brand = model.Brand,
                        CreateDateTime = DateTime.Now,
                        LastUpdateDateTime = DateTime.Now
                    };
                    var log = new BaoYangOprLog
                    {
                        LogType = "OilBrandRegionPriority",
                        IdentityID = newValue.RegionId.ToString(),
                        OldValue = "",
                        NewValue = JsonConvert.SerializeObject(newValue),
                        Remarks = "Add",
                        OperateUser = user,
                    };
                    InsertLog("BYOprLog", log);
                }              
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public bool? DeleteOilBrandRegionPriorityByPKID(int pkid, string user)
        {
            bool? result = null;
            try
            {
                var oldValue = DALBaoyang.GetOilBrandRegionPriorityByPKID(pkid);
                if (oldValue == null)
                {
                    return false;
                }
                result = DALBaoyang.DeleteOilBrandPriorityByPKID(pkid);
                if (result == true)
                {
                    var log = new BaoYangOprLog
                    {
                        LogType = "OilBrandRegionPriority",
                        IdentityID = oldValue.RegionId.ToString(),
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = "",
                        Remarks = "Delete",
                        OperateUser = user,
                    };
                    InsertLog("BYOprLog", log);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }


        public bool? EditOilBrandRegionPriority(OilBrandRegionPriorityModel model, string user)
        {
            bool? result = null;
            try
            {
                if (model.PKID < 1)
                {
                    return false;
                }
                var oldValue = DALBaoyang.GetOilBrandRegionPriorityByPKID(model.PKID);
                result = DALBaoyang.EditOilBrandRegionPriority(model);
                if (result == true)
                {
                    var newValue = new OilBrandRegionPriorityModel
                    {
                        PKID = model.PKID,
                        ProvinceId = model.ProvinceId,
                        ProvinceName = model.ProvinceName,
                        RegionId = model.RegionId,
                        CityName = model.CityName,
                        Brand = model.Brand,
                        CreateDateTime = oldValue.CreateDateTime,
                        LastUpdateDateTime = DateTime.Now
                    };
                    var log = new BaoYangOprLog
                    {
                        LogType = "OilBrandRegionPriority",
                        IdentityID = model.RegionId.ToString(),
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = JsonConvert.SerializeObject(newValue),
                        Remarks = "Update",
                        OperateUser = user,
                    };
                    InsertLog("BYOprLog", log);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        #endregion

        #region 机油品牌优先级配置--用户购买过的品牌
        /// <summary>
        /// 获取用户购买过的机油
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<OilBrandUserOrderViewModel>, int> SelectOilBrandUserOrder
            (string phoneNumber, Guid userId, int pageIndex, int pageSize)
        {
            var result = null as List<OilBrandUserOrderViewModel>;
            var totalCount = 0;
            try
            {
                if (userId != Guid.Empty)
                {
                    var searchResult = TuhuBiReadDbScopeManager.Execute(conn =>
                       DALBaoyang.SelectOilBrandUserOrder(conn, userId, pageIndex, pageSize))
                       ?? Tuple.Create(new List<OilBrandUserOrderViewModel>(), 0);
                    result = searchResult.Item1;
                    if (result != null && result.Any())
                    {
                        using (var vehicleClient = new Tuhu.Service.Vehicle.VehicleClient())
                        {
                            result = result.Select(v =>
                            {
                                if (string.IsNullOrWhiteSpace(v.VehicleID))
                                    return v;
                                else
                                {
                                    var info = vehicleClient.FetchVehicleByVehicleId(v.VehicleID).Result;//二级车型信息填充
                                    return new OilBrandUserOrderViewModel()
                                    {
                                        UserId = v.UserId,
                                        PhoneNumber = phoneNumber,
                                        VehicleID = v.VehicleID,
                                        Brand = info?.Brand,
                                        Vehicle = info?.Vehicle,
                                        PaiLiang = v.PaiLiang,
                                        Nian = v.Nian,
                                        Pid = v.Pid,
                                        OrderId = v.OrderId
                                    };
                                }
                            }).ToList();
                        }
                    }
                    totalCount = searchResult.Item2;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return Tuple.Create(result, totalCount);
        }
        #endregion

        #region 查看日志
        public Tuple<bool, List<BaoYangOprLog>> SelectBaoYangOprLog(string logType, string identityID, string operateUser, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, out int totalCount)
        {
            int count = 0;
            bool flag = true;
            List<BaoYangOprLog> list = null;
            try
            {
                list = DALBaoyang.SelectBaoYangOprLog(logType, identityID, operateUser, startTime, endTime, pageIndex, pageSize, out count);

            }
            catch (Exception ex)
            {
                flag = false;
                logger.Error(ex);
            }
            totalCount = count;
            return Tuple.Create(flag, list);
        }
        public BaoYangOprLog GetBaoYangOprLogByPKID(int pkid)
        {
            BaoYangOprLog result = null;
            try
            {
                result = DALBaoyang.GetBaoYangOprLogByPKID(pkid);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }



        #endregion


        #region 清除缓存
        public bool CleanOilBrandPriorityCache(string oilBrandPriorityType)
        {
            var key = string.Format("Priority/Oil/Brand/{0}", oilBrandPriorityType);
            return CleanBaoYangCaches(new[] { key });
        }

        public bool CleanBaoYangCaches(IEnumerable<string> keys)
        {
            try
            {
                using (var client = new Tuhu.Service.BaoYang.CacheClient())
                {
                    var result = client.Remove(keys);
                    if (!result.Success)
                    {
                        result.ThrowIfException(true);
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Vehicle SettingNew

        public Tuple<int, List<FullPriorityVehicleSettingNew>> GetBaoYangPriorityVehicleSettingNew(VehicleSettingNewSearchRequest request)
        {
            int total = 0;
            List<FullPriorityVehicleSettingNew> list = null;
            try
            {
                List<PriorityVehicleSettingNew> settings = null;
                List<PriorityVehicleOilModel> oilList = null;
                GRAlwaysOnReadDbScopeManager.Execute(conn =>
                {
                    var temps = DALBaoyang.SelectBaoYangVehicleSettingNewOil(conn, request);
                    list = temps.Item2.ToList();
                    total = temps.Item1;
                    var vehicleIds = temps.Item2.Select(x => x.VehicleId).ToList();
                    settings = DALBaoyang.SelectBaoYangVehicleSettingNew(conn, vehicleIds, new List<string> { request.PartName });
                    oilList = DALBaoyang.SelectBaoYangVehicleOilModel(conn, vehicleIds);
                });

                list.ForEach(x =>
                {
                    x.Settings = settings.Where(setting => setting.VehicleId == x.VehicleId).ToList();
                    x.Viscosities = oilList.Where(viscosity => viscosity.VehicleId == x.VehicleId)
                                           .Select(viscosity => viscosity.Viscosity).Distinct().ToList();
                    x.Grades = oilList.Where(grade => grade.VehicleId == x.VehicleId)
                                      .Select(grade => grade.Grade).Distinct().ToList();
                });

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return Tuple.Create(total, list ?? new List<FullPriorityVehicleSettingNew>());
        }

        public bool AddOrEditPriorityVehicleSettingNew(List<PriorityVehicleSettingNew> settings, string name)
        {
            bool success = false;
            try
            {
                var vehicleIds = settings.Select(x => x.VehicleId).Distinct().ToList();
                var partNames = settings.Select(x => x.PartName).Distinct().ToList();

                var existsSettings = GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.SelectBaoYangVehicleSettingNew(conn, vehicleIds, partNames));
                var idList = existsSettings.Select(x => x.PKID).ToList();

                dbScopeManagerBY.CreateTransaction(conn =>
                {
                    var deleteSuccess = idList.Any() ? DALBaoyang.DelBaoYangVehicleSettingNewByPkids(conn, idList) : true;
                    if (deleteSuccess)
                    {
                        settings.ForEach(setting => DALBaoyang.AddPriorityVehicleSettingNew(conn, setting));
                    }
                    success = true;
                });
                if (success)
                {
                    var newList = PriorityVehicleSettingNewViewModel.GetList(settings).ToList();
                    var oldList = PriorityVehicleSettingNewViewModel.GetList(existsSettings);
                    var logs = new List<BaoYangOprLog>();
                    newList.ForEach(x =>
                    {
                        var item = oldList.FirstOrDefault(s => string.Equals(s.VehicleId, x.VehicleId, StringComparison.OrdinalIgnoreCase));
                        var log = new BaoYangOprLog
                        {
                            IdentityID = $"{x.VehicleId}-{x.PartName}",
                            OldValue = item == null ? string.Empty : JsonConvert.SerializeObject(item),
                            NewValue = JsonConvert.SerializeObject(x),
                            Remarks = "修改",
                            OperateUser = name,
                        };
                        logs.Add(log);
                    });
                    InsertVehicleSettingsOprLog(logs);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return success;
        }
        public bool RemoveCache(IEnumerable<string> keys) {
            bool result = false;
            try
            {
                using (var client = new Service.BaoYang.CacheClient())
                { 
                    var serviceResult = client.Remove(keys);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            catch (Exception ex)
            { 
                logger.Error(ex.Message, ex);
            } 
            return result;
        }
        public bool DelPriorityVehicleSettingNew(List<string> vehicleIds, string partName, string name)
        {
            bool success = false;
            try
            {
                var settings = GRAlwaysOnReadDbScopeManager.Execute(conn =>
                {
                    return DALBaoyang.SelectBaoYangVehicleSettingNew(conn, vehicleIds, new List<string> { partName });
                });
                if (settings != null && settings.Any())
                {
                    dbScopeManagerBY.CreateTransaction(conn =>
                    {
                        success = DALBaoyang.DelBaoYangVehicleSettingNew(conn, vehicleIds, partName);
                    });
                }
                if (success)
                {
                    var logs = PriorityVehicleSettingNewViewModel.GetList(settings).Select(x => new BaoYangOprLog
                    {
                        IdentityID = $"{x.VehicleId}-{x.PartName}",
                        OldValue = JsonConvert.SerializeObject(x),
                        NewValue = string.Empty,
                        Remarks = "删除",
                        OperateUser = name,
                    }).ToList();
                    InsertVehicleSettingsOprLog(logs);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return success;
        }

        public bool EnableOrDisableVehicleSettingNew(List<string> vehicleIds, string partName, bool isEnabled, string name)
        {
            bool success = false;
            try
            {
                var settings = GRAlwaysOnReadDbScopeManager.Execute(conn =>
                {
                    return DALBaoyang.SelectBaoYangVehicleSettingNew(conn, vehicleIds, new List<string> { partName });
                });
                if (settings != null && settings.Any())
                {
                    dbScopeManagerBY.CreateTransaction(conn =>
                    {
                        success = DALBaoyang.EnableOrDisableVehicleSettingNew(conn, vehicleIds, partName, isEnabled);
                    });
                }
                if (success)
                {
                    var logs = PriorityVehicleSettingNewViewModel.GetList(settings).Select(x =>
                    {
                        var oldData = JsonConvert.SerializeObject(x);
                        x.IsEnabled = isEnabled;
                        var newData = JsonConvert.SerializeObject(x);
                        return new BaoYangOprLog
                        {
                            IdentityID = $"{x.VehicleId}-{x.PartName}",
                            OldValue = oldData,
                            NewValue = newData,
                            Remarks = isEnabled ? "启用" : "禁用",
                            OperateUser = name,
                        };
                    }).ToList();

                    InsertVehicleSettingsOprLog(logs);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return success;
        }

        public List<PriorityVehicleSettingNew> GetPriorityVehicleSettings(string vehicleId, string partName)
        {
            List<PriorityVehicleSettingNew> result = null;
            try
            {
                result = GRAlwaysOnReadDbScopeManager.Execute(conn =>
                     DALBaoyang.SelectBaoYangVehicleSettingNew(conn, new List<string> { vehicleId }, new List<string> { partName })
                );
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result ?? new List<PriorityVehicleSettingNew>();
        }

        public Dictionary<string, List<string>> GetOilBrandAndSeries()
        {
            Dictionary<string, List<string>> result = null;
            try
            {
                result = GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.SelectOilBrandAndSeries(conn));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result ?? new Dictionary<string, List<string>>();
        }

        public bool InsertVehicleSettingsOprLog(List<BaoYangOprLog> logs)
        {
            if (logs != null && logs.Any())
            {
                logs.ForEach(log =>
                {
                    log.LogType = "BaoYangPriorityVehicleSettingsNew";
                    LoggerManager.InsertLog("BYOprLog", log);
                });
                return true;
            }
            return false;
        }

        #endregion

        #region Product SettingNew

        public List<ProductPrioritySettingNew> GetProductPrioritySettingsNew(string partName)
        {
            List<ProductPrioritySettingNew> result = null;
            try
            {
                result = GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.SelectProductPrioritySettingsNew(conn, new[] { partName }))?.OrderBy(x=>x.Priority).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result ?? new List<ProductPrioritySettingNew>();
        }

        public bool AddProductPrioritySettingsNew(List<ProductPrioritySettingNew> settings, string name)
        {
            bool success = false;
            try
            {
                var partNames = settings.Select(x => x.PartName).Distinct();
                var list = GRAlwaysOnReadDbScopeManager.Execute(conn => DALBaoyang.SelectProductPrioritySettingsNew(conn, partNames)).ToList();

                settings.ForEach(x =>
                {
                    var item = list.FirstOrDefault(o => string.Equals(x.PartName, o.PartName) &&
                                                        string.Equals(x.PriorityType, o.PriorityType) &&
                                                        x.Priority == o.Priority);
                    if (item != null)
                    {
                        x.PKID = item.PKID;
                    }
                });

                var updatePkids = settings.Select(x => x.PKID).Distinct().ToList();
                var pkids = list.Where(x => !updatePkids.Contains(x.PKID)).Select(x => x.PKID).ToList();

                dbScopeManagerBY.CreateTransaction(conn =>
                {
                    var deleteSuccess = pkids.Any() ? DALBaoyang.DeleteProductPrioritySettingsNew(conn, pkids) : true;
                    if (deleteSuccess)
                    {
                        foreach (var setting in settings)
                        {
                            if (setting.PKID <= 0)
                            {
                                setting.PKID = DALBaoyang.AddProductPrioritySettingsNew(conn, setting);
                                success = setting.PKID > 0;
                            }
                            else
                            {
                               success = DALBaoyang.UpdateProductPrioritySettingNew(conn, setting);
                            }
                        }
                    }
                });
                if (success)
                {
                    InsertProductPrioritySettingsLogs(list, settings, name);
                    SendEmailToUserForSettingNew(list, settings, name);
                }
            }
            catch (Exception ex)
            {
                success = false;
                logger.Error(ex.Message, ex);
            }
            return success;
        }


        /// <summary>
        /// 保存推荐排序
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool SaveProductPrioritySettings(List<ProductPrioritySettingNew> settings, string name)
        {
            var success = false;
            try
            {
                // 数据库拉出数据  去重复  排序操作
                var items = GetProductPrioritySettingsNew(settings.FirstOrDefault().PartName);

                var updateIds = new List<int>();
                foreach (var setting in settings)
                {
                    var item = items.FirstOrDefault(o =>
                        string.Equals(setting.PartName, o.PartName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(setting.Brand, o.Brand, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(setting.Series, o.Series, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(setting.PriorityType, o.PriorityType, StringComparison.OrdinalIgnoreCase)
                    );
                    if (item != null)
                    {
                        setting.PKID = item.PKID;
                        updateIds.Add(item.PKID);
                    }
                }

                //获取需要删除的数据 
                var deleteIds = items.Where(x => !updateIds.Contains(x.PKID)).Select(x => x.PKID).ToList();

                dbScopeManagerBY.CreateTransaction(conn =>
                {
                    var deleteSuccess = deleteIds.Any() ? DALBaoyang.DeleteProductPrioritySettingsNew(conn, deleteIds) : true;

                    if (deleteSuccess)
                    {
                        foreach (var setting in settings)
                        {
                            if (setting.PKID == 0)
                            {
                                setting.PKID = DALBaoyang.AddProductPrioritySettingsNew(conn, setting);
                                success = setting.PKID > 0;
                            }
                            else
                            {
                                success = DALBaoyang.UpdateProductPrioritySettingNew(conn, setting);
                            }
                        }
                    }
                });

                if (success)
                {
                    //发送邮件
                    var partNames = settings.Select(x => x.PartName).Distinct();
                    InsertProductPrioritySettingsLogs(items, settings, name);
                    SendEmailToUserForSettingNew(items, settings, name);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);

            }
            return success;
        }

        public bool InsertProductPrioritySettingsLogs(List<ProductPrioritySettingNew> oilList, 
            List<ProductPrioritySettingNew> newList, string name)
        {
            var success = false;
            var @old = oilList.GroupBy(x => new { x.PartName, x.PriorityType }).Select(g => new
            {
                g.Key.PartName,
                g.Key.PriorityType,
                Priorities = g.OrderBy(x => x.Priority).Select(x => new
                {
                    x.Brand,
                    x.Series,
                    x.PID,
                    x.Priority,
                    x.IsEnabled,
                }),
            });
            var @new = newList.GroupBy(x => new { x.PartName, x.PriorityType }).Select(g => new
            {
                g.Key.PartName,
                g.Key.PriorityType,
                Priorities = g.OrderBy(x => x.Priority).Select(x => new
                {
                    x.Brand,
                    x.Series,
                    x.PID,
                    x.Priority,
                    x.IsEnabled,
                }),
            });
            var leftJoin = from x in @old
                           join y in @new on new { x.PartName, x.PriorityType } equals new { y.PartName, y.PriorityType } into temp
                           from z in temp.DefaultIfEmpty()
                           select new
                           {
                               x.PartName,
                               x.PriorityType,
                               @old = JsonConvert.SerializeObject(x),
                               @new = z == null ? string.Empty : JsonConvert.SerializeObject(z),
                           };
            var right = from x in @new
                        join y in @old on new { x.PartName, x.PriorityType } equals new { y.PartName, y.PriorityType } into temp
                        from z in temp.DefaultIfEmpty()
                        select new
                        {
                            x.PartName,
                            x.PriorityType,
                            @old = z == null ? string.Empty : JsonConvert.SerializeObject(z),
                            @new = JsonConvert.SerializeObject(x),
                        };
            var fullJoin = leftJoin.Union(right);
            var logs = fullJoin.Select(x => new BaoYangOprLog
            {
                IdentityID = $"{x.PartName}-{x.PriorityType}",
                LogType = "BaoYangProductPrioritySettingNew",
                NewValue = x.@new,
                OldValue = x.@old,
                OperateUser = name,
                Remarks = string.IsNullOrEmpty(x.@old) ? "新增" : (string.IsNullOrEmpty(x.@new) ? "删除" : "修改"),
            }).ToList();

            if (logs != null && logs.Any())
            {
                logs.ForEach(log =>
                {
                    LoggerManager.InsertLog("BYOprLog", log);
                });
                success = true;
            }
            return success;
        }

        public static void SendEmailToUserForSettingNew(List<ProductPrioritySettingNew> oilList, 
            List<ProductPrioritySettingNew> newList, string userName)
        {
            if (newList == null)
            {
                throw new ArgumentNullException(nameof(newList));
            }

            var configManager = new BaoYangConfigManager();
            var result = configManager.GetEmailPersonsConfig();//获取邮件接收的人员配置信息
            var html = GetHtml(oilList, newList, userName);//转换为html
            List<string> recevierStr = null;
            if (result != null)
            {
                recevierStr = string.Join(",", result.BaoYangRecommendConfigReceiver).Split(',').ToList();
            }
            if (recevierStr != null && recevierStr.Any())
            {
                foreach (var item in recevierStr)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        Send("保养产品推荐配置修改通知", html, item);
                    }
                }
            }
        }

        private static string GetHtml(List<ProductPrioritySettingNew> oldList, List<ProductPrioritySettingNew> newList, string userName)
        {
            StringBuilder html = new StringBuilder();


            var full = FullJoin(oldList, newList, x => new { x.PartName, x.PriorityType, x.Priority }, (x, y) => new
            {
                PartName = x?.PartName ?? y?.PartName,
                PriorityType = x?.PriorityType ?? y?.PriorityType,
                Priority = x?.Priority ?? y?.Priority ?? 0,
                PKID = x?.PKID ?? y?.PKID ?? 0,
                Before = x == null ? null : new
                {
                    x.Brand,
                    x.Series,
                    x.PID,
                    x.IsEnabled
                },
                After = y == null ? null : new
                {
                    y.Brand,
                    y.Series,
                    y.PID,
                    y.IsEnabled
                },
            });

            var result = full.GroupBy(x => new { x.PartName, x.PriorityType }).Select(g => new
            {
                g.Key.PartName,
                g.Key.PriorityType,
                Priorities = g.Select(x => new
                {
                    x.After,
                    x.Before,
                    x.Priority,
                    x.PKID,
                }).ToList(),
            });

            #region begin

            html.Append(@"<style type='text/css'>
    table.table-striped { text-align: center; width: 100%; }
    table.table-striped tr:nth-child(odd) { background-color: #EEEEEE; }
    table.table-striped tr:hover:nth-child(odd) { background: #FFEC8B; }
    table.table-striped tr:hover:nth-child(even) { background: #FFEC8B; }
    table.table-striped tr th { text-align: center; color: white; }
    table.table-striped tr th { background: #22A7F0; }
    table.table-striped tr td.diff { background: #ec9a9a; }
    table.table-striped tr td.red { background: red; }
</style>
<table border='0' cellpadding='0' cellspacing='1' class='table-striped'>
    <thead>
        <tr>
            <th rowspan='2'>名称</th>
            <th rowspan='2'>Type</th>
            <th rowspan='2'>PKID</th>
            <th colspan='4'>改变前</th>
            <th colspan='4'>改变后</th>
            <th rowspan='2'>优先级</th>
        </tr>
        <tr>
            <th>是否启用</th>
            <th>品牌</th>
            <th>系列</th>
            <th>PID</th>
            <th>是否启用</th>
            <th>品牌</th>
            <th>系列</th>
            <th>PID</th>
        </tr>
    </thead>
    <tbody>");

            #endregion

            foreach (var item in result)
            {
                if (item.Priorities.Any())
                {
                    var rowspan = item.Priorities.Count + 1;
                    html.Append($"<tr><td rowspan='{rowspan}'>{item.PartName}</td>");
                    html.Append($"<td rowspan='{rowspan}'>{item.PriorityType}</td></tr>");
                    foreach (var priority in item.Priorities)
                    {
                        var before = priority?.Before;
                        var after = priority?.After;
                        html.Append($"<tr><td>{priority.PKID}</td>");
                        html.Append($"<td>{before?.IsEnabled}</td>");
                        html.Append($"<td>{before?.Brand}</td>");
                        html.Append($"<td>{before?.Series}</td>");
                        html.Append($"<td>{before?.PID}</td>");
                        html.Append($"<td class='{(before?.IsEnabled != after?.IsEnabled ? "red" : "")}'>{after?.IsEnabled}</td>");
                        html.Append($"<td class='{(before?.Brand != after?.Brand ? "diff" : "")}'>{after?.Brand}</td>");
                        html.Append($"<td class='{(before?.Series != after?.Series ? "diff" : "")}'>{after?.Series}</td>");
                        html.Append($"<td class='{(before?.PID != after?.PID ? "diff" : "")}'>{after?.PID}</td>");
                        html.Append($"<td>{priority.Priority}</td></tr>");
                    }
                }
                else
                {
                    html.Append($"<tr><td>{item.PartName}</td>");
                    html.Append($"<td>{item.PriorityType}</td>");
                    html.Append($"<td></td>");
                    html.Append($"<td></td>");
                    html.Append($"<td></td>");
                    html.Append($"<td></td>");
                    html.Append($"<td></td>");
                    html.Append($"<td></td>");
                    html.Append($"<td></td>");
                    html.Append($"<td></td>");
                    html.Append($"<td></td>");
                    html.Append($"<td></td></tr>");
                }
            }

            #region end

            html.Append("</tbody></table>");
            html.Append($"<h3><span style='color:red'>{userName}</span>修改了保养推荐配置,修改时间：{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}</h3>");

            #endregion

            return html.ToString();
        }

        private static IEnumerable<TResult> FullJoin<TLeft, TRight, TKey, TResult>(
            IEnumerable<TLeft> left,
            Func<TLeft, TKey> leftKeySelector,
            IEnumerable<TRight> right,
            Func<TRight, TKey> rightKeySelector,
            Func<TLeft, TRight, TResult> resultSelector)
        {
            var leftJoin = from x in left
                           join y in right on leftKeySelector(x) equals rightKeySelector(y) into temp
                           from z in temp.DefaultIfEmpty()
                           select resultSelector(x, z);
            var rightJoin = from x in right
                            join y in left on rightKeySelector(x) equals leftKeySelector(y) into temp
                            from z in temp.DefaultIfEmpty()
                            select resultSelector(z, x);
            var fullJoin = leftJoin.Union(rightJoin);
            return fullJoin;
        }

        private static IEnumerable<TResult> FullJoin<T, TKey, TResult>(IEnumerable<T> left, IEnumerable<T> right,
            Func<T, TKey> keySelector, Func<T, T, TResult> resultSelector)
            => FullJoin(left, keySelector, right, keySelector, resultSelector);

        #endregion

        /// <summary>
        /// 获取保养升级购图标配置
        /// </summary>
        /// <returns></returns>
        public string GetBaoYangLevelUpIcon()
        {
            var result = string.Empty;
            try
            {
                string xml = new BaoYangConfigManager().GetConfigXml("BaoYangLevelUpIcon");
                if (!string.IsNullOrEmpty(xml))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    var node = doc.DocumentElement?.SelectSingleNode("/BaoYangLevelUpIcon");
                    if (node != null)
                    {
                        result = node.InnerText;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetBaoYangLevelUpIconConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 更新或插入保养升级购图标配置
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public bool UpsertBaoYangLevelUpIcon(string icon)
        {
            var result = false;
            try
            {
                result = dbScopeManagerBY.Execute(conn => DALBaoyang.UpsertBaoYangLevelUpIcon(conn, icon));
            }
            catch (Exception ex)
            {
                logger.Error("UpdateBaoYangLevelUpIconConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 清除保养升级购图标配置缓存
        /// </summary>
        /// <returns></returns>
        public bool RemoveBaoYangLevelUpIconCache()
        {
            var cacheKey = "BaoYangLevelUpIcon";
            return CleanBaoYangCaches(new List<string>() { cacheKey });
        }

        /// <summary>
        /// 获取保养关联项目配置
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PackageTypeRelations>> GetPackageTypeRelationsAsync()
        {
            IEnumerable<PackageTypeRelations> result = null;
            try
            {
                using (var client = new BaoYangClient())
                {
                    var serviceResult = await client.GetPackageTypeRelationsAsync();
                    serviceResult.ThrowIfException(true);
                    return serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetPackageTypeRelationsAsync", ex);
            }
            return result ?? new List<PackageTypeRelations>();
        }
    }
}
