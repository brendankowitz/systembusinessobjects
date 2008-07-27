using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.BusinessObjects.Transactions;
using NHibernate.Criterion;

namespace System.BusinessObjects.Membership.Qry
{
    public class QryFetchRoleByName
    {
        public static ICriteria Query(string rolename, Guid applicationId)
        {
            ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(Role))
                .Add(Restrictions.Eq("LoweredRoleName", rolename.ToLower()))
                .Add(Restrictions.Eq("Application.ID", applicationId));
            return qry;
        }

        public static ICriteria QueryCount(string rolename, Guid applicationId)
        {
            ICriteria qry = Query(rolename, applicationId)
                .SetProjection(Projections.RowCount());
            return qry;
        }
    }
}
