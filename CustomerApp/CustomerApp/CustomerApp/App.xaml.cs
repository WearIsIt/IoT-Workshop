using CustomerApp.Services;
using Xamarin.Forms;

namespace CustomerApp
{
    public partial class App : Application
    {
        public Page Scanner { get; set; }

        private static readonly FormsNavigationService _navigationService = new FormsNavigationService();


        public App()
        {
            InitializeComponent();

            _navigationService.Configure(PageNames.WelcomePage, typeof(Views.WelcomePage));
            _navigationService.Configure(PageNames.LoginPage, typeof(Views.LoginPage));
            _navigationService.Configure(PageNames.MainPage, typeof(Views.MainPage));
            _navigationService.Configure(PageNames.ScannerPage, typeof(Views.ScannerPage));
            _navigationService.Configure(PageNames.ItemLocationPage, typeof(Views.ItemLocationPage));
            _navigationService.Configure(PageNames.ChooseTagsPage, typeof(Views.ChooseTagsPage));
            _navigationService.Configure(PageNames.SearchResultsPage, typeof(Views.SearchResultsPage));
            _navigationService.Configure(PageNames.WishListPage, typeof(Views.WishListPage));
            _navigationService.Configure(PageNames.SignUpPage, typeof(Views.SignUpPage));
            _navigationService.Configure(PageNames.LeaderBoardPage, typeof(Views.LeaderBoardPage));
            
            MainPage = _navigationService.SetRootPage(nameof(Views.WelcomePage));
        }


        public static INavigationService NavigationService { get; } = _navigationService;


        // Use a service for providing this information
        public static bool IsUserLoggedIn { get; set; }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }


    }
}
