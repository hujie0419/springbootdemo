using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.JobCouponConfig;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class JobCouponConfigController : Controller
    {
        private Lazy<JobCouponConfigManager> Lazy_JobCouponConfigManager = new Lazy<JobCouponConfigManager>();
        public JobCouponConfigManager _JobCouponConfigManager => Lazy_JobCouponConfigManager.Value;

        public ActionResult Index(int pageIndex = 1, int pageSieze = int.MaxValue)
        {
            ViewBag.JobCouponConfigData = _JobCouponConfigManager.Select(pageIndex, pageSieze);
            return View();
        }

        public ActionResult AddOrEdit(int id)
        {
            if (id != 0)
            {
                //修改
                JobCouponConfigModel model = _JobCouponConfigManager.SelectById(id) ?? new JobCouponConfigModel() { ID = -1 };
                return View(model);
            }
            else
            {
                //新增
                JobCouponConfigModel model = new JobCouponConfigModel()
                {
                    ID = 0,
                    State = true,
                    ReturnType = 1,
                    CreateTime = DateTime.Now
                };
                return View(model);
            }
        }

        public ActionResult Save(int id, JobCouponConfigModel model)
        {
            bool result = false;
            if (id == 0)    
            {
                //新增
                model.CreateTime = DateTime.Now;
                result = _JobCouponConfigManager.Insert(model);
            }
            else           
            {
                //修改
                result = _JobCouponConfigManager.Update(model);
            }

            return RedirectToAction("Index");
        }
    }
}