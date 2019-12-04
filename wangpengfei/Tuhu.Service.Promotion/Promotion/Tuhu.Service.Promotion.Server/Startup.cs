using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using Tuhu.C.Utility.Components;
using Tuhu.HealthChecks.Cpu;
using Tuhu.HealthChecks.IIS;
using Tuhu.HealthChecks.Memory;
using Tuhu.Service.Promotion.DataAccess;
using Tuhu.Service.Promotion.Server.Config;
using Tuhu.Web;

namespace Tuhu.Service.Promotion.Server
{
    public class Startup : TuhuStartup
    {
        public Startup(IConfiguration configuration) : base(configuration) { }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(typeof(IPromotionClient).Assembly.GetName());

            services.AddHttpClient("HttpTimeoutForCommitWorkOrder", client => {
                client.Timeout = TimeSpan.Parse(ConfigurationRoot.GetSection("AppSettings:HttpTimeoutForCommitWorkOrder").Value);//00:00:20
            });

            return base.ConfigureServices(services);
        }

        /// <summary>
        /// 过滤不需要的日志
        /// </summary>
        /// <param name="loggingBuilder"></param>
        private void DisableAspNetCoreLog(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddFilter("Microsoft", LogLevel.Warning).AddFilter("System", LogLevel.Warning).AddFilter("NToastNotify", LogLevel.Warning);
        }

        protected override void ConfigureTuhuServices(ITuhuBuilder builder)
        {
            builder.AddMessageQueue().AddSentryLogging()
                .AddDbHelper().AddConnectionStrings(SqlClientFactory.Instance, builder.Configuration.GetSection("connectionString"))
                .Services.Configure<AppSettingOptions>(builder.Configuration.GetSection("AppSettings"))
                ;
            ;

            builder.AddWcfServer();//.AddServerPolicy().AddDefaultPolicy();
            builder.AddNosql();
            builder.Services.AddLogging(p => this.DisableAspNetCoreLog(p));//过滤不需要的日志

            builder.Services.AddTuhuMemoryCacheNoJson();

            //注入底层类
            builder.AddDataAccess();
            builder.AddServiceProxy();
            builder.AddManager();

            //需要开启Profiling时取消注释下面一段代码，需要哪个就请开启哪个
            //builder.AddProfiling()
            //    .AddWcfServer()//Tuhu.Service.Server --wcf客户端一定不要
            //    .AddElasticsearch()//Tuhu.Elasticsearch
            //    .AddRedis()//Tuhu.Nosql.Redis
            //    .AddAdo()//Tuhu.Profiling.Data
            //    .AddEntityFramework()/*Tuhu.Profiling.EntityFramework*/);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);

            app.UseLog4Net()
                .UseWebLog()
                .UseStaticFiles()
               //.UseProfiling()
               .UseWebApi();
        }
    }
}
