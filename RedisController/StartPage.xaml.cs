using RedisController.Models;
using System.Security.AccessControl;
using StackExchange.Redis;
using System.Windows.Input;
using System;

namespace RedisController;

public partial class StartPage : ContentPage
{
	public List<RedisDataBaseConfiguration> Configs { get; set; }

    private ConnectionService connectionService;

    public StartPage()
    {
        connectionService = new ConnectionService();

        InitializeComponent();

        Configs = new List<RedisDataBaseConfiguration>();
        Configs.Add(new RedisDataBaseConfiguration("redis1", "localhost", "32768"));
        BindingContext = this;
    }

    private async void RedisConfigurationSelected(object sender, SelectionChangedEventArgs e)
    {
        if (tableOfConfigs.SelectedItem != null)
        {
            try
            {
                var config = (RedisDataBaseConfiguration)tableOfConfigs.SelectedItem;
                await Navigation.PushAsync(new RedisDataBasePage(connectionService.getConnection(config.DataBaseID)));
            } 
            catch (Exception)
            {
                await DisplayAlert("Question?", "Would you like to play a game", "Yes", "No");
            }
            finally
            {
                tableOfConfigs.SelectedItem = null;
            }
        }
    }

    /*private async void ToCommonPage(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new RedisDataBasePage());
    }*/

    private void RedisConfigurationRemoved(object sender, System.EventArgs e)
    {
        var button = sender as ImageButton;
        var currentConfig = button.CommandParameter as RedisDataBaseConfiguration;

        // Удаление элемента из источника данных
        Configs.Remove(currentConfig);

        // Обновление источника данных для отображения обновленного списка
        tableOfConfigs.ItemsSource = new List<RedisDataBaseConfiguration>(Configs);
    }

}