using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using NHibernate.Cfg;
using System.BusinessObjects.Providers;

namespace System.BusinessObjects.Data
{
    public sealed class NHibernateThreadSlotProvider : NHibernateSessionProvider
    {

        public override ISession CurrentSession
        {
            get
            {
                LocalDataStoreSlot slot = Thread.GetNamedDataSlot(CurrentSessionKey);
                ISession currentSession = null;
                object threadData = null;

                if (slot != null)
                {
                    threadData = Thread.GetData(slot);
                    currentSession = threadData as ISession;
                }

                if (threadData == null)
                {
                    currentSession = sessionFactory.OpenSession();
                }

                if (slot != null)
                {
                    Thread.SetData(slot, currentSession);
                }
                else if (slot == null)
                {
                    slot = Thread.AllocateNamedDataSlot(CurrentSessionKey);

                    Thread.SetData(slot, currentSession);
                }

                return currentSession;
            }
        }

        public override void CloseSession()
        {
            LocalDataStoreSlot slot = Thread.GetNamedDataSlot(CurrentSessionKey);
            object threadData = Thread.GetData(slot);
            ISession currentSession = threadData as ISession;

            if (currentSession == null)
            {
                // No current session
                return;
            }

            currentSession.Close();
            Thread.FreeNamedDataSlot(CurrentSessionKey);
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
