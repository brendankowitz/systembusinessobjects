using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using System.Collections;

namespace System.BusinessObjects.Cache
{
    /// <summary>
    /// An abstract provider for access to a cache
    /// </summary>
    public abstract class CacheBase
    {
        int _timeout = 60;
        /// <summary>
        /// Returns the default cache timeout
        /// </summary>
        public int DefaultTimeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        bool _usecache = true;
        /// <summary>
        /// Returns true if the cache is being used
        /// </summary>
        public bool UseCache
        {
            get
            {
                return _usecache;
            }
        }

        public CacheBase(int defaultCacheTimeout, bool useCache)
        {
            _timeout = defaultCacheTimeout;
            _usecache = useCache;
        }

        public CacheBase(string name, System.Collections.Specialized.NameValueCollection config)
        {
            //DefaultCacheTimeout config
            object retval;
            retval = (config["DefaultCacheTimeout"]);
            if (retval == null)
            {
                //default value
                retval = 120;
            }
            _timeout = Convert.ToInt32(retval);

            //UseCache config
            retval = (config["UseCache"]) as string;
            if (retval == null)
            {
                //default value
                _usecache = true;

            }
            try
            {
                _usecache = bool.Parse(retval as string);
            }
            catch
            {
                _usecache = true;
            }
        }

        /// <summary>
        /// Create a unique string to use as the key for a cached value
        /// </summary>
        public static string CreateCacheString(string name, params object[] args)
        {
            string retval = name;

            foreach (object obj in args)
            {
                if (obj != null)
                {
                    if (obj.GetType().IsArray)
                    {
                        foreach (object o in (IEnumerable)obj)
                        {
                            retval = string.Format("{0}_{1}", retval, o.ToString());
                        }
                    }
                    else
                    retval = string.Format("{0}_{1}", retval, obj.ToString());
                }
                else
                {
                    retval = string.Format("{0}_null", retval);
                }

            }

            return retval;
        }

        /// <summary>
        /// Gets a cached value
        /// </summary>
        public abstract T GetCache<T>(string CacheKey);
        /// <summary>
        /// Gets a cached value
        /// </summary>
        public abstract object GetCache(string CacheKey);
        /// <summary>
        /// Sets a cache value
        /// </summary>
        public abstract void SetCache(string CacheKey, object objObject);
        /// <summary>
        /// Sets a cache value with storage priority
        /// </summary>
        public abstract void SetCache(string CacheKey, object objObject, System.Web.Caching.CacheItemPriority Priority);
        /// <summary>
        /// Removes a value from the cache
        /// </summary>
        public abstract void RemoveCache(string CacheKey);
    }
}
