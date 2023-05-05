using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RedisController.Models;
public class RedisDataBaseConfiguration
{
    public string DataBaseID
    {
        get;
        set;
    }

    public string DataBaseHost
    {
        get;
        set;
    }

    public string DataBasePort
    {
        get;
        set;
    }

    public DateOnly DataBaseLastConnection
    {
        get;
        set;
    }

    public string DataBasePassword
    {
        get;
        set;
    }
    public RedisDataBaseConfiguration(string identifier, string host, string port, string password)
    {
        DataBaseID = identifier;
        DataBaseHost = host;
        DataBasePort = port;
        DataBaseLastConnection = DataBaseLastConnection = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        DataBasePassword = password;
    }

    [JsonConstructor]
    public RedisDataBaseConfiguration()
    {
    }

}

