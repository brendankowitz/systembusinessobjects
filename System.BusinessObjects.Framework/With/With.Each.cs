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
    public delegate void EachItemFunc<T>(T item);

    /// <summary>
    /// The delegate signature accepted by Each.Item
    /// </summary>
    public delegate ROutput EachItemFunc<TInput, ROutput>(TInput item);

    /// <summary>
    /// Encapsulates the Each.Item operation
    /// </summary>
    public class EachIterator
    {
        private IEnumerable internalCollection;
        internal EachIterator(IEnumerable collection)
        {
            if (collection != null)
                internalCollection = collection;
            else
                internalCollection = new ArrayList();
        }

        /// <summary>
        /// The operation to perform on each collection item
        /// </summary>
        public void Item<T>(EachItemFunc<T> currentItem)
        {
            foreach (T item in internalCollection)
            {
                currentItem(item);
            }
        }

        /// <summary>
        /// The operation to perform on each collection item
        /// </summary>
        public IEnumerable<ROutput> Item<TInput, ROutput>(EachItemFunc<TInput, ROutput> currentItem)
        {
            List<ROutput> outputlist = new List<ROutput>();
            foreach (TInput item in internalCollection)
            {
                ROutput output = currentItem(item);
                if (output != null)
                    outputlist.Add(output);
            }
            return outputlist;
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
        public static EachIterator Each(IEnumerable collection)
        {
            return new EachIterator(collection);
        }
    }
}
