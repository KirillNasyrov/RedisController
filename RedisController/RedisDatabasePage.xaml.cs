using RedisController.Models;
using StackExchange.Redis;

namespace RedisController;

public partial class RedisDatabasePage : ContentPage
{
	public ConnectionService ConnectionService { get; private set; }
	public RedisDatabase DataBase { get; private set; }

	public string DataBaseName { get; private set; }

    public RedisDatabasePage(RedisDatabase dataBase, string dataBaseName)
	{
		DataBase = dataBase;
		DataBaseName = dataBaseName;
		InitializeComponent();

		BindingContext = this;
	}

	private async void BackButtonClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync(false);
	}
}