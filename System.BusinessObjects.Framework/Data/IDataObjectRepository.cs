using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.BusinessObjects.Infrastructure;

namespace System.BusinessObjects.Data
{
    public interface IDataObjectRepository<T>
        where T : DataObject
    {
        T Fetch(object Id);

        T Fetch(params ISpecification[] query);

        IEnumerable<T> Search();

        IEnumerable<T> Search(params ISpecification[] query);

        T Save(T src);
    }
}
