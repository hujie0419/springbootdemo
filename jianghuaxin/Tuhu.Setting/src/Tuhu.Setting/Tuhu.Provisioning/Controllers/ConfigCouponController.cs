using System;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.DownloadApp;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ConfigCouponController : Controller
    {
        private readonly Lazy<DownloadAppManager> lazy = new Lazy<DownloadAppManager>();

        private DownloadAppManager DownloadAppManager
        {
            get { return lazy.Value; }
        }
        //
        // GET: /ConfigCoupon/

        public ActionResult Index()
        {
            return View(DownloadAppManager.GetConfigCoupon());
        }

        public ActionResult Add(int? Id = 0)
        {
            if (Id == 0)
            {            
                return View(new ConfigCoupon());
            }
            else {
                return View(DownloadAppManager.GetConfigCoupon());
            }         
          
        }

        [HttpPost]
        public ActionResult Add(ConfigCoupon model)
        {
            if (model.Id > 0)
            {
                DownloadAppManager.UpdateConfigCoupon(model);
            }
            else
            {
                DownloadAppManager.InsertConfigCoupon(model);
            }
            return Redirect("Index");
        }
    }
}
