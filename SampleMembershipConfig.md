# Introduction #

After [the framework config](SampleConfig.md) has been successfuly setup, the membership provider is ready to be used. Below shows an example for setting up System.BusinessObjects.Membership.


# Details #

```

<?xml version="1.0" encoding="utf-8" ?>
<configuration>

  <system.web>

    <profile defaultProvider="NHibernateProvider" enabled="true">
      <providers>
        <add name="NHibernateProvider" type="System.BusinessObjects.Membership.ProfileProvider, System.BusinessObjects.Membership"
             applicationName="Blazing.Membership"/>
      </providers>
      <properties>
        <add name="Test" type="System.String" allowAnonymous="true" />
      </properties>
    </profile>
    
    <roleManager enabled="true" defaultProvider="NHibernateProvider">
      <providers>
        <clear/>
        <add name="NHibernateProvider" type="System.BusinessObjects.Membership.RoleProvider, System.BusinessObjects.Membership"
             applicationName="Blazing.Membership"/>
      </providers>
    </roleManager>
    
    <membership defaultProvider="NHibernateProvider">
      <providers>
        <clear/>
        <add name="NHibernateProvider"
             type="System.BusinessObjects.Membership.MembershipProvider, System.BusinessObjects.Membership"
             applicationName="Blazing.Membership"
             enablePasswordRetrieval="true"
             enablePasswordReset="true"
             requiresQuestionAndAnswer="true"
             requiresUniqueEmail="true"
             passwordFormat="Encrypted"
             minRequiredPasswordLength="4"
             minRequiredNonalphanumericCharacters="0"/>
      </providers>
    </membership>
    
  </system.web>
</configuration>

```