using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.BeautyYearCardDao;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BeautyYearCard;
using Tuhu.Service.KuaiXiu.Enums;
using Tuhu.Service.Product;

namespace Tuhu.Provisioning.Business.BeautyYearCardConfig
{
    public class BeautyYearCardConfigManger
    {

        private static readonly ILog logger = LogManager.GetLogger<BeautyYearCardConfigManger>();

        private static string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_AlwaysOnRead"].ConnectionString;
        private const string _yearCardServiceCodeSource = "Tuhu";
        private const string _yearCardServiceCodeChannel = "MeiRongYearCard";
        private const string _yearCardServiceCodeRevertSource = "TuhuRevert";
        private const string _yearCardLogType = "InvalidateBeautyYearCardServiceCode";
        private BeautyYearCardConfigManger()
        {
            BeautyYearCardProductId = ConfigurationManager.AppSettings["BeautyYearCardProductId"];
            if (string.IsNullOrEmpty(BeautyYearCardProductId))
                throw new KeyNotFoundException("美容年卡主产品未配置！");
        }
        private static BeautyYearCardConfigManger _Instanse;
        private static string BeautyYearCardProductId;
        public static BeautyYearCardConfigManger Instanse
        {
            get
            {
                if (_Instanse == null)
                    _Instanse = new BeautyYearCardConfigManger();
                return _Instanse;
            }
        }
        #region 美容年卡
        public Tuple<int, IEnumerable<BeautyYearCardModel>> GetBeautyYearCardConfigList(int pageIndex, int pageSize, string keyWord, int nameType, int statusType)
        {
            Tuple<int, IEnumerable<BeautyYearCardModel>> result = null;
            try
            {
                result = DalBeautyYearCardConfig.SelectBeautyYearCardConfigs(pageIndex, pageSize, keyWord, nameType, statusType);
                if (result != null && result.Item1 > 0 && result.Item2.Any())
                {
                    var configs = result.Item2.ToArray();
                    var productConfigs = DalBeautyYearCardConfig.SelectBeautyYearCardProductConfigs(configs.Select(s => s.PKID).ToArray());
                    if (productConfigs != null || productConfigs.Any())
                    {
                        configs?.ToList().ForEach(f =>
                        {
                            f.BeautyYearCardProducts = productConfigs.Where(w => w.CardId == f.PKID)?.ToArray();
                        });
                        result = Tuple.Create(result.Item1, configs.AsEnumerable());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result;
        }
        public BeautyYearCardModel GetBeautyYearCardConfigDetail(int cardId)
        {
            BeautyYearCardModel result = null;
            try
            {
                result = DalBeautyYearCardConfig.GetBeautyYearCardConfig(cardId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            //CreateBeautyYearCardConfig
            return result;
        }
        public bool SaveBeautyYearCardConfig(BeautyYearCardModel model, string user)
        {
            try
            {
                if (model.PKID > 0)//编辑
                {
                    model.BeautyYearCardRegions?.ToList().ForEach(f =>
                    {
                        f.CityId = (f.IsAllCity ? null : f.CityId);
                    });
                    var oldObject = GetBeautyYearCardConfigDetail(model.PKID);
                    var log = new BeautyOprLog
                    {
                        LogType = "UpdateBeautyYearCardConfig",
                        IdentityID = $"{model.PKID}",
                        OldValue = JsonConvert.SerializeObject(oldObject),
                        NewValue = JsonConvert.SerializeObject(model),
                        Remarks = $"更新美容年卡配置",
                        OperateUser = user,
                    };
                    model.BeautyYearCardRegions?.ToList().ForEach(f =>
                    {
                        f.CityId = (f.IsAllCity ? null : f.CityId);
                    });
                    var result = DalBeautyYearCardConfig.UpdateBeautyYearCardConfig(model);

                    LoggerManager.InsertLog("BeautyOprLog", log);
                    return result;
                }
                else//新增
                {
                    model.CardPrice = Math.Round(model.CardPrice);
                    using (var client = new ProductClient())
                    {
                        var product = DalBeautyYearCardConfig.GetParentProuductInfo(BeautyYearCardProductId);
                        if (product == null)
                            return false;
                        var pid = client.CreateProductV2(new Service.Product.Models.WholeProductInfo
                        {
                            ProductID = BeautyYearCardProductId,
                            Image_filename = model.CardImageUrl,
                            Description = model.CardName,
                            DefinitionName = product.Item3,
                            PrimaryParentCategory = product.Item2,
                            CatalogName = product.Item1,
                            DisplayName=model.CardName,
                            Name = model.CardName,
                            cy_list_price = model.CardPrice,
                            cy_marketing_price = model.CardPrice,
                        },
                           user,
                           Service.Product.Enum.ChannelType.MenDian);
                        pid.ThrowIfException(true);
                        if (!string.IsNullOrEmpty(pid.Result))
                        {
                            model.PID = pid.Result;
                            model.BeautyYearCardRegions?.ToList().ForEach(f =>
                            {
                                f.CityId = (f.IsAllCity ? null : f.CityId);
                            });
                            var result = DalBeautyYearCardConfig.CreateBeautyYearCardConfig(model);

                            var log = new BeautyOprLog
                            {
                                LogType = "CreateBeautyYearCardConfig",
                                IdentityID = $"{result}",
                                OldValue = null,
                                NewValue = JsonConvert.SerializeObject(model),
                                Remarks = $"创建美容年卡配置",
                                OperateUser = user,
                            };
                            LoggerManager.InsertLog("BeautyOprLog", log);
                            return result > 0;

                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"SaveBeautyYearCardConfig:{ex.Message}", ex);
                return false;
            }

        }
        public bool DeleteBeautyYearCardCofing(int pkid, string user)
        {
            try
            {
                var oldObject = GetBeautyYearCardConfigDetail(pkid);
                var result = DalBeautyYearCardConfig.DeleteBeautyYearCardConfig(pkid);
                var log = new BeautyOprLog
                {
                    LogType = "UpdateBeautyYearCardConfig",
                    IdentityID = $"{pkid}",
                    OldValue = JsonConvert.SerializeObject(oldObject),
                    NewValue = null,
                    Remarks = $"删除美容年卡配置",
                    OperateUser = user,
                };
                LoggerManager.InsertLog("BeautyOprLog", log);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error($"DeleteBeautyYearCardCofing:{ex.Message}", ex);
                return false;
            }

        }

        public BeautyOprLog[] GetBeautyYearCardConfigLog(int cardId)
        {
            try
            {
                return DalBeautyYearCardConfig.GetBeautyYearCardConfigLog(cardId);
            }
            catch (Exception ex)
            {
                logger.Error($"GetBeautyYearCardConfigLog:{ex.Message}", ex);
                return null;
            }

        }

        public ProductSimpleModel GetProductSimpleInfo(string pid)
        {
            ProductSimpleModel result = null;
            try
            {
                result = DalBeautyYearCardConfig.GetSubProuductInfo(pid);
            }
            catch (Exception ex)
            {
                logger.Error($"GetProductSimpleInfo:{ex.Message}", ex);
            }
            return result;
        }
        #endregion
        /// <summary>
        /// 查询用户年卡记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<ShopBeautyUserYearCard>, int> GetShopBeautyUserYearCards(string userId, string orderId, int pageIndex, int pageSize)
        {
            Tuple<IEnumerable<ShopBeautyUserYearCard>, int> result = null;

            try
            {
                result = DalBeautyYearCardConfig.SelectShopBeautyUserYearCards(userId, orderId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                logger.Error($"GetShopBeautyUserYearCards:{ex.Message}", ex);
            }

            return result;
        }
        /// <summary>
        /// 批量查询年卡配置
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<BeautyYearCardModel> GetBeautyYearCardModelsByCardIds(IEnumerable<int> cardIds)
        {
            IEnumerable<BeautyYearCardModel> result = null;

            try
            {
                result = DalBeautyYearCardConfig.SelectBeautyYearCardModelsByCardIds(cardIds);
            }
            catch (Exception ex)
            {
                logger.Error($"GetBeautyYearCardModelsByCardIds:{ex.Message}", ex);
            }

            return result ?? new List<BeautyYearCardModel>();
        }
        /// <summary>
        /// 查询用户年卡详情
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<ShopBeautyUserYearCardDetail> GetShopBeautyUserYearCardDetails(int userYearCardId)
        {
            IEnumerable<ShopBeautyUserYearCardDetail> result = null;

            try
            {
                result = DalBeautyYearCardConfig.SelectShopBeautyUserYearCardDetails(userYearCardId);
            }
            catch (Exception ex)
            {
                logger.Error($"GetShopBeautyUserYearCardDetails:{ex.Message}", ex);
            }

            return result ?? new List<ShopBeautyUserYearCardDetail>();
        }
        /// <summary>
        /// 批量查询用户年卡详情
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<ShopBeautyUserYearCardDetail> GetShopBeautyUserYearCardDetails(IEnumerable<int> userYearCardIds)
        {
            IEnumerable<ShopBeautyUserYearCardDetail> result = null;

            try
            {
                result = DalBeautyYearCardConfig.SelectShopBeautyUserYearCardDetails(userYearCardIds);
            }
            catch (Exception ex)
            {
                logger.Error($"GetShopBeautyUserYearCardDetails:{ex.Message}", ex);
            }

            return result ?? new List<ShopBeautyUserYearCardDetail>();
        }
        /// <summary>
        /// 作废年卡服务码
        /// </summary>
        /// <param name="serviceCodes"></param>
        /// <param name="yearCardId"></param>
        /// <param name="operateUser"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> InvalidateServiceCodes(IEnumerable<string> serviceCodes,int yearCardId, string operateUser)
        {
            var result = false;
            string msg = string.Empty;
            try
            {
                var cardDetails = GetShopBeautyUserYearCardDetails(yearCardId);
                var kuaiXiuService = new KuaiXiuService.KuaiXiuService();
                var allServiceCodeDetails = await kuaiXiuService.GetServiceCodeDetailsByCodes(cardDetails.Select(t => t.FwCode));
                var effictiveRequestCodes = cardDetails.Join(serviceCodes, x => x.FwCode, y => y, (x, y) => { return y; });         
                if (effictiveRequestCodes.Count() == serviceCodes.Count())
                {
                    if (effictiveRequestCodes.Join(allServiceCodeDetails, x => x, y => y.Code, (x, y) => { return y; }).All(s => (s.Status == ServiceCodeStatusType.Created || s.Status == ServiceCodeStatusType.SmsSent) && !string.Equals(s.Source, _yearCardServiceCodeRevertSource)))
                    {
                        result = await kuaiXiuService.RevertServiceCodes(serviceCodes, _yearCardServiceCodeChannel, _yearCardServiceCodeSource);
               
                        var otherServiceCodeDetails = allServiceCodeDetails.Where(t => !serviceCodes.Contains(t.Code));
                        if (result && otherServiceCodeDetails.All(s => (s.Status != ServiceCodeStatusType.Created && s.Status != ServiceCodeStatusType.SmsSent) || string.Equals(s.Source, _yearCardServiceCodeRevertSource)))
                        {
                            var invalidResult = DalBeautyYearCardConfig.InvalidUserBeautyYearCard(yearCardId);
                            if (!invalidResult)
                                logger.Error($"更新年卡作废状态失败,serviceCodes:{JsonConvert.SerializeObject(serviceCodes)},yearCardId:{yearCardId}");
                        }
                        await InsertInvalidateBeautyYearCardLog(serviceCodes, yearCardId, operateUser, result);
                    }
                    else
                    {
                        msg = "包含不可作废的选项";
                    }
                }
                else
                {
                    msg = "参数错误";
                }
            }
            catch (Exception ex)
            {
                logger.Error($"InvalidateServiceCodes:{ex.Message}", ex);
            }

            return new Tuple<bool, string>(result, msg);
        }

        /// <summary>
        /// 查询作废年卡美容操作日志
        /// </summary>
        /// <param name="identityId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BeautyOprLog>> GetInvalidateCodeOprLog(string yearCardId)
        {
            return DalBeautyYearCardConfig.GetBeautyOprLog(yearCardId, _yearCardLogType);
        }
        public async Task<bool> InsertInvalidateBeautyYearCardLog(IEnumerable<string> serviceCodes, int yearCardId, string operateUser, bool result)
        {
            bool insertResult = true;
            if (serviceCodes != null && serviceCodes.Any())
            {
                foreach (var serviceCode in serviceCodes)
                {
                    var log = new BeautyOprLog
                    {
                        LogType = _yearCardLogType,
                        IdentityID = $"{yearCardId}",
                        OldValue = serviceCode,
                        NewValue = result ? $"作废成功" : "作废失败",
                        Remarks = $"作废美容年卡服务码",
                        OperateUser = operateUser,
                    };
                    insertResult = await LoggerManager.InsertLogAsync("BeautyOprLog", log);
                }
            }
            return insertResult;
        }
    }
}
