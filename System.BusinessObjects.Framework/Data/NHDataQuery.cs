using System;
using NHibernate;
using System.Collections.Generic;

namespace System.BusinessObjects.Data
{
    public abstract class NHDataQuery<T> : IDataQuery<T>, IRequiresNHibernateSession
    {
        ISession IRequiresNHibernateSession.Session { get; set; }
        protected ISession Session { get { return ((IRequiresNHibernateSession)this).Session; } }

        public abstract IEnumerable<T> Execute();
    }
}
