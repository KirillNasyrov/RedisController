using RedisController.Models;
using StackExchange.Redis;

namespace RedisController;

public partial class RedisDatabasePage : ContentPage
{
    public ConnectionService ConnectionService { get; private set; }
	public RedisDatabase RedisDatabase { get; private set; }

	private RedisDatabaseConfiguration Configuration { get; set; }
	public Dictionary<RedisKey,RedisType> KeyTypePairs
    {
        get => RedisDatabase.GetRedisKeys().ToDictionary(key => key, key => RedisDatabase.GetKeyType(key));
    }


    public RedisDatabasePage(ConnectionMultiplexer connection, RedisDatabaseConfiguration configuration)
	{
		RedisDatabase = new RedisDatabase(connection, configuration);
		Configuration = configuration;
        InitializeComponent();

        DatabaseNameEnrty.Text = Configuration.DatabaseID;
        BindingContext = this;
	}

	private async void BackButtonClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync(false);
	}

	private async void RedisKeySelectedAsync(object sender, SelectionChangedEventArgs e)
	{
        if (TableOfKeys.SelectedItem == null)
        {
            return;
        }

        try
        {
            var selectedKey = (KeyValuePair<RedisKey, RedisType>)TableOfKeys.SelectedItem;
            TypeGrid.ColumnDefinitions.ElementAt(0).Width = new GridLength(0, GridUnitType.Star);
            TypeGrid.ColumnDefinitions.ElementAt(1).Width = new GridLength(1, GridUnitType.Star);
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Database is not connected", "OK");
        }
        finally
        {
            TableOfKeys.SelectedItem = null;
        }
    }
}