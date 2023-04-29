using RedisController.Models;

namespace RedisController;

public partial class StartPage : ContentPage
{
	StorageOfRedisDataBases storageOfRedisDataBases;
	public StartPage()
	{
		storageOfRedisDataBases = new StorageOfRedisDataBases();

		InitializeComponent();
	}

}