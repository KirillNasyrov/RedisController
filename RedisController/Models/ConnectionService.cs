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
    public List<RedisDatabaseConfiguration> Configurations { get; private set; }
    public Dictionary<string, ConnectionMultiplexer> Connections { get; set; }

    private string FileWithCongigs { get; set; }

    public ConnectionService()
    {
        Connections = new Dictionary<string, ConnectionMultiplexer>();
        Configurations = new List<RedisDatabaseConfiguration>();
        
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
            Configurations = JsonSerializer.Deserialize<List<RedisDatabaseConfiguration>>(jsonString);
            
        }
    }

    public void AddNewConnection(string dataBaseID, ConnectionMultiplexer newConnection)
    {
        Connections.Add(dataBaseID, newConnection);
    }

    public bool WasConnected(string dataBaseID)
    {
        return Connections.ContainsKey(dataBaseID);
    }


    public async Task<ConnectionMultiplexer> CreateConnectionAsync(string dataBaseID)
    {
        var config = Configurations.Find((conf) => conf.DatabaseID == dataBaseID);

        string connectionConfig = $"{config.DatabaseHost}:{config.DatabasePort}, password = {config.DatabasePassword}, allowAdmin=true";
        return await ConnectionMultiplexer.ConnectAsync(connectionConfig);
    }


    public async void UpdateConfigs()
    {
        using FileStream createStream = File.Create(FileWithCongigs);
        await JsonSerializer.SerializeAsync(createStream, Configurations);

    }
}

