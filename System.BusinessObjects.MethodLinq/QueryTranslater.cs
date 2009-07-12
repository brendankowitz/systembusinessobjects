using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace System.BusinessObjects.MethodLinq
{
    /// <summary>
    /// A Linq translator that parses a query into method arguments
    /// </summary>
    /// <remarks>
    /// Based on code from http://blogs.msdn.com/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
    /// </remarks>
    internal class QueryTranslator : ExpressionVisitor
    {
        StringBuilder queryTextBuilder;
        internal MethodInfo targetMethod;
        internal List<MethodParamValue> parsedMethodParameters = new List<MethodParamValue>();
        internal LambdaExpression selectMethod = null;
        Type ownerClass = null;

        internal QueryTranslator(Type ownerClassType)
        {
            ownerClass = ownerClassType;
        }

        internal string Translate(Expression expression)
        {
            this.queryTextBuilder = new StringBuilder();
            this.Visit(expression);
            return this.queryTextBuilder.ToString();
        }

        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
                return exp;

            switch (exp.NodeType)
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
                    if (exp is BinaryExpression)
                        return this.VisitBinary((BinaryExpression)exp);
                    if (exp is UnaryExpression)
                        return this.VisitUnary((UnaryExpression)exp);
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                this.Visit(m.Arguments[0]);
                queryTextBuilder.Append(" WHERE ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                return m;
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Single")
            {
                this.Visit(m.Arguments[0]);
                return m;
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Select")
            {
                this.Visit(m.Arguments[0]);
                queryTextBuilder.Append(" SELECT ");
                selectMethod = (LambdaExpression)StripQuotes(m.Arguments[1]);
                queryTextBuilder.Append(selectMethod.ToString());
                return m;
            }
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    queryTextBuilder.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    queryTextBuilder.Append(" CONVERT ");
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (targetMethod != null)
            {
                Expression leftEx = b.Left;

                switch (b.NodeType)
                {
                    case ExpressionType.Equal: case ExpressionType.AndAlso:

                        if (b.Left is UnaryExpression)
                            leftEx = ((UnaryExpression)b.Left).Operand;

                        if (!(leftEx is MemberExpression))
                        {
                            Visit(b.Left);
                            queryTextBuilder.AppendFormat("{0} ", b.NodeType);
                            Visit(b.Right);
                        }
                        else
                        {
                            GetParameterFromExpression(b, leftEx);
                        }

                        break;
                    default:
                        throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
                }

                return b;
            }

            throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
        }

        /// <summary>
        /// Searches for QueryableMethodParameterAttribute up a property/class path 
        /// eg. MyObject.ObjectTwo.PropertyId = 123
        /// </summary>
        private QueryableMethodParameterAttribute SearchExpressionPathForAttributes(Expression exp, List<MemberInfo> path)
        {
            object[] attList = ((MemberExpression)exp).Member.GetCustomAttributes(typeof(QueryableMethodParameterAttribute), true);
            QueryableMethodParameterAttribute attr = null;

            if (attList != null && attList.Length > 0)
            {
                attr = attList[0] as QueryableMethodParameterAttribute;
                return attr;
            }
            else
            {
                if (exp is MemberExpression)
                {
                    MemberExpression mem = (MemberExpression)exp;
                    if (mem.Expression is MemberExpression)
                    {
                        path.Add(mem.Member);
                        return SearchExpressionPathForAttributes(mem.Expression, path);
                    }
                }
            }

            return null;
        }

        private void GetParameterFromExpression(BinaryExpression b, Expression leftEx)
        {
            MethodParamValue val = new MethodParamValue();
            QueryableMethodParameterAttribute attr = SearchExpressionPathForAttributes(leftEx, val.ParamPath);

            if (attr != null)
            {
                val.Value = StripQuotes(b.Right);
                val.Param = targetMethod.GetParameters().Single(info => info.Name == attr.ParameterName);
                if (b.Right is MemberExpression)
                    val.QueryVariableName = ((MemberExpression)b.Right).Member.Name;
                else if (b.Right is ConstantExpression || b.Right is MethodCallExpression)
                { /* resolved later... */ }
                else
                    throw new NotSupportedException(string.Format("Currently values are only able to be resolved from Members and Constants. Error in {0}", b.Right.ToString()));

                if (val.Param != null)
                    parsedMethodParameters.Add(val);

                queryTextBuilder.Append(attr.ParameterName);
                if (val.ParamPath.Count > 0)
                {
                    for(int i=val.ParamPath.Count-1;i>=0;i--)
                        queryTextBuilder.AppendFormat(".{0}", val.ParamPath[i].Name);
                }
                queryTextBuilder.Append(" = ");
                if (!string.IsNullOrEmpty(val.QueryVariableName))
                    queryTextBuilder.Append(val.QueryVariableName);
                else
                    queryTextBuilder.Append(b.Right.ToString());
                queryTextBuilder.Append(" ");
            }

            else
            {
                throw new NotImplementedException(string.Format("VisitBinary '{0}' operation not supported on member '{1}'", b.NodeType.ToString(), ((MemberExpression)b.Left).Member.ToString()));
            }
        }
        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;
            if (q != null)
            {
                // assume constant nodes w/ IQueryables are Class references
                queryTextBuilder.AppendFormat("QUERY {0} ", ownerClass.Name);
    
                object[] attList = q.ElementType.GetCustomAttributes(typeof(QueryableMethodAttribute), true);
                QueryableMethodAttribute attr = null;

                if (attList != null && attList.Length > 0)
                {
                    attr = attList[0] as QueryableMethodAttribute;
                }

                targetMethod = ownerClass.GetMethod(attr.MethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, attr.Types, null);
                queryTextBuilder.AppendFormat("EXECUTE {0}(", targetMethod.Name);
                bool delimiter = false;
                foreach(ParameterInfo info in targetMethod.GetParameters())
                {
                    queryTextBuilder.AppendFormat("{1}{0}", info.Name, delimiter ? ", " : "");
                    delimiter = true;
                }
                queryTextBuilder.Append(")");

            }
            else
            {
                throw new NotImplementedException(string.Format("NotImplementedException: The constant for '{0}' is not supported", c.Value));
            }

            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                queryTextBuilder.Append(m.Member.Name);
                return m;
            }
            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        static internal object ResolveToValue(Expression expression, string fieldName)
        {
            object inval = expression.ToString();

            if (expression is MemberExpression && ((MemberExpression)expression).Expression is ConstantExpression)
            {
                MemberExpression memberExp = (MemberExpression)expression;
                if (memberExp.Member.MemberType == System.Reflection.MemberTypes.Property)
                    inval = ((ConstantExpression)memberExp.Expression).Value.GetType().GetProperty(memberExp.Member.Name).GetValue(((ConstantExpression)memberExp.Expression).Value, null);
                if (memberExp.Member.MemberType == System.Reflection.MemberTypes.Field)
                    inval = ((ConstantExpression)memberExp.Expression).Value.GetType().GetField(memberExp.Member.Name).GetValue(((ConstantExpression)memberExp.Expression).Value);
                else
                    inval = ((ConstantExpression)memberExp.Expression).Value;

            }
            else if (expression is ConstantExpression)
            {
                inval = ((ConstantExpression)expression).Value;
            }
            else if (expression is MemberExpression && ((MemberExpression)expression).Expression != null)
            {
                inval = ResolveToValue(((MemberExpression)expression).Expression, fieldName);
            }
            else if (expression is MemberExpression && expression.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression memberExp = (MemberExpression)expression;
                if (memberExp.Member.MemberType == MemberTypes.Property)
                    inval = memberExp.Type.GetProperty(memberExp.Member.Name).GetValue(null, null);
                else if (memberExp.Member.MemberType == MemberTypes.Method)
                {
                    MethodInfo meth = memberExp.Type.GetMethod(memberExp.Member.Name, BindingFlags.Static | BindingFlags.Public);
                    inval = meth.Invoke(null, null);
                }
            }

            return inval;
        }

        static internal object ResolveToValue(MethodCallExpression expression)
        {
            object obj = ResolveToValue(expression.Object, null);
            List<object> args = new List<object>();
            foreach (Expression a in expression.Arguments)
            {
                args.Add(ResolveToValue(a, null)); 
            }
            return expression.Method.Invoke(obj, args.ToArray());
        }

        static internal object GetValue(MemberInfo method, object instance)
        {
            object retval = null;
            if (method.MemberType == System.Reflection.MemberTypes.Property)
                retval = ((PropertyInfo)method).GetValue(instance, null);
            else if (method.MemberType == System.Reflection.MemberTypes.Field)
                retval = ((FieldInfo)method).GetValue(instance);
            else
                throw new NotSupportedException(string.Format("Unable to get value from {0} of type {1}", method.Name, method.MemberType));
            return retval;
        }

        static internal object SetValue(MemberInfo method, object instance, object newVal)
        {
            object retval = null;
            if (method.MemberType == System.Reflection.MemberTypes.Property)
                ((PropertyInfo)method).SetValue(instance, newVal, null);
            else if (method.MemberType == System.Reflection.MemberTypes.Field)
                ((FieldInfo)method).SetValue(instance, newVal);
            else
                throw new NotSupportedException(string.Format("Unable to Set value on {0} of type {1}", method.Name, method.MemberType));
            return retval;
        }
    }
}
