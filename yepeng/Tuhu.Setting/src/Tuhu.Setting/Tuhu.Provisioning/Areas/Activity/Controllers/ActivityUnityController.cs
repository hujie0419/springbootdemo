using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Mapping;
using Tuhu.Provisioning.Business;
using Newtonsoft.Json;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Linq.Expressions;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.Activity;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Web.Security;

namespace Tuhu.Provisioning.Areas.Activity.Controllers
{
    public  class ActivityUnityController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetGridJson(string keyword, Pagination pagination) {

            RepositoryManager db = new RepositoryManager();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                Expression<Func<ActivePageListEntity, bool>> expression = _ => _.Title.Contains(keyword.Trim()) || _.HashKey.Contains(keyword.Trim());
                var list = db.GetEntityList<ActivePageListEntity>(expression, pagination);
                return Content(JsonConvert.SerializeObject(new {
                    rows =list,
                    total=pagination.total,
                    page = pagination.page,
                    records = pagination.records
                }));
            }
            else
            {
                var list = db.GetEntityList<ActivePageListEntity>(pagination);
                return Content(JsonConvert.SerializeObject(new
                {
                    rows = list,
                    total = pagination.total,
                    page = pagination.page,
                    records = pagination.records
                }));
            }
            
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult ListForm()
        {
            ViewBag.UserName = User.Identity.Name;
            return View();
        }

        public ActionResult GetListFormJson(int keyValue)
        {
            RepositoryManager db = new RepositoryManager();
            var entity = db.GetEntity<ActivePageListEntity>(keyValue);
            return Content(JsonConvert.SerializeObject(entity, new JsonSerializerSettings() {
                DateFormatString = "yyyy-MM-dd HH:mm:ss" }));
        }

        /// <summary>
        /// 配置列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfigList()
        {
            return View();
        }

        public ActionResult GetGridTreeJson(int? keyValue)
        {
            if (keyValue == null)
                return Content("null");

            RepositoryManager repository = new RepositoryManager();
            Expression<Func<ActivePageContentEntity, bool>> expression = o => o.FKActiveID == keyValue;
            var list = repository.GetEntityList<ActivePageContentEntity>(expression);

            return Content(JsonConvert.SerializeObject(list.OrderBy(o=>o.GROUP).ThenBy(o=>o.OrderBy)));
        }


        public ActionResult GetContentForm(string group,int FKActiveID)
        {
            if (string.IsNullOrWhiteSpace(group))
                return Content("null");
            RepositoryManager repository = new RepositoryManager();
            Expression<Func<ActivePageContentEntity, bool>> expression = o => o.GROUP == group && o.FKActiveID == FKActiveID;
            var list = repository.GetEntityList<ActivePageContentEntity>(expression);
            list.ForEach(_ => {
                if (_.DisplayWay == 1)
                {
                    switch (_.Type)
                    {
                        case 6:
                            _.Type = 0;
                            break;
                        case 7:
                            _.Type = 1;
                            break;
                        case 11:
                            _.Type = 10;
                            break;

                    }
                }
            });
            return Content(JsonConvert.SerializeObject(list?.OrderBy(o => o.OrderBy)));
        }

        [ValidateInput(false)]
        public ActionResult SubmitContent(string list, string keyValue ="",bool copy=false)
        {
            list = list.Replace("\"&nbsp;\"", "null");
            IEnumerable<ActivePageContentEntity> entities = JsonConvert.DeserializeObject<IEnumerable<ActivePageContentEntity>>(list);
            try
            {
                if (entities != null && entities.Any())
                {
                    var isreplace = entities.Select(x => x.IsReplace).Distinct();
                    if (isreplace.Count() >= 2)
                    {
                        return Content(JsonConvert.SerializeObject(new
                        {
                            state = "error",
                            message = "不同单元格的是否推荐商品必须一致",
                            data = "不同单元格的是否推荐商品必须一致"
                        }));
                    }
                }
                if (string.IsNullOrWhiteSpace(keyValue) || copy == true)
                {
                    RepositoryManager repository = new RepositoryManager();
                    using (var db = repository.BeginTrans())
                    {
                        // Expression<Func<ActivePageContentEntity, bool>> expression = o => o.GROUP == keyValue;
                        foreach (var entity in entities)
                        {
                            if (entity.DisplayWay == 1)
                            {
                                switch (entity.Type)
                                {
                                    case 0:
                                        entity.Type = 6;
                                        break;
                                    case 1:
                                        entity.Type = 7;
                                        break;
                                    case 10:
                                        entity.Type = 11;
                                        break;
                                            
                                }
                            }
                            entity.CreateDateTime = DateTime.Now;
                            db.Insert<ActivePageContentEntity>(entity);
                        }
                        db.Commit();
                    }
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entities), Author = User.Identity.Name, Operation = "新增活动内容", ObjectType = "AUCAct" });
                    return Content(JsonConvert.SerializeObject(new
                    {
                        state = "success",
                        message = "操作成功",
                        data = ""
                    }));
                }
                else
                {
                    RepositoryManager repository = new RepositoryManager();
                    IEnumerable<ActivePageContentEntity> before = null;
                    using (var db = repository.BeginTrans())
                    {
                        Expression<Func<ActivePageContentEntity, bool>> expression = o => o.GROUP == keyValue;
                        before = repository.GetEntityList<ActivePageContentEntity>(expression)?.Where(_=>_.FKActiveID == entities?.FirstOrDefault()?.FKActiveID);
                        foreach (var e in before)
                        {
                            Expression<Func<ActivePageContentEntity, bool>> exp = _ => _.PKID == e.PKID;
                            db.Delete<ActivePageContentEntity>(exp);
                        }
                        //db.Delete<ActivePageContentEntity>(expression);
                        foreach (var entity in entities)
                        {
                            if (entity.DisplayWay == 1)
                            {
                                switch (entity.Type)
                                {
                                    case 0:
                                        entity.Type = 6;
                                        break;
                                    case 1:
                                        entity.Type = 7;
                                        break;
                                    case 10:
                                        entity.Type = 11;
                                        break;

                                }
                            }
                            entity.CreateDateTime = DateTime.Now;
                            db.Insert<ActivePageContentEntity>(entity);
                        }
                        db.Commit();
                    }
                    LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue= JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(entities), Author = User.Identity.Name, Operation = "修改活动内容", ObjectType = "AUCAct" });
                    return Content(JsonConvert.SerializeObject(new
                    {
                        state = "success",
                        message = "操作成功",
                        data = ""
                    }));
                }
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = em.Message,
                    data = em.Message
                }));
            }
        }

        [ValidateInput(false)]
        public ActionResult SubmitForm(ActivePageListEntity entity, int keyValue = 0)
        {
            if (keyValue <= 0)
            {
                RepositoryManager db = new RepositoryManager();
                entity.CreateDateTime = DateTime.Now;
                entity.PKGuid = Guid.NewGuid();
                entity.CreateorUser = User.Identity.Name;
                var code = Tuhu.Provisioning.Common.SecurityHelper.Sha1Encrypt(entity.PKGuid.ToString(), System.Text.Encoding.UTF8);
                bool isHad = false;
                int index = 8;
                do {
                    Expression<Func<ActivePageListEntity, bool>> exp = _ => _.HashKey == code.Substring(index-8, index);
                    var list = db.GetEntityList<ActivePageListEntity>(exp);
                    if (list != null && list.Count() > 0)
                        isHad = true;
                    else
                    {
                        isHad = false;
                        entity.HashKey = code.Substring(index-8, index);
                    }

                    index++;
                } while (isHad);
                

                db.Add<ActivePageListEntity>(entity);

                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "新增活动:" + entity.Title, ObjectType = "AUActivity", ObjectID = entity.PKID.ToString() });

                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = entity.PKID,
                    title=entity.Title
                }));
            }
            else
            {
                RepositoryManager db = new RepositoryManager();
                var beforeEntity = db.GetEntity<ActivePageListEntity>(keyValue);

                entity.UpdateDateTime = DateTime.Now;
                entity.PKID = keyValue;
                db.Update<ActivePageListEntity>(entity);
               
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(entity), BeforeValue=JsonConvert.SerializeObject(beforeEntity), Author = User.Identity.Name, Operation = "修改活动:" + entity.Title, ObjectType = "AUActivity", ObjectID = entity.PKID.ToString() });

                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = entity.PKID,
                    title=entity.Title
                }));
            }
        }

        public ActionResult DeleteForm(int pkid)
        {
            if (pkid <= 0)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "操作失败,参数不能为空"
                }));

            RepositoryManager repository = new RepositoryManager();
            Expression<Func<ActivePageListEntity, bool>> expression = _ => _.PKID == pkid;
            try
            {
                var entity = repository.GetEntity<ActivePageListEntity>(pkid);
                LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(entity), Author = User.Identity.Name, Operation = "删除活动"+entity.Title, ObjectType = "AUActivity" });
                repository.Delete<ActivePageListEntity>(expression);
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功"
                   
                }));

            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = em.Message
                }));
            }
             
        }

        public ActionResult ShareParameterForm()
        {
            return View();
        }

        public ActionResult SelectRowType()
        {
            return View();
        }

        /// <summary>
        /// 添加配置内容表单
        /// </summary>
        /// <returns></returns>
        public ActionResult ContentForm()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SubmitContentCopy(string group, int? fkActivityID)
        {

            if (string.IsNullOrWhiteSpace(group) || fkActivityID == null)
                return Content("null");
            try
            {
                Expression<Func<ActivePageContentEntity, bool>> expression = _ => _.FKActiveID == fkActivityID && _.GROUP == group;
                RepositoryManager repository = new RepositoryManager();
                var list = repository.GetEntityList<ActivePageContentEntity>(expression);
                if (list != null)
                {
                    using (var db = repository.BeginTrans())
                    {
                        foreach (var item in list)
                        {
                            db.Insert<ActivePageContentEntity>(item);
                        }
                        db.Commit();
                    }
                }
                else
                    throw new Exception("没有查询到对应的复制信息");

                return Content(JsonConvert.SerializeObject(new
                {
                    Code = 0,
                    Msg = "操作成功"
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    Code = -1,
                    Msg = em.Message
                }));
            }
        }

       
        public ActionResult DeleteContent(string group, int? fkActivityID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(group) || fkActivityID == null)
                    return Content("null");
                RepositoryManager repository = new RepositoryManager();
                Expression<Func<ActivePageContentEntity, bool>> expression = o => o.FKActiveID == fkActivityID && o.GROUP == group;
                var entities = repository.GetEntityList<ActivePageContentEntity>(expression);
                repository.Delete<ActivePageContentEntity>(expression);
                LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(entities), Author = User.Identity.Name, Operation = "删除活动内容", ObjectType = "AUCAct" });
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功"
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "操作失败",
                    data = em.Message
                }));
            }
        }


        public ActionResult CreateCopy(int pkid)
        {
            if(pkid <=0)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "请选择要复制的活动"
                }));

            RepositoryManager repository = new RepositoryManager();
            try
            {

                var entity = repository.GetEntity<ActivePageListEntity>(pkid);
                if (entity == null)
                    return Content(JsonConvert.SerializeObject(new
                    {
                        state = "error",
                        message = "pkid:" + pkid + "不存在"
                    }));

                ActivePageListEntity copyEntity = entity;

                Expression<Func<ActivePageContentEntity, bool>> expression = _ => _.FKActiveID == entity.PKID;

                var contents = repository.GetEntityList<ActivePageContentEntity>(expression);

                #region 会场配置
                Expression<Func<ActivePageHomeEntity, bool>> homeExp = _ => _.FKActiveID == entity.PKID;
                var home = repository.GetEntity<ActivePageHomeEntity>(homeExp);
                IEnumerable<ActivePageHomeDeatilEntity> homeList = null;
                if (home != null)
                {
                    Expression<Func<ActivePageHomeDeatilEntity, bool>> homeDeatilExp = _ => _.FKActiveHome == home.PKID;
                    homeList = repository.GetEntityList<ActivePageHomeDeatilEntity>(homeDeatilExp);
                }
                #endregion

                copyEntity.PKGuid = Guid.NewGuid();
                copyEntity.CreateDateTime = DateTime.Now;
                copyEntity.CreateorUser = User.Identity.Name;

                #region 设置Hash值
                var code = Tuhu.Provisioning.Common.SecurityHelper.Sha1Encrypt(entity.PKGuid.ToString(), System.Text.Encoding.UTF8);
                bool isHad = false;
                int index = 8;
                do
                {
                    Expression<Func<ActivePageListEntity, bool>> exp = _ => _.HashKey == code.Substring(index - 8, index);
                    var list = repository.GetEntityList<ActivePageListEntity>(exp);
                    if (list != null && list.Count() > 0)
                        isHad = true;
                    else
                    {
                        isHad = false;
                        copyEntity.HashKey = code.Substring(index - 8, index);
                    }

                    index++;
                } while (isHad);
                #endregion

                repository.Add<ActivePageListEntity>(copyEntity);

                #region 拷贝会场信息
                if (home != null)
                {
                    home.FKActiveID = copyEntity.PKID;
                    repository.Add<ActivePageHomeEntity>(home);
                    if (homeList != null)
                        foreach (var h in homeList)
                        {
                            h.FKActiveHome = home.PKID;
                            repository.Add<ActivePageHomeDeatilEntity>(h);
                        }
                }
                #endregion

                #region 拷贝内容
                if (contents != null)
                    foreach (var content in contents)
                    {
                        content.FKActiveID = copyEntity.PKID;
                        IEnumerable<ActivePageMenuEntity> menus = null;
                        if (content.Type == -2)
                        {
                            Expression<Func<ActivePageMenuEntity, bool>> menuExp = _ => _.FKActiveContentID == content.PKID;
                            menus = repository.GetEntityList<ActivePageMenuEntity>(menuExp);
                        }
                        repository.Add<ActivePageContentEntity>(content);
                        if (content.Type == -2 && menus != null)
                        {
                            foreach (var menu in menus)
                            {
                                menu.FKActiveContentID = content.PKID;
                                repository.Add<ActivePageMenuEntity>(menu);
                            }
                        }

                    }
                #endregion


                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功"
                }));

            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = em.Message
                }));
            }
        }

        public ActionResult Logger()
        {
            return View();
        }

        public ActionResult GetMessageLoggerJson(int id)
        {
            ConfigHistory model = LoggerManager.GetConfigHistory(id.ToString());
            if (model != null)
            {
                return Content(JsonConvert.SerializeObject(model));
            }
            else
            {
                return HttpNotFound();
            }
        }


        public ActionResult GetClientsDataJson()
        {
            List<ClientMenuEnitity> list = new List<ClientMenuEnitity>();
            #region 小程序菜单
            if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().ToLower().Contains("wechathome"))
            {
                ClientMenuEnitity allMenu = new ClientMenuEnitity()
                {
                    F_Id = Guid.NewGuid().ToString(),
                    F_FullName = "小程序业务",
                    F_Icon = "fa fa-gears",
                    F_ParentId = "0",
                    F_SortCode = 1
                };
                allMenu.ChildNodes = new List<ClientMenuEnitity>() {
                    new ClientMenuEnitity() {
                         F_FullName="业务列表",
                         F_Id="FC56BCCD-B2DC-4092-A96A-3D7021D31C46",
                         F_ParentId=allMenu.F_Id,
                         F_SortCode=1,
                         F_UrlAddress="/OwnBusiness/Index"
                    },
                    new ClientMenuEnitity() {
                        F_FullName="小程序二维码",
                        F_Id="",
                        F_ParentId=allMenu.F_Id,
                        F_SortCode=2,
                        F_UrlAddress="/wxappqrcode/index"
                    },
                    new ClientMenuEnitity(){
                         F_FullName="公众号菜单",
                         F_Id="574687E8-6B9B-4E75-A222-E1FEFE332884",
                         F_ParentId=allMenu.F_Id,
                         F_SortCode=3,
                         F_UrlAddress="/WechatHome/SubNumberList"
                    },
                    new ClientMenuEnitity(){
                        F_FullName="公众号关注回复菜单",
                        F_Id="E71051FA-05A1-4784-B321-96A88921AAEB",
                        F_ParentId=allMenu.F_Id,
                        F_SortCode=4,
                        F_UrlAddress="/WechatHome/WechatSubscribeList"
                    },
                    new ClientMenuEnitity(){
                        F_FullName="客服消息配置",
                        F_Id="8550f049-8bc6-453c-ae3c-a301bb5d4d9e",
                        F_ParentId=allMenu.F_Id,
                        F_SortCode=5,
                        F_UrlAddress="/WechatHome/WxAppConfig"
                    },
                    new ClientMenuEnitity(){
                        F_FullName="社交立减金代金券配置",
                        F_Id="bed8c99e-8dc1-480e-8a6f-4f09f7573891",
                        F_ParentId=allMenu.F_Id,
                        F_SortCode=6,
                        F_UrlAddress="/WechatHome/WechatSocialCardConfig"
                    },
                    new ClientMenuEnitity(){
                        F_FullName="社交立减金活动配置",
                        F_Id="2fd821ab-1d58-48e8-92b3-f21c65ceef02",
                        F_ParentId=allMenu.F_Id,
                        F_SortCode=7,
                        F_UrlAddress="/WechatHome/WechatSocialActivityConfig"
                    },
                    new ClientMenuEnitity(){
                        F_FullName="首页配置",
                        F_Id="",
                        F_ParentId=allMenu.F_Id,
                        F_SortCode=8,
                        F_UrlAddress="/WechatHome/HomePageConfig"
                    },
                    new ClientMenuEnitity(){
                    F_FullName="公众号关键字回复",
                    F_Id="6CCD7500-A537-4EF1-A640-04CA54A41180",
                    F_ParentId=allMenu.F_Id,
                    F_SortCode=9,
                    F_UrlAddress="/WechatHome/WechatKeywords"
                    },
                    new ClientMenuEnitity(){
                        F_FullName="公众号二维码",
                        F_Id="574687E8-6B9B-4E75-A222-E1FEFE332800",
                        F_ParentId=allMenu.F_Id,
                        F_SortCode=9,
                        F_UrlAddress="/WechatHome/WX_QRCodeConfigList"
                    }
                };
                list.Add(allMenu);

                return Content(JsonConvert.SerializeObject(list));
            }
            #endregion

            #region 门店铺货
            if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().ToLower().Contains("shopdistribution"))
            {
                ClientMenuEnitity allMenu = new ClientMenuEnitity()
                {
                    F_Id = Guid.NewGuid().ToString(),
                    F_FullName = "门店铺货",
                    F_Icon = "fa fa-gears",
                    F_ParentId = "0",
                    F_SortCode = 1
                };
                list.Add(allMenu);
                allMenu.ChildNodes = new List<ClientMenuEnitity>() { };

                return Content(JsonConvert.SerializeObject(list));
            }
            #endregion

            #region 下单完成页配置
            if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().ToLower().Contains("paymentpageconfig"))
            {
                ClientMenuEnitity allMenu = new ClientMenuEnitity()
                {
                    F_Id = Guid.NewGuid().ToString(),
                    F_FullName = "下单完成页配置",
                    F_Icon = "fa fa-gears",
                    F_ParentId = "0",
                    F_SortCode = 1
                };
                allMenu.ChildNodes = new List<ClientMenuEnitity>() {
                    //new ClientMenuEnitity() {
                    //     F_FullName="下单完成页配置",
                    //     F_Id="",
                    //     F_ParentId=allMenu.F_Id,
                    //     F_SortCode=1,
                    //     F_UrlAddress="/PaymentPageConfig/List"
                    //},
                    new ClientMenuEnitity() {
                        F_FullName="广告位配置",
                        F_Id="",
                        F_ParentId=allMenu.F_Id,
                        F_SortCode=2,
                        F_UrlAddress="/AdvertisingConfig/List"
                    }
                };
                list.Add(allMenu);
                return Content(JsonConvert.SerializeObject(list));
            }
            #endregion

            #region 常见问题配置
            if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().ToLower().Contains("productfaqmanage"))
            {
                ClientMenuEnitity allMenu = new ClientMenuEnitity()
                {
                    F_Id = Guid.NewGuid().ToString(),
                    F_FullName = "商品常见问题配置",
                    F_Icon = "fa fa-gears",
                    F_ParentId = "0",
                    F_SortCode = 1
                };
                allMenu.ChildNodes = new List<ClientMenuEnitity>() {
                    new ClientMenuEnitity() {
                         F_FullName="已配置商品列表",
                         F_Id="FC56BCCD-B2DC-4092-A96A-3D7021D31C46",
                         F_ParentId=allMenu.F_Id,
                         F_SortCode=1,
                         F_UrlAddress="/ProductFaqManage/List"
                    },
                    new ClientMenuEnitity() {
                        F_FullName="新增常见问题",
                        F_Id="",
                        F_ParentId=allMenu.F_Id,
                        F_SortCode=2,
                        F_UrlAddress="/ProductFaqManage/ConfigList"
                    }
                };
                list.Add(allMenu);

                return Content(JsonConvert.SerializeObject(list));
            }
            #endregion
            #region 机型
            if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().ToLower().Contains("devicebrand"))
            {
                ClientMenuEnitity allMenu = new ClientMenuEnitity()
                {
                    F_Id = Guid.NewGuid().ToString(),
                    F_FullName = "其他配置",
                    F_Icon = "fa fa-gears",
                    F_ParentId = "0",
                    F_SortCode = 1
                };
                allMenu.ChildNodes = new List<ClientMenuEnitity>() {
                    new ClientMenuEnitity() {
                        F_FullName="Android机型配置",
                        F_Id="FC56BCCD-B2DC-4092-A96A-3D7021D31C46",
                        F_ParentId=allMenu.F_Id,
                        F_SortCode=1,
                        F_UrlAddress="/DeviceBrand/List"
                    }, 
                };
                list.Add(allMenu);
                return Content(JsonConvert.SerializeObject(list));
            }
            #endregion
            ClientMenuEnitity activity = new ClientMenuEnitity()
            {
                F_Id = Guid.NewGuid().ToString(),
                F_FullName = "营销活动页",
                F_Icon = "fa fa-gears",
                F_ParentId = "0",
                F_SortCode = 1
            };
            activity.ChildNodes = new List<ClientMenuEnitity>()
            {
                new ClientMenuEnitity() { F_FullName="活动列表", F_Id="4794C57D-5F82-4CB5-9A9C-4F3F42B02E96", F_ParentId=activity.F_Id, F_SortCode=1, F_UrlAddress="/Activity/ActivityUnity/List" }
            };

            list.Add(activity);

            ClientMenuEnitity lucky = new ClientMenuEnitity()
            {
                 F_FullName="大翻盘抽奖",
                 F_Icon= "fa fa-desktop",
                 F_ParentId="0",
                 F_Id=Guid.NewGuid().ToString(),
                 F_SortCode=2
            };
            lucky.ChildNodes = new List<ClientMenuEnitity>()
            {
                new ClientMenuEnitity() {  Target=" target=\"_bank\" ", F_FullName="大翻盘列表", F_Id=Guid.NewGuid().ToString(), F_ParentId=lucky.F_Id, F_SortCode=1, F_UrlAddress="/LuckyWheel/List" },
                new ClientMenuEnitity() { F_FullName="新大翻盘列表",F_Id=Guid.NewGuid().ToString(),F_ParentId=lucky.F_Id,F_SortCode=2,F_UrlAddress="/BigBrand/BigBrandReward/Index" },
                new ClientMenuEnitity() { F_FullName="问答题库",F_Id=Guid.NewGuid().ToString(),F_ParentId=lucky.F_Id,F_SortCode=3,F_UrlAddress="/Answer/Index" }
            };
            list.Add(lucky);

            ClientMenuEnitity trie = new ClientMenuEnitity()
            {
                F_FullName = "轮胎列表页管理",
                F_Icon = "fa fa-bar-chart-o",
                F_ParentId = "0",
                F_Id = Guid.NewGuid().ToString(),
                F_SortCode = 3
            };
            trie.ChildNodes = new List<ClientMenuEnitity>()
            {
                new ClientMenuEnitity() {  Target=" target=\"_bank\" ", F_FullName="列表", F_Id=Guid.NewGuid().ToString(), F_ParentId=trie.F_Id, F_SortCode=1, F_UrlAddress="" }
            };
            list.Add(trie);

            ClientMenuEnitity lg = new ClientMenuEnitity()
            {
                F_FullName = "轮毂列表页管理",
                F_Icon = "fa fa-gears",
                F_ParentId = "0",
                F_Id = Guid.NewGuid().ToString(),
                F_SortCode = 4
            };
            lg.ChildNodes = new List<ClientMenuEnitity>()
            {
                new ClientMenuEnitity() { Target=" target=\"_bank\" ", F_FullName="列表", F_Id=Guid.NewGuid().ToString(), F_ParentId=lg.F_Id, F_SortCode=1, F_UrlAddress="" }
            };
            list.Add(lg);
            return Content(JsonConvert.SerializeObject(list));
        }


        public ActionResult ConvertToActivityUnity(int id)
        {
            ActivityManager manager = new ActivityManager();
            var oldEntity = manager.GetActivityBuildById(id);
            JObject json = JObject.Parse(oldEntity.Content);
            var contents = JsonConvert.DeserializeObject<IEnumerable<ActivityBuildDetail>>(json["rows"].ToString());
            BigActivityHome bigActivityHome = null;
            if (!string.IsNullOrWhiteSpace(oldEntity.BigActivityHome))
            {
                bigActivityHome = JsonConvert.DeserializeObject<BigActivityHome>(oldEntity.BigActivityHome);
            }
           IEnumerable<ActivityMenu> activityMenu = null;
            if (!string.IsNullOrWhiteSpace(oldEntity.ActivityMenu))
            {
                activityMenu = JsonConvert.DeserializeObject<IEnumerable<ActivityMenu>>(oldEntity.ActivityMenu);
            }

            TireSizeConfig tireSizeConfig = null;
            if (!string.IsNullOrWhiteSpace(oldEntity.TireSizeConfig))
            {
                tireSizeConfig = JsonConvert.DeserializeObject<TireSizeConfig>(oldEntity.TireSizeConfig);
            }

            RepositoryManager repository = new RepositoryManager();
            ActivePageListEntity newEntitiy = new ActivePageListEntity();
            try
            {
                newEntitiy.PKGuid = Guid.NewGuid();

                var code = Tuhu.Provisioning.Common.SecurityHelper.Sha1Encrypt(newEntitiy.PKGuid.ToString(), System.Text.Encoding.UTF8);
                newEntitiy.HashKey = code.Substring(0, 8);
                newEntitiy.ActivityType = oldEntity.ActivityType;
                newEntitiy.BgColor = oldEntity.BgColor;
                newEntitiy.BgImageUrl = oldEntity.BgImageUrl;
                newEntitiy.CreateDateTime = oldEntity.CreateTime;
                newEntitiy.CreateorUser = oldEntity.CreatetorUser;
                newEntitiy.DataParames = oldEntity.DataParames;
                newEntitiy.EndDate = oldEntity.EndDate;
                newEntitiy.H5Uri = "";
                newEntitiy.IsEnabled = true;
                newEntitiy.IsShowDate = oldEntity.IsShowDate == 1?true:false;
                newEntitiy.IsTireSize = oldEntity.IsTireSize;
                newEntitiy.MenuType = oldEntity.MenuType;
              
                newEntitiy.SelKeyImage = oldEntity.SelKeyImage;
                newEntitiy.SelKeyName = oldEntity.SelKeyName;
                newEntitiy.StartDate = oldEntity.CreateTime;
                newEntitiy.TireBrand = oldEntity.TireBrand;
                newEntitiy.Title = oldEntity.Title;
                newEntitiy.WWWUri = "";
                newEntitiy.PersonCharge = oldEntity.PersonWheel;
                repository.Add<ActivePageListEntity>(newEntitiy);
                using (var db = repository.BeginTrans())
                {
                    #region  添加内容
                    if (contents != null)
                    {
                        foreach (var c in contents)
                        {
                            ActivePageContentEntity item = new ActivePageContentEntity();
                            if (!string.IsNullOrWhiteSpace(c.VID))
                            {
                                item.ActivityID =Guid.Parse( c.VID);
                            }
                            item.ActivityPrice = c.ActivityPrice;
                            item.APPUrl = c.LinkUrl;
                            item.Channel = c.Channel;
                            if (!string.IsNullOrWhiteSpace(c.CID))
                                item.CID = Guid.Parse( c.CID);
                            item.CreateDateTime = DateTime.Now;
                            item.Description = c.Description;
                            item.DisplayWay = c.DisplayWay;
                            item.FKActiveID = newEntitiy.PKID;
                            item.GROUP = c.Group;
                            item.Image = c.Image;
                            item.IsUploading = c.IsUploading == 1?true:false;
                            item.LinkUrl = c.LinkUrl;
                            if (!string.IsNullOrWhiteSpace(c.OrderBy))
                                item.OrderBy = Convert.ToInt32(c.OrderBy);
                            item.PCUrl = c.PCUrl;
                            item.PID = c.PID;
                            item.ProductName = c.ProductName;
                            item.RowType = c.BigImg;
                            item.TireSize = c.TireSize;
                            
                            item.Type = c.Type;
                            if (c.Type == 6 || c.Type == 7 || c.Type == 11)
                                item.DisplayWay = 1;
                            else if (c.Type == 0 || c.Type == 1 || c.Type == 10)
                                item.DisplayWay = 0;
                            else { }
                            

                            if (item.Type != -2)
                                db.Insert<ActivePageContentEntity>(item);
                            else
                            {
                                repository.Add<ActivePageContentEntity>(item);
                                #region 添加菜单
                                if (activityMenu != null)
                                {
                                    foreach (var menu in activityMenu)
                                    {
                                        ActivePageMenuEntity menuEntity = new ActivePageMenuEntity();
                                        menuEntity.Color = menu.Color;
                                        menuEntity.CreateDateTime = DateTime.Now;
                                        menuEntity.FKActiveContentID = item.PKID;
                                        menuEntity.MenuName = menu.MenuName;
                                        menuEntity.MenuValue = menu.MenuValue;
                                        menuEntity.MenuValueEnd = menu.MenuValueEnd;
                                        menuEntity.Sort = menu.Sort;
                                        db.Insert<ActivePageMenuEntity>(menuEntity);
                                    }
                                   
                                }
                                #endregion
                            }


                        }
                    }
                    #endregion
                    //------
                    #region 添加会场
                    if (bigActivityHome != null)
                    {
                        int index = 1;
                        ActivePageHomeEntity home = new ActivePageHomeEntity();
                        home.BigHomeName = bigActivityHome.BigHomeName;
                        home.BigHomeUrl = bigActivityHome.BigHomeUrl;
                        home.BigHomeUrlWww = bigActivityHome.BigHomeUrlWww;
                        home.CreateDateTime = DateTime.Now;
                        home.FKActiveID = newEntitiy.PKID;
                        home.HidBigHomePic = bigActivityHome.HidBigHomePic;
                        home.HidBigHomePicWww = bigActivityHome.HidBigHomePicWww;
                        home.IsHome = true;
                        home.Sort = 1;
                        repository.Add<ActivePageHomeEntity>(home);
                        foreach (var h in bigActivityHome.Rows)
                        {
                            ActivePageHomeEntity fenEntity = new ActivePageHomeEntity();
                            fenEntity.BigHomeName = h.FHomeName;
                            fenEntity.CreateDateTime = DateTime.Now;
                            fenEntity.FKActiveID = newEntitiy.PKID;
                            fenEntity.HidBigHomePic = h.HidImageFHome;
                            fenEntity.HidBigHomePicWww = h.ImageFHome;
                            fenEntity.BigHomeUrl = string.Empty;
                            fenEntity.BigHomeUrlWww = string.Empty;
                            fenEntity.IsHome = false;
                            fenEntity.Sort = ++index;
                            repository.Add<ActivePageHomeEntity>(fenEntity);
                            foreach (var i in h.Items)
                            {
                                ActivePageHomeDeatilEntity deatil = new ActivePageHomeDeatilEntity();
                                deatil.BigFHomeMobileUrl = i.BigFHomeMobileUrl;
                                deatil.BigFHomeOrder = i.BigFHomeOrder;
                                deatil.BigFHomeWwwUrl = i.BigFHomeWwwUrl;
                                deatil.CreateDateTime = DateTime.Now;
                                deatil.FKActiveHome = fenEntity.PKID;
                                deatil.HidBigFHomePic = i.HidBigFHomePic;
                                deatil.HomeName = h.FHomeName;
                                db.Insert<ActivePageHomeDeatilEntity>(deatil);
                            }
                        }
                    }
                    #endregion

                    if (tireSizeConfig != null)
                    {
                        ActivePageTireSizeConfigEntity sizeConfigEntity = new ActivePageTireSizeConfigEntity();
                        sizeConfigEntity.CarInfoColor = tireSizeConfig.CarInfoColor;
                        sizeConfigEntity.CarInfoFontSize = tireSizeConfig.CarInfoFontSize;
                        sizeConfigEntity.CreateDateTime = DateTime.Now;
                        sizeConfigEntity.FillColor = tireSizeConfig.FillColor;
                        sizeConfigEntity.FKActiveID = newEntitiy.PKID;
                        sizeConfigEntity.PromptColor = tireSizeConfig.PromptColor;
                        sizeConfigEntity.IsChangeTire = tireSizeConfig.IsChangeTire == 1 ? true : false ;
                        sizeConfigEntity.IsChangeTireSize = tireSizeConfig.IsChangeTireSize == 1?true:false;
                        sizeConfigEntity.IsMargin = tireSizeConfig.IsMargin == 1 ? true : false;
                        sizeConfigEntity.IsShowTag = tireSizeConfig.IsShowTag == 1 ? true : false;
                        sizeConfigEntity.MarginColor = tireSizeConfig.MarginColor;
                        sizeConfigEntity.NoCarTypePrompt = tireSizeConfig.NoCarTypePrompt;
                        sizeConfigEntity.NoFormatPrompt = tireSizeConfig.NoFormatPrompt;
                        sizeConfigEntity.PromptColor = tireSizeConfig.PromptColor;
                        sizeConfigEntity.PromptFontSize = tireSizeConfig.PromptFontSize;
                        db.Insert<ActivePageTireSizeConfigEntity>(sizeConfigEntity);

                    }
                    db.Commit();
                }
                return Content(JsonConvert.SerializeObject(new
                {
                    Status = "导入成功"
                }));


            }
            catch (Exception em)
            {
                Expression<Func<ActivePageListEntity, bool>> expression = _ => _.PKID == newEntitiy.PKID;
                repository.Delete<ActivePageListEntity>(expression);
                return Content(JsonConvert.SerializeObject(new
                {
                    Status = "导入失败",
                    Msg = em.Message,
                    Error =em
                }));
            }

           
        }


        public ActionResult SetParameterForm()
        {
            return View();
        }

        public async Task<ActionResult> GetBaoyangProdcutJson()
        {
            using (var client = new Tuhu.Service.BaoYang.BaoYangClient())
            {
                var result = await client.GetPackageTypeZhDicAsync();
                if (result.Success)
                {
                    var dic = result.Result;
                    var list = new List<object>();
                    foreach (KeyValuePair<string, string> kv in dic)
                    {
                        list.Add(new { text=kv.Value,id=kv.Key });
                    }
                    return Content(JsonConvert.SerializeObject(list));
                }
                else
                    return Content("null");
            }
        }

        public async Task<ActionResult> RefreshSource(int? keyValue)
        {
            if (keyValue == null)
                return Content(JsonConvert.SerializeObject(new {
                   Msg="keyValue不能为空",
                   Code=-1
                }));

            RepositoryManager manager = new RepositoryManager();
            var entity = manager.GetEntity<ActivePageListEntity>(keyValue);
            if(entity == null)
                return Content(JsonConvert.SerializeObject(new
                {
                    Msg = "活动不存在",
                    Code = -1
                }));

            try
            {
                using (var client = new Tuhu.Service.Activity.ActivityClient())
                {
                    var result1 = await client.RefreshActivePageListModelCacheAsync(new Service.Activity.Models.Requests.ActivtyPageRequest()
                    {
                        Channel = "wap",
                        HashKey=entity.HashKey,
                    });
                    result1.ThrowIfException(true);
                    var result2 = await client.RefreshActivePageListModelCacheAsync(new Service.Activity.Models.Requests.ActivtyPageRequest()
                    {
                        ActivityId = entity.PKGuid.Value,
                        Channel = "website",
                        HashKey=entity.HashKey,
                    });
                    result2.ThrowIfException(true);
                }

                Expression<Func<ActivePageContentEntity, bool>> exp = _ => _.FKActiveID == keyValue;
                var list = manager.GetEntityList<ActivePageContentEntity>(exp);
                var lucy = list?.Where(_ => _.Type == 13)?.FirstOrDefault();
                if (lucy != null)
                {
                    using (var client = new Tuhu.Service.Activity.ActivityClient())
                    {
                        var result = await client.RefreshLuckWheelCacheAsync(lucy.ActivityID.ToString());
                        result.ThrowIfException(true);
                    }
                }
                return Content(JsonConvert.SerializeObject(new {
                    Code=1,
                    Msg="更新成功"
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    Msg = "更新失败",
                    Code = -1,
                    Error = em.Message
                }));
            }
        }

        public  ActionResult UpdateGroup(int keyValue, string group,string input)
        {

            RepositoryManager manager = new RepositoryManager();
            Expression<Func<ActivePageContentEntity, bool>> exp = _ => _.GROUP == group && _.FKActiveID == keyValue;
            var model = manager.GetEntityList<ActivePageContentEntity>(exp);
            if (model == null || model.Count()<=0)
                return Content(JsonConvert.SerializeObject(new {
                    Code =-1,
                    Msg="对象不存在"
                }));


            Expression<Func<ActivePageContentEntity, bool>> exitsExp = _ => _.GROUP == input.Trim() && _.FKActiveID == keyValue;
            var exitsList = manager.GetEntityList<ActivePageContentEntity>(exitsExp);
            if (exitsList != null && exitsList.Count() > 0)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    Code = -1,
                    Msg = "设置的组号不允许重复出现"
                }));

            }

            using (var db = manager.BeginTrans())
            {
                foreach (var entity in model)
                {
                    entity.GROUP = input.Trim();
                    entity.UpdateDateTime = DateTime.Now;
                    db.Update<ActivePageContentEntity>(entity);
                }
                db.Commit();
            }
            return Content(JsonConvert.SerializeObject(new
            {
                Code = 1,
                Msg = "更新成功"
            }));
        }
        public async Task<JsonResult> GetProductGroupInfoByPIdAsync(string pid)
        {
            JsonResult jr = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            using (var client = new Tuhu.Service.PinTuan.PinTuanClient())
            {
                var result = await client.GetProductGroupInfoByPIdAsync(pid);
                result.ThrowIfException(true);
                jr.Data = new
                {
                    code = 1,
                    data = result.Result
                };

            }
            return jr;
        }
    }
}