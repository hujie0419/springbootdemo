using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Mapping;
using Tuhu.Provisioning.Business;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity;
using Tuhu.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO.Repository;

namespace Tuhu.Provisioning.Areas.BigBrand.Controllers
{
    public class BigBrandPoolController : Controller
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("BigBrandPoolController");

        /// <summary>
        /// 大翻盘 配置页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FormPool()
        {
            return View();
        }

        public ActionResult OperateLogList(int PKID)
        {
            return View();
        }

        public ActionResult GetGridJson(int? fkpkid)
        {
            if (fkpkid.HasValue)
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandRewardPoolEntity, bool>> exp = _ => _.FKPKID == fkpkid.Value && _.Status == true && _.RewardType != -1;
                var list = manager.GetEntityList<BigBrandRewardPoolEntity>(exp);
                var treeList = new List<TreeGridModel>();
                foreach (BigBrandRewardPoolEntity item in list)
                {
                    TreeGridModel treeModel = new TreeGridModel();
                    treeModel.id = item.PKID.ToString();
                    bool hasChildren = list.Count(t => t.ParentPKID == item.PKID) == 0 ? false : true;
                    treeModel.text = item.Name;
                    item.Name = hasChildren == false ? "<span style=\"color:blue;\">" + item.Name + "</span>" : item.Name;
                    treeModel.isLeaf = hasChildren;
                    treeModel.parentId = string.IsNullOrWhiteSpace(item.ParentPKID.ToString()) ? "0" : item.ParentPKID.ToString();
                    treeModel.expanded = false;
                    treeModel.entityJson = JsonConvert.SerializeObject(item);

                    treeList.Add(treeModel);
                }
                return Content(treeList.TreeGridJson());
            }
            return Content("null");
        }

        /// <summary>
        /// 保存奖池
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ActionResult> SubmitForm(BigBrandRewardPoolEntity entity)
        {
            RepositoryManager manager = new RepositoryManager();

            //操作日志需要的数据
            bool isAdd = false;
            var oldInfo = new BigBrandRewardPoolEntity();

            if (entity.PKID == 0)
            {
                isAdd = true;
                entity.CreateDateTime = DateTime.Now;
                entity.LastUpdateDateTime = DateTime.Now;
                entity.CreateUserName = User.Identity.Name;
                entity.UpdateUserName = User.Identity.Name;
                entity.RewardType = 0;
                entity.Status = true;
                manager.Add<BigBrandRewardPoolEntity>(entity);
                // LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "新增大翻牌活动奖池信息", ObjectType = "BigBrand" });
            }
            else
            {
                try
                {
                    Expression<Func<BigBrandRewardListEntity, bool>> exp = _ => _.PKID == entity.PKID && _.Is_Deleted == false;
                    oldInfo = manager.GetEntity<BigBrandRewardPoolEntity>(exp);
                }
                catch (Exception ex)
                {
                    Logger.Log(Level.Error, $"ex:{ex}");
                }

                // var before = manager.GetEntity<BigBrandRewardPoolEntity>(entity.PKID);
                entity.LastUpdateDateTime = DateTime.Now;
                entity.UpdateUserName = User.Identity.Name;
                entity.Status = true;
                manager.Update<BigBrandRewardPoolEntity>(entity);
                // LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "更新大翻牌活动奖池信息", ObjectType = "BigBrand" });
            }

            //读写分离 做延时
            await Task.Delay(2000);

            #region 更新大翻盘缓存
            RefreshBigBrandCache(entity.FKPKID);
            #endregion

            //操作日志
            SubmitFormLog(oldInfo, entity, isAdd);

            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));
        }

        /// <summary>
        /// 保存奖池操作日志
        /// </summary>
        /// <param name="oldInfo"></param>
        /// <param name="newInfo"></param>
        private void SubmitFormLog(BigBrandRewardPoolEntity oldInfo, BigBrandRewardPoolEntity newInfo, bool isAdd)
        {
            //编辑时 操作日志记在奖池上，新增、删除记在活动上
            try
            {
                var operationLogModel = new SalePromotionActivityLogModel()
                {
                    ReferId = isAdd ? "BigBrandReward_" + newInfo.FKPKID : "BigBrandPool_" + newInfo.PKID,
                    ReferType = "BigBrandReward",
                    OperationLogType = isAdd ? "BigBrandPool_Insert" : "BigBrandPool_Update",
                    CreateDateTime = DateTime.Now.ToString(),
                    CreateUserName = this.User.Identity.Name,
                    LogDetailList = new List<SalePromotionActivityLogDetail>()
                };
                if (isAdd)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "奖池ID",
                        NewValue = newInfo.PKID.ToString(),
                    });
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "奖池名称",
                        NewValue = newInfo.Name,
                    });
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "奖池类型",
                        NewValue = (bool)newInfo.IsDefault ? "兜底" : "计数池",
                    });
                }
                else
                {
                    if (newInfo.Name != oldInfo?.Name)
                    {
                        operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                        {
                            Property = "奖池名称",
                            NewValue = newInfo.Name,
                            OldValue = oldInfo.Name
                        });
                    }
                    if (newInfo.IsDefault != oldInfo?.IsDefault)
                    {
                        operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                        {
                            Property = "奖池类型",
                            NewValue = (bool)newInfo.IsDefault ? "兜底" : "计数池",
                            OldValue = (bool)oldInfo.IsDefault ? "兜底" : "计数池"
                        });
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
        /// 删除奖池
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public async Task<ActionResult> DeleteForm(int pkid)
        {
            RepositoryManager manager = new RepositoryManager();
            var entity = manager.GetEntity<BigBrandRewardPoolEntity>(_ => _.PKID == pkid && _.Status == true);

            //要删除的旧数据
            var oldList = new List<BigBrandRewardPoolEntity>();
            try
            {
                if (entity.FKPKID > 0)
                {
                    Expression<Func<BigBrandRewardPoolEntity, bool>> chaild = _ => _.ParentPKID == entity.PKID;
                    oldList = manager.GetEntityList<BigBrandRewardPoolEntity>(chaild)?.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"ex:{ex}");
            }

            entity.Status = false;
            manager.Update<BigBrandRewardPoolEntity>(entity);
            // LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "删除大翻牌活动奖池信息", ObjectType = "BigBrand" });

            //读写分离 做延时
            await Task.Delay(2000);

            #region 更新大翻盘缓存
            RefreshBigBrandCache(entity.FKPKID);
            #endregion	

            //操作日志
            DeleteFormLog(entity, oldList);

            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));
        }

        /// <summary>
        /// 删除奖池/礼包操作日志
        /// </summary>
        /// <param name="oldInfo"></param>
        private void DeleteFormLog(BigBrandRewardPoolEntity oldInfo, List<BigBrandRewardPoolEntity> oldList)
        {
            try
            {
                var operationLogType = oldInfo.ParentPKID > 0 ? "BigBrandPool_DeletePackage" : "BigBrandPool_Delete";

                var operationLogModel = new SalePromotionActivityLogModel()
                {
                    ReferId = oldInfo.ParentPKID > 0 ? "BigBrandPool_" + oldInfo.ParentPKID : "BigBrandReward_" + oldInfo.FKPKID,
                    ReferType = "BigBrandReward",
                    OperationLogType = operationLogType,
                    CreateDateTime = DateTime.Now.ToString(),
                    CreateUserName = this.User.Identity.Name,
                    LogDetailList = new List<SalePromotionActivityLogDetail>()
                };
                operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                {
                    Property = "奖池ID",
                    OldValue = oldInfo.PKID.ToString(),
                });
                operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                {
                    Property = oldInfo.ParentPKID > 0 ? "礼包名称" : "奖池名称",
                    OldValue = oldInfo.Name,
                });
                if (oldInfo.IsDefault != null)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "奖池类型",
                        OldValue = (bool)oldInfo.IsDefault ? "兜底" : "计数池",
                    });
                }

                if (oldInfo.ParentPKID > 0)
                {
                    //删除礼包
                    var logRewardType = new SalePromotionActivityLogDetail()
                    {
                        Property = "奖励",
                        OldValue = GetRewardTypeLog(oldInfo.RewardType, oldList),
                    };
                    operationLogModel.LogDetailList.Add(logRewardType);
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "奖励数量",
                        OldValue = oldInfo.Number.ToString()
                    });
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "提示类型",
                        OldValue = oldInfo == null ? "" : oldInfo.PromptType == 1 ? "弹窗" : "链接",
                    });
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "提示文案",
                        OldValue = oldInfo.PromptMsg
                    });
                    if (!string.IsNullOrWhiteSpace(oldInfo.PromptImg))
                    {
                        operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                        {
                            Property = "提示图片",
                            OldValue = oldInfo.PromptImg
                        });
                    }
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "按钮提示文案",
                        OldValue = oldInfo.RedirectBtnText
                    });
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "APP链接",
                        OldValue = oldInfo.RedirectAPP
                    });
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "移动站链接",
                        OldValue = oldInfo.RedirectH5
                    });
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "华为轻应用链接",
                        OldValue = oldInfo.RedirectHuaWei
                    });
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "小程序",
                        OldValue = GetWXAPPName(oldInfo.WxAppId)
                    });
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "小程序链接",
                        OldValue = oldInfo.RedirectWXAPP
                    });
                }

                SetOperationLog(operationLogModel, "DeleteFormLog");
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Warning, ex, $"DeleteFormLog异常,ex:{ex}");
            }
        }


        public ActionResult GetFormJson(int? pkid)
        {
            if (pkid.HasValue)
            {
                RepositoryManager manager = new RepositoryManager();
                var entity = manager.GetEntity<BigBrandRewardPoolEntity>(_ => _.PKID == pkid && _.Status == true);
                return Content(JsonConvert.SerializeObject(entity));
            }
            else
                return Content("null");

        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult GetFormPackageJson(int? pkid)
        {
            if (pkid.HasValue)
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandRewardPoolEntity, bool>> self = _ => _.PKID == pkid && _.Status == true;
                var entity = manager.GetEntity<BigBrandRewardPoolEntity>(self);

                Expression<Func<BigBrandRewardPoolEntity, bool>> chaild = _ => _.ParentPKID == entity.PKID && _.Status == true;
                var list = manager.GetEntityList<BigBrandRewardPoolEntity>(chaild);
                return Content(JsonConvert.SerializeObject(new
                {
                    PackageModel = entity,
                    Child = list
                }));
            }
            else
                return Content("null");

        }

        /// <summary>
        /// 保存奖励池礼包
        /// </summary>
        /// <param name="entityString"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ActionResult> SubmitFormPackage(string entityString, string data)
        {
            data = data.Replace("\"&nbsp;\"", "null");
            entityString = entityString.Replace("\"&nbsp;\"", "null");

            IEnumerable<BigBrandRewardPoolEntity> list = JsonConvert.DeserializeObject<IEnumerable<BigBrandRewardPoolEntity>>(data);
            RepositoryManager manager = new RepositoryManager();
            var entity = JsonConvert.DeserializeObject<BigBrandRewardPoolEntity>(entityString);

            //获取操作日志数据
            var isAdd = false;
            var oldInfo = new BigBrandRewardPoolEntity();
            var oldList = new List<BigBrandRewardPoolEntity>();
            try
            {
                Expression<Func<BigBrandRewardPoolEntity, bool>> self = _ => _.PKID == entity.PKID && _.Status == true;
                oldInfo = manager.GetEntity<BigBrandRewardPoolEntity>(self);

                Expression<Func<BigBrandRewardPoolEntity, bool>> chaild = _ => _.ParentPKID == entity.PKID && _.Status == true;
                oldList = manager.GetEntityList<BigBrandRewardPoolEntity>(chaild)?.ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"SubmitFormPackage获取操作日志需要的数据异常，ex:{ex}");
            }

            if (entity.PKID == 0)
            {
                isAdd = true;
                entity.CreateDateTime = DateTime.Now;
                entity.LastUpdateDateTime = DateTime.Now;
                entity.CreateUserName = User.Identity.Name;
                entity.UpdateUserName = User.Identity.Name;
                entity.Status = true;
                manager.Add<BigBrandRewardPoolEntity>(entity);

                using (var db = manager.BeginTrans())
                {
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            item.FKPKID = entity.FKPKID;
                            item.ParentPKID = entity.PKID;
                            item.CreateDateTime = DateTime.Now;
                            item.UpdateUserName = User.Identity.Name;
                            item.LastUpdateDateTime = DateTime.Now;
                            item.CreateUserName = User.Identity.Name;
                            item.Status = true;
                            if (item.CouponGuid != null)
                            {
                                System.Data.DataTable dt = DataAccess.DAO.DALActivity.CouponVlidate(item.CouponGuid.Value.ToString());
                                if (dt == null || dt.Rows.Count == 0)
                                    return Content(JsonConvert.SerializeObject(new
                                    {
                                        state = "error",
                                        message = "配置的优惠券：" + item.CouponGuid.Value + "不存在,请重新配置",
                                        data = ""
                                    }));
                                item.Name = dt.Rows[0]["PromtionName"].ToString();
                            }
                            else if (item.WxRedBagAmount > 0)
                            {
                                item.Name = "微信红包";
                            }
                            else if (item.Integral > 0)
                                item.Name = "积分";
                            db.Insert<BigBrandRewardPoolEntity>(item);
                        }
                        db.Commit();
                    }
                }
                // LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "新增大翻牌礼包信息", ObjectType = "BigBrand" });
                // LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(list), Author = User.Identity.Name, Operation = "新增大翻牌礼包明细", ObjectType = "BigBrand" });

            }
            else
            {
                var before = manager.GetEntity<BigBrandRewardPoolEntity>(_ => _.PKID == entity.PKID && _.Status == true);
                entity.LastUpdateDateTime = DateTime.Now;
                entity.UpdateUserName = User.Identity.Name;
                manager.Update<BigBrandRewardPoolEntity>(entity);

                Expression<Func<BigBrandRewardPoolEntity, bool>> exp = _ => _.ParentPKID == entity.PKID && _.Status == true;
                using (var db = manager.BeginTrans())
                {
                    var contentList = db.FindList<BigBrandRewardPoolEntity>(exp);
                    if (contentList != null)
                    {
                        foreach (var i in contentList)
                        {
                            i.Status = false;
                            db.Update<BigBrandRewardPoolEntity>(i);
                        }
                    }
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            item.FKPKID = entity.FKPKID;
                            item.ParentPKID = entity.PKID;
                            item.CreateDateTime = DateTime.Now;
                            item.UpdateUserName = User.Identity.Name;
                            item.LastUpdateDateTime = DateTime.Now;
                            item.CreateUserName = User.Identity.Name;
                            item.Status = true;
                            if (item.CouponGuid != null)
                            {
                                System.Data.DataTable dt = DataAccess.DAO.DALActivity.CouponVlidate(item.CouponGuid.Value.ToString());
                                if (dt == null || dt.Rows.Count == 0)
                                    return Content(JsonConvert.SerializeObject(new
                                    {
                                        state = "error",
                                        message = "配置的优惠券：" + item.CouponGuid.Value + "不存在,请重新配置",
                                        data = ""
                                    }));
                                item.Name = dt.Rows[0]["PromtionName"].ToString();
                            }
                            else if (item.WxRedBagAmount > 0)
                            {
                                item.Name = "微信红包";
                            }
                            else if (item.Integral > 0)
                                item.Name = "积分";
                            db.Insert<BigBrandRewardPoolEntity>(item);
                        }
                        db.Commit();
                    }
                }

                // LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "更新大翻牌礼包信息", ObjectType = "BigBrand" });
                // LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(list), Author = User.Identity.Name, Operation = "更新大翻牌礼包明细", ObjectType = "BigBrand" });

            }

            //读写分离 做延时
            await Task.Delay(2000);

            #region 更新大翻盘缓存
            RefreshBigBrandCache(entity.FKPKID);
            #endregion

            //操作日志
            SubmitFormPackageLog(oldInfo, entity, oldList, list?.ToList(), isAdd);

            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));
        }


        /// <summary>
        /// 保存奖池礼包
        /// </summary>
        /// <param name="oldInfo"></param>
        /// <param name="newInfo"></param>
        /// <param name="oldList"></param>
        /// <param name="newList"></param>
        /// <param name="isAdd"></param>
        private void SubmitFormPackageLog(BigBrandRewardPoolEntity oldInfo, BigBrandRewardPoolEntity newInfo,
            List<BigBrandRewardPoolEntity> oldList, List<BigBrandRewardPoolEntity> newList, bool isAdd)
        {
            try
            {
                var operationLogModel = new SalePromotionActivityLogModel()
                {
                    ReferId = "BigBrandPool_" + (isAdd ? newInfo.ParentPKID : newInfo.PKID),
                    ReferType = "BigBrandPool",
                    OperationLogType = isAdd ? "BigBrandPool_InsertPackage" : "BigBrandPool_UpdatePackage",
                    CreateDateTime = DateTime.Now.ToString(),
                    CreateUserName = this.User.Identity.Name,
                    LogDetailList = new List<SalePromotionActivityLogDetail>()
                };
                //  if (!isAdd)
                //  {
                if (isAdd)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "礼包奖池ID",
                        NewValue = newInfo.PKID.ToString(),
                    });
                }
                if (newInfo.Name != oldInfo?.Name)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "礼包名称",
                        NewValue = newInfo.Name,
                        OldValue = oldInfo?.Name
                    });
                }
                var logRewardType = new SalePromotionActivityLogDetail()
                {
                    Property = "奖励",
                    NewValue = GetRewardTypeLog(newInfo.RewardType, newList),
                    OldValue = GetRewardTypeLog(oldInfo?.RewardType, oldList),
                };
                if (newInfo.RewardType != oldInfo?.RewardType || logRewardType.OldValue != logRewardType.NewValue)
                {
                    operationLogModel.LogDetailList.Add(logRewardType);
                }
                if (newInfo.Number != oldInfo?.Number)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "奖励数量",
                        NewValue = newInfo.Number.ToString(),
                        OldValue = oldInfo?.Number.ToString()
                    });
                }
                if (newInfo.PromptType != oldInfo?.PromptType)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "提示类型",
                        NewValue = newInfo.PromptType == 1 ? "弹窗" : "链接",
                        OldValue = oldInfo == null ? "" : oldInfo?.PromptType == 1 ? "弹窗" : "链接",
                    });
                }
                if (newInfo.PromptMsg != oldInfo?.PromptMsg)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "提示文案",
                        NewValue = newInfo.PromptMsg,
                        OldValue = oldInfo?.PromptMsg
                    });
                }
                if (newInfo.PromptImg != oldInfo?.PromptImg)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "提示图片",
                        NewValue = newInfo.PromptImg,
                        OldValue = oldInfo?.PromptImg
                    });
                }
                if (newInfo.RedirectBtnText != oldInfo?.RedirectBtnText)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "按钮提示文案",
                        NewValue = newInfo.RedirectBtnText,
                        OldValue = oldInfo?.RedirectBtnText
                    });
                }
                if (newInfo.RedirectAPP != oldInfo?.RedirectAPP)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "APP链接",
                        NewValue = newInfo.RedirectAPP,
                        OldValue = oldInfo?.RedirectAPP
                    });
                }
                if (newInfo.RedirectH5 != oldInfo?.RedirectH5)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "移动站链接",
                        NewValue = newInfo.RedirectH5,
                        OldValue = oldInfo?.RedirectH5
                    });
                }
                if (newInfo.RedirectHuaWei != oldInfo?.RedirectHuaWei)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "华为轻应用链接",
                        NewValue = newInfo.RedirectHuaWei,
                        OldValue = oldInfo?.RedirectHuaWei
                    });
                }
                if (newInfo.WxAppId != oldInfo?.WxAppId)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "小程序",
                        NewValue = GetWXAPPName(newInfo.WxAppId),
                        OldValue = GetWXAPPName(oldInfo?.WxAppId)
                    });
                }
                if (newInfo.RedirectWXAPP != oldInfo?.RedirectWXAPP)
                {
                    operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                    {
                        Property = "小程序链接",
                        NewValue = newInfo.RedirectWXAPP,
                        OldValue = oldInfo?.RedirectWXAPP
                    });
                }

                SetOperationLog(operationLogModel, "SubmitFormPackageLog");
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, $"SubmitFormPackageLog异常,ex:{ex}");
            }
        }

        /// <summary>
        /// 获取微信小程序名称
        /// </summary>
        /// <param name="vxappId"></param>
        /// <returns></returns>
        private string GetWXAPPName(string vxappId)
        {
            try
            {
                var wxappList = Task.Run(() => Business.Push.PushManagerNew.SelectAllWxAppConfigAsync()).ConfigureAwait(false).GetAwaiter().GetResult();
                var result = wxappList?.FirstOrDefault(t => t.appId == vxappId)?.name;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"GetWXAPPName,ex:{ex}");
                return "";
            }
        }

        /// <summary>
        /// 获取奖池礼包奖励类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetRewardTypeLog(int? value, List<BigBrandRewardPoolEntity> rewardList)
        {
            if (!(rewardList?.Count > 0))
            {
                return "";
            }

            string result = "优惠类型:";

            try
            {
                switch (value)
                {
                    case 1:
                        result += "优惠券";
                        if (rewardList.Any())
                        {
                            result += "。优惠券GUID:";
                        }
                        foreach (var item in rewardList)
                        {
                            result += $"{item.CouponGuid},";
                        }
                        break;
                    case 2:
                        result += "积 分";
                        if (rewardList.Any())
                        {
                            result += "。积分额度:";
                            result += $"{rewardList.FirstOrDefault().Integral},";
                        }
                        break;
                    case 3:
                        result += "无奖励";
                        break;
                    case 4:
                        result += "实物奖励";
                        if (rewardList.Any())
                        {
                            result += "。实物奖励名称:";
                        }
                        foreach (var item in rewardList)
                        {
                            result += $"{item.RealProductName},";
                        }
                        break;
                    case 5:
                        result += "微信红包";
                        if (rewardList.Any())
                        {
                            result += "。红包金额:";
                            result += $"{ Double.Parse(rewardList.FirstOrDefault().WxRedBagAmount.ToString()).ToString("0.00")},";
                        }
                        break;
                }
                if (result.EndsWith(",") || result.EndsWith("，"))
                {
                    result = result.Substring(0, result.Length - 1);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"GetRewardType,ex:{ex}");
            }

            return result;
        }

        public ActionResult FormPackage()
        {
            return View();
        }


        public ActionResult PageStyle()
        {
            int.TryParse(Request.QueryString["keyValue"], out int keyValue);
            BigBrandPageStyleEntity entity = null;
            if (keyValue > 0)
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.FKPKID == keyValue && _.Is_Deleted == false;
                entity = manager.GetEntity<BigBrandPageStyleEntity>(exp);
            }

            ViewBag.IsShare = entity?.IsShare ?? true;
            ViewBag.PromptStyle = entity?.PromptStyle ?? 0;
            return View();
        }

        /// <summary>
        /// 获取大翻牌页面配置
        /// </summary>
        /// <param name="FPKID"></param>
        /// <returns></returns>
        public ActionResult GetBigBrandPageConfig(int FPKID)
        {
            var status = 1;
            var model = new BigBrandPageConfigBrandEntity();
            try
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandPageConfigBrandEntity, bool>> exp = _ => _.FPKID == FPKID && _.IsDelete == false;
                model = manager.GetEntity<BigBrandPageConfigBrandEntity>(exp);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Warning, $"GetBigBrandPageConfig，FPKID：{FPKID}，ex:{ex}");
                status = 0;
            }
            return Json(new { Status = status, Data = model });
        }

        public ActionResult GetGridPageStyleJson(int keyValue, Guid? groupGuid, int PageType)
        {
            List<BigBrandPageStyleEntity> list = null;
            if (groupGuid.HasValue)
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.FKPKID == keyValue && _.GroupGuid == groupGuid && _.PageType == PageType && _.Is_Deleted == false;
                list = manager.GetEntityList<BigBrandPageStyleEntity>(exp).ToList();
            }
            else
            {
                Guid group = Guid.NewGuid();
                list = new List<BigBrandPageStyleEntity>() {
                    new BigBrandPageStyleEntity() { PKID=1, Position=1, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=2, Position=2,  GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=3, Position=3,  GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=4, Position=4, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=5, Position=5, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=6, Position=6, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=7, Position=7, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=8, Position=8, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name }


                };
            }
            return Content(JsonConvert.SerializeObject(list));
        }

        public ActionResult GetGridPageStyleJson2(int keyValue, Guid? groupGuid, int PageType)
        {
            List<BigBrandPageStyleEntity> list = null;
            if (groupGuid.HasValue)
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.FKPKID == keyValue && _.GroupGuid == groupGuid && _.PageType == PageType && _.Is_Deleted == false;
                list = manager.GetEntityList<BigBrandPageStyleEntity>(exp).ToList();
            }
            else
            {
                Guid group = Guid.NewGuid();
                list = new List<BigBrandPageStyleEntity>() {
                    new BigBrandPageStyleEntity() { PKID=1, Position=1, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=2, Position=2,  GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=3, Position=3,  GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=4, Position=4, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=5, Position=5, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=6, Position=6, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=7, Position=7, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=8, Position=8, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=9, Position=9, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name },
                    new BigBrandPageStyleEntity() { PKID=10, Position=10, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name }


                };
            }
            return Content(JsonConvert.SerializeObject(list));
        }

        public ActionResult GetGridPageStyleGroup(int keyValue)
        {
            Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.FKPKID == keyValue && _.Is_Deleted == false;
            RepositoryManager manager = new RepositoryManager();
            var list = manager.GetEntityList<BigBrandPageStyleEntity>(exp)?.ToList();

            Expression<Func<BigBrandAnsQuesEntity, bool>> exps = _ => _.BigBrandPKID == keyValue && _.Is_Deleted == false;
            var ansModel = manager.GetEntity<BigBrandAnsQuesEntity>(exps);
            if (ansModel != null && ansModel.BigBrandPKID != 0)
            {
                list.Add(new BigBrandPageStyleEntity() { GroupGuid = Guid.NewGuid(), PageType = 3 });
            }

            //var pageConfigeModel = manager.GetEntity<BigBrandPageConfigeEntity>(p=>p.FKPKID==keyValue&&p.ActivityType==0);
            //if (pageConfigeModel != null && pageConfigeModel.FKPKID != 0)
            //{
            //    list.Add(new BigBrandPageStyleEntity() { GroupGuid = Guid.NewGuid(), PageType = pageConfigeModel.ActivityType, FKPKID= pageConfigeModel.FKPKID,PKID=pageConfigeModel.PKID});
            //}

            var group = list.GroupBy(_ => _.GroupGuid);

            return Content(JsonConvert.SerializeObject(group?.Select(_ => new
            {
                Key = _.Key,
                Type = _?.ToList()?.FirstOrDefault()?.PageType
            })));
        }

        //提交 活动页面 
        public async Task<ActionResult> SubmitPageStyle(string json, BigBrandPageConfigBrandEntity pageConfig, int pageType)
        {
            IEnumerable<BigBrandPageStyleEntity> list = JsonConvert.DeserializeObject<IEnumerable<BigBrandPageStyleEntity>>(json);
            if (list != null)
            {
                RepositoryManager manager = new RepositoryManager();
                var exitGroup = list?.FirstOrDefault().GroupGuid;
                var fkpkid = list?.FirstOrDefault().FKPKID;
                Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.GroupGuid == exitGroup && _.FKPKID == fkpkid && _.Is_Deleted == false;
                var data = manager.GetEntityList<BigBrandPageStyleEntity>(exp);
                if (data != null && data.Count() > 0)
                {
                    data?.ToList().ForEach(_ =>
                    {
                        _.LastUpdateDateTime = DateTime.Now;
                        _.UpdateUserName = User.Identity.Name;
                        var item = list.Where(o => o.Position == _.Position)?.FirstOrDefault();
                        _.ImageUri = item.ImageUri;
                        _.BImageUri = item.BImageUri;
                        _.PromptStyle = item.PromptStyle;
                        _.IsShare = item.IsShare;
                    });
                    using (var db = manager.BeginTrans())
                    {
                        foreach (var item in data)
                            db.Update<BigBrandPageStyleEntity>(item);

                        if (pageType == 2)
                        {
                            //大翻牌保存图片地址
                            var insertPageConfig = InsertOrUpdatePageConfig(pageConfig, db);
                            if (!insertPageConfig)
                            {
                                return Content(JsonConvert.SerializeObject(new
                                {
                                    state = "error",
                                    message = "操作失败",
                                    data = "保存图片地址失败"
                                }));
                            }
                        }

                        db.Commit();
                    }
                }
                else
                {
                    using (var db = manager.BeginTrans())
                    {
                        list?.ToList().ForEach(_ =>
                        {
                            _.CreateDateTime = DateTime.Now;
                            _.LastUpdateDateTime = DateTime.Now;
                            _.CreateUserName = User.Identity.Name;
                            _.UpdateUserName = User.Identity.Name;
                        });

                        foreach (var item in list)
                            db.Insert<BigBrandPageStyleEntity>(item);

                        if (pageType == 2)
                        {
                            //大翻牌保存图片地址
                            var insertPageConfig = InsertOrUpdatePageConfig(pageConfig, db);
                            if (!insertPageConfig)
                            {
                                return Content(JsonConvert.SerializeObject(new
                                {
                                    state = "error",
                                    message = "操作失败",
                                    data = "保存图片地址失败"
                                }));
                            }
                        }

                        db.Commit();
                    }
                }

                //读写分离 做延时
                await Task.Delay(2000);

                #region 更新大翻盘缓存
                RefreshBigBrandCache(fkpkid);
                #endregion

                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(list), Author = User.Identity.Name, Operation = "大翻牌页面配置", ObjectType = "BigBrand" });
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
            else
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "操作失败",
                    data = "参数没有实例化"
                }));
            }
        }

        /// <summary>
        /// 保存大翻牌图片
        /// </summary>
        /// <param name="pageConfig"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private bool InsertOrUpdatePageConfig(BigBrandPageConfigBrandEntity pageConfig, RepositoryBase db)
        {
            RepositoryManager manager = new RepositoryManager();
            var pageConfigModel = new BigBrandPageConfigBrandEntity();
            try
            {
                Expression<Func<BigBrandPageConfigBrandEntity, bool>> expPage = _ => _.FPKID == pageConfig.FPKID && _.IsDelete == false;
                pageConfigModel = manager.GetEntity(expPage);

                if (pageConfigModel != null)
                {
                    //修改
                    pageConfigModel.FPKID = pageConfig.FPKID;
                    pageConfigModel.BigBrandBackImgUrl = pageConfig.BigBrandBackImgUrl;
                    pageConfigModel.BroadCastInnerImgUrl = pageConfig.BroadCastInnerImgUrl;
                    pageConfigModel.BroadCastOutImgUrl = pageConfig.BroadCastOutImgUrl;
                    pageConfigModel.BigBrandCenterBtnImgUrl = pageConfig.BigBrandCenterBtnImgUrl;
                    pageConfigModel.UpdateTime = DateTime.Now;
                    pageConfigModel.UpdateUser = this.User.Identity.Name;
                    db.Update<BigBrandPageConfigBrandEntity>(pageConfigModel);
                }
                else
                {
                    pageConfigModel = pageConfig;
                    pageConfigModel.CreateTime = DateTime.Now;
                    pageConfigModel.CreateUser = this.User.Identity.Name;
                    db.Insert<BigBrandPageConfigBrandEntity>(pageConfigModel);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"InsertOrUpdatePageConfig，FPKID：{pageConfig.FPKID}", ex);
                return false;
            }

            return true;
        }

        public async Task<ActionResult> SelectWxApp()
        {
            var result = await Business.Push.PushManagerNew.SelectAllWxAppConfigAsync();
            return Content(JsonConvert.SerializeObject(result));

        }

        public async Task<ActionResult> DeletePageStyle(int keyValue, int PageType)
        {
            if (PageType != 3)
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.FKPKID == keyValue && _.PageType == PageType && _.Is_Deleted == false;
                var pageStyleList = manager.GetEntityList<BigBrandPageStyleEntity>(exp);
                foreach (var item in pageStyleList)
                {
                    item.Is_Deleted = true;
                    item.LastUpdateDateTime = DateTime.Now;
                    item.UpdateUserName = User.Identity.Name;
                    manager.Update<BigBrandPageStyleEntity>(item);
                }

                if (PageType == 0)
                {
                    var pageConfigList = manager.GetEntityList<BigBrandPageConfigeEntity>(_ => _.FKPKID == keyValue && _.ActivityType == PageType && _.Is_Deleted == false);
                    foreach (var item in pageConfigList)
                    {
                        item.Is_Deleted = true;
                        item.LastUpdateDateTime = DateTime.Now;
                        manager.Update<BigBrandPageConfigeEntity>(item);
                    }
                }

                if (PageType == 2)
                {
                    //删除大翻盘页面配置(4张图)
                    Expression<Func<BigBrandPageConfigBrandEntity, bool>> expPage = _ => _.FPKID == keyValue && _.IsDelete == false;
                    var brandImgsModel = manager.GetEntity(expPage);
                    if (brandImgsModel != null)
                    {
                        brandImgsModel.IsDelete = true;
                        manager.Update(brandImgsModel);
                    }
                }

                //读写分离 做延时
                await Task.Delay(2000);

                #region 更新大翻盘缓存
                RefreshBigBrandCache(keyValue);
                #endregion

                return Content("1");
            }
            else
            {
                RepositoryManager manager = new RepositoryManager();
                var ansQuesList = manager.GetEntityList<BigBrandAnsQuesEntity>(p => p.BigBrandPKID == keyValue && p.Is_Deleted == false);
                foreach (var item in ansQuesList)
                {
                    item.Is_Deleted = true;
                    item.LastUpdateDateTime = DateTime.Now;
                    manager.Update<BigBrandAnsQuesEntity>(item);
                }

                //读写分离 做延时
                await Task.Delay(2000);

                #region 更新大翻盘缓存
                RefreshBigBrandCache(keyValue);
                #endregion

                return Content("1");
            }


        }

        public ActionResult AnswerQuestionsForm()
        {
            return View();
        }

        public async Task<ActionResult> SubmitAnswer(BigBrandAnsQuesEntity model)
        {
            if (model.BigBrandPKID == 0)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "抽奖PKID不能为空",
                    data = "参数没有实例化"
                }));

            RepositoryManager manager = new RepositoryManager();
            using (var db = manager.BeginTrans())
            {
                Expression<Func<BigBrandAnsQuesEntity, bool>> exp = _ => _.BigBrandPKID == model.BigBrandPKID && _.Is_Deleted == false;
                var bigBrandAnsQuesList = manager.GetEntityList<BigBrandAnsQuesEntity>(exp);
                foreach (var item in bigBrandAnsQuesList)
                {
                    item.Is_Deleted = true;
                    item.LastUpdateDateTime = DateTime.Now;
                    manager.Update<BigBrandAnsQuesEntity>(item);
                }

                model.CreateDateTime = DateTime.Now;
                model.LastUpdateDateTime = DateTime.Now;
                db.Insert<BigBrandAnsQuesEntity>(model);
                db.Commit();

                //读写分离 做延时
                await Task.Delay(2000);

                #region 更新大翻盘缓存
                RefreshBigBrandCache(model.BigBrandPKID);
                #endregion

                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, Operation = "抽奖问答配置", ObjectType = "BigBrand" });
            }
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));

        }

        public ActionResult GetAnswerEntity(int bigBrandPkid)
        {
            Expression<Func<BigBrandAnsQuesEntity, bool>> exp = _ => _.BigBrandPKID == bigBrandPkid && _.Is_Deleted == false;
            RepositoryManager manager = new RepositoryManager();
            var model = manager.GetEntity<BigBrandAnsQuesEntity>(exp);
            if (model != null)
                return Content(JsonConvert.SerializeObject(model));
            else
                return Content(JsonConvert.SerializeObject(new BigBrandAnsQuesEntity()));
        }

        public ActionResult GetAnswerInfoList()
        {
            RepositoryManager manager = new RepositoryManager();
            var list = manager.GetEntityList<AnswerInfoListEntity>("SELECT DISTINCT Tip, 0 AS PKID, GETDATE()  CreateDateTime , GETDATE()  LastUpdateDateTime, NULL OptionsA , NULL Answer , NULL OptionsB, NULL  OptionsC, NULL OptionsD, NULL OptionsReal,  IsEnabled,  Is_Deleted FROM Configuration.dbo.AnswerInfoList WITH (NOLOCK)  WHERE IsEnabled=1 AND Is_Deleted=0");
            return Content(JsonConvert.SerializeObject(list));
        }

        #region 摇奖机相关

        /// <summary>
        /// 获取摇奖机的配置
        /// </summary>
        /// <param name="FKPkid"></param>
        /// <returns></returns>
        public ActionResult GetBigBrandPageConfigeEntity(int FKPkid, int ActivityType)
        {
            Expression<Func<BigBrandPageConfigeEntity, bool>> exp = _ => _.FKPKID == FKPkid && _.ActivityType == ActivityType && _.Is_Deleted == false;
            RepositoryManager manager = new RepositoryManager();
            var model = manager.GetEntity<BigBrandPageConfigeEntity>(exp);
            if (model != null)
                return Content(JsonConvert.SerializeObject(model));
            else
                return Content(JsonConvert.SerializeObject(new BigBrandPageConfigeEntity()));
        }

        /// <summary>
        /// 保存 摇奖机的 配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> SubmitPageConfige(BigBrandPageConfigeEntity model, List<BigBrandPageStyleEntity> pageStyles)
        {
            model.DrawMachineImgUri = model.DrawMachineImgUri ?? "";
            model.HomeBgImgUri = model.HomeBgImgUri ?? "";
            model.HomeBgImgUri2 = model.HomeBgImgUri2 ?? "";
            model.ResultImgUri = model.ResultImgUri ?? "";
            if (model.FKPKID == 0)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "抽奖PKID不能为空",
                    data = "参数没有实例化"
                }));

            RepositoryManager manager = new RepositoryManager();
            using (var db = manager.BeginTrans())
            {
                Expression<Func<BigBrandPageConfigeEntity, bool>> exp = _ => _.FKPKID == model.FKPKID && _.ActivityType == model.ActivityType && _.Is_Deleted == false;
                var bigBrandPageConfigList = manager.GetEntityList<BigBrandPageConfigeEntity>(exp);
                foreach (var item in bigBrandPageConfigList)
                {
                    item.Is_Deleted = true;
                    item.LastUpdateDateTime = DateTime.Now;
                    manager.Update<BigBrandPageConfigeEntity>(item);
                }

                model.CreateDateTime = DateTime.Now;
                model.LastUpdateDateTime = DateTime.Now;
                db.Insert<BigBrandPageConfigeEntity>(model);
                foreach (var item in pageStyles)
                {
                    item.ImageUri = item.ImageUri ?? "";
                    if (item.PKID == 0)
                    {
                        item.CreateDateTime = DateTime.Now;
                        item.LastUpdateDateTime = DateTime.Now;
                        db.Insert(item);
                    }
                    else
                    {
                        var original = db.FindEntity<BigBrandPageStyleEntity>(_ => _.PKID == item.PKID && _.Is_Deleted == false);

                        if (item.ImageUri != original.ImageUri)
                        {
                            original.ImageUri = item.ImageUri;
                            original.LastUpdateDateTime = DateTime.Now;
                            db.Update(original);
                        }
                    }
                }
                db.Commit();

                //读写分离 做延时
                await Task.Delay(2000);

                #region 更新大翻盘缓存
                RefreshBigBrandCache(model.FKPKID);
                #endregion

                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, Operation = "活动页面 配置", ObjectType = "BigBrandPageConfig" });
            }
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));

        }

        /// <summary>
        /// 删除单个 活动页面的 配置的 图片
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public async Task<ActionResult> DeletePageConfigePageStyle(int PKID)
        {
            RepositoryManager manager = new RepositoryManager();
            Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.PKID == PKID && _.Is_Deleted == false;
            var bigBrandPageStyleEntity = manager.GetEntity<BigBrandPageStyleEntity>(exp);
            var fkpkid = bigBrandPageStyleEntity.FKPKID;
            bigBrandPageStyleEntity.Is_Deleted = true;
            bigBrandPageStyleEntity.LastUpdateDateTime = DateTime.Now;
            bigBrandPageStyleEntity.UpdateUserName = User.Identity.Name;
            manager.Update<BigBrandPageStyleEntity>(bigBrandPageStyleEntity);

            //读写分离 做延时
            await Task.Delay(2000);

            #region 更新大翻盘缓存
            RefreshBigBrandCache(fkpkid);
            #endregion

            return Content("1");
        }

        /// <summary>
        /// 根据 类型   和  活动的  pkid   获取 活动配置页 的 图片
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="ActivityType"></param>
        /// <returns></returns>
        public ActionResult GetPageStyleList(int FKPkid, int ActivityType)
        {
            Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.FKPKID == FKPkid && _.PageType == ActivityType && _.Is_Deleted == false;
            RepositoryManager manager = new RepositoryManager();
            var list = manager.GetEntityList<BigBrandPageStyleEntity>(exp)?.ToList();
            if (list == null || !list.Any())//未查询到数据
            {
                Guid group = Guid.NewGuid();
                list = new List<BigBrandPageStyleEntity>() {
                    new BigBrandPageStyleEntity() { PKID=0, Position=1, GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name,PageType=ActivityType },
                    new BigBrandPageStyleEntity() { PKID=0, Position=2,  GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name,PageType=ActivityType },
                    new BigBrandPageStyleEntity() { PKID=0, Position=3,  GroupGuid=group, CreateDateTime=DateTime.Now,UpdateUserName=User.Identity.Name, LastUpdateDateTime=DateTime.Now, CreateUserName=User.Identity.Name,PageType=ActivityType }
                };
            }
            return Content(JsonConvert.SerializeObject(list));
        }

        #endregion


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
                Logger.Log(Level.Error, $"{funNameString},操作日志记录异常", ex);
            }
        }

        /// <summary>
        /// 获取活动的操作日志列表
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult GetOperationLogList(string activityid, int pageIndex = 1, int pageSize = 20)
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
            return View(result.Source);
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
            }
            return Json(result);
        }

        /// <summary>
        /// 更新大翻盘缓存
        /// </summary>
        /// <param name="pkid"></param>
        public void RefreshBigBrandCache(int? pkid)
        {
            try
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandRewardListEntity, bool>> exp = _ => _.PKID == pkid.Value;
                var entity = manager.GetEntity<BigBrandRewardListEntity>(exp);
                using (var client = new Tuhu.Service.Activity.BigBrandClient())
                {
                    var result = client.UpdateBigBrand(entity.HashKeyValue);
                    if (!result.Success)
                    {
                        Logger.Log(Level.Warning, $"更新大翻盘缓存失败 ->{entity.HashKeyValue}", result.ErrorMessage);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(Level.Error, $"BigBrandPoolController -> RefreshBigBrandCache -> {pkid},异常信息：{ex.Message}，堆栈异常：{ex.StackTrace}");
            }
        }
    }
}