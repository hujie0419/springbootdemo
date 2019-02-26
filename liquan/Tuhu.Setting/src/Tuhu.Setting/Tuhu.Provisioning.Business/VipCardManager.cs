using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class VipCardManager
    {
        public enum OpertionType
        {
            Add = 1,
            EditAdd = 2,
            EditDelete = 3,
            EditEdit = 4

        }

        private static readonly string UrlPrfex = ConfigurationManager.AppSettings["VipUrl"];
        private static readonly ILog logger = LogManager.GetLogger<VipCardManager>();
        public static ListModel<VipCardModel> GetVipCardSaleList(int pageIndex, int pageSize, int clientId)
        {
            List<VipCardModel> result = new List<VipCardModel>();
            var list = new List<string>();
            try
            {
                var dbresult = DalVipCard.GetVipCardSaleList(pageIndex, pageSize, clientId).ToList();
                //list.AddRange(dbresult.Select(item => item.CardValue + "(" + item.SalePrice + ")"));
                result =
                    dbresult.GroupBy(r => new { r.ActivityId, r.ActivityName, r.ClientId, r.ClientName, r.Url, r.CreateDateTime, r.LastUpdateDateTime })
                        .Select(v => new VipCardModel
                        {
                            ActivityId = v.Key.ActivityId,
                            ActivityName = v.Key.ActivityName,
                            ClientId = v.Key.ClientId,
                            ClientName = v.Key.ClientName,
                            Url = v.Key.Url,
                            VipCards = string.Join(";", v.Select(item => item.CardValue + "(" + item.SalePrice + ")")),
                            CreateDateTime = v.Key.CreateDateTime,
                            LastUpdateDateTime = v.Key.LastUpdateDateTime

                        }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return new ListModel<VipCardModel>
            {
                Source = result.ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize),
                Pager = new PagerModel(pageIndex, pageSize)
                {
                    TotalItem = result.ToList().Count
                }
            };
        }

        public static List<VipCardDetailModel> GetAllClients()
        {
            try
            {
                return DalVipCard.GetAllClients().ToList();

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return new List<VipCardDetailModel>
            {
            };
        }

        public static List<VipCardDetailModel> GetBatchesByClientId(int clientId)
        {
            try
            {
                return DalVipCard.GetBatchesByClientId(clientId).ToList();

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return new List<VipCardDetailModel>
            {
            };
        }

        public static bool InsertVipCardModelAndDetails(List<VipCardDetailModel> details, string activityName, int clientId)
        {
            bool result;
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                try
                {
                    dbHelper.BeginTransaction();
                    var clientName = GetBatchesByClientId(clientId).Select(r => r.ClientName).FirstOrDefault();
                    var activityId = Guid.NewGuid();
                    var vipCardModel = new VipCardModel()
                    {
                        ActivityId = activityId,
                        ActivityName = activityName,
                        Url = UrlPrfex + activityId
                    };
                    var insertResult = DalVipCard.InsertVipCardModel(vipCardModel, dbHelper);
                    result = insertResult > 0;
                    if (insertResult <= 0)
                    {
                        dbHelper.Rollback();
                    }
                    else
                    {
                        var list = new List<VipCardDetailModel>();
                        if (details != null)
                        {
                            list = details.Select(r =>
                            {
                                r.ClientName = clientName;
                                r.ClientId = clientId;
                                return r;
                            }).ToList();
                        }
                        var insertDetail = DalVipCard.InsertVipCardDetailsModel(list, insertResult, dbHelper);
                        result = result && insertDetail;
                        if (!insertDetail)
                        {
                            dbHelper.Rollback();
                        }
                        else
                        {
                            OpertionLogs(OpertionType.Add, "", "", activityId.ToString());
                            dbHelper.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    result = false;
                    logger.Error(ex.Message, ex);
                }
            }
            return result;
        }

        public static List<VipCardDetailModel> GetVipCardDetailForEdit(string activityId, int clientId)
        {
            try
            {
                var cardId = DalVipCard.GetVipCardIdByActivityId(activityId);
                var cardDetails = DalVipCard.GetVipCardDetailsByActivityId(cardId).ToList();
                var batchDetails = DalVipCard.GetBatchesByClientId(clientId).ToList();
                var result = (from a in batchDetails
                              join b in cardDetails on a.BatchId equals b.BatchId into temp
                              from b in temp.DefaultIfEmpty()
                              select new VipCardDetailModel
                              {
                                  _checked = b != null,
                                  Stock = a.Stock,
                                  EndDate = a.EndDate,
                                  StartDate = a.StartDate,
                                  UseRange = a.UseRange,
                                  SalePrice = a.SalePrice,
                                  CardValue = a.CardValue,
                                  CardName = a.CardName,
                                  BatchId = a.BatchId,
                                  ClientName = a.ClientName,
                                  ClientId = a.ClientId
                              }).ToList()
                ;
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return new List<VipCardDetailModel>
            {
            };
        }

        public static bool UpdateVipCardModelAndDetails(List<VipCardDetailModel> details, string activityName, string activityId, int clientId)
        {
            bool result = true;
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                try
                {
                    //dbHelper.BeginTransaction();
                    var clientName = GetBatchesByClientId(clientId).Select(r => r.ClientName).FirstOrDefault();
                    var detailbefore = DalVipCard.GetVipCardDetailsByActivityId(activityId);
                    var cardId = DalVipCard.GetVipCardIdByActivityId(activityId);
                    var batchIds = details.Select(r => r.BatchId).ToList();

                    var IsNeedUpdateAct = false;

                    #region log

                    var batchIdBefores = detailbefore.Select(r => r.BatchId).ToList();
                    var actNamebefore = detailbefore.Select(r => r.ActivityName).FirstOrDefault();
                    var deleteBatches = batchIdBefores.Except(batchIds);
                    var addBatches = batchIds.Except(batchIdBefores);
                    if (actNamebefore != activityName)
                    {
                        IsNeedUpdateAct = true;
                        var before = actNamebefore;
                        var after = activityName;
                        OpertionLogs(OpertionType.EditEdit, before, after, activityId);
                    }
                    if (deleteBatches.Any() && batchIds.Any())
                    {
                        var before = string.Join(",", deleteBatches);
                        OpertionLogs(OpertionType.EditDelete, before, "", activityId);
                    }
                    if (addBatches.Any())
                    {
                        var after = string.Join(",", addBatches);
                        OpertionLogs(OpertionType.EditAdd, "", after, activityId);
                    }

                    #endregion

                    if (IsNeedUpdateAct)
                        DalVipCard.UpdateVipCardModel(activityName, activityId, dbHelper);
                    if (batchIds.Any())
                    {
                        var uDetail1 = DalVipCard.SetVipCardModelDetailStatus(cardId, batchIds, dbHelper);

                        var list = details.Select(r =>
                        {
                            r.ClientName = clientName;
                            r.ClientId = clientId;

                            return r;
                        }).ToList();
                        var uDetails2 = DalVipCard.UpdateVipCardModelDetails(cardId, list, dbHelper);
                    }
                }

                catch (Exception ex)
                {
                    result = false;
                    logger.Error(ex.Message, ex);
                }
            }
            return result;
        }

        public static bool OpertionLogs(OpertionType type, string beforevalue, string afterValue, string activityId)
        {
            var oprLog = new FlashSaleProductOprLog
            {
                OperateUser = ThreadIdentity.Operator.Name,
                CreateDateTime = DateTime.Now,
                BeforeValue = beforevalue,
                AfterValue = afterValue,
                LogType = "VipCard",
                LogId = activityId,
                Operation = ""
            };
            switch (type)
            {
                case OpertionType.Add:
                    oprLog.Operation = "Add";
                    LoggerManager.InsertLog("VipCardOprLog", oprLog);
                    return true;
                case OpertionType.EditAdd:
                    oprLog.Operation = "EditAdd";
                    LoggerManager.InsertLog("VipCardOprLog", oprLog);
                    return true;
                case OpertionType.EditDelete:

                    oprLog.Operation = "EditDelete";
                    LoggerManager.InsertLog("VipCardOprLog", oprLog);
                    return true;
                case OpertionType.EditEdit:

                    oprLog.Operation = "EditEdit";
                    LoggerManager.InsertLog("VipCardOprLog", oprLog);
                    return true;
                default:
                    return true;
            }
        }
    }
}
