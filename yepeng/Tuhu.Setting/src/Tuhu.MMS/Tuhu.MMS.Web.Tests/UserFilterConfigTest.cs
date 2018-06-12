using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuhu.MMS.Web.Service;
using Tuhu.MMS.Web.Domain.UserFilter;

namespace Tuhu.MMS.Web.Tests
{
    [TestClass]
    public class UserFilterConfigTest
    {
        [TestMethod]
        public void SubmitJobTest()
        {
            int jobid = 9;
            var result = UserFilterService.SetFilterJobRunAsync(jobid).GetAwaiter().GetResult();
            
            Assert.IsTrue(result);
        }

    }

}
