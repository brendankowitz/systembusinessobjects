using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.Configuration;
using System.Web;
using System.Collections.Generic;
using System.BusinessObjects.Providers;


namespace System.BusinessObjects.Cache
{
	/// <summary>
	/// Provides a controlled access to the http cache
	/// </summary>
	public class AspNetDataCache : CacheBase
	{
        public AspNetDataCache()
		{

		}

		public override T GetCache<T>(string CacheKey)
		{

            System.Web.Caching.Cache cache = HttpRuntime.Cache;
            return (T)cache.Get(CacheKey);
		}

        public override object GetCache(string CacheKey)
        {

            System.Web.Caching.Cache cache = HttpRuntime.Cache;
            return cache.Get(CacheKey);
        }

        public override void SetCache(string CacheKey, object objObject)
		{
            if (UseCache)
            {
                System.Web.Caching.Cache cache = HttpRuntime.Cache;
                cache.Remove(CacheKey);
                cache.Add(CacheKey, objObject, null, DateTime.MaxValue, TimeSpan.FromSeconds(DefaultTimeout), System.Web.Caching.CacheItemPriority.Default, null);
            }
		}

        public override void SetCache(string CacheKey, object objObject, System.Web.Caching.CacheItemPriority Priority)
		{
            if (UseCache)
            {
                System.Web.Caching.Cache cache = HttpRuntime.Cache;
                cache.Remove(CacheKey);
                cache.Add(CacheKey, objObject, null, DateTime.MaxValue, TimeSpan.FromSeconds(DefaultTimeout), Priority, null);
            }
		}

		public override void RemoveCache(string CacheKey)
		{
            System.Web.Caching.Cache cache = HttpRuntime.Cache;
            cache.Remove(CacheKey);
		}

	}
}
