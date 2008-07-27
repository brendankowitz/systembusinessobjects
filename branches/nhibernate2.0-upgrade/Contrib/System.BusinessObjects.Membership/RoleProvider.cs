﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.BusinessObjects.Membership.Qry;
using System.Web.Hosting;
using System.Configuration.Provider;
using System.BusinessObjects.Transactions;

namespace System.BusinessObjects.Membership
{
    public class RoleProvider : System.Web.Security.RoleProvider
    {
        private Application application = new Application();

        public override string ApplicationName
        {
            get { return application.ApplicationName; }
            set { application.ApplicationName = value; }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from Web.config.
            if (null == config)
            {
                throw (new ArgumentNullException("config"));
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "NHibernateRoleProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "NHibernate Role Provider");
            }

            // Call the base class implementation.
            base.Initialize(name, config);

            // Load configuration data.
            string appName = GetConfigValue(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);
            application = Application.Fetch(QryFetchApplicationByName.Query(appName));

            if (application == null)
            {
                application = new Application()
                {
                    ID = Guid.NewGuid(),
                    ApplicationName = appName,
                    Description = config["description"],
                    LoweredApplicationName = appName.ToLower()
                };
                application.Save();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            // Add the users to the given roles.
            try
            {
                // For every user in the given list attempt to add to the given roles.
                foreach (string userName in usernames)
                {
                    User user = User.Fetch<User>(QryFetchUserByName.Query(userName, application.ID));
                    foreach (string roleName in roleNames)
                    {
                        Role role = Role.Fetch(QryFetchRoleByName.Query(roleName, application.ID));
                        int i = role.Users.Count;

                        role.Users.Add(user);
                        role.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to add users to roles.", ex);
            }
        }

        public override void CreateRole(string roleName)
        {
            // Make sure we are not attempting to insert an existing role.
            if (RoleExists(roleName))
            {
                throw new ProviderException("Role already exists.");
            }

            try
            {
                Role role = new Role();
                role.RoleName = roleName;
                role.Application = application;

                role.Save();
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to create role.", ex);
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            // Assume we are unable to perform the operation.
            bool result = false;

            if (throwOnPopulatedRole && (0 < GetUsersInRole(roleName).Length))
            {
                throw new ProviderException("Role is not empty.");
            }

            // Remove role information from the data store.
            try
            {
                // Get the role information.
                Role role = Role.Fetch(QryFetchRoleByName.Query(roleName, application.ID));

                if (role != null)
                {
                    // Delete the references to applications/roles.


                    role.Delete();
                    role.Save();

                    // Indicate no errors occured.
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to delete role.", ex);
            }

            // Return the result of the operation.
            return result;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            // Prepare a placeholder for the users.
            string[] userNames = new string[0];

            // Load the list of users for the given role name.
            try
            {
                // Replace all * and ? wildcards for % and _, respectively.
                usernameToMatch = usernameToMatch.Replace('*', '%');
                usernameToMatch = usernameToMatch.Replace('?', '_');

                // Perform the search.
                IList<User> users = User.Search<User>(QrySearchUsersInRole.Query(roleName, application.ID, usernameToMatch));

                if (null != users)
                {
                    userNames = new string[users.Count];
                    for (int i = 0; i < users.Count; i++)
                    {
                        userNames[i] = users[i].UserName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to find users in role.", ex);
            }

            // Return the result of the operation.
            return userNames;
        }

        public override string[] GetAllRoles()
        {
            // Prepare a placeholder for the roles.
            string[] roleNames = new string[0];

            // Load the list of roles for the configured application name.
            try
            {
                IList<Role> list = Role.Search(QrySearchRoles.Query(application.ID));

                if (0 < list.Count)
                {
                    roleNames = new string[list.Count];
                    int i = 0;
                    foreach (Role role in list)
                    {
                        roleNames[i++] = role.RoleName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to get all roles.", ex);
            }

            // Return the result of the operation.
            return roleNames;
        }

        public override string[] GetRolesForUser(string username)
        {
            // Prepare a placeholder for the roles.
            string[] roleNames = new string[0];

            // Load the list from the data store.
            try
            {
                Membership user = Membership.Fetch<Membership>(QryFetchMemberByName.Query(username, application.ID));
                IList<Role> roles = Role.Search(QrySearchRoles.Query(user.ID, application.ID));

                if (null != roles)
                {
                    roleNames = new string[roles.Count];
                    for (int i = 0; i < roles.Count; i++)
                    {
                        roleNames[i] = roles[i].RoleName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to get roles for user.", ex);
            }

            // Return the result of the operation.
            return roleNames;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            // Prepare a placeholder for the roles.
            string[] userNames = new string[0];

            // Load the list from the data store.
            try
            {
                Role role = Role.Fetch(QryFetchRoleByName.Query(roleName, application.ID));
                IList<User> users = User.Search(QrySearchUsersInRole.Query(role.ID, application.ID));

                if (null != users)
                {
                    userNames = new string[users.Count];
                    for (int i = 0; i < users.Count; i++)
                    {
                        userNames[i] = users[i].UserName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to get users in role.", ex);
            }

            // Return the result of the operation.
            return userNames;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            bool isInRole = false;
            try
            {
                isInRole = QrySearchUsersInRole.QueryCount(roleName, application.ID, username).UniqueResult<long>() > 0;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to find users in role.", ex);
            }
            return isInRole;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            try
            {
                foreach (string userName in usernames)
                {
                    User user = User.Fetch<User>(QryFetchUserByName.Query(userName, application.ID));

                    foreach (string roleName in roleNames)
                    {
                        Role role = Role.Fetch(QryFetchRoleByName.Query(roleName, application.ID));
                        role.AutoFlush = false; //turn framework autoflushing off

                        role.Users.Remove(user);
                        role.Save();
                    }
                }

                //flush all changes
                UnitOfWork.CurrentSession.Flush();
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to remove users from roles.", ex);
            }
        }

        public override bool RoleExists(string roleName)
        {
            // Assume the role does not exist.
            bool exists;

            // Check against the data store if the role exists.
            try
            {
                exists = (QryFetchRoleByName.QueryCount(roleName, application.ID).UniqueResult<int>() > 0);
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to check if role exists.", ex);
            }

            // Return the result of the operation.
            return exists;
        }

        internal static string GetConfigValue(string configValue, string defaultValue)
        {
            return (string.IsNullOrEmpty(configValue) ? defaultValue : configValue);
        }
    }
}