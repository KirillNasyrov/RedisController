namespace RedisController;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        const int newWidth = 1000;
        const int newHeight = 600;


        window.MinimumWidth = newWidth;
        window.MinimumHeight = newHeight;

        return window;
    }
}
