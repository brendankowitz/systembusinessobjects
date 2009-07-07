using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Iesi.Collections.Generic;

#if DOT_NET_35
using System.Linq.Expressions;
#endif

namespace System.BusinessObjects.Data
{
    /// <summary>
    /// An abstract class that provides core business object functionality.
    /// This generic class provides extensions to create a strongly typed interface
    /// for the inherited class.
    /// </summary>
    [Serializable]
    public abstract class DataObject<T> : DataObject
        where T : DataObject
    {
        /// <summary>
        /// Clone a deep-copy of this object
        /// </summary>
        public new virtual T Clone()
        {
            return (T)binarySerialiseClone(this);
        }

        /// <summary>
        /// Loads a business object with the given ID
        /// </summary>
        public static new T Load(object Id)
        {
            return Load<T>(Id);
        }

        /// <summary>
        /// Loads a business object with the given ID
        /// </summary>
        public static new T Load(object Id, ISession session)
        {
            return Load<T>(Id, session);
        }

#if DOT_NET_35
        /// <summary>
        /// Returns true if the property is null or has never been set
        /// </summary>
        public virtual bool IsNull(Expression<Func<T, object>> propertyLambda)
        {
            return IsNull(System.BusinessObjects.Helpers.Property.For<T>(propertyLambda));
        }

        /// <summary>
        /// Gets a value from the internal data store
        /// </summary>
        /// <remarks>
        /// Specifying TRetVal in the input type (instead of 'object') "Expression[Func[T, TRetVal]]" 
        /// causes the returning Generic Type to be inferred from the passed in property.
        /// Very cool.
        /// </remarks>
        protected TRetVal GetValue<TRetVal>(Expression<Func<T, TRetVal>> propertyLambda)
        {
            return GetValue<TRetVal>(System.BusinessObjects.Helpers.Property.For<T, TRetVal>(propertyLambda));
        }

        /// <summary>
        /// Sets a property value in the internal property store.
        /// If a null is passed the property will be reset and removed.
        /// </summary>
        protected void SetValue(Expression<Func<T, object>> propertyLambda, object value)
        {
            SetValue(System.BusinessObjects.Helpers.Property.For<T>(propertyLambda), value);
        }
#endif

        /// <summary>
        /// Gets a strongly typed list of all business objects of this type
        /// </summary>
        public static IList<T> Search()
        {
            return Search<T>();
        }

        /// <summary>
        /// Gets a strongly typed list of all business objects of this type
        /// </summary>
        public static IList<T> Search(NHibernate.Criterion.Order orderBy)
        {
            return Search<T>(orderBy);
        }

        /// <summary>
        /// Gets a strongly typed list of business objects based on NHibernate criteria
        /// </summary>
        public static IList<T> Search(NHibernate.ICriteria criteria)
        {
            return Search<T>(criteria);
        }

#if DOT_NET_35
        /// <summary>
        /// Gets a strongly typed list of business objects based on a linq expression
        /// </summary>
        public static IList<T> Search(IEnumerable<T> linqExpression)
        {
            return Search<T>(linqExpression);
        }
#endif

        /// <summary>
        /// Gets a strongly typed list of business objects based on an NHibernate Query
        /// </summary>
        public static IList<T> Search(NHibernate.IQuery query)
        {
            return Search<T>(query);
        }

        /// <summary>
        /// Gets a strongly typed business object based on NHibernate criteria
        /// </summary>
        public static T Fetch(NHibernate.ICriteria criteria)
        {
            return Fetch<T>(criteria);
        }

#if DOT_NET_35
        /// <summary>
        /// Gets a strongly typed business object based on a linq expression
        /// </summary>
        public static T Fetch(IEnumerable<T> linqExpression)
        {
            return Fetch<T>(linqExpression);
        }
#endif

        /// <summary>
        /// Gets a strongly typed business object based on an NHibernate Query
        /// </summary>
        public static T Fetch(NHibernate.IQuery query)
        {
            return Fetch<T>(query);
        }

        /// <summary>
        /// Evicts an existing instance of this business object from NHibernate's session cache
        /// </summary>
        public static void Evict(object ID)
        {
            Evict<T>(ID);
        }

        /// <summary>
        /// Reinstantiates an object from Xml
        /// </summary>
        public static T DeserializeFromXml(string xml)
        {
           return DeserializeFromXml<T>(xml);
        }
    }
}
