using Autofac;
using Autofac.Integration.Wcf;
using Autofac.Integration.WebApi;
using System.Reflection;
using System.Web.Http;
using Tuhu.Profiling;

namespace Tuhu.Service.Hosting
{
    public class Startup : ServiceInitialize
    {
        protected override void Register(ContainerBuilder builder, Assembly[] assemblies)
        {
            builder.RegisterInstance(new ProfilingBuilder("cat")
                .EnableProfiling() //Tuhu.Profiling
                .AddElasticsearch()//Tuhu.Elasticsearch
                .AddWcfServer()//Tuhu.Service.Server
                .AddRedis()//Tuhu.Nosql.Redis
                .AddAdo()//Tuhu.Profiling.Data
                );

            base.Register(builder, assemblies);
        }

        /// <summary>站点启动代码，支持HTTP和NON-HTTP</summary>
        public static void AppInitialize()
        {
            AutofacHostFactory.Container = ServiceResolver.Initialize(new Startup());

            AutofacHostFactory.Container.Resolve<ProfilingBuilder>().Initialize();

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(AutofacHostFactory.Container);
        }
    }
}
