using Microsoft.Extensions.Caching.Distributed;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Redis server address
    options.InstanceName = "RedisApp:";
});

var app = builder.Build();

app.MapPost("/store", async (IDistributedCache cache) => 
{
    string key = "product1";
    string value = "Product: EcoBottle, Price: $15";
    byte[] valueBytes = Encoding.UTF8.GetBytes(value);
    var options = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };
    await cache.SetAsync(key, valueBytes, options);
    return Results.Ok("Data stored in Redis successfully.");
});

app.MapGet("/retrieve", async (IDistributedCache cache) =>
{
    string key = "product1";
    byte[]? cachedValue = await cache.GetAsync(key);
    if(cachedValue != null)
    {
        string value = Encoding.UTF8.GetString(cachedValue);
        return Results.Ok($"Retrieve value: {value}");
    }
    else
    {
        return Results.NotFound("The data was not found in the cache.");
    }
});

app.MapDelete("/remove", async (IDistributedCache cache) =>
{
    string key = "product1";
    byte[]? cachedValue = await cache.GetAsync(key);
    if(cachedValue == null)
    {
        return Results.NotFound($"Key '{key}' not found in Redis.");
    }
    await cache.RemoveAsync(key);
    return Results.Ok($"Key '{key}' removed from Redis.");
});

app.MapGet("/monitor", async (IDistributedCache cache) =>
{
    string key = "product1";
    byte[]? cachedValue = await cache.GetAsync(key);
    if(cachedValue != null)
    {
        return Results.Ok("Cache hit.");
    }
    else
    {
        return Results.NotFound("Cache miss.");
    }
});

app.Run();
