using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;

#if DOT_NET_35
using System.Linq;
using System.Linq.Expressions;
#endif

namespace System.BusinessObjects.Helpers
{
    /// <summary>
    /// Provides overloads for some operations in <see cref="NHibernate.Criterion.Restrictions"/> as lambda expressions
    /// </summary>
    public class RestrictBy
    {
        private RestrictBy() { }

#if DOT_NET_35
        /// <summary>
        /// Handles "Equals", "Greater Than", "Less Than", "Greater Than or Equal", "Less Than or Equal"
        /// </summary>
        /// <param name="propertyLambda">lambda function to evaluate: ()=> obj.A == 1</param>
        /// <returns>NHibernate SimpleExpression</returns>
        public static SimpleExpression Eq(Expression<Func<object>> propertyLambda)
        {
            ResolveLambda r = new ResolveLambda(propertyLambda);

            switch (r.OperationType)
            {
                case ExpressionType.Equal:
                    return Restrictions.Eq(r.PropertyName, r.Value);
                case ExpressionType.GreaterThanOrEqual:
                    return Restrictions.Ge(r.PropertyName, r.Value);
                case ExpressionType.GreaterThan:
                    return Restrictions.Gt(r.PropertyName, r.Value);
                case ExpressionType.LessThanOrEqual:
                    return Restrictions.Le(r.PropertyName, r.Value);
                case ExpressionType.LessThan:
                    return Restrictions.Lt(r.PropertyName, r.Value);
                default:
                    throw new NotSupportedException("Only ==,<,>,>=,<= operators supported in this function");
            }
        }

        public static AbstractEmptinessExpression IsEmpty(Expression<Func<object>> propertyLambda)
        {
            ResolveLambda r = new ResolveLambda(propertyLambda);
            if (r.OperationTypeSpecified)
            {
                throw new NotSupportedException("Only a property name is required");
            }
            return Restrictions.IsEmpty(r.PropertyName);
        }

        public static AbstractEmptinessExpression IsNotEmpty(Expression<Func<object>> propertyLambda)
        {
            ResolveLambda r = new ResolveLambda(propertyLambda);
            if (r.OperationTypeSpecified)
            {
                throw new NotSupportedException("Only a property name is required");
            }
            return Restrictions.IsNotEmpty(r.PropertyName);
        }

        public static AbstractCriterion IsNotNull(Expression<Func<object>> propertyLambda)
        {
            ResolveLambda r = new ResolveLambda(propertyLambda);
            if (r.OperationTypeSpecified)
            {
                throw new NotSupportedException("Only a property name is required");
            }
            return Restrictions.IsNotNull(r.PropertyName);
        }

        public static AbstractCriterion IsNull(Expression<Func<object>> propertyLambda)
        {
            ResolveLambda r = new ResolveLambda(propertyLambda);
            if (r.OperationTypeSpecified)
            {
                throw new NotSupportedException("Only a property name is required");
            }
            return Restrictions.IsNull(r.PropertyName);
        }

#endif
    }

#if DOT_NET_35
    /// <summary>
    /// A set of extensions methods for ICriteria to remove the need for .Add()
    /// </summary>
    public static class RestrictByExtensions
    {
        /// <summary>
        /// Handles "Equals", "Greater Than", "Less Than", "Greater Than or Equal", "Less Than or Equal"
        /// </summary>
        public static NHibernate.ICriteria AddEq(this NHibernate.ICriteria c, Expression<Func<object>> propertyLambda)
        {
            return c.Add(RestrictBy.Eq(propertyLambda));
        }
        public static NHibernate.ICriteria AddIsEmpty(this NHibernate.ICriteria c, Expression<Func<object>> propertyLambda)
        {
            return c.Add(RestrictBy.IsEmpty(propertyLambda));
        }
        public static NHibernate.ICriteria AddIsNotEmpty(this NHibernate.ICriteria c, Expression<Func<object>> propertyLambda)
        {
            return c.Add(RestrictBy.IsNotEmpty(propertyLambda));
        }
        public static NHibernate.ICriteria AddIsNotNull(this NHibernate.ICriteria c, Expression<Func<object>> propertyLambda)
        {
            return c.Add(RestrictBy.IsNotNull(propertyLambda));
        }
        public static NHibernate.ICriteria AddIsNull(this NHibernate.ICriteria c, Expression<Func<object>> propertyLambda)
        {
            return c.Add(RestrictBy.IsNull(propertyLambda));
        }
    }
#endif

    #region Resolve Lambda Helper
#if DOT_NET_35
    /// <summary>
    /// Helper class to break down and resolve lambda values
    /// </summary>
    internal class ResolveLambda
    {
        internal string PropertyName = "";
        internal object Value = null;
        private ExpressionType _operationType;
        internal bool OperationTypeSpecified = false;

        public ExpressionType OperationType
        {
            get
            {
                return _operationType;
            }
            set
            {
                OperationTypeSpecified = true;
                _operationType = value;
            }
        }

        internal ResolveLambda(Expression<Func<object>> expression)
        {
            Visit(expression);
        }

        internal void Visit(Expression<Func<object>> expression)
        {
            Visit(expression.Body);
        }
        internal void Visit(System.Linq.Expressions.Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.TypeIs:
                    if (expression is BinaryExpression)
                        Visit((BinaryExpression)expression);
                    else if (expression is UnaryExpression)
                        Visit((UnaryExpression)expression);
                    break;
                case ExpressionType.Constant:
                    Visit((ConstantExpression)expression);
                    break;
                case ExpressionType.MemberAccess:
                    Visit((MemberExpression)expression);
                    break;
                default:
                    throw new NotSupportedException(string.Format("lambda expression with type {0} not supported", expression.NodeType));

            }
        }
        internal void Visit(System.Linq.Expressions.UnaryExpression expression)
        {
            Visit(expression.Operand);
        }
        internal void Visit(System.Linq.Expressions.BinaryExpression expression)
        {
            OperationType = expression.NodeType;

            Visit(expression.Left);
            Visit(expression.Right);
        }
        internal void Visit(System.Linq.Expressions.ConstantExpression expression)
        {
            Value = expression.Value;
        }
        internal void Visit(System.Linq.Expressions.MemberExpression expression)
        {
            PropertyName = expression.Member.Name;
        }
    }

#endif
    #endregion
}
