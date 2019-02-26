using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.ActivityCalendar;
using Tuhu.Provisioning.Business.AutoSuppliesManagement;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Product;

namespace Tuhu.Provisioning.Business
{
    public class AutoSuppliesManager : IAutoSuppliesManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("AutoSupplies");

        private AutoSuppliesHandler handler = null;

        #endregion

        #region Advertise
        public AutoSuppliesManager()
        {
            handler = new AutoSuppliesHandler(DbScopeManager);
        }

        public List<Advertise> GetSuppliesModule()
        {
            return handler.GetSuppliesModule();
        }

        public void DeleteAdvertise(int id)
        {
            handler.DeleteAdvertise(id);
        }
        public string GetStatusName(DateTime? BeginDateTime, DateTime? EndDateTime)
        {
            return handler.GetStatusName(BeginDateTime, EndDateTime);
        }
        public void AddAdvertise(Advertise advertise)
        {
            handler.AddAdvertise(advertise);
        }
        public void UpdateAdvertise(Advertise advertise)
        {
            handler.UpdateAdvertise(advertise);
        }
        public Advertise GetAdvertiseByID(int id)
        {
            return handler.GetAdvertiseByID(id);
        }
        public bool IsExistsxAdColumnID(string AdColumnID)
        {
            return handler.IsExistsxAdColumnID(AdColumnID);
        }
        #endregion
        #region Adproduct
        public List<AdProduct> GetAdProListByAdID(int AdvertiseID)
        {
            return handler.GetAdProListByAdID(AdvertiseID);
        }
        public string GetCountByAdID(int AdvertiseID)
        {
            return handler.GetCountByAdID(AdvertiseID);
        }
        public void DeleteAdProduct(int AdvertiseID, string PID)
        {
            handler.DeleteAdProduct(AdvertiseID, PID);
        }
        public void ChangeState(int AdvertiseID, string PID, byte State)
        {
            handler.ChangeState(AdvertiseID, PID, State);
        }
        public string GetProductNameByPID(string PID)
        {
            string name = handler.GetProductNameByPID(PID);
            if (string.IsNullOrWhiteSpace(name))
            {
                using (var client = new ProductClient())
                {
                    var result = client.GetUnifiedProductsByPids(new List<string>() { PID });
                    if (result.Success && result.Result.Any())
                    {
                        name = result.Result.FirstOrDefault()?.DisplayName;
                    }
                }
            }
            return name;
        }

        public Dictionary<string, string> GetProductNamesByPids(IEnumerable<string> pids)
        {
            Dictionary<string, string> datas = handler.GetProductNamesByPids(pids);
            List<string> outSourcePids = pids.ToList().Where(p => !datas.Keys.Contains(p)).ToList() ;
            using (var client = new ProductClient())
            {
                var result = client.GetUnifiedProductsByPids( outSourcePids );
                if (result.Success && result.Result.Any())
                {
                    foreach (var item in result.Result)
                    {
                        datas.Add(item.PID,item.DisplayName);
                    }
                }
            }
            return datas;
        }

        public string GetCateNameByCateID(string CateID)
        {
            return handler.GetCateNameByCateID(CateID); 
        }

        /// <summary>
        /// 根据PID获取产品相关信息
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public DataTable GetProductInfoByPID(string PID)
        {
            return handler.GetProductInfoByPID(PID);
        }

        /// <summary>
        /// 新版本 通过pid获取产品相关信息
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public DataTable GetProductInfoByPIdNewVersion(string PID)
        {
            return handler.GetProductInfoByPIdNewVersion(PID);
        }

        public void UpdateAdProduct(int AdvertiseID, string PID, string NewPID, byte Position, decimal PromotionPrice, int PromotionNum)
        {
            handler.UpdateAdProduct(AdvertiseID, PID, NewPID, Position, PromotionPrice, PromotionNum);
        }
        public void AddAdProduct(int AdvertiseID, string PID, byte Position, byte State, decimal PromotionPrice, int PromotionNum)
        {
            handler.AddAdProduct(AdvertiseID, PID, Position, State, PromotionPrice, PromotionNum);
        }
        #endregion
        #region ActionsetTab
        public List<BizActionsetTab> GetAllActionsetTab()
        {
            return handler.GetAllActionsetTab();
        }
        public void DeleteActionsetTab(int id)
        {
            handler.DeleteActionsetTab(id);
        }
        public void AddActionsetTab(BizActionsetTab actionsetTab)
        {
            handler.AddActionsetTab(actionsetTab);
        }
        public void UpdateActionsetTab(BizActionsetTab actionsetTab)
        {
            handler.UpdateActionsetTab(actionsetTab);
        }
        public BizActionsetTab GetActionsetTabByID(int id)
        {
            return handler.GetActionsetTabByID(id);
        }
        #endregion

        public List<NewAppSet> SelectNewAppSet()
        {
            return handler.SelectNewAppSet();
        }
        public void SelectDataForActivityFromNewAppSet()
        {
            string pkidStr = string.Empty;
            //获取活动表中已有的套餐信息
            List<Tuhu.Provisioning.DataAccess.Entity.ActivityCalendar> listAc = new ActivityCalendarManager().SelectActivityByCondition(string.Empty).Where(_ => _.DataFrom.EndsWith(ActivityObject.NewAppSet.ToString())).ToList();
            //拼接已录入的活动信息,将来在套餐信息表中排除
            if (listAc.Any())
            {
                //pkidStr = listAc.Where(_ => _.DataFromId != null).Aggregate(pkidStr, (current, item) => current + (item.DataFromId.ToString() + ','));
                //pkidStr = pkidStr.Substring(0, pkidStr.Length - 1);
                listAc = listAc.Where(_ => _.CreateDate.Date == DateTime.Now.Date && _.DataFromId != null).ToList();
            }

            #region 没有的数据要添加

            //获取套餐信息表中新增的套餐信息
            //获取套餐信息表中新增的套餐信息
            List<NewAppSet> listZaConfigue;
            if (!listAc.Any())
            {
                listZaConfigue = SelectNewAppSet();
            }
            else
            {
                listZaConfigue = SelectNewAppSet().FindAll(delegate (NewAppSet info)
                {

                    if (!listAc.Select(_ => _.DataFromId).Contains(Convert.ToInt32(info.Id)) || listAc.Any(_ => _.DataFromId == Convert.ToInt32(info.Id) && _.CreateDate.Date != DateTime.Now.Date))
                    {
                        return true;
                    }
                    return false;
                });
            }
            //向活动日历信息表添加数据
            foreach (var item in listZaConfigue)
            {
                var modelAc = new DataAccess.Entity.ActivityCalendar
                {
                    BeginDate = item.Starttime.GetValueOrDefault(),
                    EndDate = item.Overtime.GetValueOrDefault(),
                    ActivityTitle = item.Modelname,
                    ActivityContent = item.Modelname,
                    ActivityUrl = item.Jumph5url,
                    CreateDate = DateTime.Now,
                    CreateBy = "SYSTEM",
                    DataFrom = ActivityObject.NewAppSet.ToString(),
                    DataFromId = Convert.ToInt32(item.Id),
                    IsActive = true,
                    Owner = item.Apptype == 1 ? "安卓" : "IOS",
                    OwnerType = item.Apptype.ToString()
                };
                new ActivityCalendarManager().AddActivityCalendar(modelAc);
            }
            #endregion
            #region
            //获取套餐信息表中新增的套餐信息
            var updateList = SelectNewAppSet().FindAll(delegate (NewAppSet info)
            {
                if (listAc.Any(_ => _.DataFromId == Convert.ToInt32(info.Id) && _.CreateDate.Date == DateTime.Now.Date))
                {
                    return true;
                }
                return false;
            });
            foreach (var item in updateList)
            {
                new ActivityCalendarManager().UpdateIsActivity(Convert.ToInt32(item.Id), ActivityObject.NewAppSet.ToString(), item.Showstatic == 1 ? true : false);
            }
            #endregion

            #region 查询旧得网页资源，如果新的里面没有则添加
            List<DataAccess.Entity.ActivityCalendar> listAc2 = new ActivityCalendarManager().SelectActivityByCondition(string.Empty).Where(_ => _.DataFrom.EndsWith(ActivityObject.Advertise.ToString())).ToList();
            //拼接已录入的活动信息,将来在套餐信息表中排除
            if (listAc2.Any())
            {
                //pkidStr = listAc.Where(_ => _.DataFromId != null).Aggregate(pkidStr, (current, item) => current + (item.DataFromId.ToString() + ','));
                //pkidStr = pkidStr.Substring(0, pkidStr.Length - 1);
                listAc2 = listAc2.Where(_ => _.CreateDate.Date == DateTime.Now.Date && _.DataFromId != null).ToList();
            }
            List<Advertise> suppliesList;
            if (!listAc2.Any())
            {
                suppliesList = GetSuppliesModule();
            }
            else
            {
                suppliesList = GetSuppliesModule().FindAll(delegate (Advertise info)
                {

                    if (!listAc2.Select(_ => _.DataFromId).Contains(Convert.ToInt32(info.PKID)) || listAc2.Any(_ => _.DataFromId == Convert.ToInt32(info.PKID) && _.CreateDate.Date != DateTime.Now.Date))
                    {
                        return true;
                    }
                    return false;
                });
            }
            suppliesList = suppliesList.Where(_ => _.AdColumnID.EndsWith("www-05")).ToList().FindAll(delegate (Advertise info)
            {
                if (!listZaConfigue.Select(_ => _.Modelname).Contains(info.Name))
                {
                    return true;
                }
                return false;
            });
            //向活动日历信息表添加数据
            foreach (var item in suppliesList)
            {
                var modelAc = new DataAccess.Entity.ActivityCalendar
                {
                    BeginDate = item.BeginDateTime.GetValueOrDefault(),
                    EndDate = item.EndDateTime.GetValueOrDefault(),
                    ActivityTitle = item.Name,
                    ActivityContent = item.Name,
                    ActivityUrl = item.Url,
                    CreateDate = DateTime.Now,
                    CreateBy = "SYSTEM",
                    DataFrom = ActivityObject.Advertise.ToString(),
                    DataFromId = item.PKID,
                    IsActive = true,
                    Owner = "网站首页Banner"
                };
                new ActivityCalendarManager().AddActivityCalendar(modelAc);
            }
            #endregion
            #region
            //获取套餐信息表中新增的套餐信息
            var updateList2 = GetSuppliesModule().Where(_ => _.AdColumnID.EndsWith("www-05")).ToList().FindAll(delegate (Advertise info)
            {
                if (listAc2.Any(_ => _.DataFromId == Convert.ToInt32(info.PKID) && _.CreateDate.Date == DateTime.Now.Date))
                {
                    return true;
                }
                return false;
            });
            foreach (var item in updateList2)
            {
                new ActivityCalendarManager().UpdateIsActivity(Convert.ToInt32(item.PKID), ActivityObject.Advertise.ToString(), item.State == 1 ? true : false);
            }
            #endregion

        }



    }
}