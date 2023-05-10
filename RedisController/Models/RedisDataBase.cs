using System;
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

}

