using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.BusinessObjects.Transactions;
using NHibernate.Criterion;

namespace System.BusinessObjects.Membership.Qry
{
    public class QrySearchRecentlyActiveUsers
    {
        public static ICriteria Query(DateTime lastActiveDateTheshold, Guid applicationId)
        {
            ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(Membership))
                .Add(Restrictions.Ge("LastActivityDate", lastActiveDateTheshold))
                .Add(Restrictions.Eq("Application.ID", applicationId));
            return qry;
        }

        public static ICriteria QueryCount(DateTime lastActiveDateTheshold, Guid applicationId)
        {
            ICriteria qry = Query(lastActiveDateTheshold, applicationId)
                .SetProjection(Projections.RowCount());
            return qry;
        }
    }
}
