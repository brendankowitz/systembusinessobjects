using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.BusinessObjects.Data;

#if DOT_NET_35
using System.Linq;
using System.Linq.Expressions;
#endif

namespace System.BusinessObjects.Helpers
{
    public class Property
    {
        public static PropertyInfo GetPropertyInfo(string name, DataObject obj)
        {
            return obj.GetType().GetProperty(name);
        }

#if DOT_NET_35
        /// <summary>
        /// Returns the string name of a property, this allows type / member safety
        /// </summary>
        /// <param name="propertyNameLambda">() => obj.Name</param>
        /// <returns>Name</returns>
        /// <remarks>
        /// From: http://www.paulstovell.com/blog/strongly-typed-property-names
        /// </remarks>
        public static string GetFor(Expression<Func<object>> propertyNameLambda)
        {
            MemberExpression member = propertyNameLambda.Body as MemberExpression;
            if (member != null)
            {
                return member.Member.Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns the string name of a property, this allows type / member safety
        /// </summary>
        /// <typeparam name="T">Type of objects whos properties to evaluate</typeparam>
        /// <param name="propertyNameLambda">obj => obj.Name</param>
        /// <returns>Name</returns>
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

        /// <summary>
        /// Returns the string name of a property, this allows type / member safety
        /// </summary>
        /// <remarks> 
        /// Adding the TRetVal parameter allows more efficient Lambda expressions (doesn't create the Convert(p => p.ID) function)
        /// also helps in Generic Type inference
        /// </remarks>
        public static string For<T, TRetVal>(Expression<Func<T, TRetVal>> propertyNameLambda)
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
#endif
    }
}
