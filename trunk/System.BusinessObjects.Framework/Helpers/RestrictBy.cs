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
        public static AbstractCriterion Eq<T>(Expression<Func<T, object>> propertyLambda)
        {
            return Add<T>(propertyLambda);
        }

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

        public static AbstractCriterion IsNotNull<T>(Expression<Func<T, object>> propertyLambda)
        {
            ResolveLambda r = new ResolveLambda();
            r.Resolve<T>(propertyLambda);
            if (r.OperationTypeSpecified)
            {
                throw new NotSupportedException("Only a property name is required");
            }
            return Restrictions.IsNotNull(r.PropertyName);
        }

        public static AbstractCriterion IsNull<T>(Expression<Func<T, object>> propertyLambda)
        {
            ResolveLambda r = new ResolveLambda();
            r.Resolve<T>(propertyLambda);
            if (r.OperationTypeSpecified)
            {
                throw new NotSupportedException("Only a property name is required");
            }
            return Restrictions.IsNull(r.PropertyName);
        }

        public static AbstractCriterion Like<T>(Expression<Func<T, object>> propertyLambda)
        {
            ResolveLambda r = new ResolveLambda();
            r.Resolve<T>(propertyLambda);
            AbstractCriterion retval;

            ExpressionType expType = r.OperationType;
            switch (expType)
            {
                case ExpressionType.NotEqual:
                    retval = Restrictions.Not(Restrictions.Like(r.PropertyName, r.Value));
                    break;
                case ExpressionType.Equal:
                    retval = Restrictions.Like(r.PropertyName, r.Value);
                    break;
                default:
                    throw new NotSupportedException(string.Format("{0} operation not supported in like", expType));
            }

            return retval;
        }

        /// <summary>
        /// Handles "Equals", "Greater Than", "Less Than", "Greater Than or Equal", "Less Than or Equal",
        /// "NotEqual", "NotNull" and "Between"
        /// </summary>
        /// <param name="propertyLambda">lambda function to evaluate: ()=> obj.A == 1</param>
        /// <returns>NHibernate SimpleExpression</returns>
        public static AbstractCriterion Add<T>(Expression<Func<T, object>> propertyLambda)
        {
            AbstractCriterion retval;
            ResolveLambda r = new ResolveLambda();
            r.Resolve<T>(propertyLambda);

            ExpressionType outerType = r.OperationType;
            object val1;
            switch (outerType)
            {
                case ExpressionType.NotEqual:
                    val1 = r.Value;
                    if (val1 == null)
                        retval = Restrictions.IsNull(r.PropertyName);
                    else
                        retval = Restrictions.Eq(r.PropertyName, val1);
                    retval = Restrictions.Not(retval);
                    break;
                case ExpressionType.Equal:
                    val1 = r.Value;
                    if(val1 == null)
                        retval = Restrictions.IsNull(r.PropertyName);
                    else
                        retval = Restrictions.Eq(r.PropertyName, val1);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    retval = Restrictions.Ge(r.PropertyName, r.Value);
                    break;
                case ExpressionType.GreaterThan:
                    retval = Restrictions.Gt(r.PropertyName, r.Value);
                    break;
                case ExpressionType.LessThanOrEqual:
                    retval = Restrictions.Le(r.PropertyName, r.Value);
                    break;
                case ExpressionType.LessThan:
                    retval = Restrictions.Lt(r.PropertyName, r.Value);
                    break;
                case ExpressionType.AndAlso: //Between
                    ExpressionType innerType1 = r.OperationType;
                    ExpressionType innerType2 = r.OperationType;
                    if (innerType1 == ExpressionType.GreaterThan && innerType2 == ExpressionType.LessThan)
                        retval = Restrictions.Between(r.PropertyName, r.Value, r.Value);
                    else if (innerType1 == ExpressionType.LessThan && innerType2 == ExpressionType.GreaterThan)
                    {
                        val1 = r.Value;
                        retval = Restrictions.Between(r.PropertyName, r.Value, val1);
                    }
                    else
                        throw new NotSupportedException("Only Between expressions can be used in this context");
                    break;
                default:
                    throw new NotSupportedException("Only ==,<,>,>=,<=,&& operators supported in this function");
            }
            return retval;
        }
#endif
    }

#if DOT_NET_35
    /// <summary>
    /// A set of extensions methods for ICriteria to remove the need for .Add()
    /// </summary>
    public static class RestrictByExtensions
    {
        public static CriteriaExpression<T> CreateExpression<T>(this NHibernate.ISession session)
        {
            return new CriteriaExpression<T>(session.CreateCriteria(typeof(T)));
        }

        public static CriteriaExpression<T> Expression<T>(this NHibernate.ICriteria c)
        {
            return new CriteriaExpression<T>(c);
        }
    }

    public class CriteriaExpression<T>
    {
        NHibernate.ICriteria inner = null;
        internal CriteriaExpression(NHibernate.ICriteria wrap)
        {
            Criteria = wrap;
        }

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
        public CriteriaExpression<T> Eq(Expression<Func<T, object>> propertyLambda)
        {
            Criteria.Add(RestrictBy.Eq(propertyLambda));
            return this;
        }
        public CriteriaExpression<T> IsEmpty(Expression<Func<T, object>> propertyLambda)
        {
            Criteria.Add(RestrictBy.IsEmpty(propertyLambda));
            return this;
        }
        public CriteriaExpression<T> IsNotEmpty(Expression<Func<T, object>> propertyLambda)
        {
            Criteria.Add(RestrictBy.IsNotEmpty(propertyLambda));
            return this;
        }
        public CriteriaExpression<T> IsNotNull(Expression<Func<T, object>> propertyLambda)
        {
            Criteria.Add(RestrictBy.IsNotNull(propertyLambda));
            return this;
        }
        public CriteriaExpression<T> IsNull(Expression<Func<T, object>> propertyLambda)
        {
            Criteria.Add(RestrictBy.IsNull(propertyLambda));
            return this;
        }
        public CriteriaExpression<T> Like(Expression<Func<T, object>> propertyLambda)
        {
            Criteria.Add(RestrictBy.Like(propertyLambda));
            return this;
        }
        /// <summary>
        /// Handles "Equals", "Greater Than", "Less Than", "Greater Than or Equal", "Less Than or Equal",
        /// "NotEqual", "NotNull" and "Between"
        /// </summary>
        /// <param name="propertyLambda">lambda function to evaluate: ()=> obj.A == 1</param>
        /// <returns>NHibernate SimpleExpression</returns>
        public CriteriaExpression<T> Add(Expression<Func<T, object>> propertyLambda)
        {
            Criteria.Add(RestrictBy.Add(propertyLambda));
            return this;
        }
        public CriteriaExpression<T> Add(ICriterion c)
        {
            Criteria.Add(c);
            return this;
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
        private Type evaluatedType;
        private string _propertyName = "";
        internal Type PropertyType;
        private Queue<object> _values = new Queue<object>();
        private Queue<ExpressionType> _operationType = new Queue<ExpressionType>();
        internal bool OperationTypeSpecified = false;

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                if (!string.IsNullOrEmpty(_propertyName))
                {
                    if (_propertyName != value)
                        throw new NotSupportedException("Unable to evaluate multiple properties in the one expression.");
                }
                _propertyName = value;
            }
        }
        public object Value
        {
            get
            {
                return _values.Dequeue();
            }
            set
            {
                _values.Enqueue(value);
            }
        }
        public ExpressionType OperationType
        {
            get
            {
                return _operationType.Dequeue();
            }
            set
            {
                OperationTypeSpecified = true;
                _operationType.Enqueue(value);
            }
        }

        internal ResolveLambda(Expression<Func<object>> expression)
        {
            Visit(expression);
        }

        internal ResolveLambda() { }

        internal void Resolve<T>(Expression<Func<T, object>> expression)
        {
            evaluatedType = typeof(T);
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
                case ExpressionType.Lambda:
                    Visit((LambdaExpression)expression);
                    break;
                default:
                    throw new NotSupportedException(string.Format("lambda expression with type {0} not supported", expression.NodeType));

            }
        }
        internal void Visit(System.Linq.Expressions.UnaryExpression expression)
        {
            Visit(expression.Operand);
        }
        internal void Visit(System.Linq.Expressions.LambdaExpression expression)
        {
            Visit(expression.Body);
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
            if (evaluatedType != null && expression.Member.DeclaringType == evaluatedType)
            { //this is the property name we are evaluating
                PropertyName = expression.Member.Name;
                PropertyType = ((System.Reflection.PropertyInfo)expression.Member).PropertyType;
            }
            else if (expression.Expression is ConstantExpression)
            { //treat this as a variable to compare against
                if (((ConstantExpression)expression.Expression).Value.GetType().IsNested)
                    Value = ((ConstantExpression)expression.Expression).Value.GetType().GetField(expression.Member.Name).GetValue(((ConstantExpression)expression.Expression).Value);
                else
                    Value = ((ConstantExpression)expression.Expression).Value;
            }
            else
            {
                throw new NotSupportedException(string.Format("Unsure how to evaluate {0}", expression));
            }
        }
    }

#endif
    #endregion
}
