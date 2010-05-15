using System;
using System.BusinessObjects.Infrastructure;
using System.Linq.Expressions;

namespace System.BusinessObjects.Membership.Qry
{
    public class RecentlyActiveMembersSpecification : Specification<Membership>
    {
        private DateTime _lastActive;
        public RecentlyActiveMembersSpecification(DateTime lastActive)
        {
            _lastActive = lastActive;
        }

        public override Expression<Func<Membership, bool>> Predicate
        {
            get { return x => x.LastActivityDate > _lastActive; }
        }
    }
}
