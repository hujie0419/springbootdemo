using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Mapping;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.Logger;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Areas.BigBrand.Controllers
{
    public class BigBrandRewardController : Controller
    {
        // GET: BigBrand/BigBrandReward

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
        /// 提交大翻盘的配置
        /// </summary>
        /// <param name="modeljson"></param>
        /// <param name="wheeljson"></param>
        /// <returns></returns>
        public async Task<ActionResult> SubmitForm(string modeljson,string wheeljson)
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
                using (var db = manager.BeginTrans())
                {
                    if (entity.PKID == 0)
                    {
                        entity.HashKeyValue = Tuhu.Provisioning.Common.SecurityHelper.Sha1Encrypt(Guid.NewGuid().ToString(), System.Text.Encoding.UTF8).Substring(0, 8);
                        entity.CreateDateTime = DateTime.Now;
                        entity.LastUpdateDateTime = DateTime.Now;
                        entity.UpdateUserName = User.Identity.Name;
                        entity.CreateUserName = User.Identity.Name;
                        manager.Add<BigBrandRewardListEntity>(entity);
                        LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "新增大翻牌活动", ObjectType = "BigBrand" });
                    }
                    else
                    {
                        entity.LastUpdateDateTime = DateTime.Now;
                        entity.UpdateUserName = User.Identity.Name;
                        manager.Update<BigBrandRewardListEntity>(entity);
                        LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "更新大翻牌活动", ObjectType = "BigBrand" });
                    }

                    Expression<Func<BigBrandWheelEntity, bool>> exp = _ => _.FKBigBrand == entity.PKID;
                    db.Delete<BigBrandWheelEntity>(exp);
                   
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
                            if(array != null && array.Length>0)
                            {
                                foreach (var s in array)
                                {
                                    int poolPKID = 0;
                                    if (int.TryParse(s, out poolPKID))
                                    {
                                        if (poolPKID != 0)
                                        {
                                            item.FKPoolPKID = poolPKID;
                                            string json = JsonConvert.SerializeObject(item);
                                            var tempModel = JsonConvert.DeserializeObject<BigBrandWheelEntity>(json);
                                            db.Insert<BigBrandWheelEntity>(tempModel);
                                        }
                                    }

                                }
                            }

                            
                        }
                    }
                    db.Commit();
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(wheel), Author = User.Identity.Name, Operation = "大翻牌明细", ObjectType = "BigBrand" });
                }


                entity = manager.GetEntity<BigBrandRewardListEntity>(p => p.PKID == entity.PKID);
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
        /// 获取大翻牌抽奖配置
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult GetGridJson(Pagination pagination)
        {
            RepositoryManager manager = new RepositoryManager();
            var list = manager.GetEntityList<BigBrandRewardListEntity>(pagination);
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
            Expression<Func<BigBrandRewardListEntity, bool>> exp = _ => _.PKID == pkid.Value;
            var entity = manager.GetEntity<BigBrandRewardListEntity>(exp);

            Expression<Func<BigBrandWheelEntity, bool>> wheelExp = _ => _.FKBigBrand == pkid.Value;

            var wheelList = manager.GetEntityList<BigBrandWheelEntity>(wheelExp);

            return Content(JsonConvert.SerializeObject(new {
                FormData=entity,
                WheelData=wheelList
            }));
        }


        public ActionResult DeleteForm(int? pkid)
        {
            if (pkid.HasValue)
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandPageStyleEntity, bool>> exp1 = _ => _.FKPKID == pkid.Value;
                manager.Delete<BigBrandPageStyleEntity>(exp1);
                
                Expression<Func<BigBrandRewardPoolEntity, bool>> exp2 = _ => _.FKPKID == pkid.Value;
                 manager.Delete<BigBrandRewardPoolEntity>(exp2);
                
                Expression<Func<BigBrandRewardListEntity, bool>> exp = _ => _.PKID == pkid.Value;
                var entity = manager.GetEntity<BigBrandRewardListEntity>(exp);
                manager.Delete<BigBrandRewardListEntity>(exp);
                LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "删除大翻牌活动", ObjectType = "BigBrand" });
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
        /// 刷新大翻盘的缓存
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult Reload(int? pkid)
        {
            RepositoryManager manager = new RepositoryManager();
            Expression<Func<BigBrandRewardListEntity, bool>> exp = _ => _.PKID == pkid.Value;
            var entity = manager.GetEntity<BigBrandRewardListEntity>(exp);

            using (var client = new Tuhu.Service.Activity.BigBrandClient())
            {
                var result = client.UpdateBigBrand(entity.HashKeyValue);
                result.ThrowIfException(true);
                if (result.Result)
                    return Content(JsonConvert.SerializeObject(new {
                        Code =1,
                        Msg="刷新成功"
                    }));
                else
                    return Content(JsonConvert.SerializeObject(new {
                        Code=0,
                        Msg="刷新失败"
                    }));
            }
        }

    }
}