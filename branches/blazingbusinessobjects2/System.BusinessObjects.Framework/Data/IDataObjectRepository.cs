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
        T Fetch(CommandQuery query);
        T FetchById<TId>(TId Id);
        IEnumerable<T> Search(params Specification<T>[] specifications);
        IEnumerable<T> Search(Query<T> query);
        IEnumerable<T> Search(CommandQuery query);
        IEnumerable<T> Search();
        IQueryable<T> AsQueryable(params Specification<T>[] specifications);
        TReturnType AsQueryable<TReturnType>(Query<T> query) where TReturnType : IQueryable;
        T Save(T src);
        void SubmitChanges();
    }
}
