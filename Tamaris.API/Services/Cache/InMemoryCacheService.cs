using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

using Tamaris.API.Services.Cache.Interfaces;

namespace Tamaris.API.Services.Cache
{
	public class InMemoryCacheService : ICacheService
	{
		private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

		public Task RemoveCachedItemAsync(string key)
		{
			_cache.Remove(key);
			return Task.CompletedTask;
		}

		public Task<string> GetCachedStringAsync(string key)
		{
			return Task.FromResult(_cache.Get<string>(key));
		}

		public Task SetCacheValueAsync(string key, string value)
		{
			_cache.Set(key, value);
			return Task.CompletedTask;
		}


		public Task<object> GetCachedObjectAsync(string key)
		{
			return Task.FromResult(_cache.Get<object>(key));
		}

		public Task SetCacheValueAsync(string key, object value)
		{
			_cache.Set(key, value);
			return Task.CompletedTask;
		}
	}
}