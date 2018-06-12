using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Service.Order;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Request;
using Newtonsoft.Json;
using Tuhu.Service.Product.Models;
using MatchGiftsResponse = Tuhu.Provisioning.Business.Models.MatchGiftsResponse;

namespace Tuhu.Provisioning.Business
{
    /// <summary>
    /// 逻辑处理-SE_GiftManageConfigBLL 
    /// </summary>
    public class SE_GiftManageConfigBLL
    {
        public static IEnumerable<SE_GiftManageConfigModel> SelectPages(int activtyType = 1, int pageIndex = 1, int pageSize = 20, string strWhere = "")
        {
            try
            {
                return SE_GiftManageConfigDAL.SelectPages(ProcessConnection.OpenConfigurationReadOnly, activtyType, pageIndex, pageSize, strWhere);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SE_GiftManageConfigModel GetEntity(int Id)
        {
            try
            {
                return SE_GiftManageConfigDAL.GetEntity(ProcessConnection.OpenConfigurationReadOnly, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Insert(SE_GiftManageConfigModel model, SE_DictionaryConfigModel log)
        {
            try
            {
                var result = SE_GiftManageConfigDAL.Insert(ProcessConnection.OpenConfiguration, model);
                var giftFlag = true;
                log.ParentId = result;
                //var reGiftP = SE_GiftManageConfigDAL.DeleteGiftProductConfig(result);
                var giftProductModels = JsonConvert.DeserializeObject<List<GiftStockModel2>>(model.GiftProducts);
                foreach (var g in giftProductModels)
                {
                    g.RuleId = result;
                    giftFlag = giftFlag && SE_GiftManageConfigDAL.MergeIntoGiftProductStock(g.Pid, g.Stock, result)>0;
                    giftFlag = giftFlag && SE_GiftManageConfigDAL.InsertGiftProductConfig(g) > 0;
                }
                return result > 0 && SE_GiftManageConfigDAL.InsertLog(ProcessConnection.OpenConfiguration, log)&& giftFlag;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(SE_GiftManageConfigModel model)
        {
            try
            {
                var giftFlag = true;
                var reGiftP = SE_GiftManageConfigDAL.DeleteGiftProductConfig(model.Id);
                giftFlag = giftFlag && reGiftP >= 0;
                var giftProductModels = JsonConvert.DeserializeObject<List<GiftStockModel2>>(model.GiftProducts);
                foreach (var g in giftProductModels)
                {
                    g.RuleId = model.Id;
                    giftFlag = giftFlag && SE_GiftManageConfigDAL.MergeIntoGiftProductStock(g.Pid, g.Stock, model.Id) > 0;
                    giftFlag = giftFlag && SE_GiftManageConfigDAL.InsertGiftProductConfig(g) > 0;
                }
                return SE_GiftManageConfigDAL.Update(ProcessConnection.OpenConfiguration, model)&& giftFlag;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Delete(int Id)
        {
            try
            {
                return SE_GiftManageConfigDAL.Delete(ProcessConnection.OpenConfiguration, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 关联功能函数
        /// <summary>
        /// 获取支付方式渠道集合
        /// </summary>
        public static IEnumerable<ChannelDictionariesModel> GetU_ChannelPayList()
        {
            try
            {
                return SE_GiftManageConfigDAL.GetU_ChannelPayListNew(ProcessConnection.OpenGungnirReadOnly);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检测PID是否存在
        /// </summary>
        public static bool CheckPID(string pid)
        {
            try
            {
                return SE_GiftManageConfigDAL.CheckPID(ProcessConnection.OpenGungnirReadOnly, pid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查询产品信息
        /// </summary>
        public static VW_ProductsModel GetVW_ProductsModel(string pid)
        {
            try
            {
                return SE_GiftManageConfigDAL.GetVW_ProductsModel(ProcessConnection.OpenGungnirReadOnly, pid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Dictionary<string, string> GetProductsModel(List<string> pids)
        {

            using (var client = new ProductClient())
            {
                var result = client.SelectCategoryNameAllLeversByPidsAsync(pids);
                result.Result.ThrowIfException();
                return result.Result.Result;
            }


        }

        public static IEnumerable<MatchGiftsResponse> SelectMatchGiftsResponse(MatchGiftsRequest request, bool isOrder)
        {
            using (var client = new GiftsClient())
            {
                if (isOrder)
                {
                    var result = client.SelectOrderGiftWithNoMemoryCacheResponse(request);
                    result.ThrowIfException();
                    return (result.Result.Select(item => new MatchGiftsResponse
                    {
                        Pid = item.Pid,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Require = item.Require,
                        GiftsType = item.GiftsType
                    }));
                }
                else
                {
                    var result = client.SelectProductDetailGiftWithNoMemoryCacheResponse(request);
                    result.ThrowIfException();
                    return (result.Result.Values.SelectMany(r => r).Select(item => new MatchGiftsResponse
                    {
                        Pid = item.Pid,
                        ProductName = item.ProductName,
                        GiftDescription = item.GiftDescription,
                    }));
                }
            }

        }
        public static int SetDisabled()
        {
            return SE_GiftManageConfigDAL.SetDisabled(ProcessConnection.OpenConfiguration);
        }
        /// <summary>
        /// 查询库存信息
        /// </summary>
        public static int? GetGiftProductStock(string pid)
        {
            try
            {
                return SE_GiftManageConfigDAL.GetGiftProductStock(ProcessConnection.OpenGungnirReadOnly, pid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 查询库存信息
        /// </summary>
        public static int? GetGiftProductSendNum(string pid,int ruleId)
        {
            int? result=null;
            try
            {
                using (var client=new GiftsClient())
                {
                    var  cacheresult = client.SelecRecordGiftSendNum(new RecordGiftSendNumRequest()
                    {
                        GiftPid = pid,
                        GiftRuleId = ruleId
                    });
                    if (cacheresult.Result > 0)
                        result = cacheresult.Result;

                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 插入库存信息
        /// </summary>
        public static int InsertGiftProductStock(string pid,int stock)
        {
            try
            {
                return SE_GiftManageConfigDAL.IntsertGiftProductStock( pid,stock);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新库存信息
        /// </summary>
        public static int UpdateGiftProductStock(string pid, int stock)
        {
            try
            {
                return SE_GiftManageConfigDAL.UpdateGiftProductStock(pid, stock);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 删除库存信息
        /// </summary>
        public static int DeleteGiftProductStock(string pid)
        {
            try
            {
                return SE_GiftManageConfigDAL.DeleteGiftProductStock(pid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int UpdateGiftLeveL(int pkid,int sort,string group)
        {
            try
            {
                return SE_GiftManageConfigDAL.UpdateGiftLeveL(pkid,sort,group);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<SE_GiftManageConfigModel>GetGiftLeveL(string group)
        {
            try
            {
                return SE_GiftManageConfigDAL.SelectGiftLeveL(ProcessConnection.OpenConfigurationReadOnly,group);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<SE_GiftManageConfigModel> GetGiftLeveLs()
        {
            try
            {
                return SE_GiftManageConfigDAL.SelectGiftLeveLs(ProcessConnection.OpenConfigurationReadOnly);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int DeleteGiftLeveLs(List<string> groups )
        {
            try
            {

                foreach (var group in groups)
                {
                    SE_GiftManageConfigDAL.DeleteGiftLeveLs(group);
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取赠品销售数量
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public  static int SelectGiftProductSaleOutCount(string pid)
        {
            var count = 0;
            try
            {
                using (var client = new OrderApiForCClient())
                {
                    var result =  client.GetPidCountByTimeRange(pid, null, null);

                    result.ThrowIfException(true);
                    count = result.Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        /// <summary>
        /// 获取赠品销售数量
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static   int SelectGiftStock(string pid)
        {
            if (!string.IsNullOrEmpty(pid) && pid == "initial")
            {
                var datas =
                    SE_GiftManageConfigDAL.SelectAllGiftManageConfigModels(ProcessConnection.OpenConfigurationReadOnly);
                {
                    foreach (var model in datas)
                    {
                        try
                        {

                            SE_GiftManageConfigDAL.DeleteGiftProductConfig(model.Id);
                            if (model.GiftProducts != null && model.GiftProducts.Any())
                            {
                                var giftProductModels =
                                    JsonConvert.DeserializeObject<List<GiftStockModel2>>(model.GiftProducts);
                                foreach (var g in giftProductModels)
                                {
                                    g.IsRetrieve = g.IsRetrieve ?? 0;
                                    g.RuleId = model.Id;
                                    SE_GiftManageConfigDAL.InsertGiftProductConfig(g);
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                    }
                }
            }
            return SE_GiftManageConfigDAL.SelectGiftStock(pid);
        }
        #endregion

        public static string GetPids(int id)
        {
            try
            {
                return SE_GiftManageConfigDAL.GetPids(ProcessConnection.OpenConfigurationReadOnly, id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<string> GetByAllNodes()
        {
            try
            {
                return SE_GiftManageConfigDAL.GetByAllNodes(ProcessConnection.OpenConfigurationReadOnly);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
