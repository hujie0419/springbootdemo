using AutoMapper;
using System.Linq;
using System.Reflection;

namespace Tuhu.Service.Activity.Server.AutoMappers
{
    public class Configuration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<LuckyCharmProfile>();
            });
        }
    }
}
