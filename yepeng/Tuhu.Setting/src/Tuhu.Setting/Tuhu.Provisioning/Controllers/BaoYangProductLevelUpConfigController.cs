using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using Tuhu.Component.Framework.Identity;
using Tuhu.Service.BaoYang;
using Tuhu.Service.BaoYang.Config;

namespace Tuhu.Provisioning.Controllers
{
    public class BaoYangProductLevelUpConfigController : Controller
    {
        public async Task<ActionResult> Index()
        {
            List<BaoYangProductLevelUpConfig> config = new List<BaoYangProductLevelUpConfig>();

            using (var client = new BaoYangDbClient())
            {
                var serviceResult = await client.SelectBaoYangProductLevelUpConfigAsync();
                if (serviceResult.Success)
                {
                    config = serviceResult.Result.ToList();
                }
            }

            return View(config);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateConfig(string packageType, string baoyangType, 
            bool inSameBrand, bool isActive)
        {
            bool result = false;

            using (var client = new BaoYangDbClient())
            {
                var serviceResult = await client.UpdateBaoYangProductLevelUpConfigAsync(packageType, baoyangType,
                    isActive, inSameBrand, ThreadIdentity.Operator.Name);

                if (serviceResult.Success)
                {
                    result = serviceResult.Result;
                }
            }

            return Json(new {Status = result});
        }
    }
}