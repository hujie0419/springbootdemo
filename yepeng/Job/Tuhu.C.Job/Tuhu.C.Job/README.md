## Tuhu忆拆分出Tuhu.Log、Tuhu.MessageQueue和Tuhu.Nosql.Couchbase
    
## 配置合并说明
#### 将下面配置中你需要的节点合并到项目配置文件中即可
``` xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
	<configSections>
		<sectionGroup name="common">
			<section name="logging" type="Common.Logging.ConfigurationSectionHandler, Common.Logging" />
		</sectionGroup>
		<section name="couchbase" type="Couchbase.Configuration.Client.Providers.CouchbaseClientSection, Couchbase.NetClient" />
		<section name="rabbitmq" type="Tuhu.MessageQueue.RabbitMQSection, Tuhu.MessageQueue" />
		<section name="zookeeper" type="Tuhu.ZooKeeper.ZooKeeperClientSection, Tuhu.ZooKeeper" />
	</configSections>
	<common>
		<logging>
			<factoryAdapter type="Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter, Common.Logging.Log4Net1211">
				<!--<arg key="configType" value="INLINE" />-->
				<arg key="configType" value="FILE-WATCH" />
				<arg key="configFile" value="~/log4net.config" />
			</factoryAdapter>
		</logging>
	</common>
	<couchbase>
		<servers>
			<add uri="http://101.231.144.215:8091/pools" />
		</servers>
		<buckets>
			<add name="asp.net" useSsl="false" password="">
				<connectionPool name="custom" maxSize="200" minSize="20" />
			</add>
		</buckets>
	</couchbase>
	<rabbitmq hostName="172.16.20.1" userName="test" password="itsme999" virtualHost="/dev" />
	<zookeeper sessionTimeOut="30000" server="172.16.20.1" distributedLockPath="/DistributedLock" masterSlavePath="/MasterSlave/Service" />
	<runtime>
		<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
			<dependentAssembly>
				<assemblyIdentity name="log4net" publicKeyToken="1b44e1d426115821" culture="neutral" />
				<codeBase version="1.2.10.0" href="log4net.old.dll" />
			</dependentAssembly>
		</assemblyBinding>
	</runtime>
</configuration>
```
#### 如果是host在iis里则可能改如下配置
``` xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
	<runtime>
		<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
			<dependentAssembly>
				<assemblyIdentity name="log4net" publicKeyToken="1b44e1d426115821" culture="neutral" />
				<codeBase version="1.2.10.0" href="bin\old\log4net.dll" />
			</dependentAssembly>
		</assemblyBinding>
	</runtime>
</configuration>
```