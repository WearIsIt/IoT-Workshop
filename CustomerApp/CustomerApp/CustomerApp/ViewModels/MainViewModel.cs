using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using CustomerApp.Annotations;
using CustomerApp.Services;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json;
using Xamarin.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace CustomerApp.ViewModels
{
    public class MainViewModel
    {
        private readonly INavigationService _navigationService;

        public MainViewModel([NotNull] INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            LogoutCommand = new Command(Logout);
            ShowItemLocationCommand = new Command(ShowItemLocation);
            SearchItemsCommand = new Command(SearchItems);

            SignUpCommand = new Command(SignUp);
            LoginCommand = new Command(Login);

            TopItemsCommand = new Command(TopItems);           
        }


        private string username;

        public MainViewModel([NotNull] INavigationService navigationService, string usernameParam)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            username = usernameParam;
            LogoutCommand = new Command(Logout);
            ShowItemLocationCommand = new Command(ShowItemLocation);
            SearchItemsCommand = new Command(SearchItems);

            SignUpCommand = new Command(SignUp);
            LoginCommand = new Command(Login);

            TopItemsCommand = new Command(TopItems);
        }


        private void Logout()
        {
            if (App.IsUserLoggedIn)
            {
                // call function app
                CreateFavoritesList();
            }
            App.IsUserLoggedIn = false;
            _navigationService.NavigateModalAsync(PageNames.WelcomePage);
        }

        public ICommand LogoutCommand { private set; get; }


        private void CreateFavoritesList()
        {
            string favString= "";
            foreach (string item in LoginViewModel.favoriteItems)
            {
                if (favString=="")
                {
                    favString += item;
                }

                else
                {
                    favString += "_" + item;
                }
            }
            UpdateFavoritesToCloud(favString);
        }


        private void UpdateFavoritesToCloud(string favoritesList)
        {
            if (favoritesList=="")
            {
                favoritesList = "NoItems";
            }
       
           string urlGetPassword = "https://customerlogin.azurewebsites.net/api/update-favorites/" + username + "/" + favoritesList;
            HttpClient client = new HttpClient();
            var response = client.GetAsync(urlGetPassword).Result;
        } 


        private void ShowItemLocation()
        {
            _navigationService.NavigateAsync(PageNames.ScannerPage, "Show");
        }

        public ICommand ShowItemLocationCommand { private set; get; }


        private void SearchItems()
        {
            _navigationService.NavigateAsync(PageNames.ChooseTagsPage, "TagsFilter");
        }

        public ICommand SearchItemsCommand { private set; get; }


        private void SignUp()
        {
            _navigationService.NavigateAsync(PageNames.SignUpPage);
        }

        public ICommand SignUpCommand { private set; get; }


        private void Login()
        {
            _navigationService.NavigateAsync(PageNames.LoginPage);
        }

        public ICommand LoginCommand { private set; get; }


        private void TopItems()
        {       
            _navigationService.NavigateAsyncWithLoad(PageNames.LeaderBoardPage, null, 400);
        }

        public ICommand TopItemsCommand { private set; get; }


    }
}
