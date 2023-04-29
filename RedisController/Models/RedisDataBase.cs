using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisController;
using RedisController.Models;
using StackExchange.Redis;

namespace RedisController.Models;

internal class RedisDataBase
{
    private ConnectionMultiplexer connectionMultiplexer;

    private IDatabase database;

    public RedisDataBase(string configuration)
    {
        connectionMultiplexer = ConnectionMultiplexer.Connect(configuration);
        database = connectionMultiplexer.GetDatabase();
    }

    public string Alias
    {
        get => connectionMultiplexer.ClientName;
    }

    public string Configuration
    {
        get => connectionMultiplexer?.Configuration;
    }
}

