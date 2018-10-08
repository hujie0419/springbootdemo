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
using Tuhu.Service.Product;

namespace Tuhu.Provisioning.Business.BeautyYearCardConfig
{
    public class BeautyYearCardConfigManger
    {

        private static readonly ILog logger = LogManager.GetLogger<BeautyYearCardConfigManger>();

        private static string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_AlwaysOnRead"].ConnectionString;

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
                        configs.ForEach(f =>
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
                    model.BeautyYearCardRegions.ForEach(f =>
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
                    model.BeautyYearCardRegions.ForEach(f =>
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
                            model.BeautyYearCardRegions.ForEach(f =>
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





    }
}
