using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using LoginViewSample.Core.Annotations;
using LoginViewSample.Core.Services;
using LoginViewSample.Core.ViewModels.AddImageViewModel;
using Xamarin.Forms;

namespace LoginViewSample.Core.ViewModels
{
    public class MainViewModel
    {
        private readonly INavigationService _navigationService;

        public MainViewModel([NotNull] INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            AddImageMain.AddToClothesDict();            AddImageMain.InitSmallDict();

            LogoutCommand = new Command(Logout);
            AddNewItemCommand = new Command(AddNewItem);
            ChangeLocationCommand = new Command(ChangeLocation);
            AddItemImageCommand = new Command(AddItemImage);
            ShowItemLocationCommand = new Command(ShowItemLocation);
        }


        private void Logout()
        {
            App.IsUserLoggedIn = false;
            _navigationService.NavigateModalAsync(PageNames.LoginPage);
        }

        public ICommand LogoutCommand { private set; get; }


        private void AddNewItem()
        {
            _navigationService.NavigateAsync(PageNames.ScannerPage, "Add");
        }

        public ICommand AddNewItemCommand { private set; get; }


        private void ChangeLocation()
        {
            _navigationService.NavigateAsync(PageNames.ScannerPage, "Change");
        }

        public ICommand ChangeLocationCommand { private set; get; }


        private void AddItemImage()
        {
            _navigationService.NavigateAsync(PageNames.ScannerPage, "Image");
        }

        public ICommand AddItemImageCommand { private set; get; }


        private void ShowItemLocation()
        {
            _navigationService.NavigateAsync(PageNames.ScannerPage, "Show");
        }

        public ICommand ShowItemLocationCommand { private set; get; }


    }
}
