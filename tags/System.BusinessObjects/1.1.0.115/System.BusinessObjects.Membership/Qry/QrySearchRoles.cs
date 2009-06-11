using System;
using NHibernate;
using System.BusinessObjects.Transactions;
using NHibernate.Criterion;

namespace System.BusinessObjects.Membership.Qry
{
    public class QrySearchRoles
    {
        public static ICriteria Query(Guid applicationId)
        {
            ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(Role))
                .Add(Restrictions.Eq("Application.ID", applicationId));
            return qry;
        }

        public static IQuery Query(Guid userId, Guid applicationId)
        {
            //this join is easier to express with hql:
            string sql = @"select r from Role r join r.Users u
                           where u.ID = :userId and r.Application.ID = :applicationId";

            IQuery qry = UnitOfWork.CurrentSession.CreateQuery(sql)
                .SetParameter("userId", userId)
                .SetParameter("applicationId", applicationId);

            return qry;
        }
    }
}
