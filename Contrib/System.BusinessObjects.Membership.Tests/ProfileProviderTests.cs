using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Profile;
using System.BusinessObjects.Transactions;

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
            UnitOfWork.CurrentSession.Flush();
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
            
            var propVal = new System.Configuration.SettingsPropertyValue(
                new System.Configuration.SettingsProperty("Test") { DefaultValue = "", PropertyType=typeof(string)}
                );
            propVal.PropertyValue = "test string";
            properties.Add(propVal);

            ProfileManager.Provider.SetPropertyValues(sc, properties);

            User.Evict(p.ID);

            User u2 = User.Load(p.ID);

            Assert.AreEqual(p.ID, u2.Profile.ID);
        }

        [Test]
        public void SetPropertyValuesAnonnymous()
        {

            System.Configuration.SettingsContext sc = new System.Configuration.SettingsContext();
            sc.Add("UserName", new Guid().ToString());
            sc.Add("IsAuthenticated", false);

            System.Configuration.SettingsPropertyValueCollection properties = new System.Configuration.SettingsPropertyValueCollection();

            var nSettingsProp = new System.Configuration.SettingsProperty("Test") { DefaultValue = "", PropertyType = typeof(string) };
            nSettingsProp.Attributes.Add("AllowAnonymous", true);
            var propVal = new System.Configuration.SettingsPropertyValue(
                nSettingsProp
                );
            propVal.PropertyValue = "test string";
            properties.Add(propVal);

            ProfileManager.Provider.SetPropertyValues(sc, properties);
        }

        [Test]
        public void GetPropertyValues()
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

            var propVal = new System.Configuration.SettingsPropertyValue(
                new System.Configuration.SettingsProperty("Test") { DefaultValue = "", PropertyType = typeof(string) }
                );
            propVal.PropertyValue = "test string";
            properties.Add(propVal);
            ProfileManager.Provider.SetPropertyValues(sc, properties);
            User.Evict(p.ID);

            System.Configuration.SettingsPropertyCollection outCol = new System.Configuration.SettingsPropertyCollection();
            outCol.Add(new System.Configuration.SettingsProperty("Test") { DefaultValue = "", PropertyType = typeof(string) });
            System.Configuration.SettingsPropertyValueCollection collection = ProfileManager.Provider.GetPropertyValues(sc, outCol);

            Assert.AreEqual("test string", collection["Test"].PropertyValue);
        }
    }
}
