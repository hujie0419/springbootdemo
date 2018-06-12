using Quartz;
using System;
using System.Reflection;
using Tuhu.C.SyncProductPriceJob.Job;
using Xunit;

namespace Tuhu.C.ThirdParty.SyncProductPrice.Test
{
    public class JobTest
    {
        [Theory]
        [InlineData(typeof(SyncRemainingsPriceJob))]
        [InlineData(typeof(SyncProductMappingJob))]
        [InlineData(typeof(SyncProductPriceJob.Job.SyncProductPriceJob))]
        public void IsJobDisallowConcurrentExecution(Type jobType)
        {
            var attrType = typeof(DisallowConcurrentExecutionAttribute);
            Assert.NotNull(jobType.GetCustomAttribute(attrType));
            Assert.True(jobType.IsDefined(attrType));
        }
    }
}
