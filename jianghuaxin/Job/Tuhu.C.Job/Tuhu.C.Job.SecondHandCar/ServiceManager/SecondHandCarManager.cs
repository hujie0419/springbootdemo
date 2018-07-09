using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.SecondHandCar;
using Tuhu.Service;
using Tuhu.Service.SecondHandCar.Model;

namespace Tuhu.C.Job.SecondHandCar.ServiceManager
{
    public class SecondHandCarManager
    {
        public static List<StashCheckReportModel> GetAllStashReport()
        {
            var result = new List<StashCheckReportModel>();
            using (var client = new SecondHandCarAdminClient())
            {
                var temp = client.GetAllStashReport();
                if (temp.Success)
                    result = temp.Result.ToList();
                return result;
            }
        }
        public static bool DeleteStashReport(int shopId, int detectOrderId)
        {
            var result = false;
            using (var client = new SecondHandCarCheckClient())
            {
                var temp = client.DeleteTemporaryCheckReport(new DeleteTemporaryCheckReport
                {
                    DetectOrderId = detectOrderId,
                    ShopId = shopId
                });
                if (temp.Success)
                    result = temp.Result;
                return result;
            }
        }
    }
}
