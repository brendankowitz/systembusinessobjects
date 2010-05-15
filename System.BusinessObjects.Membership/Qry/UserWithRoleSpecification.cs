using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.BusinessObjects.Infrastructure;
using System.Linq.Expressions;

namespace System.BusinessObjects.Membership.Qry
{
    public class UserWithRoleSpecification : Specification<User>
    {
        private Role _role;
        public UserWithRoleSpecification(Role role)
        {
            _role = role;
        }

        public override Expression<Func<User, bool>> Predicate
        {
            get { return x => x.Roles.Contains(_role); }
        }
    }

    public class MemberWithRoleSpecification : Specification<Membership>
    {
        private Role _role;
        public MemberWithRoleSpecification(Role role)
        {
            _role = role;
        }

        public override Expression<Func<Membership, bool>> Predicate
        {
            get { return x => x.Roles.Contains(_role); }
        }
    }
}
