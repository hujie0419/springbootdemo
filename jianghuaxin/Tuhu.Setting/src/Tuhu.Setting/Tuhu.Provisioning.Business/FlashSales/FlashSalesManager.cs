using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.ActivityCalendar;
using Tuhu.Provisioning.Business.FlashSalesManagement;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.Business
{
    public class FlashSalesManager : IFlashSalesManager
    {
        #region Private Fields & Ctor
        static string strConn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("FlashSales");

        private FlashSalesHandler handler = null;
        public FlashSalesManager()
        {
            handler = new FlashSalesHandler(DbScopeManager);
        }
        #endregion


        #region FlashSales

        public List<FlashSales> GetAllFlashSales()
        {
            return handler.GetAllFlashSales();
        }

        public void DeleteFlashSales(int id)
        {
            handler.DeleteFlashSales(id);
        }
        public void AddFlashSales(FlashSales flashSales)
        {
            handler.AddFlashSales(flashSales);
        }
        public void UpdateFlashSales(FlashSales flashSales)
        {
            handler.UpdateFlashSales(flashSales);
        }
        public FlashSales GetFlashSalesByID(int id)
        {
            return handler.GetFlashSalesByID(id);
        }
        public void ResetFlashSales(int id)
        {
            handler.ResetFlashSales(id);
        }
        #endregion


        #region FlashSalesProduct
        public List<FlashSalesProduct> GetProListByFlashSalesID(int FlashSalesID)
        {
            return handler.GetProListByFlashSalesID(FlashSalesID);
        }
        public string GetCountByFlashSalesID(int FlashSalesID)
        {
            return handler.GetCountByFlashSalesID(FlashSalesID);
        }
        public void DeleteFlashSalesProduct(int PKID)
        {
            handler.DeleteFlashSalesProduct(PKID);
        }
        public void ChangeStatus(int PKID, byte Status)
        {
            handler.ChangeStatus(PKID, Status);
        }
        public void ChangeIsHotSale(int PKID, bool IsHotSale)
        {
            handler.ChangeIsHotSale(PKID, IsHotSale);
        }
        public void UpdateFlashSalesProduct(FlashSalesProduct flashSalesProduct)
        {
            handler.UpdateFlashSalesProduct(flashSalesProduct);
        }
        public void AddFlashSalesProduct(FlashSalesProduct flashSalesProduct)
        {
            handler.AddFlashSalesProduct(flashSalesProduct);
        }
        #endregion

        public void SelectDataForActivityFromFlashSales()
        {
            string pkidStr = string.Empty;
            //获取活动表中已有的套餐信息
            List<DataAccess.Entity.ActivityCalendar> listAc = new ActivityCalendarManager().SelectActivityByCondition(string.Empty).Where(_ => _.DataFrom.EndsWith(ActivityObject.FlashSales.ToString())).ToList();
            //拼接已录入的活动信息,将来在套餐信息表中排除
            if (listAc.Any())
            {
                pkidStr = listAc.Where(_ => _.DataFromId != null).Aggregate(pkidStr, (current, item) => current + (item.DataFromId.ToString() + ','));
                pkidStr = pkidStr.Substring(0, pkidStr.Length - 1);
            }

            #region 没有的数据要添加

            //获取套餐信息表中新增的套餐信息
            //获取套餐信息表中新增的套餐信息
            List<FlashSales> listZaConfigue;
            if (string.IsNullOrEmpty(pkidStr))
            {
                listZaConfigue = GetAllFlashSales();
            }
            else
            {
                listZaConfigue = GetAllFlashSales().FindAll(delegate (FlashSales info)
                {
                    if (!pkidStr.Split(',').Contains(info.PKID.ToString(CultureInfo.InvariantCulture)))
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
                    BeginDate = item.StartTime,
                    ActivityTitle = item.Name,
                    ActivityContent = item.Name,
                    CreateDate = DateTime.Now,
                    CreateBy = "SYSTEM",
                    DataFrom = ActivityObject.FlashSales.ToString(),
                    DataFromId = item.PKID,
                    IsActive = true
                };
                new ActivityCalendarManager().AddActivityCalendar(modelAc);
            }
            #endregion

            #region
            //获取套餐信息表中新增的套餐信息
            var updateList = GetAllFlashSales().FindAll(delegate (FlashSales info)
            {
                if (pkidStr.Split(',').Contains(info.PKID.ToString(CultureInfo.InvariantCulture)))
                {
                    return true;
                }
                return false;
            });
            foreach (var item in updateList)
            {
                new ActivityCalendarManager().UpdateIsActivity(item.PKID, ActivityObject.FlashSales.ToString(), item.Status == 1 ? true : false);
            }
            #endregion
        }
    }
}
