using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;

namespace System.BusinessObjects.MethodLinq
{
    /// <summary>
    /// Provides an IQueryable interface to build method Queriest
    /// </summary>
    /// <remarks>
    /// Based on code from: http://blogs.msdn.com/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
    /// </remarks>
    public class MethodLinqQuery<T>: IQueryable<T>, IQueryable, IEnumerable<T>
    {
        MethodQueryProvider provider;
        Expression expression;

        public MethodLinqQuery(MethodQueryProvider provider) {
            if (provider == null) {
                throw new ArgumentNullException("provider");
            }
            this.provider = provider;
            this.expression = Expression.Constant(this);
        }

        public MethodLinqQuery(MethodQueryProvider provider, Expression expression)
        {
            if (provider == null) {
                throw new ArgumentNullException("provider");
            }
            if (expression == null) {
                throw new ArgumentNullException("expression");
            }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type)) {
                throw new ArgumentOutOfRangeException("expression");
            }
            this.provider = provider; 
            this.expression = expression;
        }

        Expression IQueryable.Expression {
            get { return this.expression; }
        }

        Type IQueryable.ElementType {
            get { return typeof(T); }
        }

        IQueryProvider IQueryable.Provider {
            get { return this.provider; }
        }

        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>)this.provider.Execute(this.expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)this.provider.Execute(this.expression)).GetEnumerator();
        }

        public override string ToString() {
            return this.provider.GetQueryText(this.expression);
        }
    }
}
