using RedisController.Models;
using System.Security.AccessControl;
using StackExchange.Redis;
using System.Windows.Input;
using System;

namespace RedisController;

public partial class StartPage : ContentPage
{
	public List<RedisDataBaseConfiguration> Configs 
    {
        get => connectionService.Configurations;
    }

    private ConnectionService connectionService;

    public StartPage()
    {
        connectionService = new ConnectionService();

        InitializeComponent();

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
                await DisplayAlert("Error", "Can not connect to data base", "OK");
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
        connectionService.Configurations.Remove(currentConfig);

        // Обновление источника данных для отображения обновленного списка
        tableOfConfigs.ItemsSource = new List<RedisDataBaseConfiguration>(Configs);
        connectionService.UpdateConfigs();
    }

    private void AddRedisDataBaseClicked(object sender, System.EventArgs e)
    {
        gridOfConfigs.ColumnDefinitions.ElementAt(5).Width = new GridLength(2, GridUnitType.Star);
    }

    private void CancelAddingDataBaseClicked(object sender, System.EventArgs e)
    {
        dataBaseNameEntry.Text = null;
        dataBaseHostEntry.Text = null;
        dataBasePortEntry.Text = null;
        dataBasePassswordEntry.Text = null;

        gridOfConfigs.ColumnDefinitions.ElementAt(5).Width = new GridLength(0, GridUnitType.Star);
    }

    public async void ApplyAddingDataBaseClicked(object sender, System.EventArgs e)
    {
        if (Configs.FindAll((config) => config.DataBaseID == dataBaseNameEntry.Text).Any())
        {
            await DisplayAlert("Error", "You already has database with such name", "OK");
        }
        else
        {
            RedisDataBaseConfiguration config = new();
            try
            {
                config = new RedisDataBaseConfiguration(dataBaseNameEntry.Text, dataBaseHostEntry.Text,
                    dataBasePortEntry.Text, dataBasePassswordEntry.Text);

                connectionService.Configurations.Add(config);
                await Navigation.PushAsync(new RedisDataBasePage(connectionService.getConnection(config.DataBaseID)));

                tableOfConfigs.ItemsSource = new List<RedisDataBaseConfiguration>(Configs);
                connectionService.UpdateConfigs();


                gridOfConfigs.ColumnDefinitions.ElementAt(5).Width = new GridLength(0, GridUnitType.Star);
            }
            catch (Exception ex)
            {
                connectionService.Configurations.Remove(config);
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

}