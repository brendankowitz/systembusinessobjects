using System.Configuration.Provider;
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
        private static NHibernate.Cfg.Configuration cfg;

        static object syncObj = new object();
        static object syncObjConfig = new object();

        public NHibernateSessionProvider()
        {
            
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            if (cfg == null)
            {
                lock (syncObjConfig)
                {
                    cfg = new NHibernate.Cfg.Configuration();
                    cfg.Configure();
                }
            }
            if (sessionFactory == null)
            {
                lock (syncObjConfig)
                {
                    sessionFactory = cfg.BuildSessionFactory();
                }
            }
        }

        /// <summary>
        /// Returns an instance of the default NHibernateSession provider
        /// </summary>
        public static NHibernateSessionProvider Provider
        {
            get
            {
                lock (syncObj)
                {
                    if (provider == null)
                        provider =
                            ProviderHelper.LoadDefaultProvider<NHibernateSessionProvider>("NHibernateSessionProvider");
                    return provider;
                }
            }
        }

        /// <summary>
        /// Returns a collection of NHibernateSession providers
        /// </summary>
        public static GenericProviderCollection<NHibernateSessionProvider> Providers
        {
            get
            {
                lock (syncObj)
                {
                    if (providers == null)
                        providers =
                            ProviderHelper.LoadProviderCollection<NHibernateSessionProvider>("NHibernateSessionProvider");
                    return providers;
                }
            }
        }

        public abstract ISession CurrentSession { get;}

        public abstract void CloseSession();

        public abstract void CloseSessionFactory();
    }
}
