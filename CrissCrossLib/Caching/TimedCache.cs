using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace CrissCrossLib.Caching
{
    /// <summary>
    /// A Timed Cache that expires items a specific amount of time after they are first added
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // Internally, this uses HttpRuntime.Cache.
    // CrissCrossLib will typically be in an ASP.NET environment so thats OK.
    // HttpRuntime.Cache can also be safely used outside of ASP.NET, according to Scott Hanselman
    // (see http://www.hanselman.com/blog/UsingTheASPNETCacheOutsideOfASPNET.aspx )
    // In .NET 4, there may be better options....
    public class TimedCache<T> where T:class
    {
        private static Random ms_random = new Random();

        private string m_uniqueKeyPrefix;
        private int m_minMinutes;
        private int m_maxMinutes;

        /// <summary>
        /// Creates a new TimedCache.
        /// Each TimedCache needs a unique uniqueKeyPrefix.
        /// Items will be cached for a random amount of minutes between minMinutes and maxMinutes.
        /// </summary>
        /// <param name="uniqueKeyPrefix">A unique key prefix for this cache only</param>
        /// <param name="minMinutes">Minimum number of minutes to cache items</param>
        /// <param name="maxMinutes">Maximum number of minutes to cache items</param>
        public TimedCache(string uniqueKeyPrefix, int minMinutes, int maxMinutes)
        {
            m_uniqueKeyPrefix = uniqueKeyPrefix;
            m_minMinutes = minMinutes;
            m_maxMinutes = maxMinutes;
        }

        public void Add(string itemKey, T item)
        {
            TimeSpan randomExpiry = MakeExpirationTimespan();
            HttpRuntime.Cache.Add(InternalKey(itemKey), item, null, Cache.NoAbsoluteExpiration, randomExpiry, CacheItemPriority.Normal, null);
        }

        public void Remove(string itemKey)
        {
            HttpRuntime.Cache.Remove(InternalKey(itemKey));
        }

        private TimeSpan MakeExpirationTimespan()
        {
            int minutes = m_minMinutes;
            if (m_minMinutes != m_maxMinutes)
                minutes = ms_random.Next(m_minMinutes, m_maxMinutes);
            TimeSpan randomExpiry = TimeSpan.FromMinutes(minutes);
            return randomExpiry;
        }

        /// <summary>
        /// Returns (or sets) the specified item from the cache.
        /// Null will be returned if an item is not found
        /// When settings, items will be updated or added as appropriate
        /// </summary>
        /// <param name="itemKey"></param>
        /// <returns></returns>
        public T this[string itemKey]
        {
            get
            {
                object possible = HttpRuntime.Cache[InternalKey(itemKey)];
                return possible as T;
                
            }
            set
            {
                TimeSpan randomExpiry = MakeExpirationTimespan();
                HttpRuntime.Cache.Insert(InternalKey(itemKey), value, null, Cache.NoAbsoluteExpiration, randomExpiry);
            }
        }

        public bool ContainsKey(string itemKey)
        {
            return (this[itemKey] != null);
        }

        private string InternalKey(string itemKey)
        {
            return string.Format("{0}_{1}", m_uniqueKeyPrefix, itemKey);
        }

    }
}
