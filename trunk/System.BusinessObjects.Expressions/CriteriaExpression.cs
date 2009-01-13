using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Criterion;

namespace System.BusinessObjects.Expressions
{
    /// <summary>
    /// A wrapper class for NHibernate's ICriteria
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CriteriaExpression<T>
    {
        internal NHibernate.ICriteria inner = null;
        internal NHibernate.Criterion.DetachedCriteria innerDetached = null;

        internal CriteriaExpression()
        {
        }
        internal CriteriaExpression(NHibernate.ICriteria wrap) 
        {
            inner = wrap;
        }

        #region internal add methods
        internal void AddCriterion(ICriterion c)
        {
            if (inner != null)
                inner.Add(c);
            else if (innerDetached != null)
                innerDetached.Add(c);
        }
        internal void AddOrder(Order c)
        {
            if (inner != null)
                inner.AddOrder(c);
            else if (innerDetached != null)
                innerDetached.AddOrder(c);
        }
        internal void AddAlias(string property, string alias)
        {
            if (inner != null)
                inner.CreateAlias(property, alias);
            else if (innerDetached != null)
                innerDetached.CreateAlias(property, alias);
        }
        internal void AddProjection(IProjection c)
        {
            if (inner != null)
                inner.SetProjection(c);
            else if (innerDetached != null)
                innerDetached.SetProjection(c);
        }
        #endregion

        /// <summary>
        /// Returns the NHibernate Criteria that has been built using this wrapper
        /// </summary>
        public virtual NHibernate.ICriteria Criteria
        {
            get
            {
                if (inner == null && innerDetached != null)
                    throw new ApplicationException("Call Generate() to create an ICriteria.");
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
            AddCriterion(RestrictBy.IsEmpty(propertyLambda));
            return this;
        }

        /// <summary>
        /// Handles ==, != as a 'like'
        /// </summary>
        public CriteriaExpression<T> Like(Expression<Func<T, object>> propertyLambda)
        {
            AddCriterion(RestrictBy.Like(propertyLambda));
            return this;
        }

        /// <summary>
        /// Adds criteria for "Equals", "Greater Than", "Less Than", "Greater Than or Equal", "Less Than or Equal",
        /// "NotEqual", "NotNull" and "Between"
        /// </summary>
        public CriteriaExpression<T> Add(Expression<Func<T, object>> propertyLambda)
        {
            AddCriterion(RestrictBy.Add(propertyLambda));
            return this;
        }

        /// <summary>
        /// Adds an NHibernate Criterion
        /// </summary>
        public CriteriaExpression<T> Add(ICriterion c)
        {
            AddCriterion(c);
            return this;
        }

        /// <summary>
        /// Sets the NHibernate Projection
        /// </summary>
        public CriteriaExpression<T> SetProjection(ProjectionFunction target, Expression<Func<T, object>> propertyLambda)
        {
            string prop = Property.For<T>(propertyLambda);
            AddProjection(target(prop));
            return this;
        }

        /// <summary>
        /// Sets the NHibernate Projection
        /// </summary>
        public CriteriaExpression<T> SetProjection(IProjection projection)
        {
            AddProjection(projection);
            return this;
        }

        /// <summary>
        /// Adds an Order by Asc by evaluating a property name
        /// </summary>
        public CriteriaExpression<T> OrderAsc(Expression<Func<T, object>> propertyLambda)
        {
            AddOrder(RestrictBy.OrderAsc(propertyLambda));
            return this;
        }

        /// <summary>
        /// Adds an Order by Desc by evaluating a property name
        /// </summary>
        public CriteriaExpression<T> OrderDesc(Expression<Func<T, object>> propertyLambda)
        {
            AddOrder(RestrictBy.OrderDesc(propertyLambda));
            return this;
        }

        /// <summary>
        /// Join an association, adding an alias to the joined entity
        /// </summary>
        public CriteriaExpression<T> Alias(Expression<Func<T, object>> propertyLambda, string alias)
        {
            AddAlias(Property.For<T>(propertyLambda), alias);
            return this;
        }

        /// <summary>
        /// Join an association, adding an alias to the joined entity and returning a wrapper to add
        /// criteria to the joined entity.
        /// </summary>
        public WithAliasCriteriaExpression<T, R, CriteriaExpression<T>> Alias<R>(Expression<Func<T, object>> propertyLambda, string alias)
        {
            AddAlias(Property.For<T>(propertyLambda), alias);
            return new WithAliasCriteriaExpression<T, R, CriteriaExpression<T>>(this, alias);
        }

        /// <summary>
        /// Uses an existing Query alias
        /// </summary>
        public WithAliasCriteriaExpression<T, R, CriteriaExpression<T>> WithAlias<R>(string alias)
        {
            return new WithAliasCriteriaExpression<T, R, CriteriaExpression<T>>(this, alias);
        }
    }
}
