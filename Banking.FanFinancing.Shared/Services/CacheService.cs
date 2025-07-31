using Banking.FanFinancing.Shared.Exceptions;
using Banking.FanFinancing.Shared.Services.Interface;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace Banking.FanFinancing.Shared.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public bool TryGet<T>(string cacheKey, out T value)
        {
            value = default!;
            var cachedData = _distributedCache.Get(cacheKey);
            if (cachedData != null)
            {
                var serializedCachedData = Encoding.UTF8.GetString(cachedData);
                if (serializedCachedData is not null)
                {
                    var result = JsonConvert.DeserializeObject<T>(serializedCachedData);
                    value = result ?? throw new NullModelException();
                }
            }
            return value != null;
        }
        public T Set<T>(string cacheKey, T value, int AbsoluteExpiration = 10, int SlidingExpiration = 5)
        {
            var serializedData = JsonConvert.SerializeObject(value);
            var byteData = Encoding.UTF8.GetBytes(serializedData);
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(AbsoluteExpiration))
                .SetSlidingExpiration(TimeSpan.FromMinutes(SlidingExpiration));
            _distributedCache.Set(cacheKey, byteData, options);
            return value;
        }
        public T Refresh<T>(string cacheKey, T value, int AbsoluteExpiration = 10, int SlidingExpiration = 5)
        {
            Remove(cacheKey);
            var serializedData = JsonConvert.SerializeObject(value);
            var byteData = Encoding.UTF8.GetBytes(serializedData);
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(AbsoluteExpiration))
                .SetSlidingExpiration(TimeSpan.FromMinutes(SlidingExpiration));
            _distributedCache.Set(cacheKey, byteData, options);
            return value;
        }
        public void Remove(string cacheKey)
        {
            _distributedCache.Remove(cacheKey);
        }
    }
}
