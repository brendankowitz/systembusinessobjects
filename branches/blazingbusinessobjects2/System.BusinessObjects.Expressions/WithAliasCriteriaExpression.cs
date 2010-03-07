using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace System.BusinessObjects.Expressions
{
    public class WithAliasCriteriaExpression<T, R, CR> where CR : CriteriaExpression<T>
    {
        CR inner = null;
        string _alias;
        internal WithAliasCriteriaExpression(CR wrap, string alias)
        {
            inner = wrap;
            _alias = alias;
        }

        /// <summary>
        /// Return to the original expression
        /// </summary>
        public CR Expression { get { return inner; } }
        /// <summary>
        /// Adds a Restrction based on a lambda evaluation of: "Equals", "Greater Than", "Less Than", "Greater Than or Equal", "Less Than or Equal",
        /// "NotEqual", "NotNull" and "Between"
        /// </summary>
        public WithAliasCriteriaExpression<T, R, CR> Add(Expression<Func<R, bool>> propertyLambda)
        {
            inner.AddCriterion(RestrictBy.Add(propertyLambda, _alias));
            return this;
        }
        /// <summary>
        /// Performs the same as Add() but returns to the original expression wrapper
        /// </summary>
        public CR AddAndReturn(Expression<Func<R, bool>> propertyLambda)
        {
            inner.AddCriterion(RestrictBy.Add(propertyLambda, _alias));
            return inner;
        }
        /// <summary>
        /// Handles ==, != as a 'like'
        /// </summary>
        public WithAliasCriteriaExpression<T, R, CR> Like(Expression<Func<R, bool>> propertyLambda)
        {
            inner.AddCriterion(RestrictBy.Like(propertyLambda, _alias));
            return this;
        }
        /// <summary>
        /// Performs the same as Like() but returns to the original expression wrapper
        /// </summary>
        public CR LikeAndReturn(Expression<Func<R, bool>> propertyLambda)
        {
            inner.AddCriterion(RestrictBy.Like(propertyLambda, _alias));
            return inner;
        }
    }
}
