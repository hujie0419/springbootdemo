﻿using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Collections.Generic;

using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Mapping;

namespace Tuhu.Provisioning.Controllers
{
    public class HomePageCarActivityController : Controller
    {
        // GET: HomePageCarActivity
        public ActionResult Index()
        {
            RepositoryManager manager = new RepositoryManager();
            Expression<Func<HomePageCarActivityEntity, bool>> exp = _ => _.PKID > 0;
            var list = manager.GetEntityList(exp);
            return View(list?.OrderBy(_ => _.OrderBy));
        }


        public ActionResult Edit(int pkid = 0)
        {
            ViewBag.CarItems = "";
            ViewBag.City = "";
            if (pkid == 0)
                return View(new HomePageCarActivityEntity { Status = true });
            RepositoryManager manager = new RepositoryManager();
            return View(manager.GetEntity<HomePageCarActivityEntity>(pkid));
        }


        public ActionResult GetBrand()
        {
            using (var client = new Service.Vehicle.VehicleClient())
            {
                var result = client.GetAllVehicles();

                result.ThrowIfException(true);
                return Content(JsonConvert.SerializeObject(result.Result.Select(_ => new
                {
                    _.Brand
                }).Distinct()));
            }
        }

        public ActionResult GetVehicle(string brand)
        {
            using (var client = new Service.Vehicle.VehicleClient())
            {
                var result = client.GetAllVehicles();

                result.ThrowIfException(true);
                return Content(JsonConvert.SerializeObject(result.Result.Where(_ => _.Brand == brand).Select(_ => new { CarName = _.Vehicle, _.VehicleId }).Distinct()));
            }
        }

        public ActionResult Save(string json, string cityInfo, string cars)
        {
            HomePageCarActivityEntity model = JsonConvert.DeserializeObject<HomePageCarActivityEntity>(json);
            if (model.PKID == 0)
            {
                RepositoryManager manger = new RepositoryManager();
                model.CreateDateTime = DateTime.Now;
                model.LastUpdateDateTime = DateTime.Now;

                model.HashKey = Common.SecurityHelper.Sha1Encrypt(Guid.NewGuid().ToString(), System.Text.Encoding.UTF8).Substring(0, 8);
                manger.Add(model);
                if (model.PKID <= 0)
                    throw new Exception("保存失败");

                var citys = cityInfo?.Replace("undefined", "")?.Trim(',')?.Split(',');
                if (citys?.Length > 0)
                {
                    Expression<Func<HomePageCarActivityRegionEntity, bool>> exp = _ => _.FKCarActviityPKID == model.PKID;
                    manger.Delete(exp);
                    foreach (var city in citys)
                    {
                        if (!string.IsNullOrWhiteSpace(city))
                            manger.Add(new HomePageCarActivityRegionEntity { LastUpdateDateTime = DateTime.Now, CreateDateTime = DateTime.Now, FKCarActviityPKID = model.PKID, RegionID = Convert.ToInt32(city) });
                    }
                }
                if (!string.IsNullOrWhiteSpace(cars))
                {
                    var carItmes = JsonConvert.DeserializeObject<IEnumerable<HomePageCACarInfoEntity>>(cars);
                    Expression<Func<HomePageCACarInfoEntity, bool>> exp = _ => _.FKCarActviityPKID == model.PKID;
                    manger.Delete(exp);

                    foreach (var item in carItmes)
                    {
                        item.CreateDateTime = DateTime.Now;
                        item.LastUpdateDateTime = DateTime.Now;
                        item.FKCarActviityPKID = model.PKID;
                        manger.Add(item);
                    }
                }
                LoggerManager.InsertOplog(new ConfigHistory { ObjectID = model.PKID.ToString(), AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ObjectType = "HPCarAct", ChangeDatetime = DateTime.Now, Operation = "新增活动车型配置" });
                return Content("1");
            }
            else
            {
                RepositoryManager manger = new RepositoryManager();
                var before = manger.GetEntity<HomePageCarActivityEntity>(model.PKID);
                model.LastUpdateDateTime = DateTime.Now;
                manger.Update(model);
                if (model.PKID <= 0)
                    throw new Exception("保存失败");
                var citys = cityInfo?.Replace("undefined", "")?.Trim(',')?.Split(',');
                if (citys?.Length > 0)
                {
                    Expression<Func<HomePageCarActivityRegionEntity, bool>> exp = _ => _.FKCarActviityPKID == model.PKID;
                    manger.Delete(exp);
                    foreach (var city in citys)
                    {
                        if (!string.IsNullOrWhiteSpace(city))
                            manger.Add(new HomePageCarActivityRegionEntity { LastUpdateDateTime = DateTime.Now, CreateDateTime = DateTime.Now, FKCarActviityPKID = model.PKID, RegionID = Convert.ToInt32(city) });
                    }
                }
                else
                {
                    Expression<Func<HomePageCarActivityRegionEntity, bool>> exp = _ => _.FKCarActviityPKID == model.PKID;
                    manger.Delete(exp);
                }


                if (!string.IsNullOrWhiteSpace(cars))
                {
                    var carItmes = JsonConvert.DeserializeObject<IEnumerable<HomePageCACarInfoEntity>>(cars);
                    Expression<Func<HomePageCACarInfoEntity, bool>> exp = _ => _.FKCarActviityPKID == model.PKID;
                    manger.Delete(exp);

                    foreach (var item in carItmes)
                    {
                        item.CreateDateTime = DateTime.Now;
                        item.LastUpdateDateTime = DateTime.Now;
                        item.FKCarActviityPKID = model.PKID;
                        manger.Add(item);
                    }
                }
                else
                {
                    Expression<Func<HomePageCACarInfoEntity, bool>> exp = _ => _.FKCarActviityPKID == model.PKID;
                    manger.Delete(exp);
                }

                LoggerManager.InsertOplog(new ConfigHistory { ObjectID = model.PKID.ToString(), AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ObjectType = "HPCarAct", ChangeDatetime = DateTime.Now, Operation = "编辑活动车型配置" });

                return Content("1");
            }
        }

        public ActionResult GetEntity(int pkid)
        {
            RepositoryManager manger = new RepositoryManager();
            var entity = manger.GetEntity<HomePageCarActivityEntity>(pkid);
            var regionList = new SE_HomePageConfigManager().GetHomePageCarActivityCity(pkid);

            Expression<Func<HomePageCACarInfoEntity, bool>> carExp = _ => _.FKCarActviityPKID == pkid;
            var carList = manger.GetEntityList(carExp);
            string carInfo = JsonConvert.SerializeObject(carList?.Select(_ => new
            {
                _.Brand,
                _.SalesName,
                _.u_Nian,
                _.u_PaiLiang,
                _.Vehicle
            }));
            ViewBag.CarItems = carInfo;
            ViewBag.City = JsonConvert.SerializeObject(regionList?.Select(_ => new { _.RegionID, _.ParentID }));
            return View("Edit", entity);
        }

        public ActionResult Delete(int pkid)
        {
            RepositoryManager manger = new RepositoryManager();
            var before = manger.GetEntity<HomePageCarActivityEntity>(pkid);
            Expression<Func<HomePageCarActivityEntity, bool>> exp = _ => _.PKID == pkid;
            manger.Delete(exp);
            LoggerManager.InsertOplog(new ConfigHistory { ObjectID = pkid.ToString(), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ObjectType = "HPCarAct", ChangeDatetime = DateTime.Now, Operation = "删除活动车型配置" });

            return Content("1");
        }

        public ActionResult Reload()
        {
            using (var client = new Service.Config.HomePageClient())
            {
                var result = client.RefreshHomePageCarActivityCache();
                result.ThrowIfException(true);
                return result.Result ? Content("1") : Content("0");
            }
        }

    }
}