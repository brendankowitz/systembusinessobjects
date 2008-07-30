using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Profile;

namespace System.BusinessObjects.Membership.Tests
{
    [TestFixture]
    public class ProfileProviderTests : NHibernateInMemoryTestFixtureBase
    {
        [Test]
        public void DeleteInactiveProfiles()
        {
            System.Web.Profile.ProfileManager.DeleteInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption.Anonymous,
                DateTime.Now);
        }

        [Test]
        public void SetPropertyValues()
        {
            User p = new User()
            {
                Application = app,
                IsAnonymous = false,
                UserName = "user1"
            };
            p.Save();

            System.Configuration.SettingsContext sc = new System.Configuration.SettingsContext();
            sc.Add("UserName", "user1");
            sc.Add("IsAuthenticated", true);

            System.Configuration.SettingsPropertyValueCollection properties = new System.Configuration.SettingsPropertyValueCollection();
            properties.Add(new System.Configuration.SettingsPropertyValue(new System.Configuration.SettingsProperty("Prop")
                {
                    DefaultValue = ""
                })
                {
                    PropertyValue = "test string"
                });

            ProfileManager.Provider.SetPropertyValues(sc, properties);
        }
    }
}
