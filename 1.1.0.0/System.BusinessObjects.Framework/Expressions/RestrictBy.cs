using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;

#if DOT_NET_35
using System.Linq;
using System.Linq.Expressions;
#endif

namespace System.BusinessObjects.Expressions
{
    /// <summary>
    /// Provides overloads for some operations in <see cref="NHibernate.Criterion.Restrictions"/> as lambda expressions
    /// </summary>
    public class RestrictBy
    {
        private RestrictBy() { }

#if DOT_NET_35
        /// <summary>
        /// Adds an IsEmpty criteria for a collection
        /// </summary>
        public static AbstractEmptinessExpression IsEmpty<T>(Expression<Func<T, object>> propertyLambda)
        {
            ResolveLambda r = new ResolveLambda();
            r.Resolve<T>(propertyLambda);
            if (r.OperationTypeSpecified)
            {
                throw new NotSupportedException("Only a property name is required");
            }
            return Restrictions.IsEmpty(r.PropertyName);
        }

        /// <summary>
        /// Adds an IsNotEmpty criteria for a collection
        /// </summary>
        public static AbstractEmptinessExpression IsNotEmpty<T>(Expression<Func<T, object>> propertyLambda)
        {
            ResolveLambda r = new ResolveLambda();
            r.Resolve<T>(propertyLambda);
            if (r.OperationTypeSpecified)
            {
                throw new NotSupportedException("Only a collection property is required");
            }
            return Restrictions.IsNotEmpty(r.PropertyName);
        }

        /// <summary>
        /// Handles ==, != as a 'like' criteria
        /// </summary>
        public static AbstractCriterion Like<T>(Expression<Func<T, object>> propertyLambda)
        {
            return Like(propertyLambda, null);
        }

        /// <summary>
        /// Handles ==, != as a 'like' criteria
        /// </summary>
        public static AbstractCriterion Like<T>(Expression<Func<T, object>> propertyLambda, string alias)
        {
            ResolveLambda r = new ResolveLambda();
            r.Resolve<T>(propertyLambda);
            AbstractCriterion retval;

            if (!string.IsNullOrEmpty(alias))
                alias += ".";
            else
                alias = "";

            ExpressionType expType = r.OperationType;
            switch (expType)
            {
                case ExpressionType.NotEqual:
                    retval = Restrictions.Not(Restrictions.Like(alias + r.PropertyName, r.Value));
                    break;
                case ExpressionType.Equal:
                    retval = Restrictions.Like(alias + r.PropertyName, r.Value);
                    break;
                default:
                    throw new NotSupportedException(string.Format("{0} operation not supported in like", expType));
            }

            return retval;
        }

        /// <summary>
        /// Adds criteria for "Equals", "Greater Than", "Less Than", "Greater Than or Equal", "Less Than or Equal",
        /// "NotEqual", "NotNull" and "Between"
        /// </summary>
        public static AbstractCriterion Add<T>(Expression<Func<T, object>> propertyLambda)
        {
            return Add(propertyLambda, null);
        }

        /// <summary>
        /// Adds criteria for "Equals", "Greater Than", "Less Than", "Greater Than or Equal", "Less Than or Equal",
        /// "NotEqual", "NotNull" and "Between"
        /// </summary>
        /// <param name="propertyLambda">lambda function to evaluate: p => p.Name == "string"</param>
        public static AbstractCriterion Add<T>(Expression<Func<T, object>> propertyLambda, string alias)
        {
            AbstractCriterion retval;
            ResolveLambda r = new ResolveLambda();
            r.Resolve<T>(propertyLambda);

            if (!string.IsNullOrEmpty(alias))
                alias += ".";
            else
                alias = "";

            ExpressionType outerType = r.OperationType;
            object val1;
            switch (outerType)
            {
                case ExpressionType.NotEqual:
                    val1 = r.Value;
                    if (val1 == null)
                        retval = Restrictions.IsNotNull(alias + r.PropertyName);
                    else
                        retval = Restrictions.Not(Restrictions.Eq(alias + r.PropertyName, val1));
                    break;
                case ExpressionType.Equal:
                    val1 = r.Value;
                    if(val1 == null)
                        retval = Restrictions.IsNull(alias + r.PropertyName);
                    else
                        retval = Restrictions.Eq(alias + r.PropertyName, val1);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    retval = Restrictions.Ge(alias + r.PropertyName, r.Value);
                    break;
                case ExpressionType.GreaterThan:
                    retval = Restrictions.Gt(alias + r.PropertyName, r.Value);
                    break;
                case ExpressionType.LessThanOrEqual:
                    retval = Restrictions.Le(alias + r.PropertyName, r.Value);
                    break;
                case ExpressionType.LessThan:
                    retval = Restrictions.Lt(alias + r.PropertyName, r.Value);
                    break;
                case ExpressionType.AndAlso: //Between
                    ExpressionType innerType1 = r.OperationType;
                    ExpressionType innerType2 = r.OperationType;
                    if (innerType1 == ExpressionType.GreaterThan && innerType2 == ExpressionType.LessThan)
                        retval = Restrictions.Between(alias + r.PropertyName, r.Value, r.Value);
                    else if (innerType1 == ExpressionType.LessThan && innerType2 == ExpressionType.GreaterThan)
                    {
                        val1 = r.Value;
                        retval = Restrictions.Between(alias + r.PropertyName, r.Value, val1);
                    }
                    else
                        throw new NotSupportedException("Only Between expressions can be used in this context");
                    break;
                default:
                    throw new NotSupportedException("Only ==,<,>,>=,<=,&& operators supported in this function");
            }
            return retval;
        }

        /// <summary>
        /// Adds an Order by Asc by evaluating a property name
        /// </summary>
        public static Order OrderAsc<T>(Expression<Func<T, object>> propertyLambda)
        {
            ResolveLambda r = new ResolveLambda();
            r.Resolve<T>(propertyLambda);
            if (r.OperationTypeSpecified)
            {
                throw new NotSupportedException("Only a property name is required");
            }
            return Order.Asc(r.PropertyName);
        }

        /// <summary>
        /// Adds an Order by Desc by evaluating a property name
        /// </summary>
        public static Order OrderDesc<T>(Expression<Func<T, object>> propertyLambda)
        {
            ResolveLambda r = new ResolveLambda();
            r.Resolve<T>(propertyLambda);
            if (r.OperationTypeSpecified)
            {
                throw new NotSupportedException("Only a property name is required");
            }
            return Order.Desc(r.PropertyName);
        }
#endif
    }
}
