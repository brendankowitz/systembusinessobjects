using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using System.BusinessObjects.Transactions;
using NHibernate.Criterion;

namespace System.BusinessObjects.Membership.Qry
{
    public class QrySearchMemberByEmail
    {
        public static ICriteria Query(string email, Guid applicationId)
        {
            ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(Membership))
                .Add(Restrictions.Like("LoweredEmail", email.ToLower()))
                .Add(Restrictions.Eq("Application.ID", applicationId));
            return qry;
        }

        public static ICriteria QueryCount(string email, Guid applicationId)
        {
            ICriteria qry = Query(email, applicationId)
                .SetProjection(Projections.RowCount());
            return qry;
        }

        public static ICriteria Query(string email, Guid applicationId, int pageSize, int pageIndex)
        {
            ICriteria qry = Query(email, applicationId)
                .SetMaxResults(pageSize)
                .SetFirstResult(pageIndex * pageSize);
            return qry;
        }
    }
}
