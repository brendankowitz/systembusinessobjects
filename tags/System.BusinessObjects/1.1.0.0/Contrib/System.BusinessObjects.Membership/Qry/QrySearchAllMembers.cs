using System;
using System.BusinessObjects.Transactions;
using NHibernate;
using NHibernate.Criterion;

namespace System.BusinessObjects.Membership.Qry
{
    public class QrySearchAllMembers
    {
        public static ICriteria Query(Guid applicationId, int pageIndex, int pageSize)
        {
            ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(Membership))
                .Add(Restrictions.Eq("Application.ID", applicationId))
                .SetMaxResults(pageSize)
                .SetFirstResult(pageIndex * pageSize);
            return qry;
        }

        public static ICriteria QueryCount(Guid applicationId)
        {
            ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(Membership))
                .Add(Restrictions.Eq("Application.ID", applicationId))
                .SetProjection(Projections.RowCount());
            return qry;
        }
    }
}
