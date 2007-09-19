
using System.BusinessObjects.Providers;
using NHibernate;
using System.Web;

namespace System.BusinessObjects.Data
{
    public class NHibernateAspContextProvider : NHibernateSessionProvider
    {
        public override NHibernate.ISession CurrentSession
        {
            get 
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

        public override void CloseSession()
        {
            CurrentSession.Close();
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
