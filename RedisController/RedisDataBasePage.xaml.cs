using RedisController.Models;
using StackExchange.Redis;

namespace RedisController;

public partial class RedisDataBasePage : ContentPage
{
	public RedisDataBase DataBase { get; set; }

	public string DataBaseName { get; set; }

    public RedisDataBasePage(RedisDataBase dataBase, string dataBaseName)
	{
		DataBase = dataBase;
		DataBaseName = dataBaseName;
		InitializeComponent();

		BindingContext = this;
	}

	private async void backButtonClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync(false);
	}
}