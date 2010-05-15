using System;
using System.Linq;
using System.Collections.Generic;
using System.BusinessObjects.Infrastructure;

namespace System.BusinessObjects.Data
{
    public interface IDataObjectRepository<T>
        where T : DataObject
    {
        T Fetch(params Specification<T>[] specifications);
        T Fetch(Query<T> query);
        T Fetch(object Id);
        IEnumerable<T> Search(params Specification<T>[] specifications);
        IEnumerable<T> Search(Query<T> query);
        IEnumerable<T> Search();
        IQueryable<T> AsQueryable(params Specification<T>[] specifications);
        IQueryable AsQueryable(Query<T> query);
        T Save(T src);
        void SubmitChanges();
    }
}
