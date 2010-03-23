using System;
using System.Data;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.BusinessObjects.Providers;
using Sample.BusinessObjects.Contacts;
using System.Collections.Generic;
using Xunit;

namespace BusinessObject.Framework.Tests
{
        /// <summary>
        /// Provides a fixture base for Unit tests to be run on an In-Memory SQLite db
        /// </summary>
        public class NHibernateInMemoryTestFixtureBase : IDisposable
        {
            protected static ISessionFactory sessionFactory;
            protected static Configuration configuration;

            protected ISession session;

            public NHibernateInMemoryTestFixtureBase()
            {
                OneTimeInitalize(typeof(Person).Assembly, GetType().Assembly);
                //NHibernateSessionProvider.CurrentFactory = sessionFactory;
                session = CreateSession();
                //NHibernateSessionProvider.Provider.CurrentSession = session;
                session.BeginTransaction();
            }

            public void Dispose()
            {
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
                //properties.Add("proxyfactory.factory_class", "NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu");
                properties.Add("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
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
                new SchemaExport(configuration).Execute(false, true, false, connection, null);
                return openSession;
            }
        }
    }
