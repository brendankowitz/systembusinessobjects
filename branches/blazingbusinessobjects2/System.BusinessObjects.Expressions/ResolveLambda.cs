using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.BusinessObjects.Expressions
{
    #region Resolve Lambda Helper
    /// <summary>
    /// Helper class to break down and resolve lambda values
    /// </summary>
    [System.Runtime.InteropServices.GuidAttribute("F11940FA-4293-4B48-9F61-97BCB3B2A4CF")]
    internal class ResolveLambda
    {
        private Type evaluatedType;
        private string _propertyName = "";
        internal Type PropertyType;
        private Queue<object> _values = new Queue<object>();
        private Queue<NHExpressionType> _operationType = new Queue<NHExpressionType>();
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
        public NHExpressionType OperationType
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

        internal ResolveLambda() { }

        internal void Resolve<T>(Expression<Func<T, object>> expression)
        {
            evaluatedType = typeof(T);
            Visit(expression);
        }

        internal void Resolve<T>(Expression<Func<T, bool>> expression)
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
                case ExpressionType.Call:
                    Visit((MethodCallExpression)expression);
                    break;
                default:
                    throw new NotSupportedException(string.Format("lambda expression with type {0} not supported", expression.NodeType));

            }
        }
        internal void Visit(System.Linq.Expressions.MethodCallExpression expression)
        {
            if (expression.Method.Name == "Contains")
            {
                OperationType = NHExpressionType.Like;
                Visit(expression.Arguments.First());
                Visit(expression.Object);
            }
            else
            {
                throw new NotSupportedException(string.Format("MethodCall {0} is not supported", expression.Method.Name));
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
            OperationType = ToExpressionType(expression.NodeType);

            Visit(expression.Left);
            Visit(expression.Right);
        }
        internal void Visit(System.Linq.Expressions.ConstantExpression expression)
        {
            Value = expression.Value;
        }
        internal void Visit(System.Linq.Expressions.MemberExpression expression)
        {
            if (evaluatedType != null && expression.Member.DeclaringType == evaluatedType && expression.Expression.NodeType == ExpressionType.Parameter)
            { //this is the property name from a parameter that we are evaluating
                PropertyName = expression.Member.Name;
                PropertyType = ((System.Reflection.PropertyInfo)expression.Member).PropertyType;
            }
            else if (expression.Expression is ConstantExpression)
            { //treat this as a variable to compare against
                if (expression.Member.MemberType == System.Reflection.MemberTypes.Property)
                    Value = ((ConstantExpression)expression.Expression).Value.GetType().GetProperty(expression.Member.Name).GetValue(((ConstantExpression)expression.Expression).Value, null);
                if (expression.Member.MemberType == System.Reflection.MemberTypes.Field)
                    Value = ((ConstantExpression)expression.Expression).Value.GetType().GetField(expression.Member.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(((ConstantExpression)expression.Expression).Value);
                else
                    Value = ((ConstantExpression)expression.Expression).Value;
            }
            else
            {
                throw new NotSupportedException(string.Format("Unsure how to evaluate {0}", expression));
            }
        }

        NHExpressionType ToExpressionType(ExpressionType t)
        {
            return (NHExpressionType)Enum.Parse(typeof(NHExpressionType), t.ToString());
        }
    }

    

    #endregion
}
