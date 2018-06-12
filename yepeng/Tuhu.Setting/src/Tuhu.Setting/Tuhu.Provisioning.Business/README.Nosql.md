## 配置合并说明
#### 使用方式为
``` C#
var client = CacheHelper.CreateXXXClient();
```
接口定义如下：
``` C#
Tuple.Create("CacheClient",
    @"/// <summary>缓存</summary>",
    new[]
    {
        Tuple.Create("Exists", "", "key是否存在", "string key"),
        Tuple.Create("Remove", "", "删除key", "string key"),
        Tuple.Create("Get", "T", "获取Value", "string key"),
        Tuple.Create("Set", "T", "插入数据", "string key, T value, TimeSpan expiration")
    }),
Tuple.Create("FirewallClient",
    @"/// <summary>防火墙</summary>",
    new[]
    {
        Tuple.Create("Hit", "bool", "添加命中", "long delta"),
        Tuple.Create("Blocked", "bool", "是否阻止", "long delta"),
        Tuple.Create("HitCount", "int", "命中次数", ""),
        Tuple.Create("Reset", "bool", "重置命中次数", "")
    }),
Tuple.Create("CounterClient",
    @"/// <summary>计数器</summary>",
    new[]
    {
        Tuple.Create("Increment", "long", "增加次数", "long delta"),
        Tuple.Create("Decrement", "long", "减少次数", "long delta"),
        Tuple.Create("Count", "long", "数次", "")
    })
```
配置支持内联和外联(外部配置文件)方式
#### 将下面配置中你需要的节点合并到项目配置文件中即可
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<configSections>
		<section name="nosql" type="Tuhu.Nosql.Configuration.NosqlConfiguration, Tuhu.Nosql" />
		<section name="redisClient" type="Tuhu.Nosql.Redis.Configuration.RedisCachingSectionHandler, Tuhu.Nosql.Redis" />
	</configSections>
	<nosql provider="Tuhu.Nosql.Redis.RedisProvider, Tuhu.Nosql.Redis" sectionName="redisClient" prefix="Tuhu.Nosql.Redis.UnitTest">
		<expiration defaultExpiration="00:30:00">
			<cache>
				<add name="aaa" expiration="01:00:00" />
				<add name="ccc" expiration="3600" />
			</cache>
			<counter defaultExpiration="00:45:00">
				<add name="bbb" expiration="01:30:00" />
			</counter>
		</expiration>
	</nosql>
	<redisClient allowAdmin="true" ssl="false" connectTimeout="500" database="1" proxy="Twemproxy">
		<hosts>
			<!--add host="172.16.20.233" cachePort="6379"/-->
			<add host="172.16.20.233" cachePort="6479"/>
			<add host="172.16.20.233" cachePort="6579"/>
		</hosts>
	</redisClient>
</configuration>
```
expiration支持timespan和浮点数(单位s)
#### 外联方式配置如下：

``` xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<nosql provider="Tuhu.Nosql.Redis.RedisProvider, Tuhu.Nosql.Redis" sectionName="redisClient" prefix="Tuhu.Nosql.Redis.UnitTest" file="Cache.config">
	</nosql>
</configuration>
```
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<nosql>
		<expiration defaultExpiration="00:30:00">
			<cache>
				<add name="aaa" expiration="01:00:00" />
			</cache>
			<counter defaultExpiration="00:45:00">
				<add name="bbb" expiration="01:30:00" />
			</counter>
		</expiration>
	</nosql>
</configuration>
```
nosql不能包含属性

