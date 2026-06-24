namespace TraineeManagement.Api.Services;

public interface IRedisService<T> where T : class
{
    
    Task<T?> GetAsync(string id);
    Task SetAsync(string id,T data, TimeSpan? expiration = null);
    Task RemoveAsync(string id);



}