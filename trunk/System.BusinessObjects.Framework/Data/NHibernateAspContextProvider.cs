
using System.BusinessObjects.Providers;
using NHibernate;
using System.Web;

namespace System.BusinessObjects.Data
{
    public class NHibernateAspContextProvider : NHibernateSessionProvider
    {
        static object syncObj = new object();

        public override NHibernate.ISession CurrentSession
        {
            get 
            {
                lock (syncObj)
                {
                    ISession currentSession = HttpContext.Current.Items[CurrentSessionKey] as ISession;

                    if (currentSession == null)
                    {
                        currentSession = sessionFactory.OpenSession();
                        HttpContext.Current.Items[CurrentSessionKey] = currentSession;
                    }

                    return currentSession;
                }
            }
        }

        public override void CloseSession()
        {
            lock (syncObj)
            {
                if (HttpContext.Current.Items.Contains(CurrentSessionKey))
                {
                    ISession currentSession = HttpContext.Current.Items[CurrentSessionKey] as ISession;
                    if (currentSession != null)
                    {
                        currentSession.Flush();
                        currentSession.Close();
                        currentSession.Dispose();
                    }
                    HttpContext.Current.Items.Remove(CurrentSessionKey);
                }
            }
        }

        public override void CloseSessionFactory()
        {
            lock (syncObj)
            {
                if (sessionFactory != null)
                {
                    sessionFactory.Close();
                }
            }
        }
    }
}
