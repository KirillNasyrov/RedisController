using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisController.Models;

public class ConnectionService
{
    public List<RedisDataBaseConfiguration> Configurations { get; private set; }
    private Dictionary<string, ConnectionMultiplexer> Connections {  get; set; }

    public ConnectionService()
    {
        Connections = new Dictionary<string, ConnectionMultiplexer>();
        Configurations = new List<RedisDataBaseConfiguration>();
        Configurations.Add(new RedisDataBaseConfiguration("redis1", "localhost", "32768", "redispw"));
    }

    public IDatabase getConnection(string dataBaseID)
    {
        if (Connections.ContainsKey(dataBaseID))
        {
            if (Connections[dataBaseID].IsConnected)
            {
                return Connections[dataBaseID].GetDatabase();
            }
            throw new ArgumentException("Data base with such name is not connected");
        }
        else
        {
            var config = Configurations.Find((conf) => conf.DataBaseID == dataBaseID);

            string connectionConfig = $"{config.DataBaseHost}:{config.DataBasePort}, password = {config.DataBasePassword}";
            Connections[dataBaseID] = ConnectionMultiplexer.Connect(connectionConfig);
            return Connections[dataBaseID].GetDatabase();
        }

        
        
    } 

    public IDatabase AddNewConnection(string dataBaseID, string configuration, string password)
    {
        if (!Connections.ContainsKey(dataBaseID))
        {
            Connections[dataBaseID] = ConnectionMultiplexer.Connect($"{configuration}, password = {password}");
            return Connections[dataBaseID].GetDatabase();
        }
        throw new ArgumentException("Data base with such name is already added");
    }
}

