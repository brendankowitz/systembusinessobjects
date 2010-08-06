using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.BusinessObjects.Providers;

namespace System.BusinessObjects.Cache
{
    /// <summary>
    /// A simple cache implementation that stores items in a Hashtable,
    /// Item's do not automatically expire, however expired items will be removed when
    /// access it attempted.
    /// </summary>
    /// <remarks>This class would typically be used in a testing environment or
    /// when no other cache is available see <seealso cref="System.BusinessObjects.Data.AspNetDataCache">AspNetDataCache</seealso>
    /// </remarks>
    public class HashtableDataCache : CacheBase
    {
        static Hashtable _data = new Hashtable();

         public HashtableDataCache(int defaultCacheTimeout, bool useCache)
             : base(defaultCacheTimeout, useCache)
        {}

         public HashtableDataCache(string name, System.Collections.Specialized.NameValueCollection config)
            : base(name, config)
        {}

        public int ItemCount
        {
            get
            {
                return _data.Count;
            }
        }

        class HashItem
        {
            public object val;
            public DateTime expire;
        }

        public override T GetCache<T>(string CacheKey)
        {
            HashItem item = _data[CacheKey] as HashItem;
            if (item != null && item.expire <= DateTime.Now)
            {
                _data.Remove(CacheKey);
                item = null;
            }
            return (T)(item == null ? default(T) : item.val);
        }

        public override object GetCache(string CacheKey)
        {
            HashItem item = _data[CacheKey] as HashItem;
            if (item != null && item.expire <= DateTime.Now)
            {
                _data.Remove(CacheKey);
                item = null;
            }
            return (item == null ? null : item.val);
        }

        public override void SetCache(string CacheKey, object objObject)
        {
            HashItem item = new HashItem();
            item.val = objObject;
            item.expire = DateTime.Now.AddSeconds(DefaultTimeout);
            _data[CacheKey] = item;
        }

        public override void SetCache(string CacheKey, object objObject, System.Web.Caching.CacheItemPriority Priority)
        {
            HashItem item = new HashItem();
            item.val = objObject;
            item.expire = DateTime.Now.AddSeconds(DefaultTimeout);
            _data[CacheKey] = item;
        }

        public override void RemoveCache(string CacheKey)
        {
            if (_data.Contains(CacheKey))
                _data.Remove(CacheKey);
        }

        /// <summary>
        /// Removes all items from the cache
        /// </summary>
        public void Flush()
        {
            _data.Clear();
        }

        /// <summary>
        /// Removes any expired items from the cache
        /// </summary>
        public void RemoveOldItems()
        {
            HashItem item;
            List<string> removeItems = new List<string>();
            foreach (string str in _data.Keys)
            {
                item = _data[str] as HashItem;
                if (item != null && item.expire <= DateTime.Now)
                {
                    removeItems.Add(str);
                }
            }
            foreach (string str in removeItems)
                _data.Remove(str);
        }
    }
}
