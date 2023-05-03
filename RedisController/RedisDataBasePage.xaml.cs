using StackExchange.Redis;

namespace RedisController;

public partial class RedisDataBasePage : ContentPage
{
	private IDatabase RedisDataBase { get; set; }
	public RedisDataBasePage(IDatabase database)
	{
		RedisDataBase = database;
		InitializeComponent();
	}
}