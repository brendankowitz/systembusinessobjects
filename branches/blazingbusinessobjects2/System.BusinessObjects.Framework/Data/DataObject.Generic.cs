using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Iesi.Collections.Generic;
using System.Linq.Expressions;

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
    }
}