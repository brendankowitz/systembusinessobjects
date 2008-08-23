using System;
using System.Collections.Generic;
using System.Text;
using System.BusinessObjects.Helpers;

#if DOT_NET_35
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Criterion;
#endif

namespace System.BusinessObjects.Expressions
{
#if DOT_NET_35
    /// <summary>
    /// A wrapper class for NHibernate's ICriteria
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CriteriaExpression<T>
    {
        internal NHibernate.ICriteria inner = null;
        internal CriteriaExpression(NHibernate.ICriteria wrap)
        {
            Criteria = wrap;
        }

        /// <summary>
        /// Returns the NHibernate Criteria that has been built using this wrapper
        /// </summary>
        public NHibernate.ICriteria Criteria
        {
            get
            {
                return inner;
            }
            set
            {
                inner = value;
            }
        }

        /// <summary>
        /// Adds an IsEmpty criteria for a collection
        /// </summary>
        public CriteriaExpression<T> IsEmpty(Expression<Func<T, object>> propertyLambda)
        {
            Criteria.Add(RestrictBy.IsEmpty(propertyLambda));
            return this;
        }

        /// <summary>
        /// Handles ==, != as a 'like'
        /// </summary>
        public CriteriaExpression<T> Like(Expression<Func<T, object>> propertyLambda)
        {
            Criteria.Add(RestrictBy.Like(propertyLambda));
            return this;
        }

        /// <summary>
        /// Adds criteria for "Equals", "Greater Than", "Less Than", "Greater Than or Equal", "Less Than or Equal",
        /// "NotEqual", "NotNull" and "Between"
        /// </summary>
        public CriteriaExpression<T> Add(Expression<Func<T, object>> propertyLambda)
        {
            Criteria.Add(RestrictBy.Add(propertyLambda));
            return this;
        }

        /// <summary>
        /// Adds an NHibernate Criterion
        /// </summary>
        public CriteriaExpression<T> Add(ICriterion c)
        {
            Criteria.Add(c);
            return this;
        }

        /// <summary>
        /// Sets the NHibernate Projection
        /// </summary>
        public CriteriaExpression<T> SetProjection(ProjectionFunction target, Expression<Func<T, object>> propertyLambda)
        {
            string prop = System.BusinessObjects.Helpers.Property.For<T>(propertyLambda);
            Criteria.SetProjection(target(prop));
            return this;
        }

        /// <summary>
        /// Sets the NHibernate Projection
        /// </summary>
        public CriteriaExpression<T> SetProjection(IProjection projection)
        {
            Criteria.SetProjection(projection);
            return this;
        }

        /// <summary>
        /// Adds an Order by Asc by evaluating a property name
        /// </summary>
        public CriteriaExpression<T> OrderAsc(Expression<Func<T, object>> propertyLambda)
        {
            Criteria.AddOrder(RestrictBy.OrderAsc(propertyLambda));
            return this;
        }

        /// <summary>
        /// Adds an Order by Desc by evaluating a property name
        /// </summary>
        public CriteriaExpression<T> OrderDesc(Expression<Func<T, object>> propertyLambda)
        {
            Criteria.AddOrder(RestrictBy.OrderDesc(propertyLambda));
            return this;
        }

        /// <summary>
        /// Join an association, adding an alias to the joined entity
        /// </summary>
        public CriteriaExpression<T> Alias(Expression<Func<T, object>> propertyLambda, string alias)
        {
            Criteria.CreateAlias(System.BusinessObjects.Helpers.Property.For<T>(propertyLambda), alias);
            return this;
        }

        /// <summary>
        /// Join an association, adding an alias to the joined entity and returning a wrapper to add
        /// criteria to the joined entity.
        /// </summary>
        public WithAliasCriteriaExpression<T, R> Alias<R>(Expression<Func<T, object>> propertyLambda, string alias)
        {
            Criteria.CreateAlias(System.BusinessObjects.Helpers.Property.For<T>(propertyLambda), alias);
            return new WithAliasCriteriaExpression<T, R>(this, alias);
        }

        /// <summary>
        /// Uses an existing Query alias
        /// </summary>
        public WithAliasCriteriaExpression<T, R> WithAlias<R>(string alias)
        {
            return new WithAliasCriteriaExpression<T, R>(this, alias);
        }
    }
#endif
}
