using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace RedisController.Models;

public class ConnectionService
{
    public List<RedisDataBaseConfiguration> Configurations { get; private set; }
    private Dictionary<string, ConnectionMultiplexer> Connections {  get; set; }

    private string FileWithCongigs { get; set; }

    public ConnectionService()
    {
        Connections = new Dictionary<string, ConnectionMultiplexer>();
        Configurations = new List<RedisDataBaseConfiguration>();
        
        var appDataDirectory = FileSystem.AppDataDirectory;
        var configurationsDirectory = Path.Combine(appDataDirectory, "Configurations");
        FileWithCongigs = Path.Combine(configurationsDirectory, "configs.json");

        if (!File.Exists(FileWithCongigs))
        {
            Directory.CreateDirectory(configurationsDirectory);
            File.Create(FileWithCongigs);
            UpdateConfigs();
        }
        else
        {
            string jsonString = File.ReadAllText(FileWithCongigs);
            Configurations = JsonSerializer.Deserialize<List<RedisDataBaseConfiguration>>(jsonString);
            
        }
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

    public async void UpdateConfigs()
    {
        using FileStream createStream = File.Create(FileWithCongigs);
        await JsonSerializer.SerializeAsync(createStream, Configurations);

    }
}

