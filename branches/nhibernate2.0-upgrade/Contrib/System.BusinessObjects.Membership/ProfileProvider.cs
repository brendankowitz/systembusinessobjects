﻿using System;
using System.Web;
using System.Collections.Specialized;
using System.BusinessObjects.Membership.Qry;
using System.Web.Hosting;
using System.Collections.Generic;
using System.BusinessObjects.Transactions;
using System.Configuration.Provider;
using System.Reflection;
using System.Globalization;
using System.Text;
using System.IO;
using System.Configuration;
using System.Security.Permissions;

namespace System.BusinessObjects.Membership
{
    public class ProfileProvider : System.Web.Profile.ProfileProvider
    {
        private Application application = new Application();

        public Application Application
        {
            get
            {
                return application;
            }
            set
            {
                application = value;
            }
        }
        public override string ApplicationName
        {
            get { return Application.ApplicationName; }
            set { Application.ApplicationName = value; }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
             if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if ((name == null) || (name.Length < 1))
            {
                name = "NHibernate Profile Provider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "NHibernate Profile Provider");
            }
            base.Initialize(name, config);

            // Load configuration data.
            string appName = GetConfigValue(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);
            Application = Application.Fetch(QryFetchApplicationByName.Query(appName));

            if (Application == null)
            {
                Application = new Application()
                {
                    ID = Guid.NewGuid(),
                    ApplicationName = appName,
                    Description = config["description"],
                    LoweredApplicationName = appName.ToLower()
                };
                Application.Save();
            }
        }

        public override int DeleteInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            IList<Profile> list = new List<Profile>();
            try
            {
                list = Profile.Search<Profile>(QrySearchProfiles.Query(authenticationOption, Application.ID, userInactiveSinceDate));

                foreach(Profile p in list)
                {
                    p.Delete();
                    p.Save();
                }
            }
            catch(Exception ex)
            {
                throw new ProviderException("Unable to delete inactive profiles.", ex);
            }
            return list.Count;
        }

        public override int DeleteProfiles(string[] usernames)
        {
            if (usernames == null || usernames.Length == 0)
                throw new ArgumentException("No user profiles to delete.", "usernames");

            int num = 0;
            bool flag = false;
            try
            {
                foreach (string username in usernames)
                {
                    Profile profile = Profile.Fetch<Profile>(QrySearchProfiles.Query(username, Application.ID));
                    if (profile != null)
                    {
                        profile.Delete();
                        profile.Save();
                    }
                }
            }
            catch
            {
                throw new ProviderException("Unable to delete profiles.");
            }
            return num;
        }

        public override int DeleteProfiles(System.Web.Profile.ProfileInfoCollection profiles)
        {
            if (profiles == null)
            {
                throw new ArgumentNullException("profiles");
            }
            if (profiles.Count < 1)
            {
                throw new ArgumentException("Parameter collection empty", "profiles");
            }
            string[] usernames = new string[profiles.Count];
            int num = 0;
            foreach (System.Web.Profile.ProfileInfo info in profiles)
            {
                usernames[num++] = info.UserName;
            }
            return this.DeleteProfiles(usernames);
        }

        public override System.Web.Profile.ProfileInfoCollection FindInactiveProfilesByUserName(System.Web.Profile.ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            if (string.IsNullOrEmpty(usernameToMatch))
                throw new ArgumentException("Username to match by cannot be empty", "usernameToMatch");

            System.Web.Profile.ProfileInfoCollection retval = new System.Web.Profile.ProfileInfoCollection();
            try
            {
                string matchOn = usernameToMatch.Replace("?", "_").Replace("*", "%");
                IList<Profile> list = Profile.Search<Profile>(QrySearchProfiles.Query(authenticationOption, matchOn, Application.ID, userInactiveSinceDate, pageIndex, pageSize));
                totalRecords = (int)QrySearchProfiles.QueryCount(authenticationOption, matchOn, Application.ID, userInactiveSinceDate).UniqueResult<long>();

                foreach (Profile p in list)
                {
                    retval.Add(p.ToProfileInfo());
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to find inactive profiles", ex);
            }

            return retval;
        }

        public override System.Web.Profile.ProfileInfoCollection FindProfilesByUserName(System.Web.Profile.ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Profile.ProfileInfoCollection GetAllInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Profile.ProfileInfoCollection GetAllProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }

        public override System.Configuration.SettingsPropertyValueCollection GetPropertyValues(System.Configuration.SettingsContext context, System.Configuration.SettingsPropertyCollection collection)
        {
            throw new NotImplementedException();
        }

        public override void SetPropertyValues(System.Configuration.SettingsContext sc, System.Configuration.SettingsPropertyValueCollection properties)
        {
            string objValue = (string)sc["UserName"];
            bool userIsAuthenticated = (bool)sc["IsAuthenticated"];

            if (((objValue != null) && (objValue.Length >= 1)) && (properties.Count >= 1))
            {
                string allNames = string.Empty;
                string allValues = string.Empty;
                byte[] buf = null;

                PrepareDataForSaving(ref allNames, ref allValues, ref buf, true, properties, userIsAuthenticated);
                if (allNames.Length != 0)
                {
                    try
                    {
                        Profile profile = Profile.Fetch<Profile>(QrySearchProfiles.Query(objValue, Application.ID));
                        if (profile == null)
                            throw new ApplicationException(string.Format("Unable to find user '{0}'", objValue));
                        profile.PropertyNames = allNames;
                        profile.PropertyValuesString = allValues;
                        profile.PropertyValuesBinary = buf;
                        profile.IsAnonymous = !userIsAuthenticated;
                        profile.LastActivityDate = DateTime.UtcNow;
                        profile.LastUpdatedDate = DateTime.UtcNow;
                        profile.Save();
                    }
                    catch(Exception ex)
                    {
                        throw new ProviderException("Unable to set user profile values.", ex);
                    }
                }
            }
        }

        internal static string GetConfigValue(string configValue, string defaultValue)
        {
            return (string.IsNullOrEmpty(configValue) ? defaultValue : configValue);
        }

        [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.SerializationFormatter)]
        internal static void PrepareDataForSaving(ref string allNames, ref string allValues, ref byte[] buf, bool binarySupported, SettingsPropertyValueCollection properties, bool userIsAuthenticated)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            MemoryStream stream = binarySupported ? new MemoryStream() : null;
            try
            {
                try
                {
                    bool flag = false;
                    foreach (SettingsPropertyValue value2 in properties)
                    {
                        if (value2.IsDirty && (userIsAuthenticated || ((bool)value2.Property.Attributes["AllowAnonymous"])))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        return;
                    }
                    foreach (SettingsPropertyValue value3 in properties)
                    {
                        if ((!userIsAuthenticated && !((bool)value3.Property.Attributes["AllowAnonymous"])) || (!value3.IsDirty && value3.UsingDefaultValue))
                        {
                            continue;
                        }
                        int length = 0;
                        int position = 0;
                        string str = null;
                        if (value3.Deserialized && (value3.PropertyValue == null))
                        {
                            length = -1;
                        }
                        else
                        {
                            object serializedValue = value3.SerializedValue;
                            if (serializedValue == null)
                            {
                                length = -1;
                            }
                            else
                            {
                                if (!(serializedValue is string) && !binarySupported)
                                {
                                    serializedValue = Convert.ToBase64String((byte[])serializedValue);
                                }
                                if (serializedValue is string)
                                {
                                    str = (string)serializedValue;
                                    length = str.Length;
                                    position = builder2.Length;
                                }
                                else
                                {
                                    byte[] buffer = (byte[])serializedValue;
                                    position = (int)stream.Position;
                                    stream.Write(buffer, 0, buffer.Length);
                                    stream.Position = position + buffer.Length;
                                    length = buffer.Length;
                                }
                            }
                        }
                        builder.Append(value3.Name + ":" + ((str != null) ? "S" : "B") + ":" + position.ToString(CultureInfo.InvariantCulture) + ":" + length.ToString(CultureInfo.InvariantCulture) + ":");
                        if (str != null)
                        {
                            builder2.Append(str);
                        }
                    }
                    if (binarySupported)
                    {
                        buf = stream.ToArray();
                    }
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
            }
            catch
            {
                throw;
            }
            allNames = builder.ToString();
            allValues = builder2.ToString();
        }

    }
}
