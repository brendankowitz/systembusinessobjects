using System;
using System.Collections.Generic;
using NHibernate;
using System.BusinessObjects.Infrastructure;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Impl;
using System.Data;
using NHibernate.Type;

namespace System.BusinessObjects.Data
{
    public abstract class GenericDataObjectRepository<T> : IDataObjectRepository<T>
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
        public T FetchById<TId>(TId Id)
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

        public abstract TReturnType AsQueryable<TReturnType>(Query<T> query) where TReturnType : IQueryable;

        public virtual T Fetch(CommandQuery query)
        {
            var qry = ConvertQuery(query);
            return qry.UniqueResult<T>();
        }

        public virtual IEnumerable<T> Search(CommandQuery query)
        {
            IQuery qry = ConvertQuery(query);
            return qry.List<T>();
        }

        private IQuery ConvertQuery(CommandQuery query)
        {
            var qry = _session.CreateQuery(query.Command);
            foreach (var p in query.Parameters)
            {
                if (p.Value != null)
                    qry.SetParameter(p.ParameterName, p.Value);

                if (p.ParameterName == "LIMIT")
                    qry.SetMaxResults(Convert.ToInt32(p.Value));
                else
                {
                    if (p.Value == null)
                    { //NH will error if passed a null value, so try to figure out the IType from the DbType
                        IType guessType = NHibernate.Type.TypeFactory.HeuristicType(p.DbType.ToString());
                        qry.SetParameter(p.ParameterName, p.Value, guessType);
                    }
                    else
                        qry.SetParameter(p.ParameterName, p.Value);
                }
            }
            return qry;
        }
    }
}
