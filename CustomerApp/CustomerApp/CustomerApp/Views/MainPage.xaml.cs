using CustomerApp.TagSearch;
using CustomerApp.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomerApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
		public MainPage() // This constructor is called after continue as guest
		{
			TagDict.AddToClothesDict(); // Initialize tags dictionary
			ViewModel = new MainViewModel(App.NavigationService);
			BindingContext = ViewModel;
			ScannerPage.popup = false;
			InitializeComponent();

			if (App.IsUserLoggedIn == false) { // If customer is logged as guest, don't show logout button
				LogoutButton.IsVisible = false;
			}

			else //If customer is logged with username and password, don't show login and sign up buttons
			{
				LoginButton.IsVisible = false;
				SignUpButton.IsVisible = false;
			}

			isDisableBack = false; // Hardware back button in enabled
		}


		private bool isDisableBack { get; set; }


		public MainPage(string username) // This constructor is called after sign up or login
		{
			TagDict.AddToClothesDict(); // Initialize tags dictionary
			ViewModel= new MainViewModel(App.NavigationService, username);
			BindingContext = ViewModel;
			ScannerPage.popup = false;
			InitializeComponent();

			if (App.IsUserLoggedIn == false) // If customer is logged as guest, don't show logout button
			{ 
				LogoutButton.IsVisible = false;
			}

			else // If customer is logged with username and password, don't show login and sign up buttons
			{
				LoginButton.IsVisible = false;
				SignUpButton.IsVisible = false;
			}

			isDisableBack = true; // Hardware back button in disabled
		}


		public MainViewModel ViewModel { get; set; }

		protected override bool OnBackButtonPressed()
		{
			return isDisableBack;
		}


		protected override async void OnAppearing()
		{
			base.OnAppearing();
		}


        private void WishList_Clicked(object sender, EventArgs e)
        {
			if (App.IsUserLoggedIn == false)
			{
				DisplayAlert("Oops..", "You must be logged in to see your favorites items", "OK");
				return;
			}

			App.NavigationService.NavigateAsync(PageNames.WishListPage, "WishList");
		}


    }
}