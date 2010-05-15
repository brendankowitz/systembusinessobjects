using System;
using System.BusinessObjects.Infrastructure;
using System.Linq.Expressions;

namespace System.BusinessObjects.Membership.Qry
{
    public class RoleWithNameSpecification : Specification<Role>
    {
        private string _roleName;
        public RoleWithNameSpecification(string roleName)
        {
            _roleName = roleName;
        }

        public override Expression<Func<Role, bool>> Predicate
        {
            get { return x => x.RoleName == _roleName; }
        }
    }
}
