using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web.Hosting;
using System.BusinessObjects.Membership.Qry;

namespace System.BusinessObjects.Membership
{
    public class RoleProvider : System.Web.Security.RoleProvider
    {
        private string applicationName = null;
        public Application Application
        {
            get
            {
                return ApplicationManager.GetApplication(applicationName);
            }
            set
            {
                applicationName = value.ApplicationName;
                ApplicationManager.SetApplication(applicationName, value);
            }
        }

        public override string ApplicationName
        {
            get
            {
                return applicationName;
            }
            set
            {
                applicationName = value;
            }
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
            Application = ApplicationManager.FetchApplication(appName, config["description"]);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            // Add the users to the given roles.
            try
            {
                var userRepository = RepositoryFactory.GetUserRepository();
                var roleRepository = RepositoryFactory.GetRoleRepository();

                // For every user in the given list attempt to add to the given roles.
                foreach (string userName in usernames)
                {
                    User user = userRepository.Fetch(new UserWithNameSpecification(userName), 
                        new UsersInApplicationSpecification(Application.ID));

                    if (user == null)
                    {
                        throw new ProviderException(string.Format("Unable find user {0} to add to role.", userName));
                    }
                    foreach (string roleName in roleNames)
                    {
                        Role role = roleRepository.Fetch(new RoleWithNameSpecification(roleName), 
                            new RoleInApplicationSpecification(Application.ID));

                        role.Users.Add(user);
                        user.Roles.Add(role);
                        userRepository.Save(user);
                    }
                }
                userRepository.SubmitChanges();
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

            var roleRepository = RepositoryFactory.GetRoleRepository();
            try
            {
                Role role = new Role();
                role.RoleName = roleName;
                role.Application = Application;

                roleRepository.Save(role);
                roleRepository.SubmitChanges();
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
            var roleRepository = RepositoryFactory.GetRoleRepository();
            try
            {
                // Get the role information.
                Role role = roleRepository.Fetch(new RoleWithNameSpecification(roleName), 
                    new RoleInApplicationSpecification(Application.ID));

                if (role != null)
                {
                    // Delete the references to applications/roles.


                    role.MarkDeleted();
                    roleRepository.Save(role);
                    roleRepository.SubmitChanges();
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
            List<string> userNames = new List<string>();

            // Load the list of users for the given role name.
            var userRepository = RepositoryFactory.GetUserRepository();
            var roleRepository = RepositoryFactory.GetRoleRepository();

            try
            {
                // Replace all * and ? wildcards for % and _, respectively.
                usernameToMatch = usernameToMatch.Replace('*', '%');
                usernameToMatch = usernameToMatch.Replace('?', '_');

                Role role = roleRepository.Fetch(new RoleWithNameSpecification(roleName), 
                            new RoleInApplicationSpecification(Application.ID));

                // Perform the search.
                IEnumerable<User> users = userRepository.Search(new UserWithNameLikeSpecification(usernameToMatch),
                    new UsersInApplicationSpecification(Application.ID),
                    new UserWithRoleSpecification(role));

                if (null != users)
                {
                    foreach (var user in users)
                    {
                        userNames.Add(user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to find users in role.", ex);
            }

            // Return the result of the operation.
            return userNames.ToArray();
        }

        public override string[] GetAllRoles()
        {
            // Prepare a placeholder for the roles.
            List<string> roleNames = new List<string>();

            // Load the list of roles for the configured application name.
            var roleRepository = RepositoryFactory.GetRoleRepository();
            try
            {
                IEnumerable<Role> list = roleRepository.Search(new RoleInApplicationSpecification(Application.ID));

                foreach (var role in list)
                {
                    roleNames.Add(role.RoleName);
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to get all roles.", ex);
            }

            // Return the result of the operation.
            return roleNames.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            // Prepare a placeholder for the roles.
            List<string> roleNames = new List<string>();

            // Load the list from the data store.
            var membershipRepository = RepositoryFactory.GetMembershipRepository();

            try
            {
                Membership user = membershipRepository.Fetch(new MemberWithNameSpecification(username),
                    new MembersInApplicationSpecification(Application.ID));
                IEnumerable<Role> roles = user.Roles;

                if (null != roles)
                {
                    foreach(var role in roles)
                    {
                        roleNames.Add(role.RoleName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to get roles for user.", ex);
            }

            // Return the result of the operation.
            return roleNames.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            // Prepare a placeholder for the roles.
            List<string> userNames = new List<string>();

            // Load the list from the data store.
            var userRepository = RepositoryFactory.GetUserRepository();
            var roleRepository = RepositoryFactory.GetRoleRepository();

            try
            {
                Role role = roleRepository.Fetch(new RoleWithNameSpecification(roleName),
                            new RoleInApplicationSpecification(Application.ID));

                IEnumerable<User> users = userRepository.Search(new UserWithRoleSpecification(role),
                    new UsersInApplicationSpecification(Application.ID));

                if (null != users)
                {
                    foreach (var user in users)
                    {
                        userNames.Add(user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to get users in role.", ex);
            }

            // Return the result of the operation.
            return userNames.ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            bool isInRole = false;

            var membershipRepository = RepositoryFactory.GetMembershipRepository();
            var roleRepository = RepositoryFactory.GetRoleRepository();

            try
            {
                Role role = roleRepository.Fetch(new RoleWithNameSpecification(roleName),
                            new RoleInApplicationSpecification(Application.ID));

                var userCount = membershipRepository.AsQueryable(new MemberWithNameSpecification(username),
                    new MembersInApplicationSpecification(Application.ID),
                    new MemberWithRoleSpecification(role));

                isInRole = userCount.Count() > 0;
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
                var userRepository = RepositoryFactory.GetUserRepository();
                var roleRepository = RepositoryFactory.GetRoleRepository();

                foreach (string userName in usernames)
                {
                    User user = userRepository.Fetch(new UserWithNameSpecification(userName),
                        new UsersInApplicationSpecification(Application.ID));

                    foreach (string roleName in roleNames)
                    {
                        Role role = roleRepository.Fetch(new RoleWithNameSpecification(roleName),
                            new RoleInApplicationSpecification(Application.ID));

                        role.Users.Remove(user);
                        user.Roles.Remove(role);
                        roleRepository.Save(role);
                    }
                }
                roleRepository.SubmitChanges();
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
            var roleRepository = RepositoryFactory.GetRoleRepository();

            try
            {
                var roleExists = roleRepository.AsQueryable(new RoleWithNameSpecification(roleName),
                    new RoleInApplicationSpecification(Application.ID));

                exists = roleExists.Count() > 0;
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
