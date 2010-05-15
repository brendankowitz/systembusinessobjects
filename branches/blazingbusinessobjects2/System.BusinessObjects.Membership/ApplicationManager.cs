﻿using System;
using System.Collections.Generic;
using System.BusinessObjects.Membership.Qry;
using System.Web.Hosting;
using System.BusinessObjects.Data;

namespace System.BusinessObjects.Membership
{
    public class ApplicationManager
    {
        private static Dictionary<string, Application> _apps = new Dictionary<string, Application>();
        static readonly object appStoreLock = new object();
        public static Application GetApplication(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            Application retval = null;
            _apps.TryGetValue(name, out retval);
            return retval;
        }
        public static void SetApplication(string name, Application app)
        {
            lock (appStoreLock)
            {
                if (_apps.ContainsKey(name))
                {
                    _apps[name] = app;
                }
                else
                {
                    _apps.Add(name, app);
                }
            }
        }

        static readonly object fetchApplication = new object();
        public static Application FetchApplication(string applicationName, string description)
        {
            // Load configuration data.
            string appName = GetConfigValue(applicationName, HostingEnvironment.ApplicationVirtualPath);

            ApplicationWithNameSpecification withName = new ApplicationWithNameSpecification(appName);
            IDataObjectRepository<Application> repo = RepositoryFactory.GetApplicationRepository();
            Application app = repo.Fetch(withName);

            if (app == null)
            {
                lock (fetchApplication)
                {
                    app = repo.Fetch(withName);
                    if (app == null)
                    {
                        app = new Application()
                        {
                            ID = Guid.NewGuid(),
                            ApplicationName = appName,
                            Description = description,
                            LoweredApplicationName = appName.ToLower()
                        };
                        repo.Save(app);
                        repo.SubmitChanges();
                    }
                }
            }
            return app;
        }

        internal static string GetConfigValue(string configValue, string defaultValue)
        {
            return (string.IsNullOrEmpty(configValue) ? defaultValue : configValue);
        }
    }
}
