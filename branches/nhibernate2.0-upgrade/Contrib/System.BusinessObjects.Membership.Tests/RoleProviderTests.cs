using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace System.BusinessObjects.Membership.Tests
{
    [TestFixture]
    public class RoleProviderTests : NHibernateInMemoryTestFixtureBase
    {
        [Test]
        public void AddRole()
        {
            System.Web.Security.Roles.CreateRole("admin");
        }

        [Test]
        public void DeleteRole()
        {
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.Roles.DeleteRole("admin");
        }

        [Test]
        public void AddUsersToRole()
        {
            System.Web.Security.Roles.CreateRole("admin");
            new MembershipProviderTests().CreateUser();

            System.Web.Security.Roles.AddUsersToRole(new string[] { "user1" }, "admin");
        }

        [Test]
        public void GetRolesForUser()
        {
            System.Web.Security.Roles.CreateRole("admin");
            new MembershipProviderTests().CreateUser();

            System.Web.Security.Roles.AddUsersToRole(new string[] { "user1" }, "admin");

            string[] list = System.Web.Security.Roles.GetRolesForUser("user1");
            Assert.AreEqual(1, list.Length);
        }

        [Test]
        public void GetUsersInRole()
        {
            System.Web.Security.Roles.CreateRole("admin");
            new MembershipProviderTests().CreateUser();

            System.Web.Security.Roles.AddUsersToRole(new string[] { "user1" }, "admin");

            string[] list = System.Web.Security.Roles.GetUsersInRole("admin");

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual("user1", list[0]);
        }

        [Test]
        public void FindUsersInRole()
        {
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.MembershipCreateStatus status;

            System.Web.Security.MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);
            System.Web.Security.MembershipUser user2 = System.Web.Security.Membership.CreateUser("user2", "password", "test2@test.com",
                                                        "question?", "yes", true, out status);

            System.Web.Security.Roles.AddUsersToRole(new string[] { "user1", "user2" }, "admin");

            string[] list = System.Web.Security.Roles.FindUsersInRole("admin", "?ser%");

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual("user1", list[0]);
        }

        [Test]
        public void IsUserInRole()
        {
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.MembershipCreateStatus status;

            System.Web.Security.MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);
            System.Web.Security.MembershipUser user2 = System.Web.Security.Membership.CreateUser("user2", "password", "test2@test.com",
                                                        "question?", "yes", true, out status);

            System.Web.Security.Roles.AddUsersToRole(new string[] { "user1", "user2" }, "admin");

            Assert.IsTrue(System.Web.Security.Roles.IsUserInRole("user2", "admin"));
            Assert.IsFalse(System.Web.Security.Roles.IsUserInRole("user3", "admin"));
        }

        [Test]
        public void GetAllRoles()
        {
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.Roles.CreateRole("users");

            string[] roles = System.Web.Security.Roles.GetAllRoles();
            Assert.AreEqual(2, roles.Length);
        }

        [Test]
        public void RemoveUsersFromRoles()
        {
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.MembershipCreateStatus status;

            System.Web.Security.MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);
            System.Web.Security.MembershipUser user2 = System.Web.Security.Membership.CreateUser("user2", "password", "test2@test.com",
                                                        "question?", "yes", true, out status);

            System.Web.Security.Roles.AddUsersToRole(new string[] { "user1", "user2" }, "admin");

            Assert.IsTrue(System.Web.Security.Roles.IsUserInRole("user2", "admin"));

            System.Web.Security.Roles.RemoveUsersFromRoles(new string[] { "user2" }, new string[] { "admin" });

            Assert.IsFalse(System.Web.Security.Roles.IsUserInRole("user2", "admin"));
        }
    }
}
