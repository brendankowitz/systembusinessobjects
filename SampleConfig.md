# Introduction #

Below demonstrates a sample web.config file for using System.BusinessObjects.Framework in a Web application.

# Details #

```
<?xml version="1.0"?>

<configuration>
	<configSections>
		<section name="NHibernateSessionProvider" type="System.BusinessObjects.Providers.ProviderSectionHandler, System.BusinessObjects.Framework" requirePermission="false" allowDefinition="MachineToApplication"/>
		<section name="DataCache" type="System.BusinessObjects.Providers.ProviderSectionHandler, System.BusinessObjects.Framework" requirePermission="false" allowDefinition="MachineToApplication"/>
		<section name="hibernate-configuration" type="NHibernate.Cfg.ConfigurationSectionHandler, NHibernate" requirePermission="false" allowDefinition="MachineToApplication"/>
  </configSections>
  
    <appSettings/>
    <connectionStrings/>

	<NHibernateSessionProvider defaultProvider="NHibernateAspContextProvider">
		<providers>
			<add name="NHibernateAspContextProvider" type="System.BusinessObjects.Data.NHibernateAspContextProvider, System.BusinessObjects.Framework"/>
		</providers>
	</NHibernateSessionProvider>

	<DataCache defaultProvider="AspNetDataCache">
		<providers>
			<add name="AspNetDataCache" type="System.BusinessObjects.Data.AspNetDataCache, System.BusinessObjects.Framework" UseCache="true" DefaultCacheTimeout="120"/>
		</providers>
	</DataCache>
	
	<hibernate-configuration xmlns="urn:nhibernate-configuration-2.2">
		<session-factory>

			<property name="dialect">NHibernate.Dialect.MySQL5Dialect</property>
			<property name="connection.provider">NHibernate.Connection.DriverConnectionProvider</property>
			<property name="connection.driver_class">NHibernate.Driver.MySqlDataDriver</property>
			<property name="connection.connection_string">Server=.;Database=addressbook_sample;Uid=sample;Pwd=sample;</property>
			<property name="current_session_context_class">NHibernate.Context.ManagedWebSessionContext</property>
			<property name="show_sql">true</property>
			
			<mapping assembly="Sample.BusinessObjects"/>
		</session-factory>
	</hibernate-configuration>

    <system.web>
	<!-- .. -->
    </system.web>
</configuration>

```