using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using System.Security.Cryptography;
using System.Web.Hosting;
using System.BusinessObjects.Membership.Qry;
using System.Configuration.Provider;
using SystemWeb = System.Web.Security;

namespace System.BusinessObjects.Membership
{
    public class MembershipProvider : SystemWeb.MembershipProvider
    {
        private const string STR_UnableToGetMembershipUser = "Unable to get membership user";
        private const string STR_TooManyMatchingUsers = "Too many matching users";
        private const string STR_PasswordResetCancelledBecauseAccountItLocked = "Password reset cancelled because account it locked.";

        #region Fields
        private Application application = new Application();
        private bool requiresQuestionAndAnswer;
        private bool requiresUniqueEmail;
        private bool enablePasswordRetrieval;
        private bool enablePasswordReset;
        private int maxInvalidPasswordAttempts;
        private int passwordAttemptWindow;
        private SystemWeb.MembershipPasswordFormat passwordFormat;
        private int minRequiredPasswordLength;
        private int minRequiredNonAlphanumericCharacters;
        private string passwordStrengthRegularExpression;
        private MachineKeySection machineKey;
        #endregion Fields

        #region Properties
        /// <summary>
        /// The name of the application using the custom membership provider.
        /// </summary>
        /// <returns>
        /// The name of the application using the custom membership provider.
        /// </returns>
        public override string ApplicationName
        {
            get { return application.ApplicationName; }
            set { application.ApplicationName = value; }
        }
        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require
        /// the user to answer a password question for password reset and retrieval.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a password answer is required for password reset and retrieval;
        /// otherwise, <c>false</c>. The default is <c>true</c>.
        /// </returns>
        public override bool RequiresQuestionAndAnswer
        {
            get { return requiresQuestionAndAnswer; }
        }
        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require
        /// a unique e-mail address for each user name.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the membership provider requires a unique e-mail address;
        /// otherwise, <c>false</c>. The default is <c>true</c>.
        /// </returns>
        public override bool RequiresUniqueEmail
        {
            get { return requiresUniqueEmail; }
        }
        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to retrieve
        /// their passwords.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the membership provider is configured to support password retrieval;
        /// otherwise, <c>false</c>. The default is <c>false</c>.
        /// </returns>
        public override bool EnablePasswordRetrieval
        {
            get { return enablePasswordRetrieval; }
        }
        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their
        /// passwords.
        /// </summary>
        ///<returns>
        ///<c>true</c> if the membership provider supports password reset; otherwise, <c>false</c>.
        /// The default is <c>true</c>.
        ///</returns>
        public override bool EnablePasswordReset
        {
            get { return enablePasswordReset; }
        }
        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the
        /// membership user is locked out.
        /// </summary>
        /// <returns>
        /// The number of invalid password or password-answer attempts allowed before the membership
        /// user is locked out.
        /// </returns>
        public override int MaxInvalidPasswordAttempts
        {
            get { return maxInvalidPasswordAttempts; }
        }
        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer
        /// attempts are allowed before the membership user is locked out.
        /// </summary>
        /// <returns>
        /// The number of minutes in which a maximum number of invalid password or password-answer attempts
        /// are allowed before the membership user is locked out.
        /// </returns>
        public override int PasswordAttemptWindow
        {
            get { return passwordAttemptWindow; }
        }
        /// <summary>
        /// Gets a value indicating the format for storing passwords in the data store.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Web.Security.MembershipPasswordFormat"></see> values indicating
        /// the format for storing passwords in the data store.
        /// </returns>
        public override SystemWeb.MembershipPasswordFormat PasswordFormat
        {
            get { return passwordFormat; }
        }
        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <returns>
        /// The minimum length required for a password. 
        /// </returns>
        public override int MinRequiredPasswordLength
        {
            get { return minRequiredPasswordLength; }
        }
        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <returns>
        /// The minimum number of special characters that must be present in a valid password.
        /// </returns>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return minRequiredNonAlphanumericCharacters; }
        }
        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <returns>
        /// A regular expression used to evaluate a password.
        /// </returns>
        public override string PasswordStrengthRegularExpression
        {
            get { return passwordStrengthRegularExpression; }
        }
        #endregion Properties

        #region Initialization
        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific
        /// attributes specified in the configuration for this provider.</param>
        /// <param name="name">The friendly name of the provider.</param>
        /// <exception cref="ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="InvalidOperationException">An attempt is made to call <see cref="Initialize(System.String,System.Collections.Specialized.NameValueCollection)"></see> on a provider after the provider has already been initialized.</exception>
        /// <exception cref="ArgumentException">The name of the provider has a length of zero.</exception>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            // Initialize values from Web.config.
            if (null == config)
            {
                throw (new ArgumentNullException("config"));
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "NHibernateMembershipProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "NHibernate Membership Provider");
            }
            // Call the base class implementation.
            base.Initialize(name, config);

            // Load configuration data.
            string appName = GetConfigValue(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);
            application = Application.Fetch(QryFetchApplicationByName.Query(appName));

            if(application == null)
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

             requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "False"));
             requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "True"));
             enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "True"));
             enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "True"));
             maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
             passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
             minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
             minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredAlphaNumericCharacters"], "1"));
             passwordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], string.Empty));

             // Initialize the password format.
             switch (GetConfigValue(config["passwordFormat"], "Hashed"))
             {
                 case "Hashed":
                     passwordFormat = SystemWeb.MembershipPasswordFormat.Hashed;
                     break;
                 case "Encrypted":
                     passwordFormat = SystemWeb.MembershipPasswordFormat.Encrypted;
                     break;
                 case "Clear":
                     passwordFormat = SystemWeb.MembershipPasswordFormat.Clear;
                     break;
                 default:
                     throw new ProviderException("password format not supported");
             }

             Configuration.Configuration cfg = WebConfigurationManager.OpenWebConfiguration(HostingEnvironment.ApplicationVirtualPath);
             machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");
             if ("Auto".Equals(machineKey.Decryption))
             {
                 // Create our own key if one has not been specified.
                 machineKey.DecryptionKey = CreateKey(24);
                 machineKey.ValidationKey = CreateKey(64);
             }
        }
        #endregion Initialization

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            // Assume we are unable to perform the operation.
            bool result = false;

            // Ensure we are dealing with a valid user.
            if (ValidateUser(username, oldPassword))
            {
                // Raise the ValidatingPassword event in case an event handler has been defined.
                SystemWeb.ValidatePasswordEventArgs args = new SystemWeb.ValidatePasswordEventArgs(username, newPassword, true);
                OnValidatingPassword(args);
                if (args.Cancel)
                {
                    // Check for a specific error message.
                    if (null != args.FailureInformation)
                    {
                        throw (args.FailureInformation);
                    }
                    else
                    {
                        throw new ProviderException("Password change cancelled.");
                    }
                }

                // Get the user from the data store.
                Membership user = Membership.Fetch<Membership>(QryFetchMemberByName.Query(username, application.ID));

                if (null != user)
                {
                    try
                    {
                        // Encode the new password.
                        user.Password = EncodePassword(newPassword, user.PasswordSalt);
                        user.LastPasswordChangedDate = DateTime.Now;
                        user.LastActivityDate = DateTime.Now;

                        // Update user record with the new password.
                        user.Save();
                        result = true;
                    }
                    catch
                    {
                        throw new SystemWeb.MembershipPasswordException("Password change cancelled due to account being locked.");
                    }
                }
            }

            // Return the result of the operation.
            return result;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            // Assume we are unable to perform the operation.
            bool result = false;

            // Ensure we are dealing with a valid user.
            if (ValidateUser(username, password))
            {
                // Get the user from the data store.
                Membership user = Membership.Fetch<Membership>(QryFetchMemberByName.Query(username, application.ID));
                if (null != user)
                {
                    try
                    {
                        // Update the new password question and answer.
                        user.PasswordQuestion = newPasswordQuestion;
                        user.PasswordAnswer = EncodePassword(newPasswordAnswer, user.PasswordSalt);
                        user.LastActivityDate = DateTime.Now;
                        // Update user record with the new password.
                        user.Save();
                        // Indicate a successful operation.
                        result = true;
                    }
                    catch
                    {
                        throw new SystemWeb.MembershipPasswordException("Unable to change account security question and answer.");
                    }
                }
            }

            // Return the result of the operation.
            return result;
        }

        public override SystemWeb.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out SystemWeb.MembershipCreateStatus status)
        {
            SystemWeb.ValidatePasswordEventArgs args = new SystemWeb.ValidatePasswordEventArgs(username, password, true);
            
            OnValidatingPassword(args);
            
            if (args.Cancel)
            {
                status = SystemWeb.MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && !string.IsNullOrEmpty(GetUserNameByEmail(email)))
            {
                status = SystemWeb.MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            if (QryFetchUserByName.QueryCount(username, application.ID).UniqueResult<int>() > 0)
            {
                status = SystemWeb.MembershipCreateStatus.DuplicateUserName;
            }
            else
            {
                Membership user = new Membership();
                user.UserName = username;
                user.IsAnonymous = false;
                user.Application = application;

                user.Password = EncodePassword(password, machineKey.ValidationKey);
                user.PasswordFormat = (int)PasswordFormat;
                user.PasswordSalt = machineKey.ValidationKey;
                user.Email = email;
                user.PasswordQuestion = passwordQuestion;
                user.PasswordAnswer = EncodePassword(passwordAnswer, machineKey.ValidationKey);
                user.IsApproved = isApproved;

                try
                {
                    user.Save();
                    status = SystemWeb.MembershipCreateStatus.Success;
                }
                catch (Exception ex)
                {
                    throw new ProviderException("Failed to create user", ex);
                }


                return GetUser(username, false);
            }
            return null;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            // Assume we are unable to perform the operation.
            bool result;

            // Delete the corresponding user record from the data store.
            try
            {
                // Get the user information.
                IList<Membership> members = Membership.Search <Membership>(QryFetchMemberByName.Query(username, application.ID));

                if (members != null && members.Count == 1)
                {
                    // Process commands to delete all data for the user in the database.
                    if (deleteAllRelatedData)
                    {
                        throw new NotImplementedException();
                    }

                    // Delete the user record.
                    members[0].Delete();
                    members[0].Save();
                }

                result = true;
            }
            catch (Exception ex)
            {
                throw new ProviderException("User was unable to be deleted.", ex);
            }

            // Return the result of the operation.
            return result;
        }

        public override SystemWeb.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            // Create a placeholder for all user accounts retrived, if any.
            SystemWeb.MembershipUserCollection users = new SystemWeb.MembershipUserCollection();

            // Get the user record from the data store.
            try
            {
                // Replace all * and ? wildcards for % and _, respectively.
                emailToMatch = emailToMatch.Replace('*', '%');
                emailToMatch = emailToMatch.Replace('?', '_');

                // Perform the search.
                IList<Membership> page = Membership.Search<Membership>(QrySearchMemberByEmail.Query(emailToMatch, application.ID, pageSize, pageIndex));
                totalRecords = QrySearchMemberByEmail.QueryCount(emailToMatch, application.ID).UniqueResult<int>();
                
                foreach (Membership appUser in page)
                {
                    users.Add(appUser.ToMembershipUser(Name));
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to get users by email", ex);
            }

            // Return the result of the operation.
            return users;
        }

        public override SystemWeb.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            // Create a placeholder for all user accounts retrived, if any.
            SystemWeb.MembershipUserCollection users = new SystemWeb.MembershipUserCollection();

            // Get the user record from the data store.
            try
            {
                // Replace all * and ? wildcards for % and _, respectively.
                usernameToMatch = usernameToMatch.Replace('*', '%');
                usernameToMatch = usernameToMatch.Replace('?', '_');

                // Perform the search.
                IList<Membership> page = Membership.Search<Membership>(QrySearchMemberByName.Query(usernameToMatch, application.ID, pageSize, pageIndex));
                totalRecords = QrySearchMemberByName.QueryCount(usernameToMatch, application.ID).UniqueResult<int>();

                foreach (Membership appUser in page)
                {
                    users.Add(appUser.ToMembershipUser(Name));
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to get users by name.", ex);
            }

            // Return the result of the operation.
            return users;
        }

        public override SystemWeb.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            // Create a placeholder for all user accounts retrived, if any.
            SystemWeb.MembershipUserCollection users = new SystemWeb.MembershipUserCollection();

            try
            {
                IList<Membership> page = Membership.Search<Membership>(QrySearchAllMembers.Query(application.ID, pageIndex, pageSize));
                totalRecords = QrySearchAllMembers.QueryCount(application.ID).UniqueResult<int>();

                foreach (Membership appUser in page)
                {
                    users.Add(appUser.ToMembershipUser(Name));
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to page through all users.", ex);
            }

            // Return the result of the operation.
            return users;
        }

        public override int GetNumberOfUsersOnline()
        {
            // Assume there are no users online.
            int numberOfUsersOnline;

            // Get a count of users whose LastActivityDate is greater than the threashold.
            try
            {
                // Determine the threashold based on the configured time window against which we'll compare.
                TimeSpan onlineSpan = new TimeSpan(0, SystemWeb.Membership.UserIsOnlineTimeWindow, 0);
                DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

                numberOfUsersOnline = QrySearchRecentlyActiveUsers.QueryCount(compareTime, application.ID).UniqueResult<int>();
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to get number of users online", ex);
            }

            // Return the result of the operation.
            return numberOfUsersOnline;
        }

        public override string GetPassword(string username, string answer)
        {
            // Assume we are not able to fetch the user's password.
            string password = null;

            // Ensure password retrievals are allowed.
            if (!EnablePasswordRetrieval)
            {
                throw new ProviderException("Password retrieval not enabled.");
            }
            // Is the request made when the password in Hashed?
            if (SystemWeb.MembershipPasswordFormat.Hashed == PasswordFormat)
            {
                throw new ProviderException("Hashed passwords cannot be retrieved.");
            }

            // Get the user from the data store.
            Membership user = Membership.Fetch<Membership>(QryFetchMemberByName.Query(username, application.ID));
            if (null != user)
            {
                // Determine if the user is required to answer a password question.
                if (RequiresQuestionAndAnswer && !CheckPassword(answer, user.PasswordAnswer, user.PasswordSalt))
                {
                    user.FailedPasswordAnswerAttemptCount++;
                    user.Save();

                    throw new System.Web.Security.MembershipPasswordException("Security answer was not correct.");
                }

                // Once the answer has been given, if required, determine if we need to unencode the password before
                // we return it. The call to UnencodePassword will just return the given password as is if the password
                // format is set to MembershipPasswordFormat.Clear.
                password = UnencodePassword(user.Password);
            }

            // Return the retrieved password.
            return password;
        }

        public override SystemWeb.MembershipUser GetUser(string username, bool userIsOnline)
        {
            // Assume we were unable to find the user.
            System.Web.Security.MembershipUser user = null;
            Membership loadedUser = null;

            // Don't accept empty user names.
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username");
            }

            // Get the user record from the data store.
            try
            {
                IList<Membership> members = Membership.Search <Membership>(QryFetchMemberByName.Query(username, application.ID));

                if (members.Count == 1)
                {
                    loadedUser = members[0];
                    user = loadedUser.ToMembershipUser(Name);
                }
                else if (1 < members.Count)
                {
                    throw new ProviderException(STR_TooManyMatchingUsers);
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException(STR_UnableToGetMembershipUser, ex);
            }

            // Determine if we need to update the activity information.
            if (userIsOnline && (user != null) && loadedUser != null)
            {
                // Update the last activity timestamp (LastActivityDate).
                loadedUser.LastActivityDate = DateTime.Now;
                loadedUser.Save();
            }

            // Return the resulting user.
            return user;
        }

        public override SystemWeb.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            // Assume we were unable to find the user.
            SystemWeb.MembershipUser user = null;
            Membership member = null;

            // Ensure the provider key is valid.
            if (null == providerUserKey)
            {
                throw (new ArgumentNullException("providerUserKey"));
            }

            // Get the user record from the data store.
            try
            {
                member = Membership.Load<Membership>((Guid)providerUserKey);
                if (member != null)
                {
                    user = member.ToMembershipUser(Name);
                }
                else
                {
                    throw new ProviderException(STR_UnableToGetMembershipUser);
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException(STR_UnableToGetMembershipUser);
            }

            // Determine if we need to update the activity information.
            if (userIsOnline && (member != null))
            {
                // Update the last activity timestamp (LastActivityDate).
                member.LastActivityDate = DateTime.Now;
                member.Save();
            }

            // Return the resulting user.
            return user;
        }

        public override string GetUserNameByEmail(string email)
        {
            // Assume we were unable to find the corresponding user name.
            string username = null;

            // Don't accept empty emails.
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            // Get the user record from the data store.
            try
            {
                IList<Membership> list = Membership.Search<Membership>(QrySearchMemberByEmail.Query(email, application.ID));

                if (1 == list.Count)
                {
                    username = list[0].ToString();
                }
                else if (1 < list.Count)
                {
                    throw new ProviderException(STR_TooManyMatchingUsers);
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException(STR_UnableToGetMembershipUser, ex);
            }

            // Return the name of the user associated to the given e-mail address, if any.
            return username;
        }

        public override string ResetPassword(string username, string answer)
        {
            // Prepare a placeholder for the new passowrd.
            string newPassword;

            // Ensure password retrievals are allowed.
            if (!EnablePasswordReset)
            {
                throw new SystemWeb.MembershipPasswordException("Password reset is not enabled.");
            }

            Membership user = Membership.Fetch<Membership>(QryFetchMemberByName.Query(username, application.ID));

            // Determine if a valid answer has been given if question and answer is required.
            if ((null == answer) && RequiresQuestionAndAnswer)
            {
                user.FailedPasswordAnswerAttemptCount++;
                user.Save();

                throw new SystemWeb.MembershipPasswordException("Password answer required for reset.");
            }

            // Generate a new random password of the specified length.
            newPassword = SystemWeb.Membership.GeneratePassword(minRequiredPasswordLength, MinRequiredNonAlphanumericCharacters);

            // Raise the ValidatingPassword event in case an event handler has been defined.
            SystemWeb.ValidatePasswordEventArgs args = new SystemWeb.ValidatePasswordEventArgs(username, newPassword, true);
            OnValidatingPassword(args);
            if (args.Cancel)
            {
                // Check for a specific error message.
                if (null != args.FailureInformation)
                {
                    throw (args.FailureInformation);
                }
                else
                {
                    throw new SystemWeb.MembershipPasswordException("Password reset cancelled due to new password validation.");
                }
            }

            if (null != user)
            {
                // Determine if the user is locked out of the system.
                if (user.IsLockedOut)
                {
                    throw new SystemWeb.MembershipPasswordException(STR_PasswordResetCancelledBecauseAccountItLocked);
                }

                // Determine if the user is required to answer a password question.
                if (RequiresQuestionAndAnswer && !CheckPassword(answer, user.PasswordAnswer, user.PasswordSalt))
                {
                    user.FailedPasswordAnswerAttemptCount++;
                    user.Save();

                    throw new SystemWeb.MembershipPasswordException("Password reset cancelled because security answer was incorrect.");
                }

                // Update user record with the new password.
                try
                {
                    user.Password = EncodePassword(newPassword, user.PasswordSalt);
                    user.LastPasswordChangedDate = DateTime.Now;
                    user.LastActivityDate = DateTime.Now;
                    user.Save();
                }
                catch
                {
                    throw new SystemWeb.MembershipPasswordException(STR_PasswordResetCancelledBecauseAccountItLocked);
                }
            }

            // Return the resulting new password.
            return newPassword;
        }

        public override bool UnlockUser(string userName)
        {
            // Assume we are unable to perform the operation.
            bool result = false;

            try
            {
                // Get the user record form the data store.
                Membership member = Membership.Fetch<Membership>(QryFetchMemberByName.Query(userName, application.ID));

                if (member != null)
                {
                    member.IsLockedOut = false;
                    member.LastLockoutDate = DateTime.Now;
                    member.LastActivityDate = DateTime.Now;
                    member.Save();

                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("User was unable to be unlocked.", ex);
            }

            // Return the result of the operation.
            return result;
        }

        public override void UpdateUser(SystemWeb.MembershipUser user)
        {
            try
            {
                Membership member = Membership.Fetch<Membership>(QryFetchMemberByName.Query(user.UserName, application.ID));
                member.FromMembershipUser(user);
                member.Save();
            }
            catch (Exception ex)
            {
                throw new ProviderException("User was unable to be updated", ex);
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            // Assume the given user is not valid.
            bool isValid = false;

            // Get the password and the flag indicating the user is approved.
            Membership member = Membership.Fetch<Membership>(QryFetchMemberByName.Query(username, application.ID));

            if (null != member)
            {
                // Ensure the passwords match but only if the user is not already locked out of the system.
                if (!member.IsLockedOut && CheckPassword(password, member.Password, member.PasswordSalt))
                {
                    // Ensure the user is allowed to login.
                    if (member.IsApproved)
                    {
                        // Indicate the user is valid.
                        isValid = true;
                        // Update the user's last login date.
                        member.LastLoginDate = DateTime.Now;
                        member.Save();
                    }
                }
                else
                {
                    // Update the failure count.
                    member.FailedPasswordAttemptCount++;
                    member.Save();
                }
            }

            // Return the result of the operation.
            return isValid;
        }

        internal static string GetConfigValue(string configValue, string defaultValue)
        {
            return (string.IsNullOrEmpty(configValue) ? defaultValue : configValue);
        }

        public static string CreateKey(int numBytes)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[numBytes];
            rng.GetBytes(buff);
            return BytesToHexString(buff);
        }

        /// <summary>
        /// Compares password values based on the <see cref="MembershipPasswordFormat"/> property value.
        /// </summary>
        /// <param name="password1">first password to compare.</param>
        /// <param name="password2">second password to compare</param>
        /// <param name="validationKey">key to use when encoding the password.</param>
        /// <returns></returns>
        private bool CheckPassword(string password1, string password2, string validationKey)
        {
            // Format the password as required for comparison.
            switch (PasswordFormat)
            {
                case SystemWeb.MembershipPasswordFormat.Hashed:
                    password1 = EncodePassword(password1, validationKey);
                    break;
                case SystemWeb.MembershipPasswordFormat.Encrypted:
                    password2 = UnencodePassword(password2);
                    break;
            }

            // Return the result of the comparison.
            return (password1 == password2);
        }
        /// <summary>
        /// Encrypts, Hashes, or leaves the password clear based on the <see cref="PasswordFormat"/> property value.
        /// </summary>
        /// <param name="password">the password to encode.</param>
        /// <param name="validationKey">key to use when encoding the password.</param>
        /// <returns>
        /// The encoded password only if all parameters are specified. If <c>validationKey</c> is <c>null</c>
        /// then the given <c>password</c> is returned untouched.
        /// </returns>
        private string EncodePassword(string password, string validationKey)
        {
            // Assume no encoding is performed.
            string encodedPassword = password;

            // Only perform the encoding if all parameters are passed and valid.
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(validationKey))
            {
                // Determine the type of encoding required.
                switch (PasswordFormat)
                {
                    case SystemWeb.MembershipPasswordFormat.Clear:
                        // Nothing to do.
                        break;
                    case SystemWeb.MembershipPasswordFormat.Encrypted:
                        encodedPassword = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                        break;
                    case SystemWeb.MembershipPasswordFormat.Hashed:
                        // If we are not password a validation key, use the default specified.
                        if (string.IsNullOrEmpty(validationKey))
                        {
                            // The machine key will either come from the Web.config file or it will be automatically generate
                            // during initialization.
                            validationKey = machineKey.ValidationKey;
                        }
                        HMACSHA1 hash = new HMACSHA1();
                        hash.Key = HexToByte(validationKey);
                        encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                        break;
                    default:
                        throw new ProviderException("Password is in an unsupported format");
                }
            }

            // Return the encoded password.
            return encodedPassword;
        }
        /// <summary>
        /// Decrypts or leaves the password clear based on the <see cref="PasswordFormat"/> property value.
        /// </summary>
        /// <param name="password">password to unencode.</param>
        /// <returns>Unencoded password.</returns>
        private string UnencodePassword(string password)
        {
            // Assume no unencoding is performed.
            string unencodedPassword = password;

            // Determine the type of unencoding required.
            switch (PasswordFormat)
            {
                case SystemWeb.MembershipPasswordFormat.Clear:
                    // Nothing to do.
                    break;
                case SystemWeb.MembershipPasswordFormat.Encrypted:
                    unencodedPassword = Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(unencodedPassword)));
                    break;
                case SystemWeb.MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Password can not be hashed");
                default:
                    throw new ProviderException("Password is in an unsupported format");
            }

            // Return the unencoded password.
            return unencodedPassword;
        }
        /// <summary>
        /// Converts a hexadecimal string to a byte array. Used to convert encryption key values from the configuration.
        /// </summary>
        /// <param name="hexString">hexadecimal string to conver.</param>
        /// <returns><c>byte</c> array containing the converted hexadecimal string contents.</returns>
        private static byte[] HexToByte(string hexString)
        {
            byte[] bytes = new byte[hexString.Length / 2 + 1];
            for (int i = 0; i <= hexString.Length / 2 - 1; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
        public static string BytesToHexString(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(64);
            for (int counter = 0; counter < bytes.Length; counter++)
            {
                hexString.Append(string.Format("{0:X2}", bytes[counter]));
            }
            return hexString.ToString();
        }
    }
}
