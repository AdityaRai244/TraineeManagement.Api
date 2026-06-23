using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class RedisService<T> : IRedisService<T> where T : class
{



    private readonly ILogger<RedisService<T>> _logger;
    private readonly IDistributedCache _cache;

    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(10);
    private readonly string _keyPrefix = $"{typeof(T).Name}:";

    private string GetCacheKey(string id) => $"{_keyPrefix}{id}";




    public RedisService(ILogger<RedisService<T>> logger, IDistributedCache distributedCache)
    {
        _cache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync(string id)
    {
        try
        {
            string fullKey = GetCacheKey(id);
            string? cachedJson = await _cache.GetStringAsync(fullKey);

            if (cachedJson is null)
            {
                _logger.LogInformation("Cache miss for key: {Key}", fullKey);
                return null;
            }

            _logger.LogInformation("Cache hit for key: {Key}", fullKey);
            return JsonSerializer.Deserialize<T>(cachedJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read from Redis for ID: {Id}", id);
            return null;
        }


    }

    public async Task SetAsync(string id, T data, TimeSpan? expiration = null)
    {

        try
        {
            string fullKey = GetCacheKey(id);
            string finalData = JsonSerializer.Serialize(data);
            var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(expiration ?? _defaultExpiration);
            await _cache.SetStringAsync(fullKey, finalData, options);
            _logger.LogInformation("Cache set for key: {Key}", fullKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set redis cache for ID: {Id}", id);
        }

    }

    public async Task RemoveAsync(string id)
    {
        try
        {

            string key = GetCacheKey(id);
            await _cache.RemoveAsync(key);
            _logger.LogInformation("Cache remove for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set redis cache for ID: {Id}", id);
        }
    }




}