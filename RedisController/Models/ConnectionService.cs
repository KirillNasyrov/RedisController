using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisController.Models;

public class ConnectionService
{
    private Dictionary<string, ConnectionMultiplexer> connections {  get; set; }
    private Dictionary<string, string> passwords { get; set; }

    public ConnectionService()
    {
        connections = new Dictionary<string, ConnectionMultiplexer>();
        passwords = new Dictionary<string, string>();
        connections["redis1"] = ConnectionMultiplexer.Connect("localhost:32768, password = redispw");
    }

    public IDatabase getConnection(string dataBaseID)
    {
        return connections[dataBaseID].GetDatabase();
    }

    public IDatabase AddNewConnection(string dataBaseID, string configuration, string password)
    {
        if (!connections.ContainsKey(dataBaseID))
        {
            connections[dataBaseID] = ConnectionMultiplexer.Connect($"{configuration}, password = {password}");
            return connections[dataBaseID].GetDatabase();
        }
        throw new ArgumentException("Data base with such name is already added");
    }
}

