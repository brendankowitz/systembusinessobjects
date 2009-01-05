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
    public delegate void EachItemFunc(object item);

    /// <summary>
    /// The delegate signature accepted by Each.Item
    /// </summary>
    public delegate object EachItemFuncOut(object item);

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
        /// This is used because NHibernate's ISet does not contain a Get() function...
        /// </summary>
        public T Select<T>(int itemIndex)
        {
            int counter = 0;
            foreach (T item in internalCollection)
            {
                if(counter == itemIndex)
                    return item;
                counter++;
            }
            return default(T);
        }

        /// <summary>
        /// Selects an item matching a predicate
        /// *Replace with Built-in extension method in 3.5
        /// </summary>
        public T Select<T>(Predicate<T> predicate)
        {
            foreach (T item in internalCollection)
            {
                if (predicate(item))
                    return item;
            }
            return default(T);
        }

        /// <summary>
        /// Splits an IList into an array of smaller ILists
        /// </summary>
        public IList<T>[] Split<T>(int segmentSize)
        {
            List<List<T>> list = new List<List<T>>();
            List<T> curGroup = new List<T>();

            foreach (T item in internalCollection)
            {
                if (curGroup.Count >= segmentSize)
                {
                    list.Add(curGroup);
                    curGroup = new List<T>();
                }
                curGroup.Add(item);
            }

            if (curGroup.Count > 0)
                list.Add(curGroup);

            return list.ToArray();
        }


        /// <summary>
        /// The operation to perform on each collection item
        /// </summary>
        public void Item(EachItemFunc currentItem)
        {
            foreach (object item in internalCollection)
            {
                currentItem(item);
            }
        }

        /// <summary>
        /// The operation to perform on each collection item
        /// </summary>
        public void Item<T>(Action<T> currentItem)
        {
            foreach (T item in internalCollection)
            {
                currentItem(item);
            }
        }

        /// <summary>
        /// The operation to perform on each collection item
        /// </summary>
        public IEnumerable<ROutput> Item<TInput, ROutput>(Converter<TInput, ROutput> currentItem)
        {
            List<ROutput> outputlist = new List<ROutput>();
            return Item(outputlist, currentItem);
        }

        /// <summary>
        /// The operation to perform on each collection item
        /// </summary>
        public ICollection<ROutput> Item<TInput, ROutput>(ICollection<ROutput> outputArray, Converter<TInput, ROutput> currentItem)
        {
            ICollection<ROutput> outputlist = outputArray;
            foreach (TInput item in internalCollection)
            {
                ROutput output = currentItem(item);
                if (output != null)
                    outputlist.Add(output);
            }
            return outputlist;
        }

        /// <summary>
        /// The operation to perform on each collection item
        /// </summary>
        public IList Item(IList outputArray, EachItemFuncOut currentItem)
        {
            IList outputlist = outputArray;
            foreach (object item in internalCollection)
            {
                object output = currentItem(item);
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
        /// 
        /// Note: Many of these methods are obselete under .net3.5.
        /// </summary>
        public static EachIterator Each(IEnumerable collection)
        {
            return new EachIterator(collection);
        }

        /// <summary>
        /// Performs a Ruby-style Collection.Each operation
        /// A more efficient method is to use the 'foreach' statement, however
        /// under some circumstances this may be cleaner. ie. A function that matches the
        /// delegate signature is able to be passed in.
        /// </summary>
        public static EachIterator Each<T>(IEnumerable<T> collection)
        {
            return new EachIterator(collection);
        }
    }


#if DOT_NET_35
    /// <summary>
    /// Provides .net 3.5 extensions for the With Class
    /// </summary>
    public static class WithExtension
    {
        /// <summary>
        /// Performs a Ruby-style Collection.Each operation
        /// A more efficient method is to use the 'foreach' statement, however
        /// under some circumstances this may be cleaner. ie. A function that matches the
        /// delegate signature is able to be passed in.
        /// </summary>
        public static EachIterator Each(this IEnumerable collection)
        {
            return new EachIterator(collection);
        }

        /// <summary>
        /// Performs a Ruby-style Collection.Each operation
        /// A more efficient method is to use the 'foreach' statement, however
        /// under some circumstances this may be cleaner. ie. A function that matches the
        /// delegate signature is able to be passed in.
        /// </summary>
        public static EachIterator Each<T>(IEnumerable<T> collection)
        {
            return new EachIterator(collection);
        }

        /// <summary>
        /// Performs a Ruby-style Collection.Each operation
        /// A more efficient method is to use the 'foreach' statement, however
        /// under some circumstances this may be cleaner. ie. A function that matches the
        /// delegate signature is able to be passed in.
        /// </summary>
        public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            new EachIterator(collection).Item<T>(action);
        }

        /// <summary>
        /// Selects an item matching a predicate
        /// *Replace with Built-in extension method in 3.5
        /// </summary>
        public static T SelectBy<T>(this IEnumerable<T> collection, Predicate<T> predicate)
        {
            
            return new EachIterator(collection).Select<T>(predicate);
        }
    }
#endif
}
