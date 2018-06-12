<configuration>
	<configSections>
		<sectionGroup name="common">
			<section name="logging" type="Common.Logging.ConfigurationSectionHandler, Common.Logging" />
		</sectionGroup>
		<section name="rabbitmq" type="Tuhu.MessageQueue.RabbitMQSection, Tuhu" />
		<section name="zookeeper" type="Tuhu.ZooKeeper.MasterSlaveClientSection, Tuhu.ZooKeeper" />
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
	<rabbitmq hostName="101.231.144.215" userName="test" password="itsme999" virtualHost="/dev" />
	<zookeeper sessionTimeOut="10000" server="101.231.144.215" path="/MasterSlave/Service/ServiceName" />
	<runtime>
		<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
			<dependentAssembly>
				<assemblyIdentity name="log4net" publicKeyToken="1b44e1d426115821" culture="neutral" />
				<codeBase version="1.2.10.0" href="log4net.old.dll" />
			</dependentAssembly>
		</assemblyBinding>
	</runtime>
</configuration>