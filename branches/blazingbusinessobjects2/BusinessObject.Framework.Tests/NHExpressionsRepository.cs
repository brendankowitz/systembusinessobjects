using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.BusinessObjects.Data;
using System.BusinessObjects.Infrastructure;
using NHibernate;
using System.BusinessObjects.Expressions;

namespace BusinessObject.Framework.Tests
{
    public class NHExpressionsRepository<T> : GenericDataObjectRepository<T>
        where T : DataObject
    {
        public NHExpressionsRepository(ISession session) : base(session)
        {
        }

        ICriteria Convert(ISpecification[] specifications)
        {
            var qry = _session.CreateCriteria<T>().Expression<T>();
            foreach (ISpecification spec in specifications)
            {
                if (spec is Specification<T>)
                    qry = qry.Add(((Specification<T>)spec).Predicate);
                else if (spec is Join<T>)
                    qry = qry.Alias(((Join<T>)spec).Predicate);
 
            }
            return qry.Criteria;
        }

        public override T Fetch(params ISpecification[] specifications)
        {
            return Convert(specifications).UniqueResult<T>();
        }

        public override IEnumerable<T> Search(params ISpecification[] specifications)
        {
            return Convert(specifications).List<T>();
        }
    }
}
