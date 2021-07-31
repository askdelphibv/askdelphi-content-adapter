using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.Cache
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public async Task<T> GetAsync<T>(CacheKey key)
        {
            string memoryCacheKey = key.AsString();
            if (!memoryCache.TryGetValue<T>(memoryCacheKey, out T result))
            {
                return await Task.FromResult(default(T));
            }
            return await Task.FromResult<T>(result);
        }

        public Task<T> SetWithAbsoluteExpirationAsync<T>(CacheKey key, T value, DateTimeOffset expiresAt)
        {
            string memoryCacheKey = key.AsString();
            T result = memoryCache.Set<T>(memoryCacheKey, value, expiresAt);
            return Task.FromResult<T>(result);
        }

        public Task<T> SetWithSlidingExpiration<T>(CacheKey key, T value, TimeSpan expireAfter)
        {
            string memoryCacheKey = key.AsString();
            T result = memoryCache.Set<T>(memoryCacheKey, value, new MemoryCacheEntryOptions { SlidingExpiration = expireAfter });
            return Task.FromResult<T>(result);
        }
    }
}
