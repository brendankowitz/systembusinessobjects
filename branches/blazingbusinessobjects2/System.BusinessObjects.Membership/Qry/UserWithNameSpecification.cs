using System;
using System.BusinessObjects.Infrastructure;
using System.Linq.Expressions;

namespace System.BusinessObjects.Membership.Qry
{
    public class UserWithNameSpecification : Specification<User>
    {
        private string _username;

        public UserWithNameSpecification(string username)
        {
            _username = username;
        }

        public override Expression<Func<User, bool>> Predicate
        {
            get { return x => x.UserName == _username; }
        }
    }

    public class UserWithNameLikeSpecification : Specification<User>
    {
        private string _username;

        public UserWithNameLikeSpecification(string username)
        {
            _username = username;
        }

        public override Expression<Func<User, bool>> Predicate
        {
            get { return x => x.UserName.Contains(_username); }
        }
    }
}
