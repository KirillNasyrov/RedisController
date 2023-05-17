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
    private KeyValuePair<RedisKey, RedisType> SelectedKey { get; set; }

    public RedisDatabasePage(ConnectionMultiplexer connection, RedisDatabaseConfiguration configuration)
    {
        TypeGrids = new Dictionary<RedisType, int>()
        {
            {RedisType.String, 1 },
            {RedisType.List, 2 },
            {RedisType.Set, 3 },
            {RedisType.Hash, 4 },
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
            {RedisType.Hash, HashKeyNameLabel },
        };

        TypeEditors = new Dictionary<RedisType, Editor>()
        {
            {RedisType.String, StringValueEditor },
        };

        BindingContext = this;
    }


    private void RefreshDatabaseButtonClicked(object sender, EventArgs e)
    {
        TableOfKeys.ItemsSource = new Dictionary<RedisKey, RedisType>(KeyTypePairs);
    }




    public async void HelpButtonClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Help", "Telegram: https://t.me/Kir1llLL", "OK");
    }



    private async void BrowserRedisOpenClicked(object sender, System.EventArgs e)
    {
        try
        {
            Uri uri = new Uri("https://redis.io/docs/");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception)
        {
            // An unexpected error occurred. No browser may be installed on the device.
        }
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
            SelectedKey = (KeyValuePair<RedisKey, RedisType>)TableOfKeys.SelectedItem;
            foreach (var column in TypeGrid.ColumnDefinitions)
            {
                column.Width = new GridLength(0, GridUnitType.Star);
            }
            TypeGrid.ColumnDefinitions.ElementAt(TypeGrids[SelectedKey.Value]).Width = new GridLength(1, GridUnitType.Star);
            TypeLabels[SelectedKey.Value].Text = SelectedKey.Key;
            if (SelectedKey.Value == RedisType.String)
            {
                StringValueEditor.Text = await RedisDatabase.StringGetAsync(SelectedKey.Key);
            }
            if (SelectedKey.Value == RedisType.List)
            {
                ListValueCollectionView.ItemsSource = await RedisDatabase.ListGetAsync(SelectedKey.Key);
            }
            if (SelectedKey.Value == RedisType.Set)
            {
                SetValueCollectionView.ItemsSource = await RedisDatabase.SetGetAsync(SelectedKey.Key);
            }
            if (SelectedKey.Value == RedisType.Hash)
            {
                HashValueCollectionView.ItemsSource = await RedisDatabase.HashGetAsync(SelectedKey.Key);
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
            CloseAll();
            TableOfKeys.SelectedItem = null;
        }
    }

    private void EditValueButtonClicked(object sender, EventArgs e)
    {
        EditingStringKeyGrid.RowDefinitions.ElementAt(0).Height = 45;
        TypeEditors[SelectedKey.Value].IsReadOnly = false;
    }

    public async void SaveChangesButtonClickedAsync(object sender, EventArgs e)
    {
        var deletingKey = SelectedKey.Key;
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
            EditingStringKeyGrid.RowDefinitions.ElementAt(0).Height = 0;
            TypeEditors[SelectedKey.Value].IsReadOnly = true;
        }
    }


    private void CancelAddingKeyButtonClicked(object sender, EventArgs e)
    {

        TypeGrid.ColumnDefinitions.ElementAt(5).Width = new GridLength(0, GridUnitType.Star);

        TypeGrid.ColumnDefinitions.ElementAt(0).Width = new GridLength(1, GridUnitType.Star);
    }


    private void CancelEditingKeyButtonClicked(object sender, EventArgs e)
    {
        CloseAll();
    }

    public async void DeleteKeyButtonClickedAsync(object sender, EventArgs e)
    {
        var deletingKey = SelectedKey.Key;
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
            EditingStringKeyGrid.RowDefinitions.ElementAt(0).Height = 0;
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

        TypeOfAddingKeyPicker.SelectedIndex = 0;
        NameOfAddingKeyEntry.Text = null;
        StringValyeOfAddingKeyEditor.Text = null;

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

    private void AddElementToListButtonClicked(object sender, EventArgs e)
    {
        GettingNewElementForListEntry.Text = null;

        EditingListKeyGrid.RowDefinitions.ElementAt(1).Height = 0;

        EditingListKeyGrid.RowDefinitions.ElementAt(0).Height = 100;
    }

    private async void ApplyAddingElementToListButtonClickedAsync(object sender, EventArgs e)
    {
        var key = SelectedKey.Key;
        try
        {
            var newValue = GettingNewElementForListEntry.Text;
            if (DirectionOfAddingElementForListPicker.SelectedIndex == 0)
            {
                await RedisDatabase.ListAddRightAsync(key, newValue);
                ListValueCollectionView.ItemsSource = await RedisDatabase.ListGetAsync(SelectedKey.Key);
            }
            else
            {
                await RedisDatabase.ListAddLeftAsync(key, newValue);
                ListValueCollectionView.ItemsSource = await RedisDatabase.ListGetAsync(SelectedKey.Key);
            }
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error while adding element", "OK");
        }
        finally
        {
            EditingListKeyGrid.RowDefinitions.ElementAt(0).Height = 0;
        }

    }



    private void RemoveElementFromListButtonClicked(object sender, EventArgs e)
    {
        GettingCountOfRemovingElementForListEntry.Text = null;

        EditingListKeyGrid.RowDefinitions.ElementAt(0).Height = 0;

        EditingListKeyGrid.RowDefinitions.ElementAt(1).Height = 100;
    }

    private async void ApplyRemovalElementToListButtonClickedAsync(object sender, EventArgs e)
    {
        var key = SelectedKey.Key;
        try
        {
            var count = int.Parse(GettingCountOfRemovingElementForListEntry.Text);
            if (DirectionOfRemovingElementForListPicker.SelectedIndex == 0)
            {
                await RedisDatabase.ListRemoveRightAsync(key, count);
                ListValueCollectionView.ItemsSource = await RedisDatabase.ListGetAsync(SelectedKey.Key);
            }
            else
            {
                await RedisDatabase.ListRemoveLeftAsync(key, count);
                ListValueCollectionView.ItemsSource = await RedisDatabase.ListGetAsync(SelectedKey.Key);
            }
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error while removing element", "OK");
        }
        finally
        {
            EditingListKeyGrid.RowDefinitions.ElementAt(1).Height = 0;
        }
    }






    private void AddElementToSetButtonClicked(object sender, EventArgs e)
    {
        GettingNewMemberForSetEntry.Text = null;

        EditingSetKeyGrid.RowDefinitions.ElementAt(1).Height = 0;

        EditingSetKeyGrid.RowDefinitions.ElementAt(0).Height = 100;
    }


    private async void ApplyAddingMemberToSetButtonClickedAsync(object sender, EventArgs e)
    {
        var key = SelectedKey.Key;
        try
        {
            var value = GettingNewMemberForSetEntry.Text;
            await RedisDatabase.SetAddAsync(key, value);
            SetValueCollectionView.ItemsSource = await RedisDatabase.SetGetAsync(SelectedKey.Key);
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error while adding member", "OK");
        }
        finally
        {
            EditingSetKeyGrid.RowDefinitions.ElementAt(0).Height = 0;
        }
    }




    private void RemoveElementFromSetButtonClicked(object sender, EventArgs e)
    {
        GettingRemovingMemberFromSetEntry.Text = null;

        EditingSetKeyGrid.RowDefinitions.ElementAt(0).Height = 0;

        EditingSetKeyGrid.RowDefinitions.ElementAt(1).Height = 100;
    }

    private async void ApplyRemovalMemberFromSetButtonClickedAsync(object sender, EventArgs e)
    {
        var key = SelectedKey.Key;
        try
        {
            var value = GettingRemovingMemberFromSetEntry.Text;
            await RedisDatabase.SetRemoveAsync(key, value);
            SetValueCollectionView.ItemsSource = await RedisDatabase.SetGetAsync(SelectedKey.Key);
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error while removing member", "OK");
        }
        finally
        {
            EditingSetKeyGrid.RowDefinitions.ElementAt(1).Height = 0;
        }
    }


    private void AddFieldToHashButtonClicked(object sender, EventArgs e)
    {
        GettingNewFieldForHashEntry.Text = null;
        GettingNewValueForHashEntry.Text = null;

        EditingHashKeyGrid.RowDefinitions.ElementAt(1).Height = 0;

        EditingHashKeyGrid.RowDefinitions.ElementAt(0).Height = 100;
    }

    private async void ApplyAddingFieldToHashButtonClickedAsync(object sender, EventArgs e)
    {
        var key = SelectedKey.Key;
        try
        {
            var field = GettingNewFieldForHashEntry.Text;
            var value = GettingNewValueForHashEntry.Text;
            HashEntry[] hashEntry = { new HashEntry(field, value) } ;
            await RedisDatabase.HashSetFieldAsync(key, hashEntry);
            HashValueCollectionView.ItemsSource = await RedisDatabase.HashGetAsync(SelectedKey.Key);
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error while adding filed", "OK");
        }
        finally
        {
            EditingHashKeyGrid.RowDefinitions.ElementAt(0).Height = 0;
        }
    }



    private void RemoveFieldFromHashButtonClicked(object sender, EventArgs e)
    {
        GettingRemovingFieldFromHashEntry.Text = null;

        EditingHashKeyGrid.RowDefinitions.ElementAt(0).Height = 0;

        EditingHashKeyGrid.RowDefinitions.ElementAt(1).Height = 100;
    }

    private async void ApplyRemovalFieldFromHashButtonClickedAsync(object sender, EventArgs e)
    {
        var key = SelectedKey.Key;
        try
        {
            var filed = GettingRemovingFieldFromHashEntry.Text;
            await RedisDatabase.HashRemoveFieldAsync(key, filed);
            HashValueCollectionView.ItemsSource = await RedisDatabase.HashGetAsync(SelectedKey.Key);
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error while adding filed", "OK");
        }
        finally
        {
            EditingHashKeyGrid.RowDefinitions.ElementAt(1).Height = 0;
        }
    }

    public void CloseAll()
    {
        EditingStringKeyGrid.RowDefinitions.ElementAt(0).Height = 0;

        EditingListKeyGrid.RowDefinitions.ElementAt(0).Height = 0;
        EditingListKeyGrid.RowDefinitions.ElementAt(1).Height = 0;

        EditingSetKeyGrid.RowDefinitions.ElementAt(0).Height = 0;
        EditingSetKeyGrid.RowDefinitions.ElementAt(1).Height = 0;

        EditingHashKeyGrid.RowDefinitions.ElementAt(0).Height = 0;
        EditingHashKeyGrid.RowDefinitions.ElementAt(1).Height = 0;
    }
}