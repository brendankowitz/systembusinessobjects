using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.BusinessObjects.Infrastructure;

namespace System.BusinessObjects.Data
{
    public sealed class Join<T> : ISpecification
    {
        internal ISpecification _first;
        public Expression<Func<T, object>> Predicate { get; private set; }

        public Join(ISpecification first, Expression<Func<T, object>> property)
        {
            _first = first;
            Predicate = property;
        }

        public bool IsSatisfiedBy(object candidate)
        {
            throw new NotImplementedException();
        }
    }
}
