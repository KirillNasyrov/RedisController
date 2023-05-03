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
                await DisplayAlert("Alert", "Can not connect to data base", "OK");
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

        // �������� �������� �� ��������� ������
        Configs.Remove(currentConfig);

        // ���������� ��������� ������ ��� ����������� ������������ ������
        tableOfConfigs.ItemsSource = new List<RedisDataBaseConfiguration>(Configs);
    }

}