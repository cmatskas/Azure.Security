namespace Azure.Security
{
    using System;
    using System.Runtime.Caching;

    public class Cache
    {
        public static Cache Current => new Cache();

        // Cache
        private readonly MemoryCache _dataCache;
        public CacheItemPolicy CachePolicy { get; set; }

        public Cache()
        {
            // Initialise the cache provider and get the default cache container
            _dataCache = MemoryCache.Default;

            // Set up a default item policy
            CachePolicy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromHours(3)
            };
        }

        /// <summary>
        /// Add the specified object to the cache
        /// </summary>
        public void AddItem<T>(string key, T value)
        {
            // Add the item straight into the cache
            _dataCache.Set(key, value, CachePolicy);
        }

        /// <summary>
        /// Add the specified object to the cache for the specified amount of minutes
        /// </summary>
        public void AddItem<T>(string key, T value, int cacheMins)
        {
            // Create a cache policy
            var itemCachePolicy = new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheMins)
            };

            // Add the item straight into the cache
            _dataCache.Set(key, value, itemCachePolicy);
        }

        /// <summary>
        /// Get an item from the cache
        /// </summary>
        /// <returns>Returns null if item does not exist</returns>
        public T GetItem<T>(string key)
        {
            // Try to get the object from the cache
            var obj = _dataCache.Get(key);

            // Return the item or null
            if (obj != null)
                return (T)(obj);
            
            return default;
        }

        /// <summary>
        /// Remove an item from the cache
        /// </summary>
        public void RemoveItem(string key)
        {
            // Try to remove the object from the cache;
            _dataCache.Remove(key);
        }
    }
}
