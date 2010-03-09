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

        ICriteria Convert(Specification<T>[] specifications)
        {
            var qry = _session.CreateCriteria<T>().Expression<T>();
            foreach (var spec in specifications)
            {
                qry = qry.Add(((Specification<T>)spec).Predicate);
            }
            return qry.Criteria;
        }

        public override T Fetch(params Specification<T>[] specifications)
        {
            return Convert(specifications).UniqueResult<T>();
        }

        public override IEnumerable<T> Search(params Specification<T>[] specifications)
        {
            return Convert(specifications).List<T>();
        }

        public override T Fetch(Query<T> query)
        {
            throw new NotImplementedException("Full Linq queries are not implemented by this repository");
        }

        public override IEnumerable<T> Search(Query<T> query)
        {
            throw new NotImplementedException("Full Linq queries are not implemented by this repository");
        }
    }
}
