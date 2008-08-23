using System;
using System.Collections.Generic;
using System.Text;

#if DOT_NET_35
using System.Linq;
using System.Linq.Expressions;
#endif

namespace System.BusinessObjects.Expressions
{
#if DOT_NET_35
    public class WithAliasCriteriaExpression<T, R>
    {
        CriteriaExpression<T> inner = null;
        string _alias;
        internal WithAliasCriteriaExpression(CriteriaExpression<T> wrap, string alias)
        {
            inner = wrap;
            _alias = alias;
        }

        /// <summary>
        /// Return to the original expression
        /// </summary>
        public CriteriaExpression<T> Expression { get { return inner; } }
        /// <summary>
        /// Adds a Restrction based on a lambda evaluation of: "Equals", "Greater Than", "Less Than", "Greater Than or Equal", "Less Than or Equal",
        /// "NotEqual", "NotNull" and "Between"
        /// </summary>
        public WithAliasCriteriaExpression<T, R> Add(Expression<Func<R, object>> propertyLambda)
        {
            inner.Criteria.Add(RestrictBy.Add(propertyLambda, _alias));
            return this;
        }
        /// <summary>
        /// Performs the same as Add() but returns to the original expression wrapper
        /// </summary>
        public CriteriaExpression<T> AddAndReturn(Expression<Func<R, object>> propertyLambda)
        {
            inner.Criteria.Add(RestrictBy.Add(propertyLambda, _alias));
            return inner;
        }
        /// <summary>
        /// Handles ==, != as a 'like'
        /// </summary>
        public WithAliasCriteriaExpression<T, R> Like(Expression<Func<R, object>> propertyLambda)
        {
            inner.Criteria.Add(RestrictBy.Like(propertyLambda, _alias));
            return this;
        }
        /// <summary>
        /// Performs the same as Like() but returns to the original expression wrapper
        /// </summary>
        public CriteriaExpression<T> LikeAndReturn(Expression<Func<R, object>> propertyLambda)
        {
            inner.Criteria.Add(RestrictBy.Like(propertyLambda, _alias));
            return inner;
        }
    }
#endif
}
