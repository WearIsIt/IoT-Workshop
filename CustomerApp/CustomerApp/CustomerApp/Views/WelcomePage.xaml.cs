using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using CustomerApp.WelcomePageDesign;
using Xamarin.Forms.Xaml;
using CustomerApp.ViewModels;

namespace CustomerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            BindingContext = new WelcomeViewModel(App.NavigationService);
            ScannerPage.popup = false;
            InitializeComponent();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(async () =>
            {
                await Animations.FadeAnimY(FaceButton);
                await Animations.FadeAnimY(LoginButton);
                await Animations.FadeAnimY(SignupButton);
            });
        }


        protected void Back(object s, EventArgs e)
        {
            Navigation.PopAsync();
        }


        protected void Login(object s, EventArgs e)
        {
        }


        protected override bool OnBackButtonPressed()
        {
            return true;
        }


    }
}