using System.BusinessObjects.Providers;
using System.Collections;
using System.Data;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.Collections.Generic;

using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using ClassInitialize = NUnit.Framework.TestFixtureSetUpAttribute;
using ClassCleanup = NUnit.Framework.TestFixtureTearDownAttribute;


namespace System.BusinessObjects.Membership.Tests
{
        /// <summary>
        /// Provides a fixture base for Unit tests to be run on an In-Memory SQLite db
        /// </summary>
        [TestClass]
        public class NHibernateInMemoryTestFixtureBase
        {
            protected static ISessionFactory sessionFactory;
            protected static NHibernate.Cfg.Configuration configuration;

            protected ISession session;

            [TestInitialize]
            public void Setup()
            {
                OneTimeInitalize(typeof(System.BusinessObjects.Membership.Membership).Assembly, this.GetType().Assembly);
                NHibernateSessionProvider.CurrentFactory = sessionFactory;
                session = CreateSession();
                session.BeginTransaction();
                NHibernateSessionProvider.Provider.CurrentSession = session;
            }

            [TestCleanup]
            public void FixtureTearDown()
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
                new SchemaExport(configuration).Execute(false, true, false, true, connection, null);
                return openSession;

            }
        }
    }
