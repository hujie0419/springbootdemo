using AutoMapper;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Response;

namespace Tuhu.Service.Activity.Server.AutoMappers
{
    public class LuckyCharmProfile : Profile
    {
        public LuckyCharmProfile()
        {
            CreateMap<LuckyCharmActivityModel, LuckyCharmActivityInfoResponse>();
            CreateMap<LuckyCharmUserModel, LuckyCharmUserInfoRespone>();
        }
    }
}
