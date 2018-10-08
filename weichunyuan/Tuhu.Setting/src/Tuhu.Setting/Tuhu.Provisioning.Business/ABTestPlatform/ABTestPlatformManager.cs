using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.Service.Config;
using Tuhu.Service.Config.Models;
using Tuhu.Provisioning.DataAccess.DAO;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity.ABTestPlatform;

namespace Tuhu.Provisioning.Business.ABTestPlatform
{
    public class ABTestPlatformManager
    {
        private static DalABTestPlatform dal = null;

        public ABTestPlatformManager()
        {
            dal = new DalABTestPlatform();
        }

        public List<ABTestDetail> GetAllTestList()
        {
            var dt = dal.GetABTestDetail();
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.ConvertTo<ABTestDetail>().ToList();
        }

        public List<string> GetAllTestName()
        {
            var dt = dal.GetAllTestName();
            if (dt == null || dt.Rows.Count == 0)
                return null;

            List<string> result = new List<string>();
            foreach (DataRow item in dt.Rows) result.Add(item["TestName"].ToString());
            return result;
        }

        public ABTestDetail GetABTestDetailByGuid(Guid testGuid)
        {
            var dt = dal.GetABTestDetailByGuid(testGuid);
            if (dt == null || dt.Rows == null || dt.Rows.Count < 1) return null;

            ABTestDetail test = new ABTestDetail()
            {
                TestGuid = testGuid,
                TestName = dt.Rows[0]["TestName"].ToString(),
                TestScale = Convert.ToDouble(dt.Rows[0]["TestScale"].ToString()) * 100,
                Status = dt.Rows[0]["Status"].ToString(),
                CreateTime = Convert.ToDateTime(dt.Rows[0]["CreateTime"].ToString()),
                LastUpdateDataTime = Convert.ToDateTime(dt.Rows[0]["LastUpdateDataTime"].ToString()),
                Creator = dt.Rows[0]["Creator"].ToString(),
                GroupList = new List<ABTestGroupDetail>(),
                GroupNum = Convert.ToInt32(dt.Rows[0]["GroupNum"].ToString())
            };

            var dtGroup = dal.GetABTestGroupDetailByGuid(testGuid);
            test.GroupList = dtGroup.ConvertTo<ABTestGroupDetail>().ToList();

            return test;
        }

        public bool CreateABTest(ABTestDetail test)
        {
            bool result = false;
            List<ABTestDetail> input = new List<ABTestDetail>();
            input.Add(test);
            using (var client = new ConfigClient())
            {
                var createResult = client.CreateABTestDetail(input);
                if (!createResult.Success) return false;
                else result = createResult.Result;
            }
            return result;
        }

        public bool UpdateABTest(ABTestDetail test)
        {
            bool result = false;
            List<ABTestDetail> input = new List<ABTestDetail>();
            input.Add(test);
            using (var client = new ConfigClient())
            {
                var createResult = client.UpdateABTestDetail(input);
                if (!createResult.Success) return false;
                else result = createResult.Result;
            }
            return result;
        }

        public bool DeleteABTest(Guid testGuid, string testName)
        {
            if (string.IsNullOrWhiteSpace(testName) || testGuid == Guid.Empty) return false;
            var result = dal.DeleteABTestByGuid(testGuid);
            if (!result) return false;
            else
            {
                List<string> input = new List<string>();
                input.Add(testName);
                using (var client = new ConfigClient())
                {
                    var deleteResult = client.DeleteABTestDetailCache(input);
                    if (!deleteResult.Success) return false;
                    else result = deleteResult.Result;
                }
                return result;
            }
        }

        public bool SelectABTestResult(string testGuid, string testName, string status, string testGroupId)
        {
            if (string.IsNullOrWhiteSpace(testGuid) ||
                string.IsNullOrWhiteSpace(testName) ||
                string.IsNullOrWhiteSpace(status) ||
                string.IsNullOrWhiteSpace(testGroupId))
                return false;

            var result = dal.SelectABTestResult(testGuid, testGroupId);
            if (!result) return false;
            else
            {
                List<string> input = new List<string>();
                input.Add(testName);
                using (var client = new ConfigClient())
                {
                    var deleteResult = client.UpdateABTestDetailCache(input);
                    if (!deleteResult.Success) return false;
                    else result = deleteResult.Result;
                }
                return result;
            }
        }

        public bool InsertLog(ABTestEditLog log)
        {
            return dal.InsertLog(log);
        }

        public List<ABTestEditLog> GetLog(Guid testGuid)
        {
            var dt = dal.GetLog(testGuid);
            if (dt == null || dt.Rows == null || dt.Rows.Count < 1) return null;

            return dt.ConvertTo<ABTestEditLog>().ToList();
        }
    }
}