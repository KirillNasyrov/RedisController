using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RedisController.Models;
public class RedisDatabaseConfiguration
{
    public string DatabaseID { get; set; }

    public string DatabaseHost { get; set; }

    public string DatabasePort { get; set; }

    public DateOnly DatabaseLastConnection { get; set; }

    public string DatabasePassword { get; set; }
    public RedisDatabaseConfiguration(string identifier, string host, string port, string password)
    {
        DatabaseID = identifier;
        DatabaseHost = host;
        DatabasePort = port;
        DatabaseLastConnection = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        DatabasePassword = password;
    }

    [JsonConstructor]
    public RedisDatabaseConfiguration()
    {
    }

    public void UpdateLastTimeConnection()
    {
        DatabaseLastConnection = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
    }

}

