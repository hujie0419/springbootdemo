using Microsoft.Extensions.DependencyInjection;
using Tuhu.Service.Promotion.DataAccess.IRepository;
using Tuhu.Service.Promotion.DataAccess.Repository;

namespace Tuhu.Service.Promotion.DataAccess
{
    public static class TuhuBuilderExtensions
    {
        /// <summary>
        /// 注入 DataAccess
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ITuhuBuilder AddDataAccess(this ITuhuBuilder builder)
        {
            builder.Services.AddScoped<IPromotionCodeRepository, PromotionCodeRepository>();
            builder.Services.AddScoped<ICouponGetRuleRepository, CouponGetRuleRepository>();
            builder.Services.AddScoped<ICouponUseRuleRepository, CouponUseRuleRepository>();
            builder.Services.AddScoped<ICouponGetRuleAuditRepository, CouponGetRuleAuditRepository>();
            builder.Services.AddScoped<IPromotionTaskRepository, PromotionTaskRepository>();
            builder.Services.AddScoped<IPromotionTaskPromotionListRepository, PromotionTaskPromotionListRepository>();
            builder.Services.AddScoped<IPromotionSingleTaskUsersRepository, PromotionSingleTaskUsersRepository>();
            builder.Services.AddScoped<IPromotionSingleTaskUsersHistoryRepository, PromotionSingleTaskUsersHistoryRepository>();
            builder.Services.AddScoped<IChannelDictionariesRepository, ChannelDictionariesRepository>();
            builder.Services.AddScoped<IPromotionTaskProductCategoryRepository, PromotionTaskProductCategoryRepository>();
            builder.Services.AddScoped<IPromotionOprLogRepository, PromotionOprLogRepository>();
            builder.Services.AddScoped<IPromotionTaskBudgetRepository, PromotionTaskBudgetRepository>();
            builder.Services.AddScoped<IRegionRepository, RegionRepository>();
            builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
            builder.Services.AddScoped<IUserActivityApplyRepository, UserActivityApplyRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            return builder;
        }
    }
}
