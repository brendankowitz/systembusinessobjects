using System;
using System.BusinessObjects.Infrastructure;
using System.Linq.Expressions;

namespace System.BusinessObjects.Membership.Qry
{
    public class MembersInApplicationSpecification : Specification<Membership>
    {
        private Guid _applicationId;
        public MembersInApplicationSpecification(Guid applicationId)
        {
            _applicationId = applicationId;
        }

        public override Expression<Func<Membership, bool>> Predicate
        {
            get { return x => x.Application.ID == _applicationId; }
        }
    }

    public class UsersInApplicationSpecification : Specification<User>
    {
        private Guid _applicationId;
        public UsersInApplicationSpecification(Guid applicationId)
        {
            _applicationId = applicationId;
        }

        public override Expression<Func<User, bool>> Predicate
        {
            get { return x => x.Application.ID == _applicationId; }
        }
    }
}
