using System;
using Xunit;

namespace System.BusinessObjects.Membership.Tests
{
    public class RoleProviderTests : NHibernateInMemoryTestFixtureBase
    {
        [Fact]
        public void AddRole()
        {
            System.Web.Security.Roles.CreateRole("admin");
        }

        [Fact]
        public void DeleteRole()
        {
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.Roles.DeleteRole("admin");
        }

        [Fact]
        public void AddUsersToRole()
        {
            MembershipProviderTests.CreateUser();
            System.Web.Security.Roles.CreateRole("admin");

            System.Web.Security.Roles.AddUsersToRole(new string[] { "user1" }, "admin");
        }

        [Fact]
        public void AddUserToRole()
        {
            MembershipProviderTests.CreateUser();
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.Roles.AddUserToRole("user1", "admin");
        }

        [Fact]
        public void GetRolesForUser()
        {
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.MembershipCreateStatus status;
            System.Web.Security.MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            System.Web.Security.Roles.AddUsersToRole(new string[] { "user1" }, "admin");

            string[] list = System.Web.Security.Roles.GetRolesForUser("user1");
            Assert.Equal(1, list.Length);
        }

        [Fact]
        public void GetUsersInRole()
        {
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.MembershipCreateStatus status;
            System.Web.Security.MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);

            System.Web.Security.Roles.AddUsersToRole(new string[] { "user1" }, "admin");

            session.Flush();

            string[] list = System.Web.Security.Roles.GetUsersInRole("admin");

            Assert.Equal(1, list.Length);
            Assert.Equal("user1", list[0]);
        }

        [Fact]
        public void FindUsersInRole()
        {
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.MembershipCreateStatus status;

            System.Web.Security.MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);
            System.Web.Security.MembershipUser user2 = System.Web.Security.Membership.CreateUser("user2", "password", "test2@test.com",
                                                        "question?", "yes", true, out status);

            System.Web.Security.Roles.AddUsersToRole(new string[] { "user1", "user2" }, "admin");

            session.Flush();

            string[] list = System.Web.Security.Roles.FindUsersInRole("admin", "?ser%");

            Assert.Equal(2, list.Length);
        }

        [Fact]
        public void IsUserInRole()
        {
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.MembershipCreateStatus status;

            System.Web.Security.MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);
            System.Web.Security.MembershipUser user2 = System.Web.Security.Membership.CreateUser("user2", "password", "test2@test.com",
                                                        "question?", "yes", true, out status);

            System.Web.Security.Roles.AddUsersToRole(new string[] { "user1", "user2" }, "admin");

            session.Flush();

            Assert.True(System.Web.Security.Roles.IsUserInRole("user2", "admin"));
            Assert.False(System.Web.Security.Roles.IsUserInRole("user3", "admin"));
        }

        [Fact]
        public void GetAllRoles()
        {
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.Roles.CreateRole("users");

            string[] roles = System.Web.Security.Roles.GetAllRoles();
            Assert.Equal(2, roles.Length);
        }

        [Fact]
        public void RemoveUsersFromRoles()
        {
            System.Web.Security.Roles.CreateRole("admin");
            System.Web.Security.MembershipCreateStatus status;

            System.Web.Security.MembershipUser user = System.Web.Security.Membership.CreateUser("user1", "password", "test@test.com",
                                                       "question?", "yes", true, out status);
            System.Web.Security.MembershipUser user2 = System.Web.Security.Membership.CreateUser("user2", "password", "test2@test.com",
                                                        "question?", "yes", true, out status);

            System.Web.Security.Roles.AddUsersToRole(new string[] { "user1", "user2" }, "admin");

            session.Flush();

            Assert.True(System.Web.Security.Roles.IsUserInRole("user2", "admin"));

            System.Web.Security.Roles.RemoveUsersFromRoles(new string[] { "user2" }, new string[] { "admin" });

            session.Flush();

            Assert.False(System.Web.Security.Roles.IsUserInRole("user2", "admin"));
        }
    }
}
