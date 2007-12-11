using System.BusinessObjects.Providers;
using System.Collections;
using System.Data;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

//#if !NUNIT
//    using Microsoft.VisualStudio.TestTools.UnitTesting;
//#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using ClassInitialize = NUnit.Framework.TestFixtureSetUpAttribute;
using ClassCleanup = NUnit.Framework.TestFixtureTearDownAttribute;
using Sample.BusinessObjects.Contacts;
//#endif

namespace BusinessObject.Framework.Tests
{
        /// <summary>
        /// Provides a fixture base for Unit tests to be run on an In-Memory SQLite db
        /// </summary>
        [TestClass]
        public class NHibernateInMemoryTestFixtureBase
        {
            protected static ISessionFactory sessionFactory;
            protected static Configuration configuration;

            protected ISession session;

//#if !NUNIT
//            private TestContext testContextInstance;
//            /// <summary>
//            ///Gets or sets the test context which provides
//            ///information about and functionality for the current test run.
//            ///</summary>
//            public TestContext TestContext
//            {
//                get
//                {
//                    return testContextInstance;
//                }
//                set
//                {
//                    testContextInstance = value;
//                }
//            }
//#endif

            [TestInitialize]
            public void Setup()
            {
                OneTimeInitalize(typeof(Person).Assembly);
                NHibernateSessionProvider.CurrentFactory = sessionFactory;
                session = CreateSession();
                NHibernateSessionProvider.Provider.CurrentSession = session;
            }

            [TestCleanup]
            public void FixtureTearDown()
            {
                session.Dispose();
                session = null;
                //sessionFactory.Dispose();
            }



            /// <summary> 
            /// Initialize NHibernate and builds a session factory 
            /// Note, this is a costly call so it will be executed only one. 
            /// </summary> 
            public static void OneTimeInitalize(params Assembly[] assemblies)
            {
                if (sessionFactory != null)
                    return;
                Hashtable properties = new Hashtable();
                properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SQLite20Driver");
                properties.Add("hibernate.dialect", "NHibernate.Dialect.SQLiteDialect");
                properties.Add("show_sql", "true");
                properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
                properties.Add("hibernate.connection.connection_string", "Data Source=:memory:;Version=3;New=True;");
                configuration = new Configuration();
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
