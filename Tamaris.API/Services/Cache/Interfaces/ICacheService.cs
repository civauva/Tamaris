using System.Threading.Tasks;

namespace Tamaris.API.Services.Cache.Interfaces
{
	public interface ICacheService
	{
		Task RemoveCachedItemAsync(string key);

		Task<string> GetCachedStringAsync(string key);
		Task SetCacheValueAsync(string key, string value);

		Task<object> GetCachedObjectAsync(string key);
		Task SetCacheValueAsync(string key, object value);
	}
}