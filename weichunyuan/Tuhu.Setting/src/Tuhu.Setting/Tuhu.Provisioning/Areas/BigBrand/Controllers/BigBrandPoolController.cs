using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Mapping;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.Business;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Areas.BigBrand.Controllers
{
    public class BigBrandPoolController : Controller
    {
        // GET: BigBrand/BigBrandPool
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


        public ActionResult GetGridJson(int? fkpkid)
        {
            if (fkpkid.HasValue)
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandRewardPoolEntity, bool>> exp = _ => _.FKPKID == fkpkid.Value && _.Status==true && _.RewardType != -1;
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


        public ActionResult SubmitForm(BigBrandRewardPoolEntity entity)
        {
            RepositoryManager manager = new RepositoryManager();
            if (entity.PKID == 0)
            {
                entity.CreateDateTime = DateTime.Now;
                entity.LastUpdateDateTime = DateTime.Now;
                entity.CreateUserName = User.Identity.Name;
                entity.UpdateUserName = User.Identity.Name;
                entity.RewardType = 0;
                entity.Status = true;
                manager.Add<BigBrandRewardPoolEntity>(entity);
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "新增大翻牌活动奖池信息", ObjectType = "BigBrand" });
            }
            else
            {
                var before = manager.GetEntity<BigBrandRewardPoolEntity>(entity.PKID);
                entity.LastUpdateDateTime = DateTime.Now;
                entity.UpdateUserName = User.Identity.Name;
                entity.Status = true;
                manager.Update<BigBrandRewardPoolEntity>(entity);
                LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue= JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "更新大翻牌活动奖池信息", ObjectType = "BigBrand" });

            }
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));
        }

        public ActionResult DeleteForm(int pkid)
        {
            RepositoryManager manager = new RepositoryManager();
            var entity = manager.GetEntity<BigBrandRewardPoolEntity>(pkid);
            entity.Status = false;
            manager.Update<BigBrandRewardPoolEntity>(entity);
            LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "删除大翻牌活动奖池信息", ObjectType = "BigBrand" });
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));
        }


        public ActionResult GetFormJson(int? pkid)
        {
            if (pkid.HasValue)
            {
                RepositoryManager manager = new RepositoryManager();
                var entity = manager.GetEntity<BigBrandRewardPoolEntity>(pkid);
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
                Expression<Func<BigBrandRewardPoolEntity, bool>> self = _ => _.PKID == pkid;
                var entity = manager.GetEntity<BigBrandRewardPoolEntity>(self);

                Expression<Func<BigBrandRewardPoolEntity, bool>> chaild = _ => _.ParentPKID == entity.PKID && _.Status == true; ;
                var list = manager.GetEntityList<BigBrandRewardPoolEntity>(chaild);
                return Content(JsonConvert.SerializeObject(new {
                    PackageModel = entity,
                    Child=list
                }));
            }
            else
                return Content("null");

        }


        public ActionResult SubmitFormPackage(string entityString, string data)
        {
            data = data.Replace("\"&nbsp;\"", "null");
            entityString = entityString.Replace("\"&nbsp;\"", "null");

            IEnumerable<BigBrandRewardPoolEntity> list = JsonConvert.DeserializeObject<IEnumerable<BigBrandRewardPoolEntity>>(data);
            RepositoryManager manager = new RepositoryManager();
            var entity = JsonConvert.DeserializeObject<BigBrandRewardPoolEntity>(entityString);
            if (entity.PKID == 0)
            {
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
                                if(dt == null || dt.Rows.Count == 0)
                                  return  Content(JsonConvert.SerializeObject(new
                                    {
                                        state = "error",
                                        message = "配置的优惠券："+item.CouponGuid.Value+"不存在,请重新配置",
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
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "新增大翻牌礼包信息", ObjectType = "BigBrand" });
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(list), Author = User.Identity.Name, Operation = "新增大翻牌礼包明细", ObjectType = "BigBrand" });

            }
            else
            {
                var before = manager.GetEntity<BigBrandRewardPoolEntity>(entity.PKID);
                entity.LastUpdateDateTime = DateTime.Now;
                entity.UpdateUserName = User.Identity.Name;
                manager.Update<BigBrandRewardPoolEntity>(entity);

                Expression<Func<BigBrandRewardPoolEntity, bool>> exp = _ => _.ParentPKID == entity.PKID;
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
                            }else if (item.WxRedBagAmount > 0)
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

                LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue=JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "更新大翻牌礼包信息", ObjectType = "BigBrand" });
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(list), Author = User.Identity.Name, Operation = "更新大翻牌礼包明细", ObjectType = "BigBrand" });

            }
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));
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
                Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.FKPKID == keyValue;
                entity = manager.GetEntity<BigBrandPageStyleEntity>(exp);
            }
           
            ViewBag.IsShare = entity?.IsShare ?? true;
            ViewBag.PromptStyle = entity?.PromptStyle ?? 0;
            return View();
        }


        public ActionResult GetGridPageStyleJson(int keyValue, Guid? groupGuid,int PageType)
        {
            List<BigBrandPageStyleEntity> list = null;
            if (groupGuid.HasValue)
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.FKPKID == keyValue && _.GroupGuid == groupGuid && _.PageType == PageType;
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

        public ActionResult GetGridPageStyleJson2(int keyValue, Guid? groupGuid,int PageType)
        {
            List<BigBrandPageStyleEntity> list = null;
            if (groupGuid.HasValue)
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.FKPKID == keyValue && _.GroupGuid == groupGuid && _.PageType == PageType;
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
            Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.FKPKID == keyValue;
            RepositoryManager manager = new RepositoryManager();
            var list = manager.GetEntityList<BigBrandPageStyleEntity>(exp)?.ToList();

            Expression<Func<BigBrandAnsQuesEntity, bool>> exps = _ => _.BigBrandPKID == keyValue;
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
           
            return Content(JsonConvert.SerializeObject(group?.Select(_=> new {
                Key = _.Key,
                Type = _?.ToList()?.FirstOrDefault()?.PageType
            })));
        }

        //提交 活动页面 
        public ActionResult SubmitPageStyle(string json)
        {
            IEnumerable<BigBrandPageStyleEntity> list = JsonConvert.DeserializeObject<IEnumerable<BigBrandPageStyleEntity>>(json);
            if (list != null)
            {
                RepositoryManager manager = new RepositoryManager();
                var exitGroup = list?.FirstOrDefault().GroupGuid;
                var fkpkid = list?.FirstOrDefault().FKPKID;
                Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.GroupGuid == exitGroup && _.FKPKID == fkpkid;
                var data = manager.GetEntityList<BigBrandPageStyleEntity>(exp);
                if (data != null && data.Count()>0)
                {
                    data.ForEach(_ =>
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

                        db.Commit();
                    }
                }
                else
                {
                    using (var db = manager.BeginTrans())
                    {
                        list.ForEach(_ =>
                        {
                            _.CreateDateTime = DateTime.Now;
                            _.LastUpdateDateTime = DateTime.Now;
                            _.CreateUserName = User.Identity.Name;
                            _.UpdateUserName = User.Identity.Name;
                        });

                        foreach (var item in list)
                            db.Insert<BigBrandPageStyleEntity>(item);
                        db.Commit();
                    }
                }

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

        public async Task<ActionResult> SelectWxApp()
        {
            var result = await Business.Push.PushManagerNew.SelectAllWxAppConfigAsync();
            return Content(JsonConvert.SerializeObject(result));

        }

        public ActionResult DeletePageStyle(int keyValue,int PageType)
        {
            if (PageType!=3)
            {
                RepositoryManager manager = new RepositoryManager();
                Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.FKPKID == keyValue && _.PageType==PageType;
                manager.Delete<BigBrandPageStyleEntity>(exp);
                if (PageType==0)
                {
                    manager.Delete<BigBrandPageConfigeEntity>(_ => _.FKPKID == keyValue && _.ActivityType == PageType);
                }
                return Content("1");
            }
            else
            {
                RepositoryManager manager = new RepositoryManager();
                manager.Delete<BigBrandAnsQuesEntity>(p=>p.BigBrandPKID== keyValue);
                return Content("1");
            }
           
           
        }

        public ActionResult AnswerQuestionsForm()
        {
            return View();
        }

        public ActionResult SubmitAnswer(BigBrandAnsQuesEntity model)
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
                Expression<Func<BigBrandAnsQuesEntity, bool>> exp = _ => _.BigBrandPKID == model.BigBrandPKID;
                db.Delete<BigBrandAnsQuesEntity>(exp);
                model.CreateDateTime = DateTime.Now;
                model.LastUpdateDateTime = DateTime.Now;
                db.Insert<BigBrandAnsQuesEntity>(model);
                db.Commit();
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
            Expression<Func<BigBrandAnsQuesEntity, bool>> exp = _ => _.BigBrandPKID == bigBrandPkid;
            RepositoryManager manager = new RepositoryManager();
            var model =  manager.GetEntity<BigBrandAnsQuesEntity>(exp);
            if (model != null)
                return Content(JsonConvert.SerializeObject(model));
            else
                return Content(JsonConvert.SerializeObject(new BigBrandAnsQuesEntity()));
        }

        public ActionResult GetAnswerInfoList()
        {
            RepositoryManager manager = new RepositoryManager();
            var list =manager.GetEntityList<AnswerInfoListEntity>("SELECT DISTINCT Tip, 0 AS PKID, GETDATE()  CreateDateTime , GETDATE()  LastUpdateDateTime, NULL OptionsA , NULL Answer , NULL OptionsB, NULL  OptionsC, NULL OptionsD, NULL OptionsReal,  IsEnabled FROM Configuration.dbo.AnswerInfoList WITH (NOLOCK)  WHERE IsEnabled=1 ");
            return Content(JsonConvert.SerializeObject(list));
        }

        #region 摇奖机相关

        /// <summary>
        /// 获取摇奖机的配置
        /// </summary>
        /// <param name="FKPkid"></param>
        /// <returns></returns>
        public ActionResult GetBigBrandPageConfigeEntity(int FKPkid,int ActivityType)
        {
            Expression<Func<BigBrandPageConfigeEntity, bool>> exp = _ => _.FKPKID == FKPkid && _.ActivityType == ActivityType;
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
        public ActionResult SubmitPageConfige(BigBrandPageConfigeEntity model,List<BigBrandPageStyleEntity> pageStyles)
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
                Expression<Func<BigBrandPageConfigeEntity, bool>> exp = _ => _.FKPKID == model.FKPKID && _.ActivityType == model.ActivityType;
                db.Delete<BigBrandPageConfigeEntity>(exp);
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
                        var original = db.FindEntity<BigBrandPageStyleEntity>(item.PKID);
                        
                        if (item.ImageUri != original.ImageUri)
                        {
                            original.ImageUri = item.ImageUri;
                            original.LastUpdateDateTime = DateTime.Now;
                            db.Update(original);
                        }                      
                    }
                }
                db.Commit();
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
        public ActionResult DeletePageConfigePageStyle(int PKID)
        {
            RepositoryManager manager = new RepositoryManager();
            Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.PKID == PKID;
            manager.Delete<BigBrandPageStyleEntity>(exp);
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
            Expression<Func<BigBrandPageStyleEntity, bool>> exp = _ => _.FKPKID == FKPkid && _.PageType == ActivityType;
            RepositoryManager manager = new RepositoryManager();
            var list = manager.GetEntityList<BigBrandPageStyleEntity>(exp)?.ToList();
            if (list==null || !list.Any())//未查询到数据
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
    }
}