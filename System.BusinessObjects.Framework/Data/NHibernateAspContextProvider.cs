
using System.BusinessObjects.Providers;
using NHibernate;
using System.Web;
using NHibernate.Context;
using System.BusinessObjects.Helpers;

namespace System.BusinessObjects.Data
{
    public class NHibernateAspContextProvider : NHibernateSessionProvider
    {
        static readonly object syncObj = new object();

        public static new ISessionFactory CurrentFactory
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
                            
                            //The Reflection optimiser as used in "ClassCompositeIdBinder.cs"
                            //is not compatible with ASP.NET Medium trust, so this provider
                            //will detect this and turn it off.
                            //switch(WebHelper.GetCurrentTrustLevel())
                            //{
                            //    case AspNetHostingPermissionLevel.High:
                            //    case AspNetHostingPermissionLevel.Unrestricted:
                            //        NHibernate.Cfg.Environment.UseReflectionOptimizer = true;
                            //        break;
                            //    default:
                            //        NHibernate.Cfg.Environment.UseReflectionOptimizer = false;
                            //    break;
                            //}

                            cfg.Configure();
                        }
                        sessionFactory = cfg.BuildSessionFactory();
                    }
                }
                return sessionFactory;
            }
        }

        public override NHibernate.ISession CurrentSession
        {
            get 
            {
                ISession session = null;
                try
                {
                    session = CurrentFactory.GetCurrentSession();
                }
                catch
                {
                    session = BindNewSession();
                }
                return session;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    ManagedWebSessionContext.Bind(HttpContext.Current, value);
                }
            }
        }

        public ISession BindNewSession()
        {
            if (HttpContext.Current != null)
            {
                ISession newSession = CurrentFactory.OpenSession();
                ManagedWebSessionContext.Bind(HttpContext.Current, newSession);
                return newSession;
            }
            return null;
        }

        public void UnbindSession()
        {
            ManagedWebSessionContext.Unbind(HttpContext.Current, CurrentFactory);
        }

        public override void CloseSession()
        {
            CurrentSession.Close();
            CurrentSession.Dispose();
            UnbindSession();
        }

        public override void CloseSessionFactory()
        {
            if (sessionFactory != null)
            {
                sessionFactory.Close();
            }
        }
    }
}
