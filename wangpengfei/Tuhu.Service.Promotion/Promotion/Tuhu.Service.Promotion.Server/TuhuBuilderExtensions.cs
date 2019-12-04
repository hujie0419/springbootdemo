using Microsoft.Extensions.DependencyInjection;
using Tuhu.Service.ConfigBase;
using Tuhu.Service.ConfigLog;
using Tuhu.Service.CreatePromotion;
using Tuhu.Service.Order;
using Tuhu.Service.ProductQuery;
using Tuhu.Service.Promotion.Server.Manager;
using Tuhu.Service.Promotion.Server.Manager.IManager;
using Tuhu.Service.Promotion.Server.ServiceProxy;
using Tuhu.Service.Push;
using Tuhu.Service.Shop;
using Tuhu.Service.UserAccount;
using Tuhu.Service.Utility;
using IMonitorClient = Tuhu.Service.C.Monitor.IMonitorClient;
using MonitorClient = Tuhu.Service.C.Monitor.MonitorClient;

namespace Tuhu.Service.Promotion.DataAccess
{
    public static class TuhuBuilderExtensions
    {
        /// <summary>
        /// 注入manager
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ITuhuBuilder AddManager(this ITuhuBuilder builder)
        {
            builder.Services.AddScoped<ICouponManager, CouponManager> ();
            //builder.Services.AddHttpClient<ICouponGetRuleManager, CouponGetRuleManager>(options => options.BaseAddress = new System.Uri( builder.Configuration.GetSection("HttpClient:xxx:Address").Value));
            builder.Services.AddScoped<ICouponGetRuleManager, CouponGetRuleManager>(); 
            builder.Services.AddScoped<ICouponGetRuleAuditManager, CouponGetRuleAuditManager>();
            builder.Services.AddScoped<ICouponTaskManager, CouponTaskManager>();
            builder.Services.AddScoped<ICommonManager, CommonManager>();
            builder.Services.AddScoped<IRegionManager, RegionManager>();
            builder.Services.AddScoped<IActivityManager, ActivityManager>();
            builder.Services.AddScoped<IUserActivityApplyManager, UserActivityApplyManager>();
            builder.Services.AddScoped<IUserManager, UserManager>();

            return builder;
        }

        /// <summary>
        /// 注入 ServiceProxy
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ITuhuBuilder AddServiceProxy(this ITuhuBuilder builder)
        {
            //基础配置
            builder.Services.AddScoped<IConfigBaseClient, ConfigBaseClient>();
            builder.Services.AddScoped<IConfigBaseService, ConfigBaseService>();

            //日志
            builder.Services.AddScoped<IConfigLogClient, ConfigLogClient>();
            builder.Services.AddScoped<Server.ServiceProxy.IConfigLogService, ConfigLogService>();
            //创建优惠券
            builder.Services.AddScoped<ICreatePromotionClient, CreatePromotionClient>();
            builder.Services.AddScoped<Server.ServiceProxy.ICreatePromotionService, CreatePromotionService>();

            //订单
            builder.Services.AddScoped<IOrderApiForCClient, OrderApiForCClient>();
            builder.Services.AddScoped<Server.ServiceProxy.IServiceProxy.IOrderService, OrderService>();

            //门店
            builder.Services.AddScoped<IShopClient, ShopClient>();
            builder.Services.AddScoped<Server.ServiceProxy.IServiceProxy.IShopService, ShopService>();


            //门店
            builder.Services.AddScoped<IProductInfoQueryClient, ProductInfoQueryClient>();
            builder.Services.AddScoped<Server.ServiceProxy.IServiceProxy.IProductService, ProductService>();


            //用户
            builder.Services.AddScoped<IUserAccountClient, UserAccountClient>();
            builder.Services.AddScoped<Server.ServiceProxy.IServiceProxy.IUserAccountService, UserAccountService>();

            //短信
            builder.Services.AddScoped<ISmsClient, SmsClient>();
            builder.Services.AddScoped<Server.ServiceProxy.IServiceProxy.ISmsService, SmsService>();

            //推送
            builder.Services.AddScoped<ITemplatePushClient, TemplatePushClient>();
            builder.Services.AddScoped<Server.ServiceProxy.IServiceProxy.IPushService, PushService>();

            //监控 - metric
            builder.Services.AddScoped<IMonitorClient, MonitorClient>();
            builder.Services.AddScoped<Server.ServiceProxy.IServiceProxy.IMonitorService, MonitorService>();

            return builder;
        }


    }
}
