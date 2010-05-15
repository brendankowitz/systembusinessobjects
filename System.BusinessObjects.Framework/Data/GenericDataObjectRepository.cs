using System;
using System.Collections.Generic;
using NHibernate;
using System.BusinessObjects.Infrastructure;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Impl;
using System.Data;

namespace System.BusinessObjects.Data
{
    public abstract class GenericDataObjectRepository<T> : IDataObjectRepository<T>, IScopedRepository<T>
        where T : DataObject
    {
        protected readonly ISession _session;
        public GenericDataObjectRepository(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Loads a business object with the given ID
        /// </summary>
        public T Fetch(object Id)
        {
            T newObj = _session.Get<T>(Id);
            return newObj;
        }

        public abstract T Fetch(params Specification<T>[] specifications);

        public abstract T Fetch(Query<T> query);

        /// <summary>
        /// Returns a list of all business objects of this type
        /// </summary>
        public IEnumerable<T> Search()
        {
            NHibernate.ICriteria qry = _session.CreateCriteria(typeof(T));
            IEnumerable<T> list = qry.List<T>();
            return list;
        }

        public abstract IEnumerable<T> Search(params Specification<T>[] specifications);

        public abstract IEnumerable<T> Search(Query<T> query);

        public T Save(T src)
        {
            DataRowState state = GetRowstate(src);

            if (src.IsDeleted && state != DataRowState.Deleted)
            {
                _session.Delete(src);
            }
            else if(!src.IsDeleted && src.IsDirty)
            {
                if (state == DataRowState.Detached || state == DataRowState.Added)
                {
                    _session.Save(src);
                }
                else
                {
                    _session.Update(src);
                }
                src.SetSaveRowState();
            }
            src.IsDirty = false;
            return src;
        }

        private EntityEntry GetEntry(T obj)
        {
                return ((SessionImpl)_session).PersistenceContext.GetEntry(obj);
        }

        private DataRowState GetRowstate(T obj)
        {
             EntityEntry e = GetEntry(obj);
             if (e != null)
             {
                 switch (e.Status)
                 {
                     case Status.Deleted:
                     case Status.Gone:
                         return DataRowState.Deleted;
                     case Status.Loading:
                     case Status.Loaded:
                         try
                         {
                             int[] changes =
                                 e.Persister.FindDirty(e.LoadedState, e.Persister.GetPropertyValues(this, EntityMode.Map), this,
                                                       ((SessionImpl)_session).
                                                           GetSessionImplementation());
                             if (changes == null)
                                 return DataRowState.Unchanged;
                             else
                                 return DataRowState.Modified;
                         }
                         catch
                         {
                             return DataRowState.Unchanged;
                         }

                     default:
                         if (e.ExistsInDatabase)
                             return DataRowState.Modified;
                         else
                             return DataRowState.Detached;
                 }
             }
             return DataRowState.Detached;
        }

        public void SubmitChanges()
        {
            _session.Flush();
        }

        public abstract IQueryable<T> AsQueryable(params Specification<T>[] specifications);

        public abstract IQueryable AsQueryable(Query<T> query);

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            SubmitChanges();
        }

        #endregion
    }
}
