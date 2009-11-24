using System;
using System.BusinessObjects.Transactions;
using NHibernate;
using NHibernate.Criterion;

namespace System.BusinessObjects.Membership.Qry
{
    public class QryFetchApplicationByName
    {
        public static ICriteria Query(string applicationName)
        {
            ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(Application))
                .Add(Restrictions.Eq("LoweredApplicationName", applicationName.ToLower()));
            return qry;
        }
    }
}
