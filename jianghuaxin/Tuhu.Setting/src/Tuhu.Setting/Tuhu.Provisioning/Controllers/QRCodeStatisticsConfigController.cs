using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.QRCodeStatisticsConfig;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class QRCodeStatisticsConfigController : Controller
    {
        private readonly static QRCodeStatisticsConfigManager _QRCodeStatisticsConfigManager = new QRCodeStatisticsConfigManager();

        public ActionResult Index(string queryName)
        {
            IEnumerable<QRCodeStatisticsConfigModel> data = _QRCodeStatisticsConfigManager.GetListByPage(queryName);

            if (data != null)
            {
                data.ToList().ForEach(c =>
                {
                    var groups = data.Where(a => a.EventKey == c.EventKey
                    && a.Year == c.Year
                    && a.Month == c.Month
                    && a.Day == c.Day);

                    c.SumViewModel = string.Join("|", groups.Select(b => b.Event.ToLower() == "subscribe"
                    ? string.Format("{0}", b.Sum)
                    : string.Format("{0}", b.Sum)));
                });

                data = data.Distinct(new QRCodeStatisticsComparer()).ToList();

                ViewBag._QRCodeStatisticsList = data;
            }
            return View();
        }

        public class QRCodeStatisticsComparer : IEqualityComparer<QRCodeStatisticsConfigModel>
        {
            public bool Equals(QRCodeStatisticsConfigModel p1, QRCodeStatisticsConfigModel p2)
            {
                if (p1 == null)
                    return p2 == null;
                return p1.EventKey == p2.EventKey
                    && p1.Year == p2.Year
                    && p1.Month == p2.Month
                    && p1.Day == p2.Day;
            }

            public int GetHashCode(QRCodeStatisticsConfigModel p)
            {
                if (p == null)
                    return 0;
                return p.EventKey.GetHashCode();
            }
        }
    }
}