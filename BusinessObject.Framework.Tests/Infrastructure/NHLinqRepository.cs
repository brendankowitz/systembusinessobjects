using System;
using System.Collections.Generic;
using System.Linq;
using System.BusinessObjects.Data;
using System.BusinessObjects.Infrastructure;
using NHibernate;
using NHibernate.Linq;

namespace BusinessObject.Framework.Tests
{
    public class NHLinqRepository<T> : GenericDataObjectRepository<T>
        where T : DataObject
    {
        public NHLinqRepository(ISession session)
            : base(session)
        {
        }

        INHibernateQueryable<T> Convert(Specification<T>[] specifications)
        {
            var qry = _session.Linq<T>();
            foreach (var spec in specifications)
            {
                qry.Where(spec.Predicate);
            }
            return qry;
        }

        public override T Fetch(params Specification<T>[] specifications)
        {
            return Convert(specifications).SingleOrDefault();
        }

        public override IEnumerable<T> Search(params Specification<T>[] specifications)
        {
            return Convert(specifications).ToList();
        }

        public override T Fetch(System.BusinessObjects.Infrastructure.Query<T> query)
        {
            var qry = _session.Linq<T>();
            query.Expression(qry);
            return qry.SingleOrDefault();
        }

        public override IEnumerable<T> Search(System.BusinessObjects.Infrastructure.Query<T> query)
        {
            var qry = _session.Linq<T>();
            query.Expression(qry);
            return qry.ToList();
        }
    }
}
