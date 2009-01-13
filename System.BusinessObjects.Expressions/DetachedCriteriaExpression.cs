using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NHibernate.Criterion;

namespace System.BusinessObjects.Expressions
{
    /// <summary>
    /// A wrapper class for NHibernate's DetachedCriteria
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DetachedCriteriaExpression<T> : CriteriaExpression<T>
    {
        public DetachedCriteriaExpression(NHibernate.Criterion.DetachedCriteria wrap)
        {
            innerDetached = wrap;
        }

        /// <summary>
        /// Creates a new DetachedCriteriaExpression for type T
        /// </summary>
        public static DetachedCriteriaExpression<T> Create()
        {
            return new DetachedCriteriaExpression<T>(NHibernate.Criterion.DetachedCriteria.For<T>());
        }

        /// <summary>
        /// Returns the NHibernate Criteria that has been built using this wrapper
        /// </summary>
        public new DetachedCriteria Criteria
        {
            get
            {
                return innerDetached;
            }
            set
            {
                innerDetached = value;
            }
        }

        /// <summary>
        /// Returns the NHibernate Criteria that has been built using this wrapper
        /// </summary>
        public NHibernate.ICriteria Generate(NHibernate.ISession session)
        {
            if (inner == null)
            {
                inner = innerDetached.GetExecutableCriteria(session);
                return inner;
            }
            else
            {
                throw new ApplicationException("Expression has already been associated with a Session.");
            }
        }

        /// <summary>
        /// Adds an IsEmpty criteria for a collection
        /// </summary>
        public new DetachedCriteriaExpression<T> IsEmpty(Expression<Func<T, object>> propertyLambda)
        {
            return base.IsEmpty(propertyLambda) as DetachedCriteriaExpression<T>;
        }

        /// <summary>
        /// Handles ==, != as a 'like'
        /// </summary>
        public new DetachedCriteriaExpression<T> Like(Expression<Func<T, object>> propertyLambda)
        {
            return base.Like(propertyLambda) as DetachedCriteriaExpression<T>;
        }

        /// <summary>
        /// Adds criteria for "Equals", "Greater Than", "Less Than", "Greater Than or Equal", "Less Than or Equal",
        /// "NotEqual", "NotNull" and "Between"
        /// </summary>
        public new DetachedCriteriaExpression<T> Add(Expression<Func<T, object>> propertyLambda)
        {
            return base.Add(propertyLambda) as DetachedCriteriaExpression<T>;
        }

        /// <summary>
        /// Adds an NHibernate Criterion
        /// </summary>
        public new DetachedCriteriaExpression<T> Add(ICriterion c)
        {
            return base.Add(c) as DetachedCriteriaExpression<T>;
        }

        /// <summary>
        /// Sets the NHibernate Projection
        /// </summary>
        public new DetachedCriteriaExpression<T> SetProjection(ProjectionFunction target, Expression<Func<T, object>> propertyLambda)
        {
            return base.SetProjection(target, propertyLambda) as DetachedCriteriaExpression<T>;
        }

        /// <summary>
        /// Sets the NHibernate Projection
        /// </summary>
        public new DetachedCriteriaExpression<T> SetProjection(IProjection projection)
        {
            return base.SetProjection(projection) as DetachedCriteriaExpression<T>;
        }

        /// <summary>
        /// Adds an Order by Asc by evaluating a property name
        /// </summary>
        public new DetachedCriteriaExpression<T> OrderAsc(Expression<Func<T, object>> propertyLambda)
        {
            return base.OrderAsc(propertyLambda) as DetachedCriteriaExpression<T>;
        }

        /// <summary>
        /// Adds an Order by Desc by evaluating a property name
        /// </summary>
        public new DetachedCriteriaExpression<T> OrderDesc(Expression<Func<T, object>> propertyLambda)
        {
            return base.OrderDesc(propertyLambda) as DetachedCriteriaExpression<T>;
        }

        /// <summary>
        /// Join an association, adding an alias to the joined entity
        /// </summary>
        public new DetachedCriteriaExpression<T> Alias(Expression<Func<T, object>> propertyLambda, string alias)
        {
            return base.Alias(propertyLambda, alias) as DetachedCriteriaExpression<T>;
        }

        /// <summary>
        /// Join an association, adding an alias to the joined entity and returning a wrapper to add
        /// criteria to the joined entity.
        /// </summary>
        public WithAliasCriteriaExpression<T, R, DetachedCriteriaExpression<T>> Alias<R>(Expression<Func<T, object>> propertyLambda, string alias)
        {
            AddAlias(Property.For<T>(propertyLambda), alias);
            return new WithAliasCriteriaExpression<T, R, DetachedCriteriaExpression<T>>(this, alias);
        }

        /// <summary>
        /// Uses an existing Query alias
        /// </summary>
        public WithAliasCriteriaExpression<T, R, DetachedCriteriaExpression<T>> WithAlias<R>(string alias)
        {
            return new WithAliasCriteriaExpression<T, R, DetachedCriteriaExpression<T>>(this, alias);
        }
    }
}
