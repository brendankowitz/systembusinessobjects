using System;
using System.Web.Security;
using System.BusinessObjects.Membership.Qry;
using System.Web.Profile;
using Xunit;

namespace System.BusinessObjects.Membership.Tests
{
    public class MembershipProviderTests : NHibernateInMemoryTestFixtureBase
    {       
        public static void CreateUser()
        {
            System.Web.Security.MembershipCreateStatus status;
             MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                        "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);
            Assert.Equal("user1", user.UserName);
            Assert.Equal("test@test.com", user.Email);
            Assert.Equal("question?", user.PasswordQuestion);
            Assert.Equal(true, user.IsApproved);
        }

        [Fact]
        public void CreateUserTest()
        {
            CreateUser();
        }

        [Fact]
        public void CanFetchApplication()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            var repository = GetMembershipRepository<Application>();
            var userRepository = GetMembershipRepository<User>();

            Application app = repository.Fetch(new ApplicationWithNameSpecification("Blazing.Membership"));
            
            Assert.NotNull(app);

            User u = userRepository
                .Fetch(new UserWithNameSpecification("user1"), 
                    new UsersInApplicationSpecification(app.ID));

            Assert.Equal(u.Application.ID, app.ID);
        }

        [Fact]
        public void GetAllUsers_Paging()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test1@test.com",
                                                       "question?", "yes", true, out status);
            MembershipUser user2 = System.Web.Security.Membership.CreateUser("user2", "password", "test2@test.com",
                                                       "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            session.Flush();

            int total;
            MembershipUserCollection col = System.Web.Security.Membership.GetAllUsers(0, 1, out total);

            Assert.Equal(1, col.Count);
            Assert.Equal(2, total);
        }

        [Fact]
        public void GetUserNameByEmail()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test123@test.com",
                                                       "question?", "yes", true, out status);
            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            string username = System.Web.Security.Membership.GetUserNameByEmail("test123@test.com");
            Assert.Equal("user1", username);
        }

        [Fact]
        public void GetUsersByEmail_Paging()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test123@test.com",
                                                       "question?", "yes", true, out status);
            MembershipUser user2 = System.Web.Security.Membership.CreateUser("user2", "password", "test321@test.net",
                                                       "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            int total;
            MembershipUserCollection col = System.Web.Security.Membership.FindUsersByEmail("test???@test*", 0, 1, out total);

            Assert.Equal(1, col.Count);
            Assert.Equal(2, total);
        }

        [Fact]
        public void GetUsersByName_Paging()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test123@test.com",
                                                       "question?", "yes", true, out status);
            MembershipUser user2 = System.Web.Security.Membership.CreateUser("user2", "password", "test321@test.net",
                                                       "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            int total;
            MembershipUserCollection col = System.Web.Security.Membership.FindUsersByName("?ser%", 0, 1, out total);

            Assert.Equal(1, col.Count);
            Assert.Equal(2, total);
        }

        [Fact]
        public void UsersOnline()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            user.LastActivityDate = DateTime.Now;
            System.Web.Security.Membership.UpdateUser(user);

            Assert.Equal(1, System.Web.Security.Membership.GetNumberOfUsersOnline());
        }

        [Fact]
        public void UpdateUser()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            user.Comment = "Hello Comment";
            System.Web.Security.Membership.UpdateUser(user);
        }

        [Fact]
        public void GetProviderUser()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            object providerKey = user.ProviderUserKey;

            MembershipUser testUser = System.Web.Security.Membership.GetUser(providerKey, false);

            Assert.Equal(user.UserName, testUser.UserName);
        }

        [Fact]
        public void ChangePassword()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            user.ChangePassword("password", "password1");
        }

        [Fact]
        public void ChangePasswordQuestion()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            bool retval = user.ChangePasswordQuestionAndAnswer("password", "new question", "new answer");
            Assert.True(retval);
        }

        [Fact]
        public void ResetPassword()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            string newpassword = user.ResetPassword("yes");
        }

        [Fact]
        public void GetPassword()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            string password = user.GetPassword("yes");
            Assert.Equal("password", password);
        }

        [Fact]
        public void UnlockUser()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            bool retval = user.UnlockUser();
            Assert.True(retval);
        }

        [Fact]
        public void ApproveUser()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            user.IsApproved = true;
            System.Web.Security.Membership.UpdateUser(user);
        }

        [Fact]
        public void DeleteUser()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

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

            Assert.Equal(System.Web.Security.MembershipCreateStatus.Success, status);

            System.Web.Security.Membership.DeleteUser("user1");
        }
    }
}
