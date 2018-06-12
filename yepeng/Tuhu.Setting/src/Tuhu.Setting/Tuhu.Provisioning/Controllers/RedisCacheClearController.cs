using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using Tuhu.Nosql.Configuration;
using Tuhu.Nosql.Redis;
using Tuhu.Nosql.Redis.Configuration;
using Tuhu.Service;

namespace Tuhu.Provisioning.Controllers
{
    public class RedisCacheClearController : Controller
    {

        public static readonly List<string> ServiceList = new List<string>()
        {
            "Tuhu.Service.Vehicle",
            "Tuhu.Service.Product",
            "Tuhu.Service.Config",
            "Tuhu.Service.Member",
            "Tuhu.Service.BaoYang",
            "Tuhu.Service.Comment",
            "Tuhu.Service.Config",
            "Tuhu.Service.GaiZhuang",
            "Tuhu.Service.Push",
            "Tuhu.Service.SEO",
            "Tuhu.Service.ThirdParty",
            "Tuhu.Service.UserAccount"
        };
        // GET: RedisCacheClear
        public ActionResult Index()
        {

            ViewBag.ServiceList = ServiceList;
            ViewBag.RedisDataType = typeof(RedisDataType).GetEnumNames();
            return View();
        }
        
        public JsonResult Execute(string site, string cacheName, string cacheKey,RedisDataType redisDataType)
        {
            try
            {

                if (Initialize(site, redisDataType, cacheName, cacheKey))
                    return Json(new {result = true});
                else
                    return Json(new {result = false, message = "清理缓存失败"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new {result = false, message = e.Message}, "text/html");
            }
            
        }

        public Proxy Proxy = Proxy.None;
        public bool Initialize(string prefix, RedisDataType redisDataType, string name, string key)
        {
            prefix = $"{prefix}/{redisDataType.ToString()}/{name}/";
            var configuration = (INosqlConfiguration) ConfigurationManager.GetSection("nosql");
            var redisConfiguration = (IRedisCachingConfiguration) ConfigurationManager.GetSection(configuration.SectionName);
            int database = (int) redisDataType;
            if (redisConfiguration.Proxy == "Twemproxy")
            {
                Proxy = Proxy.Twemproxy;
                database = 0;
            }
            else if (redisConfiguration.Proxy == "Cluster")
            {
                prefix = "{" + prefix + "}";
                database = 0;
            }
            using (var provider = GetConnectedMultiplexer(redisConfiguration))
            {
                var db = provider.GetDatabase(database).WithKeyPrefix(prefix);
                var result =  db.KeyDelete(key);
                return result;
            }
        }
        private ConnectionMultiplexer GetConnectedMultiplexer(IRedisCachingConfiguration configuration)
        {
            return  CreateConnectionMultiplexer(configuration);
        }
        private ConnectionMultiplexer CreateConnectionMultiplexer(IRedisCachingConfiguration cachingConfiguration)
        {
            var options = new ConfigurationOptions
            {
                Ssl = cachingConfiguration.Ssl,
                AllowAdmin = cachingConfiguration.AllowAdmin,
                Password = cachingConfiguration.Password,
                AbortOnConnectFail = cachingConfiguration.AbortOnConnectFail,
                Proxy = Proxy,
                ConnectTimeout = cachingConfiguration.ConnectTimeout,
                DefaultDatabase = cachingConfiguration.Database
            };

            foreach (RedisHost redisHost in cachingConfiguration.RedisHosts)
            {
                options.EndPoints.Add(redisHost.Host, redisHost.CachePort);
            }


            var connectionMultiplexer = ConnectionMultiplexer.Connect(options);

            return connectionMultiplexer;
        }
    }
}