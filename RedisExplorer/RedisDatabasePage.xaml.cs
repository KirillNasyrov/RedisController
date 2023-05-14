using RedisExplorer.Models;
using StackExchange.Redis;

namespace RedisExplorer;

public partial class RedisDatabasePage : ContentPage
{
    public ConnectionService ConnectionService { get; private set; }
    public RedisDatabase RedisDatabase { get; private set; }
    private RedisDatabaseConfiguration Configuration { get; set; }
    public Dictionary<RedisKey, RedisType> KeyTypePairs
    {
        get => RedisDatabase.GetRedisKeys().ToDictionary(key => key, key => RedisDatabase.GetKeyType(key));
    }
    private Dictionary<RedisType, int> TypeGrids { get; set; }
    private Dictionary<RedisType, Label> TypeLabels { get; set; }
    private Dictionary<RedisType, Editor> TypeEditors { get; set; }
    private KeyValuePair<RedisKey, RedisType> selectedKey { get; set; }

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
            {RedisType.List, ListKeyNameLabel },
            {RedisType.Set, SetKeyNameLabel },
        };

        TypeEditors = new Dictionary<RedisType, Editor>()
        {
            {RedisType.String, StringValueEditor },
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
            selectedKey = (KeyValuePair<RedisKey, RedisType>)TableOfKeys.SelectedItem;
            foreach (var column in TypeGrid.ColumnDefinitions)
            {
                column.Width = new GridLength(0, GridUnitType.Star);
            }
            TypeGrid.ColumnDefinitions.ElementAt(TypeGrids[selectedKey.Value]).Width = new GridLength(1, GridUnitType.Star);
            TypeLabels[selectedKey.Value].Text = selectedKey.Key;
            if (selectedKey.Value == RedisType.String)
            {
                StringValueEditor.Text = await RedisDatabase.StringGetAsync(selectedKey.Key);
            }
            if (selectedKey.Value == RedisType.List)
            {
                var list = await RedisDatabase.ListGetAsync(selectedKey.Key);
                ListValueCollectionView.ItemsSource = await RedisDatabase.ListGetAsync(selectedKey.Key);
            }
            if (selectedKey.Value == RedisType.Set)
            {
                var set = await RedisDatabase.SetGetAsync(selectedKey.Key);
                SetValueCollectionView.ItemsSource = await RedisDatabase.SetGetAsync(selectedKey.Key);
            }
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Can not select the key", "OK");
        }
        finally
        {
            TableOfKeys.SelectedItem = null;
        }
    }

    private void EditValueButtonClicked(object sender, EventArgs e)
    {
        GridWithCancelEditButtons.RowDefinitions.ElementAt(0).Height = 45;
        TypeEditors[selectedKey.Value].IsReadOnly = false;
    }

    public async void SaveChangesButtonClickedAsync(object sender, EventArgs e)
    {
        var deletingKey = selectedKey.Key;
        try
        {
            await RedisDatabase.StringSetValueAsync(deletingKey, StringValueEditor.Text);
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Can not edit the key", "OK");
        }
        finally
        {
            GridWithCancelEditButtons.RowDefinitions.ElementAt(0).Height = 0;
            TypeEditors[selectedKey.Value].IsReadOnly = true;
        }
    }

    private void CancelEditingOrAddingKeyButtonClicked(object sender, EventArgs e)
    {
        GridWithCancelEditButtons.RowDefinitions.ElementAt(0).Height = 0;
        foreach (var column in TypeGrid.ColumnDefinitions)
        {
            column.Width = new GridLength(0, GridUnitType.Star);
        }
        TypeGrid.ColumnDefinitions.ElementAt(0).Width = new GridLength(1, GridUnitType.Star);
    }

    public async void DeleteKeyButtonClickedAsync(object sender, EventArgs e)
    {
        var deletingKey = selectedKey.Key;
        try
        {
            await RedisDatabase.DeleteKeyAsync(deletingKey);
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Can not delete the key", "OK");
        }
        finally
        {
            TableOfKeys.ItemsSource = new Dictionary<RedisKey, RedisType>(KeyTypePairs);
            GridWithCancelEditButtons.RowDefinitions.ElementAt(0).Height = 0;
            foreach (var column in TypeGrid.ColumnDefinitions)
            {
                column.Width = new GridLength(0, GridUnitType.Star);
            }
            TypeGrid.ColumnDefinitions.ElementAt(0).Width = new GridLength(1, GridUnitType.Star);
        }

    }

    private void AddNewKeyButtonClicked(object sender, EventArgs e)
    {
        foreach (var column in TypeGrid.ColumnDefinitions)
        {
            column.Width = new GridLength(0, GridUnitType.Star);
        }
        TypeGrid.ColumnDefinitions.ElementAt(5).Width = new GridLength(1, GridUnitType.Star);
    }

    private async void ApplyAddingNewKeyButtonClickedAsync(object sender, EventArgs e)
    {
        try
        {
            var type = TypeOfAddingKeyPicker.SelectedItem;

            var key = NameOfAddingKeyEntry.Text;
            await RedisDatabase.StringSetValueAsync(key, StringValyeOfAddingKeyEditor.Text);

        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Can not edit the key", "OK");
        }
        finally
        {
            TableOfKeys.ItemsSource = new Dictionary<RedisKey, RedisType>(KeyTypePairs);
            foreach (var column in TypeGrid.ColumnDefinitions)
            {
                column.Width = new GridLength(0, GridUnitType.Star);
            }
            TypeGrid.ColumnDefinitions.ElementAt(0).Width = new GridLength(1, GridUnitType.Star);
        }



    }
}