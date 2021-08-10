using System.ComponentModel;
using System.Windows.Input;
using CustomerApp.Services;
using Xamarin.Forms;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Xamarin.CommunityToolkit.UI.Views.Options;

// based on https://mallibone.com/post/creating-a-login-screen-with-xamarinforms

namespace CustomerApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private bool _areCredentialsInvalid;
        public static HashSet<string> favoriteItems;
      

        public LoginViewModel(INavigationService navigationService)
        {
            AuthenticateCommand = new Command(() =>
            {
                AreCredentialsInvalid = !UserAuthenticated(Username, Password);
                if (AreCredentialsInvalid) return;

                App.IsUserLoggedIn = true;
                Username = Username.Trim();
                navigationService.NavigateModalAsync(PageNames.MainPage, Username);
            });

            AreCredentialsInvalid = false;
        }
     

        private bool UserAuthenticated(string username, string password)
        {
            if (string.IsNullOrEmpty(username)
                || string.IsNullOrEmpty(password))
            {
                return false;
            }
            username = username.ToLower(); // email address is not case sensitive
            string urlGetPassword = "https://customerlogin.azurewebsites.net/api/get-password/" + username;
        
            HttpClient client = new HttpClient();
            var response = client.GetAsync(urlGetPassword).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            LoginEntity entity = JsonConvert.DeserializeObject<LoginEntity>(result);

            if (entity == null)
            {
                return false;
            }

            if (entity.Password == password)
            {
                if (entity.Favorites != "NoItems") // user favorites is not empty
                {
                    favoriteItems = new HashSet<string>(entity.Favorites.Split('_').ToList());
                }

                else
                {
                    favoriteItems = new HashSet<string>();
                }
                
                return true;
            }
            return false;
        }


        public string Username
        {
            get => _username;
            set
            {
                if (value == _username) return;
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (value == _password) return;
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand AuthenticateCommand { get; set; }

        public bool AreCredentialsInvalid
        {
            get => _areCredentialsInvalid;
            set
            {
                if (value == _areCredentialsInvalid) return;
                _areCredentialsInvalid = value;
                OnPropertyChanged(nameof(AreCredentialsInvalid));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
