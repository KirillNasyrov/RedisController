using RedisController.Models;
using System.Security.AccessControl;
using StackExchange.Redis;

namespace RedisController;

public partial class StartPage : ContentPage
{
	public List<RedisDataBaseConfiguration> Configs { get; set; }

    public StartPage()
    {
        InitializeComponent();

        Configs = new List<RedisDataBaseConfiguration>() { };
        Configs .Add(new RedisDataBaseConfiguration("redis1", "localhost", "32778"));
        Configs.Add(new RedisDataBaseConfiguration("redis2", "localhost", "32779"));
        Configs.Add(new RedisDataBaseConfiguration("redis3", "localhost", "32780"));
        Configs.Add(new RedisDataBaseConfiguration("redis4", "localhost", "32781"));
        Configs.Add(new RedisDataBaseConfiguration("redis5", "localhost", "32782"));
        Configs.Add(new RedisDataBaseConfiguration("redis6", "localhost", "32783"));
        Configs.Add(new RedisDataBaseConfiguration("redis7", "localhost", "32784"));
        BindingContext = this;
    }

    void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is RedisDataBaseConfiguration currentConfig)
        {

            ToCommonPage(sender, e);
            //Configs.Remove(currentConfig);
            //tableOfConfigs.ItemsSource = new List<RedisDataBaseConfiguration>(Configs);
            //tableOfConfigs.SelectedItem = null;
        }
    }

    private async void ToCommonPage(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new RedisDataBasePage());
    }

    private void OnButtonClicked(object sender, System.EventArgs e)
    {
        var button = sender as ImageButton;
        var currentConfig = button.CommandParameter as RedisDataBaseConfiguration;

        // Удаление элемента из источника данных
        Configs.Remove(currentConfig);

        // Обновление источника данных для отображения обновленного списка
        tableOfConfigs.ItemsSource = new List<RedisDataBaseConfiguration>(Configs);
    }

}