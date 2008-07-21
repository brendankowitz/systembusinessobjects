using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.BusinessObjects.Data;

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
        public static string GetFor(System.Linq.Expressions.Expression<Func<object>> propertyNameLambda)
        {
            System.Linq.Expressions.MemberExpression member = propertyNameLambda.Body as System.Linq.Expressions.MemberExpression;
            if (member != null)
            {
                return member.Member.Name;
            }
            return string.Empty;
        }
#endif
    }
}
