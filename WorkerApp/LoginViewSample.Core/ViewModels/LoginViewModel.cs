using System.ComponentModel;
using System.Windows.Input;
using LoginViewSample.Core.Services;
using Xamarin.Forms;
using System.Net.Http;
using System;
using Newtonsoft.Json;

// based on https://mallibone.com/post/creating-a-login-screen-with-xamarinforms

namespace LoginViewSample.Core.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private bool _areCredentialsInvalid;

        public LoginViewModel(INavigationService navigationService)
        {
            AuthenticateCommand = new Command(() =>
            {
                AreCredentialsInvalid = !UserAuthenticated(Username, Password);
                if (AreCredentialsInvalid) return;

                App.IsUserLoggedIn = true;
                navigationService.NavigateModalAsync(PageNames.MainPage);
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

            string urlGetPassword = "https://projectfunctionapps.azurewebsites.net/api/get-password/" + username;
            HttpClient client = new HttpClient();
            var response = client.GetAsync(urlGetPassword).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            LoginEntity entity = JsonConvert.DeserializeObject<LoginEntity>(result);

            if (entity==null)
            {
                return false;
            }
            return entity.Password==password;
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
                if(value == _areCredentialsInvalid) return;
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
