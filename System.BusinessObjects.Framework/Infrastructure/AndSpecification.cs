using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.BusinessObjects.Infrastructure
{
    public sealed class AndSpecification : ISpecification
    {
        internal ISpecification One;
        internal ISpecification Other;

        internal AndSpecification(ISpecification x, ISpecification y)
        {
            One = x;
            Other = y;
        }

        public bool IsSatisfiedBy(object candidate)
        {
            return One.IsSatisfiedBy(candidate) && Other.IsSatisfiedBy(candidate);
        }
    }
}
