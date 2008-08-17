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
    /// <summary>
    /// A set of extensions methods for ICriteria to remove the need for .Add()
    /// </summary>
    public static class RestrictByExtensions
    {
        /// <summary>
        /// Creates a wrapper class that can use lambda expressions to add Restrictions
        /// </summary>
        public static CriteriaExpression<T> CreateExpression<T>(this NHibernate.ISession session)
        {
            return new CriteriaExpression<T>(session.CreateCriteria(typeof(T)));
        }

        /// <summary>
        /// Creates a wrapper class that can use lambda expressions to add Restrictions
        /// </summary>
        public static CriteriaExpression<T> Expression<T>(this NHibernate.ICriteria c)
        {
            return new CriteriaExpression<T>(c);
        }

        /// <summary>
        /// Adds a single Restriction to the current criteria using a lambda expression
        /// </summary>
        public static NHibernate.ICriteria Expression<T>(this NHibernate.ICriteria c, Expression<Func<T, object>> propertyLambda)
        {
            return new CriteriaExpression<T>(c).Criteria;
        }
    }
#endif
}
