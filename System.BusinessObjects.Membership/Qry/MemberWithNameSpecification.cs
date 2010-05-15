using System;
using System.Linq;
using System.BusinessObjects.Infrastructure;
using System.Linq.Expressions;

namespace System.BusinessObjects.Membership.Qry
{
    public class MemberWithNameSpecification : Specification<Membership>
    {
        private string _username;

        public MemberWithNameSpecification(string username)
        {
            _username = username;
        }

        public override Expression<Func<Membership, bool>> Predicate
        {
            get { return x => x.UserName.Contains(_username); }
        }
    }
}
