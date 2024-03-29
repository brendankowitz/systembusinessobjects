using System.Configuration.Provider;
using System.Security.Permissions;
using NHibernate;
using System.Diagnostics;

namespace System.BusinessObjects.Providers
{
    public abstract class NHibernateSessionProvider : ProviderBase
    {
        static NHibernateSessionProvider provider = null;
        static GenericProviderCollection<NHibernateSessionProvider> providers = null;

        protected const string CurrentSessionKey = "nhibernate.current_session";
        protected static ISessionFactory sessionFactory;
        protected static NHibernate.Cfg.Configuration cfg;

        static object syncObj = new object();

        public NHibernateSessionProvider()
        {
            
        }

        public static ISessionFactory CurrentFactory
        {
            get
            {
                lock (syncObj)
                {
                    if (sessionFactory == null)
                    {
                        if (cfg == null)
                        {
                            cfg = new NHibernate.Cfg.Configuration();                           
                            cfg.Configure();
                        }
                        sessionFactory = cfg.BuildSessionFactory();
                    }
                }
                return sessionFactory;
            }
            set
            {
                lock (syncObj)
                {
                    sessionFactory = value;
                }
            }
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

        }

        /// <summary>
        /// Returns an instance of the default NHibernateSession provider
        /// </summary>
        public static NHibernateSessionProvider Provider
        {
            get
            {
                if (provider == null)
#if USE_WINDSOR
                    provider = ServiceLocator.NHibernateSessionProvider;
#else
                    provider = ProviderHelper.LoadDefaultProvider<NHibernateSessionProvider>("NHibernateSessionProvider");
#endif
                return provider;
            }
        }

        /// <summary>
        /// Returns a collection of NHibernateSession providers
        /// </summary>
        public static GenericProviderCollection<NHibernateSessionProvider> Providers
        {
            get
            {
                if(providers == null)
                    providers = ProviderHelper.LoadProviderCollection<NHibernateSessionProvider>("NHibernateSessionProvider");
                return providers;
            }
        }

        public abstract ISession CurrentSession { get; set; }

        public abstract void CloseSession();

        public abstract void CloseSessionFactory();
    }
}
