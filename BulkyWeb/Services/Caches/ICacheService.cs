namespace BulkyWeb.Services.Caches
{
    public interface ICacheService
    {
        Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);

    }
}
