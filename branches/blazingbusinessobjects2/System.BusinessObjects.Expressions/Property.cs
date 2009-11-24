using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace System.BusinessObjects.Expressions
{
    public static class Property
    {
        public static string For<T>(Expression<Func<T, object>> propertyNameLambda)
        {
            MemberExpression member;
            if (propertyNameLambda.Body is UnaryExpression)
                member = ((UnaryExpression)propertyNameLambda.Body).Operand as MemberExpression;
            else
                member = propertyNameLambda.Body as MemberExpression;
            if (member != null)
            {
                return member.Member.Name;
            }
            else
            {
                throw new ArgumentException("Could not determine property name.", "propertyNameLambda");
            }
        }
    }
}
