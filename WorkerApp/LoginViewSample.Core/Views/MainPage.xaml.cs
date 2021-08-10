using LoginViewSample.Core.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LoginViewSample.Core.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
		    BindingContext = ViewModel;
			ScannerPage.popup = false;
			InitializeComponent();
		}


	    public MainViewModel ViewModel { get; } = new MainViewModel(App.NavigationService);

		protected override bool OnBackButtonPressed() // Hardware back button is disabled
		{
			return true;
		}


		protected override async void OnAppearing()
	    {
	        base.OnAppearing();
        }


	}
}
