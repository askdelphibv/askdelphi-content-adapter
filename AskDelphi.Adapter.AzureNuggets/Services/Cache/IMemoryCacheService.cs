using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.Cache
{
    public interface IMemoryCacheService
    {
        Task<T> GetAsync<T>(CacheKey key);
        Task<T> SetWithAbsoluteExpirationAsync<T>(CacheKey key, T value, DateTimeOffset expiresAt);
        Task<T> SetWithSlidingExpiration<T>(CacheKey key, T value, TimeSpan expireAfter);
    }
}
