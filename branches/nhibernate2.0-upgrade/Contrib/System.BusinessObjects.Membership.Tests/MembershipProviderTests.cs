using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Security;
using System.BusinessObjects.Membership.Qry;

namespace System.BusinessObjects.Membership.Tests
{
    [TestFixture]
    public class MembershipProviderTests : NHibernateInMemoryTestFixtureBase
    {
        [Test]
        public void CreateUser()
        {
            System.Web.Security.MembershipCreateStatus status;
             MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                        "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail();

            Assert.AreEqual("user1", user.UserName);
            Assert.AreEqual("test@test.com", user.Email);
            Assert.AreEqual("question?", user.PasswordQuestion);
            Assert.AreEqual(true, user.IsApproved);
        }

        [Test]
        [Ignore("Fails when run as part of a fixture...dont know why...maybe sqlite")]
        public void CanFetchApplication()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            Application app = Application.Fetch(QryFetchApplicationByName.Query("Blazing.Membership"));
            
            Assert.IsNotNull(app);

            User u = User.Fetch(QryFetchUserByName.Query("user1", app.ID));

            Assert.AreEqual(u.Application.ID, app.ID);
        }

        [Test]
        public void GetAllUsers_Paging()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test1@test.com",
                                                       "question?", "yes", true, out status);
            MembershipUser user2 = System.Web.Security.Membership.CreateUser("user2", "password", "test2@test.com",
                                                       "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail(status.ToString());

            int total;
            MembershipUserCollection col = System.Web.Security.Membership.GetAllUsers(0, 1, out total);

            Assert.AreEqual(1, col.Count);
            Assert.AreEqual(2, total);
        }

        [Test]
        public void GetUsersByEmail_Paging()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test123@test.com",
                                                       "question?", "yes", true, out status);
            MembershipUser user2 = System.Web.Security.Membership.CreateUser("user2", "password", "test321@test.net",
                                                       "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail();

            int total;
            MembershipUserCollection col = System.Web.Security.Membership.FindUsersByEmail("test???@test*", 0, 1, out total);

            Assert.AreEqual(1, col.Count);
            Assert.AreEqual(2, total);
        }

        [Test]
        public void GetUsersByName_Paging()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test123@test.com",
                                                       "question?", "yes", true, out status);
            MembershipUser user2 = System.Web.Security.Membership.CreateUser("user2", "password", "test321@test.net",
                                                       "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail();

            int total;
            MembershipUserCollection col = System.Web.Security.Membership.FindUsersByName("?ser%", 0, 1, out total);

            Assert.AreEqual(1, col.Count);
            Assert.AreEqual(2, total);
        }

        [Test]
        public void UsersOnline()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail();

            user.LastActivityDate = DateTime.Now;
            System.Web.Security.Membership.UpdateUser(user);

            Assert.AreEqual(1, System.Web.Security.Membership.GetNumberOfUsersOnline());
        }

        [Test]
        public void UpdateUser()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail();

            user.Comment = "Hello Comment";
            System.Web.Security.Membership.UpdateUser(user);
        }

        [Test]
        public void GetProviderUser()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail();

            object providerKey = user.ProviderUserKey;

            MembershipUser testUser = System.Web.Security.Membership.GetUser(providerKey, false);

            Assert.AreEqual(user.UserName, testUser.UserName);
        }

        [Test]
        public void ChangePassword()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail();

            user.ChangePassword("password", "password1");
        }

        [Test]
        public void ChangePasswordQuestion()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail();

            bool retval = user.ChangePasswordQuestionAndAnswer("password", "new question", "new answer");
            Assert.IsTrue(retval);
        }

        [Test]
        public void ResetPassword()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail();

            string newpassword = user.ResetPassword("yes");
        }

        [Test]
        public void GetPassword()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail();

            string password = user.GetPassword("yes");
            Assert.AreEqual("password", password);
        }

        [Test]
        public void UnlockUser()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail();

            bool retval = user.UnlockUser();
            Assert.IsTrue(retval);
        }

        [Test]
        public void ApproveUser()
        {
            System.Web.Security.MembershipCreateStatus status;
            MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            if (status != System.Web.Security.MembershipCreateStatus.Success)
                Assert.Fail();

            user.IsApproved = true;
            System.Web.Security.Membership.UpdateUser(user);
        }
    }
}
