using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.Data;

namespace System.BusinessObjects.Data
{
    public class GenericDataObjectRepository<T> : IDataObjectRepository<T>
        where T : DataObject
    {
        private readonly ISession _session;
        public GenericDataObjectRepository(ISession session)
        {
            _session = session;
        }

        private void CheckSessionInterface(IDataQuery<T> query)
        {
            if(query is IRequiresNHibernateSession)
            {
                ((IRequiresNHibernateSession)query).Session = _session;
            }
        }

        /// <summary>
        /// Loads a business object with the given ID
        /// </summary>
        public T Fetch(object Id)
        {
            T newObj = _session.Get<T>(Id);
            return newObj;
        }

        public T Fetch(IDataQuery<T> query)
        {
            CheckSessionInterface(query);
            return query.Execute().FirstOrDefault();
        }

        /// <summary>
        /// Returns a list of all business objects of this type
        /// </summary>
        public IEnumerable<T> Search()
        {
            NHibernate.ICriteria qry = _session.CreateCriteria(typeof(T));
            IEnumerable<T> list = qry.List<T>();
            return list;
        }

        public IEnumerable<T> Search(IDataQuery<T> query)
        {
            CheckSessionInterface(query);
            IEnumerable<T> list = query.Execute();
            return list;
        }

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
