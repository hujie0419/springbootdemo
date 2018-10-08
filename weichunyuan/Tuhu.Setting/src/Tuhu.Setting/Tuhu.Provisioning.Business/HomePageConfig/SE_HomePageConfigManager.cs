using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Mapping;
using System.Data.SqlClient;

namespace Tuhu.Provisioning.Business
{
    public class SE_HomePageConfigManager
    {
        public IEnumerable<SE_HomePageConfig> GetPersonlCenterConfig()
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetPersonlCenterConfig(connection);
            }

        }

        public IEnumerable<SE_HomePageConfig> GetHomePageList()
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetHomePageConfig(connection);
            }

        }

        public IEnumerable<SE_HomePageConfig> GetPersonalCenterConfig()
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetPersonalCenterConfig(connection);
            }

        }


        /// <summary>
        /// 添加首页信息，同时添加 状态栏 标题栏 车型库
        /// </summary>
        /// <param name="homepage"></param>
        /// <returns></returns>
        public int AddHomePage(SE_HomePageConfig homepage)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    var dal = new DALSE_HomePageConfig();

                    if (dal.IsExitsVersion(connection, homepage.StartVersion, homepage.EndVersion, null))
                        throw new Exception("有版本号不能重叠");

                    homepage.IsEnabled = false;
                    int id = dal.AddHomePage(connection, homepage, transaction);
                    bool statusID = dal.AddHomePageModule(connection, new SE_HomePageModuleConfig()
                    {
                        CreateDateTime = DateTime.Now,
                        FKHomePage = id,
                        IsEnabled = true,
                        ModuleType = 8,
                        ModuleName = "状态栏",
                        IsMoreCity = false,
                        IsMoreChannel = false,
                        PriorityLevel = 1,
                        UpdateDateTime = DateTime.Now
                    }, transaction);
                    bool tongzhiID = dal.AddHomePageModule(connection, new SE_HomePageModuleConfig()
                    {
                        CreateDateTime = DateTime.Now,
                        FKHomePage = id,
                        IsEnabled = true,
                        ModuleType = 9,
                        ModuleName = "标题栏",
                        IsMoreCity = false,
                        IsMoreChannel = false,
                        PriorityLevel = 2,
                        UpdateDateTime = DateTime.Now
                    }, transaction);
                    bool carID = dal.AddHomePageModule(connection, new SE_HomePageModuleConfig()
                    {
                        CreateDateTime = DateTime.Now,
                        FKHomePage = id,
                        IsEnabled = true,
                        ModuleType = 10,
                        ModuleName = "车型库",
                        IsMoreCity = false,
                        IsMoreChannel = false,
                        PriorityLevel = 3,
                        UpdateDateTime = DateTime.Now
                    }, transaction);
                    if (statusID && tongzhiID && carID)
                        transaction.Commit();
                    else
                        throw new Exception("没有完整插入");
                    return 1;
                }
                catch (Exception em)
                {
                    transaction.Rollback();
                    throw new Exception(em.Message);
                }
            }
        }


        /// <summary>
        /// 添加首页信息
        /// </summary>
        /// <param name="homepage"></param>
        /// <returns></returns>
        public int AddPersonalCenter(SE_HomePageConfig homepage)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    var dal = new DALSE_HomePageConfig();
                    homepage.IsEnabled = false;
                    int id = dal.AddHomePage(connection, homepage, transaction);
                    transaction.Commit();
                    return 1;
                }
                catch (Exception em)
                {
                    transaction.Rollback();
                    throw new Exception(em.Message);
                }
            }
        }


        public bool UpdateHomePage(SE_HomePageConfig model)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                DALSE_HomePageConfig dal = new DALSE_HomePageConfig();

                if (dal.IsExitsVersion(connection, model.StartVersion, model.EndVersion, model.ID) && model.ID != 1)
                    throw new Exception("有版本号不能重叠");
                return dal.UpdateHomePage(connection, model);

                if (!model.IsEnabled)
                    if (dal.IsExitsEnabled(connection, model.ID, model.TypeConfig.Value))
                        return dal.UpdateHomePage(connection, model);
                    else
                        throw new Exception("必须有一个为可用状态");
                else
                {
                    if (dal.IsExitsEnabled(connection, model.ID, model.TypeConfig.Value))
                    {
                        return dal.UpdateHomePageAll(connection, model);
                        // throw new Exception("只能有一个首页处于开启状态");
                    }
                    else
                        return dal.UpdateHomePage(connection, model);
                }
            }
        }


        public bool DeleteHomePage(int id)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().DeleteHomePage(connection, id);
            }
        }

        /// <summary>
        /// 添加模块以及模块的属性
        /// </summary>
        /// <param name="module"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool AddHomePageModule(SE_HomePageModuleConfig module)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().AddHomePageModule(connection, module);
            }
        }


        /// <summary>
        /// 更新模块信息
        /// </summary>
        /// <param name="module"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool UpdatePageModule(SE_HomePageModuleConfig module)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().UpdateHomePageModule(connection, module);
            }
        }


        /// <summary>
        ///  删除模块信息 联机删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteModule(int id, int pkid)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().DeleteHomePageModule(connection, id, pkid);
            }
        }

        /// <summary>
        /// 更新模块顺序
        /// </summary>
        /// <param name="modules"></param>
        /// <returns></returns>
        public bool UpdateHomePageModulePriorityLevel(IEnumerable<SE_HomePageModuleConfig> modules)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().UpdateHomePageModulePriorityLevel(connection, modules);
            }
        }

        /// <summary>
        /// 查询当前首页下模块总数
        /// </summary>
        /// <param name="fkHome"></param>
        /// <returns></returns>
        public int SelectHomePageModulePriorityLevel(int fkHome)
        {
            if (fkHome <= 0)
                return -1;
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().SelectHomePageModulePriorityLevel(connection, fkHome);
            }
        }


        /// <summary>
        /// 获取模块信息
        /// </summary>
        /// <param name="id">模块ID</param>
        /// <returns></returns>
        public SE_HomePageModuleConfig GetHomePageModuleEntity(int id)
        {
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetHomePageModuleEntity(c, id);
            }
        }




        /// <summary>
        /// 查询首页实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SE_HomePageConfig GetHomePageEntity(int id)
        {
            if (id > 0)
            {
                using (var c = ProcessConnection.OpenGungnirReadOnly)
                {
                    return new DALSE_HomePageConfig().GetHomePageEntity(c, id);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据首页ID查询模块列表
        /// </summary>
        /// <param name="fkID"></param>
        /// <returns></returns>
        public IEnumerable<SE_HomePageModuleConfig> GetHomePageModuleList(int fkID)
        {
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetHomePageModuleConfig(c, fkID);
            }
        }


        /// <summary>
        /// 获取附属模块列表
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public IEnumerable<SE_HomePageModuleHelperConfig> GetHomePageModuleHelperList(int moduleId)
        {
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetHomeModuleHelperList(c, moduleId);
            }
        }

        /// <summary>
        /// 添加附属模块
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool AddHomePageModuleHelper(SE_HomePageModuleHelperConfig model, int pkid)
        {
            using (var c = ProcessConnection.OpenGungnir)
            {
                var dal = new DALSE_HomePageConfig();

                if (string.IsNullOrWhiteSpace(model.City))
                    return dal.AddHomePageModuleHelper(c, model, null, pkid);

                var list = new List<SE_ModuleHelperCityConfig>();
                var citys = model.City.TrimEnd(',').Split(',');
                foreach (var s in citys)
                {
                    if (string.IsNullOrWhiteSpace(s))
                        continue;

                    var city = new SE_ModuleHelperCityConfig
                    {
                        CreateDateTime = DateTime.Now,
                        UpdateDateTime = DateTime.Now,
                        FKRegionPKID = Convert.ToInt32(s)
                    };
                    city.UpdateDateTime = DateTime.Now;
                    list.Add(city);
                }
                return dal.AddHomePageModuleHelper(c, model, list, pkid);

            }
        }

        public bool UpdateHomePageModuleHelperPattern(SE_HomePageModuleHelperConfig model)
        {
            using (var c = ProcessConnection.OpenGungnir)
            {
                DALSE_HomePageConfig dal = new DALSE_HomePageConfig();
                return dal.UpdateHomePageModuleHelperPattern(c, model);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool UpdateHomePageModuleHelper(SE_HomePageModuleHelperConfig model, int pkid)
        {
            using (var c = ProcessConnection.OpenGungnir)
            {
                var dal = new DALSE_HomePageConfig();

                if (string.IsNullOrWhiteSpace(model.City))
                    return dal.UpdateHomePageModuleHelper(c, model, null, pkid);

                var list = new List<SE_ModuleHelperCityConfig>();
                var citys = model.City.TrimEnd(',').Split(',');
                foreach (var s in citys)
                {
                    if (string.IsNullOrWhiteSpace(s))
                        continue;

                    var city = new SE_ModuleHelperCityConfig
                    {
                        CreateDateTime = DateTime.Now,
                        UpdateDateTime = DateTime.Now,
                        FKRegionPKID = Convert.ToInt32(s)
                    };
                    city.UpdateDateTime = DateTime.Now;
                    city.FKHomePageModuleHelperID = model.ID;
                    list.Add(city);
                }
                return dal.UpdateHomePageModuleHelper(c, model, list, pkid);

            }
        }

        public bool UpdateHomePageModuleHelperBgImage(SE_HomePageModuleHelperConfig model)
        {
            using (var c = ProcessConnection.OpenGungnir)
            {
                DALSE_HomePageConfig dal = new DALSE_HomePageConfig();
                return dal.UpdateHomePageModuleHelperBgImage(c, model);
            }
        }


        public bool DeleteHomePageModuleHelper(int moduleHelperID)
        {
            DALSE_HomePageConfig dal = new DALSE_HomePageConfig();
            using (var c = ProcessConnection.OpenGungnir)
            {
                return dal.DeleteHomePageModuleHelper(c, moduleHelperID);
            }
        }

        public SE_HomePageModuleHelperConfig GetHomePageModuleHelperEntity(int id)
        {
            DALSE_HomePageConfig dal = new DALSE_HomePageConfig();
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                var model = dal.GetHomeModuleHelperEntity(c, id);
                return model;
            }
        }

        public IEnumerable<SE_ModuleHelperCityConfig> GetModuleHelperCityList(int moduleHelperID)
        {
            DALSE_HomePageConfig dal = new DALSE_HomePageConfig();
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                var model = dal.GetModuleHelperCityEntityList(c, moduleHelperID);
                return model;
            }
        }

        public IEnumerable<Region> GetRegionList(string pkids)
        {
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetRegionList(c, pkids);
            }
        }

        /// <summary>
        /// 修改附属模块
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerable<SE_HomePageModuleConfig> GetHomePageModuleChildList(int parent)
        {
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetHomePageModuleChildList(c, parent);
            }
        }


        /// <summary>
        /// 根据模块类型获取模块的名称
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public string GetModuleTypeName(int moduleType)
        {
            string moduleTypeName = string.Empty;
            switch (moduleType)
            {
                case 1:
                    moduleTypeName = "主业务";
                    break;
                case 2:
                    moduleTypeName = "banner";
                    break;
                case 3:
                    moduleTypeName = "天天秒杀";
                    break;
                case 4:
                    moduleTypeName = "新人专区";
                    break;
                case 5:
                    moduleTypeName = "汽车头条";
                    break;
                case 6:
                    moduleTypeName = "保养提醒";
                    break;
                case 7:
                    moduleTypeName = "自定义模块";
                    break;
                case 8:
                    moduleTypeName = "状态栏";
                    break;
                case 9:
                    moduleTypeName = "标题栏";
                    break;
                case 10:
                    moduleTypeName = "车型库";
                    break;
                case 20:
                    moduleTypeName = "动画模块";
                    break;
                case 22:
                    moduleTypeName = "轮胎模块";
                    break;
                case 26:
                    moduleTypeName = "保养模块";
                    break;
                case 28:
                    moduleTypeName = "改装模块";
                    break;
                case 29:
                    moduleTypeName = "美容团购模块";
                    break;
                case 30:
                    moduleTypeName = "拼图模块";
                    break;
                case 32:
                    moduleTypeName = "应用市场通栏";
                    break;
                case 33:
                    moduleTypeName = "自定义人群&渠道模块";
                    break;
                case 35:
                    moduleTypeName = "车型活动模块";
                    break;
                case 36:
                    moduleTypeName = "0元砍价+0元众测";
                    break;
                case 37:
                    moduleTypeName = "拼团模块";
                    break;
                case 38:
                    moduleTypeName = "顶部Banner";
                    break;
                case 39:
                    moduleTypeName = "顶部轮播图";
                    break;
                default:
                    break;
            }
            return moduleTypeName;
        }

        /// <summary>
        /// 根据模块类型获取模块的名称
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public string GetPersonalModuleTypeName(int moduleType)
        {
            string moduleTypeName = string.Empty;
            switch (moduleType)
            {
                case 1:
                    moduleTypeName = "一行四列";
                    break;
                case 2:
                    moduleTypeName = "banner(高)";
                    break;
                case 3:
                    moduleTypeName = "banner(低)";
                    break;
                case 4:
                    moduleTypeName = "优惠券";
                    break;
                case 5:
                    moduleTypeName = "会员商城";
                    break;
                case 6:
                    moduleTypeName = "一行两列";
                    break;
                default:
                    break;
            }
            return moduleTypeName;
        }


        #region 参数对应
        public IEnumerable<SE_WapParameterConfig> GetWapParameterList() => new DALSE_HomePageConfig().GetWapParameterList(ProcessConnection.OpenGungnirReadOnly);

        public bool AddWapParameter(SE_WapParameterConfig model) => new DALSE_HomePageConfig().AddWapParameter(ProcessConnection.OpenGungnir, model);

        public bool DeleteWapParameter(int id) => new DALSE_HomePageConfig().DeleteWapParameter(ProcessConnection.OpenGungnir, id);
        #endregion

        public IEnumerable<Tuhu.Provisioning.DataAccess.Mapping.HomePageCarActivityRegionEntity> GetHomePageCarActivityCity(int fkpkid) => new DALSE_HomePageConfig().GetHomePageCarActivityCity(ProcessConnection.OpenConfigurationReadOnly, fkpkid);

        #region 模块内容
        public SE_HomePageModuleContentConfig GetHomePageContentEntity(int id)
        {
            if (id == 0)
                return null;
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetHomePageModuleContentEntity(c, id);
            }
        }

        public IEnumerable<SE_HomePageModuleContentConfig> GetHomePageContentList(int? moduleID, int? moduleHelperID)
        {
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetHomePageContentList(c, moduleID, moduleHelperID);
            }
        }

        /// <summary>
        /// 添加模块内容数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public int AddHomePageContent(SE_HomePageModuleContentConfig model, int pkid)
        {
            using (var c = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().AddHomePageContent(c, model, pkid);
            }
        }


        /// <summary>
        /// 修改模块内容数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool UpdateHomePageContent(SE_HomePageModuleContentConfig model, int pkid)
        {
            using (var c = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().UpdateHomePageContent(c, model, pkid);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteHomePageContent(int id, int pkid)
        {
            using (var c = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().DeleteHomePageContent(c, id, pkid);
            }
        }
        public List<SE_HomePageModuleHelperConfig> GetSE_HomePageModuleHelperConfigsByFkHomePageId(int fkid)
        {
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetSE_HomePageModuleHelperConfigsByFkHomePageId(c, fkid);
            }
        }
        public List<DeviceBrandModel> SelectDeviceBrands()
        {
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().SelectDeviceBrands(c);
            }
        }
        public List<DeviceTypeModel> SelectMobileModels(int brandId)
        {
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().SelectDeviceTypes(c, brandId);
            }
        }
        #endregion




        #region 闪屏
        public bool AddFlashScreenEntity(SE_FlashScreenConfig model)
        {
            using (var c = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().AddFlashScreenEntity(ProcessConnection.OpenGungnir, model);
            }
        }

        public IEnumerable<SE_FlashScreenConfig> GetFlashScreenList(int type)
        {
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetFlashScreenList(c, type);
            }
        }


        public bool UpdateFlashScreenEntity(SE_FlashScreenConfig model)
        {
            using (var c = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().UpdateFlashScreenEntity(c, model);
            }
        }

        public bool DeleteFlashScreen(int id)
        {
            using (var c = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().DeleteFlashScreen(c, id);
            }
        }

        public SE_FlashScreenConfig GetFlashScreenEntity(int id)
        {
            using (var c = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetFlashScreenEntity(c, id);
            }
        }
        #endregion

        #region 瀑布流

        public bool AddFlow(SE_HomePageFlowConfig model)
        {
            DALSE_HomePageConfig dal = new DALSE_HomePageConfig();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.AddFlow(connection, model);
            }
        }

        public bool UpdateFlow(SE_HomePageFlowConfig model)
        {
            DALSE_HomePageConfig dal = new DALSE_HomePageConfig();
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return dal.UpdateFlow(connection, model);
            }
        }

        public SE_HomePageFlowConfig GetFlowEntity(int id)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                DALSE_HomePageConfig dal = new DALSE_HomePageConfig();
                return dal.GetFlowEntity(connection, id);
            }
        }


        public IEnumerable<SE_HomePageFlowConfig> GetFlowList(string type)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                DALSE_HomePageConfig dal = new DALSE_HomePageConfig();
                return dal.GetFlowList(connection, type);
            }
        }


        public bool DeleteFlow(int id)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                DALSE_HomePageConfig dal = new DALSE_HomePageConfig();
                return dal.DeleteFlow(connection, id);
            }
        }


        public IEnumerable<SE_HomePageFlowProductConfig> GetFlowProductList(int flowID)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetFlowProductList(connection, flowID);
            }
        }

        public bool AddFlowProduct(SE_HomePageFlowProductConfig model)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().AddFlowProduct(connection, model);
            }
        }

        public bool DeleteFlowProduct(int id)
        {
            using (var connection = ProcessConnection.OpenGungnir)
            {
                return new DALSE_HomePageConfig().DeleteFlowProduct(connection, id);
            }
        }

        #endregion

        /// <summary>
        /// 获取城市的父级ID
        /// </summary>
        /// <param name="pkids"></param>
        /// <returns></returns>
        public IEnumerable<string> GetCityGroupProvince(string pkids)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetCityGroupProvince(connection, pkids);
            }
        }

        /// <summary>
        /// 获取首页显示的
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SE_HomePageModuleConfig> GetPreViewModuleList()
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetPreViewModuleList(connection);
            }
        }



        #region 首页菜单
        public bool AddHomePageMenu(SE_HomePageMenuConfig model)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return new DALSE_HomePageConfig().AddHomePageMenu(connection, model);
            }
        }

        public bool UpdateHomePageMenu(SE_HomePageMenuConfig model)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return new DALSE_HomePageConfig().UpdateHomePageMenu(connection, model);
            }
        }


        public bool DeleteHomePageMenu(int id)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return new DALSE_HomePageConfig().DeleteHomePageMenu(connection, id);
            }
        }


        public SE_HomePageMenuConfig GetHomePageMenuEntity(int id)
        {
            using (var connection = ProcessConnection.OpenConfigurationReadOnly)
            {
                return new DALSE_HomePageConfig().GetHomePageMenuEntity(connection, id);
            }
        }

        public IEnumerable<SE_HomePageMenuConfig> GetHomePageMenuList(int id)
        {
            using (var connection = ProcessConnection.OpenConfigurationReadOnly)
            {
                return new DALSE_HomePageConfig().GetHomePageMenuList(connection, id);
            }
        }


        public IEnumerable<SE_HomePageMenuListConfig> GetHomePageMenuListList()
        {
            using (var connection = ProcessConnection.OpenConfigurationReadOnly)
            {
                return new DALSE_HomePageConfig().GetHomePageMenuListList(connection);
            }
        }

        public SE_HomePageMenuListConfig GetHomePageMenuListEntity(int id)
        {
            using (var connection = ProcessConnection.OpenConfigurationReadOnly)
            {
                return new DALSE_HomePageConfig().GetHomePageMenuListEntity(connection, id);
            }
        }

        public bool AddHomePageMenuList(SE_HomePageMenuListConfig model)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return new DALSE_HomePageConfig().AddHomePageMenuList(connection, model);
            }
        }

        public bool ExistsHomePageMenuList(DateTime startDate, DateTime endDate)
        {
            using (var connection = ProcessConnection.OpenConfigurationReadOnly)
            {
                return new DALSE_HomePageConfig().ExistsHomePageMenuList(connection, startDate, endDate);
            }
        }

        public bool UpdateHomePageMenuList(SE_HomePageMenuListConfig model)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return new DALSE_HomePageConfig().UpdateHomePageMenuList(connection, model);
            }
        }

        public bool DeleteHomePageMenuList(int id)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return new DALSE_HomePageConfig().DeleteHomePageMenuList(connection, id);
            }
        }

        #endregion

        public bool UpdateNextDateTime(DateTime date)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return new DALSE_HomePageConfig().UpdateNextDateTime(connection, date);
            }
        }

        public string GetNextDateTime()
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                return new DALSE_HomePageConfig().GetNextDateTime(connection);
            }
        }


        public IEnumerable<SE_HomePageAnimationContent> GetHomePageAnimationContentList(int? moduleID, int? moduleHelperID)
        {
            using (var conn = ProcessConnection.OpenConfigurationReadOnly)
            {
                return new DALSE_HomePageConfig().GetHomePageAnimationContentList(conn, moduleID, moduleHelperID);
            }
        }

        public bool AddHomePageAnimationContent(SE_HomePageAnimationContent model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return new DALSE_HomePageConfig().AddHomePageAnimationContent(conn, model);
            }
        }

        public bool UpdateHomePageAnimationContent(SE_HomePageAnimationContent model)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return new DALSE_HomePageConfig().UpdateHomePageAnimationContent(conn, model);
            }
        }

        public SE_HomePageAnimationContent GetHomePageAnimationContentEntity(int id)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return new DALSE_HomePageConfig().GetHomePageAnimationContentEntity(conn, id);
            }
        }

        public bool DeleteHomePageAnimationContent(int id)
        {
            using (var conn = ProcessConnection.OpenConfiguration)
            {
                return new DALSE_HomePageConfig().DeleteHomePageAnimationContent(conn, id);
            }
        }


        public bool ContentDateTimeValidate(SE_HomePageModuleContentConfig model)
        {
            if (model.StartDateTime?.CompareTo(model.EndDateTime.Value) >= 0)
                return false;

            var contentlist = GetHomePageContentList(model.FKHomePageModuleID, model.FKHomePageModuleHelperID);
            foreach (var item in contentlist?.Where(o => o.PriorityLevel == model.PriorityLevel))
            {
                if (item.ID == model.ID)
                    continue;

                if (model?.StartDateTime?.CompareTo(item.EndDateTime) <= 0 && model?.StartDateTime?.CompareTo(item.StartDateTime) > 0)
                    return false;

                if (model?.StartDateTime?.CompareTo(item.StartDateTime) < 0 && model?.EndDateTime?.CompareTo(item.StartDateTime) >= 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 四个配置位置时，获取组合的方式
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetPuzzle4(int type, IEnumerable<SE_HomePageModuleContentConfig> list)
        {
            Dictionary<string, object> puzzles = new Dictionary<string, object>();
            DateTime? startDateTime = null;
            DateTime? endDateTime = null;
            foreach (var frist in list.Where(o => o.PriorityLevel == 1).OrderBy(o => o.StartDateTime))
            {
                startDateTime = frist.StartDateTime;
                endDateTime = frist.EndDateTime;
                foreach (var second in list.Where(o => o.PriorityLevel == 2 && IsContain(frist.StartDateTime, frist.EndDateTime, o.StartDateTime, o.EndDateTime)).OrderBy(o => o.StartDateTime))
                {
                    startDateTime = GetLastDateTime(second.StartDateTime, startDateTime);
                    endDateTime = GetFirstDateTime(second.EndDateTime, endDateTime);

                    foreach (var third in list
                        .Where(o => o.PriorityLevel == 3 && IsContain(frist.StartDateTime, frist.EndDateTime, o.StartDateTime, o.EndDateTime) && IsContain(second.StartDateTime, second.EndDateTime, o.StartDateTime, o.EndDateTime))
                        .OrderBy(o => o.StartDateTime))
                    {
                        startDateTime = GetLastDateTime(third.StartDateTime, startDateTime);
                        endDateTime = GetFirstDateTime(third.EndDateTime, endDateTime);
                        foreach (var fourth in list.Where(o => o.PriorityLevel == 4 && IsContain(frist.StartDateTime, frist.EndDateTime, o.StartDateTime, o.EndDateTime) && IsContain(second.StartDateTime, second.EndDateTime, o.StartDateTime, o.EndDateTime) && IsContain(third.StartDateTime, third.EndDateTime, o.StartDateTime, o.EndDateTime))
                            .OrderBy(o => o.StartDateTime))
                        {
                            startDateTime = GetLastDateTime(fourth.StartDateTime, startDateTime);
                            endDateTime = GetFirstDateTime(fourth.EndDateTime, endDateTime);

                            puzzles.Add(frist.ID.ToString() + second.ID.ToString() + third.ID.ToString() + fourth.ID.ToString(), new
                            {
                                First = new
                                {
                                    ID = frist.ID,
                                    ImgUrl = frist.ButtonImageUrl,
                                    PriorityLevel = frist.PriorityLevel
                                },
                                Second = new
                                {
                                    ID = second.ID,
                                    ImgUrl = second.ButtonImageUrl,
                                    PriorityLevel = second.PriorityLevel
                                },
                                Third = new
                                {
                                    ID = third.ID,
                                    ImgUrl = third.ButtonImageUrl,
                                    PriorityLevel = third.PriorityLevel
                                },
                                Fourth = new
                                {
                                    ID = fourth.ID,
                                    ImgUrl = fourth.ButtonImageUrl,
                                    PriorityLevel = fourth.PriorityLevel
                                },
                                Type = type,
                                StartDateTime = startDateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                                EndDateTime = endDateTime?.ToString("yyyy-MM-dd HH:mm:ss")
                            });
                        }
                    }
                }
            }
            return puzzles;
        }

        /// <summary>
        /// 三个配置位置时，组合的方式
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetPuzzle3(int type, IEnumerable<SE_HomePageModuleContentConfig> list)
        {
            Dictionary<string, object> puzzles = new Dictionary<string, object>();

            DateTime? startDateTime = null;
            DateTime? endDateTime = null;
            foreach (var frist in list.Where(o => o.PriorityLevel == 1).OrderBy(o => o.StartDateTime))
            {
                startDateTime = frist.StartDateTime;
                endDateTime = frist.EndDateTime;

                foreach (var second in list.Where(p => p.PriorityLevel == 2)
                    .Where(o => IsContain(frist.StartDateTime, frist.EndDateTime, o.StartDateTime, o.EndDateTime)).OrderBy(o => o.StartDateTime))
                {
                    startDateTime = GetLastDateTime(second.StartDateTime, startDateTime);
                    endDateTime = GetFirstDateTime(second.EndDateTime, endDateTime);

                    foreach (var third in list.Where(p => p.PriorityLevel == 3)
                        .Where(o => IsContain(second.StartDateTime, second.EndDateTime, o.StartDateTime, o.EndDateTime))
                        .Where(o => IsContain(frist.StartDateTime, frist.EndDateTime, o.StartDateTime, o.EndDateTime))
                        .OrderBy(o => o.StartDateTime))
                    {
                        startDateTime = GetLastDateTime(third.StartDateTime, startDateTime);
                        endDateTime = GetFirstDateTime(third.EndDateTime, endDateTime);

                        puzzles.Add(frist.ID.ToString() + second.ID.ToString() + third.ID.ToString(), new
                        {
                            First = new
                            {
                                ID = frist.ID,
                                ImgUrl = frist.ButtonImageUrl,
                                PriorityLevel = frist.PriorityLevel
                            },
                            Second = new
                            {
                                ID = second.ID,
                                ImgUrl = second.ButtonImageUrl,
                                PriorityLevel = second.PriorityLevel
                            },
                            Third = new
                            {
                                ID = third.ID,
                                ImgUrl = third.ButtonImageUrl,
                                PriorityLevel = third.PriorityLevel
                            },
                            Type = type,
                            StartDateTime = startDateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                            EndDateTime = endDateTime?.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                    }
                }
            }
            return puzzles;
        }

        /// <summary>
        /// 留个配置位置时
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetPuzzle6(int type, IEnumerable<SE_HomePageModuleContentConfig> list)
        {
            Dictionary<string, object> puzzles = new Dictionary<string, object>();

            foreach (var frist in list.Where(o => o.PriorityLevel == 1).OrderBy(o => o.StartDateTime))
            {
                foreach (var second in list.Where(p => p.PriorityLevel == 2).Where(o => o.StartDateTime?.CompareTo(frist.StartDateTime.Value) <= 0 && o.EndDateTime?.CompareTo(frist.EndDateTime) >= 0).OrderBy(o => o.StartDateTime))
                {
                    foreach (var third in list.Where(p => p.PriorityLevel == 3).Where(o => o.StartDateTime?.CompareTo(second.StartDateTime.Value) <= 0 && o.EndDateTime?.CompareTo(second.EndDateTime) >= 0).OrderBy(o => o.StartDateTime))
                    {
                        foreach (var fourth in list.Where(p => p.PriorityLevel == 4).Where(o => o.StartDateTime?.CompareTo(third.StartDateTime.Value) <= 0 && o.EndDateTime?.CompareTo(third.EndDateTime) >= 0).OrderBy(o => o.StartDateTime))
                        {
                            foreach (var fifth in list.Where(p => p.PriorityLevel == 5).Where(o => o.StartDateTime?.CompareTo(fourth.StartDateTime.Value) <= 0 && o.EndDateTime?.CompareTo(fourth.EndDateTime) >= 0).OrderBy(o => o.StartDateTime))
                                foreach (var sixth in list.Where(p => p.PriorityLevel == 6).Where(o => o.StartDateTime?.CompareTo(fifth.StartDateTime.Value) <= 0 && o.EndDateTime?.CompareTo(fifth.EndDateTime) >= 0).OrderBy(o => o.StartDateTime))
                                    puzzles.Add(frist.ID.ToString() + second.ID.ToString() + third.ID.ToString() + fourth.ID.ToString() + fifth.ID.ToString() + sixth.ID.ToString(), new
                                    {
                                        First = new
                                        {
                                            ID = frist.ID,
                                            ImgUrl = frist.ButtonImageUrl,
                                            PriorityLevel = frist.PriorityLevel
                                        },
                                        Second = new
                                        {
                                            ID = second.ID,
                                            ImgUrl = second.ButtonImageUrl,
                                            PriorityLevel = second.PriorityLevel
                                        },
                                        Third = new
                                        {
                                            ID = third.ID,
                                            ImgUrl = third.ButtonImageUrl,
                                            PriorityLevel = third.PriorityLevel
                                        },
                                        Fourth = new
                                        {
                                            ID = fourth.ID,
                                            ImgUrl = fourth.ButtonImageUrl,
                                            PriorityLevel = fourth.PriorityLevel
                                        },
                                        Fifth = new
                                        {
                                            ID = fifth.ID,
                                            ImgUrl = fifth.ButtonImageUrl,
                                            PriorityLevel = fifth.PriorityLevel
                                        },
                                        Sixth = new
                                        {
                                            ID = sixth.ID,
                                            ImgUrl = sixth.ButtonImageUrl,
                                            PriorityLevel = sixth.PriorityLevel
                                        },
                                        Type = type
                                    });
                        }
                    }
                }
            }
            return puzzles;
        }


        public bool AddPuzzleConfig(IEnumerable<SE_HomePageContentPuzzle> entities, int type)
        {
            try
            {
                RepositoryManager manager = new RepositoryManager(ConnectionStrings.Configuration);


                using (var db = manager.BeginTrans())
                {


                    var moduleID = entities.Where(o => o.FKModuleID != null).GroupBy(o => o.FKModuleID);
                    foreach (var k in moduleID)
                    {
                        System.Linq.Expressions.Expression<Func<SE_HomePageContentPuzzle, bool>> expression = (o) => o.FKModuleID == k.Key;
                        db.Delete<SE_HomePageContentPuzzle>(expression);
                    }

                    var moduleHelperID = entities.Where(o => o.FKModuleHelperID != null).GroupBy(o => o.FKModuleHelperID);
                    foreach (var k in moduleHelperID)
                    {
                        System.Linq.Expressions.Expression<Func<SE_HomePageContentPuzzle, bool>> expression = (o) => o.FKModuleHelperID == k.Key;
                        db.Delete<SE_HomePageContentPuzzle>(expression);
                    }

                    var groupList = entities.GroupBy(o => o.GroupID);
                    foreach (var g in groupList)
                    {
                        List<SE_HomePageContentPuzzle> values = new List<SE_HomePageContentPuzzle>();
                        foreach (var item in g)
                            values.Add(item);

                        PuzzleManager puzzle = new PuzzleManager(values.OrderBy(o => o.PriorityLevel), type);
                        string puzzleUri = puzzle.CreatePuzzleUri();

                        for (int i = 0; i < values.Count(); i++)
                        {
                            var entity = values[i];
                            var postion = puzzle.GetPostion(entity.ImageUrl, entity.PriorityLevel);
                            entity.Width = puzzle.Width;
                            entity.Height = puzzle.Height;
                            entity.ImagePuzzleUrl = puzzleUri;
                            entity.UpperLeftX = postion.Item1;
                            entity.UpperLeftyY = postion.Item2;
                            entity.LowerRightX = postion.Item3;
                            entity.LowerRightY = postion.Item4;
                            db.Insert<SE_HomePageContentPuzzle>(entity);
                        }
                    }

                    db.Commit();
                }
                return true;
            }
            catch (Exception em)
            {

                throw em;
            }
        }


        public IEnumerable<SE_HomePageContentPuzzle> GetHomePageContentPuzzleList(int? moduleID, int? moduleHelperID)
        {
            RepositoryManager manager = new RepositoryManager();
            SqlParameter[] paramers = null;
            string sql = "";
            if (moduleID != null)
            {
                sql = @"SELECT  * FROM Configuration.dbo.SE_HomePageContentPuzzle WITH (NOLOCK) WHERE  FKModuleID=@FKModuleID ";
                paramers = new SqlParameter[] {
                new SqlParameter("@FKModuleID",moduleID)
                };
            }

            if (moduleHelperID != null)
            {
                sql = @"SELECT  * FROM Configuration.dbo.SE_HomePageContentPuzzle WITH (NOLOCK) WHERE  FKModuleHelperID=@FKModuleHelperID ";

                paramers = new SqlParameter[] {
                new SqlParameter("@FKModuleHelperID",moduleHelperID)
                };
            }

            return manager.GetEntityList<SE_HomePageContentPuzzle>(sql, paramers);
        }


        private bool IsContain(DateTime? startDT, DateTime? endDT, DateTime? selfStartDT, DateTime? selfEndDT)
        {
            bool result = true;

            if (selfStartDT?.CompareTo(selfEndDT) >= 0)
            {
                return result = false;
            }

            if (startDT?.CompareTo(endDT) >= 0)
            {
                return result = false;
            }

            if (selfStartDT?.CompareTo(endDT) < 0 && selfEndDT?.CompareTo(endDT) < 0 && selfEndDT?.CompareTo(startDT) < 0)
            {
                result = false;
            }
            if (selfStartDT?.CompareTo(endDT) > 0)
                result = false;


            return result;
        }


        private DateTime? GetLastDateTime(DateTime? compareDT, DateTime? selfDT)
        {
            return compareDT?.CompareTo(selfDT) > 0 ? compareDT : selfDT;
        }

        private DateTime? GetFirstDateTime(DateTime? compareDT, DateTime? selfDT)
        {
            return compareDT?.CompareTo(selfDT) < 0 ? compareDT : selfDT;
        }

        /// <summary>
        /// 复制新增
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool CopyHomePage(int pkid)
        {
            RepositoryManager managerReadOnely = new RepositoryManager();
            RepositoryManager manager = new RepositoryManager(ConnectionStrings.Configuration);
            var dal = new DALSE_HomePageConfig();
            var entity = GetHomePageEntity(pkid);
            entity.IsEnabled = false;
            using (var connection = ProcessConnection.OpenGungnir)
            {

                var transaction = connection.BeginTransaction();
                dal.AddHomePage(connection, entity, transaction);

                var moduleList = GetHomePageModuleList(pkid);
                foreach (var module in moduleList)
                {
                    module.FKHomePage = entity.ID;
                    var modulePKID = dal.AddHomePageModule(connection, transaction, module);//添加模块
                    var moduleHelperList = GetHomePageModuleHelperList(module.ID);
                    if (moduleHelperList != null)
                        foreach (var moduleHelper in moduleHelperList)
                        {
                            moduleHelper.FKHomePageModuleID = modulePKID;
                            //子模块ID
                            var moduleHelperPKID = dal.AddHomePageModuleHelper(connection, transaction, moduleHelper);//添加子模块
                            var cityList = GetModuleHelperCityList(moduleHelper.ID);
                            if (cityList != null)
                                dal.AddHomePageModuleHelperByCity(connection, transaction, cityList, moduleHelperPKID);///添加分城市
                            var moduleHelperContentList = GetHomePageContentList(null, moduleHelper.ID);
                            if (moduleHelperContentList != null)//添加内容
                                foreach (var content in moduleHelperContentList)
                                {
                                    content.FKHomePageModuleHelperID = moduleHelperPKID;
                                    var contentHelperPKID = dal.AddHomePageContent(connection, transaction, content, pkid);

                                    if (moduleHelper.ModuleType == 30)
                                    {
                                        System.Linq.Expressions.Expression<Func<SE_HomePageContentPuzzle, bool>> exp = _ => _.FKID == content.ID;
                                        var puzzleEntity = managerReadOnely.GetEntity(exp);
                                        if (puzzleEntity != null)
                                        {
                                            puzzleEntity.FKID = contentHelperPKID;
                                            puzzleEntity.FKModuleHelperID = moduleHelperPKID;
                                            manager.Add(puzzleEntity);
                                        }
                                    }
                                }
                        }

                    var moduleContentList = GetHomePageContentList(module.ID, null);
                    if (moduleContentList != null)//添加内容
                        foreach (var content in moduleContentList)
                        {
                            content.FKHomePageModuleID = modulePKID;
                            int contentPKID = dal.AddHomePageContent(connection, transaction, content, pkid);
                            if (module.ModuleType == 30)
                            {
                                System.Linq.Expressions.Expression<Func<SE_HomePageContentPuzzle, bool>> exp = _ => _.FKID == content.ID;
                                var puzzleEntity = managerReadOnely.GetEntity(exp);
                                if (puzzleEntity != null)
                                {
                                    puzzleEntity.FKID = contentPKID;
                                    puzzleEntity.FKModuleID = modulePKID;
                                    manager.Add(puzzleEntity);
                                }
                            }

                        }

                }

                transaction.Commit();
                return true;
            }
        }

        public Dictionary<string, object> GetPuzzleList(int? moduleID, int? moduleHelperID)
        {

            //1 四平分 2 挑两头 3 六大块 4 两大两小 5 三大块
            int puzzleType = 0;
            if (moduleID != null)
                puzzleType = GetHomePageModuleEntity(moduleID.Value).Pattern.Value;
            if (moduleHelperID != null)
                puzzleType = GetHomePageModuleHelperEntity(moduleHelperID.Value).Pattern.Value;

            var list = GetHomePageContentList(moduleID, moduleHelperID).Where(o => o.IsEnabled == true);
            if (list == null)
                return null;

            int count = list.GroupBy(o => o.PriorityLevel).Count();

            #region 判断配置内容是否完整
            string message = "";
            bool isFull = true;
            if (puzzleType == 1)
                if (count != 4)
                {
                    isFull = false;
                    message = count > 4 ? "配置内容超过4个，请删除或者禁用多余的" : "配置内容少于4个,请增加内容";
                }

            if (puzzleType == 2)
                if (count != 3)
                {
                    isFull = false;
                    message = count > 3 ? "配置内容超过3个，请删除或者禁用多余的" : "配置内容少于3个,请增加内容";
                }

            if (puzzleType == 3)
                if (count != 3)
                {
                    isFull = false;
                    message = count > 3 ? "配置内容超过3个，请删除或者禁用多余的" : "配置内容少于3个,请增加内容";
                }

            if (puzzleType == 4)
                if (count != 4)
                {
                    isFull = false;
                    message = count > 4 ? "配置内容超过4个，请删除或者禁用多余的" : "配置内容少于4个,请增加内容";
                }

            if (puzzleType == 5)
                if (count != 3)
                {
                    isFull = false;
                    message = count > 3 ? "配置内容超过3个，请删除或者禁用多余的" : "配置内容少于3个,请增加内容";
                }
            #endregion

            if (!isFull)
                return null;

            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (puzzleType == 1 || puzzleType == 4)
                dic = GetPuzzle4(puzzleType, list);
            if (puzzleType == 2 || puzzleType == 5 || puzzleType == 3)
                dic = GetPuzzle3(puzzleType, list);
            //if (puzzleType == 3)
            //    dic = manager.GetPuzzle6(puzzleType, list);
            if (dic.Count <= 0)
                return null;

            return dic;
        }

    }
}

