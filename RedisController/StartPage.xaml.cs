using RedisController.Models;

namespace RedisController;

public partial class StartPage : ContentPage
{
	List<RedisDataBaseConfiguration> configs;

    public StartPage()
    {
        InitializeComponent();

        configs = new List<RedisDataBaseConfiguration>();
		configs.Add(new RedisDataBaseConfiguration("redis1", "localhost", "32778"));
        configs.Add(new RedisDataBaseConfiguration("redis2", "localhost", "32779"));
        configs.Add(new RedisDataBaseConfiguration("redis3", "localhost", "32780"));
        configs.Add(new RedisDataBaseConfiguration("redis4", "localhost", "32781"));
        configs.Add(new RedisDataBaseConfiguration("redis5", "localhost", "32782"));
        configs.Add(new RedisDataBaseConfiguration("redis6", "localhost", "32783"));
        configs.Add(new RedisDataBaseConfiguration("redis7", "localhost", "32784"));
        tableOfConfigs.ItemsSource = configs;
    }

}