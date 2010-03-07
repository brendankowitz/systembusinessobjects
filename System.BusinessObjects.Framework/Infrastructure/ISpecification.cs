using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.BusinessObjects.Infrastructure
{
    public interface ISpecification
    {
        bool IsSatisfiedBy(object candidate);
    }
}
