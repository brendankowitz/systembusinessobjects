using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using NHibernate.Mapping;

namespace System.BusinessObjects.With
{
    /// <summary>
    /// The delegate signature accepted by Each.Item
    /// </summary>
    public delegate void EachItemDelegate<T>(T item);

    /// <summary>
    /// Encapsulates the Each.Item operation
    /// </summary>
    public class EachIterator
    {
        private ICollection iternalCollection;
        internal EachIterator(ICollection collection)
        {
            iternalCollection = collection;
        }

        /// <summary>
        /// The operation to perform on each collection item
        /// </summary>
        public void Item<T>(EachItemDelegate<T> currentItem)
        {
            foreach (T item in iternalCollection)
            {
                currentItem(item);
            }
        }
    }

    public partial class With
    {
        /// <summary>
        /// Performs a Ruby-style Collection.Each operation
        /// A more efficient method is to use the 'foreach' statement, however
        /// under some circumstances this may be cleaner. ie. A function that matches the
        /// delegate signature is able to be passed in.
        /// </summary>
        public static EachIterator Each(ICollection collection)
        {
            return new EachIterator(collection);
        }

        
    }
}
