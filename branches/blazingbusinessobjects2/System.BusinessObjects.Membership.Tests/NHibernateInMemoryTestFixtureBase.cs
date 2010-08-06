using System.BusinessObjects.Providers;
using System.Collections;
using System.Data;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.Collections.Generic;
using System.BusinessObjects.Data;
using BusinessObject.Framework.Tests;

namespace System.BusinessObjects.Membership.Tests
{
        /// <summary>
        /// Provides a fixture base for Unit tests to be run on an In-Memory SQLite db
        /// </summary>
    public class NHibernateInMemoryTestFixtureBase : IDisposable, IMembershipRepositoryFactory
        {
            protected static ISessionFactory sessionFactory;
            protected static NHibernate.Cfg.Configuration configuration;
            protected Application app = new Application();

            protected ISession session;


            public NHibernateInMemoryTestFixtureBase()
            {
                OneTimeInitalize(typeof(System.BusinessObjects.Membership.Membership).Assembly, this.GetType().Assembly);
                session = CreateSession();
                session.BeginTransaction();
                MembershipProviderRepository.Set(this);

                app.ApplicationName = "Blazing.Membership";
                app.Description = "NHibernateMembership";
                session.Save(app);


                ((MembershipProvider)System.Web.Security.Membership.Provider).Application = app;
                ((RoleProvider)System.Web.Security.Roles.Provider).Application = app;
                //((ProfileProvider)System.Web.Profile.ProfileManager.Provider).Application = app;
            }

            public void Dispose()
            {
                if (session != null)
                    session.Flush();

                if (session.Transaction != null)
                    session.Transaction.Rollback();

                session.Dispose();
                session = null;
            }

            /// <summary> 
            /// Initialize NHibernate and builds a session factory 
            /// Note, this is a costly call so it will be executed only one. 
            /// </summary> 
            public static void OneTimeInitalize(params Assembly[] assemblies)
            {
                if (sessionFactory != null)
                    return;
                IDictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
                properties.Add("dialect", "NHibernate.Dialect.SQLiteDialect");
                properties.Add("show_sql", "true");
                properties.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
                properties.Add("connection.connection_string", "Data Source=:memory:;Version=3;New=True;");
                properties.Add("proxyfactory.factory_class", "NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu");
                configuration = new NHibernate.Cfg.Configuration();
                configuration.Properties = properties;

                foreach (Assembly assembly in assemblies)
                {
                    configuration = configuration.AddAssembly(assembly);
                }
                sessionFactory = configuration.BuildSessionFactory();
            }

            public ISession CreateSession()
            {
                ISession openSession = sessionFactory.OpenSession();
                IDbConnection connection = openSession.Connection;
                new SchemaExport(configuration).Execute(false, true, false, connection, null);
                return openSession;
            }

            #region IMembershipRepositoryFactory Members

            public IDataObjectRepository<T> GetMembershipRepository<T>() where T : DataObject
            {
                return new NHLinqRepository<T>(session);
            }

            #endregion
        }
    }
