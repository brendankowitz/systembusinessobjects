using System;
using System.BusinessObjects.Infrastructure;
using System.Linq.Expressions;

namespace System.BusinessObjects.Membership.Qry
{
    public class RoleInApplicationSpecification : Specification<Role>
    {
        private Guid _applicationId;
        public RoleInApplicationSpecification(Guid applicationId)
        {
            _applicationId = applicationId;
        }

        public override Expression<Func<Role, bool>> Predicate
        {
            get { return x => x.Application.ID == _applicationId; }
        }
    }
}
