using System;
using System.Linq;
using System.BusinessObjects.Infrastructure;
using System.Linq.Expressions;

namespace System.BusinessObjects.Membership.Qry
{
    public class MembersWithEmailSpecification : Specification<Membership>
    {
        private string _email;

        public MembersWithEmailSpecification(string email)
        {
            _email = email;
        }

        public override Expression<Func<Membership, bool>> Predicate
        {
            get { return x => x.Email.Contains(_email); }
        }
    }
}
