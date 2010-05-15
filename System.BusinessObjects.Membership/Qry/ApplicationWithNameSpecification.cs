using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.BusinessObjects.Infrastructure;
using System.Linq.Expressions;

namespace System.BusinessObjects.Membership.Qry
{
    public class ApplicationWithNameSpecification : Specification<Application>
    {
        private string _applicationName;
        public ApplicationWithNameSpecification(string applicationName)
        {
            _applicationName = applicationName;
        }

        public override Expression<Func<Application, bool>> Predicate
        {
            get { return x => x.ApplicationName == _applicationName; }
        }
    }
}
