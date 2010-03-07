using System;
using System.Collections.Generic;
using NHibernate;
using System.BusinessObjects.Infrastructure;

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
        public T Fetch(object Id)
        {
            T newObj = _session.Get<T>(Id);
            return newObj;
        }

        public abstract T Fetch(params ISpecification[] specifications);

        /// <summary>
        /// Returns a list of all business objects of this type
        /// </summary>
        public IEnumerable<T> Search()
        {
            NHibernate.ICriteria qry = _session.CreateCriteria(typeof(T));
            IEnumerable<T> list = qry.List<T>();
            return list;
        }

        public abstract IEnumerable<T> Search(params ISpecification[] specifications);

        public T Save(T src)
        {
            if (src.IsDeleted)
            {
                _session.Delete(src);
            }
            else
            {
                _session.SaveOrUpdate(src);
                src.SetSaveRowState();
            }
            src.IsDirty = false;
            return src;
        }
    }
}
