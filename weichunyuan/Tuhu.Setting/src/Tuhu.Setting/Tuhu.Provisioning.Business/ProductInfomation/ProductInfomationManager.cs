using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ProductInfomationManagement
{
    public class ProductInfomationManager : IProductInfomationManager
    {
        private static readonly IConnectionManager connectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);

        private static readonly IConnectionManager tuhuProductConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["StarterSite_productcatalogConnectionString"].ConnectionString);
        private static readonly IConnectionManager pcAlwaysOnReadConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString);

        private static readonly IDBScopeManager TuHuProductDbScopeManager = new DBScopeManager(tuhuProductConnectionManager);
        private readonly IDBScopeManager PCAlwaysOnReadDbScopeManager = null;

        private static readonly ILog logger = LoggerFactory.GetLogger("ProductInfomation");

        private ProductInfomationHandler handler = null;
        private ProductInfomationHandler tuHuProductHandler = null;

        public ProductInfomationManager()
        {
            handler = new ProductInfomationHandler(DbScopeManager);
            tuHuProductHandler = new ProductInfomationHandler(TuHuProductDbScopeManager);
            PCAlwaysOnReadDbScopeManager = new DBScopeManager(pcAlwaysOnReadConnectionManager);
        }

        public List<ProductInformation> GetAllProductInfomation(Dictionary<string, string>.KeyCollection pricenNameList, Dictionary<string, string>.KeyCollection stockNameList)
        {
            return handler.GetAllProductInfomation(pricenNameList, stockNameList);
        }

        public List<ProductInfo_Order> GetProductInfo_Order(string PIDS, string OrderChannel)
        {
            return handler.GetProductInfo_Order(PIDS, OrderChannel);
        }
        public List<ProductInfo_Order> GetProductInfo_Order(string PIDS, string OrderChannel, string fromRegion)
        {
            return handler.GetProductInfo_Order(PIDS, OrderChannel, fromRegion);
        }
        public void UpdateCommission(string PID, float Commission)
        {
            handler.UpdateCommission(PID, Commission);
        }
        public void UpdatePrice(string PID, float Price, string ShopCode)
        {
            handler.UpdatePrice(PID, Price, ShopCode);
        }
        public List<ProductUser> GetProductUser(string UserPhone, string UserTel, string TaoBaoID, string CarNO)
        {
            return handler.GetProductUser(UserPhone, UserTel, TaoBaoID, CarNO);
        }
        public List<BizShopSimple> GetShopSimple()
        {
            return handler.GetShopSimple();
        }
        public int SaveOrder(BizOrder bizOrder)
        {
            return handler.SaveOrder(bizOrder);
        }
        public void SaveOrderNo(int PKID, string OrderNo)
        {
            handler.SaveOrderNo(PKID, OrderNo);
        }
        public void SaveOrderItem(OrderListProduct orderListProduct)
        {
            handler.SaveOrderItem(orderListProduct);
        }
        public long PlatFormItemID(string PID, string OrderChannel)
        {
            return handler.PlatFormItemID(PID, OrderChannel);
        }
        public void SaveOrderSumPrice(int PKID, decimal SumMarkedMoney, int SumNum, decimal SumDisMoney, decimal SumMoney)
        {
            handler.SaveOrderSumPrice(PKID, SumMarkedMoney, SumNum, SumDisMoney, SumMoney);
        }

        public FlashSalesProductPara GetFlashSalesProductParaByPID(string PID)
        {
            return handler.GetFlashSalesProductParaByPID(PID);
        }

        public List<string> SelectBaoYangPIDs()
        {
            try
            {
                return handler.SelectBaoYangPIDs();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new ProductInfomationException(BizErrorCode.SystemError, "查询保养PID出错", innerEx);
                logger.Log(Level.Error, innerEx, "Error occurred in GetBaoYangPIDs");

                throw exception;
            }
        }

        public List<ProductInformation> SelectBYPID()
        {
            try
            {
                return handler.SelectBYPID();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new ProductInfomationException(BizErrorCode.SystemError, "查询保养PID出错", innerEx);
                logger.Log(Level.Error, innerEx, "Error occurred in SelectBYPID");

                throw exception;
            }
        }

        public List<ProductSalesPrice> SelectProductSalesPrice()
        {
            try
            {
                return tuHuProductHandler.SelectProductSalesPrice();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerExce)
            {
                var exception = new ProductInfomationException(0, "查询产品渠道销售价出错", innerExce);
                logger.Log(Level.Error, exception, "Error occurred in SelectProductSalesPrice");

                throw exception;
            }
        }

        public void UpdateOrderProductInfoCache(DataTable cacheOrderProductInfo)
        {
            try
            {
                handler.UpdateOrderProductInfoCache(cacheOrderProductInfo);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerExce)
            {
                logger.Log(Level.Error, innerExce, "Error occurred in UpdateOrderProductInfoCache");

                throw;
            }
        }

        public string SelectBrandByPID(string PID)
        {
            try
            {
                return tuHuProductHandler.SelectBrandByPID(PID);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerExce)
            {
                var exception = new ProductInfomationException(0, "查询产品品牌出错", innerExce);
                logger.Log(Level.Error, exception, "Error occurred in SelectBrandByPID");

                throw;
            }
        }
        public List<TireProductModel> GetTireProductByID(string ProductID, string VariantID)
        {
            return DalProductInfomation.GetTireProductByID(ProductID, VariantID);
        }

        public List<SKUProductCategory> GetAllProductCategories()
        {
            List<SKUProductCategory> result = new List<SKUProductCategory>();
            try
            {
                result = PCAlwaysOnReadDbScopeManager.Execute(conn => handler.GetAllProductCategories(conn));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "Error occurred in GetAllProductCategories");
            }

            return result;
        }
        /// <summary>
        /// 获取所有蓄电池的PID
        /// </summary>
        /// <returns></returns>
        public List<SKUPBatteryPID> GetAllBatteryPids()    
        {
            List<SKUPBatteryPID> result=new List<SKUPBatteryPID>();
            try
            {
                result= PCAlwaysOnReadDbScopeManager.Execute(conn => handler.GetAllBatteryPID(conn));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "Error occurred in GetAllBatteryPID");
            }

            return result;
        }    
        public List<Tuple<string, string, int>> GetSkuProductsAndStockQuantity(string productPID,
            string brand, string category)
        {
            List<Tuple<string, string, int>> result = new List<Tuple<string, string, int>>();
            try
            {
                result = PCAlwaysOnReadDbScopeManager.Execute(conn => handler.GetSkuProductsAndStockQuantity(conn,productPID,brand, category));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "Error occurred in GetSkuProductsAndStockQuantity");
            }

            return result;
        }

        public String GetNameById(String pId)
        {
            String result = "";
            try
            {
                result = PCAlwaysOnReadDbScopeManager.Execute(conn => handler.GetNameById(conn, pId));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "Error occurred in GetNameById");
            }

            return result;

        }
    }
}
