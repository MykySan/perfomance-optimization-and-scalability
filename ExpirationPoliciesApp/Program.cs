using StackExchange.Redis;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var redis = ConnectionMultiplexer.Connect("localhost");
        var db = redis.GetDatabase();

        string key = "absolute-key";
        string value = "This is an absolute expiration item.";

        await db.StringSetAsync(key, value);
        db.KeyExpire(key, TimeSpan.FromMinutes(1));

        Console.WriteLine($"Cached Value: {await db.StringGetAsync(key)}");

        await Task.Delay(61000);

        Console.WriteLine($"Cached Value after expiration: {await db.StringGetAsync(key)}");

        string slidingkey = "sliding-key";
        string slidingvalue = "Sliding expiration data";

        await db.StringSetAsync(slidingkey, slidingvalue, TimeSpan.FromSeconds(30));

        for (int i = 0; i < 3; i++)
        {
            RedisValue cachedValue = await db.StringGetAsync(slidingkey);

            if(!cachedValue.IsNullOrEmpty)
            {
                Console.WriteLine($"Access {i + 1}: Cached Value: {cachedValue}");
                db.KeyExpire(slidingkey, TimeSpan.FromSeconds(30));
            }
            else
            {
                Console.WriteLine($"Access {i + 1}: Key '{slidingkey}' does not exist.");
                break;
            }
            await Task.Delay(10000);
        }
        await Task.Delay(31000);
        RedisValue finalValue = await db.StringGetAsync(slidingkey);
        Console.WriteLine($"Cached Value after expiration: {await db.StringGetAsync(slidingkey)}");
    
        string parentKey = "product";
        string childKey = "inventory";

        await db.StringSetAsync(parentKey, "Product data");
        await db.StringSetAsync(childKey, "Inventory data");

        Console.WriteLine("\nInitial Cache State:");
        Console.WriteLine($"Parent Key: {await db.StringGetAsync(parentKey)}");
        Console.WriteLine($"Child Key: {await db.StringGetAsync(childKey)}");

        Console.WriteLine("\nUpdatind parent entry...");
        await db.StringSetAsync(parentKey, "Updated product data");

        if(await db.StringGetAsync(parentKey) == "Updated product data")
        {
            Console.WriteLine("Parent updated. Expiring dependent entry...");
            await db.KeyDeleteAsync(childKey);
        }

        Console.WriteLine("\nFinal Cache State:");
        Console.WriteLine($"Parent Key: {await db.StringGetAsync(parentKey)}");
        Console.WriteLine($"Child Key: {await db.StringGetAsync(childKey)}");
    }
}