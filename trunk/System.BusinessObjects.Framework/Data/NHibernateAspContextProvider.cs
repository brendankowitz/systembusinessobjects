
using System.BusinessObjects.Providers;
using NHibernate;
using System.Web;
using NHibernate.Context;

namespace System.BusinessObjects.Data
{
    public class NHibernateAspContextProvider : NHibernateSessionProvider
    {
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
