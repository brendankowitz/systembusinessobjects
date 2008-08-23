using System;
using System.Collections.Generic;
using System.Text;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using NHibernate;

namespace System.BusinessObjects.Providers
{
    public static class ServiceLocator
    {
#if USE_WINDSOR
        static IWindsorContainer container;

        public static IWindsorContainer Container
        {
            get
            {
                if (container == null)
                    container = new WindsorContainer(new XmlInterpreter());
                return container;
            }
            set
            {
                container = value;
            }
        }

        public static CacheProvider CacheProvider
        {
            get
            {
                return Container.Resolve(typeof(CacheProvider)) as CacheProvider;
            }
        }

        public static NHibernateSessionProvider SessionProvider
        {
            get
            {
                return Container.Resolve(typeof(NHibernateSessionProvider)) as NHibernateSessionProvider;
            }
        }
#else
        /// <summary>
        /// Returns the current session provider
        /// </summary>
        public static NHibernateSessionProvider SessionProvider
        {
            get
            {
                return NHibernateSessionProvider.Provider;
            }
        }
#endif
    }
}
