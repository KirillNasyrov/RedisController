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
    private Dictionary<RedisType, int> TypeGrids { get; set; }
    private Dictionary<RedisType, Label> TypeLabels { get; set; }

    public RedisDatabasePage(ConnectionMultiplexer connection, RedisDatabaseConfiguration configuration)
	{
        TypeGrids = new Dictionary<RedisType, int>()
        {
            {RedisType.String, 1 },
            {RedisType.List, 2 },
            {RedisType.Set, 3 },
            {RedisType.Hash, 4 },
            {RedisType.Stream, 5 }
        };
		RedisDatabase = new RedisDatabase(connection, configuration);
		Configuration = configuration;
        InitializeComponent();

        DatabaseNameEnrty.Text = Configuration.DatabaseID;

        TypeLabels = new Dictionary<RedisType, Label>()
        {
            {RedisType.String, StringKeyNameLabel },
            {RedisType.List, ListKeyNameLabel }
        };

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
            foreach (var column in TypeGrid.ColumnDefinitions)
            {
                column.Width = new GridLength(0, GridUnitType.Star);
            }
            TypeGrid.ColumnDefinitions.ElementAt(TypeGrids[selectedKey.Value]).Width = new GridLength(1, GridUnitType.Star);
            TypeLabels[selectedKey.Value].Text = selectedKey.Key;
            StringValueEntry.Text = await RedisDatabase.StringGetAsync(selectedKey.Key);
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