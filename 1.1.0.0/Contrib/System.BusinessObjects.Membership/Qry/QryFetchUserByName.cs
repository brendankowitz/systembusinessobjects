using System;
using System.BusinessObjects.Transactions;
using NHibernate;
using NHibernate.Criterion;

namespace System.BusinessObjects.Membership.Qry
{
    public class QryFetchUserByName
    {
        public static ICriteria Query(string username, Guid applicationId)
        {
            ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(User))
                .Add(Restrictions.Eq("LoweredUserName", username.ToLower()))
                .Add(Restrictions.Eq("Application.ID", applicationId));
            return qry;
        }

        public static ICriteria QueryCount(string username, Guid applicationId)
        {
            ICriteria qry = Query(username, applicationId)
                .SetProjection(Projections.RowCount());
            return qry;
        }
    }
}
