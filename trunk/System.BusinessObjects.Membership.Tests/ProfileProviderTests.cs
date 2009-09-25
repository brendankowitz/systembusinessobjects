using System;
using System.Web.Profile;
using System.BusinessObjects.Transactions;
using Xunit;

namespace System.BusinessObjects.Membership.Tests
{
    public class ProfileProviderTests : NHibernateInMemoryTestFixtureBase
    {
        public void saveProfile(string user, bool saveUser)
        {
            if (saveUser)
            {
                User p = new User()
                {
                    Application = app,
                    IsAnonymous = false,
                    UserName = user
                };
                p.Save();
            }

            System.Configuration.SettingsContext sc = new System.Configuration.SettingsContext();
            sc.Add("UserName", user);
            sc.Add("IsAuthenticated", true);

            System.Configuration.SettingsPropertyValueCollection properties = new System.Configuration.SettingsPropertyValueCollection();

            var propVal = new System.Configuration.SettingsPropertyValue(
                new System.Configuration.SettingsProperty("Test") { DefaultValue = "", PropertyType = typeof(string) }
                );
            propVal.PropertyValue = "test string";
            properties.Add(propVal);

            ProfileManager.Provider.SetPropertyValues(sc, properties);
        }

        [Fact]
        public void DeleteInactiveProfiles()
        {
            System.Web.Profile.ProfileManager.DeleteInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption.Anonymous,
                DateTime.Now);
            UnitOfWork.CurrentSession.Flush();
        }

        [Fact]
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

            Assert.Equal(p.ID, u2.Profile.ID);
        }

        [Fact]
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

        [Fact]
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

            Assert.Equal("test string", collection["Test"].PropertyValue);
        }

        [Fact]
        public void GetAllProfiles()
        {
            saveProfile("user1", true);

            int total;
            ProfileInfoCollection profiles = ProfileManager.GetAllProfiles(ProfileAuthenticationOption.All, 0, 10, out total);

            Assert.Equal(1, total);
            Assert.Equal("user1", profiles["user1"].UserName);
        }

        [Fact]
        public void GetAllInactiveProfiles()
        {
            saveProfile("user1", false);

            ProfileInfoCollection profiles = ProfileManager.GetAllInactiveProfiles(ProfileAuthenticationOption.All, DateTime.Now);

            Assert.Equal(1, profiles.Count);
            Assert.Equal("user1", profiles["user1"].UserName);
        }

        [Fact]
        public void GetProfileCounts()
        {
            saveProfile("user1", false);

            int profiles = ProfileManager.GetNumberOfProfiles(ProfileAuthenticationOption.All);
            Assert.Equal(1, profiles);

            profiles = ProfileManager.GetNumberOfProfiles(ProfileAuthenticationOption.Authenticated);
            Assert.Equal(1, profiles);

            profiles = ProfileManager.GetNumberOfProfiles(ProfileAuthenticationOption.Anonymous);
            Assert.Equal(0, profiles);
        }

        [Fact]
        public void FindInactiveProfilesByUserName()
        {
            saveProfile("user1", false);

            ProfileInfoCollection profiles = ProfileManager.FindInactiveProfilesByUserName(ProfileAuthenticationOption.All, "?ser*", DateTime.Now);

            Assert.Equal(1, profiles.Count);
            Assert.Equal("user1", profiles["user1"].UserName);
        }

        [Fact]
        public void FindProfilesByUserName()
        {
            saveProfile("user1", false);

            ProfileInfoCollection profiles = ProfileManager.FindProfilesByUserName(ProfileAuthenticationOption.All, "?ser*");

            Assert.Equal(1, profiles.Count);
            Assert.Equal("user1", profiles["user1"].UserName);
        }

        [Fact]
        public void FindProfilesByUserName2()
        {
            saveProfile("user1", false);

            int total;
            ProfileInfoCollection profiles = ProfileManager.FindProfilesByUserName(ProfileAuthenticationOption.All, "?ser*", 0, 10, out total);

            Assert.Equal(1, total);
            Assert.Equal("user1", profiles["user1"].UserName);
        }

        [Fact]
        public void DeleteUserProfile()
        {
            saveProfile("user1", false);

            Assert.True(ProfileManager.DeleteProfile("user1"));
        }

        [Fact]
        public void DeleteUserProfile2()
        {
            saveProfile("user1", false);

            ProfileInfoCollection profiles = ProfileManager.FindProfilesByUserName(ProfileAuthenticationOption.All, "?ser*");

            int total = ProfileManager.DeleteProfiles(profiles);
            Assert.Equal(1, total);
        }
    }
}
