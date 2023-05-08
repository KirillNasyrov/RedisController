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

    private async void RedisConfigurationSelectedAsync(object sender, SelectionChangedEventArgs e)
    {
        if (tableOfConfigs.SelectedItem != null)
        {
            try
            {
                var selectedConfig = (RedisDataBaseConfiguration)tableOfConfigs.SelectedItem;

                if (!connectionService.WasConnected(selectedConfig.DataBaseID))
                {
                    var connectionTask = connectionService.GetConnectionAsync(selectedConfig.DataBaseID);

                    var connection = await connectionTask;
                    connectionService.AddNewConnection(selectedConfig.DataBaseID, connection);
                }
               
                var connectedDataBase = new RedisDataBase(connectionService.GetAddedDataBase(selectedConfig.DataBaseID));

                await Navigation.PushAsync(new RedisDataBasePage(connectedDataBase, selectedConfig.DataBaseID), false);
            }
            catch (ArgumentException ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
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

    private void AddRedisDataBaseButtonClicked(object sender, System.EventArgs e)
    {
        gridOfConfigs.ColumnDefinitions.ElementAt(5).Width = new GridLength(2, GridUnitType.Star);
    }

    private void CancelAddingDataBaseButtonClicked(object sender, System.EventArgs e)
    {
        dataBaseNameEntry.Text = null;
        dataBaseHostEntry.Text = null;
        dataBasePortEntry.Text = null;
        dataBasePassswordEntry.Text = null;

        gridOfConfigs.ColumnDefinitions.ElementAt(5).Width = new GridLength(0, GridUnitType.Star);
    }

    public async void ApplyAddingDataBaseButtonClickedAsync(object sender, System.EventArgs e)
    {
        if (Configs.FindAll((config) => config.DataBaseID == dataBaseNameEntry.Text).Any())
        {
            await DisplayAlert("Error", "You already has database with such name", "OK");
        }
        else
        {
            var newConfig = new RedisDataBaseConfiguration(dataBaseNameEntry.Text, dataBaseHostEntry.Text,
                    dataBasePortEntry.Text, dataBasePassswordEntry.Text);
            try
            {
                var connectionTask = connectionService.GetConnectionAsync(newConfig.DataBaseID);

                var connection = await connectionTask;
                connectionService.AddNewConnection(newConfig.DataBaseID, connection);


                var connectedDataBase = new RedisDataBase(connectionService.GetAddedDataBase(newConfig.DataBaseID));
                connectionService.Configurations.Add(newConfig);


                await Navigation.PushAsync(new RedisDataBasePage(connectedDataBase, newConfig.DataBaseID), false);

                tableOfConfigs.ItemsSource = new List<RedisDataBaseConfiguration>(Configs);
                connectionService.UpdateConfigs();


                gridOfConfigs.ColumnDefinitions.ElementAt(5).Width = new GridLength(0, GridUnitType.Star);
            }
            catch (ArgumentException ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Can not connect", "OK");
            }
        }
    }

}