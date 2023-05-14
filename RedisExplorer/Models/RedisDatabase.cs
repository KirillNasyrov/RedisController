using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisExplorer.Models;

public class RedisDatabase
{
    public IServer Server { get; set; }
    public IDatabase Database { get; set; }

    public RedisDatabase(ConnectionMultiplexer connection, RedisDatabaseConfiguration configuration)
    {
        Database = connection.GetDatabase();

        Server = connection.GetServer($"{configuration.DatabaseHost}:{configuration.DatabasePort}");
    }

    public IEnumerable<RedisKey> GetRedisKeys()
    {
        if (Server.IsConnected)
        {
            return Server.Keys(0);
        }
        throw new InvalidOperationException("Server is not connected");
    }

    public RedisType GetKeyType(RedisKey key)
    {
        return Database.KeyType(key);
    }

    public async Task<RedisValue> StringGetAsync(RedisKey key)
    {
        if (Server.IsConnected)
        {
            return await Database.StringGetAsync(key);
        }
        throw new InvalidOperationException("Server is not connected");
    }

    public async Task DeleteKeyAsync(RedisKey key)
    {
        if (Server.IsConnected)
        {
            await Database.KeyDeleteAsync(key);
        }
        else
        {
            throw new InvalidOperationException("Server is not connected");
        }
    }

    public async Task StringSetValueAsync(RedisKey key, RedisValue value)
    {
        if (Server.IsConnected)
        {
            await Database.StringSetAsync(key, value);
        }
        else
        {
            throw new InvalidOperationException("Server is not connected");
        }
    }

    public async Task<List<Tuple<int, RedisValue>>> ListGetAsync(RedisKey key)
    {
        if (Server.IsConnected)
        {
            var length = (int)(await Database.ListLengthAsync(key));
            var list = new List<Tuple<int, RedisValue>> (length);

            for (int i = 0; i < length; ++i)
            {
                list.Add(new Tuple<int, RedisValue>(i, await Database.ListGetByIndexAsync(key, i)));
            }
            return list;
        }
        else
        {
            throw new InvalidOperationException("Server is not connected");
        }
    }

    public async Task<List<RedisValue>> SetGetAsync(RedisKey key)
    {
        if (Server.IsConnected)
        {
            return new List<RedisValue> (await Database.SetMembersAsync(key));
        }
        else
        {
            throw new InvalidOperationException("Server is not connected");
        }
    }

}

