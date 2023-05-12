﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisController;
using RedisController.Models;
using StackExchange.Redis;

namespace RedisController.Models;

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

}

