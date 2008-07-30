using System;
using System.BusinessObjects.Transactions;
using NHibernate;

namespace System.BusinessObjects.Membership.Qry
{
    public class QrySearchUsersInRole
    {
        public static IQuery Query(Guid roleId, Guid applicationId)
        {
            //this join is easier to express with hql:
            string sql = @"select u from Role r join r.Users u
                           where r.ID = :roleId and r.Application.ID = :applicationId";

            IQuery qry = UnitOfWork.CurrentSession.CreateQuery(sql)
                .SetParameter("roleId", roleId)
                .SetParameter("applicationId", applicationId);

            return qry;
        }

        public static IQuery Query(string roleName, Guid applicationId, string userNameLike)
        {
            //this join is easier to express with hql:
            string sql = @"select u from Role r join r.Users u
                           where r.LoweredRoleName = :roleName and 
                           r.Application.ID = :applicationId and
                           u.LoweredUserName like :userName";

            IQuery qry = UnitOfWork.CurrentSession.CreateQuery(sql)
                .SetParameter("roleName", roleName.ToLower())
                .SetParameter("applicationId", applicationId)
                .SetParameter("userName", userNameLike);

            return qry;
        }

        public static IQuery QueryCount(string roleName, Guid applicationId, string userName)
        {
            //this join is easier to express with hql:
            string sql = @"select count(u) from Role r join r.Users u
                           where r.LoweredRoleName = :roleName and 
                           r.Application.ID = :applicationId and
                           u.LoweredUserName = :userName";

            IQuery qry = UnitOfWork.CurrentSession.CreateQuery(sql)
                .SetParameter("roleName", roleName.ToLower())
                .SetParameter("applicationId", applicationId)
                .SetParameter("userName", userName);

            return qry;
        }
    }
}
