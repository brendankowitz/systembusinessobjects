using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.BusinessObjects.Data;

namespace System.BusinessObjects.Infrastructure
{
    public static class SpecificationExtensions
    {
        public static ISpecification And(this ISpecification src, ISpecification other)
        {
            return new AndSpecification(src, other);
        }

        public static ISpecification Or(this ISpecification src, ISpecification other)
        {
            throw new NotImplementedException();
        }

        public static ISpecification Join<T>(this Specification<T> src, Expression<Func<T, object>> property)
        {
            return new Join<T>(src, property);
        }

        public static ISpecification Not(this ISpecification src)
        {
            throw new NotImplementedException();
        }
    }
}
