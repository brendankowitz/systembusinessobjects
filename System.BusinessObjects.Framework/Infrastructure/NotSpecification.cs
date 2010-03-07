using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.BusinessObjects.Infrastructure
{
    public sealed class NotSpecification : ISpecification
    {
        private ISpecification Wrapped;

        public NotSpecification(ISpecification x)
        {
            Wrapped = x;
        }

        public bool IsSatisfiedBy(object candidate)
        {
            return !Wrapped.IsSatisfiedBy(candidate);
        }
    }
}
