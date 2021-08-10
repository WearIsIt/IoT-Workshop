using LoginViewSample.Core.Services;
using Xamarin.Forms;

namespace LoginViewSample.Core
{
	public partial class App : Application
	{
		public Page Scanner { get; set; }

		private static readonly FormsNavigationService _navigationService = new FormsNavigationService();

		public App()
		{
			InitializeComponent();

            _navigationService.Configure(PageNames.LoginPage, typeof(Views.LoginPage));
            _navigationService.Configure(PageNames.MainPage, typeof(Views.MainPage));
			_navigationService.Configure(PageNames.ScannerPage, typeof(Views.ScannerPage));
			_navigationService.Configure(PageNames.SelectLocationPage, typeof(Views.SelectLocationPage));
			_navigationService.Configure(PageNames.AddImagePage, typeof(Views.AddImagePage));
			_navigationService.Configure(PageNames.TagsPage, typeof(Views.TagsPage));
			_navigationService.Configure(PageNames.ItemLocationPage, typeof(Views.ItemLocationPage));
			
			MainPage = _navigationService.SetRootPage(nameof(Views.LoginPage));
		}

	    public static INavigationService NavigationService { get; } = _navigationService;

        // Use a service for providing this information
        public static bool IsUserLoggedIn { get; set; }

	    protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
