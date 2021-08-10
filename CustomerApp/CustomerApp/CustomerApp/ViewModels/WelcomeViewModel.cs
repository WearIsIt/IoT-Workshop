using CustomerApp.Annotations;
using CustomerApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CustomerApp.ViewModels
{
    public class WelcomeViewModel
    {
        private readonly INavigationService _navigationService;

        public WelcomeViewModel([NotNull] INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            ContinueAsGuestCommand = new Command(ContinueAsGuest);
            LogInCommand = new Command(LogIn);
            SignUpCommand = new Command(SignUp);
        }

        private void LogIn()
        {
            _navigationService.NavigateAsync(PageNames.LoginPage);
        }

        public ICommand LogInCommand { private set; get; }


        private void ContinueAsGuest()
        {
            App.IsUserLoggedIn = false;
            _navigationService.NavigateAsync(PageNames.MainPage);
        }

        public ICommand ContinueAsGuestCommand { private set; get; }


        private void SignUp()
        {
           App.IsUserLoggedIn = false;
            _navigationService.NavigateAsync(PageNames.SignUpPage);
        }

        public ICommand SignUpCommand { private set; get; }


    }
}
