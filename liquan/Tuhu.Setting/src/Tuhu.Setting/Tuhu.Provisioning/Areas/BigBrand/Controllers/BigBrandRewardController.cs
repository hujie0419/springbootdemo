using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Mapping;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business;
using System.Linq.Expressions;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity;
using Tuhu.Models;
using Tuhu.Component.Framework;
using Tuhu.Service.Push;

namespace Tuhu.Provisioning.Areas.BigBrand.Controllers
{
    public class BigBrandRewardController : Controller
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("BigBrandPoolController");

        /// <summary>
        /// 新大翻盘列表 页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        /// <summary>
        /// 组合对应奖池字符串
        /// </summary>
        /// <param name="newWheel"></param>
        /// <returns></returns>
        private string GetWheelListLogStr(IEnumerable<BigBrandWheelEntity> newWheel)
        {
            string result = "";
            if (!(newWheel?.Count() > 0))
            {
                return result;
            }

            var groupWheel = newWheel.GroupBy(x => x.TimeNumber);
            foreach (var item in groupWheel)
            {
                var poolText = "";
                foreach (var item1 in item)
                {
                    var poolPKIDs = string.IsNullOrWhiteSpace(item1.FKPoolPKIDText) ? item1.FKPoolPKID.ToString() : item1.FKPoolPKIDText;
                    poolText += poolPKIDs + ",";
                }
                poolText = poolText.Substring(0, poolText.Length - 1);
                result += $"第{item.FirstOrDefault().TimeNumber}轮奖池ID:{poolText},";
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }

        /// <summary>
        /// 提交大翻盘的配置
        /// </summary>
        /// <param name="modeljson"></param>
        /// <param name="wheeljson"></param>
        /// <returns></returns>
        public async Task<ActionResult> SubmitForm(string modeljson, string wheeljson)
        {
            modeljson = modeljson.Replace("\"&nbsp;\"", "null");
            wheeljson = wheeljson.Replace("\"&nbsp;\"", "null");
            try
            {
                BigBrandRewardListEntity entity = JsonConvert.DeserializeObject<BigBrandRewardListEntity>(modeljson);

                IEnumerable<BigBrandWheelEntity> wheel = null;
                if (!string.IsNullOrWhiteSpace(wheeljson))
                {
                    wheel = JsonConvert.DeserializeObject<IEnumerable<BigBrandWheelEntity>>(wheeljson);
                }

                RepositoryManager manager = new RepositoryManager();

                //获取修改前旧数据
                var oldEntity = new BigBrandRewardListEntity();
                var oldWheel = new List<BigBrandWheelEntity>();
                if (entity.PKID > 0)
                {
                    try
                    {
                        oldEntity = manager.GetEntity<BigBrandRewardListEntity>(p => p.PKID == entity.PKID && p.Is_Deleted == false);

                        Expression<Func<BigBrandWheelEntity, bool>> wheelExp = _ => _.FKBigBrand == entity.PKID && _.Is_Deleted == false;
                        oldWheel = manager.GetEntityList<BigBrandWheelEntity>(wheelExp)?.ToList();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(Level.Error, $"ex:{ex}");
                    }
                }

                using (var db = manager.BeginTrans())
                {
                    string errorMsg = string.Empty;
                    var newWheelList = new List<BigBrandWheelEntity>();

                    //判断FKBigBrand，TimeNumber，FKPoolPKID(奖池ID) 是否唯一
                    if (wheel != null)
                    {
                        foreach (var item in wheel)
                        {
                            item.CreateDateTime = DateTime.Now;
                            item.CreateUserName = User.Identity.Name;
                            item.LastUpdateDateTime = DateTime.Now;
                            item.UpdateUserName = User.Identity.Name;
                            item.FKBigBrand = entity.PKID;

                            var array = item.FKPoolPKIDText.TrimEnd(',').Split(',');
                            if (array != null && array.Length > 0)
                            {
                                foreach (var s in array)
                                {
                                    int poolPKID = 0;
                                    if (int.TryParse(s, out poolPKID))
                                    {
                                        if (poolPKID != 0)
                                        {
                                            item.FKPoolPKID = poolPKID;
                                            var wheelPkidList = manager.GetEntityList<BigBrandWheelEntity>(_ => _.FKBigBrand == item.FKBigBrand && _.TimeNumber == item.TimeNumber && _.FKPoolPKID == item.FKPoolPKID && _.Is_Deleted == false).ToList().Select(p => p.PKID).ToList();
                                            var existWheelList = manager.GetEntityList<BigBrandWheelEntity>(_ => _.FKBigBrand == item.FKBigBrand && _.TimeNumber == item.TimeNumber && _.FKPoolPKID == item.FKPoolPKID && _.Is_Deleted == false && !wheelPkidList.Contains(_.PKID));
                                            if (existWheelList != null && existWheelList.ToList().Count > 0)
                                            {
                                                if (!errorMsg.Contains("第" + item.TimeNumber + "轮"))
                                                {
                                                    errorMsg = errorMsg + "第" + item.TimeNumber + "轮奖池ID,";
                                                }
                                            }
                                            else
                                            {
                                                string json = JsonConvert.SerializeObject(item);
                                                var tempModel = JsonConvert.DeserializeObject<BigBrandWheelEntity>(json);
                                                newWheelList.Add(tempModel);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (errorMsg.Length > 0)
                    {
                        errorMsg = errorMsg.TrimEnd(',') + "的值重复了！";
                        return Content(JsonConvert.SerializeObject(new
                        {
                            state = "false",
                            message = errorMsg,
                            data = ""
                        }));
                    }

                    if (entity.PKID == 0)
                    {
                        entity.HashKeyValue = Tuhu.Provisioning.Common.SecurityHelper.Sha1Encrypt(Guid.NewGuid().ToString(), System.Text.Encoding.UTF8).Substring(0, 8);
                        entity.CreateDateTime = DateTime.Now;
                        entity.LastUpdateDateTime = DateTime.Now;
                        entity.UpdateUserName = User.Identity.Name;
                        entity.CreateUserName = User.Identity.Name;
                        manager.Add<BigBrandRewardListEntity>(entity);
                        // LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "新增大翻牌活动", ObjectType = "BigBrand" });
                    }
                    else
                    {
                        entity.LastUpdateDateTime = DateTime.Now;
                        entity.UpdateUserName = User.Identity.Name;
                        manager.Update<BigBrandRewardListEntity>(entity);
                        // LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "更新大翻牌活动", ObjectType = "BigBrand" });
                    }

                    Expression<Func<BigBrandWheelEntity, bool>> exp = _ => _.FKBigBrand == entity.PKID && _.Is_Deleted == false;
                    var bigBrandWheelList = manager.GetEntityList<BigBrandWheelEntity>(exp);
                    foreach (var item in bigBrandWheelList)
                    {
                        item.Is_Deleted = true;
                        item.LastUpdateDateTime = DateTime.Now;
                        item.UpdateUserName = User.Identity.Name;
                        manager.Update<BigBrandWheelEntity>(item);
                    }
                    if (wheel != null)
                    {
                        foreach (var item in newWheelList)
                        {
                            db.Insert<BigBrandWheelEntity>(item);
                        }
                    }
                    db.Commit();
                    // LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(wheel), Author = User.Identity.Name, Operation = "大翻牌明细", ObjectType = "BigBrand" });
                }

                entity = manager.GetEntity<BigBrandRewardListEntity>(p => p.PKID == entity.PKID && p.Is_Deleted == false);

                //操作日志
                SubmitFormLog(oldEntity, entity, oldWheel, wheel);

                //读写分离 做延时
                await Task.Delay(3000);
                #region 更新大翻盘缓存
                using (var client = new Tuhu.Service.Activity.BigBrandClient())
                {
                    var result = client.UpdateBigBrand(entity.HashKeyValue);
                    result.ThrowIfException(true);
                }
                #endregion

                #region 更新 活动页的 缓存
                //筛选 出大翻盘相关的 活动页 配置
                List<ActivePageContentEntity> activeEntityList = manager.GetEntityList<ActivePageContentEntity>(p => p.HashKey == entity.HashKeyValue && p.Type == 20).ToList();
                foreach (var activeEntity in activeEntityList)
                {
                    var activePageList = manager.GetEntity<ActivePageListEntity>(activeEntity.FKActiveID);
                    using (var client = new Tuhu.Service.Activity.ActivityClient())
                    {
                        var result1 = await client.RefreshActivePageListModelCacheAsync(new Service.Activity.Models.Requests.ActivtyPageRequest()
                        {
                            Channel = "wap",
                            HashKey = activePageList.HashKey,
                        });
                        result1.ThrowIfException(true);
                        var result2 = await client.RefreshActivePageListModelCacheAsync(new Service.Activity.Models.Requests.ActivtyPageRequest()
                        {
                            ActivityId = activePageList.PKGuid.Value,
                            Channel = "website",
                            HashKey = activePageList.HashKey,
                        });
                        result2.ThrowIfException(true);
                    }

                    var list = manager.GetEntityList<ActivePageContentEntity>(_ => _.FKActiveID == activeEntity.FKActiveID);
                    var lucy = list?.Where(_ => _.Type == 13)?.FirstOrDefault();
                    if (lucy != null)
                    {
                        using (var client = new Tuhu.Service.Activity.ActivityClient())
                        {
                            var result = await client.RefreshLuckWheelCacheAsync(lucy.ActivityID.ToString());
                            result.ThrowIfException(true);
                        }
                    }
                }

                #endregion

                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
            catch (Exception em)
            {
                throw em;
            }
        }

        /// <summary>
        /// 保存大翻牌配置操作日志
        /// </summary>
        /// <param name="oldInfo"></param>
        /// <param name="newInfo"></param>
        /// <param name="oldWheel"></param>
        /// <param name="newWheel"></param>
        private void SubmitFormLog(BigBrandRewardListEntity oldInfo, BigBrandRewardListEntity newInfo,
            IEnumerable<BigBrandWheelEntity> oldWheel, IEnumerable<BigBrandWheelEntity> newWheel)
        {
            try
            {
                var isAdd = oldInfo?.PKID > 0 ? false : true;
                var operationLogType = isAdd ? "BigBrandReward_Insert" : "BigBrandReward_Update";

                var operationLogModel = new SalePromotionActivityLogModel()
                {
                    ReferId = "BigBrandReward_" + newInfo.PKID,
                    ReferType = "BigBrandReward",
                    OperationLogType = operationLogType,
                    CreateDateTime = DateTime.Now.ToString(),
                    CreateUserName = this.User.Identity.Name,
                    LogDetailList = new List<SalePromotionActivityLogDetail>()
                };
                if (newInfo.Title != oldInfo?.Title)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "名称",
                        NewValue = newInfo.Title,
                        OldValue = isAdd ? "" : oldInfo?.Title
                    });
                }
                if (newInfo.BigBrandType != oldInfo?.BigBrandType)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "抽奖类型",
                        NewValue = newInfo.BigBrandType == 1 ? "普通抽奖" : (newInfo.BigBrandType == 2 ? "积分抽奖" : "定人群抽奖"),
                        OldValue = isAdd ? "" : oldInfo?.BigBrandType == 1 ? "普通抽奖" : (oldInfo?.BigBrandType == 2 ? "积分抽奖" : "定人群抽奖")
                    });
                }
                if (newInfo.PreTimes != oldInfo?.PreTimes)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "抽奖次数",
                        NewValue = newInfo.PreTimes.ToString(),
                        OldValue = isAdd ? "" : oldInfo?.PreTimes.ToString()
                    });
                }
                if (newInfo.CompletedTimes != oldInfo?.CompletedTimes)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "抽奖次数(分享后)",
                        NewValue = newInfo.CompletedTimes.ToString(),
                        OldValue = isAdd ? "" : oldInfo?.CompletedTimes.ToString()
                    });
                }
                if (newInfo.Period != oldInfo?.Period || newInfo.PeriodType != oldInfo?.PeriodType)
                {
                    var newType = newInfo.PeriodType == 1 ? "小时" : (newInfo.PeriodType == 2 ? "天" : "月");
                    var oldType = isAdd ? "" : oldInfo?.PeriodType == 1 ? "小时" : (oldInfo?.PeriodType == 2 ? "天" : "月");
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "抽奖周期",
                        NewValue = newInfo.Period + newType,
                        OldValue = isAdd ? "" : oldInfo?.Period + oldType
                    });
                }
                if (newInfo.StartDateTime != oldInfo?.StartDateTime)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "开始时间",
                        NewValue = newInfo.StartDateTime.ToString(),
                        OldValue = isAdd ? "" : oldInfo?.StartDateTime.ToString()
                    });
                }
                if (newInfo.NeedIntegral != oldInfo?.NeedIntegral)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "单次抽奖消耗积分",
                        NewValue = newInfo.NeedIntegral.ToString(),
                        OldValue = isAdd ? "" : oldInfo?.NeedIntegral.ToString()
                    });
                }
                if (newInfo.AfterLoginType != oldInfo?.AfterLoginType)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "抽奖验证类型",
                        NewValue = newInfo.AfterLoginType == 1 ? "微信授权" : "",
                        OldValue = isAdd ? "" : oldInfo?.AfterLoginType == 1 ? "微信授权" : ""
                    });
                }
                if (newInfo.AfterLoginValue != oldInfo?.AfterLoginValue)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "必须关注公众号",
                        NewValue = GetWXName(newInfo.AfterLoginValue),
                        OldValue = GetWXName(oldInfo?.AfterLoginValue)
                    });
                }
                if (newWheel?.Count() > 0 || oldWheel?.Count() > 0)
                {
                    var logModel = new SalePromotionActivityLogDetail()
                    {
                        Property = "对应奖池",
                        NewValue = GetWheelListLogStr(newWheel),
                        OldValue = isAdd ? "" : GetWheelListLogStr(oldWheel)
                    };
                    if (!(!string.IsNullOrWhiteSpace(logModel.OldValue) && logModel.OldValue == logModel.NewValue))
                    {
                        operationLogModel.LogDetailList.Add(logModel);
                    }
                }
                SetOperationLog(operationLogModel, "SubmitFormLog");
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Warning, ex, $"SubmitFormLog异常,ex:{ex}");
            }
        }

        /// <summary>
        /// 获取微信公众号名称
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetWXName(string value)
        {
            var resultStr = "";
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            var splitList = value.Split(',');
            using (var client = new WeiXinPushClient())
            {
                var clientResult = client.SelectWxConfigs();
                if (clientResult.Success)
                {
                    var wxOpenList = clientResult.Result.Where(t => (t.Type == "WX_Open") && !string.IsNullOrWhiteSpace(t.channel))
                    ?.Select(t => new { t.name, t.channel });
                    var containsList = wxOpenList?.Where(t => splitList.Contains(t.channel))?.Select(a => a.name)?.ToList();
                    if (containsList?.Count() > 0)
                    {
                        resultStr = string.Join(",", containsList);
                    }
                }
            }
            return resultStr;
        }

        /// <summary>
        /// 获取大翻牌抽奖配置
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult GetGridJson(string keyword, Pagination pagination)
        {
            var list = new List<BigBrandRewardListEntity>();
            RepositoryManager manager = new RepositoryManager();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                Expression<Func<BigBrandRewardListEntity, bool>> expression = _ => (_.Title.Contains(keyword.Trim()) || _.HashKeyValue.Contains(keyword.Trim())) && _.Is_Deleted == false;
                list = manager.GetEntityList<BigBrandRewardListEntity>(expression, pagination);
            }
            else
            {
                Expression<Func<BigBrandRewardListEntity, bool>> expression = _ => _.Is_Deleted == false;
                list = manager.GetEntityList<BigBrandRewardListEntity>(expression, pagination);
            }

            return Content(JsonConvert.SerializeObject(new
            {
                rows = list,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            }));
        }


        public ActionResult GetFormJson(int? pkid)
        {
            RepositoryManager manager = new RepositoryManager();
            Expression<Func<BigBrandRewardListEntity, bool>> exp = _ => _.PKID == pkid.Value && _.Is_Deleted == false;
            var entity = manager.GetEntity<BigBrandRewardListEntity>(exp);

            Expression<Func<BigBrandWheelEntity, bool>> wheelExp = _ => _.FKBigBrand == pkid.Value && _.Is_Deleted == false;

            var wheelList = manager.GetEntityList<BigBrandWheelEntity>(wheelExp);

            return Content(JsonConvert.SerializeObject(new
            {
                FormData = entity,
                WheelData = wheelList
            }));
        }


        public async Task<ActionResult> DeleteForm(int? pkid)
        {
            if (pkid.HasValue)
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandPageStyleEntity, bool>> exp1 = _ => _.FKPKID == pkid.Value && _.Is_Deleted == false;

                var pageStyleList = manager.GetEntityList<BigBrandPageStyleEntity>(exp1);
                foreach (var item in pageStyleList)
                {
                    item.Is_Deleted = true;
                    item.LastUpdateDateTime = DateTime.Now;
                    item.UpdateUserName = User.Identity.Name;
                    manager.Update<BigBrandPageStyleEntity>(item);
                }


                Expression<Func<BigBrandRewardPoolEntity, bool>> exp2 = _ => _.FKPKID == pkid.Value && _.Status == true;

                var rewardPoolList = manager.GetEntityList<BigBrandRewardPoolEntity>(exp2);
                foreach (var item in rewardPoolList)
                {
                    item.Status = false;
                    item.LastUpdateDateTime = DateTime.Now;
                    item.UpdateUserName = User.Identity.Name;
                    manager.Update<BigBrandRewardPoolEntity>(item);
                }

                Expression<Func<BigBrandRewardListEntity, bool>> exp = _ => _.PKID == pkid.Value && _.Is_Deleted == false;
                var originEntity = manager.GetEntity<BigBrandRewardListEntity>(exp);
                var entity = manager.GetEntity<BigBrandRewardListEntity>(exp);

                if (entity != null)
                {
                    entity.Is_Deleted = true;
                    entity.LastUpdateDateTime = DateTime.Now;
                    entity.UpdateUserName = User.Identity.Name;
                    manager.Update<BigBrandRewardListEntity>(entity);
                }

                // LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(originEntity), Author = User.Identity.Name, Operation = "删除大翻牌活动", ObjectType = "BigBrand" });

                //读写分离 做延时
                await Task.Delay(2000);

                #region 更新大翻盘缓存
                var bigBrandPoolController = new BigBrandPoolController();
                bigBrandPoolController.RefreshBigBrandCache(pkid);
                #endregion

                //操作日志
                DeleteFormLog(pkid);

                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
            else
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "参数不能为空",
                    data = ""
                }));
        }

        /// <summary>
        /// 删除大翻牌活动操作日志
        /// </summary>
        /// <param name="pkid"></param>
        private void DeleteFormLog(int? pkid)
        {
            try
            {
                var operationLogType = "BigBrandReward_Delete";

                var operationLogModel = new SalePromotionActivityLogModel()
                {
                    ReferId = "BigBrandReward_" + pkid,
                    ReferType = "BigBrandReward",
                    OperationLogType = operationLogType,
                    CreateDateTime = DateTime.Now.ToString(),
                    CreateUserName = this.User.Identity.Name,
                    LogDetailList = new List<SalePromotionActivityLogDetail>()
                };
                SetOperationLog(operationLogModel, "DeleteFormLog");
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Warning, ex, $"DeleteFormLog异常,ex:{ex}");
            }
        }

        /// <summary>
        /// 刷新大翻盘的缓存
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult Reload(int? pkid)
        {
            RepositoryManager manager = new RepositoryManager();
            Expression<Func<BigBrandRewardListEntity, bool>> exp = _ => _.PKID == pkid.Value && _.Is_Deleted == false;
            var entity = manager.GetEntity<BigBrandRewardListEntity>(exp);

            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.UpdateBigBrand(entity.HashKeyValue);
                result.ThrowIfException(true);
                if (result.Result)
                    return Content(JsonConvert.SerializeObject(new
                    {
                        Code = 1,
                        Msg = "刷新成功"
                    }));
                else
                    return Content(JsonConvert.SerializeObject(new
                    {
                        Code = 0,
                        Msg = "刷新失败"
                    }));
            }
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="operationLogModel"></param>
        /// <param name="funNameString"></param>
        private void SetOperationLog(SalePromotionActivityLogModel operationLogModel, string funNameString)
        {
            try
            {
                using (var logClient = new SalePromotionActivityLogClient())
                {
                    var logResult = logClient.InsertAcitivityLogAndDetail(operationLogModel);
                    if (!(logResult.Success && logResult.Result))
                    {
                        Logger.Log(Level.Warning, $"{funNameString}操作日志记录失败ErrorMessage:{logResult.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"{funNameString},操作日志记录异常，ex{ex}");
            }
        }

        /// <summary>
        /// 获取活动的操作日志列表
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult GetOperationLogList(string activityid, int pageIndex = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(activityid))
            {
                activityid = Request.QueryString["activityid"];
            }
            ViewBag.activityid = activityid;
            var result = new PagedModel<SalePromotionActivityLogModel>();
            using (var client = new SalePromotionActivityLogClient())
            {
                var listResult = client.GetOperationLogList(activityid, pageIndex, pageSize);
                if (listResult.Success && listResult.Result != null)
                {
                    result = listResult.Result;
                }
            }
            ViewBag.pageSize = pageSize;
            ViewBag.pageIndex = pageIndex;
            ViewBag.totalRecords = result.Pager.Total;
            ViewBag.totalPage = result.Pager.Total % pageSize == 0
                ? result.Pager.Total / pageSize : (result.Pager.Total / pageSize) + 1;
            return View("OperationLogList", result.Source);
        }

        /// <summary>
        /// 获取日志详情
        /// </summary>
        /// <param name="FPKID"></param>
        /// <returns></returns>
        public ActionResult GetOperationLogDetailList(string PKID)
        {
            var result = new List<SalePromotionActivityLogDetail>();
            using (var client = new SalePromotionActivityLogClient())
            {
                var listResult = client.GetOperationLogDetailList(PKID);
                if (listResult.Success && listResult.Result != null)
                {
                    result = listResult.Result.ToList();
                }
                if (result?.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        item.OldValue = item.OldValue == null ? "" : item.OldValue;
                        item.NewValue = item.NewValue == null ? "" : item.NewValue;
                    }
                }
            }
            return Json(result);
        }

    }
}