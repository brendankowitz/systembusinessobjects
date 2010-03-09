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
        T Fetch(params Specification<T>[] specifications);
        T Fetch(Query<T> query);
        IEnumerable<T> Search(params Specification<T>[] specifications);
        IEnumerable<T> Search(Query<T> query);
        IEnumerable<T> Search();
        T Save(T src);
    }
}
