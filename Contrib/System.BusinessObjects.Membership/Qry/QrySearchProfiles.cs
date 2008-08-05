using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using System.BusinessObjects.Transactions;
using NHibernate.Criterion;

namespace System.BusinessObjects.Membership.Qry
{
    public class QrySearchProfiles
    {
        public static ICriteria Query(System.Web.Profile.ProfileAuthenticationOption profileOptions, Guid applicationId)
        {
            ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(Profile))
                .CreateAlias("User", "u")
                .Add(Restrictions.Eq("u.Application.ID", applicationId));

            if (profileOptions == System.Web.Profile.ProfileAuthenticationOption.Anonymous)
                qry.Add(Restrictions.Eq("u.IsAnonymous", true));
            else if (profileOptions == System.Web.Profile.ProfileAuthenticationOption.Authenticated)
                qry.Add(Restrictions.Eq("u.IsAnonymous", false));

            return qry;
        }

        public static ICriteria Query(System.Web.Profile.ProfileAuthenticationOption profileOptions, Guid applicationId, DateTime inactiveSince)
        {
            ICriteria qry = Query(profileOptions, applicationId)
                .Add(Restrictions.Le("u.LastActivityDate", inactiveSince));

            return qry;
        }

        public static ICriteria Query(string username, Guid applicationId)
        {
            ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(Profile))
                .CreateAlias("User", "u")
                .Add(Restrictions.Eq("u.Application.ID", applicationId))
                .Add(Restrictions.Eq("u.LoweredUserName", username));

            return qry;
        }

        public static ICriteria Query(System.Web.Profile.ProfileAuthenticationOption profileOptions, string usernameToMatch, Guid applicationId, DateTime inactiveSince, int pageIndex, int pageSize)
        {
            ICriteria qry = Query(profileOptions, applicationId, inactiveSince)
                .CreateAlias("User", "u")
                .Add(Restrictions.Like("u.LoweredUserName", usernameToMatch.ToLower()))
                .SetMaxResults(pageSize)
                .SetFirstResult(pageIndex * pageSize);

            return qry;
        }

        public static ICriteria QueryCount(System.Web.Profile.ProfileAuthenticationOption profileOptions, string usernameToMatch, Guid applicationId, DateTime inactiveSince)
        {
            ICriteria qry = Query(profileOptions, applicationId, inactiveSince)
                .CreateAlias("User", "u")
                .Add(Restrictions.Like("u.LoweredUserName", usernameToMatch.ToLower()))
                .SetProjection(Projections.RowCount());
            return qry;
        }

        public static ICriteria QueryCount(System.Web.Profile.ProfileAuthenticationOption profileOptions, Guid applicationId, DateTime inactiveSince)
        {
            ICriteria qry = Query(profileOptions, applicationId, inactiveSince)
                .SetProjection(Projections.RowCount());
            return qry;
        }
    }
}
