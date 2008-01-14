using System;
using System.Collections.Generic;
using System.Text;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace System.BusinessObjects.Providers
{
    public partial class ServiceLocator
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

        public static NHibernateSessionProvider NHibernateSessionProvider
        {
            get
            {
                return Container.Resolve(typeof(NHibernateSessionProvider)) as NHibernateSessionProvider;
            }
        }
#else

#endif
    }
}
