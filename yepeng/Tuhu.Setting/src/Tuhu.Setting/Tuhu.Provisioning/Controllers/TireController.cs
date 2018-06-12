﻿using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.Business.Tire;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Tire;
using Tuhu.Provisioning.Models;
using Tuhu.Service.Activity;
using Tuhu.Service.Config;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Activity.Models.Requests;
using ConfigModel = Tuhu.Service.Config.Models;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Mapping;
using System.Data;
using Tuhu.Service.Member;
using Tuhu.Service.UserAccount.Enums;
using System.Text.RegularExpressions;
using Tuhu.Provisioning.Business.PromotionCodeManagerment;
using NPOI.SS.UserModel;
using Tuhu.Provisioning.DataAccess.DAO.CompetingProductsMonitor;

namespace Tuhu.Provisioning.Controllers
{
    public class TireController : Controller
    {
        //
        // GET: /Tire/[PowerManage]

        #region 
        private readonly Lazy<TireSpecParamsConfigManager> lazy = new Lazy<TireSpecParamsConfigManager>();
        private TireSpecParamsConfigManager TireSpecParamsConfigManager
        {
            get { return lazy.Value; }
        }
        #endregion

        #region 根据车型推荐轮胎
        [PowerManage]
        public ActionResult RecommendByVehicle()
        {
            TireViewModel model = new TireViewModel()
            {
                VehicleDepartment = RecommendManager.GetVehicleDepartment(),
                //VehicleOneTwoLevel = RecommendManager.GetVehicleOneTwoLevel(),
                Brands = RecommendManager.GetBrands(),
                VehicleBodyTypes = RecommendManager.GetVehicleBodys(),
                TireSize = RecommendManager.SelectALLTireSize()
            };
            return View(model);
        }
        /// <summary>
        /// 弹框(品牌车型)
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectVehicle()
        {
            return View(RecommendManager.GetVehicleOneTwoLevel());
        }

        public ActionResult GetSpecificationsByVehicle(string vehicleIDS)
        {
            return Json(RecommendManager.GetSpecificationsByVehicle(vehicleIDS));
        }

        public PartialViewResult List(string Departments, string VehicleIDS, string PriceRanges, string VehicleBodyTypes, string Specifications, string Brands, int IsShow, string PID, int PageSize, string Province, string City)
        {
            var model = RecommendManager.SelectListNew(Departments, VehicleIDS, PriceRanges, VehicleBodyTypes, Specifications, Brands, IsShow, PID, Province, City);
            ViewBag.PageSize = PageSize;
            return PartialView(model);
        }
        public ActionResult GetYPbyTireSize(string PIDS, string TireSize)
        {
            return Json(RecommendManager.GetMatchBySize(PIDS, TireSize), JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckPID(string PID, string TireSize)
        {
            return Json(RecommendManager.CheckPID(PID, TireSize));
        }
        public ActionResult CheckPIDReplace(string PID, string Type)
        {
            return Json(RecommendManager.CheckPIDReplace(PID, Type));
        }

        public ActionResult SaveSingle(string PID1, string PID2, string PID3, string Reason1, string Reason2, string Reason3, string TireSize, string VehicleID)
        {
            if (string.IsNullOrWhiteSpace(TireSize) || string.IsNullOrWhiteSpace(VehicleID) || (string.IsNullOrWhiteSpace(PID1) && string.IsNullOrWhiteSpace(PID2) && string.IsNullOrWhiteSpace(PID3)))
                return Json(-1);

            List<VehicleTireRecommend> list = new List<VehicleTireRecommend>();
            if (!string.IsNullOrWhiteSpace(PID1))
                list.Add(new VehicleTireRecommend() { PID = PID1, Reason = Reason1, Postion = 2, TireSize = TireSize, Vehicleid = VehicleID });
            if (!string.IsNullOrWhiteSpace(PID2))
                list.Add(new VehicleTireRecommend() { PID = PID2, Reason = Reason2, Postion = 4, TireSize = TireSize, Vehicleid = VehicleID });
            if (!string.IsNullOrWhiteSpace(PID3))
                list.Add(new VehicleTireRecommend() { PID = PID3, Reason = Reason3, Postion = 7, TireSize = TireSize, Vehicleid = VehicleID });


            //日志beforevalue
            var model = RecommendManager.SelectTireRecommendByVehicleAndSize(TireSize, VehicleID);
            //操作
            var result = RecommendManager.SaveSingle(list);
            if (result > 0)
            {
                //记录日志
                InsertSaveSingleLog(model, PID1, PID2, PID3, Reason1, Reason2, Reason3, TireSize, VehicleID);
            }


            return Json(result);
        }

        static void InsertSaveSingleLog(IEnumerable<VehicleTireRecommend> model, string PID1, string PID2, string PID3, string Reason1, string Reason2, string Reason3, string TireSize, string VehicleID)
        {
            if (string.IsNullOrWhiteSpace(PID1))
                Reason1 = null;
            if (string.IsNullOrWhiteSpace(PID2))
                Reason2 = null;
            if (string.IsNullOrWhiteSpace(PID3))
                Reason3 = null;
            var valueModel = new ForLogModel()
            {
                NewPID1 = PID1,
                NewPID2 = PID2,
                NewPID3 = PID3,
                NewReason1 = Reason1,
                NewReason2 = Reason2,
                NewReason3 = Reason3,
                TireSize = TireSize,
                Vehicleid = VehicleID
            };
            var isCreate = true;
            if (model != null && model.Any())
            {
                //修改
                isCreate = false;
                valueModel.OldPID1 = model.FirstOrDefault(C => C.Postion == 2)?.PID;
                valueModel.OldPID2 = model.FirstOrDefault(C => C.Postion == 4)?.PID;
                valueModel.OldPID3 = model.FirstOrDefault(C => C.Postion == 7)?.PID;

                valueModel.OldReason1 = model.FirstOrDefault(C => C.Postion == 2)?.Reason;
                valueModel.OldReason2 = model.FirstOrDefault(C => C.Postion == 4)?.Reason;
                valueModel.OldReason3 = model.FirstOrDefault(C => C.Postion == 7)?.Reason;
            }
            AddOprLog(JsonConvert.SerializeObject(valueModel), "TireTJ", isCreate ? "新增" : "修改", valueModel.Vehicleid + "|" + valueModel.TireSize);
        }
        static void InsertSaveManyLog(IEnumerable<VehicleTireRecommend> modellist, string PID1, string PID2, string PID3, string Reason1, string Reason2, string Reason3, string TireSize, string VehicleIDS)
        {
            if (string.IsNullOrWhiteSpace(PID1))
                Reason1 = null;
            if (string.IsNullOrWhiteSpace(PID2))
                Reason2 = null;
            if (string.IsNullOrWhiteSpace(PID3))
                Reason3 = null;
            var GUID = Guid.NewGuid().ToString();
            foreach (var vehicleID in VehicleIDS.Split(','))
            {
                var model = modellist == null || !modellist.Any() ? null : modellist.Where(C => C.Vehicleid == vehicleID);

                var valueModel = new ForLogModel()
                {
                    TireSize = TireSize,
                    Vehicleid = vehicleID,
                    Group = GUID
                };
                var isCreate = true;
                if (model != null && model.Any())
                {
                    var oldPID1 = model.FirstOrDefault(C => C.Postion == 2)?.PID;
                    var oldPID2 = model.FirstOrDefault(C => C.Postion == 4)?.PID;
                    var oldPID3 = model.FirstOrDefault(C => C.Postion == 7)?.PID;
                    var oldReason1 = model.FirstOrDefault(C => C.Postion == 2)?.Reason;
                    var oldReason2 = model.FirstOrDefault(C => C.Postion == 4)?.Reason;
                    var oldReason3 = model.FirstOrDefault(C => C.Postion == 7)?.Reason;
                    //修改
                    isCreate = false;
                    valueModel.OldPID1 = oldPID1;
                    valueModel.OldPID2 = oldPID2;
                    valueModel.OldPID3 = oldPID3;

                    valueModel.OldReason1 = oldReason1;
                    valueModel.OldReason2 = oldReason2;
                    valueModel.OldReason3 = oldReason3;

                    valueModel.NewPID1 = string.IsNullOrWhiteSpace(PID1) ? oldPID1 : PID1;
                    valueModel.NewPID2 = string.IsNullOrWhiteSpace(PID2) ? oldPID2 : PID2;
                    valueModel.NewPID3 = string.IsNullOrWhiteSpace(PID3) ? oldPID3 : PID3;

                    valueModel.NewReason1 = string.IsNullOrWhiteSpace(Reason1) ? oldReason1 : Reason1;
                    valueModel.NewReason2 = string.IsNullOrWhiteSpace(Reason2) ? oldReason2 : Reason2;
                    valueModel.NewReason3 = string.IsNullOrWhiteSpace(Reason3) ? oldReason3 : Reason3;
                }
                else
                {
                    valueModel.NewPID1 = PID1;
                    valueModel.NewPID2 = PID2;
                    valueModel.NewPID3 = PID3;

                    valueModel.NewReason1 = Reason1;
                    valueModel.NewReason2 = Reason2;
                    valueModel.NewReason3 = Reason3;
                }
                AddOprLog(JsonConvert.SerializeObject(valueModel), "TireTJ", isCreate ? "批量编辑(新增)" : "批量编辑(修改)", valueModel.Vehicleid + "|" + valueModel.TireSize);
            }

        }

        public ActionResult SaveMany(string PID1, string PID2, string PID3, string Reason1, string Reason2, string Reason3, string TireSize, string VehicleIDS)
        {
            if (string.IsNullOrWhiteSpace(TireSize) || string.IsNullOrWhiteSpace(VehicleIDS) || (string.IsNullOrWhiteSpace(PID1) && string.IsNullOrWhiteSpace(PID2) && string.IsNullOrWhiteSpace(PID3)))
                return Json(-1);
            List<VehicleTireRecommend> list = new List<VehicleTireRecommend>();
            if (!string.IsNullOrWhiteSpace(PID1))
                list.Add(new VehicleTireRecommend() { PID = PID1, Reason = Reason1, Postion = 2, TireSize = TireSize, VehicleIDS = VehicleIDS.Split(',') });
            if (!string.IsNullOrWhiteSpace(PID2))
                list.Add(new VehicleTireRecommend() { PID = PID2, Reason = Reason2, Postion = 4, TireSize = TireSize, VehicleIDS = VehicleIDS.Split(',') });
            if (!string.IsNullOrWhiteSpace(PID3))
                list.Add(new VehicleTireRecommend() { PID = PID3, Reason = Reason3, Postion = 7, TireSize = TireSize, VehicleIDS = VehicleIDS.Split(',') });

            //日志brefoevalue
            var modellist = RecommendManager.SelectTireRecommendByVehicleSAndSize(TireSize, VehicleIDS);
            //操作
            var result = RecommendManager.SaveMany(list);
            if (result > 0)
            {
                InsertSaveManyLog(modellist, PID1, PID2, PID3, Reason1, Reason2, Reason3, TireSize, VehicleIDS);
            }

            return Json(result);
        }

        public ActionResult ReplacePID(string OldPID, string NewPID, string Reason)
        {
            //日志beforevalue
            var model = RecommendManager.SelectTireRecommendByPID(OldPID);
            //替换
            var result = RecommendManager.ReplacePID(OldPID, NewPID, Reason);
            if (result > 0)
            {
                //记录日志
                InsertReplaceLog(model, OldPID, NewPID, Reason);
            }
            return Json(result);
        }
        static void InsertReplaceLog(IEnumerable<VehicleTireRecommend> model, string OldPID, string NewPID, string Reason)
        {
            var dic = model.GroupBy(C => C.Vehicleid + "|" + C.TireSize).ToDictionary(C => C.Key, C => C.ToList());
            var GUID = Guid.NewGuid().ToString();

            foreach (var item in dic)
            {
                var valueModel = new ForLogModel()
                {
                    Vehicleid = item.Value.FirstOrDefault().Vehicleid,
                    TireSize = item.Value.FirstOrDefault().TireSize,
                    OldPID1 = item.Value.FirstOrDefault(C => C.Postion == 2)?.PID,
                    OldPID2 = item.Value.FirstOrDefault(C => C.Postion == 4)?.PID,
                    OldPID3 = item.Value.FirstOrDefault(C => C.Postion == 7)?.PID,
                    OldReason1 = item.Value.FirstOrDefault(C => C.Postion == 2)?.Reason,
                    OldReason2 = item.Value.FirstOrDefault(C => C.Postion == 4)?.Reason,
                    OldReason3 = item.Value.FirstOrDefault(C => C.Postion == 7)?.Reason,

                    NewPID1 = item.Value.FirstOrDefault(C => C.Postion == 2)?.PID == OldPID ? NewPID : item.Value.FirstOrDefault(C => C.Postion == 2)?.PID,
                    NewPID2 = item.Value.FirstOrDefault(C => C.Postion == 4)?.PID == OldPID ? NewPID : item.Value.FirstOrDefault(C => C.Postion == 4)?.PID,
                    NewPID3 = item.Value.FirstOrDefault(C => C.Postion == 7)?.PID == OldPID ? NewPID : item.Value.FirstOrDefault(C => C.Postion == 7)?.PID,

                    NewReason1 = item.Value.FirstOrDefault(C => C.Postion == 2)?.PID == OldPID ? Reason : item.Value.FirstOrDefault(C => C.Postion == 2)?.Reason,
                    NewReason2 = item.Value.FirstOrDefault(C => C.Postion == 4)?.PID == OldPID ? Reason : item.Value.FirstOrDefault(C => C.Postion == 4)?.Reason,
                    NewReason3 = item.Value.FirstOrDefault(C => C.Postion == 7)?.PID == OldPID ? Reason : item.Value.FirstOrDefault(C => C.Postion == 7)?.Reason,
                    Group = GUID
                };
                AddOprLog(JsonConvert.SerializeObject(valueModel), "TireTJ", "批量替换", valueModel.Vehicleid + "|" + valueModel.TireSize);
            }
        }
        public ActionResult Delete(string TireSize, string VehicleID)
        {
            //日志beforevalue
            var model = RecommendManager.SelectTireRecommendByVehicleAndSize(TireSize, VehicleID);
            if (model == null || model.Count() == 0)
                return Json(-1);
            //删除
            var result = RecommendManager.Delete(TireSize, VehicleID);
            if (result > 0)
            {
                //记录日志   
                InsertDeleteLog(model);
            }
            return Json(result);
        }
        static void InsertDeleteLog(IEnumerable<VehicleTireRecommend> model)
        {
            var valueModel = new ForLogModel()
            {
                Vehicleid = model.FirstOrDefault().Vehicleid,
                TireSize = model.FirstOrDefault().TireSize,
                OldPID1 = model.FirstOrDefault(C => C.Postion == 2)?.PID,
                OldPID2 = model.FirstOrDefault(C => C.Postion == 4)?.PID,
                OldPID3 = model.FirstOrDefault(C => C.Postion == 7)?.PID,
                OldReason1 = model.FirstOrDefault(C => C.Postion == 2)?.Reason,
                OldReason2 = model.FirstOrDefault(C => C.Postion == 4)?.Reason,
                OldReason3 = model.FirstOrDefault(C => C.Postion == 7)?.Reason
            };
            AddOprLog(JsonConvert.SerializeObject(valueModel), "TireTJ", "删除", valueModel.Vehicleid + "|" + valueModel.TireSize);
        }


        public ActionResult ShowOprLog(string vehicleID, string tireSize, string Author)
        {

            var logs = LoggerManager.SelectOprLogByObjectTypeAndAftervalue("TireTJ", vehicleID + "|" + tireSize);

            var logData = logs.Select(c =>
        new VehicleTireRecommendLogModel()
        {
            Author = c.Author,
            ChangeDateTime = c.ChangeDatetime,
            Operation = c.Operation,
            LogValue = JsonConvert.DeserializeObject<ForLogModel>(c.BeforeValue)
        }
   );
            return View(logData);
        }


        #endregion

        #region 根据PID强制推荐

        public ActionResult QZTJIndex()
        {

            var tireSizes = RecommendManager.SelectALLTireSize();
            var brands = RecommendManager.GetBrands();
            return View(Tuple.Create(tireSizes, brands));
        }

        public PartialViewResult QZTJList(QZTJSelectModel QZTJ, int pageIndex = 1, int pageSize = 10)
        {
            PagerModel pager = new PagerModel(pageIndex, pageSize);
            var model = RecommendManager.SelectQZTJTires(QZTJ, pager);
            ViewBag.QZTJ = QZTJ;
            ViewBag.pageSize = pageSize;
            return PartialView(new ListModel<QZTJModel>() { Pager = pager, Source = model });
        }

        public ActionResult DeleteQZTJ(string PID)
        {
            var model = RecommendManager.SelectQZTJByPID(PID);
            if (!model.Any())
                return Json(-1);
            else
            {
                var result = RecommendManager.DeleteQZTJByPID(PID);
                if (result > 0)
                {
                    var oprValue = new BeforeValueForQZTJ()
                    {
                        PID = PID,
                        OldPID1 = model.FirstOrDefault(C => C.Postion == 1)?.PID,
                        OldPID2 = model.FirstOrDefault(C => C.Postion == 2)?.PID,
                        OldPID3 = model.FirstOrDefault(C => C.Postion == 3)?.PID,

                        OldReason1 = model.FirstOrDefault(C => C.Postion == 1)?.Reason,
                        OldReason2 = model.FirstOrDefault(C => C.Postion == 2)?.Reason,
                        OldReason3 = model.FirstOrDefault(C => C.Postion == 3)?.Reason,

                        OldImage1 = model.FirstOrDefault(C => C.Postion == 1)?.Image,
                        OldImage2 = model.FirstOrDefault(C => C.Postion == 2)?.Image,
                        OldImage3 = model.FirstOrDefault(C => C.Postion == 3)?.Image,


                    };
                    AddOprLog(JsonConvert.SerializeObject(oprValue), "TireQZTJ", "删除", PID);
                }
                return Json(result);
            }
        }

        public ActionResult CheckPIDQZTJ(string TireSize, string PID)
        {
            var result = RecommendManager.CheckPIDQZTJ(TireSize, PID);
            if (result == null)
                return Json(new { Status = 0, Message = "PID不存在或者与规格不匹配" });
            else
                return Json(new { Status = 1, Message = result });
        }

        public ActionResult SaveQZTJSingle(string PID, string RProduct)
        {

            var model = new QZTJModel()
            {
                PID = PID,
                Products = JsonConvert.DeserializeObject<List<RecommendProductModel>>(RProduct)
            };
            //修改前的数据
            var modelBefore = RecommendManager.SelectQZTJByPID(PID);
            var result = RecommendManager.SaveQZTJSingle(model);
            if (result > 0)
            {
                var oprValue = new BeforeValueForQZTJ()
                {
                    PID = PID,
                    NewPID1 = model.Products.FirstOrDefault(C => C.Postion == 1)?.PID,
                    NewPID2 = model.Products.FirstOrDefault(C => C.Postion == 2)?.PID,
                    NewPID3 = model.Products.FirstOrDefault(C => C.Postion == 3)?.PID,
                    NewReason1 = model.Products.FirstOrDefault(C => C.Postion == 1)?.Reason,
                    NewReason2 = model.Products.FirstOrDefault(C => C.Postion == 2)?.Reason,
                    NewReason3 = model.Products.FirstOrDefault(C => C.Postion == 3)?.Reason,
                    NewImage1 = model.Products.FirstOrDefault(C => C.Postion == 1)?.Image,
                    NewImage2 = model.Products.FirstOrDefault(C => C.Postion == 2)?.Image,
                    NewImage3 = model.Products.FirstOrDefault(C => C.Postion == 3)?.Image,
                };
                if (modelBefore.Any())
                {
                    oprValue.OldPID1 = modelBefore.FirstOrDefault(C => C.Postion == 1)?.PID;
                    oprValue.OldPID2 = modelBefore.FirstOrDefault(C => C.Postion == 2)?.PID;
                    oprValue.OldPID3 = modelBefore.FirstOrDefault(C => C.Postion == 3)?.PID;
                    oprValue.OldReason1 = modelBefore.FirstOrDefault(C => C.Postion == 1)?.Reason;
                    oprValue.OldReason2 = modelBefore.FirstOrDefault(C => C.Postion == 2)?.Reason;
                    oprValue.OldReason3 = modelBefore.FirstOrDefault(C => C.Postion == 3)?.Reason;
                    oprValue.OldImage1 = modelBefore.FirstOrDefault(C => C.Postion == 1)?.Image;
                    oprValue.OldImage2 = modelBefore.FirstOrDefault(C => C.Postion == 2)?.Image;
                    oprValue.OldImage3 = modelBefore.FirstOrDefault(C => C.Postion == 3)?.Image;
                    AddOprLog(JsonConvert.SerializeObject(oprValue), "TireQZTJ", "修改", PID);
                }
                else
                {
                    AddOprLog(JsonConvert.SerializeObject(oprValue), "TireQZTJ", "创建", PID);
                }
            }
            return Json(result);
        }


        public ActionResult SaveQZTJMany(string PIDS, string RProduct)
        {
            var list = new List<QZTJModel>();
            foreach (var PID in PIDS.Split(';'))
            {
                list.Add(new QZTJModel()
                {
                    PID = PID,
                    Products = JsonConvert.DeserializeObject<List<RecommendProductModel>>(RProduct)
                });
            }

            //日志
            var listBefore = RecommendManager.SelectQZTJByPIDs(PIDS);

            var result = RecommendManager.SaveQZTJMany(list);
            if (result > 0)
            {
                var GUID = Guid.NewGuid();
                foreach (var item in list)
                {
                    var oprValue = new BeforeValueForQZTJ()
                    {
                        PID = item.PID,
                        NewPID1 = item.Products.Any(P => P.Postion == 1) ? item.Products.FirstOrDefault(P => P.Postion == 1).PID : listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 1)?.RecommendPID,
                        NewPID2 = item.Products.Any(P => P.Postion == 2) ? item.Products.FirstOrDefault(P => P.Postion == 2).PID : listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 2)?.RecommendPID,
                        NewPID3 = item.Products.Any(P => P.Postion == 3) ? item.Products.FirstOrDefault(P => P.Postion == 3).PID : listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 3)?.RecommendPID,

                        NewReason1 = item.Products.Any(P => P.Postion == 1) ? item.Products.FirstOrDefault(P => P.Postion == 1).Reason : listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 1)?.Reason,
                        NewReason2 = item.Products.Any(P => P.Postion == 2) ? item.Products.FirstOrDefault(P => P.Postion == 2).Reason : listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 2)?.Reason,
                        NewReason3 = item.Products.Any(P => P.Postion == 3) ? item.Products.FirstOrDefault(P => P.Postion == 3).Reason : listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 3)?.Reason,

                        NewImage1 = item.Products.Any(P => P.Postion == 1) ? item.Products.FirstOrDefault(P => P.Postion == 1).Image : listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 1)?.Image,
                        NewImage2 = item.Products.Any(P => P.Postion == 2) ? item.Products.FirstOrDefault(P => P.Postion == 2).Image : listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 2)?.Image,
                        NewImage3 = item.Products.Any(P => P.Postion == 3) ? item.Products.FirstOrDefault(P => P.Postion == 3).Image : listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 3)?.Image,
                        Group = GUID.ToString()

                    };
                    if (listBefore.Any(p => p.PID == item.PID))
                    {
                        oprValue.OldPID1 = listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 1)?.RecommendPID;
                        oprValue.OldPID2 = listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 2)?.RecommendPID;
                        oprValue.OldPID3 = listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 3)?.RecommendPID;

                        oprValue.OldReason1 = listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 1)?.Reason;
                        oprValue.OldReason2 = listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 2)?.Reason;
                        oprValue.OldReason3 = listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 3)?.Reason;


                        oprValue.OldImage1 = listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 1)?.Image;
                        oprValue.OldImage2 = listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 2)?.Image;
                        oprValue.OldImage3 = listBefore.FirstOrDefault(P => P.PID == item.PID && P.Postion == 3)?.Image;




                        AddOprLog(JsonConvert.SerializeObject(oprValue), "TireQZTJ", "批量修改", item.PID);
                    }
                    else
                    {
                        AddOprLog(JsonConvert.SerializeObject(oprValue), "TireQZTJ", "批量创建", item.PID);
                    }
                }
            }
            return Json(result);
        }

        public ActionResult QZTJOprLog(string pid)
        {
            var logs = LoggerManager.SelectOprLogByObjectTypeAndAftervalue("TireQZTJ", pid);

            var logData = logs.Select(c =>
                  new QZTJLogModel()
                  {
                      Author = c.Author,
                      ChangeDateTime = c.ChangeDatetime,
                      Operation = c.Operation,
                      LogValue = JsonConvert.DeserializeObject<BeforeValueForQZTJ>(c.BeforeValue)
                  }
             );


            return View(logData);
        }

        #endregion


        #region 轮胎价格管理系统
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult Price()
        {
            var brands = RecommendManager.GetBrands();
            var tireSizes = RecommendManager.SelectALLTireSize();
            return View(Tuple.Create(brands, tireSizes));
        }
        [PowerManage]
        public ActionResult TirePriceReport()
        {
            var brands = RecommendManager.GetBrands();
            return View(Tuple.Create(brands));
        }
        [PowerManage]
        public ActionResult TirePriceCostReport()
        {
            var brands = RecommendManager.GetBrands();
            return View(Tuple.Create(brands));
        }
        public ActionResult GetStock(string pid)
        {
            var model = PriceManager.GetStock(pid);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFlashSalePriceByPID(string pid)
        {
            return Json(PriceManager.GetFlashSalePriceByPID(pid), JsonRequestBehavior.AllowGet);
        }
        public ActionResult IsLowerThanActivityPrice(string pid, decimal price)
        {
            var data = PriceManager.GetFlashSalePriceByPID(pid);
            if (data == null || !data.Any())
                return Json(1);
            else
                return Json(data.Any(_ => _.Price >= price) ? 0 : 1);
        }
        public PartialViewResult PriceProductList(PriceSelectModel selectModel, string pageType, int pageIndex = 1, int pageSize = 10)
        {
            PagerModel pager = new PagerModel(pageIndex, pageSize);
            var model = PriceManager.SelectPriceProductList(selectModel, pager);
            if (model != null && model.Count() > 0)
            {
                var pids = model.Select(g => g.PID).ToList();
                var activePrices = PriceManager.SelectActivePriceByPids(pids);
                var WhiteList = GetShowStatusByPids(pids);

                var monitoryDal = new CompetingProductsMonitorDAL();
                using (var conn = ProcessConnection.OpenConfigurationReadOnly)
                {
                    //根据Pids查询竞品中最低价商品
                    var monitorList = monitoryDal.GetProductsMonitorbyPids(conn, pids);

                    foreach (var pid in pids)
                    {
                        var rel = model.Where(g => g.PID == pid).First();
                        if (activePrices != null)
                        {
                            var val = activePrices.Where(g => g.PID == pid).FirstOrDefault();
                            rel.ActivePrice = val == null ? null : val.ActivePrice;
                        }
                        if (WhiteList != null)
                        {
                            var Status = WhiteList.Where(g => g.PID == pid).FirstOrDefault();
                            rel.ShowStatus = Status == null ? 0 : Status.ShowStatus;
                        }
                        if (monitorList.Count() > 0)
                        {
                            var monitorModel = monitorList.Where(g => g.Pid == pid).FirstOrDefault();
                            if (monitorModel != null)
                            {
                                rel.MinPrice = monitorModel.MinPrice;
                            }
                        }

                    }
                }
            }
            ViewBag.SelectModel = selectModel;
            ViewBag.PageType = pageType;
            return PartialView(new ListModel<TireListModel>() { Pager = pager, Source = model });
        }
        [PowerManage]
        public ActionResult ExportExcel(PriceSelectModel selectModel)
        {

            selectModel.MatchWarnLine = -99;
            var pagesize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ExportSize"]);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row = sheet1.CreateRow(0);
            var fileName = $"轮胎价格管理{ThreadIdentity.Operator.Name.Split('@')[0]}.xls";

            row.CreateCell(0).SetCellValue("品牌");
            row.CreateCell(1).SetCellValue("PID");
            row.CreateCell(2).SetCellValue("产品名称");
            row.CreateCell(3).SetCellValue("原配车型");
            row.CreateCell(4).SetCellValue("展示状态");
            row.CreateCell(5).SetCellValue("库存");
            row.CreateCell(6).SetCellValue("近7天销量");
            row.CreateCell(7).SetCellValue("近30天销量");
            row.CreateCell(8).SetCellValue("进货价");
            row.CreateCell(9).SetCellValue("最近一次采购价");
            row.CreateCell(10).SetCellValue("理论指导价");
            row.CreateCell(11).SetCellValue("实际指导价");
            row.CreateCell(12).SetCellValue("官网价格");
            row.CreateCell(13).SetCellValue("毛利率");
            row.CreateCell(14).SetCellValue("毛利额");
            row.CreateCell(15).SetCellValue("途虎淘宝");
            row.CreateCell(16).SetCellValue("途虎淘宝2");
            row.CreateCell(17).SetCellValue("途虎天猫1");
            row.CreateCell(18).SetCellValue("途虎天猫2");
            row.CreateCell(19).SetCellValue("途虎天猫3");
            row.CreateCell(20).SetCellValue("途虎天猫4");
            row.CreateCell(21).SetCellValue("途虎京东");
            row.CreateCell(22).SetCellValue("途虎京东旗舰");
            row.CreateCell(23).SetCellValue("汽配龙");
            row.CreateCell(24).SetCellValue("京东自营");
            row.CreateCell(25).SetCellValue("特维轮天猫");
            row.CreateCell(26).SetCellValue("汽车超人零售");
            row.CreateCell(27).SetCellValue("汽车超人批发");
            row.CreateCell(28).SetCellValue("劵后价");
            //row.CreateCell(29).SetCellValue("最低活动价");
            row.CreateCell(29).SetCellValue("汽配龙毛利额");
            row.CreateCell(30).SetCellValue("汽配龙毛利率");
            row.CreateCell(31).SetCellValue("工厂店毛利额");
            row.CreateCell(32).SetCellValue("工厂店毛利率");
            row.CreateCell(33).SetCellValue("采购在途");


            PagerModel pager = new PagerModel(1, pagesize);
            var list = PriceManager.SelectPriceProductList(selectModel, pager, true);
            var i = 0;
            while (list.Any())
            {
                var pids = list.Select(g => g.PID).ToList();
                //var activePrices = PriceManager.SelectActivePriceByPids(pids);
                var WhiteList = GetShowStatusByPids(pids);
                //var CaigouZaitu = PriceManager.SelectCaigouZaituByPids(pids);

                foreach (var item in list)
                {
                    //var val = activePrices.Where(g => g.PID == item.PID).FirstOrDefault();
                    var Status = WhiteList.Where(g => g.PID == item.PID).FirstOrDefault();
                    var guidePriceStr = item.cost == null ? "" : (item.cost.Value + item.cost.Value * item.JiaQuan / 100).ToString("0.00");
                    //item.ActivePrice = val == null ? null : val.ActivePrice;
                    item.ShowStatus = Status == null ? 0 : Status.ShowStatus;
                    //item.CaigouZaitu = CaigouZaitu?.Where(g => g.PID == item.PID).FirstOrDefault()?.CaigouZaitu;
                    var guidePriceStr_1 = "";
                    if (guidePriceStr == "")
                    {
                        if (item.JDSelfPrice != null)
                        {
                            guidePriceStr_1 = item.JDSelfPrice.Value.ToString("0.00");
                        }
                    }
                    else
                    {
                        if (item.JDSelfPrice == null)
                        {
                            guidePriceStr_1 = guidePriceStr;
                        }
                        else
                        {
                            if (Convert.ToDecimal(guidePriceStr) >= Convert.ToDecimal(item.JDSelfPrice))
                            {
                                guidePriceStr_1 = item.JDSelfPrice.Value.ToString("0.00");
                            }
                            else
                            {
                                guidePriceStr_1 = guidePriceStr;
                            }
                        }
                    }
                    var ShowStatus = "N/A";
                    if (item.ShowStatus == 1)
                    {
                        ShowStatus = "有货";
                    }
                    else if (item.ShowStatus == 2)
                    {
                        ShowStatus = "缺货";
                    }
                    else if (item.ShowStatus == 3)
                    {
                        ShowStatus = "不展示";
                    }

                    var QPLProfi = item.QPLPrice != null && item.cost != null ? (item.QPLPrice.Value - item.cost.Value).ToString("0.00") : "-";
                    var QPLProfitRate = item.cost != null && item.QPLPrice != null && item.QPLPrice.Value != 0 ? ((item.QPLPrice.Value - item.cost.Value) / item.QPLPrice.Value).ToString("0.00%") : "-";
                    var ShopProfit = item.Price > 0 && item.QPLPrice != null ? (item.Price - item.QPLPrice.Value).ToString("0.00") : "-";
                    var ShopProfitRate = item.Price > 0 && item.QPLPrice != null ? ((item.Price - item.QPLPrice.Value) / item.Price).ToString("0.00%") : "-";
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(++i);
                    rowtemp.CreateCell(0).SetCellValue(item.Brand);
                    rowtemp.CreateCell(1).SetCellValue(item.PID);
                    rowtemp.CreateCell(2).SetCellValue(item.ProductName);
                    rowtemp.CreateCell(3).SetCellValue(item.VehicleCount);
                    rowtemp.CreateCell(4).SetCellValue(ShowStatus);
                    rowtemp.CreateCell(5).SetCellValue(item.totalstock == null ? "" : item.totalstock.Value.ToString());
                    rowtemp.CreateCell(6).SetCellValue(item.num_week == null ? "" : item.num_week.Value.ToString());
                    rowtemp.CreateCell(7).SetCellValue(item.num_month == null ? "" : item.num_month.Value.ToString());
                    rowtemp.CreateCell(8).SetCellValue(item.cost == null ? "" : item.cost.Value.ToString("0.00"));
                    rowtemp.CreateCell(9).SetCellValue(item.PurchasePrice == null ? "" : item.PurchasePrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(10).SetCellValue(guidePriceStr);
                    rowtemp.CreateCell(11).SetCellValue(guidePriceStr_1);
                    rowtemp.CreateCell(12).SetCellValue(item.Price.ToString("0.00"));
                    rowtemp.CreateCell(13).SetCellValue(item.cost.GetValueOrDefault(0) > 0 && item.Price > 0 ? ((item.Price - item.cost.Value) / item.Price).ToString("0.00%") : "");
                    rowtemp.CreateCell(14).SetCellValue(item.cost.GetValueOrDefault(0) > 0 && item.Price > 0 ? (item.Price - item.cost.Value).ToString("0.00") : "");
                    rowtemp.CreateCell(15).SetCellValue(item.TBPrice == null ? "" : item.TBPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(16).SetCellValue(item.TB2Price == null ? "" : item.TB2Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(17).SetCellValue(item.TM1Price == null ? "" : item.TM1Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(18).SetCellValue(item.TM2Price == null ? "" : item.TM2Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(19).SetCellValue(item.TM3Price == null ? "" : item.TM3Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(20).SetCellValue(item.TM4Price == null ? "" : item.TM4Price.Value.ToString("0.00"));
                    rowtemp.CreateCell(21).SetCellValue(item.JDPrice == null ? "" : item.JDPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(22).SetCellValue(item.JDFlagShipPrice == null ? "" : item.JDFlagShipPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(23).SetCellValue(item.QPLPrice == null ? "" : item.QPLPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(24).SetCellValue(item.JDSelfPrice == null ? "" : item.JDSelfPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(25).SetCellValue(item.TWLTMPrice == null ? "" : item.TWLTMPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(26).SetCellValue(item.MLTTMPrice == null ? "" : item.MLTTMPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(27).SetCellValue(item.MLTPrice == null ? "" : item.MLTPrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(28).SetCellValue(item.CouponPrice == null ? "" : item.CouponPrice.Value.ToString("0.00"));
                    //rowtemp.CreateCell(29).SetCellValue(item.ActivePrice == null ? "" : item.ActivePrice.Value.ToString("0.00"));
                    rowtemp.CreateCell(29).SetCellValue(QPLProfi);
                    rowtemp.CreateCell(30).SetCellValue(QPLProfitRate);
                    rowtemp.CreateCell(31).SetCellValue(ShopProfit);
                    rowtemp.CreateCell(32).SetCellValue(ShopProfitRate);
                    rowtemp.CreateCell(33).SetCellValue(item.CaigouZaitu == null ? "" : item.CaigouZaitu.Value.ToString());
                }
                pager.CurrentPage += 1;
                list = PriceManager.SelectPriceProductList(selectModel, pager, true);
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }


        [PowerManage]
        public ActionResult PriceGuidePara(string type = "")
        {
            return View(new GuideViewModel()
            {
                Warn = type == "warn" ? PriceManager.SelectWarningLine() : null,
                Para = type == "warn" ? null : PriceManager.SelectGuideParaByType(type)
            });
        }

        public ActionResult SaveGuidePara(GuidePara model)
        {
            var result = PriceManager.SaveGuidePara(model);
            if (result > 0 && result != 99)
                AddOprLog(model.Value.GetValueOrDefault(0).ToString(), "TirePara", "修改", model.Type + "|" + HttpUtility.UrlDecode(model.Item));
            return Json(result);
        }

        public ActionResult SaveWarningLine(WarningLineModel model)
        {
            if (model.PKID > 0 && model.UpperLimit > 0 && model.LowerLimit > 0)
            {
                var result = PriceManager.UpdateWarningLine(model);
                if (result > 0)
                    AddOprLog(model.LowerLimit + "|" + model.UpperLimit, "TireWarn", "修改", model.PKID.ToString());
                return Json(result);
            }
            else
                return Json(-1);
        }
        public ActionResult GuideParaLog(string key)
        {
            var logs = LoggerManager.SelectOprLogByObjectTypeAndAftervalue("TirePara", key);
            return View(logs);
        }
        public ActionResult GuideWarnLog(int pkid, decimal min, decimal max)
        {
            ViewBag.guidePrice = string.Concat(min.ToString("0.00"), "-", max.ToString("0.00"));
            var logs = LoggerManager.SelectOprLogByObjectTypeAndAftervalue("TireWarn", pkid.ToString());
            return View(logs);
        }

        public async Task<ActionResult> UpdateListPrice(string pid, decimal price, string remark)
        {
            var result = await UpdatePriceAsync(pid, price, string.IsNullOrWhiteSpace(remark) ? "通过GLXT修改" : $"通过GLXT修改(备注：{remark})");

            return Json(result);
        }

        private async Task<int> UpdatePriceAsync(string pid, decimal price, string changeReason)
        {
            using (var client = new ProductClient())
            {
                var result = await client.UpdateProductPriceAsync(new Service.Product.Models.UpdateProductModel()
                {
                    Pid = pid,
                    Price = price,
                    ChangeReason = changeReason,
                    ChangeUser = ThreadIdentity.Operator.Name
                });
                result.ThrowIfException(true);
                return result.Result;
            }
        }

        public ActionResult ApplyUpdatePrice(ExamUpdatePriceModel model)
        {
            if (model == null)
                return Json(-99);
            model.ApplyPerson = ThreadIdentity.Operator.Name;
            var result = PriceManager.ApplyUpdatePrice(model);
            if (!Request.Url.Host.Contains(".tuhu.cn"))
                TuhuMessage.SendEmail("［待审批］轮胎价格修改申请", "wangxiaoyu1@tuhu.cn;diaojingwen@tuhu.cn;fanjuan@tuhu.cn", $"{ThreadIdentity.Operator.Name}于{DateTime.Now}申请将{model.PID}轮胎价格修改为{model.Price}元，超过系统预警线，请您审批！<br/><a href='http://setting.tuhu.cn/Tire/ExamPrice' target='_blank'>点此审核</a>");
            else
                TuhuMessage.SendEmail("［待审批］轮胎价格修改申请", "wangxiaoyu1@tuhu.cn;xujian@tuhu.cn;wanglidong@tuhu.cn;xuliyuan@tuhu.cn;wangzengliang@tuhu.cn", $"{ThreadIdentity.Operator.Name}于{DateTime.Now}申请将{model.PID}轮胎价格修改为{model.Price}元，超过系统预警线，请您审批！<br/><a href='http://setting.tuhu.cn/Tire/ExamPrice' target='_blank'>点此审核</a>");
            return Json(result);

        }

        //public ActionResult NoGuidePriceBeyond(string Brand,string TireSize,string Pattern,int AddOrJian,decimal Price)
        //{
        //    var result = PriceManager.NoGuidePriceBeyond(Brand, TireSize, Pattern, AddOrJian, Price);
        //    return Json(result);
        //}

        //public ActionResult UpdateListPriceBitch(string Brand, string TireSize, string Pattern, int AddOrJian, decimal Price)
        //{
        //    var result = PriceManager.UpdateListPriceBitch(Brand, TireSize, Pattern, AddOrJian, Price);
        //    return Json(result);
        //}

        [PowerManage]
        public ActionResult ExamPrice()
        {
            return View(PriceManager.SelectNeedExamTire());
        }

        public async Task<ActionResult> GotoExam(int pkid, string pid, decimal price, int type, decimal? cost, decimal? PurchasePrice, int? totalstock, int? num_week, int? num_month, decimal? guidePrice, decimal nowPrice, string maoliLv, string chaochu, decimal? jdself, decimal? maolie)
        {
            var resultExam = PriceManager.GotoExam(type > 0, ThreadIdentity.Operator.Name, pid, cost, PurchasePrice, totalstock, num_week, num_month, guidePrice, nowPrice, maoliLv, chaochu, jdself, maolie);
            var model = PriceManager.FetchPriceExam(pkid);
            if (type <= 0)
            {
                TuhuMessage.SendEmail("[被驳回]轮胎价格修改申请", model.ApplyPerson, $"您于{DateTime.Now}申请将{model.PID}轮胎价格修改为{model.Price}元，已被{model.ExamPerson}驳回。");
                return Json(resultExam);
            }
            int result = -99;
            if (resultExam > 0 && type > 0)
                result = await UpdatePriceAsync(pid, price, string.IsNullOrWhiteSpace(model.Remark) ? $"通过GLXT审核通过后修改(申请原因：{model.Reason})" : $"通过GLXT审核通过后修改(申请原因：{model.Reason})(备注：{model.Remark})");
            return Json(result);
        }

        public ActionResult ExamLog(string PID, int PageIndex = 1, int PageSize = 50)
        {
            if (string.IsNullOrWhiteSpace(PID))
                PID = null;
            PagerModel pager = new PagerModel(PageIndex, PageSize);
            var list = PriceManager.SelectExamLogByPID(PID, pager);
            ViewBag.PID = PID;
            return View(new ListModel<ExamUpdatePriceModel>() { Pager = pager, Source = list });
        }

        public ActionResult PriceChangeLog(string pid)
        {
            return View(PriceManager.PriceChangeLog(pid));
        }
        public ActionResult LookYuanPeiByPID(string pid) => Json(PriceManager.LookYuanPeiByPID(pid), JsonRequestBehavior.AllowGet);
        public ActionResult ZXTPrice(string pid)
        {
            var result = PriceManager.PriceChangeLog(pid).Where(c => c.ChangeDateTime > DateTime.Now.AddYears(-1)).OrderBy(c => c.ChangeDateTime).ToList();
            if (result.Any())
            {
                ViewBag.xArr = string.Join(",", result.Select(_ => _.ChangeDateTime.ToShortDateString()));
                ViewBag.yArr = string.Join(",", result.Select(_ => _.NewPrice.ToString()));
            }
            ViewBag.PID = pid;
            return View(result.OrderByDescending(_ => _.ChangeDateTime).ToList());
        }
        public ActionResult ZXTCost(string pid)
        {
            var result2 = new List<ZXTCost>();
            var result = PriceManager.GetZXTPurchaseByPID(pid).OrderBy(c => c.CreatedDatetime).ToList();
            if (result.Any())
            {
                var dic = result.GroupBy(_ => _.CreatedDatetime.ToShortDateString()).ToDictionary(_ => _.Key, _ => _.ToList());
                foreach (var item in dic)
                {
                    if (item.Value.Count == 1)
                        result2.AddRange(item.Value);
                    else
                    {
                        var tempModel = new ZXTCost()
                        {
                            CreatedDatetime = item.Value.FirstOrDefault().CreatedDatetime,
                            CostPrice = item.Value.Average(_ => _.CostPrice),
                            PID = item.Value.FirstOrDefault().PID,
                            Num = item.Value.Sum(_ => _.Num),
                            WareHouse = "多仓库"
                        };
                        result2.Add(tempModel);
                    }
                }
                if (result2.Any())
                {
                    ViewBag.xArr = string.Join(",", result2.Select(_ => $"({_.WareHouse + _.Num}条)   " + _.CreatedDatetime.ToShortDateString()));
                    ViewBag.yArr = string.Join(",", result2.Select(_ => _.CostPrice.ToString("0.00")));
                }
            }
            ViewBag.PID = pid;
            return View(result.OrderByDescending(_ => _.CreatedDatetime).ToList());
        }

        private int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        #endregion

        #region 花纹测评管理平台
        public ActionResult PatternComment()
        {
            var brands = RecommendManager.GetBrands();


            return View(Tuple.Create(brands));
        }
        public ActionResult GetPatternsByBrand(string brand)
        {
            var patterns = PatternManager.SelectPatternsByBrand(brand);
            return Json(patterns, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTireSizesByBrand(string brand)
        {
            var tiresizes = PriceManager.GetTireSizesByBrand(brand);
            return Json(tiresizes, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult PatternList(TirePatternModel model, int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            var data = PatternManager.SelectList(model, pager);
            ViewBag.Select = model;
            return PartialView(new ListModel<TirePatternModel>() { Pager = pager, Source = data });
        }

        [PowerManage]
        public ActionResult TireRecall()
        {
            return View();
        }


        public PartialViewResult TireRecallList(TireRecallModel model, int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            var data = TireRecallManager.SelectList(model, pager);
            ViewBag.Select = model;
            return PartialView(new ListModel<TireRecallModel>() { Pager = pager, Source = data });
        }


        /// <summary>
        /// 导出Excel模版
        /// </summary>
        /// <returns></returns>
        public FileResult ExportTiresRecallTemplate(TireRecallModel TireRecallmodel)
        {



            var statusText = GetStatusText(TireRecallmodel.Status);
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;
            row.CreateCell(cellNum++).SetCellValue("申诉编号");
            row.CreateCell(cellNum++).SetCellValue("订单号");
            row.CreateCell(cellNum++).SetCellValue("车牌号");
            row.CreateCell(cellNum++).SetCellValue("手机号");
            row.CreateCell(cellNum++).SetCellValue("行驶证照片");
            row.CreateCell(cellNum++).SetCellValue("轮胎DOT细节图");
            row.CreateCell(cellNum++).SetCellValue("轮胎和行驶证合照");
            row.CreateCell(cellNum++).SetCellValue("轮胎个数");
            row.CreateCell(cellNum++).SetCellValue("驳回原因");
            row.CreateCell(cellNum++).SetCellValue("申诉状态");
            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 50 * 256);
            sheet.SetColumnWidth(cellNum++, 100 * 256);
            sheet.SetColumnWidth(cellNum++, 50 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            //var pager = new PagerModel(pageIndex, pageSize);
            var result = TireRecallManager.SelectList(TireRecallmodel);
            if (result != null && result.Any())
            {
                int modelRowCount = 1;
                foreach (var model in result.Where(q => q != null))
                {
                    int modelCol = 0;
                    var modelRow = sheet.CreateRow(modelRowCount);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.PKID);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.OrderNo);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.CarNo);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.Mobile);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.VehicleLicenseImg);
                    modelRow.CreateCell(modelCol++).SetCellValue(string.Join(";", (model.TireDetailImg ?? string.Empty).Split(new char[] { ',' })));
                    modelRow.CreateCell(modelCol++).SetCellValue(model.TireAndLicenseImg);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.Num);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.Reason);
                    modelRow.CreateCell(modelCol++).SetCellValue(GetStatusText(model.Status));
                    modelRowCount++;
                }
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            if (!string.IsNullOrWhiteSpace(statusText))
            {
                statusText = statusText + "状态";
            }
            return File(ms.ToArray(), "application/x-xls", $"{statusText}普利司通轮胎数据 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");


        }


        public string GetStatusText(short status)
        {
            string statustext = "审核中";
            switch (status)
            {
                case 1:
                    statustext = "通过";
                    break;
                case 2:
                    statustext = "驳回";
                    break;
                case -1:
                    statustext = "";
                    break;
            }
            return statustext;
        }

        public JsonResult TireRecallAudit(long pkid, int status, string reason = default(string))
        {
            //审核通过
            if (status == 1 || status == 2)
            {
                var detail = TireRecallManager.FetchTireRecall(pkid);
                detail.Operator = ThreadIdentity.Operator.Name;
                detail.OperateType = status;
                if (detail == null)
                {
                    return Json(new
                    {
                        Code = 0
                    });
                }
                if (TireRecallManager.UpdateStatus(pkid, status, reason))
                {
                    detail.Reason = reason;
                    var order =
                        TireRecallManager.FetchSpecial_Bridgestone_Pidweekyear(
                            int.Parse(Regex.Replace(detail.OrderNo, @"[^\d]*", "")));

                    if (status == 1)
                    {

                        if (order != null)
                        {
                            //获取申诉手机号对应的userid
                            var userId = GetUserIdByMobile(detail.Mobile);
                            if (userId != null)
                            {
                                CreatePromotionCode(userId.Value, order, detail);
                                //[TODO] 发送成功短信
                                using (var client = new Tuhu.Service.Utility.SmsClient())
                                {
                                    client.SendSms(detail.Mobile, 158, order.UserName, "普利司通", "1", "30");
                                }
                            }
                        }

                    }
                    else
                    {

                        //[TODO] 发送驳回短信
                        using (var client = new Tuhu.Service.Utility.SmsClient())
                        {
                            client.SendSms(detail.Mobile, 159, order?.UserName, "普利司通", reason, "l.tuhu.cn/6CL3g");
                        }
                    }
                    TireRecallManager.InsertProductRecallLog(detail);
                    return Json(new
                    {
                        Code = 1
                    });
                }
            }

            return Json(new
            {
                Code = 0
            });
        }

        public ActionResult TireRecallHistoryModule(long pkid)
        {
            var loglist = TireRecallManager.GetTireRecallLog(pkid);
            return View(loglist.ToList());
        }

        bool CreatePromotionCode(Guid userId, Special_Bridgestone_Pidweekyear order, TireRecallModel recallDetail)
        {
            var promotion = new DataAccess.PromotionCode()
            {
                BatchId = order.orderid,
                CodeChannel = "普利司通轮胎召回",
                UserID = userId,
                StartTime = DateTime.Now.Date,
                EndTime = DateTime.Now.AddDays(30).Date,
                RuleID = 123299,
                PromotionName = "全网轮胎优惠券（限到店订单）",
                Description = "普利司通以旧换新 指定门店可用(点击使用优惠券查看门店明细)",
                Creater = "zhangjialing@tuhu.cn",
                DepartmentName = "销售",
                IntentionName = "售后赔付",
                Issuer = "zhangjialing@tuhu.cn",
                IssueChannle = "普利司通轮胎召回",
                IssueChannleId = nameof(TireController)
            };
            if (order.pid == "TR-BS-TECHNO|8")
            {
                promotion.MinMoney = 235 * recallDetail.Num;
                promotion.Discount = 235 * recallDetail.Num;
            }
            else if (order.pid == "TR-BS-TECHNO|12")
            {
                promotion.MinMoney = 269 * recallDetail.Num;
                promotion.Discount = 269 * recallDetail.Num;
            }
            else
            {
                promotion = null;
            }
            if (promotion != null)
            {
                return PromotionCodeManager.CreatePromotionCodeNew(promotion) > 0;
            }
            return false;
        }

        Guid? GetUserIdByMobile(string mobile)
        {
            Guid? userId = null;
            using (var client = new Tuhu.Service.UserAccount.UserAccountClient())
            {
                userId = client.GetUserByMobile(mobile)?.Result?.UserId;
            }
            if (userId == null)
            {
                using (var client = new Service.UserAccount.UserAccountClient())
                {
                    var response = client.CreateUserRequest(new Service.UserAccount.Models.CreateUserRequest()
                    {
                        ChannelIn = nameof(ChannelIn.H5),
                        UserCategoryIn = nameof(UserCategoryIn.Tuhu),
                        Profile = new Service.UserAccount.Models.UserProfile(),
                        MobileNumber = mobile,
                        IsMobileVerified = false
                    });
                    if (response.Success)
                    {
                        userId = response.Result?.UserId;
                    }
                }
            }
            return userId;
        }

        public ActionResult SaveAddMany(TirePatternModel model, string Patterns)
        {
            var modelPatterns = JsonConvert.DeserializeObject<IEnumerable<BrandToPatterns>>(Patterns);
            model.Patterns = modelPatterns;
            var result = PatternManager.SaveAddMany(model);
            if (result > 0)
            {
                var GUID = Guid.NewGuid();
                foreach (var brand in model.Patterns)
                {
                    foreach (var pattern in brand.Patterns.Split(','))
                    {
                        TirePatternModel tempModel = new TirePatternModel()
                        {
                            Image = model.Image,
                            Title = model.Title,
                            Describe = model.Describe,
                            Author = model.Author,
                            Date = model.Date,
                            ArticleLink = model.ArticleLink,
                            IsShow = false,
                            Brand = brand.Brand,
                            Pattern = pattern,
                            Group = GUID.ToString()
                        };
                        SavePatternArticleToCache(pattern);
                        AddOprLog(JsonConvert.SerializeObject(tempModel), "PatternPCe", "批量增加", brand.Brand + "|" + pattern);
                    }
                }

            }
            return Json(result);
        }
        static void SavePatternArticleToCache(string pattern)
        {
            string pid = PatternManager.SelectPIDByPattern(pattern);
            if (string.IsNullOrWhiteSpace(pid))
                return;
            var key = string.Concat("FetchPatternApi_", pid.Split('|')[0]);

            var data = PatternManager.SelectPatternForCache(pattern);
        }
        public ActionResult SaveUpdateOrAdd(TirePatternModel model)
        {
            var result = PatternManager.SaveUpdateOrAdd(model);
            if (result > 0)
            {
                SavePatternArticleToCache(model.Pattern);
                AddOprLog(JsonConvert.SerializeObject(model), "PatternPCe", model.PKID > 0 ? "修改" : "增加", model.Brand + "|" + HttpUtility.UrlDecode(model.Pattern));
            }
            return Json(result);
        }

        public ActionResult CanShow(TirePatternModel model)
        {
            return Json(PatternManager.CanShow(model), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PatternLog(string key)
        {
            var logs = LoggerManager.SelectOprLogByObjectTypeAndAftervalue("PatternPCe", key);
            return View(logs);
        }

        public ActionResult DeletePatternArticle(int PKID)
        {
            var model = PatternManager.FetchByPKID(PKID);
            if (model == null) return Json(-99);
            var result = PatternManager.DeletePatternArticle(PKID);
            if (result > 0)
            {
                SavePatternArticleToCache(model.Pattern);
                AddOprLog(JsonConvert.SerializeObject(model), "PatternPCe", "删除", model.Brand + "|" + HttpUtility.UrlDecode(model.Pattern));
            }
            return Json(result);
        }
        #endregion

        #region 轮胎列表页活动配置
        public ActionResult ListActivity()
        {
            TireViewModel model = new TireViewModel()
            {
                VehicleDepartment = RecommendManager.GetVehicleDepartment(),
                Brands = RecommendManager.GetBrands(),
                VehicleBodyTypes = RecommendManager.GetVehicleBodys(),
                TireSize = RecommendManager.SelectALLTireSize()
            };
            return View(model);
        }
        public ActionResult ImageUploadToAli(string filepath)
        {
            var result = "";
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var rd = new Random();
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    using (var client = new Tuhu.Service.Utility.FileUploadClient())
                    {
                        var res = client.UploadImage(new Service.Utility.Request.ImageUploadRequest()
                        {
                            Contents = buffer,
                            DirectoryName = "Tire",
                            MaxHeight = 1920,
                            MaxWidth = 1920,
                        });
                        if (res.Success && res.Result != null)
                        {
                            result = Tuhu.ImageHelper.GetImageUrl(res.Result);
                        }
                    }
                }
                catch (Exception exp)
                {
                    WebLog.LogException(exp);
                }
            }

            return Content(result);
        }
        public PartialViewResult ListActivityList(ListActCondition model, int PageSize = 10, int PageIndex = 1)
        {
            PagerModel pager = new PagerModel(PageIndex, PageSize);
            var list = ListActivityManager.SelectList(model, pager);
            ViewBag.selectModel = model;
            return PartialView(new ListModel<ActivityItem>() { Pager = pager, Source = list });
        }

        public ActionResult CheckGetRuleGUID(Guid? guid) => Json(guid == null ? -99 : ListActivityManager.CheckGetRuleGUID(guid), JsonRequestBehavior.AllowGet);//0无 -99无 1有


        [HttpPost]
        public ActionResult SaveBitchAdd(ActivityItem model, string Products, string VehicleTireSize)
        {
            if (!string.IsNullOrWhiteSpace(Products))
                model.Products = JsonConvert.DeserializeObject<IEnumerable<ActivityProducts>>(Products);
            model.VehicleIDTireSize = JsonConvert.DeserializeObject<IEnumerable<VehicleAndTireSize>>(VehicleTireSize);
            if (!model.VehicleIDTireSize.Any())
                return Json(-98);
            List<Guid> activityIDs;
            var result = ListActivityManager.SaveBitchAdd(model, out activityIDs);
            if (result > 0)
            {

                Guid GUID = Guid.NewGuid();
                foreach (var activityID in activityIDs)
                {
                    UpdateCacheListActivity(activityID);

                    var value = new ListActLog()
                    {
                        ActivityID = activityID,
                        ActivityName = model.ActivityName,
                        ButtonText = model.ButtonText,
                        ButtonType = model.ButtonType.Value,
                        EndTime = model.EndTime.Value,
                        GetRuleGUID = model.GetRuleGUID,
                        Icon = model.Icon,
                        Image = model.Image,
                        Image2 = model.Image2,
                        StartTime = model.StartTime.Value,
                        Status = model.Status.Value,
                        TireSize = model.TireSize,
                        VehicleId = model.VehicleId,
                        Products = model.Products,
                        GUID = GUID
                    };
                    AddOprLog(JsonConvert.SerializeObject(value), "T-ListAct", "批量添加活动", activityID.ToString());
                }
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult EditActivity(ActivityItem model, string Products)
        {
            if (!string.IsNullOrWhiteSpace(Products))
                model.Products = JsonConvert.DeserializeObject<IEnumerable<ActivityProducts>>(Products);

            var result = ListActivityManager.EditActivity(model);

            if (result > 0)
            {
                UpdateCacheListActivity(model.ActivityID.Value);
                var value = new ListActLog()
                {
                    ActivityID = model.ActivityID.Value,
                    ActivityName = model.ActivityName,
                    ButtonText = model.ButtonText,
                    ButtonType = model.ButtonType.Value,
                    EndTime = model.EndTime.Value,
                    GetRuleGUID = model.GetRuleGUID,
                    Icon = model.Icon,
                    Image = model.Image,
                    Image2 = model.Image2,
                    StartTime = model.StartTime.Value,
                    Status = model.Status.Value,
                    TireSize = model.TireSize,
                    VehicleId = model.VehicleId,
                    Products = model.Products
                };
                AddOprLog(JsonConvert.SerializeObject(value), "T-ListAct", "编辑", model.ActivityID.Value.ToString());
            }
            return Json(result);
        }
        public ActionResult SelectRelationPIDs(Guid ActivityID)
        {
            return Json(ListActivityManager.SelectRelationPIDs(ActivityID), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteListActivity(Guid ActivityID)
        {
            var tempModel = ListActivityManager.GetListActivityByID(ActivityID);
            var result = ListActivityManager.DeleteListActivity(ActivityID);
            if (result > 0)
            {
                UpdateCacheListActivity(ActivityID);
                var value = new ListActLog()
                {
                    ActivityID = tempModel.ActivityID.Value,
                    ActivityName = tempModel.ActivityName,
                    ButtonText = tempModel.ButtonText,
                    ButtonType = tempModel.ButtonType.Value,
                    EndTime = tempModel.EndTime.Value,
                    GetRuleGUID = tempModel.GetRuleGUID,
                    Icon = tempModel.Icon,
                    Image2 = tempModel.Image2,
                    Image = tempModel.Image,
                    StartTime = tempModel.StartTime.Value,
                    Status = tempModel.Status.Value,
                    TireSize = tempModel.TireSize,
                    VehicleId = tempModel.VehicleId,
                    Products = tempModel.Products
                };
                AddOprLog(JsonConvert.SerializeObject(value), "T-ListAct", "删除", ActivityID.ToString());
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult BitchOff(string ActivityIDs)
        {
            Guid GUID = Guid.NewGuid();
            List<ListActLog> logs = new List<ListActLog>();
            foreach (var activityID in ActivityIDs.Split(','))
            {
                var tempModel = ListActivityManager.GetListActivityByID(Guid.Parse(activityID));
                var value = new ListActLog()
                {
                    ActivityID = tempModel.ActivityID.Value,
                    ActivityName = tempModel.ActivityName,
                    ButtonText = tempModel.ButtonText,
                    ButtonType = tempModel.ButtonType.Value,
                    EndTime = tempModel.EndTime.Value,
                    GetRuleGUID = tempModel.GetRuleGUID,
                    Icon = tempModel.Icon,
                    Image2 = tempModel.Image2,
                    Image = tempModel.Image,
                    StartTime = tempModel.StartTime.Value,
                    Status = tempModel.Status.Value,
                    TireSize = tempModel.TireSize,
                    VehicleId = tempModel.VehicleId,
                    Products = tempModel.Products,
                    GUID = GUID
                };
                logs.Add(value);
            }

            var result = ListActivityManager.BitchOff(ActivityIDs);
            if (result > 0)
            {
                foreach (var activityID in ActivityIDs.Split(','))
                {
                    UpdateCacheListActivity(Guid.Parse(activityID));
                }
                foreach (var log in logs)
                {
                    AddOprLog(JsonConvert.SerializeObject(log), "T-ListAct", "批量禁用", log.ActivityID.ToString());
                }

            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult BitchOn(string ActivityIDs)
        {
            Guid GUID = Guid.NewGuid();
            List<ListActLog> logs = new List<ListActLog>();
            foreach (var activityID in ActivityIDs.Split(','))
            {
                var tempModel = ListActivityManager.GetListActivityByID(Guid.Parse(activityID));
                var value = new ListActLog()
                {
                    ActivityID = tempModel.ActivityID.Value,
                    ActivityName = tempModel.ActivityName,
                    ButtonText = tempModel.ButtonText,
                    ButtonType = tempModel.ButtonType.Value,
                    EndTime = tempModel.EndTime.Value,
                    GetRuleGUID = tempModel.GetRuleGUID,
                    Icon = tempModel.Icon,
                    Image2 = tempModel.Image2,
                    Image = tempModel.Image,
                    StartTime = tempModel.StartTime.Value,
                    Status = tempModel.Status.Value,
                    TireSize = tempModel.TireSize,
                    VehicleId = tempModel.VehicleId,
                    Products = tempModel.Products,
                    GUID = GUID
                };
                logs.Add(value);
            }
            var result = ListActivityManager.BitchOn(ActivityIDs);
            if (result > 0)
            {
                foreach (var activityID in ActivityIDs.Split(','))
                {
                    UpdateCacheListActivity(Guid.Parse(activityID));
                }
                foreach (var log in logs)
                {
                    AddOprLog(JsonConvert.SerializeObject(log), "T-ListAct", "批量禁用", log.ActivityID.ToString());
                }

            }
            return Json(result);
        }

        public ActionResult GetListActivityByID(Guid ActivityID)
        {
            return Json(ListActivityManager.GetListActivityByID(ActivityID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListActivityLog(string key)
        {
            var logs = LoggerManager.SelectOprLogByObjectTypeAndAftervalue("T-ListAct", key);
            return View(logs);
        }


        public bool UpdateCacheListActivity(Guid activityID)
        {
            var model = ListActivityManager.GetListActivityByID(activityID);
            using (var client = new ActivityClient())
            {
                var result1 = client.UpdateTireActivityCache(model.VehicleId, model.TireSize);
                result1.ThrowIfException(true);
                var result2 = client.UpdateActivityPidsCache(activityID);
                result2.ThrowIfException(true);
                return result2.Result;
            }
        }


        public ActionResult ReplaceListActivityItem(string ActivityName, string Image, string Icon, string Image2, string ButtonText, string ActivityIDs)
        {
            ActivityName = string.IsNullOrWhiteSpace(ActivityName) ? null : ActivityName;
            ButtonText = string.IsNullOrWhiteSpace(ButtonText) ? null : ButtonText;
            Icon = string.IsNullOrWhiteSpace(Icon) ? null : Icon;
            Image2 = string.IsNullOrWhiteSpace(Image2) ? null : Image2;
            Image = string.IsNullOrWhiteSpace(Image) ? null : Image;
            var result = ListActivityManager.ReplaceListActivityItem(ActivityName, Image, Icon, Image2, ButtonText, ActivityIDs);
            if (result > 0)
            {
                foreach (var aid in ActivityIDs.Split(';'))
                {
                    var value = new ListActLog()
                    {
                        ActivityID = Guid.Parse(aid),
                        ActivityName = ActivityName,
                        ButtonText = ButtonText,
                        Icon = Icon,
                        Image = Image,
                    };
                    AddOprLog(JsonConvert.SerializeObject(value), "T-ListAct", "批量替换", aid);
                }
                Thread.Sleep(1000);
                foreach (var aid in ActivityIDs.Split(';'))
                    UpdateCacheListActivity(Guid.Parse(aid));
            }
            return Json(result);
        }
        #endregion

        #region 可立即安装轮胎配置

        public ActionResult InstallNowIndex()
        {
            var tireSizes = RecommendManager.SelectALLTireSize();
            return View(Tuple.Create(tireSizes));
        }

        public PartialViewResult InstallNowList(InstallNowConditionModel model, int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            var data = InstallNowManager.SelectList(model, pager);
            ViewBag.Select = model;
            return PartialView(new ListModel<InstallNowModel>() { Pager = pager, Source = data });
        }
        [HttpPost]
        public ActionResult FetchDisPlayNameByPID(String pid)
        {
            if (!pid.Contains("|"))
                return Json(null);
            else
                return Json(InstallNowManager.FetchDisPlayNameByPID(pid));
        }
        public ActionResult DeleteInstallNow(int pkid)
        {
            var model = InstallNowManager.SelectInstallNowByPKIDS(pkid.ToString()).FirstOrDefault();
            var result = InstallNowManager.DeleteInstallNow(pkid);
            if (result > 0)
            {
                var log = new InstallNowModel()
                {
                    CityId = model.CityId,
                    PID = model.PID,
                    Status = model.Status
                };
                AddOprLog(JsonConvert.SerializeObject(log), "InstallNow", "删除", log.CityId + ";" + log.PID);
                Thread.Sleep(1000);
                ReflashInstallNowCache(model.CityId);
            }
            return Json(result);
        }
        public ActionResult InstallNowBitchOn(string PKIDS)
        {
            if (!string.IsNullOrWhiteSpace(PKIDS))
            {
                var list = InstallNowManager.SelectInstallNowByPKIDS(PKIDS);
                list = list.Where(_ => !_.Status);
                if (!list.Any())
                    return Json(-2);//无需启用，已经是启用
                Guid guid = Guid.NewGuid();

                var result = InstallNowManager.BitchOn(list);
                List<int> ids = new List<int>();
                foreach (var item in list)
                {
                    var log = new InstallNowModel()
                    {
                        CityId = item.CityId,
                        PID = item.PID,
                        Status = item.Status,
                        LogBitGroup = guid
                    };
                    AddOprLog(JsonConvert.SerializeObject(log), "InstallNow", "批量启用", log.CityId + ";" + log.PID);
                    if (!ids.Contains(item.CityId))
                        ids.Add(item.CityId);
                }
                if (ids.Any())
                {
                    Thread.Sleep(1000);
                    foreach (var id in ids)
                    {
                        ReflashInstallNowCache(id);
                    }
                }
                return Json(result);
            }
            return Json(-1);
        }

        public ActionResult InstallNowBitchOff(string PKIDS)
        {
            if (!string.IsNullOrWhiteSpace(PKIDS))
            {
                var list = InstallNowManager.SelectInstallNowByPKIDS(PKIDS);
                list = list.Where(_ => _.Status);
                if (!list.Any())
                    return Json(-99);//无需禁用，已经是禁用
                Guid guid = Guid.NewGuid();

                var result = InstallNowManager.BitchOff(string.Join(",", list.Select(_ => _.PKID)));
                List<int> ids = new List<int>();
                foreach (var item in list)
                {
                    var log = new InstallNowModel()
                    {
                        CityId = item.CityId,
                        PID = item.PID,
                        Status = item.Status,
                        LogBitGroup = guid
                    };
                    AddOprLog(JsonConvert.SerializeObject(log), "InstallNow", "批量禁用", log.CityId + ";" + log.PID);
                    if (!ids.Contains(item.CityId))
                        ids.Add(item.CityId);
                }
                if (ids.Any())
                {
                    Thread.Sleep(1000);
                    foreach (var id in ids)
                    {
                        ReflashInstallNowCache(id);
                    }
                }
                return Json(result);
            }
            return Json(-1);
        }
        static void AddOprLog(string ValueJson, string ObjectType, string Operation, string afterValue = null)
        {
            var oprLog = new Tuhu.Provisioning.DataAccess.Entity.OprLog();
            oprLog.Author = ThreadIdentity.Operator.Name;
            oprLog.ChangeDatetime = DateTime.Now;
            oprLog.BeforeValue = ValueJson;
            oprLog.AfterValue = afterValue;
            oprLog.ObjectType = ObjectType;
            oprLog.Operation = Operation;
            new OprLogManager().AddOprLog(oprLog);
        }
        [HttpPost]
        public ActionResult SaveInstallNow(string cityIds, string pidObj)
        {
            var PIDS = JsonConvert.DeserializeObject<IEnumerable<DataAccess.Entity.Tire.PidModel>>(pidObj);
            if (!string.IsNullOrWhiteSpace(cityIds) && PIDS.Any())
            {
                var result = InstallNowManager.SaveInstallNow(cityIds, PIDS);
                if (result.IsSuccess)
                {
                    List<int> ids = new List<int>();
                    foreach (var item in result.SuccessItem)
                    {
                        var log = new InstallNowModel()
                        {
                            CityId = item.CityId,
                            PID = item.PID,
                            Status = item.Status
                        };

                        AddOprLog(JsonConvert.SerializeObject(log), "InstallNow", "添加", log.CityId + ";" + log.PID);
                        if (!ids.Contains(item.CityId))
                            ids.Add(item.CityId);
                    }
                    if (ids.Any())
                    {
                        Thread.Sleep(1000);
                        foreach (var id in ids)
                        {
                            ReflashInstallNowCache(id);
                        }
                    }
                }
                return Json(result);
            }
            return Json(new ResultModel() { IsSuccess = false, ReturnCode = -3, ReturnMessage = "城市和PID不可为空" });
        }

        public ActionResult ShowInstallNowOprLog(int cityId, string pid, string tireSize, string province, string city)
        {
            var logs = LoggerManager.SelectOprLogByObjectTypeAndAftervalue("InstallNow", cityId + ";" + pid);


            return View(Tuple.Create(logs, new Dictionary<string, string>() { { "tireSize", tireSize }, { "province", province }, { "city", city } }));
        }

        static bool ReflashInstallNowCache(int cityId)
        {
            using (var client = new Tuhu.Service.Config.CacheClient())
            {
                var result = client.RefreshInstallNowConfig(cityId);
                result.ThrowIfException(true);
                return result.Result;
            }
        }
        #endregion

        #region 安装费加价
        public ActionResult InstallFee()
        {
            var brands = RecommendManager.GetBrands();
            var tireSizes = RecommendManager.SelectALLTireSize();
            return View(Tuple.Create(brands, tireSizes));
        }

        public PartialViewResult InstallFeeList(InstallFeeConditionModel condition, int pageIndex = 1, int pageSize = 10)
        {
            PagerModel pager = new PagerModel(pageIndex, pageSize);
            var data = InstallFeeManager.SelectInstallFeeList(condition, pager);
            return PartialView(Tuple.Create(new ListModel<InstallFeeModel>(pager, data), condition));
        }

        public ActionResult SelectInstallNowTest(string pids, int cityId)
        {
            using (var client = new ConfigClient())
            {
                var result = client.GetInstallNowConfigPids(pids.Split(';'), cityId);
                result.ThrowIfException(true);
                return Json(result.Result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult InstallFeeEdit(string Pid, decimal AddPrice, decimal OldPrice)
        {
            if (OldPrice <= 0 && AddPrice <= 0)
                return Json(1);
            var result = InstallFeeManager.InstallFeeEdit(Pid, AddPrice, OldPrice > 0);
            if (result > 0)
            {
                ReflashInstallFeeCache(new List<string>() { Pid });
                AddOprLog(AddPrice.ToString("0.00"), "AddTireFee", AddPrice > 0 ? "编辑" : "删除", Pid);
            }
            return Json(result);
        }

        public ActionResult InstallFeeBitchEdit(decimal AddPrice, string json)
        {
            var list = JsonConvert.DeserializeObject<IEnumerable<PIDandPrice>>(json);
            List<string> errorPids = new List<string>();
            foreach (var item in list)
            {
                if (item.OldPrice <= 0 && AddPrice <= 0)
                    continue;
                var result = InstallFeeManager.InstallFeeEdit(item.PID, AddPrice, item.OldPrice > 0);
                if (result > 0)
                    AddOprLog(AddPrice.ToString("0.00"), "AddTireFee", AddPrice > 0 ? "批量编辑" : "删除(批量编辑)", item.PID);
                else
                    errorPids.Add(item.PID);
            }
            ReflashInstallFeeCache(list.Select(_ => _.PID));
            if (errorPids.Any())
                return Json(new { code = 0, list = errorPids });
            else
                return Json(new { code = 1, list = errorPids });
        }

        public ActionResult InstallFeeLog(string key)
        {
            var logs = LoggerManager.SelectOprLogByObjectTypeAndAftervalue("AddTireFee", key);
            return View(logs);
        }

        public bool ReflashInstallFeeCache(IEnumerable<string> pids)
        {
            Thread.Sleep(1000);

            using (var client = new Service.Config.CacheClient())
            {
                var result = client.RefreshAddInstallFeeConfig(pids);
                result.ThrowIfException(true);
            }
            var pids_temp = InstallFeeManager.SelectPackagePIDs(pids);
            pids = pids.Union(pids_temp).Distinct();
            TuhuNotification.SendNotification("ProductModify", new Dictionary<string, object>
            {
                ["pids"] = pids,
                ["type"] = "UpsertProduct"
            }, 1000 * 6);

            using (var client2 = new Service.Product.CacheClient())
            {
                var result = client2.RefreshInstallFeeCache(pids.ToList());
                result.ThrowIfException(true);
                return result.Result;
            }
        }
        public ActionResult SelectInstallFeeCache(string pids)
        {
            using (var client = new Service.Config.ConfigClient())
            {
                var result = client.SelectAddInstallFeeByPids(pids.Split(';'));
                result.ThrowIfException(true);
                return Json(result.Result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SelectProductInstallFee(string pids, int num)
        {
            using (var client = new Service.Product.ProductClient())
            {
                var request = new List<ProductInstallFeeRequest>();
                foreach (var item in pids.Split(';'))
                {
                    request.Add(new ProductInstallFeeRequest() { PID = item, Quantity = num });
                }
                var result = client.SelectInstallFeeByPids(request);
                result.ThrowIfException(true);
                return Json(result.Result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 缺货状态查询
        public ActionResult TireStockout()
        {
            return View(RecommendManager.SelectALLTireSize());
        }
        public PartialViewResult TireStockoutList(StockoutStatusRequest request, string Address, int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            var data = StockoutStatusManager.SelectList(request, pager);
            ViewBag.Select = request;
            ViewBag.Address = Address;
            return PartialView(new ListModel<StockoutStatusModel>() { Pager = pager, Source = data });
        }
        #endregion

        #region 缺货状态白名单
        //public ActionResult TireStockoutWhite()
        //{
        //    ViewBag.PID = "";
        //    return View(RecommendManager.SelectALLTireSize());
        //}
        public ActionResult TireStockoutWhite(string pid)
        {
            if (string.IsNullOrEmpty(pid))
            {
                ViewBag.PID = "";
            }
            else
            {
                ViewBag.PID = pid;
            }
            return View(RecommendManager.SelectALLTireSize());
        }
        public PartialViewResult TireStockoutWhiteList(WhiteRequest model, string Address, int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            var data = StockoutStatusManager.SelectWhiteList(model, pager);
            ViewBag.PID = model.PID;
            ViewBag.Address = Address;
            ViewBag.Request = model;
            return PartialView(new ListModel<StockoutStatusWhiteModel>() { Pager = pager, Source = data });
        }

        public IEnumerable<ShowStatusModel> GetShowStatusByPids(List<string> pids)
        {
            return StockoutStatusManager.GetShowStatusByPids(pids);
        }

        public ActionResult SaveTireStockWhite(string pids)
        {
            var result = StockoutStatusManager.SaveTireStockWhite(pids);
            if (result > 0)
            {
                foreach (var pid in pids.Split(';'))
                    AddOprLog(null, "StockWhite", "加入白名单(人工)", pids);
                ReflashStockoutStatusWhite();
            }
            return Json(result);
        }
        public ActionResult RemoveWhite(string pid)
        {
            var result = StockoutStatusManager.RemoveWhite(pid);
            if (result > 0)
            {
                AddOprLog(null, "StockWhite", "移除白名单(人工)", pid);
                ReflashStockoutStatusWhite();
            }
            return Json(result);
        }
        private bool ReflashStockoutStatusWhite()
        {
            Thread.Sleep(1000);
            using (var client = new Service.Product.CacheClient())
            {
                var result = client.RefreshTireStockoutStatusWhiteListCache();
                result.ThrowIfException(true);
                return result.Result;
            }
        }

        public ActionResult SelectTireStockoutStatusWhiteList()
        {
            using (var client = new ProductClient())
            {
                var result = client.SelectTireStockoutStatusWhiteList();
                result.ThrowIfException(true);
                return Json(result.Result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult StockoutWhiteOprLog(string pid)
        {

            var logs = LoggerManager.SelectOprLogByObjectTypeAndAftervalue("StockWhite", pid);
            return View(logs);
        }
        #endregion

        #region 新增券后价相关
        public ActionResult CouponPriceChangeLog(string pid)
        {
            return View(PriceManager.CouponPriceChangeLog(pid));
        }
        //public 

        public int UpdateCouponPrice(string pid, decimal? oldprice, decimal price)
        {
            using (var client = new Tuhu.Service.Product.ProductConfigClient())
            {
                var result = client.SelectProductExtraPropertiesByPid(pid);
                if (result.Result != null && !result.Result.CanUseCoupon)
                {
                    return -3;
                }
            }
            var data = PriceManager.UpdateCouponPrice(new CouponPriceHistory()
            {
                PID = pid,
                OldPrice = oldprice,
                NewPrice = price,
                ChangeUser = ThreadIdentity.Operator.Name,
                ChangeReason = "手动修改"
            });
            if (data > 0)
            {
                TuhuNotification.SendNotification("ProductModify", new Dictionary<string, object>
                {
                    ["pid"] = pid,
                    ["type"] = "UpsertProduct"
                }, 3000);
            }
            return data;
        }

        /// <summary>
        /// 新增劵后价申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int ApplyUpdateCouponPrice(PriceApplyRequest model)
        {
            var applyperson = ThreadIdentity.Operator.Name;
            var pid = model.PID;
            using (var client = new Tuhu.Service.Product.ProductConfigClient())
            {
                var result = client.SelectProductExtraPropertiesByPid(pid);
                if (result.Result != null && !result.Result.CanUseCoupon)
                {
                    return -3;
                }
            }
            return PriceManager.ApplyUpdateCouponPrice(model, applyperson);
        }

        /// <summary>
        /// 获得所有券后价申请
        /// </summary>
        /// <returns></returns>

        [PowerManage]
        public ActionResult GetCouponPriceApply()
        {
            return View(PriceManager.GetCouponPriceApply());
        }

        /// <summary>
        /// 券后价审核结果设置
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpPost]
        public int ApprovalCouponPrice(int PKID, bool pass)
        {
            var data = PriceManager.ApprovalCouponPrice(PKID, pass, ThreadIdentity.Operator.Name);
            if (pass == true && data != null)
            {
                TuhuNotification.SendNotification("ProductModify", new Dictionary<string, object>
                {
                    ["pid"] = data,
                    ["type"] = "UpsertProduct"
                }, 3000);
            }
            if (data == null)
            {
                return 0;
            }
            else
            {
                return 1;
            }

        }
        [PowerManage]
        public ActionResult CouponPriceApprovalLog(string PID, int PageIndex = 1, int PageSize = 50)
        {
            if (string.IsNullOrWhiteSpace(PID))
                PID = null;
            PagerModel pager = new PagerModel(PageIndex, PageSize);
            var list = PriceManager.CouponPriceApprovalLog(PID, pager);
            ViewBag.PID = PID;
            return View(new ListModel<PriceApproval>() { Pager = pager, Source = list });
        }

        public int DeleteCouponPrice(string pid, decimal? price)
        {
            var data = PriceManager.DeleteCouponPrice(pid, ThreadIdentity.Operator.Name, price);
            if (data > 0)
            {
                TuhuNotification.SendNotification("ProductModify", new Dictionary<string, object>
                {
                    ["pid"] = pid,
                    ["type"] = "UpsertProduct"
                }, 3000);
            }
            return data;
        }
        #endregion


        public ActionResult TireInsuranceYearsConfig()
        {
            var brands = RecommendManager.GetBrands();
            var tireSizes = RecommendManager.SelectALLTireSize();
            return View(Tuple.Create(brands, tireSizes));
        }

        public async Task<PartialViewResult> TireInsuranceYearsList(InstallFeeConditionModel condition, int pageIndex = 1, int pageSize = 10)
        {
            PagerModel pager = new PagerModel(pageIndex, pageSize);
            var data = await TireInsuranceYearsManager.SelectInstallFeeListAsync(condition, pager);
            return PartialView(Tuple.Create(new ListModel<TireInsuranceYearModel>(pager, data), condition));
        }

        [HttpPost]
        public async Task<JsonResult> TireInsuranceYearsEdit(IEnumerable<string> pids, int years)
        {
            var issuccess = true;
            JsonResult jr = new JsonResult();
            try
            {
                if (pids != null && pids.Any())
                {
                    Dictionary<string, string> beforeDict = new Dictionary<string, string>();
                    using (var client = new Tuhu.Service.Product.ProductConfigClient())
                    {
                        var result = client.SelectProductExtraPropertiesByPids(pids);
                        result.ThrowIfException(true);
                        var models = new List<ProductExtraProperties>();
                        foreach (var pid in pids)
                        {
                            var item = result.Result.FirstOrDefault(x => x.PID == pid) ?? new ProductExtraProperties()
                            {
                                PID = pid,
                                TireInsuranceYears = 1
                            };
                            beforeDict[pid] = item.TireInsuranceYears.ToString();
                            item.TireInsuranceYears = years;
                            models.Add(item);
                        }

                        var logs = models.Select(x => new TireModifyConfigLog()
                        {
                            UserId = HttpContext.User.Identity.Name,
                            Before = beforeDict.ContainsKey(x.PID) ? beforeDict[x.PID] : "1",
                            After = years.ToString(),
                            Pid = x.PID,
                            Type = "TireInsuranceYears"
                        });
                        var updateresult = client.BatchCreateOrUpdateProductExtraProperties(models);
                        updateresult.ThrowIfException(true);
                        issuccess = updateresult.Success && updateresult.Result > 0;
                        await TireInsuranceYearsManager.WriteLogsAsync(logs);
                    }
                    jr.Data = new { code = issuccess ? 1 : 0, msg = issuccess ? "保存成功" : "保存失败" };
                }
                else
                {
                    jr.Data = new { code = 0, msg = "pid不能为空" };
                }
            }
            catch (System.Exception ex)
            {
                jr.Data = new { code = 0, msg = ex.Message };
            }

            return jr;
        }

        [HttpGet]
        public async Task<ActionResult> ShowTireModifyLogs(string pid, string type)
        {
            var logs = await TireInsuranceYearsManager.SelectLogsAsync(pid, type);
            return View(logs?.ToList() ?? new List<TireModifyConfigLog>());
        }



        /// <summary>
        /// 区域库存查询
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectRegionStock()
        {
            var tireSizes = RecommendManager.SelectALLTireSize();
            ViewData["tireSizes"] = tireSizes;
            return View();
        }
        public JsonResult GetRegion()
        {
            return Json(StockoutStatusManager.GetAllRegion(), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult SelectRegionStockList(RegionStockRequest model, string Address, int PageIndex = 1, int PageSize = 10)
        {
            var pager = new PagerModel(PageIndex, PageSize);
            ViewBag.Address = Address;
            ViewBag.Request = model;
            var data = StockoutStatusManager.SelectRegionStockList(model, pager);
            return PartialView(new ListModel<RegionStockModel>() { Pager = pager, Source = data });
        }

        public ActionResult GetTireCount(TiresOrderRecordRequestModel model)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            using (var activityclient = new ActivityClient())
            {
                var result = activityclient.SelectTiresOrderRecord(model);
                if (result != null && result.Result != null)
                {
                    dic = result.Result;
                }
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateTireCount(TiresOrderRecordRequestModel model)
        {
            bool isupdatSuccess = true;
            using (var activityclient = new ActivityClient())
            {
                var result = activityclient.RedisTiresOrderRecordReconStruction(model);
                if (result != null)
                {
                    isupdatSuccess = result.Result;
                }
            }
            return Json(isupdatSuccess, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 区域库存查询
        /// </summary>
        /// <returns></returns>
        public ActionResult TireCountManage()
        {
            return View();
        }
        /// <summary>
        /// 天降神券黑名单管理
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        [PowerManage]
        public ActionResult CouponBlackList()
        {
            return View();
        }
        public PartialViewResult CouponBlackList2(string phoneNum, int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            ViewBag.Search = phoneNum;
            var data = PatternManager.GetCouponBlackList(pager, phoneNum);
            return PartialView(new ListModel<CouponBlackItem>() { Pager = pager, Source = data });
        }
        [PowerManage]
        public int AddCouponBlackList(string phoneNums)
        {
            if (string.IsNullOrWhiteSpace(phoneNums))
                return 0;
            return PatternManager.AddCouponBlackList(phoneNums);
        }
        [PowerManage]
        public int DeleteCouponBlackList(string phoneNum)
        {
            if (string.IsNullOrWhiteSpace(phoneNum))
                return 0;
            return PatternManager.DeleteCouponBlackList(phoneNum);
        }

        #region 限购黑名单

        [PowerManage]
        public ActionResult TireBlackList()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TireBlackList2(string BlackNumber, int pageIndex = 1, int pageSize = 10, int Type = 1)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            ViewBag.BlackNumber = BlackNumber;
            ViewBag.Type = Type;
            var data = PatternManager.GetTireBlackList(BlackNumber, Type, pager);
            return View(new ListModel<TireBlackListItem>() { Pager = pager, Source = data });
        }
        [PowerManage]
        [HttpPost]
        public int AddTireBlackList(string BlackNumber, int Type)
        {
            if (string.IsNullOrWhiteSpace(BlackNumber) || Type < 0 || Type > 4)
            {
                return 3;
            }
            var dat = PatternManager.CheckTireBlackListItem(BlackNumber, Type);
            if (dat)
            {
                return 2;
            }
            var result = PatternManager.AddTireBlackListItem(BlackNumber, Type);
            if (result)
            {
                PatternManager.AddTireBlackListLog(BlackNumber, ThreadIdentity.Operator.Name, 1);
                return 1;
            }
            return 0;
        }
        [PowerManage]
        [HttpPost]
        public int DeleteTireBlackListItem(string BlackNumber, int Type = 1)
        {
            if (string.IsNullOrWhiteSpace(BlackNumber))
            {
                return 0;
            }
            var result = PatternManager.DeleteTireBlackListItem(BlackNumber, Type);
            if (result > 0)
            {
                PatternManager.AddTireBlackListLog(BlackNumber, ThreadIdentity.Operator.Name, 2);
                return 1;
            }
            return 0;
        }
        [HttpPost]
        public JsonResult BlackListItemHistory(string BlackNumber)
        {
            var result = new List<TireBlackListLog>();
            if (!string.IsNullOrWhiteSpace(BlackNumber))
            {
                result = PatternManager.BlackListItemHistory(BlackNumber).ToList();
            }
            return Json(result);
        }

        [HttpPost]
        public PartialViewResult TireBlackList3(string BlackNumber, int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            ViewBag.BlackNumber = BlackNumber;
            var data = PatternManager.SelectTireBlackListLog(BlackNumber, pager);
            return PartialView(new ListModel<TireBlackListLog>() { Pager = pager, Source = data });
        }


        public JsonResult ImportBlackListData()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (!file.FileName.Contains(".xlsx") && !file.FileName.Contains(".xls"))
                    return Json(new { Code = -1, Info = "请上传.xlsx文件或者.xls文件！" });
                var excel = new Controls.ExcelHelper(file.InputStream, file.FileName);
                var dt = excel.ExcelToDataTable("Sheet1", true);
                List<string> errordt = new List<string>();
                List<string> errordt2 = new List<string>();
                List<string> errordt3 = new List<string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    var blackNumber = dr["BlackNumber"]?.ToString();
                    int.TryParse(dr["Type"]?.ToString(), out var type);
                    if (!string.IsNullOrWhiteSpace(blackNumber) && type > 0)
                    {
                        var result = AddTireBlackList(blackNumber, type);
                        if (result == 3)
                            errordt.Add(blackNumber);
                        else if (result == 2)
                            errordt2.Add(blackNumber);
                        else if (result == 0)
                            errordt3.Add(blackNumber);
                    }
                }
                var info = $"上传成功!\n";
                if (errordt.Any())
                    info += $"以下数据格式不正确，插入失败：\n {string.Join(";", errordt)} \n";
                if (errordt2.Any())
                    info += $"以下数据已存在，插入失败：\n {string.Join(";", errordt2)} \n";
                if (errordt3.Any())
                    info += $"以下数据，插入失败：\n {string.Join(";", errordt3)} \n";
                return Json(new { Code = 1, Info = info });
            }
            return Json(new { Code = -1, Info = "请选中文件" });
        }


        public FileResult DownLoadTemp()
        {
            var fileName = $"限购黑名单上传模板.xls";
            HSSFWorkbook book = new HSSFWorkbook();
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            NPOI.SS.UserModel.IRow row = sheet1.CreateRow(0);
            row.CreateCell(0).SetCellValue("BlackNumber");
            row.CreateCell(1).SetCellValue("Type");
            row.CreateCell(2).SetCellValue("Type字段中：1代表手机号，2代表设备号，3代表支付账号，4代表用户ID");
            MemoryStream ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }
        #endregion


        #region 换轮胎入口跳转活动页配置
        public ActionResult ActPageOnTireChange()
        {
            TireViewModel model = new TireViewModel()
            {
                TireSize = RecommendManager.SelectALLTireSize()
            };
            return View(model);
        }

        public PartialViewResult ActPageOnTireChangeList(ListActCondition model, int PageSize = 10, int PageIndex = 1)
        {
            PagerModel pager = new PagerModel(PageIndex, PageSize);
            var tireActlist = ListActivityManager.SelectActPageOnTireChange(model, pager).ToList();
            var hashkeyList = tireActlist.Where(_ => !string.IsNullOrWhiteSpace(_.HashKey)).Select(_ => _.HashKey).ToList();
            RepositoryManager repository = new RepositoryManager();
            var activePageList = repository.GetEntityList<ActivePageListEntity>(_ => hashkeyList.Any(hl => hl == _.HashKey));
            if (tireActlist.Any() && activePageList != null && activePageList.Any())
            {
                tireActlist.ForEach(tl => activePageList.ForEach(ap =>
                {
                    if (ap.HashKey == tl.HashKey)
                    {
                        tl.StartTime = ap.StartDate;
                        tl.EndTime = ap.EndDate;
                        tl.Manager = ap.PersonCharge;
                    }
                }));
            }
            ViewBag.selectModel = model;
            return PartialView(new ListModel<ActivityItem>() { Pager = pager, Source = tireActlist });
        }

        public ActionResult TireActEditModule(int PKId, string vehicleId, string tireSize)
        {
            if (PKId < 0)
                return View(new ActivityItem());
            var tireActModel = ListActivityManager.FetchTireActivityById(PKId);
            if (string.IsNullOrWhiteSpace(tireActModel.VehicleId))
                tireActModel.VehicleId = vehicleId;
            if (string.IsNullOrWhiteSpace(tireActModel.TireSize))
                tireActModel.TireSize = tireSize;
            return View(tireActModel);
        }

        public ActionResult AddOrUpdateTireAct(ActivityItem model)
        {
            if (string.IsNullOrWhiteSpace(model.HashKey))
                return Json(-1);
            RepositoryManager repository = new RepositoryManager();
            var entity = repository.GetEntity<ActivePageListEntity>(_ => _.HashKey == model.HashKey);
            if (entity == null)
                return Json(-1);
            model.StartTime = entity.StartDate;
            model.EndTime = entity.EndDate;
            model.Manager = entity.PersonCharge;
            if (model.PKId.GetValueOrDefault() > 0)
            {
                var tireActModel = ListActivityManager.FetchTireActivityById(model.PKId.GetValueOrDefault());
                if (tireActModel == null || tireActModel.PKId.GetValueOrDefault() <= 0)
                    return Json(0);
                if (model.HashKey == tireActModel.HashKey && model.Status == tireActModel.Status)
                    return Json(1);
                var updateResult = ListActivityManager.UpdateTireChangedAct(model);
                if (updateResult)
                {
                    LoggerManager.InsertLog("TireChangedActivity", new
                    {
                        HashKey = model.HashKey,
                        State = model.Status.ToString(),
                        OldHashKey = tireActModel.HashKey,
                        OldState = tireActModel.Status.ToString(),
                        VehicleId = model.VehicleId,
                        TireSize = model.TireSize,
                        Author = HttpContext.User.Identity.Name,
                        message = "更新TireChangedActivity配置"
                    });
                }
                return Json(updateResult ? 1 : 0);
            }
            var addResult = ListActivityManager.AddTireChangedAct(model);
            if (addResult)
            {
                LoggerManager.InsertLog("TireChangedActivity", new
                {
                    HashKey = model.HashKey,
                    State = model.Status.ToString(),
                    VehicleId = model.VehicleId,
                    TireSize = model.TireSize,
                    Author = HttpContext.User.Identity.Name,
                    message = "创建TireChangedActivity配置"
                });
            }
            return Json(addResult ? 3 : 2);
        }

        public ActionResult AddOrUpdateTireActInBatch(string hashKey, int? status, List<ActivityItem> activityList)
        {
            if (activityList == null || !activityList.Any())
                return Json(-2);
            RepositoryManager repository = new RepositoryManager();
            var entity = repository.GetEntity<ActivePageListEntity>(_ => _.HashKey == hashKey);
            if (entity == null)
                return Json(-1);
            var activityToUpdateList = activityList.Where(_ => _.PKId != null).ToList();
            var pkids = activityToUpdateList.Select(_ => _.PKId.ToString()).ToList();
            var activityToAddList = activityList.Where(_ => _.PKId == null).ToList();
            activityToAddList.ForEach(al => { al.HashKey = hashKey; al.Status = status; al.Manager = entity.PersonCharge; al.StartTime = entity.StartDate; al.EndTime = entity.EndDate; });

            var listManager = new ListActivityManager();
            var addAndUpdateResult = listManager.AddAndUpdareTireChangeActInBatch(new ActivityItem { HashKey = hashKey, Status = status, Manager = entity.PersonCharge, StartTime = entity.StartDate, EndTime = entity.EndDate }
                    , pkids, activityToAddList);
            if (!addAndUpdateResult)
                return Json(0);
            if (activityToUpdateList != null && activityToUpdateList.Any())
            {
                foreach (var singleTireAct in activityToUpdateList)
                {
                    if (singleTireAct.HashKey != hashKey || singleTireAct.Status != status)
                    {
                        LoggerManager.InsertLog("TireChangedActivity", new
                        {
                            HashKey = hashKey,
                            State = status?.ToString(),
                            OldHashKey = singleTireAct.HashKey,
                            OldState = singleTireAct.Status.ToString(),
                            VehicleId = singleTireAct.VehicleId,
                            TireSize = singleTireAct.TireSize,
                            Author = HttpContext.User.Identity.Name,
                            message = "更新TireChangedActivity配置"
                        });
                    }
                }
            }
            if (activityToAddList != null && activityToAddList.Any())
            {
                foreach (var singleTireAct in activityToAddList)
                {
                    LoggerManager.InsertLog("TireChangedActivity", new
                    {
                        HashKey = hashKey,
                        State = status?.ToString(),
                        VehicleId = singleTireAct.VehicleId,
                        TireSize = singleTireAct.TireSize,
                        Author = HttpContext.User.Identity.Name,
                        message = "创建TireChangedActivity配置"
                    });
                }
            }
            return Json(1);
        }

        public ActionResult DeleteTireChangedAct(int PKId)
        {
            var tireActModel = ListActivityManager.FetchTireActivityById(PKId);
            if (tireActModel == null || tireActModel.PKId.GetValueOrDefault() <= 0)
                return Json(-1);
            var deleteResult = ListActivityManager.DeleteTireChangedAct(PKId);
            if (deleteResult)
            {
                LoggerManager.InsertLog("TireChangedActivity", new
                {
                    OldHashKey = tireActModel.HashKey,
                    OldState = tireActModel.Status.ToString(),
                    VehicleId = tireActModel.VehicleId,
                    TireSize = tireActModel.TireSize,
                    Author = HttpContext.User.Identity.Name,
                    message = "删除TireChangedActivity配置"
                });
            }
            return Json(deleteResult ? 1 : 0);
        }

        public ActionResult SelectTireActOprLogByHashkey(string vehicleId, string tireSize)
        {
            var result = ListActivityManager.SelectTireChangedActivityLog(vehicleId, tireSize)?.OrderByDescending(_ => _.CreateDatetime).ToList() ?? new List<TireChangedActivityLog>();
            return result != null && result.Any()
                ? Json(new { status = "success", data = result }, JsonRequestBehavior.AllowGet)
                : Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RefreshTireChangedActCache(List<ActivityItem> idSizeList)
        {
            using (var activityclient = new Service.Activity.CacheClient())
            {
                var flag = true;
                if (idSizeList == null && !idSizeList.Any())
                    return Json(-1);
                foreach (var idSizePair in idSizeList)
                {
                    var result = activityclient.RemoveRedisCacheKey("ActivityDeafult", $"TireChangedActivity/{idSizePair.VehicleId}/{idSizePair.TireSize}", "TireChangedActivity/{VehicleId}/{TireSize}");
                    if (!result.Success)
                        flag = false;
                }
                return flag ? Json(1) : Json(0);
            }
        }
        #endregion

        #region 轮胎规格指数管理后台
        [PowerManage]
        public ActionResult TireSpecParamsConfig()
        {
            return View();
        }

        public PartialViewResult TireSpecParamsConfigList(TireSpecParamsConfigQuery query)
        {
            List<TireSpecParamsModel> list = new List<TireSpecParamsModel>();
            list = TireSpecParamsConfigManager.QueryTireSpecParamsModel(query);

            var pager = new PagerModel(query.PageIndex, query.PageDataQuantity);
            ViewBag.query = query;
            pager.TotalItem = query.TotalCount;

            return PartialView(new ListModel<TireSpecParamsModel>()
            {
                Pager = pager,
                Source = list
            });
        }

        public ActionResult TireSpecParamsConfigEditModule(string pid)
        {
            TireSpecParamsModel model = TireSpecParamsConfigManager.SelectTireSpecParamsModelByPid(pid);
            TireSpecParamsModel result = new TireSpecParamsModel();
            if (!string.IsNullOrWhiteSpace(model.PId))
            {
                result = model;
            }
            else
            {
                result.PKID = 0;
                result.PId = pid;
                result.DisplayName = model.DisplayName;
                result.ProductID = model.ProductID;
            }
            return View(result);
        }

        public JsonResult SaveTireSpecParamsConfig(TireSpecParamsConfig tspc)
        {
            bool flag = false;
            if (tspc.PKID == 0)
            {
                flag = TireSpecParamsConfigManager.InsertTireSpecParamsConfig(tspc);
                TireSpecParamsEditLog log = new TireSpecParamsEditLog()
                {
                    PId = tspc.PId,
                    ChangeBefore = "",
                    ChangeAfter = "新增",
                    LastUpdateDataTime = DateTime.Now,
                    Operator = ThreadIdentity.Operator.Name,
                };
                bool flagLog = TireSpecParamsConfigManager.InsertTireSpecParamsEditLog(log);
            }
            else
            {
                TireSpecParamsModel model = TireSpecParamsConfigManager.SelectTireSpecParamsModelByPid(tspc.PId);
                flag = TireSpecParamsConfigManager.UpdateTireSpecParamsConfig(tspc);

                var compare = CompareDiff(model, tspc);
                TireSpecParamsEditLog log = new TireSpecParamsEditLog()
                {
                    PId = tspc.PId,
                    ChangeBefore = compare.Item1,
                    ChangeAfter = compare.Item2,
                    LastUpdateDataTime = DateTime.Now,
                    Operator = ThreadIdentity.Operator.Name,
                };
                bool flagLog = TireSpecParamsConfigManager.InsertTireSpecParamsEditLog(log);
            }
            using (var clientConfig = new ProductConfigClient())
            {
                var pids = new List<string>();
                pids.Add(tspc.PId);
                clientConfig.RefreashTireSpecParamsConfigCache(pids);
            }
            return Json(flag);
        }

        public ActionResult TireSpecParamsEditHistoryModule(string pid)
        {
            List<TireSpecParamsEditLog> logResult = TireSpecParamsConfigManager.SelectTireSpecParamsEditLog(pid);
            return View(logResult);
        }

        private Tuple<string, string> CompareDiff(TireSpecParamsModel model, TireSpecParamsConfig tspc)
        {
            string before = "";
            string after = "";

            if (model.QualityInspectionName != tspc.QualityInspectionName)
            {
                before = before + " 质检名称：" + (string.IsNullOrWhiteSpace(model.QualityInspectionName) ? "空" : model.QualityInspectionName);
                after = after + " 质检名称：" + (string.IsNullOrWhiteSpace(tspc.QualityInspectionName) ? "空" : tspc.QualityInspectionName);
            }
            if (model.OriginPlace != tspc.OriginPlace)
            {
                before = before + " 产地：" + (string.IsNullOrWhiteSpace(model.OriginPlace) ? "空" : model.OriginPlace);
                after = after + " 产地：" + (string.IsNullOrWhiteSpace(tspc.OriginPlace) ? "空" : tspc.OriginPlace);
            }
            if (model.RimProtection != tspc.RimProtection)
            {
                before = before + " 轮辋保护：" + (model.RimProtection == null ? "空" : model.RimProtection == false ? "否" : "是");
                after = after + " 轮辋保护：" + (tspc.RimProtection == null ? "空" : tspc.RimProtection == false ? "否" : "是");
            }
            if (model.TireLoad != tspc.TireLoad)
            {
                before = before + " 载重：" + (string.IsNullOrWhiteSpace(model.TireLoad) ? "空" : model.TireLoad);
                after = after + " 载重：" + (string.IsNullOrWhiteSpace(tspc.TireLoad) ? "空" : tspc.TireLoad);
            }
            if (model.MuddyAndSnow != tspc.MuddyAndSnow)
            {
                before = before + " M+S：" + (model.MuddyAndSnow == null ? "空" : model.MuddyAndSnow == false ? "否" : "是");
                after = after + " M+S：" + (tspc.MuddyAndSnow == null ? "空" : tspc.MuddyAndSnow == false ? "否" : "是");
            }
            if (model.ThreeT_Treadwear != tspc.ThreeT_Treadwear)
            {
                before = before + " 3T耐磨指数：" + (string.IsNullOrWhiteSpace(model.ThreeT_Treadwear) ? "空" : model.ThreeT_Treadwear);
                after = after + " 3T耐磨指数：" + (string.IsNullOrWhiteSpace(tspc.ThreeT_Treadwear) ? "空" : tspc.ThreeT_Treadwear);
            }
            if (model.ThreeT_Traction != tspc.ThreeT_Traction)
            {
                before = before + " 3T抓地指数：" + (string.IsNullOrWhiteSpace(model.ThreeT_Traction) ? "空" : model.ThreeT_Traction);
                after = after + " 3T抓地指数：" + (string.IsNullOrWhiteSpace(tspc.ThreeT_Traction) ? "空" : tspc.ThreeT_Traction);
            }
            if (model.ThreeT_Temperature != tspc.ThreeT_Temperature)
            {
                before = before + " 3T温度指数：" + (string.IsNullOrWhiteSpace(model.ThreeT_Temperature) ? "空" : model.ThreeT_Temperature);
                after = after + " 3T温度指数：" + (string.IsNullOrWhiteSpace(tspc.ThreeT_Temperature) ? "空" : tspc.ThreeT_Temperature);
            }
            if (model.TireCrown_Polyester != tspc.TireCrown_Polyester)
            {
                before = before + " 胎冠聚酯层数：" + (model.TireCrown_Polyester == null ? "空" : model.TireCrown_Polyester.ToString());
                after = after + " 胎冠聚酯层数：" + (tspc.TireCrown_Polyester == null ? "空" : tspc.TireCrown_Polyester.ToString());
            }
            if (model.TireCrown_Steel != tspc.TireCrown_Steel)
            {
                before = before + " 胎冠钢丝层数：" + (model.TireCrown_Steel == null ? "空" : model.TireCrown_Steel.ToString());
                after = after + " 胎冠钢丝层数：" + (tspc.TireCrown_Steel == null ? "空" : tspc.TireCrown_Steel.ToString());
            }
            if (model.TireCrown_Nylon != tspc.TireCrown_Nylon)
            {
                before = before + " 胎冠尼龙层数：" + (model.TireCrown_Nylon == null ? "空" : model.TireCrown_Nylon.ToString());
                after = after + " 胎冠尼龙层数：" + (tspc.TireCrown_Nylon == null ? "空" : tspc.TireCrown_Nylon.ToString());
            }
            if (model.TireSideWall_Polyester != tspc.TireSideWall_Polyester)
            {
                before = before + " 胎侧聚酯层数：" + (model.TireSideWall_Polyester == null ? "空" : model.TireSideWall_Polyester.ToString());
                after = after + " 胎侧聚酯层数：" + (tspc.TireSideWall_Polyester == null ? "空" : tspc.TireSideWall_Polyester.ToString());
            }
            if (model.TireLable_RollResistance != tspc.TireLable_RollResistance)
            {
                before = before + " 标签滚动阻力：" + (string.IsNullOrWhiteSpace(model.TireLable_RollResistance) ? "空" : model.TireLable_RollResistance);
                after = after + " 标签滚动阻力：" + (string.IsNullOrWhiteSpace(tspc.TireLable_RollResistance) ? "空" : tspc.TireLable_RollResistance);
            }
            if (model.TireLable_WetGrip != tspc.TireLable_WetGrip)
            {
                before = before + " 标签湿滑抓地性：" + (string.IsNullOrWhiteSpace(model.TireLable_WetGrip) ? "空" : model.TireLable_WetGrip);
                after = after + " 标签湿滑抓地性：" + (string.IsNullOrWhiteSpace(tspc.TireLable_WetGrip) ? "空" : tspc.TireLable_WetGrip);
            }
            if (model.TireLable_Noise != tspc.TireLable_Noise)
            {
                before = before + " 标签噪音：" + (string.IsNullOrWhiteSpace(model.TireLable_Noise) ? "空" : model.TireLable_Noise);
                after = after + " 标签噪音：" + (string.IsNullOrWhiteSpace(tspc.TireLable_Noise) ? "空" : tspc.TireLable_Noise);
            }
            if (model.PatternSymmetry != tspc.PatternSymmetry)
            {
                before = before + " 花纹对称：" + (string.IsNullOrWhiteSpace(model.PatternSymmetry) ? "空" : model.PatternSymmetry);
                after = after + " 花纹对称：" + (string.IsNullOrWhiteSpace(tspc.PatternSymmetry) ? "空" : tspc.PatternSymmetry);
            }
            if (model.TireGuideRotation != tspc.TireGuideRotation)
            {
                before = before + " 导向：" + (string.IsNullOrWhiteSpace(model.TireGuideRotation) ? "空" : model.TireGuideRotation);
                after = after + " 导向：" + (string.IsNullOrWhiteSpace(tspc.TireGuideRotation) ? "空" : tspc.TireGuideRotation);
            }

            return Tuple.Create(before, after);
        }
        #endregion
    }
}
