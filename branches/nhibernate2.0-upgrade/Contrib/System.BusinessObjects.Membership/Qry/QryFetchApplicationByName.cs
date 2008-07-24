using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.BusinessObjects.Transactions;
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
