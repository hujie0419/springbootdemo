using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.Vehicle;
using Tuhu.Service.Utility;
using Tuhu.Service.Vehicle;
using Tuhu.Service.Vehicle.Request;

namespace Tuhu.Provisioning.Controllers
{
    //车型认证审核
    public class VehicleAuthAuditController : Controller
    {
        #region 6周年 车型认证审核
        // GET: VehicleTypeCertificationAudit
        [PowerManage]
        public ActionResult Index()
        {
            return View();
        }


        public PartialViewResult List(VehicleTypeCertificationAuditRequest query)
        {
            using (var client = new Tuhu.Service.Vehicle.VehicleClient())
            {

                var result = client.SelectVehicleAsync(query);
                result.ThrowIfException(true);
                ViewBag.Query = query;

                var logs = new VehicleTypeCertificationAuditLogManager().SelectLogs(result.Result.Select(_ => _.CarId).ToList());

                var logsDic = logs.ToDictionary(_ => _.CarId, _ => _);

                ViewBag.LogsDic = logsDic;
                return PartialView(result.Result);
            }
        }


        public JsonResult SetStatus(Guid carId, int newStatus, int oldStatus, string description, string mobile, string carNo)
        {
            using (var client = new Tuhu.Service.Vehicle.VehicleClient())
            {
                var result = client.UpdateVehicleTypeCertificationStatus(new VehicleTypeCertificationRequest()
                {
                    CarId = carId,
                    Status = newStatus
                });
                result.ThrowIfException(true);
                new VehicleTypeCertificationAuditLogManager().InsertLog(
                    new DataAccess.Entity.VehicleTypeCertificationAuditLogModel()
                    {
                        Author = HttpContext.User.Identity.Name,
                        CarId = carId,
                        OldValue = oldStatus.ToString(),
                        NewValue = newStatus.ToString(),
                        Description = description
                    });
            }
            //审核通过 发短信
            if (newStatus == 1)
            {
                using (var smsClient = new SmsClient())
                {
                    smsClient.SendSms(mobile, 131, carNo);
                }
            }
            else if (newStatus == -1) //改为未认证 发短信
            {
                using (var smsClient = new SmsClient())
                {
                    smsClient.SendSms(mobile, 130, carNo);
                }
            }
            return Json(new
            {
                success = true
            });
        }
        #endregion
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult IndexNew()
        {
            return View();
        }

        public PartialViewResult ListNew(DataAccess.Entity.VehicleAuthAuditRequest query)
        {
            var manager = new VehicleTypeCertificationAuditLogManager();
            var result = manager.SelectVehicleAuditInfo(query);
            ViewBag.Query = query;
            var logs = manager.SelectLogs(result.Select(_ => _.CarId).ToList());
            var logsDic = logs.ToDictionary(_ => _.CarId, _ => _);

            ViewBag.LogsDic = logsDic;
            return PartialView(result);
        }

        public PartialViewResult Logs(Guid carId)
        {
            var manager = new VehicleTypeCertificationAuditLogManager();
            var result = manager.SelectLogsByCarId(carId);
            return PartialView(result);
        }

        public JsonResult SetStatusNew(Guid carId, int newStatus, int oldStatus, string description, string mobile,
            string carNo)
        {
            var manager = new VehicleTypeCertificationAuditLogManager();
            manager.UpdateVehicleAuditStatus(carId, newStatus);
            manager.InsertLog(
                new DataAccess.Entity.VehicleTypeCertificationAuditLogModel()
                {
                    Author = HttpContext.User.Identity.Name,
                    CarId = carId,
                    OldValue = oldStatus.ToString(),
                    NewValue = newStatus.ToString(),
                    Description = description
                });

            //审核通过 发短信
            if (newStatus == 1)
            {
                using (var smsClient = new SmsClient())
                {
                    smsClient.SendSms(mobile, 138, carNo);
                }
            }
            else if (newStatus == 2) //审核不通过 发短信
            {
                using (var smsClient = new SmsClient())
                {
                    smsClient.SendSms(mobile, 139, carNo);
                }
            }
            return Json(new
            {
                success = true
            });
        }
    }
}