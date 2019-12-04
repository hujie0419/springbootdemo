using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Xunit;
using Xunit.Abstractions;

[assembly: TestFramework("Tuhu.Service.Promotion.IntegrationTest.Startup", "Tuhu.Service.Promotion.IntegrationTest")]
namespace Tuhu.Service.Promotion.IntegrationTest
{
    public class Startup : TuhuTestFramework
    {
        public Startup(IMessageSink messageSink) : base(messageSink) { }
#if TUHU2
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.TryAddScoped<IPromotionClient, PromotionClient>();
            services.TryAddScoped<ICouponGetRuleClient, CouponGetRuleClient>();

            base.ConfigureServices(services);
        }
#else
        protected override void ConfigureTuhuServices(ITuhuBuilder builder)
        {
            builder.AddWcfClient().AddPromotion().AddClientPolicy().AddDefaultPolicy();
        }
#endif
    }
}
